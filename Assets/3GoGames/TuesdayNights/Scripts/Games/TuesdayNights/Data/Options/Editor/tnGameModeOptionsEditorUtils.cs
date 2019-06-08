using UnityEditor;

public static class tnGameModeOptionsEditorUtils
{
    [MenuItem("Assets/Create/TuesdayNights/Database/GameModeOptions Database")]
    public static void CreateGameModeOptions()
    {
        ScriptableObjectUtility.CreateAsset<tnGameModeOptions>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Database/GameModeConfigs Database")]
    public static void CreateGameModeOptionsConfigs()
    {
        ScriptableObjectUtility.CreateAsset<tnGameModeConfigsDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Options/Game Mode Config")]
    public static void CreateGameModeConfig()
    {
        ScriptableObjectUtility.CreateAsset<tnGameModeConfigDescriptor>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Options/Game Mode Int Option")]
    public static void CreateGameModeIntOption()
    {
        ScriptableObjectUtility.CreateAsset<tnGameModeIntOptionDescriptor>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Options/Game Mode Float Option")]
    public static void CreateGameModeFloatOption()
    {
        ScriptableObjectUtility.CreateAsset<tnGameModeFloatOptionDescriptor>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Options/Game Mode String Option")]
    public static void CreateGameModeStringOption()
    {
        ScriptableObjectUtility.CreateAsset<tnGameModeStringOptionDescriptor>();
    }
}
