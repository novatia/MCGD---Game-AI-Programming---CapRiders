using System.Collections.Generic;

public class AchievementUnlocker
{
    private string m_AchievementId;
    private bool m_CheckOnEvent;
    private string m_EventName;
    private List<UserStatCondition> m_Conditions = null;

    public bool checkOnEvent
    {
        get { return m_CheckOnEvent; }
    }

    public string eventName
    {
        get { return m_EventName; }
    }

    // LOGIC

    public void Initialize()
    {
        StatsModule statsModule = GameServices.GetModuleMain<StatsModule>();
        if (statsModule != null)
        {
            for (int conditionIndex = 0; conditionIndex < m_Conditions.Count; ++conditionIndex)
            {
                UserStatCondition condition = m_Conditions[conditionIndex];
                if (condition != null)
                {
                    condition.Initialize(statsModule);
                }
            }
        }

        if (m_CheckOnEvent)
        {
            if (m_EventName != "")
            {
                Messenger.AddListener(m_EventName, OnEvent);
            }
        }
    }

    public void Uninitialize()
    {
        if (m_CheckOnEvent)
        {
            if (m_EventName != "")
            {
                Messenger.RemoveListener(m_EventName, OnEvent);
            }
        }
    }

    public void Update()
    {
        if (m_CheckOnEvent)
            return;

        InternalTest();
    }

    // INTERNALS

    private void InternalTest()
    {
        bool result = Evaluate();
        if (result)
        {
            UnlockAchievement();
        }
    }

    private bool Evaluate()
    {
        bool unlock = (m_Conditions.Count > 0);

        for (int conditionIndex = 0; conditionIndex < m_Conditions.Count; ++conditionIndex)
        {
            UserStatCondition condition = m_Conditions[conditionIndex];
            if (condition != null)
            {
                bool conditionValue = condition.Evaluate();
                unlock &= conditionValue;
            }
        }

        return unlock;
    }

    private void UnlockAchievement()
    {
        AchievementsModule achievementModule = GameServices.GetModuleMain<AchievementsModule>();
        if (achievementModule != null)
        {
            achievementModule.UnlockAchievement(m_AchievementId);
        }
    }

    // EVENTS

    private void OnEvent()
    {
        if (m_Conditions.Count > 0)
        {
            InternalTest();
        }
        else
        {
            UnlockAchievement();
        }
    }

    // CTOR

    public AchievementUnlocker(AchievementUnlockerDescriptor i_Descriptor)
    {
        m_AchievementId = i_Descriptor.achievementId;

        m_CheckOnEvent = i_Descriptor.checkOnEvent;
        m_EventName = i_Descriptor.eventName;

        m_Conditions = new List<UserStatCondition>();

        for (int conditionDescriptorIndex = 0; conditionDescriptorIndex < i_Descriptor.conditionsCount; ++conditionDescriptorIndex)
        {
            UserStatConditionDescriptor conditionDescriptor = i_Descriptor.GetConditionDescriptor(conditionDescriptorIndex);
            if (conditionDescriptor != null)
            {
                UserStatCondition condition = conditionDescriptor.BuildCondition();
                if (condition != null)
                {
                    m_Conditions.Add(condition);
                }
            }
        }
    }
}
