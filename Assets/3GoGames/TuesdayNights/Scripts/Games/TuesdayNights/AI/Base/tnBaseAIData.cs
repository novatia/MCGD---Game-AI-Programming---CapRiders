using UnityEngine;

using System.Collections.Generic;

public class tnBaseAIData
{
    // Characters

    private List<Transform> m_MyTeam = null;
    private List<Transform> m_OpponentTeam = null;

    // Ball

    private Transform m_Ball = null;

    // Goals

    private Transform m_Goal = null;
    private Transform m_OpponentGoal = null;

    // Reference position.

    private Vector2 m_ReferencePosition = Vector2.zero;

    // Anchors

    private Transform m_TopLeftAnchor = null;
    private Transform m_BottomRightAnchor = null;

    private Transform m_MidfieldAnchor = null;

    private Transform m_GoalTop = null;
    private Transform m_GoalBottom = null;

    private Transform m_AreaTop = null;
    private Transform m_AreaBottom = null;

    // ACCESSORS

    public Transform ball
    {
        get
        {
            return m_Ball;
        }
    }

    public Transform goal
    {
        get
        {
            return m_Goal;
        }
    }

    public int myTeamCharactersCount
    {
        get
        {
            return m_MyTeam.Count;
        }
    }

    public int opponentTeamCharactersCount
    {
        get
        {
            return m_OpponentTeam.Count;
        }
    }

    public Transform opponentGoal
    {
        get
        {
            return m_OpponentGoal;
        }
    }

    public Vector2 referencePosition
    {
        get
        {
            return m_ReferencePosition;
        }
    }

    public Transform topLeftAnchor
    {
        get
        {
            return m_TopLeftAnchor;
        }
    }

    public Transform bottomRightAnchor
    {
        get
        {
            return m_BottomRightAnchor;
        }
    }

    public Transform midfieldAnchor
    {
        get
        {
            return m_MidfieldAnchor;
        }
    }

    public Transform goalTop
    {
        get
        {
            return m_GoalTop;
        }
    }

    public Transform goalBottom
    {
        get
        {
            return m_GoalBottom;
        }
    }

    public Transform areaTop
    {
        get
        {
            return m_AreaTop;
        }
    }

    public Transform areaBottom
    {
        get
        {
            return m_AreaBottom;
        }
    }

    public Transform GetMyTeamCharacter(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_MyTeam.Count)
        {
            return null;
        }

        return m_MyTeam[i_Index];
    }

    public Transform GetOpponentTeamCharacter(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_OpponentTeam.Count)
        {
            return null;
        }

        return m_OpponentTeam[i_Index];
    }

    public void AddMyTeamCharacter(Transform i_Character)
    {
        m_MyTeam.Add(i_Character);
    }

    public void AddOpponentTeamCharacter(Transform i_Character)
    {
        m_OpponentTeam.Add(i_Character);
    }

    public void SetBall(Transform i_Ball)
    {
        m_Ball = i_Ball;
    }

    public void SetGoal(Transform i_Goal)
    {
        m_Goal = i_Goal;
    }

    public void SetOpponentGoal(Transform i_Goal)
    {
        m_OpponentGoal = i_Goal;
    }

    public void SetReferencePosition(Vector2 i_ReferencePosition)
    {
        m_ReferencePosition = i_ReferencePosition;
    }

    public void SetTopLeftAnchor(Transform i_Anchor)
    {
        m_TopLeftAnchor = i_Anchor;
    }

    public void SetBottomRightAnchor(Transform i_Anchor)
    {
        m_BottomRightAnchor = i_Anchor;
    }

    public void SetMidfieldAnchor(Transform i_Anchor)
    {
        m_MidfieldAnchor = i_Anchor;
    }

    public void SetGoalTop(Transform i_Anchor)
    {
        m_GoalTop = i_Anchor;
    }

    public void SetGoalBottom(Transform i_Anchor)
    {
        m_GoalBottom = i_Anchor;
    }

    public void SetAreaTop(Transform i_Anchor)
    {
        m_AreaTop = i_Anchor;
    }

    public void SetAreaBottom(Transform i_Anchor)
    {
        m_AreaBottom = i_Anchor;
    }

    // CTOR

    public tnBaseAIData()
    {
        m_MyTeam = new List<Transform>();
        m_OpponentTeam = new List<Transform>();
    }
}
