using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using WiFiInput.Common;

//General architecture of EasyWiFIController (discovery part) is as follows
//Client will broadcast it's address UDP broadcast in the discovery phase
//Server upon recieving broadcast will reply to client letting it to know to stop broadcasting and giving client a client number
//Client will then send back an inventory of its controls/backchannels
//Server will send back the server key for each of the controls/backchannels
//client will then start sending controller data at specified rate
//(optional) server will then start sending backchannel data at specified rate

//NOTE: Obviously it would be a more straightforward architecture if the server broadcast (not the client) but apparently
//      some mobile phones block receiving broadcasts (not all and there are ways around but doesn't seem to work reliably)
//      and mobile phones are likely to be the client

//This class contains all the sockets/communication details below is a message cheatsheet

//Client Broadcast ->   "Client_Broadcast:(ClientIP)"
//Server Broadcast Response -> "Broadcast_ServerResponse:(ServerIP)"
//Client Inventory -> "Client_Inventory:(ClientIP):controltype#controlname,controltype#controlname,controltype#controlname,...etc" (can be one or many)
//Server Inventory Response -> "Inventory_ServerResponse:serverkey#controlname,serverkey#controlname,serverkey#controlname,...etc" (one for each sent in inventory)
//Client Controller data (endlines in data stream) -> "Client_ControllerData:(packet#):
//(serverkey)#datapoint,datapoint,datapoint,...etc
//(serverkey)#datapoint,datapoint,datapoint,...etc
//(serverkey)#datapoint,datapoint,datapoint,...etc" (one for each client control and each control may have one or many datapoints)
//Server Backchannel data (endlines in data stream) -> "Server_BackchannelData:(packet#):
//(clientkey)#datapoint,datapoint, ...etc
//(clientkey)#datapoint,datapoint, ...etc (one for each server backchannel)
//Server Heartbeat -> "Server_Heartbeat:(packet#)"
//Client Log -> "Client_Log:(logmessage)"
//Client Disconnect -> "Client_Disconnect:(ClientIP)"

namespace WiFiInput.Client
{
    public static class WiFiInputController
    {
        public static string serverIPAddress;
        public static string myIPAddress;
        public static string appName;
        public static bool isVerbose;
        public static int serverSocketListenPort;
        public static int clientScoketListenPort;
        public static Thread listenThread;
        public static Thread broadcastThread;

        public static Dictionary<string, BaseControllerType> controllerDataDictionary;
        public static List<string> controllerDataDictionaryKeys = null;

        public static CURRENT_CLIENT_STATE clientState;
        public static bool readyToTransmitInventory; //this is different that the state of network traffic, this is if locally all client controls have registered
        public static bool readyToTransmitBackchannel;
        public static DateTime lastHeartbeatTime;
        public static UdpClient clientBroadcast;
        public static UdpClient clientSend;
        public static UdpClient clientListen;
        public static IPEndPoint clientBroadcastEndPoint;
        public static IPEndPoint clientSendEndPoint;
        public static IPEndPoint clientListenEndPoint;

        static WiFiInputController()
        {
            serverIPAddress = string.Empty;
            appName = string.Empty;
        }

        public static void Initialize()
        {
            controllerDataDictionary = new Dictionary<string, BaseControllerType>();
            controllerDataDictionaryKeys = new List<string>();
        }

        public static void Run(string name, int serverPort, int clientPort, bool verbose, bool clientConnectAutomatically)
        {
            appName = name;
            serverSocketListenPort = serverPort;
            clientScoketListenPort = clientPort;
            isVerbose = verbose;

            myIPAddress = "127.0.0.1";

            if (isVerbose)
            {
                Debug.Log("Initializing Easy WiFi Controller...");
            }

            //if we're initing with connect automatically this will be changed below when checking for server
            clientState = CURRENT_CLIENT_STATE.NotConnected;

            //setup for client broadcast (setup for client send to server happens later)
            clientBroadcast = new UdpClient()
            {
                EnableBroadcast = true
            };
            clientBroadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, serverSocketListenPort);

            //setup for client listening
            clientListen = new UdpClient()
            {
                EnableBroadcast = true
            };
            clientListen.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            clientListenEndPoint = new IPEndPoint(IPAddress.Any, clientScoketListenPort);
            clientListen.Client.Bind(clientListenEndPoint);
            listenThread = new Thread(new ThreadStart(listen))
            {
                IsBackground = true
            };
            listenThread.Start();

            if (clientConnectAutomatically)
            {
                checkForServer();
            }
        }

