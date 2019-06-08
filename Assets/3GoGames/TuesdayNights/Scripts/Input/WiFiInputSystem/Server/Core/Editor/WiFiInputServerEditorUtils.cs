using UnityEditor;

namespace WiFiInput.Server
{
    public static class WiFiInputServerEditorUtils
    {
        [MenuItem("Assets/Create/WiFi Input/Server/Players Database")]
        public static void CreateWiFiPlayersDatabase()
        {
            ScriptableObjectUtility.CreateAsset<WiFiPlayersDatabase>();
        }

        [MenuItem("Assets/Create/WiFi Input/Server/Controls Database")]
        public static void CreateControlsDatabase()
        {
            ScriptableObjectUtility.CreateAsset<WiFiControlsDatabase>();
        }

        [MenuItem("Assets/Create/WiFi Input/Server/Actions Database")]
        public static void CreateWiFiActionsDatabase()
        {
            ScriptableObjectUtility.CreateAsset<WiFiActionsDatabase>();
        }

        [MenuItem("Assets/Create/WiFi Input/Server/WiFi Input Manager Config")]
        public static void CreateWiFiInputManagerConfig()
        {
            ScriptableObjectUtility.CreateAsset<WiFiInputManagerConfig>();
        }
    }
}
