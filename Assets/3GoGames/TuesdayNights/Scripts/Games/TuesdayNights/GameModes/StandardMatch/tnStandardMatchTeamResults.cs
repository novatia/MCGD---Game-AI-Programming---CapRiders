using UnityEngine;
using System.Collections;

using TrueSync;

public class tnStandardMatchTeamResults : tnBaseMatchTeamResults
{
    [AddTracking]
    private int m_Score = 0;

    public int score
    {
        get
        {
            return m_Score;
        }
        set
        {
            m_Score = value;
        }
    }

    // CTOR

    public tnStandardMatchTeamResults(int i_Id)
        : base (i_Id)
    {
        m_Score = 0;
    }
}
