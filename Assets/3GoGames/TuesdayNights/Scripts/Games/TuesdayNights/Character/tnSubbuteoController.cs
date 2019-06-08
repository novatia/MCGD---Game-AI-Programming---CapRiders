using UnityEngine;

using System;

using TuesdayNights;

using TrueSync;

[RequireComponent(typeof(TSRigidBody2D))]
public class tnSubbuteoController : TrueSyncBehaviour, tnISyncablePlayerInput
{
    // Serializable fields

    [Header("Ranges")]

    [SerializeField]
    private FPRange m_ForceRange = null;
    [SerializeField]
    private FPRange m_PressureTimeRange = null;

    [SerializeField]
    private FPRange m_CooldownRange = null;

    [Header("Input")]

    [SerializeField]
    private bool m_EnableInputCompression = false;

    private static int s_InputPrecision = 32768;

    // Fields

    [AddTracking]
    private TSVector2 m_MoveDirection = TSVector2.zero;

    [AddTracking]
    private FP m_CooldownTimer = FP.Zero;

    [AddTracking]
    private FP m_PressureTime = FP.Zero;
    [AddTracking]
    private FP m_ChargeLevel = FP.Zero;

    [AddTracking]
    private bool m_PrevPressed = false;

    [AddTracking]
    private bool m_RunSyncedUpdate = true;

    private byte m_HorizontalAxisCode = 0;
    private byte m_VerticalAxisCode = 0;
    private byte m_ButtonPressedCode = 0;

    // COMPONENTS

    private TSRigidBody2D m_Rigidbody2D = null;

    private tnCharacterInput m_CharacterInput = null;
    private tnRespawn m_Respawn = null;

    private Animator m_Animator = null;

    // GETTERS

    public TSVector2 moveDirection
    {
        get
        {
            return m_MoveDirection;
        }
    }

    public bool isInCooldown
    {
        get
        {
            return (m_CooldownTimer > FP.Zero);
        }
    }

    public FP chargeLevel
    {
        get
        {
            return m_ChargeLevel;
        }
    }

    public bool isCharging
    {
        get
        {
            return (m_ChargeLevel > FP.Zero);
        }
    }

    public bool runSyncedUpdate
    {
        get { return m_RunSyncedUpdate; }
        set { m_RunSyncedUpdate = value; }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        // Cache components.

        {
            m_Rigidbody2D = GetComponentInChildren<TSRigidBody2D>();

            m_CharacterInput = GetComponentInChildren<tnCharacterInput>();
            m_Respawn = GetComponentInChildren<tnRespawn>();

            m_Animator = GetComponentInChildren<Animator>();
        }

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_SubbuteoController;
    }

