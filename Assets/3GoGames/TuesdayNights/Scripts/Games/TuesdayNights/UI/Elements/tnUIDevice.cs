using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum DeviceState
{
    Invalid = 0,
    Disabled = 1,
    Left = 2,
    Center = 3,
    Right = 4,
}

[RequireComponent(typeof(Image))]
public abstract class tnUIDevice : MonoBehaviour
{
    [SerializeField]
    private float m_DisabledAlpha = 0.2f;

    [SerializeField]
    private float m_TransitionTime = 0.5f;

    [Serializable]
    public class MoveEvent : UnityEvent { }

    [Serializable]
    public class Action1Event : UnityEvent { }
    [Serializable]
    public class Action2Event : UnityEvent { }

    [SerializeField]
    private MoveEvent m_OnMove = new MoveEvent();
    public MoveEvent onMOve { get { return m_OnMove; } set { m_OnMove = value; } }

    [SerializeField]
    private Action1Event m_OnAction1 = new Action1Event();
    public Action1Event onAction1 { get { return m_OnAction1; } set { m_OnAction1 = value; } }

    [SerializeField]
    private Action2Event m_OnAction2 = new Action2Event();
    public Action2Event onAction2 { get { return m_OnAction2; } set { m_OnAction2 = value; } }

    private int m_PlayerId = Hash.s_NULL;

    private string m_PlayerName = "";

    private tnUITeam m_TeamA = null;
    private tnUITeam m_TeamB = null;
    private Image m_Image = null;

    private ControllerAnchor m_DefaultAnchor = null;

    private GridEntry m_GridEntry = null;

    private bool m_IsInTransition = false;

    private Vector3 m_TargetPosition = Vector3.zero;
    private Vector3 m_StartPosition = Vector3.zero;
    private float m_Timer = 0f;

    private DeviceState m_CurrentState = DeviceState.Invalid;

    private bool m_HasFocus = true;

    public int playerId
    {
        get { return m_PlayerId; }
    }

    public string playerName
    {
        get
        {
            return m_PlayerName;
        }
    }

    public bool isInTransition
    {
        get { return m_IsInTransition; }
    }

    public bool hasFocus
    {
        get
        {
            return m_HasFocus;
        }

        set
        {
            m_HasFocus = value;
        }
    }

    public DeviceState deviceState
    {
        get { return m_CurrentState; }
    }

    // ABSTRACT

    protected abstract bool isInputActive { get; }

    protected abstract void InternalBindToPlayer(string i_PlayerName);

    protected abstract bool InternalGetProceedButton();
    protected abstract bool InternalGetCancelButton();

    protected abstract bool InternalGetRightButton();
    protected abstract bool InternalGetLeftButton();

    protected abstract bool InternalGetAddBotButton();
    protected abstract bool InternalGetRemoveBotButton();

    // VIRTUALS

    protected virtual void OnStateExit(DeviceState i_OldState)
    {

    }

    protected virtual void OnStateEnter(DeviceState i_OldState, DeviceState i_NewState)
    {

    }

    protected virtual void OnDeactivate()
    {

    }

    protected virtual void OnActivate()
    {

    }

    // MonoBehaviour's interface

