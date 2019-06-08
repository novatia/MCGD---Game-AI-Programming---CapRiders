using UnityEditor;

namespace WiFiInput.Client
{
    [CustomEditor(typeof(WiFiInputManager))]
    public class WiFiInputManagerEditor : Editor
    {
        private static readonly string[] _dontIncludeMeClient = new string[] { "serverSendBackchannel", "serverSendHeartbeatRate", "clientTimeout" };

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, _dontIncludeMeClient);
            serializedObject.ApplyModifiedProperties();
        }
    }
}