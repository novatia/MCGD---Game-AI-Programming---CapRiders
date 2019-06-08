using BaseMatchEvents;
using UnityEngine;

using TuesdayNights;

[RequireComponent(typeof(tnCharacterInput))]
public class tnRumble : MonoBehaviour
{
    private tnRumbleParams m_Params = null;

    private float m_CurrentInensity = 0f;
    private float m_Timer = 0f;

    private tnCharacterInput m_CharacterInput = null;

    private tnRespawn m_Respawn = null;
    private tnKick m_Kick = null;
    private tnKickable m_Kickable = null;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_CharacterInput = GetComponent<tnCharacterInput>();

        m_Respawn = GetComponent<tnRespawn>();
        m_Kick = GetComponent<tnKick>();
        m_Kickable = GetComponent<tnKickable>();
    }

    void OnEnable()
    {
        m_CharacterInput.StopVibration();

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccurred;
        }

        if (m_Kick != null)
        {
            m_Kick.kickEvent += OnKickOccurred;
        }

        if (m_Kickable != null)
        {
            m_Kickable.kickedEvent += OnKickRecevied;
        }

        Messenger.AddListener<tnGoalEventParams>("ValidatedGoal", OnGoalOccurred);
    }

    void OnDisable()
    {
        m_CharacterInput.StopVibration();

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccurred;
        }

        if (m_Kick != null)
        {
            m_Kick.kickEvent -= OnKickOccurred;
        }

        if (m_Kickable != null)
        {
            m_Kickable.kickedEvent -= OnKickRecevied;
        }

        Messenger.RemoveListener<tnGoalEventParams>("ValidatedGoal", OnGoalOccurred);
    }

    void Update()
    {
        bool rumbleEnabled;
        GameSettings.TryGetBoolMain(Settings.s_UseRumble, out rumbleEnabled);

        if (Time.timeScale == 0f || !rumbleEnabled)
        {
            m_CharacterInput.SetVibration(0f, 0f);
        }
        else
        {
            if (m_Timer > 0f)
            {
                m_Timer -= Time.deltaTime;

                if (m_Timer <= 0f)
                {
                    m_Timer = 0f;

                    m_CurrentInensity = 0f;
                }
            }

            m_CharacterInput.SetVibration(m_CurrentInensity, m_CurrentInensity);
        }
    }

    void OnCollisionEnter2D(Collision2D i_Collision)
    {
        OnCollisionOccurred(i_Collision);
    }

    // BUSINESS LOGIC

    public void SetParams(tnRumbleParams i_Params)
    {
        m_Params = i_Params;
    }

    // INTERNALS

    private void InternalSetVibrationValues(float i_Intensity, float i_Duration)
    {
        if (i_Intensity > m_CurrentInensity)
        {
            m_CurrentInensity = i_Intensity;
            m_Timer = i_Duration;
        }
        else
        {
            // Ignore vibration.
        }
    }

    // EVENTS

    private void OnRespawnOccurred()
    {
        m_CurrentInensity = 0f;
        m_Timer = 0f;
    }
    
    private void OnKickOccurred(tnKickable i_Target)
    {
        if (i_Target == null)
            return;

        if (m_Params == null)
            return;

        if (i_Target.CompareTag(Tags.s_Ball))
        {
            InternalSetVibrationValues(m_Params.kickBallIntensity, m_Params.kickBallDuration);
        }
        else
        {
            if (i_Target.CompareTag(Tags.s_Character))
            {
                InternalSetVibrationValues(m_Params.kickCharacterIntensity, m_Params.kickCharacterDuration);
            }
        }
    }

    private void OnKickRecevied()
    {
        if (m_Params == null)
            return;

        InternalSetVibrationValues(m_Params.kickReceivedIntensity, m_Params.kickReceivedDuration);
    }

    private void OnGoalOccurred(tnGoalEventParams i_Params)
    {
        if (m_Params == null)
            return;

        InternalSetVibrationValues(m_Params.goalIntensity, m_Params.goalDuration);
    }

    private void OnCollisionOccurred(Collision2D i_Collision)
    {
        if (m_Params == null)
            return;

        float relativeVelocity2 = i_Collision.relativeVelocity.sqrMagnitude;

        float minVelocity2 = m_Params.collisionMinVelocity * m_Params.collisionMinVelocity;
        float maxVelocity2 = m_Params.collisionMaxVelocity * m_Params.collisionMaxVelocity;

        float velocityFactor = MathUtils.GetClampedPercentage(relativeVelocity2, minVelocity2, maxVelocity2);

        float intensity = Mathf.Lerp(m_Params.collisionMinIntensity, m_Params.collisionMaxIntensity, velocityFactor);
        float duration = Mathf.Lerp(m_Params.collisionMinDuration, m_Params.collisionMaxDuration, velocityFactor);

        InternalSetVibrationValues(intensity, duration);
    }
}
