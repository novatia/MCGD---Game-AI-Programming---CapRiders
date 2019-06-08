using UnityEngine;

using FullInspector;

namespace WiFiInput.Server
{
    public class WiFiInputManagerConfig : BaseScriptableObject
    {
        [SerializeField]
        private string m_ApplicationName = "";

        [SerializeField]
        private int m_ServerSocketPort = 2015;
        [SerializeField]
        private int m_ClientSocketPort = 2016;

        [SerializeField]
        private float m_ClientTimeout = 3f;

        [SerializeField]
        private bool m_ServerSendBackchannel = false;

        [SerializeField]
        private float m_ServerSendHeartbeatRate = 0.5f;

        [SerializeField]
        private bool m_LogVerbose = false;

        public string applicationName
        {
            get { return m_ApplicationName; }
        }

        public int serverSocketPort
        {
            get { return m_ServerSocketPort; }
        }

        public int clientSocketPort
        {
            get { return m_ClientSocketPort; }
        }

        public float clientTimeout
        {
            get { return m_ClientTimeout; }
        }

        public bool serverSendBackchannel
        {
            get { return m_ServerSendBackchannel; }
        }

        public float serverSendHeartbeatRate
        {
            get { return m_ServerSendHeartbeatRate; }
        }

        public bool logVerbose
        {
            get { return m_LogVerbose; }
        }
    }
}
