using UnityEngine;

using System.Collections.Generic;

using TrueSync;

public abstract class tnBaseAIInputFiller : tnAIInputFiller
{
    // Fields

    private Transform m_Ball = null;

    private Transform m_MyGoal = null;
    private Transform m_OpponentGoal = null;

    private Vector2 m_ReferencePosition = Vector2.zero;

    private List<Transform> m_Team = null;

    private List<Transform> m_Teammates = null;
    private List<Transform> m_Opponents = null;

    private Transform m_TopLeft = null;
    private Transform m_BottomRight = null;
    private Transform m_Midfield = null;

    private float m_GKAreaMinHeight = 0f;
    private float m_GKAreaMaxHeight = 0f;

    private float m_GKAreaWidth = 2f;
    private float m_GKAreaHeight = 2f;

    private float m_GoalMinHeight = 0f;
    private float m_GoalMaxHeight = 0f;

    private float m_GoalWidth = 1f;

    private float m_ColliderRadius = 0f;
    private float m_BallRadius = 0f;

    private bool m_Initialized = false;

    // ACCESSORS

    protected int teamCharactersCount
    {
        get
        {
            return m_Team.Count;
        }
    }

    protected int teammatesCount
    {
        get { return m_Teammates.Count; }
    }

    protected int opponentsCount
    {
        get { return m_Opponents.Count; }
    }

    protected Vector2 myPosition
    {
        get
        {
            if (self == null)
            {
                return Vector2.zero;
            }

            return self.transform.position;
        }
    }

    protected Transform ball
    {
        get { return m_Ball; }
    }

    protected Vector2 ballPosition
    {
        get
        {
            if (ball == null)
            {
                return Vector2.zero;
            }

            return ball.position;
        }
    }

    protected float ballDistance
    {
        get
        {
            if (self == null || ball == null)
            {
                return float.MaxValue;
            }

            return Vector2.Distance(myPosition, ballPosition);
        }
    }

    protected Transform myGoal
    {
        get { return m_MyGoal; }
    }

    protected Transform opponentGoal
    {
        get { return m_OpponentGoal; }
    }

    protected Vector2 myGoalPosition
    {
        get
        {
            if (m_MyGoal == null)
            {
                return Vector2.zero;
            }

            return m_MyGoal.transform.position;
        }
    }

    protected Vector2 opponentGoalPosition
    {
        get
        {
            if (m_OpponentGoal == null)
            {
                return Vector2.zero;
            }

            return m_OpponentGoal.transform.position;
        }
    }

    protected Vector2 referencePosition
    {
        get { return m_ReferencePosition; }
    }

    protected Vector2 topLeft
    {
        get
        {
            if (m_TopLeft == null)
            {
                return Vector2.zero;
            }

            return m_TopLeft.position;
        }
    }

    protected Vector2 bottomRight
    {
        get
        {
            if (m_BottomRight == null)
            {
                return Vector2.zero;
            }

            return m_BottomRight.position;
        }
    }

    protected Vector2 topRight
    {
        get
        {
            return new Vector2(bottomRight.x, topLeft.y);
        }
    }

    protected Vector2 bottomLeft
    {
        get
        {
            return new Vector2(topLeft.x, bottomRight.y);
        }
    }

    protected Vector2 midfield
    {
        get
        {
            if (m_Midfield == null)
            {
                return Vector2.zero;
            }

            return m_Midfield.position;
        }
    }

    protected float fieldWidth
    {
        get
        {
            if (m_BottomRight == null || m_TopLeft == null)
            {
                return 0f;
            }

            return m_BottomRight.position.x - m_TopLeft.position.x;
        }
    }

    protected float fieldHeight
    {
        get
        {
            if (m_TopLeft == null || m_BottomRight == null)
            {
                return 0f;
            }

            return m_TopLeft.position.y - m_BottomRight.position.y;
        }
    }

    protected float halfFieldWidth
    {
        get
        {
            return fieldWidth / 2f;
        }
    }

    protected float halfFieldHeight
    {
        get
        {
            return fieldHeight / 2f;
        }
    }

    protected float gkAreaMinHeight
    {
        get
        {
            return m_GKAreaMinHeight;
        }
    }

    protected float gkAreaMaxHeight
    {
        get
        {
            return m_GKAreaMaxHeight;
        }
    }

    protected float gkAreaWidth
    {
        get { return m_GKAreaWidth; }
    }

    protected float gkAreaHeight
    {
        get { return m_GKAreaHeight; }
    }

    protected float goalMinHeight
    {
        get
        {
            return m_GoalMinHeight;
        }
    }

    protected float goalMaxHeight
    {
        get
        {
            return m_GoalMaxHeight;
        }
    }

    protected float goalWidth
    {
        get { return m_GoalWidth; }
    }

    protected float colliderRadius
    {
        get
        {
            return m_ColliderRadius;
        }
    }

    protected float ballRadius
    {
        get
        {
            return m_BallRadius;
        }
    }

    protected bool initialized
    {
        get
        {
            return m_Initialized;
        }
    }

