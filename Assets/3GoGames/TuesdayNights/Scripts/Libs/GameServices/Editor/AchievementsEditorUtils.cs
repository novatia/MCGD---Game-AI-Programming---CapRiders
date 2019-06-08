using UnityEditor;

public static class AchievementsEditorUtils
{
    [MenuItem("Assets/Create/Achievements/Achievements Database")]
    public static void CreateAchievementsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<AchievementsDatabase>();
    }

    [MenuItem("Assets/Create/Achievements/Achievements Unlockers Database")]
    public static void CreateAchievementsUnlockersDatabase()
    {
        ScriptableObjectUtility.CreateAsset<AchievementsUnlockersDatabase>();
    }
}