        //this method is called upon init of the client automatically (if set to) to look for the server
        //this can also be invoked in gamecode to essentially relook for server (if we want to reconnect or find a new one)
        public static void checkForServer()
        {
            //reset the connection variables
            clientState = CURRENT_CLIENT_STATE.Broadcasting;
            serverIPAddress = String.Empty;

            broadcastThread = new Thread(new ThreadStart(broadcastLoop))
            {
                IsBackground = true
            };
            broadcastThread.Start();
        }

        public static void broadcastLoop()
        {
            byte[] bytes;
            while (clientState != CURRENT_CLIENT_STATE.SendingControllerData)
            {
                //broadcast until we know who the server is
                if (isVerbose)
                {
                    sendClientLog(createClientLogMessage("Sending out client broadcast..."));
                }
                bytes = Encoding.UTF8.GetBytes(WiFiInputConstants.MESSAGETYPE_BROADCAST + WiFiInputConstants.SPLITMESSAGE_COLON + myIPAddress);
                try
                {
                    clientBroadcast.Send(bytes, (int)bytes.Length, clientBroadcastEndPoint);
                }
                catch (Exception ex)
                {
                    //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                    //log the error and move on (try again later)
                    Debug.Log(ex.Message);
                }
                Thread.Sleep(3000);
            }
        }

        public static void listen()
        {
            byte[] numArray;
            string message;

            while (true)
            {
                try
                {
                    numArray = WiFiInputController.clientListen.Receive(ref clientListenEndPoint);
                    message = Encoding.UTF8.GetString(numArray, 0, numArray.Length);

                    if (message.Contains(WiFiInputConstants.MESSAGETYPE_SERVER_RESPONSE_BROADCAST))
                    {
                        //server broadcast response message
                        listenServerBroadcastResponse(message);
                    }
                    else if (message.Contains(WiFiInputConstants.MESSAGETYPE_SERVER_RESPONSE_INVENTORY))
                    {
                        //server inventory response message
                        listenServerInventoryResponse(message);
                    }
                    else if (message.Contains(WiFiInputConstants.MESSAGETYPE_BACKCHANNEL_DATA))
                    {
                        // server backchannel message
                        // TODO: listenServerBackchannelmessage(message);
                    }
                    else if (message.Contains(WiFiInputConstants.MESSAGETYPE_HEARTBEAT))
                    {
                        //server heartbeat message
                        listenServerHeartbeatmessage(message);
                    }
                }
                catch (Exception exception)
                {
                    //closing a connection during a blocking receive will generate an exception
                    //we'll break out of the loop when we counter this exception
                    if (isVerbose)
                    {
                        Debug.Log("Normal socket listening interrupted to exit. Source:" + exception.Source);
                    }
                    break;
                }
            }
        }

