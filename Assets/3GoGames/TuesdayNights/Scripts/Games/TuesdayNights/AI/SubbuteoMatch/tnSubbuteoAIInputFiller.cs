using UnityEngine;

using TuesdayNights;

public class tnSubbuteoAIInputFiller : tnSubbuteoAIInputFillerBase
{
    // Types

    private enum State
    {
        Idle,
        Charging,
        Cooldown,
        HeartMovement,
    }

    // Fields

    private State m_State = State.Idle;

    private State m_TargetState = State.Idle;
    private bool m_ChangeStateRequest = false;

    // Idle variables

    private float m_IdleMinWaitingTime = 1f;
    private float m_IdleMinWaitingTimer = 0f;

    private float m_IdleMaxWaitingTime = 15f;
    private float m_IdleMaxWaitingTimer = 0f;

    // Charging variables

    private float m_ChargingTargetLevel = 0f;

    // HeartMovement variables

    private float m_RandomAngle = 0f;

    // tnInputFiller's INTERFACE

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {
        if (!initialized || self == null)
        {
            ResetInputData(i_Data);
            return;
        }

        Vector2 direction = Vector2.zero;
        bool passButton = false;

        UpdateState();
        CallUpdate(m_State, i_FrameTime, out direction, out passButton);

        i_Data.SetAxis(InputActions.s_HorizontalAxis, direction.x);
        i_Data.SetAxis(InputActions.s_VerticalAxis, direction.y);

        i_Data.SetButton(InputActions.s_PassButton, passButton);
    }

    public override void Clear()
    {

    }

    public override void DrawGizmos()
    {
        base.DrawGizmos();
    }

    public override void DrawGizmosSelected()
    {
        base.DrawGizmosSelected();
    }

    // INTERNALS

    private void UpdateState()
    {
        if (m_ChangeStateRequest)
        {
            if (m_State != m_TargetState)
            {
                CallOnExit(m_State);

                m_State = m_TargetState;

                CallOnEnter(m_State);
            }

            m_ChangeStateRequest = false;
        }
    }

    private void SetState(State i_State)
    {
        m_TargetState = i_State;
        m_ChangeStateRequest = true;
    }

    private void CallOnEnter(State i_State)
    {
        switch (i_State)
        {
            case State.Idle:
                OnIdleEnter();
                break;

            case State.Charging:
                OnChargingEnter();
                break;

            case State.Cooldown:
                OnCooldownEnter();
                break;

            case State.HeartMovement:
                OnHeartMovementEnter();
                break;
        }
    }

    private void CallUpdate(State i_State, float i_FrameTime, out Vector2 o_Direction, out bool o_PassButton)
    {
        Vector2 direction = Vector2.zero;
        bool passButton = false;

        switch(i_State)
        {
            case State.Idle:
                IdleUpdate(i_FrameTime, out direction, out passButton);
                break;

            case State.Charging:
                ChargingUpdate(i_FrameTime, out direction, out passButton);
                break;

            case State.Cooldown:
                CooldownUpdate(i_FrameTime, out direction, out passButton);
                break;

            case State.HeartMovement:
                HeartMovementUpdate(i_FrameTime, out direction, out passButton);
                break;
        }

        o_Direction = direction;
        o_PassButton = passButton;
    }

    private void CallOnExit(State i_State)
    {
        switch (i_State)
        {
            case State.Idle:
                OnIdleExit();
                break;

            case State.Charging:
                OnChargingExit();
                break;

            case State.Cooldown:
                OnCooldownExit();
                break;

            case State.HeartMovement:
                OnHeartMovementExit();
                break;
        }
    }

    // Idle

    private void OnIdleEnter()
    {
        m_IdleMinWaitingTimer = m_IdleMinWaitingTime;
        m_IdleMaxWaitingTimer = m_IdleMaxWaitingTime;
    }

    private void IdleUpdate(float i_FrameTime, out Vector2 o_Direction, out bool o_PassButton)
    {
        Vector2 direction = Vector2.zero;
        bool passButton = false;

        m_IdleMinWaitingTimer = Mathf.Max(0f, m_IdleMinWaitingTimer - i_FrameTime);
        m_IdleMaxWaitingTimer = Mathf.Max(0f, m_IdleMaxWaitingTimer - i_FrameTime);

        if (m_IdleMinWaitingTimer == 0f)
        {
            Transform betterPlacedCharacter;
            ComputeBetterPlacedCharacter(out betterPlacedCharacter);

            if (betterPlacedCharacter == self.transform)
            {
                SetState(State.Charging);
            }
            else
            {
                if (m_IdleMaxWaitingTimer == 0f)
                {
                    SetState(State.HeartMovement);
                }
            }
        }

        o_Direction = direction;
        o_PassButton = passButton;
    }

    private void OnIdleExit()
    {

    }

    // Charging

    private void OnChargingEnter()
    {
        m_ChargingTargetLevel = Random.value;
    }

    private void ChargingUpdate(float i_FrameTime, out Vector2 o_Direction, out bool o_PassButton)
    {
        Vector2 direction = Vector2.zero;
        bool passButton = false;

        // Compute direction

        {
            Vector2 target;
            ComputeTarget(out target);

            Vector2 myPosition = self.transform.position;

            Vector2 selfToTarget = target - myPosition;
            Vector2 selfToTargetDirection = selfToTarget.normalized;

            direction = selfToTargetDirection;
        }

        // Compute pass button

        {
            tnSubbuteoController subbuteoController = self.GetComponent<tnSubbuteoController>();
            if (subbuteoController != null)
            {
                float currentChargeLevel = (float)subbuteoController.chargeLevel;
                passButton = (currentChargeLevel < m_ChargingTargetLevel);

                if (!passButton)
                {
                    SetState(State.Cooldown);
                }
            }
        }

        o_Direction = direction;
        o_PassButton = passButton;
    }

