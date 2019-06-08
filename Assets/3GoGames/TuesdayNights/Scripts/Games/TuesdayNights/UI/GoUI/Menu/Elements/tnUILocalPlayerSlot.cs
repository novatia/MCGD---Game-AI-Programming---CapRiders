using UnityEngine;
using UnityEngine.UI;

using WiFiInput.Server;

using PlayerInputEvents;

public class tnUILocalPlayerSlot : MonoBehaviour
{
    private static string s_DefaultText = "PRESS START TO JOIN";

    [SerializeField]
    private Image m_Image = null;
    [SerializeField]
    private Text m_PlayerName = null;
    [SerializeField]
    private Text m_OverlayText = null;

    [SerializeField]
    private Sprite m_Keyboard = null;
    [SerializeField]
    private Sprite m_Controller = null;
    [SerializeField]
    private Sprite m_Phone = null;

    private int m_PlayerId = Hash.s_NULL;

    // MonoBehaviour's interface

    void Awake()
    {
        Clear();
    }

    void OnEnable()
    {
        InputSystem.onControllerConnectedEventMain += OnControllerConnectedEvent;
        InputSystem.onControllerDisconnectedEventMain += OnControllerDisconnectedEvent;

        RefreshView();
    }

    void OnDisable()
    {
        InputSystem.onControllerConnectedEventMain -= OnControllerConnectedEvent;
        InputSystem.onControllerDisconnectedEventMain -= OnControllerDisconnectedEvent;
    }

    // LOGIC

    public void Bind(int i_PlayerId)
    {
        m_PlayerId = i_PlayerId;
        RefreshView();
    }

    public void Clear()
    {
        m_PlayerId = Hash.s_NULL;
        RefreshView();
    }

    public void SetPlayerName(string i_PlayerName, int i_GuestIndex)
    {
        string playerName = i_PlayerName + ((i_GuestIndex > 0) ? (" (" + i_GuestIndex + ")") : "");
        SetPlayerName(playerName);
    }

    // INTERNALS

    private void RefreshView()
    {
        tnPlayerData playerData = tnGameData.GetPlayerDataMain(m_PlayerId);

        bool hasValidPlayer = (playerData != null);

        if (hasValidPlayer)
        {
            PlayerInput playerInput;
            WiFiPlayerInput wifiPlayerInput;
            tnInputUtils.GetPlayersInputs(m_PlayerId, out playerInput, out wifiPlayerInput);

            bool joystick = (playerInput != null) ? (playerInput.JoystickCount > 0) : false;
            bool keyboard = (playerInput != null) ? (playerInput.JoystickCount == 0) : false;
            bool phone = (wifiPlayerInput != null) ? true : false;

            SetImageActive(true);
            if (joystick) SetImageSprite(m_Controller);
            if (keyboard) SetImageSprite(m_Keyboard);
            if (phone) SetImageSprite(m_Phone);

            SetPlayerNameActive(true);

            SetOverlayText("");
            SetOverlayTextActive(false);

            Color color = playerData.color;
            SetColor(color);
        }
        else
        {
            SetImageActive(false);

            SetPlayerName("");
            SetPlayerNameActive(false);

            SetOverlayText(s_DefaultText);
            SetOverlayTextActive(true);

            SetColor(Color.white);
        }
    }

    private void SetImageActive(bool i_Active)
    {
        if (m_Image != null)
        {
            m_Image.enabled = i_Active;
        }
    }

    private void SetImageSprite(Sprite i_Sprite)
    {
        if (m_Image != null)
        {
            m_Image.sprite = i_Sprite;
        }
    }

    private void SetPlayerNameActive(bool i_Active)
    {
        if (m_PlayerName != null)
        {
            m_PlayerName.enabled = i_Active;
        }
    }

    private void SetPlayerName(string i_Name)
    {
        if (m_PlayerName != null)
        {
            m_PlayerName.text = i_Name;
        }
    }

    private void SetOverlayTextActive(bool i_Active)
    {
        if (m_OverlayText != null)
        {
            m_OverlayText.enabled = i_Active;
        }
    }

    private void SetOverlayText(string i_Text)
    {
        if (m_OverlayText != null)
        {
            m_OverlayText.text = i_Text;
        }
    }

    private void SetColor(Color i_Color)
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

    // EVENTS

    private void OnControllerConnectedEvent(ControllerEventParams i_Params)
    {
        if (Hash.IsNullOrEmpty(m_PlayerId))
            return;

        RefreshView();
    }

    private void OnControllerDisconnectedEvent(ControllerEventParams i_Params)
    {
        if (Hash.IsNullOrEmpty(m_PlayerId))
            return;

        RefreshView();
    }
}
