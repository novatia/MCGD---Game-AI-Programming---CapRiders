using UnityEngine;

using System;

using WiFiInput.Common;

namespace WiFiInput.Server
{
    public class WiFiInputManager : MonoBehaviour
    {
        public int serverSocketPort = 2015;
        public int clientSocketPort = 2016;

        [Tooltip("Amount of time in seconds that if the server hasn't recieved any data from a particular client will consider it a timeout and mark it disconnected. 0 is disabled")]
        [Range(0f, 100f)]
        public float clientTimeout = 0f;

        [Tooltip("Only set true if you have some data that you're communicating back from your game to your phone controller")]
        public bool serverSendBackchannel = false;

        [Tooltip("Rate at which you want your server to send heartbeats back to your controller in seconds. 0 is disabled")]
        [Range(0f, 100f)]
        public float serverSendHeartbeatRate = 0f;

        public bool clientConnectAutomatically = true;
        public CONTROLLERDATA_MAX_SEND_RATE clientMaxSendRate = CONTROLLERDATA_MAX_SEND_RATE.CapAt90Hz;

        [Tooltip("Amount of time in seconds that the controller checks for timeout. If the check fails twice in a row then the controller will disconnect. 0 is disabled")]
        [Range(0f, 100f)]
        public float heartbeatTimeout = 0f;

        public bool logVerbose = false;

        public string applicationName = "";

        int heartbeatPacketNumber = 0;

        // LOGIC

        public void Initialize()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            WiFiInputController.initialize(applicationName, serverSocketPort, clientSocketPort, logVerbose, clientConnectAutomatically);

            if (serverSendHeartbeatRate > 0f)
            {
                InvokeRepeating("SendServerHeartbeat", serverSendHeartbeatRate, serverSendHeartbeatRate);
            }
            if (clientTimeout > 0f)
            {
                InvokeRepeating("CheckForClientTimeout", clientTimeout, clientTimeout);
            }
        }

        // MonoBehaviour's interface

        void OnApplicationQuit()
        {
            WiFiInputController.endUDPClientAndThread();
        }

        // INTERNALS

        private void SendServerHeartbeat()
        {
            WiFiInputController.createAndSendHeartbeatMessages(heartbeatPacketNumber);

            //increment the packet number
            if (heartbeatPacketNumber < WiFiInputConstants.ROLLOVER_PACKET_NUMBER)
                heartbeatPacketNumber++;
            else
                heartbeatPacketNumber = 0;
        }

        private void CheckForClientTimeout()
        {
            //check for any clients that may have timed out (if we just had a reconnection skip this time around
            if (!(WiFiInputController.isConnect == true
            && (DateTime.UtcNow - WiFiInputController.lastCallbackTime).TotalSeconds < clientTimeout
            && (DateTime.UtcNow - WiFiInputController.lastCallbackTime).TotalSeconds > 0d))
            {
                WiFiInputUtilities.CheckForClientTimeout(clientTimeout);
            }
        }
    }
}
