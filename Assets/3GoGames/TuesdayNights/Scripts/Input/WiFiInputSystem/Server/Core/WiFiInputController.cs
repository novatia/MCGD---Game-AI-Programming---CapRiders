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

namespace WiFiInput.Server
{
    public delegate void controllerConnectionsChangedHandler(bool isConnect, int playerNumber);

    public delegate void OnControllerRegisteredCallback(string key);

    public static class WiFiInputController
    {
        public static string myIPAddress;
        public static string appName;
        public static bool isVerbose;
        public static int serverSocketListenPort;
        public static int clientScoketListenPort;
        public static Thread listenThread;
        public static Thread broadcastThread;

        public static event controllerConnectionsChangedHandler On_ConnectionsChanged;
        public static event OnControllerRegisteredCallback OnControllerRegisteredEvent;

        public static Dictionary<string, BaseControllerType> controllerDataDictionary = null;
        public static List<string> controllerDataDictionaryKeys = null;

        public static CURRENT_SERVER_STATE serverState;
        public static UdpClient serverListen;
        public static Dictionary<string, UdpClient> serverSendDictionary;
        public static IPEndPoint serverListenEndPoint;
        public static Dictionary<string, IPEndPoint> serverSendEndPointDictionary;
        public static List<string> serverSendKeys;
        public static int lastConnectedPlayerNumber = WiFiInputConstants.PLAYERNUMBER_DISCONNECTED;
        public static bool isConnect = false;
        public static bool isNew = false;
        public static DateTime lastCallbackTime;

        static WiFiInputController()
        {
            appName = string.Empty;
        }

        public static void initialize(string name, int serverPort,  int clientPort, bool verbose, bool clientConnectAutomatically)
        {
            appName = name;
            serverSocketListenPort = serverPort;
            clientScoketListenPort = clientPort;
            isVerbose = verbose;

            myIPAddress = "127.0.0.1";

            controllerDataDictionary = new Dictionary<string, BaseControllerType>();
            controllerDataDictionaryKeys = new List<string>();

            serverSendKeys = new List<string>();

            if (isVerbose)
            {
                Debug.Log("Initializing Easy WiFi Controller...");
            }

            serverSendDictionary = new Dictionary<string, UdpClient>();
            serverSendEndPointDictionary = new Dictionary<string, IPEndPoint>();

            //setup for server listening (setup for server send happens later)
            serverListen = new UdpClient()
            {
                EnableBroadcast = true
            };
            serverListen.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            serverListenEndPoint = new IPEndPoint(IPAddress.Any, serverSocketListenPort);
            serverListen.Client.Bind(serverListenEndPoint);
            listenThread = new Thread(new ThreadStart(listen))
            {
                IsBackground = true
            };
            listenThread.Start();

            serverState = CURRENT_SERVER_STATE.Listening;
        }

