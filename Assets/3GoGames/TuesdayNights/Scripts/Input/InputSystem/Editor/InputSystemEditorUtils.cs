using UnityEditor;

public static class InputSystemEditorUtils
{ 
    [MenuItem("Assets/Create/Input/Input Module Config")]
    public static void CreateInputModuleConfig()
    {
        ScriptableObjectUtility.CreateAsset<InputModuleConfig>();
    }
}