        //used by the client to tell the server we are disconnecting
        public static void sendDisconnect(string message)
        {
            if (isVerbose)
            {
                sendClientLog(createClientLogMessage("Sending Disconnect message... " + message));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            if (clientState != CURRENT_CLIENT_STATE.Broadcasting && clientState != CURRENT_CLIENT_STATE.NotConnected)
            {
                try
                { 
                    //if we're broadcasting we don't yet know the server
                    clientSend.Send(bytes, (int)bytes.Length, clientSendEndPoint);
                }
                catch (Exception ex)
                {
                    //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                    //log the error and move on (try again later)
                    Debug.Log(ex.Message);
                }

                clientState = CURRENT_CLIENT_STATE.NotConnected;
            }
        }

        //used by the client to send controller data to the server
        public static void sendClientLog(string message)
        {
            Debug.Log(message);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            if (clientState != CURRENT_CLIENT_STATE.Broadcasting && clientState != CURRENT_CLIENT_STATE.NotConnected)
            {
                try
                { 
                    //if we're broadcasting we don't yet know the server
                    clientSend.Send(bytes, (int)bytes.Length, clientSendEndPoint);
                }
                catch (Exception ex)
                {
                    //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                    //log the error and move on (try again later)
                    Debug.Log(ex.Message);
                }
            }
        }

        //used by the client to send controller data to the server
        public static void sendWiFIControllerData(string message)
        {
            if (isVerbose)
            {
                sendClientLog(createClientLogMessage("Sending controller data... " + message));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            try
            { 
                clientSend.Send(bytes, (int)bytes.Length, clientSendEndPoint);
            }
            catch (Exception ex)
            {
                //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                //log the error and move on (try again later)
                Debug.Log(ex.Message);
            }
        }

        //used by the client to send controller data to the server
        public static void sendControllerInventory(string message)
        {
            if (isVerbose)
            {
                sendClientLog(createClientLogMessage("Sending out client Inventory..." + message));
            }
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            try
            { 
                clientSend.Send(bytes, (int)bytes.Length, clientSendEndPoint);
            }
            catch (Exception ex)
            {
                //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                //log the error and move on (try again later)
                Debug.Log(ex.Message);
            }
        }

        //used by the client listening to a server responding to a broadcast message
        public static void listenServerBroadcastResponse(string message)
        {
            if (clientState == CURRENT_CLIENT_STATE.Broadcasting)
            {
                string[] splitMessage;

                if (isVerbose)
                {
                    sendClientLog(createClientLogMessage("Recieved Server Broadcast Response... " + message));
                }
                splitMessage = message.Split(WiFiInputConstants.SPLITARRAY_COLON, StringSplitOptions.RemoveEmptyEntries);
                serverIPAddress = splitMessage[1];

                //we've received the server response so add endpoint to send directly to server
                clientSend = new UdpClient()
                {
                    EnableBroadcast = true
                };
                clientSendEndPoint = new IPEndPoint(IPAddress.Parse(serverIPAddress), serverSocketListenPort);

                clientState = CURRENT_CLIENT_STATE.ServerFound;
                if (readyToTransmitInventory)
                {
                    //if we aren't ready the wifi manager will send it when ready
                    sendControllerInventory(createInventoryMessage());
                }
            }
        }

        //used by the client listening to a server responding to a inventory message
        public static void listenServerInventoryResponse(string message)
        {
            string[] splitMessage, splitMessage2;

            if (isVerbose)
            {
                sendClientLog(createClientLogMessage("Received Server's Inventory Response... " + message));
            }
            splitMessage = message.Split(WiFiInputConstants.SPLITARRAY_COLON, StringSplitOptions.RemoveEmptyEntries);
            splitMessage = splitMessage[1].Split(WiFiInputConstants.SPLITARRAY_COMMA, StringSplitOptions.RemoveEmptyEntries);

            //map the server keys to the data structure
            for (int i = 0; i < splitMessage.Length; i++)
            {
                splitMessage2 = splitMessage[i].Split(WiFiInputConstants.SPLITARRAY_POUND, StringSplitOptions.RemoveEmptyEntries);
                controllerDataDictionary[splitMessage2[1]].serverKey = splitMessage2[0];
            }

            //we now have the server keys so set the flag to start the controller data stream
            clientState = CURRENT_CLIENT_STATE.SendingControllerData;
        }

        //used by the client listening to a server heartbeat message
        public static void listenServerHeartbeatmessage(string message)
        {
            if (isVerbose)
            {
                sendClientLog(createClientLogMessage("Recieved Server's Heartbeat message... " + message));
            }

            //register the current time of recieving heartbeat
            lastHeartbeatTime = DateTime.UtcNow;
        }

        public static string createInventoryMessage()
        {
            List<string> keys = controllerDataDictionaryKeys;
            BaseControllerType temp;

            string message = WiFiInputConstants.MESSAGETYPE_CLIENT_INVENTORY + WiFiInputConstants.SPLITMESSAGE_COLON + myIPAddress + WiFiInputConstants.SPLITMESSAGE_COLON;

            for (int keyIndex = 0; keyIndex < keys.Count; ++keyIndex)
            {
                string key = keys[keyIndex];

                temp = controllerDataDictionary[key];

                message += temp.controllerType + WiFiInputConstants.SPLITMESSAGE_POUND + temp.clientKey + WiFiInputConstants.SPLITMESSAGE_COMMA;
            }

            return message;
        }

        public static string createControllerDataMessage(int packetNumber)
        {
            List<string> keys = controllerDataDictionaryKeys;
            BaseControllerType temp;
            
            string message = WiFiInputConstants.MESSAGETYPE_CONTROLLER_DATA + WiFiInputConstants.SPLITMESSAGE_COLON + packetNumber.ToString() + WiFiInputConstants.SPLITMESSAGE_COLON;

            for (int keyIndex = 0; keyIndex < keys.Count; ++keyIndex)
            {
                string key = keys[keyIndex];

                temp = controllerDataDictionary[key];

                if (!temp.controllerType.Contains(WiFiInputConstants.BACKCHANNEL_FILTER))
                {
                    message += temp.serverKey + WiFiInputConstants.SPLITMESSAGE_POUND + temp.mapStructureToNetworkData();
                }
            }

            return message;
        }

        public static string createClientLogMessage(string logMessage)
        {
            Debug.Log(logMessage);
            string message = WiFiInputConstants.MESSAGETYPE_CLIENTLOG + WiFiInputConstants.SPLITMESSAGE_COLON + logMessage;
            return message;
        }

        public static string createDisconnectMessage()
        {
            string message = WiFiInputConstants.MESSAGETYPE_DISCONNECT + WiFiInputConstants.SPLITMESSAGE_COLON + myIPAddress;
            return message;
        }

        //this method is responsible for registering a control which will instance a specific controller type
        //add it to the data structure and set its key and return it
        //There are two ways this method is called, one on the client the UI script in its startup registers it client side
        //The second way is the server when a client sends over its inventory
        public static string registerControl(string type,string name, string IP="0")
        {
            if (controllerDataDictionary == null)
                return "";

            BaseControllerType controller;
            string key = "";

            switch (type)
            {
                case WiFiInputConstants.CONTROLLERTYPE_AXIS:
                    controller = new AxisControllerType();
                    controller.controllerType = WiFiInputConstants.CONTROLLERTYPE_AXIS;
                    break;
                case WiFiInputConstants.CONTROLLERTYPE_BUTTON:
                    controller = new ButtonControllerType();
                    controller.controllerType = WiFiInputConstants.CONTROLLERTYPE_BUTTON;
                    break;
                default:
                    //should not occur
                    controller = null;
                    Debug.Log("Error: a controller type that isn't defined was registered");
                    break;
            }


            //on the client we care what both the client and server key is but only client is known at registration time
            //the server key is discovered when the server responds to the inventory and will be populated then
            controller.clientKey = name;
            key = controller.clientKey;
                
            if (!controllerDataDictionary.ContainsKey(key))
            {
                //brand new first time here
                controllerDataDictionary.Add(key, controller);
                controllerDataDictionaryKeys.Add(key);
            }
            else if (controllerDataDictionary[key].logicalPlayerNumber == WiFiInputConstants.PLAYERNUMBER_DISCONNECTED)
            {
                //already here just reassign (original disconnected and new one is here)
                //we will only change the player number if it's been occupied in between
                controllerDataDictionary[key].lastReceivedPacketTime = DateTime.UtcNow;
                controllerDataDictionary[key].justReconnected = true;
                if (!WiFiInputUtilities.isPlayerNumberOccupied(controllerDataDictionary[key].previousConnectionPlayerNumber,controllerDataDictionary[key].clientKey))
                {
                    //not occupied assign it's previous number 
                    controllerDataDictionary[key].logicalPlayerNumber = controllerDataDictionary[key].previousConnectionPlayerNumber;
                }
                else
                {
                    //occupied give it the lowest available number
                    controllerDataDictionary[key].logicalPlayerNumber = controller.logicalPlayerNumber;
                }
            }

            return key;
        }

        //this function is responsible for looping through all the keys and giving out the NEW player number
        //remember this instance has not been added yet
        public static int getNewPlayerNumber(string controlName)
        {
            int player = WiFiInputUtilities.getHighestPlayerNumber() + 1;
            int returnIndex = 0;
            bool foundControl = false;

            for (int i = 0; i <= player; i++)
            {
                foundControl = WiFiInputUtilities.isPlayerNumberOccupied(i, controlName);

                if (foundControl == false)
                {
                    //we found no control with this player number (lowest yet so return it)
                    returnIndex = i;
                    break;
                }
            }

            return returnIndex;
        }

        //not all platforms have unity network in them (WP8, windows store, etc)
        //don't use this method on mono platforms though it doesn't seem to work
        public static string getNetworkIPAddress()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        //closes out all the UDPClients and Threads
        public static void endUDPClientAndThread()
        {
            //close connections
            if (clientListen != null)
            {
                clientListen.Close();
                clientListen = null;
            }
            if (clientBroadcast != null)
            {
                clientBroadcast.Close();
                clientBroadcast = null;
            }

            if (clientSend != null)
            {
                clientSend.Close();
                clientSend = null;
            }

            //abort threads
            if (listenThread != null)
            {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
                listenThread.Interrupt();
#else
                listenThread.Abort();//not support in il2cpp
#endif
                listenThread = null;
            }
            if (broadcastThread != null)
            {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_TVOS)
                broadcastThread.Interrupt();
#else
                broadcastThread.Abort();//not support in il2cpp
#endif
                broadcastThread = null;
            }
        }
    }
}