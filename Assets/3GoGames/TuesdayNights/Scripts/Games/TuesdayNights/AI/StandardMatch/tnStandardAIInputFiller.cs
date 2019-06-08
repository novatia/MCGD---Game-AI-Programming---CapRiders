using UnityEngine;

using TuesdayNights;

public class tnStandardAIInputFiller : tnStandardAIInputFillerBase
{
    private enum BehaviourMode
    {
        None,               // Grey
        Defender,           // Green
        Striker,            // Red
    }

    private enum BehaviourType
    {
        None,               // Grey
        Wait,               // White
        Recover,            // Yellow
        ChargeBall,         // Red
        SeekAndFlee,        // Green
        ProtectGoal,        // Blue
        Attract,            // Magenta
    }

    private AIRole m_Role = AIRole.Null;

    private BehaviourMode m_BehaviourMode = BehaviourMode.None;
    private BehaviourType m_BehaviourType = BehaviourType.None;

    private bool m_IsAttracting = false;

    private float m_KickCooldownTimer = 0f;
    private float m_DashCooldownTimer = 0f;
    private float m_TackleCooldownTimer = 0f;
    private float m_AttractCooldownTimer = 0f;

    private float m_RecoverTimer = 0f;

    private float m_AttractTimer = 0f;

    private Vector2 m_Axes = Vector2.zero;
    private Vector2 m_AxesSpeed = Vector2.zero;

    // PARAMETERS.

    // Seek-and-flee behaviour.

    private float m_MinFleeDistanceFactor = 0.25f;
    private float m_MaxFleeDistanceFactor = 0.50f;

    // Separation.

    private float m_SeparationThreshold = 3f;

    // Energy thresholds.

    private float m_MinDashEnergy = 0.40f;
    private float m_MinKickEnergy = 0.05f;
    private float m_MinTackleEnergy = 0.50f;
    private float m_MinAttractEnergy = 0.10f;

    // Cooldown timers.

    private float m_DashCooldown = 0.50f;
    private float m_KickCooldown = 0.25f;
    private float m_TackleCooldown = 2.0f;
    private float m_AttractCooldown = 0.5f;

    // Dash behaviour.

    private float m_DashDistance = 3.5f;
    private float m_ForcedDashDistance = 2f;

    // Kick behaviour.

    private float m_KickPrecision = 0.1f;

    // Tackle behaviour.

    private float m_TackleRadius = 0.8f;
    private float m_BallDistanceThreshold = 2f;

    // Attract behaviour.

    private float m_AttractMinRadius = 0.70f;
    private float m_AttractMaxRadius = 0.95f;

    private float m_AttractTimeThreshold = 2f;

    // Extra parameters.

    private float m_RecoverRadius = 1.0f;
    private float m_RecoverTimeThreshold = 1.0f;

    private float m_SmoothTime = 0.0f;

    // STATIC VARIABLES

    private static string s_Params = "Data/AI/StandardMatch/AIParams";

