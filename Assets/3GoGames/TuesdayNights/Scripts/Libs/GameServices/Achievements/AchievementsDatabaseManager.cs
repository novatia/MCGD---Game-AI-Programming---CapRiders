using UnityEngine;

using System.Collections.Generic;

public class AchievementsDatabaseManager
{
    private Dictionary<int, Achievement> m_Achievements = null;
    private List<int> m_AchievementsIds = null;
    
    // GETTERS

    public int achievementsCount
    {
        get { return m_Achievements.Count; }
    }
    
    // LOGIC

    public void Initialize(string i_DatabaseResourcePath)
    {
        AchievementsDatabase database = Resources.Load<AchievementsDatabase>(i_DatabaseResourcePath);

        if (database != null)
        {
            foreach (string key in database.keys)
            {
                Achievement achievement = database.GetAchievement(key);
                if (achievement != null)
                {
                    int hash = StringUtils.GetHashCode(key);
                    m_Achievements.Add(hash, achievement);
                    m_AchievementsIds.Add(hash);
                }
            }
        }
        else
        {
            LogManager.LogWarning(this, "Database not loaded.");
        }
    }

    public Achievement GetAchievement(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetAchievement(hash);
    }

    public Achievement GetAchievement(int i_Id)
    {
        Achievement achievement = null;
        m_Achievements.TryGetValue(i_Id, out achievement);
        return achievement;
    }

    public Achievement GetAchievementByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_AchievementsIds.Count)
        {
            return null;
        }

        int id = m_AchievementsIds[i_Index];
        return GetAchievement(id);
    }

    // CTOR

    public AchievementsDatabaseManager()
    {
        m_Achievements = new Dictionary<int, Achievement>();
        m_AchievementsIds = new List<int>();
    }
}
