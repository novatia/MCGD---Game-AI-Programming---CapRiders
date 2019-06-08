using UnityEngine;
using System.Collections;

using TrueSync;

public class tnBaseMatchCharacterResults : tnCharacterResults
{
    // SHOTS

    [AddTracking]
    protected int m_Shots = 0;
    public int shots
    {
        get
        {
            return m_Shots;
        }

        set
        {
            m_Shots = value;
        }
    }

    // SHOTS ON TARGET

    [AddTracking]
    protected int m_ShotsOnTarget = 0;
    public int shotsOnTarget
    {
        get
        {
            return m_ShotsOnTarget;
        }

        set
        {
            m_ShotsOnTarget = value;
        }
    }

    // DISTANCE RUN

    [AddTracking]
    protected FP m_DistanceRun = FP.Zero;
    public FP distanceRun
    {
        get
        {
            return m_DistanceRun;
        }

        set
        {
            m_DistanceRun = value;
        }
    }

    // DASH COUNT

    [AddTracking]
    protected int m_DashCount = 0;
    public int dashCount
    {
        get
        {
            return m_DashCount;
        }

        set
        {
            m_DashCount = value;
        }
    }

    // BALL TOUCHES

    [AddTracking]
    protected int m_BallTouches = 0;
    public int ballTouches
    {
        get
        {
            return m_BallTouches;
        }

        set
        {
            m_BallTouches = value;
        }
    }

    // ATTRACT TIME

    [AddTracking]
    protected FP m_AttratcTime = FP.Zero;
    public FP attractTime
    {
        get
        {
            return m_AttratcTime;
        }

        set
        {
            m_AttratcTime = value;
        }
    }

    // GOAL SCORED

    [AddTracking]
    protected int m_GoalScored = 0;
    public int goalScored
    {
        get
        {
            return m_GoalScored;
        }

        set
        {
            m_GoalScored = value;
        }
    }

    // GOAL SAVED

    [AddTracking]
    protected int m_GoalSaved = 0;
    public int goalSaved
    {
        get
        {
            return m_GoalSaved;
        }

        set
        {
            m_GoalSaved = value;
        }
    }

    // TACKLES

    [AddTracking]
    protected int m_Tackles = 0;
    public int tackles
    {
        get
        {
            return m_Tackles;
        }

        set
        {
            m_Tackles = value;
        }
    }

    // TACKLES RECEIVED

    [AddTracking]
    protected int m_TacklesReceived = 0;
    public int tacklesReceived
    {
        get
        {
            return m_TacklesReceived;
        }

        set
        {
            m_TacklesReceived = value;
        }
    }

    // CTOR

    public tnBaseMatchCharacterResults(int i_Id)
        : base (i_Id)
    {
        m_Shots = 0;
        m_ShotsOnTarget = 0;

        m_DistanceRun = FP.Zero;

        m_DashCount = 0;

        m_BallTouches = 0;

        m_AttratcTime = FP.Zero;

        m_GoalScored = 0;
        m_GoalSaved = 0;

        m_Tackles = 0;
        m_TacklesReceived = 0;
    }
}