    // tnInputFiller's INTERFACE

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {
        if (!initialized || self == null)
        {
            ResetInputData(i_Data);
            return;
        }

        if (m_Role == AIRole.Null)
        {
            ResetInputData(i_Data);
            return;
        }

        // Clear status.

        SetBehaviourMode(BehaviourMode.None);
        SetBehaviourType(BehaviourType.None);

        // Update timers.

        UpdateCooldownTimers(i_FrameTime);

        UpdateRecoverTimer(i_FrameTime);
        UpdateAttractTimer(i_FrameTime);

        // Update actions.

        Vector2 axes = Vector2.zero;

        bool requestKick = false;
        bool requestDash = false;

        bool attract = false;

        if (UpdateAttract(out axes))
        {
            // You're attracting the ball. Skip regular movement.

            attract = true;
        }
        else
        {
            if (m_Role == AIRole.Midfielder)
            {
                // You're a midfielder. Based on ball position, you can behave as defender or striker.

                if (IsBallInMyHalfSide()) // Ball is in my half side. Behave as defender.
                {
                    UpdateDefender(out axes, out requestKick, out requestDash);

                    SetBehaviourMode(BehaviourMode.Defender);
                }
                else // Ball is on the opponent side. Behave as striker.
                {
                    UpdateStriker(out axes, out requestKick, out requestDash);

                    SetBehaviourMode(BehaviourMode.Striker);
                }
            }
            else
            {
                if (m_Role == AIRole.Defender)
                {
                    UpdateDefender(out axes, out requestKick, out requestDash);

                    SetBehaviourMode(BehaviourMode.Defender);
                }
                else // m_Role == AIRole.Striker
                {
                    UpdateStriker(out axes, out requestKick, out requestDash);

                    SetBehaviourMode(BehaviourMode.Striker);
                }
            }
        }

        // Update action status.

        {
            // Kick

            if (m_KickCooldownTimer == 0f && (CheckEnergy(m_MinKickEnergy)))
            {
                if (requestKick)
                {
                    m_KickCooldownTimer = m_KickCooldown;
                }
            }
            else
            {
                requestKick = false; // Inhibit action.
            }

            // Dash

            if (m_DashCooldownTimer == 0f && (CheckEnergy(m_MinDashEnergy)))
            {
                if (requestDash)
                {
                    m_DashCooldownTimer = m_DashCooldown;
                }
            }
            else
            {
                requestDash = false; // Inhibit action.
            }

            // Tackle

            if (!requestKick)
            {
                if (m_TackleCooldownTimer == 0f && (CheckEnergy(m_MinTackleEnergy)))
                {
                    Transform nearestOpponent;
                    if (UpdateTackle(out nearestOpponent))
                    {
                        requestKick = true;

                        m_TackleCooldownTimer = m_TackleCooldown;
                    }
                }
            }
        }

        if (m_SmoothTime > 0f)
        {
            axes.x = Mathf.SmoothDamp(m_Axes.x, axes.x, ref m_AxesSpeed.x, m_SmoothTime);
            axes.y = Mathf.SmoothDamp(m_Axes.y, axes.y, ref m_AxesSpeed.y, m_SmoothTime);
        }

        m_Axes = axes;

        // Fill input data.

        i_Data.SetAxis(InputActions.s_HorizontalAxis, axes.x);
        i_Data.SetAxis(InputActions.s_VerticalAxis, axes.y);

        i_Data.SetButton(InputActions.s_PassButton, requestKick);
        i_Data.SetButton(InputActions.s_ShotButton, requestDash);

        i_Data.SetButton(InputActions.s_AttractButton, attract);
    }

    public override void Clear()
    {
        m_BehaviourMode = BehaviourMode.None;
        m_BehaviourType = BehaviourType.None;

        m_IsAttracting = false;

        m_KickCooldownTimer = 0f;
        m_DashCooldownTimer = 0f;
        m_TackleCooldownTimer = 0f;
        m_AttractCooldownTimer = 0f;

        m_RecoverTimer = 0f;

        m_AttractTimer = 0f;

        m_Axes = Vector2.zero;
        m_AxesSpeed = Vector2.zero;
    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();

        // Axes.

        Gizmos.color = Color.white;
        Gizmos.DrawLine(myPosition, myPosition + m_Axes);

        // Behaviour Mode.

        switch (m_BehaviourMode)
        {
            case BehaviourMode.Defender:
                Gizmos.color = Color.green;
                break;

            case BehaviourMode.Striker:
                Gizmos.color = Color.red;
                break;
        }

        Gizmos.DrawSphere(myPosition, 0.15f);

        // Behaviour Type.

        switch (m_BehaviourType)
        {
            case BehaviourType.Wait:
                Gizmos.color = Color.white;
                break;

            case BehaviourType.Recover:
                Gizmos.color = Color.yellow;
                break;

            case BehaviourType.ChargeBall:
                Gizmos.color = Color.red;
                break;

            case BehaviourType.SeekAndFlee:
                Gizmos.color = Color.green;
                break;

            case BehaviourType.ProtectGoal:
                Gizmos.color = Color.blue;
                break;

            case BehaviourType.Attract:
                Gizmos.color = Color.magenta;
                break;

            default:
                Gizmos.color = Color.grey;
                break;
        }

        Gizmos.DrawWireSphere(myPosition, colliderRadius * 1.5f);

        /*

        // Separation.

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(myPosition, m_SeparationThreshold);

        // Seek-and-flee.

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(myPosition, m_MinFleeDistanceFactor * fieldHeight);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(myPosition, m_MaxFleeDistanceFactor * fieldHeight);

        */
    }

    public override void DrawGizmosSelected()
    {
        base.DrawGizmosSelected();
    }

