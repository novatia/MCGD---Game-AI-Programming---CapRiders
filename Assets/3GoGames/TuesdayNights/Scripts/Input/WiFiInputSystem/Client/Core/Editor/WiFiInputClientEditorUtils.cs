using UnityEditor;

namespace WiFiInput.Client
{
    public static class WiFiInputClientEditorUtils
    {
        [MenuItem("Assets/Create/WiFi Input/Client/WiFi Input Manager Config")]
        public static void CreateWiFiInputManagerConfig()
        {
            ScriptableObjectUtility.CreateAsset<WiFiInputManagerConfig>();
        }
    }
}