    private void OnChargingExit()
    {

    }

    // Cooldown

    private void OnCooldownEnter()
    {

    }

    private void CooldownUpdate(float i_FrameTime, out Vector2 o_Direction, out bool o_PassButton)
    {
        Vector2 direction = Vector2.zero;
        bool passButton = false;

        tnSubbuteoController controller = self.GetComponent<tnSubbuteoController>();
        if (controller != null)
        {
            if (!controller.isInCooldown)
            {
                SetState(State.Idle);
            }
        }

        o_Direction = direction;
        o_PassButton = passButton;
    }

    private void OnCooldownExit()
    {

    }

    // HeartMovement

    private void OnHeartMovementEnter()
    {
        m_ChargingTargetLevel = Random.value;
        m_RandomAngle = Random.value * 360f;
    }

    private void HeartMovementUpdate(float i_FrameTime, out Vector2 o_Direction, out bool o_PassButton)
    {
        Vector2 direction = Vector2.zero;
        bool passButton = false;

        // Compute direction

        {
            direction = Vector2.up;
            direction = direction.Rotate(m_RandomAngle);
        }

        // Compute pass button

        {
            tnSubbuteoController subbuteoController = self.GetComponent<tnSubbuteoController>();
            if (subbuteoController != null)
            {
                float currentChargeLevel = (float)subbuteoController.chargeLevel;
                passButton = (currentChargeLevel < m_ChargingTargetLevel);

                if (!passButton)
                {
                    SetState(State.Cooldown);
                }
            }
        }

        o_Direction = direction;
        o_PassButton = passButton;
    }

    private void OnHeartMovementExit()
    {

    }

    // UTILS

    private void ComputeBetterPlacedCharacter(out Transform o_BetterPlacedCharacter)
    {
        o_BetterPlacedCharacter = null;

        Vector2 t;
        ComputeT(out t);

        Vector2 myGoalPosition = myGoal.position;
        Transform betterPlaced = null;
        float maxDot = 0f;

        Vector2 ballToOpponentGoalDirection = GetDirection(ball.position, opponentGoal.position);

        for (int index = 0; index < teamCharactersCount; ++index)
        {
            Transform character = GetTeamCharacter(index);

            if (character == null)
                continue;

            Vector2 characterPosition = character.position;

            if ((myGoalPosition.x < midfield.x && characterPosition.x < t.x) || (myGoalPosition.x > midfield.x && characterPosition.x > t.x))
            {
                Vector2 characterToOpponentGoalDirection = GetDirection(character.position, opponentGoal.position);

                float dot = Vector2.Dot(ballToOpponentGoalDirection, characterToOpponentGoalDirection);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    betterPlaced = character;
                }
            }
        }

        if (betterPlaced == null)
        {
            Vector2 ballPosition = ball.position;
            float minDistance = float.MaxValue;

            for (int index = 0; index < teamCharactersCount; ++index)
            {
                Transform character = GetTeamCharacter(index);

                if (character == null)
                    continue;

                Vector2 characterPosition = character.position;
                float distance = Vector2.Distance(characterPosition, ballPosition);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    betterPlaced = character;
                }
            }
        }

        o_BetterPlacedCharacter = betterPlaced;
    }

    private void ComputeTarget(out Vector2 o_Target)
    {
        Vector2 target = Vector2.zero;

        Vector2 t;
        ComputeT(out t);

        Vector2 myGoalPosition = myGoal.position;
        Vector2 myPosition = self.transform.position;

        if ((myGoalPosition.x < midfield.x && myPosition.x < t.x) || (myGoalPosition.x > midfield.x && myPosition.x > t.x))
        {
            target = t;
        }
        else
        {
            Vector2 ballPosition = ball.position;

            if (myGoalPosition.x < midfield.x)
            {
                float minX = midfield.x - halfFieldWidth;
                float ballOffesetFromBorder = ballPosition.x - minX;

                target = ballPosition + Vector2.left * 2 * ballOffesetFromBorder;
            }
            else
            {
                if (myGoalPosition.x > midfield.x)
                {
                    float maxX = midfield.x + halfFieldWidth;
                    float ballOffesetFromBorder = maxX - ballPosition.x;

                    target = ballPosition + Vector2.right * 2 * ballOffesetFromBorder;
                }
            }

        }

        o_Target = target;
    }

    private void ComputeT(out Vector2 o_T)
    {
        Vector2 opponentGoalPosition = opponentGoal.position;
        Vector2 ballPosition = ball.position;

        Vector2 ballToOpponentGoalDirection = GetDirection(ballPosition, opponentGoalPosition);

        Vector2 t = ballPosition - ballToOpponentGoalDirection * (0.25f + 0.3f);

        o_T = t;
    }

    private Vector2 GetDirection(Vector2 i_From, Vector2 i_To)
    {
        Vector2 ab = i_To - i_From;
        Vector2 direction = ab.normalized;

        return direction;
    }

    // CTOR

    public tnSubbuteoAIInputFiller(GameObject i_Self)
        : base(i_Self)
    {

    }
}