    // INTERNALS

    private void UpdateDefender(out Vector2 o_Axes, out bool o_KickButton, out bool o_DashButton)
    {
        o_Axes = Vector2.zero;

        o_KickButton = false;
        o_DashButton = false;

        // You're a defender.

        if (!IsBallInMyHalfSide())
        {
            if (!IsCharacterBehindBall(self))
            {
                // You're a defender and you're not behind the ball. Return through your goal.

                Recover(myGoalPosition, out o_Axes, out o_KickButton, out o_DashButton);

                SetBehaviourType(BehaviourType.Recover);
            }
            else
            {
                Vector2 defenderPosition = referencePosition;

                Vector2 axes = SeekPosition(defenderPosition, colliderRadius); // Ball is on opponent side and you're behind the ball. WAIT.
                o_Axes = axes;

                SetBehaviourType(BehaviourType.Wait);
            }
        }
        else
        {
            // The ball is on your side - your controlled area.

            if (IsCharacterBehindBall(self))
            {
                // I'm behind the ball. If I'm the nearest to the ball, I'll try to kick it away. Otherwise, check if you must protect the goal.

                Transform nearestTeamateToBall = GetTeammateBehindBallNearestTo(ball, true);
                if (nearestTeamateToBall == self.transform)
                {
                    ChargeBall(out o_Axes, out o_KickButton, out o_DashButton);

                    SetBehaviourType(BehaviourType.ChargeBall);
                }
                else
                {
                    // I'm not the nearest player to the ball. Maybe I must protect my goal. Otherwise, seek-and-flee.

                    Transform nearestTeamateToGkArea = GetTeammateBehindBallNearestTo(myGoal, true);
                    if (nearestTeamateToGkArea == self.transform)
                    {
                        // Protect your goal!

                        Vector2 gkPosition;
                        ComputeGkPosition(out gkPosition);

                        o_Axes = Seek(gkPosition, colliderRadius);

                        // If you can kick the ball away, try to do it.

                        float kickRadius = colliderRadius;
                        kickRadius += ballRadius;
                        kickRadius += 0.25f; // Safety threshold.

                        if (ballDistance < kickRadius)
                        {
                            o_KickButton = true;
                        }

                        SetBehaviourType(BehaviourType.ProtectGoal);
                    }
                    else
                    {
                        SeekAndFleeBall(out o_Axes);

                        SetBehaviourType(BehaviourType.SeekAndFlee);
                    }
                }
            }
            else
            {
                // You're a defender and you're not behind the ball. Is your ball inside gk area?

                if (IsNearToGoal(ballPosition))
                {
                    // Ball is inside my gk area. Check if you're the nearest player to the ball.

                    Transform nearestTeammate = GetTeammateNearestTo(ball, true);
                    if (nearestTeammate == self.transform)
                    {
                        // You're the nearest player to the ball. Return through your goal and recover your position.

                        Recover(myGoalPosition, out o_Axes, out o_KickButton, out o_DashButton);

                        SetBehaviourType(BehaviourType.Recover);
                    }
                    else
                    {
                        // You aren't the nearest. Seek-and-flee.

                        SeekAndFleeBall(out o_Axes);

                        SetBehaviourType(BehaviourType.SeekAndFlee);
                    }
                }
                else
                {
                    // Ball is far from your gk area. Return through your goal and recover your position.

                    Recover(myGoalPosition, out o_Axes, out o_KickButton, out o_DashButton);

                    SetBehaviourType(BehaviourType.Recover);
                }
            }
        }
    }

