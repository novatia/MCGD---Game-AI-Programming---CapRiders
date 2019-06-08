using UnityEditor;

public static class tnAIEditorUtils
{
    [MenuItem("Assets/Create/TuesdayNights/AI/Standard AI Params")]
    public static void CreateStandardAIParams()
    {
        ScriptableObjectUtility.CreateAsset<tnStandardAIInputFillerParams>();
    }
}
