using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class AchievementsDatabase : BaseScriptableObject
{
    [SerializeField]
    private Dictionary<string, Achievement> m_Achievements = new Dictionary<string, Achievement>();

    public int count
    {
        get { return m_Achievements.Count; }
    }

    public Dictionary<string, Achievement>.KeyCollection keys
    {
        get { return m_Achievements.Keys; }
    }

    public Achievement GetAchievement(string i_Id)
    {
        Achievement achievement = null;
        m_Achievements.TryGetValue(i_Id, out achievement);
        return achievement;
    }
}