    private void UpdateStriker(out Vector2 o_Axes, out bool o_KickButton, out bool o_DashButton)
    {
        o_Axes = Vector2.zero;

        o_KickButton = false;
        o_DashButton = false;

        // You're a striker.

        if (IsBallInMyHalfSide())
        {
            if (IsCharacterBehindBall(self))
            {
                // You're on my side and behind the ball, so move ahead to maintain an advanced position.

                Recover(opponentGoalPosition, out o_Axes, out o_KickButton, out o_DashButton);

                SetBehaviourType(BehaviourType.Recover);
            }
            else
            {
                Vector2 strikerPosition = referencePosition;
                strikerPosition.x = midfield.x - (referencePosition.x - midfield.x);

                Vector2 axes = SeekPosition(strikerPosition, colliderRadius); // Ball is on your side and you're in an advanced position. WAIT.
                o_Axes = axes;

                SetBehaviourType(BehaviourType.Wait);
            }
        }
        else
        {
            // The ball is on opponent side - your controlled area.

            if (!IsCharacterBehindBall(self))
            {
                // You're not behind the ball, so try to recover a useful position.

                Recover(myGoalPosition, out o_Axes, out o_KickButton, out o_DashButton);

                SetBehaviourType(BehaviourType.Recover);
            }
            else
            {
                // You are behind the ball, so try to attack if you're the nearest player. Otherwise, seek-and-flee.

                Transform nearestTeammate = GetTeammateBehindBallNearestTo(ball, true);
                if (nearestTeammate == self.transform)
                {
                    ChargeBall(out o_Axes, out o_KickButton, out o_DashButton);

                    SetBehaviourType(BehaviourType.ChargeBall);
                }
                else 
                {
                    SeekAndFleeBall(out o_Axes);

                    SetBehaviourType(BehaviourType.SeekAndFlee);
                }
            }
        }
    }

    private bool UpdateAttract(out Vector2 o_Axes)
    {
        o_Axes = Vector2.zero;

        bool isAttracting = false;

        if (CheckEnergy(m_MinAttractEnergy))
        {
            if (m_AttractCooldownTimer == 0f && (m_AttractTimer < m_AttractTimeThreshold))
            {
                if (!IsBehindBall(self) && ballDistance < m_AttractMaxRadius)
                {
                    if (ballDistance < m_AttractMinRadius)
                    {
                        Vector2 attractDirection = Vector2.zero;
                        bool forcedDirection = false;

                        if (myGoalPosition.x < midfield.x)
                        {
                            if (!IsCloseToRightBorder(ball, 0.1f))
                            {
                                if (IsCloseToLeftBorder(ball, 0.1f))
                                {
                                    attractDirection += Vector2.right;
                                    forcedDirection = true;
                                }

                                if (IsCloseToTopBorder(ball, 0.1f))
                                {
                                    attractDirection += Vector2.down;
                                    forcedDirection = true;
                                }

                                if (IsCloseToBottomBorder(ball, 0.1f))
                                {
                                    attractDirection += Vector2.up;
                                    forcedDirection = true;
                                }
                            }
                        }

                        if (myGoalPosition.x > midfield.x)
                        {
                            if (!IsCloseToLeftBorder(ball, 0.1f))
                            {
                                if (IsCloseToRightBorder(ball, 0.1f))
                                {
                                    attractDirection += Vector2.left;
                                    forcedDirection = true;
                                }

                                if (IsCloseToTopBorder(ball, 0.1f))
                                {
                                    attractDirection += Vector2.down;
                                    forcedDirection = true;
                                }

                                if (IsCloseToBottomBorder(ball, 0.1f))
                                {
                                    attractDirection += Vector2.up;
                                    forcedDirection = true;
                                }
                            }
                        }

                        if (!forcedDirection)
                        {
                            Vector2 myGoalDirection = GetMyGoalDirection(self);

                            Vector2 ballDirection = GetBallDirection(self);
                            Vector2 axisA = new Vector2(ballDirection.y, -ballDirection.x);
                            Vector2 axisB = -axisA;

                            float dotA = Vector2.Dot(axisA, myGoalDirection);
                            float dotB = Vector2.Dot(axisB, myGoalDirection);

                            if (dotA > dotB)
                            {
                                attractDirection = axisA;
                            }
                            else // dotB > dotA
                            {
                                attractDirection = axisB;
                            }
                        }

                        o_Axes = attractDirection; // The ball is very close to you, you can start moving with it attached.
                    }
                    else
                    {
                        o_Axes = Vector2.zero;
                    }

                    isAttracting = true;

                    SetBehaviourType(BehaviourType.Attract);
                }
            }
        }

        if (!isAttracting && m_IsAttracting)
        {
            m_AttractCooldownTimer = m_AttractCooldown;
        }

        m_IsAttracting = isAttracting;

        return m_IsAttracting;
    }

