using UnityEngine;
using System;

[Serializable]
public class Achievement
{
    [SerializeField]
    private string m_SteamId = "";

    private bool m_Achieved = false;

    public string steamId
    {
        get { return m_SteamId; }
    }

    public bool isAchieved
    {
        get { return m_Achieved; }
    }

    // LOGIC

    public void SetAchieved(bool i_Achieved)
    {
        m_Achieved = i_Achieved;
    }
}
