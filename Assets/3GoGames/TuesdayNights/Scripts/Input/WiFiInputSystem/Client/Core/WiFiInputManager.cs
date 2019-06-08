using UnityEngine;

using System;

using WiFiInput.Common;

namespace WiFiInput.Client
{
    public class WiFiInputManager : MonoBehaviour
    {
        public int serverSocketPort = 2015;
        public int clientSocketPort = 2016;

        public bool clientConnectAutomatically = true;
        public CONTROLLERDATA_MAX_SEND_RATE clientMaxSendRate = CONTROLLERDATA_MAX_SEND_RATE.CapAt90Hz;

        [Tooltip("Amount of time in seconds that the controller checks for timeout. If the check fails twice in a row then the controller will disconnect. 0 is disabled")]
        [Range(0f, 100f)]
        public float heartbeatTimeout = 0f;

        public bool logVerbose = false;

        public string applicationName = "";

        bool transmittedInventory = false;

        int packetNumber = 0;
        int consecutiveAttempts = 0;
        float lastSendTime = 0f;
        float maxRate = 0f;

        private bool m_Running = false;

        public bool isRunning
        {
            get { return m_Running; }
        }

        // Use this for initialization

        public void Run()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            WiFiInputController.Run(applicationName, serverSocketPort, clientSocketPort, logVerbose, clientConnectAutomatically);

            lastSendTime = Time.realtimeSinceStartup;

            //set the rate
            switch (clientMaxSendRate)
            {
                case CONTROLLERDATA_MAX_SEND_RATE.CapAt30Hz:
                    maxRate = .033333333f;
                    break;
                case CONTROLLERDATA_MAX_SEND_RATE.CapAt60Hz:
                    maxRate = .016666666f;
                    break;
                case CONTROLLERDATA_MAX_SEND_RATE.CapAt90Hz:
                    maxRate = .011111111f;
                    break;
                case CONTROLLERDATA_MAX_SEND_RATE.SendAsFastAsPossible:
                    maxRate = 0f;
                    break;
                case CONTROLLERDATA_MAX_SEND_RATE.SendInfrequentlyOncePerSecond:
                    maxRate = 1f;
                    break;
                default:
                    maxRate = 0f;
                    break;
            }

            //There can be a timing issue between when the server has been discovered but we might not be ready yet
            //to transmit our inventory.  Becuase this script executes in start and registrations in awake all the local registrations have now take place
            WiFiInputController.readyToTransmitInventory = true;

            if (!transmittedInventory && (WiFiInputController.clientState != CURRENT_CLIENT_STATE.Broadcasting && WiFiInputController.clientState != CURRENT_CLIENT_STATE.NotConnected))
            {
                //depending on the order in which client and server are started check our state and send inventory immediately
                WiFiInputController.sendControllerInventory(WiFiInputController.createInventoryMessage());
                WiFiInputController.clientState = CURRENT_CLIENT_STATE.SendingInventory;
            }

            if (heartbeatTimeout > 0f)
            {
                InvokeRepeating("checkHeartbeatForTimeout", heartbeatTimeout, heartbeatTimeout);
            }

            m_Running = true;
        }

        void Awake()
        {
            WiFiInputController.Initialize();
        }

        void LateUpdate()
        {
            if (!m_Running)
                return;

            //we are the client we are responsible for triggering the send of Easy WiFi Controllers data structure over
            //the network and because of this we want to order this script to run after the others

            if (WiFiInputController.clientState == CURRENT_CLIENT_STATE.SendingControllerData)
            {
                //send the data
                if ((Time.realtimeSinceStartup - lastSendTime) > maxRate)
                {
                    lastSendTime = Time.realtimeSinceStartup;
                    WiFiInputController.sendWiFIControllerData(WiFiInputController.createControllerDataMessage(packetNumber));

                    //increment the packet number
                    if (packetNumber < WiFiInputConstants.ROLLOVER_PACKET_NUMBER)
                        packetNumber++;
                    else
                        packetNumber = 0;
                }
            }
        }

        void OnApplicationQuit()
        {
            if (!m_Running)
                return;

            WiFiInputController.endUDPClientAndThread();
        }

        void checkHeartbeatForTimeout()
        {
            if (!m_Running)
                return;

            if (WiFiInputController.clientState == CURRENT_CLIENT_STATE.SendingControllerData)
            {
                if ((DateTime.UtcNow - WiFiInputController.lastHeartbeatTime).TotalSeconds > Convert.ToDouble(heartbeatTimeout))
                {
                    consecutiveAttempts++;
                    //we have passed the heartbeat timeout disconnect and go back to discovery mode
                    if (consecutiveAttempts > 1)
                    {
                        //disconnect

                        if (logVerbose)
                        {
                            Debug.Log("Heartbeat timeout. Going Back to Discovery Mode");
                        }
                        WiFiInputController.clientState = CURRENT_CLIENT_STATE.NotConnected;
                        consecutiveAttempts = 0;

                        //take this time to set all the client side controls ready to reconnect
                        BaseControllerType temp;
                        if (WiFiInputController.controllerDataDictionary != null)
                        {
                            foreach (string key in WiFiInputController.controllerDataDictionary.Keys)
                            {
                                temp = WiFiInputController.controllerDataDictionary[key];
                                temp.justReconnected = true;
                            }
                        }

                        //go back to discovery mode and check for a new server
                        WiFiInputController.checkForServer();
                    }
                }
            }
        }
    }
}
