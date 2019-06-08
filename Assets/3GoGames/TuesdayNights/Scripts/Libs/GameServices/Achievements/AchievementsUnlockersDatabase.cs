using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class AchievementsUnlockersDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<AchievementUnlockerDescriptor> m_AchievementsUnlockersDescriptors = new List<AchievementUnlockerDescriptor>();

    public int elementsCount
    {
        get { return m_AchievementsUnlockersDescriptors.Count; }
    }

    public AchievementUnlockerDescriptor GetAchievementConditionDescriptor(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_AchievementsUnlockersDescriptors.Count)
        {
            return null;
        }

        return m_AchievementsUnlockersDescriptors[i_Index];
    }
}
