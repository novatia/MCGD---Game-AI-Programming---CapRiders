using UnityEditor;

public static class UIEditorUtils
{
    [MenuItem("Assets/Create/UI/Database/Icons")]
    public static void CreateIconsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<UIIconsDatabase>();
    }
}