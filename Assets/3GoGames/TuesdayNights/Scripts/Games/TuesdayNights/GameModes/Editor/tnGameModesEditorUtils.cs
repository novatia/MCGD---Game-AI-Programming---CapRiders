using UnityEditor;

public static class tnGameModesEditorUtils
{
    [MenuItem("Assets/Create/TuesdayNights/Database/Game Modes")]
    public static void CreateGameModesDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnGameModesDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Data/Game Mode")]
    public static void CreateGameMode()
    {
        ScriptableObjectUtility.CreateAsset<tnGameModeDataDescriptor>();
    }
}