    private bool UpdateTackle(out Transform o_Opponent)
    {
        // Try to push away other characters.

        Transform nearestOpponent = GetOpponentNearestTo(self);

        o_Opponent = nearestOpponent;

        bool tackleRequested = false;

        Vector2 nearestOpponetPos = nearestOpponent.position;
        float distance = Vector2.Distance(myPosition, nearestOpponetPos);

        if (distance < m_TackleRadius)
        {
            if (ballDistance > m_BallDistanceThreshold)
            {
                tackleRequested = true;
            }
        }

        return tackleRequested;
    }

    private void UpdateCooldownTimers(float i_FrameTime)
    {
        if (m_KickCooldownTimer > 0f)
        {
            m_KickCooldownTimer -= i_FrameTime;

            if (m_KickCooldownTimer < 0f)
            {
                m_KickCooldownTimer = 0f;
            }
        }

        if (m_DashCooldownTimer > 0f)
        {
            m_DashCooldownTimer -= i_FrameTime;

            if (m_DashCooldownTimer < 0f)
            {
                m_DashCooldownTimer = 0f;
            }
        }

        if (m_TackleCooldownTimer > 0f)
        {
            m_TackleCooldownTimer -= i_FrameTime;

            if (m_TackleCooldownTimer < 0f)
            {
                m_TackleCooldownTimer = 0f;
            }
        }

        if (m_AttractCooldownTimer > 0f)
        {
            m_AttractCooldownTimer -= i_FrameTime;

            if (m_AttractCooldownTimer < 0f)
            {
                m_AttractCooldownTimer = 0f;
            }
        }
    }

    private void UpdateRecoverTimer(float i_FrameTime)
    {
        float ballSpeed = GetVehicleSpeed(ball);
        float mySpeed = GetVehicleSpeed(self);

        if (ballDistance < m_RecoverRadius && (ballSpeed < 0.15f && mySpeed < 0.15f))
        {
            m_RecoverTimer += i_FrameTime;
        }
        else
        {
            m_RecoverTimer = 0f;
        }
    }

    private void UpdateAttractTimer(float i_FrameTime)
    {
        if (m_IsAttracting)
        {
            m_AttractTimer += i_FrameTime;
        }
        else
        {
            m_AttractTimer = 0f;
        }
    } 

    // UTILS

    private void SeekAndFleeBall(out Vector2 o_Axes)
    {
        Vector2 target;
        ComputeFieldPosition(out target);

        o_Axes = Seek(target, colliderRadius);
    }

    private Vector2 SeekPosition(Vector2 i_Position, float i_Tolerance)
    {
        Vector2 target;
        ComputeFieldPosition(out target);

        target += i_Position;
        target *= 0.5f;

        Vector2 axes = Seek(target, i_Tolerance);
        return axes;
    }

    private void ComputeFieldPosition(out Vector2 o_Position)
    {
        Vector2 target = Vector2.zero;

        // Compute sepration.

        {
            Vector2 separationTarget;
            ComputeSeparationTarget(out separationTarget);

            target += separationTarget;
        }

        // Seek/Flee ball.

        {
            float fleeMinDistance = fieldHeight * m_MinFleeDistanceFactor;
            if (ballDistance < fleeMinDistance)
            {
                Vector2 seekTarget = myPosition;
                ComputeFleeTarget(ballPosition, fleeMinDistance, out seekTarget);

                target += seekTarget;
                target *= 0.5f;
            }
            else
            {
                float fleeMaxDistance = fieldHeight * m_MaxFleeDistanceFactor;
                if (ballDistance > fleeMaxDistance)
                {
                    Vector2 seekTarget = myPosition;
                    ComputeSeekTarget(ballPosition, fleeMaxDistance, out seekTarget);

                    target += seekTarget;
                    target *= 0.5f;
                }
            }
        }

        o_Position = target;
    }

    private void ComputeSeekTarget(Vector2 i_Source, float i_Threshold, out Vector2 o_Target)
    {
        Vector2 selfToSource = i_Source - myPosition;
        float distanceToSource = selfToSource.magnitude;

        Vector2 selfToSourceDirection = selfToSource.normalized;

        o_Target = myPosition + selfToSourceDirection * (distanceToSource - i_Threshold);
    }

    private void ComputeFleeTarget(Vector2 i_Source, float i_Threshold, out Vector2 o_Target)
    {
        Vector2 sourceToSelfDirection = myPosition - i_Source;
        sourceToSelfDirection.Normalize();

        o_Target = i_Source + sourceToSelfDirection * i_Threshold;
    }

