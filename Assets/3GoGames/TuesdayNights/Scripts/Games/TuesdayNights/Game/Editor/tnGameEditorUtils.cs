using UnityEditor;

public static class tnGameEditorUtils
{
    [MenuItem("Assets/Create/TuesdayNights/Game/Rumble Params")]
    public static void CreateRumbleParams()
    {
        ScriptableObjectUtility.CreateAsset<tnRumbleParams>();
    }
}
