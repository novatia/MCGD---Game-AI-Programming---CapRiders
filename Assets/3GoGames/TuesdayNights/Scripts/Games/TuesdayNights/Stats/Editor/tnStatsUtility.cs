using UnityEditor;

public static class vpStatsUtility
{
    [MenuItem("Assets/Create/TuesdayNights/Stats/Stats Asset")]
    public static void CreateStatsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnStatsDatabase>();
    }
}
