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

    public override void Clear()
    {
        m_KickCooldownTimer = 0f;

    }

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {
        //DEFINE LOCAL VARS
        Vector2 axes = Vector2.zero;

        bool requestKick = false;
        bool requestDash = false;
        bool attract = false;

        if (GetMyIndex() == 0)
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
                            if (!m_NextKick)
                            {

                                Vector3 ballDirection = new Vector3(GetBallDirection(self).x, GetBallDirection(self).y,0.0f);
                                Vector3 goalDirection = new Vector3( GetOpponentGoalDirection(self).x, GetOpponentGoalDirection(self).y,0.0f) ;
                                
                                axes = -Seek(Vector2.Perpendicular(goalDirection));

                                Quaternion look = Quaternion.LookRotation(goalDirection, ballDirection);

                                float vertical =360- look.eulerAngles.x;
                                float horizontal =360- look.eulerAngles.y;
                                Debug.Log("H: "+ horizontal+" V: "+ vertical);

                                if (vertical < 4.0f)
                                {
                                    m_NextKick = true;
                                }
                            }
                            else
                            {
                                    axes = Seek(opponentGoalPosition);
                                    requestKick = true;
                                    //set kick after 10ms
                                    m_NextKick = false;
                            }
                        }
                        else
                        {
                            //move along path
                            axes = Seek(opponentGoalPosition);
                            axes.x += 0.15f;
                            attract = true;
                        }
                    }
                    else
                    {
                        //Obstacle avoidance
                        requestKick = true;
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
                Vector2 target = new Vector2(myGoalPosition.x - 2.0f, ballPosition.y);
                axes = Seek(target, colliderRadius);

            }
            else if (IHaveBall()) // io ho la palla
            {
                // "attiro" la palla
                attract = true;
                // mi allontano dalla porta (raggiungo compagno di squadra)
                axes = Seek(GetTeammateByIndex(0).position, colliderRadius);
                // lancio quando la palla è in posizione corretta
                //requestDash = true;
                requestKick = true;
            }
            else if (TheyHaveBall()) // gli avversari hanno la palla
            {
                // mi avvicino alla porta
                axes = Seek(new Vector2(myGoalPosition.x - 2.0f, myGoalPosition.y), colliderRadius);
                // cerco di predire la posizione di tiro e la intercetto
                axes = Interpose(ball, ball);
            }
            else if (IsBallInMyHalfSide())
            {
                axes = Seek(ballPosition, colliderRadius);
                requestKick = true;
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

