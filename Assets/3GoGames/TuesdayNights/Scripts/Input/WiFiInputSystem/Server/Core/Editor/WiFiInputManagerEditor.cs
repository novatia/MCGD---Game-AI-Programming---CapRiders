using UnityEditor;

namespace WiFiInput.Server
{
    [CustomEditor(typeof(WiFiInputManager))]
    public class WiFiInputManagerEditor : Editor
    {
        private static readonly string[] _dontIncludeMeServer = new string[] { "clientConnectAutomatically", "heartbeatTimeout", "clientMaxSendRate" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, _dontIncludeMeServer);
            serializedObject.ApplyModifiedProperties();
        }
    }
}