    protected virtual void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    protected virtual void OnEnable()
    {
        Deactivate();
    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void Update()
    {
        if (!isInputActive)
        {
            Deactivate();
            return;
        }

        if (m_CurrentState == DeviceState.Disabled)
        {
            Activate();
        }

        if (!hasFocus)
        {
            return;
        }

        if (!m_IsInTransition)
        {
            if (m_TeamA == null || m_TeamB == null)
                return;

            bool right = GetRightButton();
            bool left = GetLeftButton();

            if (right)
            {
                MoveRight();

                if (m_OnMove != null)
                {
                    m_OnMove.Invoke();
                }
            }
            else
            {
                if (left)
                {
                    MoveLeft();

                    if (m_OnMove != null)
                    {
                        m_OnMove.Invoke();
                    }
                }
            }

            bool addBotButton = GetAddBotButton();
            bool removeBotButton = GetRemoveBotButton();

            if (addBotButton)
            {
                if (m_CurrentState == DeviceState.Left)
                {
                    if (AddBotToLeft())
                    {
                        if (m_OnAction1 != null)
                        {
                            m_OnAction1.Invoke();
                        }
                    }
                }
                else
                {
                    if (m_CurrentState == DeviceState.Right)
                    {
                        if (AddBotToRight())
                        {
                            if (m_OnAction1 != null)
                            {
                                m_OnAction1.Invoke();
                            }
                        }
                    }
                }
            }

            if (removeBotButton)
            {
                if (m_CurrentState == DeviceState.Left)
                {
                    if (RemoveBotFromLeft())
                    {
                        if (m_OnAction2 != null)
                        {
                            m_OnAction2.Invoke();
                        }
                    }
                }
                else
                {
                    if (m_CurrentState == DeviceState.Right)
                    {
                        if (RemoveBotFromRight())
                        {
                            if (m_OnAction2 != null)
                            {
                                m_OnAction2.Invoke();
                            }
                        }
                    }
                }
            }
        }
        else
        {
            m_Timer += Time.deltaTime;
            float percentage = Mathf.Clamp01(m_Timer / m_TransitionTime);
            Vector3 pos = Vector3.Lerp(m_StartPosition, m_TargetPosition, percentage);
            rectTransform.position = pos;

            if (m_Timer > m_TransitionTime)
            {
                rectTransform.position = m_TargetPosition;
                m_IsInTransition = false;
            }
        }
    }

    // PROTECTED

    protected RectTransform rectTransform
    {
        get
        {
            return (RectTransform)transform;
        }
    }

    protected Vector3 defaultPosition
    {
        get
        {
            if (m_DefaultAnchor != null && m_DefaultAnchor.rectTransform != null)
            {
                return m_DefaultAnchor.rectTransform.position;
            }

            return Vector3.zero;
        }
    }

    protected void SetImage(Sprite i_Sprite)
    {
        m_Image.sprite = i_Sprite;
    }

    public bool GetProceedButton()
    {
        if (!isInputActive)
        {
            return false;
        }

        if (!hasFocus)
        {
            return false;
        }

        return InternalGetProceedButton();
    }

    public bool GetCancelButton()
    {
        if (!isInputActive)
        {
            return false;
        }

        if (!hasFocus)
        {
            return false;
        }

        return InternalGetCancelButton();
    }

    public bool GetRightButton()
    {
        if (!isInputActive)
        {
            return false;
        }

        if (!hasFocus)
        {
            return false;
        }

        return InternalGetRightButton();
    }

    public bool GetLeftButton()
    {
        if (!isInputActive)
        {
            return false;
        }

        if (!hasFocus)
        {
            return false;
        }

        return InternalGetLeftButton();
    }

    public bool GetAddBotButton()
    {
        if (!isInputActive)
        {
            return false;
        }

        if (!hasFocus)
        {
            return false;
        }

        return InternalGetAddBotButton();
    }

    public bool GetRemoveBotButton()
    {
        if (!isInputActive)
        {
            return false;
        }

        if (!hasFocus)
        {
            return false;
        }

        return InternalGetRemoveBotButton();
    }

    // LOGIC

    public void SetPlayerId(string i_PlayerId)
    {
        int hash = StringUtils.GetHashCode(i_PlayerId);
        SetPlayerId(hash);
    }

    public void SetPlayerId(int i_PlayerId)
    {
        m_PlayerId = i_PlayerId;
    }

    public void SetPlayerName(string i_PlayerName)
    {
        m_PlayerName = i_PlayerName;

        InternalBindToPlayer(i_PlayerName);
    }

    public void SetDefaultAnchor(ControllerAnchor i_Anchor)
    {
        m_DefaultAnchor = i_Anchor;

        ResetPosition();
    }

    public void ResetPosition()
    {
        rectTransform.position = defaultPosition;

        if (m_DefaultAnchor != null)
        {
            rectTransform.sizeDelta = new Vector2(m_DefaultAnchor.width, m_DefaultAnchor.height);
        }
    }

    public void SetColor(Color i_Color, bool i_IgnoreAlpha = false)
    {
        float currentAlpha = m_Image.color.a;
        float targetAlpha = (i_IgnoreAlpha) ? currentAlpha : i_Color.a;
        Color targetColor = new Color(i_Color.r, i_Color.g, i_Color.b, targetAlpha);
        m_Image.color = targetColor;
    }

    public void SetColorAlpha(float i_Alpha)
    {
        float alpha = Mathf.Clamp01(i_Alpha);
        Color current = m_Image.color;
        Color newColor = new Color(current.r, current.g, current.b, alpha);
        SetColor(newColor);
    }

    public void SetTeamsManagers(tnUITeam i_TeamA, tnUITeam i_TeamB)
    {
        m_TeamA = i_TeamA;
        m_TeamB = i_TeamB;
    }

    // INTERNALS

    private void SetState(DeviceState i_State)
    {
        OnStateExit(m_CurrentState);

        DeviceState old = m_CurrentState;
        m_CurrentState = i_State;

        OnStateEnter(old, m_CurrentState);
    }

    private GridEntry GetTeamAFirstAvailableEntry()
    {
        if (m_TeamA != null)
        {
            return m_TeamA.GetFirstAvailableEntry();
        }

        return null;
    }

    private GridEntry GetTeamBFirstAvailableEntry()
    {
        if (m_TeamB != null)
        {
            return m_TeamB.GetFirstAvailableEntry();
        }

        return null;
    }

    private void MoveLeft()
    {
        if (m_CurrentState == DeviceState.Left || m_CurrentState == DeviceState.Disabled)
            return;

        if (m_CurrentState == DeviceState.Center)
        {
            GridEntry gridEntry = GetTeamAFirstAvailableEntry();
            if (gridEntry != null)
            {
                ControllerAnchor controllerAnchor = gridEntry.controllerAnchor;

                if (controllerAnchor == null)
                    return;

                RectTransform targetTransform = controllerAnchor.rectTransform;
                if (targetTransform != null)
                {
                    m_GridEntry = gridEntry;

                    gridEntry.device = this;
                    m_CurrentState = DeviceState.Left;

                    m_TargetPosition = targetTransform.position;
                    m_StartPosition = rectTransform.position;

                    m_Timer = 0f;

                    m_IsInTransition = true;
                }
            }
        }
        else
        {
            if (m_CurrentState == DeviceState.Right)
            {
                m_GridEntry.device = null;
                m_GridEntry = null;

                m_CurrentState = DeviceState.Center;

                m_TargetPosition = defaultPosition;
                m_StartPosition = rectTransform.position;

                m_Timer = 0f;

                m_IsInTransition = true;
            }
        }
    }

    private void MoveRight()
    {
        if (m_CurrentState == DeviceState.Right || m_CurrentState == DeviceState.Disabled)
            return;

        if (m_CurrentState == DeviceState.Center)
        {
            GridEntry gridEntry = GetTeamBFirstAvailableEntry();
            if (gridEntry != null)
            {
                ControllerAnchor controllerAnchor = gridEntry.controllerAnchor;

                if (controllerAnchor == null)
                    return;

                RectTransform targetTransform = controllerAnchor.rectTransform;
                if (targetTransform != null)
                {
                    m_GridEntry = gridEntry;

                    gridEntry.device = this;
                    m_CurrentState = DeviceState.Right;

                    m_TargetPosition = targetTransform.position;
                    m_StartPosition = rectTransform.position;

                    m_Timer = 0f;

                    m_IsInTransition = true;
                }
            }
        }
        else
        {
            if (m_CurrentState == DeviceState.Left)
            {
                m_GridEntry.device = null;
                m_GridEntry = null;

                m_CurrentState = DeviceState.Center;

                m_TargetPosition = defaultPosition;
                m_StartPosition = rectTransform.position;

                m_Timer = 0f;

                m_IsInTransition = true;
            }
        }
    }

    private void Activate()
    {
        if (m_CurrentState != DeviceState.Disabled)
            return;

        SetState(DeviceState.Center);
        SetColorAlpha(1f);

        ResetPosition();

        m_IsInTransition = false;

        OnActivate();
    }

    private void Deactivate()
    {
        if (m_CurrentState == DeviceState.Disabled)
            return;

        SetState(DeviceState.Disabled);
        SetColorAlpha(m_DisabledAlpha);

        ResetPosition();

        m_IsInTransition = false;

        if (m_GridEntry != null)
        {
            m_GridEntry.device = null;
            m_GridEntry = null;
        }

        OnDeactivate();
    }

    private bool AddBotToLeft()
    {
        if (m_TeamA == null)
        {
            return false;
        }

        return m_TeamA.AddBot();
    }

    private bool AddBotToRight()
    {
        if (m_TeamB == null)
        {
            return false;
        }

        return m_TeamB.AddBot();
    }

    private bool RemoveBotFromLeft()
    {
        if (m_TeamA == null)
        {
            return false;
        }

        return m_TeamA.RemoveBot();
    }

    private bool RemoveBotFromRight()
    {
        if (m_TeamB == null)
        {
            return false;
        }

        return m_TeamB.RemoveBot();
    }
}