    // BUSINESS LOGIC

    public void Setup(tnBaseAIData i_Data)
    {
        if (i_Data == null)
            return;

        bool errorOccurred = false;

        // Team.

        {
            int teamCharactersCount = i_Data.myTeamCharactersCount;

            for (int index = 0; index < teamCharactersCount; ++index)
            {
                Transform character = i_Data.GetMyTeamCharacter(index);

                if (character == null)
                    continue;

                m_Team.Add(character);
            }
        }

        // Teammates.

        {
            int teamCharactersCount = i_Data.myTeamCharactersCount;

            for (int index = 0; index < teamCharactersCount; ++index)
            {
                Transform character = i_Data.GetMyTeamCharacter(index);

                if (character == null)
                    continue;

                if (character.gameObject != self)
                {
                    m_Teammates.Add(character);
                }
            }
        }

        // Opponents.

        {
            int opponentsCount = i_Data.opponentTeamCharactersCount;

            for (int opponentIndex = 0; opponentIndex < opponentsCount; ++opponentIndex)
            {
                Transform opponent = i_Data.GetOpponentTeamCharacter(opponentIndex);

                if (opponent == null)
                    continue;

                m_Opponents.Add(opponent);
            }
        }

        // Ball.

        {
            m_Ball = i_Data.ball;

            if (m_Ball == null)
            {
                errorOccurred |= true;
            }
            else
            {
                // Init ball radius.

                /*

                CircleCollider2D ballCollider = m_Ball.GetComponent<CircleCollider2D>();
                if (ballCollider == null)
                {
                    errorOccurred |= true;
                }
                else
                {
                    m_BallRadius = ballCollider.radius;
                }

                */

                m_BallRadius = 0.25f;

                // Apply object scale, assuming that it is uniform.

                m_BallRadius *= m_Ball.transform.localScale.x;
            }
        }

        // Goal.

        {
            m_MyGoal = i_Data.goal;

            if (m_MyGoal == null)
            {
                errorOccurred |= true;
            }
        }

        // Opponent goal.

        {
            m_OpponentGoal = i_Data.opponentGoal;

            if (m_OpponentGoal == null)
            {
                errorOccurred |= true;
            }
        }

        // Reference position.

        {
            m_ReferencePosition = i_Data.referencePosition;
        }

        // Anchors.

        {
            m_TopLeft = i_Data.topLeftAnchor;
            m_BottomRight = i_Data.bottomRightAnchor;

            if (m_TopLeft == null || m_BottomRight == null)
            {
                errorOccurred |= true;
            }
        }

        {
            m_Midfield = i_Data.midfieldAnchor;

            if (m_Midfield == null)
            {
                errorOccurred |= true;
            }
        }

        // Goal width.

        {
            Transform goalTop = i_Data.goalTop;
            Transform goalBottom = i_Data.goalBottom;

            if (goalTop == null || goalBottom == null)
            {
                errorOccurred |= true;
            }
            else
            {
                m_GoalMinHeight = goalBottom.position.y;
                m_GoalMaxHeight = goalTop.position.y;

                m_GoalWidth = goalTop.position.y - goalBottom.position.y;
            }
        }

        // Area size.

        {
            Transform topLeftAnchor = i_Data.topLeftAnchor;

            Transform areaTop = i_Data.areaTop;
            Transform areaBottom = i_Data.areaBottom;

            if (areaTop == null || areaBottom == null || topLeftAnchor == null)
            {
                errorOccurred |= true;
            }
            else
            {
                m_GKAreaMinHeight = areaBottom.position.y;
                m_GKAreaMaxHeight = areaTop.position.y;

                m_GKAreaWidth = areaTop.position.x - topLeftAnchor.position.x;
                m_GKAreaHeight = areaTop.position.y - areaBottom.position.y;
            }
        }

        if (errorOccurred)
        {
            Debug.LogError("[AI]" + "[" + self.name + "]" + " " + "Initialization failed.");
        }

        m_Initialized = !errorOccurred;
    }

    // STEERING

    protected Vector2 Seek(GameObject i_Go, float i_Tolerance = 0f)
    {
        if (i_Go == null)
        {
            return Vector2.zero;
        }

        Transform transform = i_Go.transform;
        return Seek(transform);
    }

    protected Vector2 Seek(Transform i_Transform, float i_Tolerance = 0f)
    {
        if (i_Transform == null)
        {
            return Vector2.zero;
        }

        Vector2 position = i_Transform.position;
        return Seek(position);
    }

    protected Vector2 Seek(Vector2 i_Target, float i_Tolerance = 0f)
    {
        if (self == null)
        {
            return Vector2.zero;
        }

        Vector2 difference = i_Target - myPosition;

        if (difference.sqrMagnitude < i_Tolerance * i_Tolerance)
        {
            return Vector2.zero;
        }

        Vector2 direction = difference.normalized;
        return direction;
    }

    protected Vector2 Flee(GameObject i_Go, float i_Tolerance = 0f)
    {
        if (i_Go == null)
        {
            return Vector2.zero;
        }

        Transform transform = i_Go.transform;
        return Flee(transform);
    }

