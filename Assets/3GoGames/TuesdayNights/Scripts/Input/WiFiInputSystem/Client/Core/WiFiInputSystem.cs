using UnityEngine;

namespace WiFiInput.Client
{
    public class WiFiInputSystem : Singleton<WiFiInputSystem>
    {
        private WiFiInputManager m_InputManager = null;

        // STATIC

        public static bool isRunningMain
        {
            get
            {
                if (Instance != null)
                {
                    return Instance.isRunning;
                }

                return false;
            }
        }

        public static void InitializeMain()
        {
            if (Instance != null)
            {
                Instance.Initialize();
            }
        }

        public static void RunMain()
        {
            if (Instance != null)
            {
                Instance.Run();
            }
        }

        // LOGIC

        public bool isRunning
        {
            get
            {
                if (m_InputManager != null)
                {
                    return m_InputManager.isRunning;
                }

                return false;
            }
        }

        public void Initialize()
        {
            CreateInputManager();
        }

        public void Run()
        {
            if (m_InputManager != null)
            {
                m_InputManager.Run();
            }
        }

        // INTERNALS

        private void CreateInputManager()
        {
            m_InputManager = gameObject.AddComponent<WiFiInputManager>();

            WiFiInputManagerConfig config = Resources.Load<WiFiInputManagerConfig>("Input/WiFi/Client_WiFiInputManagerConfig");
            if (config != null)
            {
                m_InputManager.serverSocketPort = config.serverSocketPort;
                m_InputManager.clientSocketPort = config.clientSocketPort;
                m_InputManager.heartbeatTimeout = config.heartbeatTimeout;
                m_InputManager.logVerbose = config.logVerbose;

                m_InputManager.applicationName = config.applicationName;
            }
            else
            {
                m_InputManager.serverSocketPort = 2015;
                m_InputManager.clientSocketPort = 2016;
                m_InputManager.heartbeatTimeout = 3f;
                m_InputManager.logVerbose = false;

                m_InputManager.applicationName = "Default";
            }
        }
    }
}