    private void ComputeSeparationTarget(out Vector2 o_Target)
    {
        Vector2 sum = Vector2.zero;
        int count = 0;

        for (int teammateIndex = 0; teammateIndex < teammatesCount; ++teammateIndex)
        {
            Transform teammate = GetTeammateByIndex(teammateIndex);
            if (teammate != null)
            {
                Vector2 teammatePosition = teammate.position;
                Vector2 teammateToSelf = myPosition - teammatePosition;

                float distance = teammateToSelf.magnitude;

                if (distance < m_SeparationThreshold)
                {
                    Vector2 teammateToSelfDirection = teammateToSelf.normalized;

                    Vector2 target = teammatePosition + teammateToSelfDirection * m_SeparationThreshold;
                    sum += target;

                    ++count;
                }
            }
        }

        if (count > 0)
        {
            Vector2 separationTarget = sum / count;
            o_Target = separationTarget;
        }
        else
        {
            o_Target = myPosition;
        }
    }

    private void ComputeGkPosition(out Vector2 o_Position)
    {
        Vector2 gkPosition = GetFieldPosition(myGoalPosition);

        float idealHeight = ballPosition.y;

        Vector2 ballVelocity = GetVehicleVelocity(ball);
        if (ballVelocity.magnitude > 0.1f)
        {
            Vector2 ballDirection = ballVelocity.normalized;
            Vector2 ballToGoalDirection = GetMyGoalDirection(ball);

            float dot = Vector2.Dot(ballDirection, ballToGoalDirection);

            if (dot > 0f)
            {
                idealHeight = ballPosition.y + (gkPosition.x - ballPosition.x) * (ballVelocity.y / ballVelocity.x);
            }
        }

        float goalMinHeight = gkPosition.y - goalWidth / 2f;
        float goalMaxHeight = gkPosition.y + goalWidth / 2f;

        idealHeight = Mathf.Clamp(idealHeight, goalMinHeight, goalMaxHeight);

        if (gkPosition.x > midfield.x) // Right goal.
        {
            gkPosition.x -= colliderRadius;
            gkPosition.x -= 0.15f;
        }
        else // Left goal.
        {
            gkPosition.x += colliderRadius;
            gkPosition.x += 0.15f;
        }

        gkPosition.y = idealHeight;

        o_Position = gkPosition;
    }

    private void Recover(Vector2 i_Target, out Vector2 o_Axes, out bool o_KickButton, out bool o_DashButton)
    {
        Vector2 targetDistance = i_Target - myPosition;

        Vector2 targetDirection = targetDistance.normalized;

        {
            // Apply direction correction in order to avoid the ball.

            Vector2 pointA = ballPosition;
            pointA.y += ballRadius;
            pointA.y += colliderRadius;
            pointA.y += 0.05f; // Safety tolerance.

            Vector2 pointB = ballPosition;
            pointB.y -= ballRadius;
            pointB.y -= colliderRadius;
            pointB.y -= 0.05f; // Safety tolerance.

            Vector2 selfToA = pointA - myPosition;
            Vector2 selfToB = pointB - myPosition;

            Vector2 directionA = selfToA.normalized;
            Vector2 directionB = selfToB.normalized;

            float angleA = Vector2.Angle(targetDirection, directionA);
            float angleB = Vector2.Angle(targetDirection, directionB);

            float angleAB = Vector2.Angle(directionA, directionB);

            if (angleA < angleAB && angleB < angleAB)
            {
                // Target direction is into ball fov. Apply a correction using the shortest line.

                if (selfToA.sqrMagnitude < selfToB.sqrMagnitude) // A is the nearest point.
                {
                    targetDirection = directionA;
                }
                else // B is the nearest point.
                {
                    targetDirection = directionB;
                }
            }
            else
            {
                // Target direction is fine. Nothing to do.
            }
        }

        o_Axes = targetDirection;

        o_KickButton = false;

        if (targetDistance.sqrMagnitude > m_ForcedDashDistance * m_ForcedDashDistance)
        {
            // Force dash movement, to recover your position quickly.

            o_DashButton = true;
        }
        else
        {
            o_DashButton = false;
        }
    }

