using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public enum tnUI_MP_DeviceSide
{
    Left = -1,
    Center = 0,
    Right = 1,
}

public enum tnUI_MP_DeviceState
{
    None,
    Closed,
    Open,
    Occupied,
}

public class tnUI_MP_Device : MonoBehaviour
{
    [Header("Logic")]

    [SerializeField]
    private RectTransform m_LeftAnchor = null;
    [SerializeField]
    private RectTransform m_CenterAnchor = null;
    [SerializeField]
    private RectTransform m_RightAnchor = null;
    [SerializeField]
    private RectTransform m_Graphics = null;

    [SerializeField]
    private float m_Speed = 3840f;
    [SerializeField]
    private float m_DisabledAlpha = 0.25f;

    [Header("Audio")]

    [SerializeField]
    private SfxDescriptor m_Move_Sfx = null;

    private Image m_Image = null;
    private Image m_Ready = null;
    private Image m_Captain = null;
    private Text m_PlayerName = null;
    private Text m_OverlayLabel = null;
    private CanvasGroup m_CanvasGroup = null;

    private tnUI_MP_DeviceSide m_CurrentSide = tnUI_MP_DeviceSide.Center;
    private tnUI_MP_DeviceState m_CurrentState = tnUI_MP_DeviceState.None;

    private bool m_IsActive = false;
    private bool m_IsReady = false;
    private bool m_IsCaptain = false;

    private IEnumerator m_CurrentCorroutine = null;

    private bool m_IsInTransition = false;

    private Vector2 leftAnchoredPosition
    {
        get
        {
            if (m_LeftAnchor != null)
            {
                return m_LeftAnchor.anchoredPosition;
            }

            return Vector2.zero;
        }
    }

    private Vector2 centerAnchoredPosition
    {
        get
        {
            if (m_CenterAnchor != null)
            {
                return m_CenterAnchor.anchoredPosition;
            }

            return Vector2.zero;
        }
    }

    private Vector2 rightAnchoredPosition
    {
        get
        {
            if (m_RightAnchor != null)
            {
                return m_RightAnchor.anchoredPosition;
            }

            return Vector2.zero;
        }
    }

    // ACCESSORS

    public bool isInTransition
    {
        get
        {
            return m_IsInTransition;
        }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        if (m_Graphics != null)
        {
            Transform imageTransform = m_Graphics.Find("Image");
            if (imageTransform != null)
            {
                m_Image = imageTransform.GetComponent<Image>();
            }

            Transform readyTransform = m_Graphics.Find("Ready");
            if (readyTransform != null)
            {
                m_Ready = readyTransform.GetComponent<Image>();
            }

            Transform captainTransform = m_Graphics.Find("Captain");
            if (captainTransform != null)
            {
                m_Captain = captainTransform.GetComponent<Image>();
            }

            Transform playerNameTransform = m_Graphics.Find("PlayerName");
            if (playerNameTransform != null)
            {
                m_PlayerName = playerNameTransform.GetComponent<Text>();
            }

            Transform overlayLabelTransform = m_Graphics.Find("OverlayLabel");
            if (overlayLabelTransform != null)
            {
                m_OverlayLabel = overlayLabelTransform.GetComponent<Text>();
            }

            m_CanvasGroup = m_Graphics.GetComponent<CanvasGroup>();
        }

        Clear();
    }

    // LOGIC

    public void ForceDeviceSide(tnUI_MP_DeviceSide i_Side)
    {
        m_CurrentSide = i_Side;

        CancelCoroutine();

        Vector2 targetPosition = GetIdealPosition();
        Internal_SetDeviceAnchoredPosition(targetPosition);
    }

    public void SetDeviceSide(tnUI_MP_DeviceSide i_Side, bool i_Immediatly)
    {
        if (!m_IsActive)
            return;

        if (i_Side != m_CurrentSide)
        {
            m_CurrentSide = i_Side;

            CancelCoroutine();

            SfxPlayer.PlayMain(m_Move_Sfx);

            if (!i_Immediatly)
            {
                m_CurrentCorroutine = MoveDevice();
                StartCoroutine(m_CurrentCorroutine);
            }
            else
            {
                Vector2 targetPosition = GetIdealPosition();
                Internal_SetDeviceAnchoredPosition(targetPosition);
            }
        }
    }

    public void SetDeviceColor(Color i_Color)
    {
        Internal_SetColor(i_Color);
    }

    public void SetPlayerName(string i_PlayerName)
    {
        Internal_SetPlayerName(i_PlayerName);
    }

    public void SetState(tnUI_MP_DeviceState i_State)
    {
        if (m_CurrentState == i_State)
            return;

        Internal_SetState(i_State);
    }

    public void SetReady(bool i_Ready)
    {
        if (i_Ready == m_IsReady)
            return;

        m_IsReady = i_Ready;

        if (m_CurrentState == tnUI_MP_DeviceState.Occupied)
        {
            Internal_SetReadyEnabled(m_IsReady);
        }
    }

    public void SetCaptain(bool i_Captain)
    {
        m_IsCaptain = i_Captain;

        if (m_CurrentState == tnUI_MP_DeviceState.Occupied)
        {
            Internal_SetCaptainEnabled(m_IsCaptain);
        }
    }

