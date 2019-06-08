using UnityEngine;

using FullInspector;

public class tnGameModeConfigDescriptor : BaseScriptableObject
{
    [SerializeField]
    private string m_MatchDurationOption = "NULL";
    [SerializeField]
    private string m_RefereeOption = "OFF";
    [SerializeField]
    private string m_GoldenGoalOption = "ON";

    public string matchDurationOption
    {
        get { return m_MatchDurationOption; }
    }

    public string refereeOption
    {
        get { return m_RefereeOption; }
    }

    public string goldenGoalOption
    {
        get { return m_GoldenGoalOption; }
    }
}