    private void ChargeBall(out Vector2 o_Axes, out bool o_KickButton, out bool o_DashButton)
    {
        o_Axes = Vector2.zero;

        o_KickButton = false;
        o_DashButton = false;

        Vector2 target = ballPosition;

        Vector2 direction = ballPosition - opponentGoalPosition;
        direction.Normalize();

        float offset = ballRadius;
        offset += colliderRadius;
        offset += 0.05f; // Tolerance threshold.

        target += direction * offset;

        target = ClampPosition(target, 0.2f);

        o_Axes = Seek(target);

        if (m_RecoverTimer > m_RecoverTimeThreshold)
        {
            o_KickButton = true; // Force kick to recover.
        }
        else
        {
            float targetDistance = Vector2.Distance(target, myPosition);

            if (targetDistance > m_DashDistance)
            {
                o_DashButton = true;
            }
            else
            {
                if (targetDistance < m_KickPrecision)
                {
                    o_KickButton = true;
                }
            }
        }
    }

    private Vector2 ClampPosition(Vector2 i_Position, float i_Tolerance = 0f)
    {
        // Check left.

        float leftThreshold = midfield.x - halfFieldWidth;
        leftThreshold += i_Tolerance;

        if (i_Position.x < leftThreshold)
        {
            i_Position.x = leftThreshold;
        }

        // Check right.

        float rightThreshold = midfield.x + halfFieldWidth;
        rightThreshold -= i_Tolerance;

        if (i_Position.x > rightThreshold)
        {
            i_Position.x = rightThreshold;
        }

        // Check top.

        float topThreshold = midfield.y + halfFieldHeight;
        topThreshold -= i_Tolerance;

        if (i_Position.y > topThreshold)
        {
            i_Position.y = topThreshold;
        }

        // Check bottom.

        float bottomThreshold = midfield.y - halfFieldHeight;
        bottomThreshold += i_Tolerance;

        if (i_Position.y < bottomThreshold)
        {
            i_Position.y = bottomThreshold;
        }

        return i_Position;
    }

    private void SetBehaviourMode(BehaviourMode i_Mode)
    {
        m_BehaviourMode = i_Mode;
    }

    private void SetBehaviourType(BehaviourType i_Type)
    {
        m_BehaviourType = i_Type;
    }

    // CTOR

    public tnStandardAIInputFiller(GameObject i_Self, AIRole i_Role)
        : base(i_Self)
    {
        m_Role = i_Role;

        // Setup parameters.

        tnStandardAIInputFillerParams aiParams = Resources.Load<tnStandardAIInputFillerParams>(s_Params);

        if (aiParams != null)
        {
            // Seek-and-flee behaviour.

            aiParams.GetMinFleeDistanceFactor(out m_MinFleeDistanceFactor);
            aiParams.GetMaxFleeDistanceFactor(out m_MaxFleeDistanceFactor);

            // Separation.

            aiParams.GetSeparationThreshold(out m_SeparationThreshold);

            // Energy thresholds.

            aiParams.GetMinDashEnergy(out m_MinDashEnergy);
            aiParams.GetMinKickEnergy(out m_MinKickEnergy);
            aiParams.GetMinTackleEnergy(out m_MinTackleEnergy);
            aiParams.GetMinAttractEnergy(out m_MinAttractEnergy);

            // Cooldown timers.

            aiParams.GetDashCooldown(out m_DashCooldown);
            aiParams.GetKickCooldown(out m_KickCooldown);
            aiParams.GetTackleCooldown(out m_TackleCooldown);
            aiParams.GetAttractCooldown(out m_AttractCooldown);

            // Dash behaviour.

            aiParams.GetDashDistance(out m_DashDistance);
            aiParams.GetForcedDashDistance(out m_ForcedDashDistance);

            // Kick behaviour.

            aiParams.GetKickPrecision(out m_KickPrecision);

            // Tackle behaviour.

            aiParams.GetTackleRadius(out m_TackleRadius);
            aiParams.GetBallDistanceThreshold(out m_BallDistanceThreshold);

            // Attract behaviour.

            aiParams.GetAttractMinRadius(out m_AttractMinRadius);
            aiParams.GetAttractMaxRadius(out m_AttractMaxRadius);

            aiParams.GetAttractTimeThreshold(out m_AttractTimeThreshold);

            // Extra parameters.

            aiParams.GetRecoverRadius(out m_RecoverRadius);
            aiParams.GetRecoverTimeThreshold(out m_RecoverTimeThreshold);
        }
    }
}