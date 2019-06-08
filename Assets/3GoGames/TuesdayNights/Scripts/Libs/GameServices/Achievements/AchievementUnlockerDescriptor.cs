using UnityEngine;

using System;
using System.Collections.Generic;

[Serializable]
public class AchievementUnlockerDescriptor
{
    [SerializeField]
    private string m_AchievementId = "";
    [SerializeField]
    private bool m_CheckOnEvent = false;
    [SerializeField]
    private string m_EventName = "";
    [SerializeField]
    private List<UserStatConditionDescriptor> m_ConditionsDescriptor = new List<UserStatConditionDescriptor>();

    public string achievementId
    {
        get { return m_AchievementId; }
    }

    public bool checkOnEvent
    {
        get { return m_CheckOnEvent; }
    }

    public string eventName
    {
        get { return m_EventName; }
    }

    public int conditionsCount
    {
        get { return m_ConditionsDescriptor.Count; }
    }

    public UserStatConditionDescriptor GetConditionDescriptor(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_ConditionsDescriptor.Count)
        {
            return null;
        }

        return m_ConditionsDescriptor[i_Index];
    }
}
