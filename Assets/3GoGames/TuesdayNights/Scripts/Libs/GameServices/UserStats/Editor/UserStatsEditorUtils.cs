using UnityEditor;

public static class UserStatsEditorUtils
{
    [MenuItem("Assets/Create/UserStats/UserStats Database")]
    public static void CreateUserStatsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<UserStatsDatabase>();
    }

    [MenuItem("Assets/Create/UserStats/Remote Stats Map")]
    public static void CreateRemoteStatsMap()
    {
        ScriptableObjectUtility.CreateAsset<RemoteStatsMap>();
    }
}