    public void Clear()
    {
        ForceDeviceSide(tnUI_MP_DeviceSide.Center);

        SetDeviceColor(Color.white);
        SetPlayerName("");
        SetCaptain(false);
        SetReady(false);

        SetState(tnUI_MP_DeviceState.Closed);
    }

    // INTERNAL

    private void CancelCoroutine()
    {
        if (m_CurrentCorroutine != null)
        {
            StopCoroutine(m_CurrentCorroutine);
            m_CurrentCorroutine = null;
            m_IsInTransition = false;
        }
    }

    private IEnumerator MoveDevice()
    {
        if (m_Graphics == null)
            yield break;

        m_IsInTransition = true;

        Vector2 startPosition = m_Graphics.anchoredPosition;
        Vector2 targetPosition = GetIdealPosition();

        float totalDistance = Vector2.Distance(targetPosition, startPosition);
        float time = (m_Speed == 0f) ? 0.25f : (totalDistance / m_Speed);

        float currentTime = 0f;

        while (currentTime < time)
        {
            currentTime += Time.deltaTime;

            float percentage = Mathf.Clamp01(currentTime / time);
            Vector2 nextPosition = Vector2.Lerp(startPosition, targetPosition, percentage);
            Internal_SetDeviceAnchoredPosition(nextPosition);

            yield return null;
        }

        Internal_SetDeviceAnchoredPosition(targetPosition);

        m_IsInTransition = false;
    }

    private void Internal_SetColor(Color i_Color)
    {
        if (m_Image != null)
        {
            m_Image.color = i_Color;
        }

        if (m_PlayerName != null)
        {
            m_PlayerName.color = i_Color;
        }
    }

    private void Internal_SetAlpha(float i_Alpha)
    {
        if (m_CanvasGroup == null)
            return;

        m_CanvasGroup.alpha = i_Alpha;
    }

    private void Internal_SetPlayerGraphicsEnabled(bool i_Enabled)
    {
        if (m_Image != null)
        {
            m_Image.enabled = i_Enabled;
        }

        if (m_PlayerName != null)
        {
            m_PlayerName.enabled = i_Enabled;
        }
    }

    private void Internal_SetPlayerName(string i_Text)
    {
        if (m_PlayerName != null)
        {
            m_PlayerName.text = i_Text;
        }
    }

    private void Internal_SetOverlayLabel(string i_Text)
    {
        if (m_OverlayLabel != null)
        {
            m_OverlayLabel.text = i_Text;
        }
    }

    private void Internal_SetOverlayLabelEnabled(bool i_Enabled)
    {
        if (m_OverlayLabel != null)
        {
            m_OverlayLabel.enabled = i_Enabled;
        }
    }

    private void Internal_SetReadyEnabled(bool i_Enabled)
    {
        if (m_Ready != null)
        {
            m_Ready.enabled = i_Enabled;
        }
    }

    private void Internal_SetCaptainEnabled(bool i_Enabled)
    {
        if (m_Captain != null)
        {
            m_Captain.enabled = i_Enabled;
        }
    }

    private Vector2 GetIdealPosition()
    {
        Vector2 targetPosition = (m_CurrentSide == tnUI_MP_DeviceSide.Left) ? leftAnchoredPosition : (m_CurrentSide == tnUI_MP_DeviceSide.Center) ? centerAnchoredPosition : rightAnchoredPosition;
        return targetPosition;
    }

    private void Internal_SetDeviceAnchoredPosition(Vector2 i_AnchoredPosition)
    {
        if (m_Graphics == null)
            return;

        m_Graphics.anchoredPosition = i_AnchoredPosition;
    }

    private void Internal_SetState(tnUI_MP_DeviceState i_State)
    {
        if (i_State == tnUI_MP_DeviceState.None)
            return;

        switch (i_State)
        {
            case tnUI_MP_DeviceState.Closed:

                Internal_SetAlpha(m_DisabledAlpha);
                Internal_SetPlayerGraphicsEnabled(false);
                Internal_SetOverlayLabelEnabled(true);
                Internal_SetOverlayLabel("CLOSED");
                Internal_SetCaptainEnabled(false);
                Internal_SetReadyEnabled(false);
                m_IsCaptain = false;
                m_IsReady = false;

                m_IsActive = false;

                break;

            case tnUI_MP_DeviceState.Open:

                Internal_SetAlpha(1f);
                Internal_SetPlayerGraphicsEnabled(false);
                Internal_SetOverlayLabelEnabled(true);
                Internal_SetOverlayLabel("EMPTY");
                Internal_SetCaptainEnabled(false);
                Internal_SetReadyEnabled(false);
                m_IsCaptain = false;
                m_IsReady = false;

                m_IsActive = false;

                break;

            case tnUI_MP_DeviceState.Occupied:

                Internal_SetAlpha(1f);
                Internal_SetPlayerGraphicsEnabled(true);
                Internal_SetOverlayLabelEnabled(false);
                Internal_SetOverlayLabel("");
                Internal_SetCaptainEnabled(m_IsCaptain);
                Internal_SetReadyEnabled(m_IsReady);

                m_IsActive = true;

                break;
        }
    }
}