        public static void listen()
        {
            byte[] numArray;
            string message;

            if (isVerbose)
            {
                Debug.Log("listening for communication...");
            }

            while (true)
            {
                try
                {
                    numArray = WiFiInputController.serverListen.Receive(ref serverListenEndPoint);
                    message = Encoding.UTF8.GetString(numArray, 0, numArray.Length);

                    if (message.Contains(WiFiInputConstants.MESSAGETYPE_CLIENTLOG))
                    {
                        //client log message
                        listenClientLogMessage(message);
                    }
                    else if (message.Contains(WiFiInputConstants.MESSAGETYPE_BROADCAST))
                    {
                        //client broadcast message
                        listenClientBroadcast(message);
                    }
                    else if (message.Contains(WiFiInputConstants.MESSAGETYPE_CLIENT_INVENTORY))
                    {
                        //client inventory message
                        listenClientInventoryMessage(message);
                    }
                    else if (message.Contains(WiFiInputConstants.MESSAGETYPE_CONTROLLER_DATA))
                    {
                        //client controller data
                        listenClientControllerData(message);
                    }
                    else if (message.Contains(WiFiInputConstants.MESSAGETYPE_DISCONNECT))
                    {
                        listenClientDisconnect(message);
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

        //used by the server to send our backchannel data
        public static void sendWiFIBackchannelData(string message, string clientIP)
        {
            if (isVerbose)
            {
                Debug.Log("Sending out server backchannel data... " + message);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            if (serverState != CURRENT_SERVER_STATE.Disconnecting)
            {
                try
                { 
                    serverSendDictionary[clientIP].Send(bytes, (int)bytes.Length, serverSendEndPointDictionary[clientIP]);
                }
                catch (Exception ex)
                {
                    //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                    //log the error and move on (try again later)
                    Debug.Log(ex.Message);
                }
            }
        }

        //used by the server to send our heartbeat
        public static void sendHeartbeat(string message, string clientIP)
        {
            if (isVerbose)
            {
                Debug.Log("Sending out server heartbeat... " + message);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            if (serverState != CURRENT_SERVER_STATE.Disconnecting)
            {
                try
                { 
                    serverSendDictionary[clientIP].Send(bytes, (int)bytes.Length, serverSendEndPointDictionary[clientIP]);
                }
                catch (Exception ex)
                {
                    //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                    //log the error and move on (try again later)
                    Debug.Log(ex.Message);
                }
            }
        }

        //used by the server in response to a client's broadcast
        public static void sendServerBroadcastResponse(string message, string clientIP)
        {
            if (isVerbose)
            {
                Debug.Log("Sending out server broadcast response... " + message);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            if (serverState != CURRENT_SERVER_STATE.Disconnecting)
            {
                try
                { 
                    serverSendDictionary[clientIP].Send(bytes, (int)bytes.Length, serverSendEndPointDictionary[clientIP]);
                }
                catch (Exception ex)
                {
                    //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                    //log the error and move on (try again later)
                    Debug.Log(ex.Message);
                }
            }
        }

        //used by the server in response to a client's inventory
        public static void sendServerInventoryResponse(string message, string clientIP)
        {
            if (isVerbose)
            {
                Debug.Log("Sending out server inventory Reply... " + message);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            if (serverState != CURRENT_SERVER_STATE.Disconnecting)
            {
                try
                { 
                    serverSendDictionary[clientIP].Send(bytes, (int)bytes.Length, serverSendEndPointDictionary[clientIP]);
                }
                catch (Exception ex)
                {
                    //the most common exception here is if you forgot to turn on your wifi (network unreachable)
                    //log the error and move on (try again later)
                    Debug.Log(ex.Message);
                }
            }
        }

        //used by the server listening to a client broadcast message
        public static void listenClientBroadcast(string message)
        {
            string[] splitMessage;
            UdpClient newClient;
            IPEndPoint newIPEndPoint;

            if (isVerbose)
            {
                Debug.Log("Recieved Client Broadcast... " + message);
            }
            splitMessage = message.Split(WiFiInputConstants.SPLITARRAY_COLON, StringSplitOptions.RemoveEmptyEntries);
            string newClientIPAddress = splitMessage[1];

            if (serverState != CURRENT_SERVER_STATE.Disconnecting)
            {

                if (!serverSendDictionary.ContainsKey(newClientIPAddress))
                {
                    //we've received a client broadcast so add it and send a response
                    newClient = new UdpClient()
                    {
                        EnableBroadcast = true
                    };
                    serverSendDictionary.Add(newClientIPAddress, newClient);
                    serverSendKeys.Add(newClientIPAddress);
                    newIPEndPoint = new IPEndPoint(IPAddress.Parse(newClientIPAddress), clientScoketListenPort);
                    serverSendEndPointDictionary.Add(newClientIPAddress, newIPEndPoint);
                }
                sendServerBroadcastResponse(WiFiInputConstants.MESSAGETYPE_SERVER_RESPONSE_BROADCAST + WiFiInputConstants.SPLITMESSAGE_COLON + myIPAddress, newClientIPAddress);
            }
        }

        //used by the server listening to a client inventory message
        public static void listenClientInventoryMessage(string message)
        {
            string[] splitMessage, splitMessage2;
            string clientIP;
            List<string> responseKeys = new List<string>();;
            string currentServerKey = null;

            if (isVerbose)
            {
                Debug.Log("Recieved Client Inventory... " + message);
            }

            splitMessage = message.Split(WiFiInputConstants.SPLITARRAY_COLON, StringSplitOptions.RemoveEmptyEntries);

            clientIP = splitMessage[1];

            splitMessage = splitMessage[2].Split(WiFiInputConstants.SPLITARRAY_COMMA, StringSplitOptions.RemoveEmptyEntries);
          
            //we've received a client inventory so loop and register each control
            for (int i = 0; i < splitMessage.Length; i++)
            {
                splitMessage2 = splitMessage[i].Split(WiFiInputConstants.SPLITARRAY_POUND, StringSplitOptions.RemoveEmptyEntries);
                currentServerKey = registerControl(splitMessage2[0], splitMessage2[1],clientIP);
                responseKeys.Add(currentServerKey+ WiFiInputConstants.SPLITMESSAGE_POUND +splitMessage2[1] + WiFiInputConstants.SPLITMESSAGE_COMMA);
            }

            //notify all the controls that a connection has changed
            if (isNew)
            {
                //reset the flag to false now
                isNew = false;

                //since this is a new connection (which can mean either brand new or previously disconnected)
                //send a callback
                lastCallbackTime = DateTime.UtcNow;
                isConnect = true;
                lastConnectedPlayerNumber = controllerDataDictionary[currentServerKey].logicalPlayerNumber;
                On_ConnectionsChanged(isConnect, lastConnectedPlayerNumber);
            }

            //now we need to tell the client the server key for each inventory item
            sendServerInventoryResponse(createInventoryResponseMessage(responseKeys), clientIP);
        }

        //used by the server listening to a client controller data message
        public static void listenClientControllerData(string message)
        {
            string[] splitMessage, splitMessage2;
            string packetNumber;
            string currentServerKey;

            if (isVerbose)
            {
                Debug.Log("Recieved Controller Data... " + message);
            }
            splitMessage = message.Split(WiFiInputConstants.SPLITARRAY_COLON, StringSplitOptions.RemoveEmptyEntries);
            packetNumber = splitMessage[1];
            splitMessage = splitMessage[2].Split(WiFiInputConstants.SPLITARRAY_NEWLINE, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < splitMessage.Length; i++)
            {
                //here we have each line (including the server key)
                splitMessage2 = splitMessage[i].Split(WiFiInputConstants.SPLITARRAY_POUND, StringSplitOptions.RemoveEmptyEntries);
                currentServerKey = splitMessage2[0];

                //pass the rest of the line (without server key) into the mapping function
                if (splitMessage2.Length > 1 && serverState != CURRENT_SERVER_STATE.Disconnecting && controllerDataDictionary.ContainsKey(currentServerKey))
                {
                    controllerDataDictionary[currentServerKey].mapNetworkDataToStructure(Convert.ToInt32(packetNumber), splitMessage2[1]);
                }
            }
        }

        //used by the server listening to a client log message
        public static void listenClientLogMessage(string message)
        {
            if (isVerbose)
            {
                Debug.Log("Client log: " + message);
            }
        }

        //used by the server listening to a client disconnect message
        public static void listenClientDisconnect(string message)
        {
            string[] splitMessage;
            BaseControllerType temp = null;

            if (isVerbose)
            {
                Debug.Log("Recieved Client Disconnect... " + message);
            }
            splitMessage = message.Split(WiFiInputConstants.SPLITARRAY_COLON, StringSplitOptions.RemoveEmptyEntries);
            string clientIPAddress = splitMessage[1];

            if (serverState != CURRENT_SERVER_STATE.Disconnecting)
            {
                //loop on the main collection but all have the same keys
                for (int keyIndex = 0; keyIndex < controllerDataDictionaryKeys.Count; ++keyIndex)
                {
                    string key = controllerDataDictionaryKeys[keyIndex];
                    if (key.Contains(clientIPAddress))
                    {
                        //this will in effect notify the scripts watching this data
                        temp = controllerDataDictionary[key];
                        temp.previousConnectionPlayerNumber = temp.logicalPlayerNumber;
                        temp.logicalPlayerNumber = WiFiInputConstants.PLAYERNUMBER_DISCONNECTED;
                    }
                }

                //notify all the controls that a connection has changed
                isConnect = false;
                if (temp != null)
                    On_ConnectionsChanged(false,temp.previousConnectionPlayerNumber);
                
            }
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

        public static string createInventoryResponseMessage(List<string> serverkeys)
        {
            string message = WiFiInputConstants.MESSAGETYPE_SERVER_RESPONSE_INVENTORY + WiFiInputConstants.SPLITMESSAGE_COLON;

            foreach (string serverkey in serverkeys)
            {
                //remember pound is already in message
                message += serverkey;
            }

            return message;
        }

        public static void createAndSendHeartbeatMessages(int packetNumber)
        {
            BaseControllerType temp;

            //loop through each client
            foreach (string client in serverSendKeys)
            {
                bool send = true;
                string message = WiFiInputConstants.MESSAGETYPE_HEARTBEAT + WiFiInputConstants.SPLITMESSAGE_COLON + packetNumber.ToString();

                for (int keyIndex = 0; keyIndex < controllerDataDictionaryKeys.Count; ++keyIndex)
                {
                    string key = controllerDataDictionaryKeys[keyIndex];

                    temp = controllerDataDictionary[key];

                    if (temp.clientIP.Equals(client) && temp.logicalPlayerNumber == WiFiInputConstants.PLAYERNUMBER_DISCONNECTED)
                    {
                        //we've found that this is actually disconnected
                        send = false;
                        break;
                    }
                }

                //we don't want to send heartbeat to those marked as disconnected
                if (send)
                {
                    WiFiInputController.sendHeartbeat(message, client);
                }
            }
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
        public static string registerControl(string type, string name, string IP = "0")
        {
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

            controller.serverKey = IP + name;
            controller.clientKey = name;
            controller.clientIP = IP;
            controller.logicalPlayerNumber = getNewPlayerNumber(name);
            controller.lastReceivedPacketTime = DateTime.UtcNow;
            lastConnectedPlayerNumber = controller.logicalPlayerNumber;
            isConnect = true;
            key = controller.serverKey;

            if (!controllerDataDictionary.ContainsKey(key))
            {
                //brand new first time here
                controllerDataDictionary.Add(key, controller);
                controllerDataDictionaryKeys.Add(key);
                isNew = true;
            }
            else if (controllerDataDictionary[key].logicalPlayerNumber == WiFiInputConstants.PLAYERNUMBER_DISCONNECTED)
            {
                //already here just reassign (original disconnected and new one is here)
                //we will only change the player number if it's been occupied in between
                controllerDataDictionary[key].lastReceivedPacketTime = DateTime.UtcNow;
                controllerDataDictionary[key].justReconnected = true;
                if (!WiFiInputUtilities.isPlayerNumberOccupied(controllerDataDictionary[key].previousConnectionPlayerNumber, controllerDataDictionary[key].clientKey))
                {
                    //not occupied assign it's previous number 
                    controllerDataDictionary[key].logicalPlayerNumber = controllerDataDictionary[key].previousConnectionPlayerNumber;
                }
                else
                {
                    //occupied give it the lowest available number
                    controllerDataDictionary[key].logicalPlayerNumber = controller.logicalPlayerNumber;
                }
                isNew = true;
            }
            else
            {
                //was already here and isn't currently marked as disconnected (most likely a delayed packet)
                isNew = false;
            }

            if (isNew)
            {
                if (OnControllerRegisteredEvent != null)
                {
                    OnControllerRegisteredEvent(key);
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

        //forces the connection callback to be called
        public static void forceConnectionRefresh()
        {
            lastCallbackTime = DateTime.UtcNow;
            On_ConnectionsChanged(isConnect, lastConnectedPlayerNumber);
        }

        //closes out all the UDPClients and Threads
        public static void endUDPClientAndThread()
        {
            if (serverListen != null)
            {
                serverListen.Close();
                serverListen = null;
            }

            if (serverSendKeys != null)
            {
                foreach (string key in serverSendKeys)
                {
                    if (serverSendDictionary[key] != null)
                    {
                        serverSendDictionary[key].Close();
                        serverSendDictionary[key] = null;
                    }
                }
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
