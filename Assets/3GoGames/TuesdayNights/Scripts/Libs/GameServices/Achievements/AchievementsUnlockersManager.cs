using UnityEngine;

using System.Collections.Generic;

public class AchievementsUnlockersManager : MonoBehaviour
{
    private AchievementsUnlockersDatabase m_Database = null;
    private List<AchievementUnlocker> m_AchievementsUnlockers = new List<AchievementUnlocker>();

    // MonoBehaviour's interface

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Initialize("Database/Achievements/AchievementsUnlockersDatabase");

        GameServices gameServices = FindObjectOfType<GameServices>();
        if (gameServices != null)
        {
            transform.parent = gameServices.transform;
        }
    }

    void OnEnable()
    {
        InitializeUnlockers();
    }

    void OnDisable()
    {
        UninitializeUnlockers();
    }

    void Update()
    {
        UpdateUnlockers();
    }

    // INTERNALS

    private void Initialize(string i_DatabaseResourcePath)
    {
        m_Database = Resources.Load<AchievementsUnlockersDatabase>(i_DatabaseResourcePath);

        if (m_Database != null)
        {
            for (int achievementConditionDescriptorIndex = 0; achievementConditionDescriptorIndex < m_Database.elementsCount; ++achievementConditionDescriptorIndex)
            {
                AchievementUnlockerDescriptor achievementUnlockerDescriptor = m_Database.GetAchievementConditionDescriptor(achievementConditionDescriptorIndex);
                if (achievementUnlockerDescriptor != null)
                {
                    string achievementId = achievementUnlockerDescriptor.achievementId;
                    if (achievementId != "")
                    {
                        AchievementUnlocker achievementUnlocker = new AchievementUnlocker(achievementUnlockerDescriptor);
                        m_AchievementsUnlockers.Add(achievementUnlocker);
                    }
                }
            }
        }
        else
        {
            LogManager.LogWarning(this, "Database could not be loaded.");
        }
    }

    private void InitializeUnlockers()
    {
        for (int index = 0; index < m_AchievementsUnlockers.Count; ++index)
        {
            AchievementUnlocker achievementUnlocker = m_AchievementsUnlockers[index];
            if (achievementUnlocker != null)
            {
                achievementUnlocker.Initialize();
            }
        }
    }

    private void UninitializeUnlockers()
    {
        for (int index = 0; index < m_AchievementsUnlockers.Count; ++index)
        {
            AchievementUnlocker achievementUnlocker = m_AchievementsUnlockers[index];
            if (achievementUnlocker != null)
            {
                achievementUnlocker.Uninitialize();
            }
        }
    }

    private void UpdateUnlockers()
    {
        for (int index = 0; index < m_AchievementsUnlockers.Count; ++index)
        {
            AchievementUnlocker achievementUnlocker = m_AchievementsUnlockers[index];
            if (achievementUnlocker != null)
            {
                achievementUnlocker.Update();
            }
        }
    }
}
