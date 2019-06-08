using UnityEngine;

using System.Collections.Generic;

public class tnGameModeOptionsManager
{
    private tnGameModeFloatOption m_MatchDurationOption = null;
    private tnGameModeStringOption m_RefereeOption = null;
    private tnGameModeStringOption m_GoldenGoalOption = null;

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnGameModeOptions database = Resources.Load<tnGameModeOptions>(i_DatabasePath);

        if (database != null)
        {
            tnGameModeFloatOptionDescriptor matchDurationOptionDescriptor = database.matchDurationOption;
            m_MatchDurationOption = new tnGameModeFloatOption(matchDurationOptionDescriptor);

            tnGameModeStringOptionDescriptor refereeOptionDescriptor = database.refereeOption;
            m_RefereeOption = new tnGameModeStringOption(refereeOptionDescriptor);

            tnGameModeStringOptionDescriptor goldenGoalOptionDescriptor = database.goldenGoalOption;
            m_GoldenGoalOption = new tnGameModeStringOption(goldenGoalOptionDescriptor);
        }
        else
        {
            m_MatchDurationOption = new tnGameModeFloatOption(null);
            m_RefereeOption = new tnGameModeStringOption(null);
            m_GoldenGoalOption = new tnGameModeStringOption(null);

            LogManager.LogWarning(this, "Database not loaded.");
        }
    }

    // Match duration

    public List<int> GetMatchDurationOptionKeys()
    {
        if (m_MatchDurationOption != null)
        {
            return m_MatchDurationOption.GetKeys();
        }

        return null;
    }

    public bool TryGetMatchDurationValue(string i_Id, out float o_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return TryGetMatchDurationValue(hash, out o_Value);
    }

    public bool TryGetMatchDurationValue(int i_Id, out float o_Value)
    {
        o_Value = 0f;

        if (m_MatchDurationOption != null)
        {
            return m_MatchDurationOption.TryGetValue(i_Id, out o_Value);
        }

        return false;
    }

    // Referee

    public List<int> GetRefereeOptionKeys()
    {
        if (m_RefereeOption != null)
        {
            return m_RefereeOption.GetKeys();
        }

        return null;
    }

    public bool TryGetRefereeValue(string i_Id, out string o_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return TryGetRefereeValue(hash, out o_Value);
    }

    public bool TryGetRefereeValue(int i_Id, out string o_Value)
    {
        o_Value = "";

        if (m_RefereeOption != null)
        {
            return m_RefereeOption.TryGetValue(i_Id, out o_Value);
        }

        return false;
    }

    // Golden goal

    public List<int> GetGoldenGoalOptionKeys()
    {
        if (m_GoldenGoalOption != null)
        {
            return m_GoldenGoalOption.GetKeys();
        }

        return null;
    }
    
    public bool TryGetGoldenGoalValue(string i_Id, out string o_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return TryGetGoldenGoalValue(hash, out o_Value);
    }

    public bool TryGetGoldenGoalValue(int i_Id, out string o_Value)
    {
        o_Value = "";

        if (m_GoldenGoalOption != null)
        {
            return m_GoldenGoalOption.TryGetValue(i_Id, out o_Value);
        }

        return false;
    }

    // CTOR

    public tnGameModeOptionsManager()
    {

    }
}