    protected Vector2 Flee(Transform i_Transform, float i_Tolerance = 0f)
    {
        if (i_Transform == null)
        {
            return Vector2.zero;
        }

        Vector2 position = i_Transform.position;
        return Flee(position);
    }

    protected Vector2 Flee(Vector2 i_Target, float i_Tolerance = 0f)
    {
        if (self == null)
        {
            return Vector2.zero;
        }

        Vector2 difference = myPosition - i_Target;

        if (difference.sqrMagnitude < i_Tolerance * i_Tolerance)
        {
            return Vector2.zero;
        }

        Vector2 direction = difference.normalized;
        return direction;
    }

    protected Vector2 Pursuit(GameObject i_Go, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Go == null)
        {
            return Vector2.zero;
        }

        Transform transform = i_Go.transform;
        return Pursuit(transform, i_MaxLookAheadTime);
    }

    protected Vector2 Pursuit(Transform i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (self == null || i_Target == null)
        {
            return Vector2.zero;
        }

        Vector2 evaderPosition = i_Target.position;
        Vector2 myPosition = self.transform.position;

        Vector2 toEvader = evaderPosition - myPosition;
        float evaderDistance = toEvader.magnitude;

        Vector2 evaderVelocity = GetVehicleVelocity(i_Target);

        float mySpeed = GetVehicleSpeed(self);
        float evaderSpeed = GetVehicleSpeed(i_Target);

        float lookAheadTime = i_MaxLookAheadTime;
        if (mySpeed + evaderSpeed > 0f)
        {
            lookAheadTime = evaderDistance / (mySpeed + evaderSpeed);
        }

        return Seek(evaderPosition + evaderVelocity * lookAheadTime);
    }

    protected Vector2 Evade(GameObject i_Go, float i_MaxLookAheadTime = 0.5f)
    {
        if (i_Go == null)
        {
            return Vector2.zero;
        }

        Transform transform = i_Go.transform;
        return Evade(transform, i_MaxLookAheadTime);
    }

    protected Vector2 Evade(Transform i_Target, float i_MaxLookAheadTime = 0.5f)
    {
        if (self == null || i_Target == null)
        {
            return Vector2.zero;
        }

        Vector2 hunterPosition = i_Target.position;
        Vector2 myPosition = self.transform.position;

        Vector2 toHunter = hunterPosition - myPosition;
        float hunterDistance = toHunter.magnitude;

        Vector2 hunterVelocity = GetVehicleVelocity(i_Target);

        float mySpeed = GetVehicleSpeed(self);
        float hunterSpeed = GetVehicleSpeed(i_Target);

        float lookAheadTime = i_MaxLookAheadTime;
        if (mySpeed + hunterSpeed > Mathf.Epsilon)
        {
            lookAheadTime = hunterDistance / (mySpeed + hunterSpeed);
        }

        return Flee(hunterPosition + hunterVelocity * lookAheadTime);
    }

    protected Vector2 Interpose(GameObject i_A, GameObject i_B, float i_Weight = 0.5f)
    {
        if (i_A == null || i_B == null)
        {
            return Vector2.zero;
        }

        Transform a = i_A.transform;
        Transform b = i_B.transform;

        return Interpose(a, b, i_Weight);
    }

    protected Vector2 Interpose(Transform i_TargetA, Transform i_TargetB, float i_Weight = 0.5f)
    {
        if (self == null || i_TargetA == null || i_TargetB == null)
        {
            return Vector2.zero;
        }

        float w = Mathf.Clamp01(i_Weight);

        Vector2 myPosition = self.transform.position;
        Vector2 aPosition = i_TargetA.position;
        Vector2 bPosition = i_TargetB.position;

        Vector2 aVelocity = GetVehicleVelocity(i_TargetA);
        Vector2 bVelocity = GetVehicleVelocity(i_TargetB);

        float mySpeed = GetVehicleSpeed(self);

        Vector2 midPoint = Vector2.Lerp(aPosition, bPosition, w);

        Vector2 toMidPoint = midPoint - myPosition;
        float midPointDistance = toMidPoint.magnitude;

        float timeToReachMidPoint = 0f;
        if (mySpeed > Mathf.Epsilon)
        {
            timeToReachMidPoint = midPointDistance / mySpeed;
        }

        Vector2 aFuturePosition = aPosition + aVelocity * timeToReachMidPoint;
        Vector2 bFuturePosition = bPosition + bVelocity * timeToReachMidPoint;

        Vector2 futureMidPoint = Vector2.Lerp(aFuturePosition, bFuturePosition, w);

        return Seek(futureMidPoint);
    }

    // UTILS

    protected Transform GetTeamCharacter(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Team.Count)
        {
            return null;
        }

        return m_Team[i_Index];
    }

    protected Vector2 GetVehicleVelocity(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return Vector2.zero;
        }

        return GetVehicleVelocity(i_Transform.gameObject);
    }

    protected Vector2 GetVehicleVelocity(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return Vector2.zero;
        }

        TSRigidBody2D rigidbody2d = i_Go.GetComponent<TSRigidBody2D>();
        if (rigidbody2d != null)
        {
            TSVector2 tsVelocity = rigidbody2d.velocity;
            Vector2 velocity = tsVelocity.ToVector();
            return velocity;
        }

        return Vector2.zero;
    }

    protected float GetVehicleSpeed(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return 0f;
        }

        return GetVehicleSpeed(i_Transform.gameObject);
    }

    protected float GetVehicleSpeed(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return 0f;
        }

        TSRigidBody2D rigidbody2d = i_Go.GetComponent<TSRigidBody2D>();
        if (rigidbody2d != null)
        {
            TSVector2 tsVelocity = rigidbody2d.velocity;
            FP tsSpeed = tsVelocity.magnitude;
            float speed = tsSpeed.AsFloat();
            return speed;
        }

        return 0f;
    }

    protected bool IsInMyHalfSide(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return false;
        }

        return IsInMyHalfSide(i_Go.transform);
    }

    protected bool IsInMyHalfSide(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return false;
        }

        return IsInMyHalfSide(i_Transform.position);
    }

    protected bool IsInMyHalfSide(Vector2 i_Position)
    {
        if (m_MyGoal == null)
        {
            return false;
        }

        float positionX = i_Position.x;
        float myGoalX = m_MyGoal.transform.position.x;
        float midfieldX = midfield.x;

        return ((myGoalX > midfieldX && positionX > midfieldX) || (myGoalX < midfieldX && positionX < midfieldX)); 
    }

    protected bool IsBallInMyHalfSide()
    {
        return IsInMyHalfSide(m_Ball);
    }

    protected bool IsBehindBall(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return false;
        }

        return IsBehindBall(i_Go.transform);
    }

    protected bool IsBehindBall(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return false;
        }

        Vector2 position = i_Transform.position;
        return IsBehindBall(position);
    }

    protected bool IsBehindBall(Vector2 i_Position)
    {
        if (m_Ball == null || m_MyGoal == null)
        {
            return false;
        }

        Vector2 ballPosition = m_Ball.transform.position;
        Vector2 myGoalPosition = m_MyGoal.transform.position;

        return ((myGoalPosition.x > 0f && i_Position.x > ballPosition.x) || (myGoalPosition.x < 0f && i_Position.x < ballPosition.x));
    }

    protected bool IsCharacterBehindBall(GameObject i_Character)
    {
        if (i_Character == null)
        {
            return false;
        }

        return IsCharacterBehindBall(i_Character.transform);
    }

    protected bool IsCharacterBehindBall(Transform i_Character)
    {
        if (i_Character == null)
        {
            return false;
        }

        if (m_Ball == null || m_MyGoal == null)
        {
            return false;
        }

        Vector2 ballPosition = m_Ball.transform.position;
        Vector2 myGoalPosition = m_MyGoal.transform.position;

        float borderThreshold = GetColliderRadius(i_Character);
        borderThreshold += 0.5f; // Safety threshold.

        if (myGoalPosition.x > 0f)
        {
            return (i_Character.position.x > ballPosition.x) || (m_BottomRight != null && (m_BottomRight.position.x - i_Character.position.x < borderThreshold));
        }
        else
        {
            return (i_Character.position.x < ballPosition.x) || (m_TopLeft != null && (i_Character.position.x - m_TopLeft.position.x < borderThreshold));
        }
    }

    protected bool IsCloseToBorder(GameObject i_Go, float i_Tolerance = 0.1f)
    {
        if (i_Go == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Go);
        tolerance += i_Tolerance;

        return IsCloseToBorder(i_Go.transform.position, tolerance);
    }

    protected bool IsCloseToBorder(Transform i_Transform, float i_Tolerance = 0.1f)
    {
        if (i_Transform == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Transform);
        tolerance += i_Tolerance;

        return IsCloseToBorder(i_Transform.position, tolerance);
    }

    protected bool IsCloseToBorder(Vector2 i_Position, float i_Tolerance = 0.1f)
    {
        if (m_TopLeft == null || m_BottomRight == null)
        {
            return false;
        }

        // Check left.

        if (i_Position.x - m_TopLeft.position.x < i_Tolerance)
        {
            return true;
        }

        // Check right.

        if (m_BottomRight.position.x - i_Position.x < i_Tolerance)
        {
            return true;
        }

        // Check top.

        if (m_TopLeft.position.y - i_Position.y < i_Tolerance)
        {
            return true;
        }

        // Check bottom.

        if (i_Position.y - m_BottomRight.position.y < i_Tolerance)
        {
            return true;
        }

        return false;
    }

    protected bool IsCloseToLeftBorder(GameObject i_Go, float i_Tolerance = 0.1f)
    {
        if (i_Go == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Go);
        tolerance += i_Tolerance;

        return IsCloseToLeftBorder(i_Go.transform.position, tolerance);
    }

    protected bool IsCloseToRightBorder(GameObject i_Go, float i_Tolerance = 0.1f)
    {
        if (i_Go == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Go);
        tolerance += i_Tolerance;

        return IsCloseToRightBorder(i_Go.transform.position, tolerance);
    }

    protected bool IsCloseToTopBorder(GameObject i_Go, float i_Tolerance = 0.1f)
    {
        if (i_Go == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Go);
        tolerance += i_Tolerance;

        return IsCloseToTopBorder(i_Go.transform.position, tolerance);
    }

    protected bool IsCloseToBottomBorder(GameObject i_Go, float i_Tolerance = 0.1f)
    {
        if (i_Go == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Go);
        tolerance += i_Tolerance;

        return IsCloseToBottomBorder(i_Go.transform.position, tolerance);
    }

    protected bool IsCloseToLeftBorder(Transform i_Transform, float i_Tolerance = 0.1f)
    {
        if (i_Transform == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Transform);
        tolerance += i_Tolerance;

        return IsCloseToLeftBorder(i_Transform.position, tolerance);
    }

    protected bool IsCloseToRightBorder(Transform i_Transform, float i_Tolerance = 0.1f)
    {
        if (i_Transform == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Transform);
        tolerance += i_Tolerance;

        return IsCloseToRightBorder(i_Transform.position, tolerance);
    }

    protected bool IsCloseToTopBorder(Transform i_Transform, float i_Tolerance = 0.1f)
    {
        if (i_Transform == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Transform);
        tolerance += i_Tolerance;

        return IsCloseToTopBorder(i_Transform.position, tolerance);
    }

    protected bool IsCloseToBottomBorder(Transform i_Transform, float i_Tolerance = 0.1f)
    {
        if (i_Transform == null)
        {
            return false;
        }

        float tolerance = GetColliderRadius(i_Transform);
        tolerance += i_Tolerance;

        return IsCloseToBottomBorder(i_Transform.position, tolerance);
    }

    protected bool IsCloseToLeftBorder(Vector2 i_Position, float i_Tolerance = 0.1f)
    {
        if (m_TopLeft == null)
        {
            return false;
        }

        return (i_Position.x - m_TopLeft.position.x < i_Tolerance);
    }

    protected bool IsCloseToRightBorder(Vector2 i_Position, float i_Tolerance = 0.1f)
    {
        if (m_TopLeft == null)
        {
            return false;
        }

        return (m_BottomRight.position.x - i_Position.x < i_Tolerance);
    }

    protected bool IsCloseToTopBorder(Vector2 i_Position, float i_Tolerance = 0.1f)
    {
        if (m_TopLeft == null)
        {
            return false;
        }

        return (m_TopLeft.position.y - i_Position.y < i_Tolerance);
    }

    protected bool IsCloseToBottomBorder(Vector2 i_Position, float i_Tolerance = 0.1f)
    {
        if (m_TopLeft == null)
        {
            return false;
        }

        return (i_Position.y - m_BottomRight.position.y < i_Tolerance);
    }

    protected bool IsOverMidfield(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return false;
        }

        return IsOverMidfield(i_Go.transform);
    }

    protected bool IsOverMidfield(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return false;
        }

        Vector2 position = i_Transform.position;
        return IsOverMidfield(position);
    }

    protected bool IsOverMidfield(Vector2 i_Position)
    {
        if (m_MyGoal == null)
        {
            return false;
        }

        return (i_Position.y > 0f);
    }

    protected bool IsNearToGoal(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return false;
        }

        return IsNearToGoal(i_Go.transform);
    }

    protected bool IsNearToGoal(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return false;
        }

        Vector2 position = i_Transform.position;
        return IsNearToGoal(position);
    }

    protected bool IsNearToGoal(Vector2 i_Position)
    {
        Vector2 myGoalReference = GetFieldPosition(myGoalPosition);

        if (myGoalReference.x < midfield.x) // Left goal.
        {
            return (i_Position.x < (myGoalReference.x + gkAreaWidth));
        }
        else // Right goal.
        {
            return (i_Position.x > (myGoalReference.x - gkAreaWidth));
        }
    }

    protected bool IsNearToOpponentGoal(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return false;
        }

        return IsNearToOpponentGoal(i_Go.transform);
    }

    protected bool IsNearToOpponentGoal(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return false;
        }

        Vector2 position = i_Transform.position;
        return IsNearToOpponentGoal(position);
    }

    protected bool IsNearToOpponentGoal(Vector2 i_Position)
    {
        Vector2 opponentGoalReference = GetFieldPosition(opponentGoalPosition);

        if (opponentGoalReference.x < midfield.x) // Left goal.
        {
            return (i_Position.x < (opponentGoalReference.x + gkAreaWidth));
        }
        else // Right goal.
        {
            return (i_Position.x > (opponentGoalReference.x - gkAreaWidth));
        }
    }

    protected bool IsInMyGKArea(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return false;
        }

        return IsInMyGKArea(i_Go.transform);
    }

    protected bool IsInMyGKArea(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return false;
        }

        Vector2 position = i_Transform.position;
        return IsInMyGKArea(position);
    }

    protected bool IsInMyGKArea(Vector2 i_Position)
    {
        Vector2 myGoalReference = GetFieldPosition(myGoalPosition);

        if (myGoalReference.x < midfield.x) // Left goal.
        {
            return (i_Position.x < (myGoalReference.x + gkAreaWidth)) && (i_Position.y > gkAreaMinHeight && i_Position.y < gkAreaMaxHeight);
        }
        else // Right goal.
        {
            return (i_Position.x > (myGoalReference.x - gkAreaWidth)) && (i_Position.y > gkAreaMinHeight && i_Position.y < gkAreaMaxHeight);
        }
    }

    protected Vector2 GetFieldPosition(Vector2 i_Position)
    {
        // Check left.

        float leftThreshold = midfield.x - halfFieldWidth;

        if (i_Position.x < leftThreshold)
        {
            i_Position.x = leftThreshold;
        }

        // Check right.

        float rightThreshold = midfield.x + halfFieldWidth;

        if (i_Position.x > rightThreshold)
        {
            i_Position.x = rightThreshold;
        }

        // Check top.

        float topThreshold = midfield.y + halfFieldHeight;

        if (i_Position.y > topThreshold)
        {
            i_Position.y = topThreshold;
        }

        // Check bottom.

        float bottomThreshold = midfield.y - halfFieldHeight;

        if (i_Position.y < bottomThreshold)
        {
            i_Position.y = bottomThreshold;
        }

        return i_Position;
    }

    protected Transform GetTeammateNearestTo(GameObject i_Go, bool i_IncludeMySelf = true)
    {
        if (i_Go == null)
        {
            return null;
        }

        return GetTeammateNearestTo(i_Go.transform, i_IncludeMySelf);
    }

    protected Transform GetTeammateNearestTo(Transform i_Transform, bool i_IncludeMySelf = true)
    {
        if (i_Transform == null)
        {
            return null;
        }

        Vector2 position = i_Transform.position;
        return GetTeammateNearestTo(position, i_IncludeMySelf);
    }

    protected Transform GetTeammateNearestTo(Vector2 i_Position, bool i_IncludeMySelf = true)
    {
        Transform nearest = null;
        float minDistance = float.MaxValue;

        for (int teammateIndex = 0; teammateIndex < teamCharactersCount; ++teammateIndex)
        {
            Transform teammate = m_Team[teammateIndex];
            if (teammate != null)
            {
                if (i_IncludeMySelf || (teammate != self.transform))
                {
                    Vector2 teammatePosition = teammate.position;
                    Vector2 toTarget = i_Position - teammatePosition;
                    float distance = toTarget.magnitude;
                    if (distance <= minDistance)
                    {
                        nearest = teammate;
                        minDistance = distance;
                    }
                }
            }
        }

        return nearest;
    }

    protected Transform GetOpponentNearestTo(Vector2 i_Position)
    {
        Transform nearest = null;
        float minDistance = float.MaxValue;

        for (int opponentIndex = 0; opponentIndex < opponentsCount; ++opponentIndex)
        {
            Transform opponent = m_Opponents[opponentIndex];
            if (opponent != null)
            {
                Vector2 opponentPosition = opponent.position;
                Vector2 toTarget = i_Position - opponentPosition;
                float distance = toTarget.magnitude;
                if (distance <= minDistance)
                {
                    nearest = opponent;
                    minDistance = distance;
                }
            }
        }

        return nearest;
    }

    protected Transform GetOpponentNearestTo(GameObject i_Go)
    {
        if (i_Go == null)
        {
            return null;
        }

        return GetOpponentNearestTo(i_Go.transform);
    }

    protected Transform GetOpponentNearestTo(Transform i_Transform)
    {
        if (i_Transform == null)
        {
            return null;
        }

        Vector2 position = i_Transform.position;
        return GetOpponentNearestTo(position);
    }

    protected Transform GetTeammateBehindBallNearestTo(GameObject i_Go, bool i_IncludeMySelf = true)
    {
        if (i_Go == null)
        {
            return null;
        }

        return GetTeammateBehindBallNearestTo(i_Go.transform, i_IncludeMySelf);
    }

    protected Transform GetTeammateBehindBallNearestTo(Transform i_Transform, bool i_IncludeMySelf = true)
    {
        if (i_Transform == null)
        {
            return null;
        }

        Vector2 position = i_Transform.position;
        return GetTeammateBehindBallNearestTo(position, i_IncludeMySelf);
    }

    protected Transform GetTeammateBehindBallNearestTo(Vector2 i_Position, bool i_IncludeMySelf = true)
    {
        Transform nearest = null;
        float minDistance = float.MaxValue;

        for (int teammateIndex = 0; teammateIndex < teamCharactersCount; ++teammateIndex)
        {
            Transform teammate = m_Team[teammateIndex];
            if (teammate != null)
            {
                if (i_IncludeMySelf || (teammate != self.transform))
                {
                    if (IsCharacterBehindBall(teammate))
                    {
                        Vector2 teammatePosition = teammate.position;
                        Vector2 toTarget = i_Position - teammatePosition;
                        float distance = toTarget.magnitude;
                        if (distance < minDistance)
                        {
                            nearest = teammate;
                            minDistance = distance;
                        }
                    }
                }
            }
        }

        return nearest;
    }

    protected Transform GetTeammateByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Teammates.Count)
        {
            return null;
        }

        return m_Teammates[i_Index];
    }

    protected Transform GetOpponentByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Opponents.Count)
        {
            return null;
        }

        return m_Opponents[i_Index];
    }

    protected Vector2 GetMyGoalDirection(GameObject i_From)
    {
        if (i_From == null)
        {
            return Vector2.zero;
        }

        Transform transform = i_From.transform;
        return GetMyGoalDirection(transform);
    }

    protected Vector2 GetMyGoalDirection(Transform i_From)
    {
        if (i_From == null)
        {
            return Vector2.zero;
        }

        Vector2 position = i_From.position;
        return GetMyGoalDirection(position);
    }

    protected Vector2 GetMyGoalDirection(Vector2 i_From)
    {
        if (m_MyGoal == null)
        {
            return Vector2.zero;
        }

        Vector2 toMyGoal = myGoalPosition - i_From;
        Vector2 toMyGoalDirection = toMyGoal.normalized;

        return toMyGoalDirection;
    }

    protected Vector2 GetOpponentGoalDirection(GameObject i_From)
    {
        if (i_From == null)
        {
            return Vector2.zero;
        }

        Transform transform = i_From.transform;
        return GetOpponentGoalDirection(transform);
    }

    protected Vector2 GetOpponentGoalDirection(Transform i_From)
    {
        if (i_From == null)
        {
            return Vector2.zero;
        }

        Vector2 position = i_From.position;
        return GetOpponentGoalDirection(position);
    }

    protected Vector2 GetOpponentGoalDirection(Vector2 i_From)
    {
        if (m_OpponentGoal == null)
        {
            return Vector2.zero;
        }

        Vector2 toOpponentGoal = opponentGoalPosition - i_From;
        Vector2 toOpponentGoalDirection = toOpponentGoal.normalized;

        return toOpponentGoalDirection;
    }

    protected Vector2 GetBallDirection(GameObject i_From)
    {
        if (i_From == null)
        {
            return Vector2.zero;
        }

        Transform transform = i_From.transform;
        return GetBallDirection(transform);
    }

    protected Vector2 GetBallDirection(Transform i_From)
    {
        if (i_From == null)
        {
            return Vector2.zero;
        }

        Vector2 position = i_From.position;
        return GetBallDirection(position);
    }

    protected Vector2 GetBallDirection(Vector2 i_From)
    {
        if (m_Ball == null)
        {
            return Vector2.zero;
        }

        Vector2 toBall = ballPosition - i_From;
        Vector2 toBallDirection = toBall.normalized;

        return toBallDirection;
    }

    protected Vector2 GetSeparationDirection(float i_DistanceThreshold, float i_FleeThreshold = 0f)
    {
        Vector2 sum = Vector2.zero;
        int count = 0;

        for (int teammateIndex = 0; teammateIndex < teammatesCount; ++teammateIndex)
        {
            Transform teammate = m_Teammates[teammateIndex];
            if (teammate != null)
            {
                Vector2 teammatePosition = teammate.position;
                float distance = Vector2.Distance(myPosition, teammatePosition);
                if (distance < i_DistanceThreshold)
                {
                    sum += teammatePosition;
                    ++count;
                }
            }
        }

        if (count > 0)
        {
            Vector2 midPoint = sum / count;
            return Flee(midPoint, i_FleeThreshold);
        }
        else
        {
            return Vector2.zero;
        }
    }

    protected Vector2 GetCohesionDirection(float i_DistanceThreshold, float i_SeekThreshold = 0f)
    {
        Vector2 sum = Vector2.zero;
        int count = 0;

        for (int teammateIndex = 0; teammateIndex < teammatesCount; ++teammateIndex)
        {
            Transform teammate = m_Teammates[teammateIndex];
            if (teammate != null)
            {
                Vector2 teammatePosition = teammate.position;
                float distance = Vector2.Distance(myPosition, teammatePosition);
                if (distance < i_DistanceThreshold)
                {
                    sum += teammatePosition;
                    ++count;
                }
            }
        }

        if (count > 0)
        {
            Vector2 midPoint = sum / count;
            return Seek(midPoint, i_SeekThreshold);
        }
        else
        {
            return Vector2.zero;
        }
    }

    protected float GetColliderRadius(GameObject i_Character)
    {
        if (i_Character == null)
        {
            return 0f;
        }

        return GetColliderRadius(i_Character.transform);
    }

    protected float GetColliderRadius(Transform i_Character)
    {
        if (i_Character == null)
        {
            return 0f;
        }

        /*

        float radius = 0f;

        CircleCollider2D circleCollider2D = i_Character.GetComponent<CircleCollider2D>();
        if (circleCollider2D != null)
        {
            radius = circleCollider2D.radius;
        }

        return radius;

        */

        return 0.3f;
    }

    // tnInputFiller's INTERFACE

    public override void DrawGizmos()
    {

    }

    public override void DrawGizmosSelected()
    {
        // Draw teammates.

        {
            Gizmos.color = Color.green;

            for (int teammateIndex = 0; teammateIndex < m_Teammates.Count; ++teammateIndex)
            {
                Transform teammateTransform = m_Teammates[teammateIndex];
                if (teammateTransform != null)
                {
                    Gizmos.DrawWireSphere(teammateTransform.position, 0.5f);
                }
            }
        }

        // Draw opponents.

        {
            Gizmos.color = Color.red;

            for (int opponentIndex = 0; opponentIndex < m_Opponents.Count; ++opponentIndex)
            {
                Transform opponentTransform = m_Opponents[opponentIndex];
                if (opponentTransform != null)
                {
                    Gizmos.DrawWireSphere(opponentTransform.position, 0.5f);
                }
            }
        }

        // Draw ball.

        {
            Gizmos.color = Color.blue;

            if (m_Ball != null)
            {
                Gizmos.DrawWireSphere(m_Ball.position, 0.5f);
            }
        }

        // Draw Goals.

        {
            Gizmos.color = Color.green;

            if (m_MyGoal != null)
            {
                Gizmos.DrawWireSphere(m_MyGoal.position, 0.1f);
            }
        }

        {
            Gizmos.color = Color.red;

            if (m_OpponentGoal != null)
            {
                Gizmos.DrawWireSphere(m_OpponentGoal.position, 0.1f);
            }
        }

        // Draw field.

        {
            if (m_TopLeft != null && m_BottomRight != null)
            {
                // Left.

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_TopLeft.position.x, m_BottomRight.position.y);

                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Right.

                {
                    Vector2 pointA = new Vector2(m_BottomRight.position.x, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x, m_BottomRight.position.y);

                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Top.

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x, m_TopLeft.position.y);

                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Bottom.

                {
                    Vector2 pointA = new Vector2(m_BottomRight.position.x, m_BottomRight.position.y);
                    Vector2 pointB = new Vector2(m_TopLeft.position.x, m_BottomRight.position.y);

                    Gizmos.color = Color.white;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Left border.

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x + colliderRadius + 0.05f, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_TopLeft.position.x + colliderRadius + 0.05f, m_BottomRight.position.y);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Right border.

                {
                    Vector2 pointA = new Vector2(m_BottomRight.position.x - colliderRadius - 0.05f, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x - colliderRadius - 0.05f, m_BottomRight.position.y);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Top border.

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x, m_TopLeft.position.y - colliderRadius - 0.05f);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x, m_TopLeft.position.y - colliderRadius - 0.05f);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Bottom border.

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x, m_BottomRight.position.y + colliderRadius + 0.05f);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x, m_BottomRight.position.y + colliderRadius + 0.05f);

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Left margin.

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x + colliderRadius + 0.5f, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_TopLeft.position.x + colliderRadius + 0.5f, m_BottomRight.position.y);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Right margin.

                {
                    Vector2 pointA = new Vector2(m_BottomRight.position.x - colliderRadius - 0.5f, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x - colliderRadius - 0.5f, m_BottomRight.position.y);

                    Gizmos.color = Color.grey;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // Goal.

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x, m_GoalMaxHeight);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x, m_GoalMaxHeight);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(pointA, pointB);
                }

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x, m_GoalMinHeight);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x, m_GoalMinHeight);

                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(pointA, pointB);
                }

                // GK Area.

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x, m_GKAreaMaxHeight);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x, m_GKAreaMaxHeight);

                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(pointA, pointB);
                }

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x, m_GKAreaMinHeight);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x, m_GKAreaMinHeight);

                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(pointA, pointB);
                }

                {
                    Vector2 pointA = new Vector2(m_TopLeft.position.x + m_GKAreaWidth, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_TopLeft.position.x + m_GKAreaWidth, m_BottomRight.position.y);

                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(pointA, pointB);
                }

                {
                    Vector2 pointA = new Vector2(m_BottomRight.position.x - m_GKAreaWidth, m_TopLeft.position.y);
                    Vector2 pointB = new Vector2(m_BottomRight.position.x - m_GKAreaWidth, m_BottomRight.position.y);

                    Gizmos.color = Color.cyan;
                    Gizmos.DrawLine(pointA, pointB);
                }
            }
        }

        // Draw midfield.

        {
            Gizmos.color = Color.white;

            if (m_Midfield != null)
            {
                Gizmos.DrawSphere(m_Midfield.position, 0.2f);
            }
        }
    }

    // CTOR

    public tnBaseAIInputFiller(GameObject i_Self)
        : base(i_Self)
    {
        m_Team = new List<Transform>();

        m_Teammates = new List<Transform>();
        m_Opponents = new List<Transform>();

        m_ColliderRadius = GetColliderRadius(self);
    }
}