using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TuesdayNights;
using System;

public class mcgd201819AIInputFiller : tnStandardAIInputFillerBase
{
    private float m_KickCooldownTimer = 0f;
    private float m_DashCooldownTimer = 0f;
    private float m_TackleCooldownTimer = 0f;
    private float m_AttractCooldownTimer = 0f;

    private float m_RecoverTimer = 0f;

    private float m_AttractTimer = 0f;

    private float m_MinDashEnergy = 0.40f;
    private float m_MinKickEnergy = 0.05f;
    private float m_MinTackleEnergy = 0.50f;

    private float m_KickCooldown = 0.25f;
    private float m_DashCooldown = 0.50f;
    private float m_TackleCooldown = 2.0f;

    bool m_NextKick = false;
    int m_NextKickRoundCount = 3;

    private float m_SmoothTime = 0.0f;
    private Vector2 m_Axes = Vector2.zero;
    private Vector2 m_AxesSpeed = Vector2.zero;

    private float m_RecoverRadius = 1.0f;
    private bool m_IsAttracting = false;

    private float m_AttractMinRadius = 0.70f;
    private float m_AttractMaxRadius = 0.95f;

    private float m_AttractTimeThreshold = 2f;
    private float m_MinAttractEnergy = 0.10f;
    private float m_AttractCooldown = 0.5f;

    private float m_ForcedDashDistance = 2f;
    private float m_RecoverTimeThreshold = 1.0f;
    private float m_KickPrecision = 0.1f;
    private float m_DashDistance = 3.5f;



    public override void Clear()
    {
        m_KickCooldownTimer = 0f;

    }

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {

        // Update timers.

        UpdateCooldownTimers(i_FrameTime);

        UpdateRecoverTimer(i_FrameTime);
        UpdateAttractTimer(i_FrameTime);

        //DEFINE LOCAL VARS
        Vector2 axes = Vector2.zero;

        bool requestKick = false;
        bool requestDash = false;
        bool attract = false;
        if (UpdateAttract(out axes))
        {
            // You're attracting the ball. Skip regular movement.

            attract = true;
        }
        else if (GetMyIndex() == 0)
        {
            //ATTACK
            if (WeHaveBall())
            {
                //we have ball
                if (IHaveBall())
                {
                    //SELECT GOOD PATH TO GOAL
                    if (PathIsFree())
                    {
                        if (CanIShoot())
                        {
                            //bring ball to right direction 
                            ChargeBall(out axes, out requestKick, out requestDash);
                        }
                        else
                        {
                            //move along path
                            axes = Seek(opponentGoalPosition);
                            axes.x += 0.15f;
                            //attract = true;
                        }
                    }
                    else
                    {
                        //Obstacle avoidance
                        ChargeBall(out axes, out requestKick, out requestDash);
                    }
                }
                else
                {
                    //supporter
                    //KEEP CONSTANT DINSTANCE
                    for (int i = 0; i < teamCharactersCount; i++)
                    {
                        if (GetTeammateByIndex(i) != self)
                        {
                            axes = Seek(GetTeammateByIndex(i).gameObject.transform, colliderRadius);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (TheyHaveBall())
                {
                    //they have ball
                    if (GoalIsDistant())
                    {
                        //Goto ball
                        axes = Seek(ballPosition, colliderRadius);
                    }
                    else
                    {
                        //Goto to mygoal
                        axes = Seek(myGoalPosition, colliderRadius);
                    }
                }
                else
                {
                    //NOBODY HAVE BALL
                    //SELECT GOOD PATH TO BALL
                    requestDash = true;
                    axes = Seek(ballPosition);
                }

            }

            
        }
        else
        {
            //DEFEND
            if (WeHaveBall() && !IHaveBall()) //il mio compagno ha la palla
            {
                // a distanza fissa dalla porta e segue la palla come posizione y
                Recover(myGoalPosition, out axes, out requestKick, out requestDash);
                //axes = Seek(target, colliderRadius);

            }
            else if (IHaveBall()) // io ho la palla
            {
                // "attiro" la palla
                //attract = true;
                // mi allontano dalla porta (raggiungo compagno di squadra)
                axes = Seek(GetTeammateByIndex(0).position, colliderRadius);
                // lancio quando la palla è in posizione corretta
                //requestDash = true;
                ChargeBall(out axes, out requestKick, out requestDash);
            }
            else if (TheyHaveBall()) // gli avversari hanno la palla
            {
                // mi avvicino alla porta
                axes = Seek(new Vector2(myGoalPosition.x - 2.0f, myGoalPosition.y), colliderRadius);
                // cerco di predire la posizione di tiro e la intercetto
                axes = Interpose(ball, ball);
                Recover(myGoalPosition, out axes, out requestKick, out requestDash);
            }
            else if (IsBallInMyHalfSide())
            {
                axes = Seek(ballPosition, colliderRadius);
                requestKick = true;
                ChargeBall(out axes, out requestKick, out requestDash);
            }
            else
            {
                axes = Seek(new Vector2(myGoalPosition.x - 2.0f, myGoalPosition.y), colliderRadius);
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
            /*
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
            */
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

    // CTOR
    public mcgd201819AIInputFiller(GameObject i_Self)
        : base(i_Self)
    {

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

}

public class Test1AIFactory : tnBaseStandardMatchAIFactory
{
    private List<AIRole> m_Roles = null;
    private int m_AICreated = 0;

    private static AIRole s_DefaultRole = AIRole.Midfielder;

    protected override void OnConfigure(tnTeamDescription i_TeamDescription)
    {
        base.OnConfigure(i_TeamDescription);

        //ADD BEHAVIOUR TREE INSTANCE

    }

    protected override tnStandardAIInputFillerBase OnCreateAI(int i_Index, GameObject i_Character)
    {
        if (m_Roles.Count == 0 || m_AICreated >= m_Roles.Count)
        {
            return CreateInputFiller(s_DefaultRole, i_Character);
        }

        AIRole role = m_Roles[m_AICreated++];
        return CreateInputFiller(role, i_Character);
    }

    public Test1AIFactory()
        : base()
    {
        m_Roles = new List<AIRole>();
    }

    private tnStandardAIInputFillerBase CreateInputFiller(AIRole i_Role, GameObject i_Character)
    {
        //return new tnStandardAIInputFiller(i_Character, i_Role);
        return new mcgd201819AIInputFiller(i_Character);
    }
}

