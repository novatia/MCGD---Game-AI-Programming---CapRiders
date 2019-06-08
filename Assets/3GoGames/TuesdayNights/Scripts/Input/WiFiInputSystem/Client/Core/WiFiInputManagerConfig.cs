using UnityEngine;

using FullInspector;

namespace WiFiInput.Client
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
        private float m_HeartbeatTimeout = 3f;

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

        public float heartbeatTimeout
        {
            get { return m_HeartbeatTimeout; }
        }

        public bool logVerbose
        {
            get { return m_LogVerbose; }
        }
    }
}
