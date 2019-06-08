using UnityEngine;

using FullInspector;

public class tnGameModeOptions : BaseScriptableObject
{
    [SerializeField]
    private tnGameModeFloatOptionDescriptor m_MatchDurationOption = null;
    [SerializeField]
    private tnGameModeStringOptionDescriptor m_RefereeOption = null;
    [SerializeField]
    private tnGameModeStringOptionDescriptor m_GoldenGoalOption = null;

    public tnGameModeFloatOptionDescriptor matchDurationOption
    {
        get { return m_MatchDurationOption; }
    }

    public tnGameModeStringOptionDescriptor refereeOption
    {
        get { return m_RefereeOption; }
    }

    public tnGameModeStringOptionDescriptor goldenGoalOption
    {
        get { return m_GoldenGoalOption; }
    }
}