    void OnEnable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccurred;
        }
    }

    void OnDisable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccurred;
        }
    }

    void Update()
    {
        // Update animator.

        if (m_Animator != null)
        {
            bool inCooldown = (m_CooldownTimer > FP.Zero);
            m_Animator.SetBool("InCooldown", inCooldown);
        }
    }

        // tnISyncablePlayerInput's interface

    public void SyncedInput(tnInput o_Input)
    {
        float horizontalAxis = m_CharacterInput.GetAxis(InputActions.s_HorizontalAxis);
        float verticalAxis = m_CharacterInput.GetAxis(InputActions.s_VerticalAxis);

        bool buttonPressed = m_CharacterInput.GetButton(InputActions.s_PassButton);

        if (m_EnableInputCompression)
        {
            int intX = (int)(horizontalAxis * s_InputPrecision);
            int intY = (int)(verticalAxis * s_InputPrecision);

            o_Input.SetInt(m_HorizontalAxisCode, intX);
            o_Input.SetInt(m_VerticalAxisCode, intY);
        }
        else
        {
            o_Input.SetFP(m_HorizontalAxisCode, FP.FromFloat(horizontalAxis));
            o_Input.SetFP(m_VerticalAxisCode, FP.FromFloat(verticalAxis));
        }

        o_Input.SetByte(m_ButtonPressedCode, buttonPressed ? (byte)1 : (byte)0);
    }

    // TrueSyncBehaviour's INTERFACE

    public override void OnSyncedStart()
    {
        base.OnSyncedStart();

        tnCharacterInfo characterInfo = GetComponent<tnCharacterInfo>();
        int characterIndex = (characterInfo != null) ? characterInfo.characterIndex : 0;
        int inputBase = characterIndex * 10;

        m_HorizontalAxisCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_HorizontalAxis);
        m_VerticalAxisCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_VerticalAxis);
        m_ButtonPressedCode = Convert.ToByte(inputBase + TrueSyncInputKey.s_Subbuteo_CharacterController_ButtonPressed);
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        if (!m_RunSyncedUpdate)
            return;

        // Get delta time.

        FP deltaTime = TrueSyncManager.deltaTimeMain;

        // Update movement direction.

        FP horizontalAxis;
        FP verticalAxis;

        if (m_EnableInputCompression)
        {
            int intX = TrueSyncInput.GetInt(m_HorizontalAxisCode);
            int intY = TrueSyncInput.GetInt(m_VerticalAxisCode);

            horizontalAxis = intX / (FP)s_InputPrecision;
            verticalAxis = intY / (FP)s_InputPrecision;
        }
        else
        {
            horizontalAxis = TrueSyncInput.GetFP(m_HorizontalAxisCode);
            verticalAxis = TrueSyncInput.GetFP(m_VerticalAxisCode);
        }

        FP horizontalAxisAbs = MathFP.Abs(horizontalAxis);
        FP verticalAxisAbs = MathFP.Abs(verticalAxis);

        TSVector2 moveDirection = new TSVector2(horizontalAxis, verticalAxis);
        moveDirection.Normalize();

        m_MoveDirection = moveDirection;

        // Update player action.

        if (m_CooldownTimer > FP.Zero)
        {
            m_CooldownTimer -= deltaTime;

            if (m_CooldownTimer < FP.Zero)
            {
                m_CooldownTimer = FP.Zero;
            }
        }

        if (m_CooldownTimer == FP.Zero)
        {
            bool buttonPressed = (TrueSyncInput.GetByte(m_ButtonPressedCode) > 0);
            if (buttonPressed)
            {
                m_PressureTime += deltaTime;
                m_ChargeLevel = MathFP.GetClampedPercentage(m_PressureTime, m_PressureTimeRange.min, m_PressureTimeRange.max);
            }
            else
            {
                if (m_PrevPressed)
                {
                    FP axisThreshold = FP.One / FP.Ten;

                    if (horizontalAxisAbs > axisThreshold || verticalAxisAbs > axisThreshold)
                    {
                        // Apply an insant force.

                        FP requestForceMagnitude = MathFP.Lerp(m_ForceRange.min, m_ForceRange.max, m_ChargeLevel);
                        TSVector2 moveForce = m_MoveDirection * requestForceMagnitude;
                        m_Rigidbody2D.AddForce(moveForce);

                        // Update cooldown timer.

                        m_CooldownTimer = MathFP.Lerp(m_CooldownRange.min, m_CooldownRange.max, m_ChargeLevel);
                    }
                    else
                    {
                        m_CooldownTimer = FP.Zero;
                    }
                }

                m_PressureTime = FP.Zero;
                m_ChargeLevel = FP.Zero;
            }

            m_PrevPressed = buttonPressed;
        }
        else
        {
            m_PressureTime = FP.Zero;
            m_ChargeLevel = FP.Zero;

            m_PrevPressed = false;
        }
    }

    // EVENTS

    private void OnRespawnOccurred()
    {
        ForceStop();
    }

    // INTERNALS

    private void ForceStop()
    {
        m_MoveDirection = TSVector2.zero;

        m_CooldownTimer = FP.Zero;

        m_PressureTime = FP.Zero;
        m_ChargeLevel = FP.Zero;

        m_PrevPressed = false;
    }
}