using UnityEngine;
using UnityEngine.UI;

public enum tnRoomEntryState
{
    Disabled,
    Normal,
    Highlighted,
}

public struct tnUIRoomEntryData
{
    private Sprite m_StadiumThmbnail;
    private string m_StadiumName;
    private string m_GameMode;
    private string m_Rules;
    private string m_HostName;
    private int m_PlayersCount;
    private int m_TotalPlayers;
    private int m_Ping;

    // ACCESSORS

    public Sprite stadiumThumbnail
    {
        get
        {
            return m_StadiumThmbnail;
        }
    }

    public string stadiumName
    {
        get
        {
            return m_StadiumName;
        }
    }

    public string gameMode
    {
        get
        {
            return m_GameMode;
        }
    }

    public string rules
    {
        get
        {
            return m_Rules;
        }
    }

    public string hostName
    {
        get
        {
            return m_HostName;
        }
    }

    public int playersCount
    {
        get
        {
            return m_PlayersCount;
        }
    }

    public int totalPlayers
    {
        get
        {
            return m_TotalPlayers;
        }
    }

    public int ping
    {
        get
        {
            return m_Ping;
        }
    }

    // CTOR

    public tnUIRoomEntryData(Sprite i_StadiumThumbnail, string i_StadiumName, string i_GameMode, string i_Rules, string i_HostName, int i_PlayersCount, int i_TotalPlayers, int i_Ping)
    {
        m_StadiumThmbnail = i_StadiumThumbnail;
        m_StadiumName = i_StadiumName;
        m_Rules = i_Rules;
        m_GameMode = i_GameMode;
        m_HostName = i_HostName;
        m_PlayersCount = i_PlayersCount;
        m_TotalPlayers = i_TotalPlayers;
        m_Ping = i_Ping;
    }
}

public class tnUIRoomEntry : MonoBehaviour
{
    // Types

    private enum PingState
    {
        Low,
        Medium,
        High,
    }

    // STATIC

    private static int s_Disabled_BoolId = Animator.StringToHash("Disabled");
    private static int s_Normal_BoolId = Animator.StringToHash("Normal");
    private static int s_Highligthed_BoolId = Animator.StringToHash("Highlighted");

    // Serializeable fields

    [Header("Widgets")]

    [SerializeField]
    private Image m_StadiumThumbnail = null;
    [SerializeField]
    private Text m_StadiumName = null;
    [SerializeField]
    private Text m_GameMode = null;
    [SerializeField]
    private Text m_Rules = null;
    [SerializeField]
    private Text m_HostName = null;
    [SerializeField]
    private Text m_Players = null;
    [SerializeField]
    private Text m_Ping = null;
    [SerializeField]
    private Image m_PingIcon = null;

    [Header("Ping")]

    [SerializeField]
    private int m_LowPingThreshold = 80;
    [SerializeField]
    private int m_HighPingThreshold = 140;

    [SerializeField]
    private Sprite m_LowPingSprite = null;
    [SerializeField]
    private Sprite m_MediumPingSprite = null;
    [SerializeField]
    private Sprite m_HighPingSprite = null;

    [SerializeField]
    private Color m_LowPingColor = Color.white;
    [SerializeField]
    private Color m_MediumPingColor = Color.white;
    [SerializeField]
    private Color m_HighPingColor = Color.white;

    // Fields

    private tnRoomEntryState m_State = tnRoomEntryState.Disabled;

    // Components

    private Animator m_Animator = null;
    
    // ACCESSORS

    public bool isDisabled
    {
        get
        {
            return (m_State == tnRoomEntryState.Disabled);
        }
    }

    public bool isHighlighted
    {
        get
        {
            return (m_State == tnRoomEntryState.Highlighted);
        }
    }

    public bool isVisible
    {
        get
        {
            return !isDisabled;
        }
    }

    // MonoBehaviour's interface

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Update()
    {
        Internal_UpdateAnimator();
    }

    // LOGIC

    public void Clear()
    {
        SetStadium(null, "");
        SetGameMode("", "");
        SetHostName("");
        SetPlayers(0, 0);
        SetPing(0);

        SetState(tnRoomEntryState.Disabled);
    }

    public void SetStadium(Sprite i_Thumbnail, string i_Name)
    {
        Internal_SetStadium(i_Thumbnail, i_Name);
    }

    public void SetGameMode(string i_Name, string i_Rules)
    {
        Internal_SetGameMode(i_Name, i_Rules);
    }

    public void SetHostName(string i_Name)
    {
        Internal_SetHostName(i_Name);
    }

    public void SetPlayers(int i_PlayersCount, int i_TotalPlayers)
    {
        string playersText = i_PlayersCount.ToString() + " / " + i_TotalPlayers.ToString();
        Internal_SetPlayers(playersText);
    }

    public void SetPing(int i_Ping)
    {
        Internal_SetPing(i_Ping);
    }

    public void SetData(tnUIRoomEntryData i_Data)
    {
        SetStadium(i_Data.stadiumThumbnail, i_Data.stadiumName);
        SetGameMode(i_Data.gameMode, i_Data.rules);
        SetHostName(i_Data.hostName);
        SetPlayers(i_Data.playersCount, i_Data.totalPlayers);
        SetPing(i_Data.ping);
    }

    public void Select()
    {
        if (m_State == tnRoomEntryState.Normal)
        {
            SetState(tnRoomEntryState.Highlighted);
        }
    }

    public void Deselct()
    {
        if (m_State == tnRoomEntryState.Highlighted)
        {
            SetState(tnRoomEntryState.Normal);
        }
    }

    public void Enable()
    {
        if (m_State == tnRoomEntryState.Disabled)
        {
            SetState(tnRoomEntryState.Normal);
        }
    }

    // INTERNALS

    private void SetState(tnRoomEntryState i_State)
    {
        Internal_SetState(i_State);
        Internal_UpdateAnimator();
    }

    private void Internal_UpdateAnimator()
    {
        if (m_Animator == null)
            return;

        if (!m_Animator.isInitialized)
            return;

        bool disabled = (m_State == tnRoomEntryState.Disabled);
        bool normal = (m_State == tnRoomEntryState.Normal);
        bool highlighted = (m_State == tnRoomEntryState.Highlighted);

        if (m_Animator != null)
        {
            m_Animator.SetBool(s_Disabled_BoolId, disabled);
            m_Animator.SetBool(s_Normal_BoolId, normal);
            m_Animator.SetBool(s_Highligthed_BoolId, highlighted);
        }
    }

    private void Internal_SetStadium(Sprite i_Thumbnail, string i_Name)
    {
        if (m_StadiumThumbnail != null)
        {
            m_StadiumThumbnail.sprite = i_Thumbnail;
        }

        if (m_StadiumName != null)
        {
            m_StadiumName.text = i_Name;
        }
    }

    private void Internal_SetGameMode(string i_Name, string i_Rules)
    {
        if (m_GameMode != null)
        {
            m_GameMode.text = i_Name;
        }

        if (m_Rules != null)
        {
            m_Rules.text = i_Rules;
        }
    }
    
    private void Internal_SetHostName(string i_Text)
    {
        if (m_HostName != null)
        {
            m_HostName.text = i_Text;
        }
    }

    private void Internal_SetPlayers(string i_Text)
    {
        if (m_Players != null)
        {
            m_Players.text = i_Text;
        }
    }

    private void Internal_SetPing(int i_Ping)
    {
        if (m_Ping != null)
        {
            m_Ping.text = i_Ping.ToString();
            Color colorPing = GetPingColor(i_Ping);
            m_Ping.color = colorPing;
        }

        if (m_PingIcon != null)
        {
            Sprite pingSprite = GetPingSprite(i_Ping);
            m_PingIcon.sprite = pingSprite;
        }
    }

    private void Internal_SetState(tnRoomEntryState i_State)
    {
        if (i_State == m_State)
            return;

        m_State = i_State;
    }

    // UTILS

    private PingState GetPingState(int i_Ping)
    {
        int lowThreshold = Mathf.Max(0, m_LowPingThreshold);
        int highThreshold = Mathf.Max(lowThreshold, m_HighPingThreshold);

        if (i_Ping <= lowThreshold)
        {
            return PingState.Low;
        }
        else
        {
            if (i_Ping <= highThreshold)
            {
                return PingState.Medium;
            }

            return PingState.High;
        }
    }

    private Sprite GetPingSprite(PingState i_PingState)
    {
        Sprite pingSprite = null;

        switch (i_PingState)
        {
            case PingState.High:
                pingSprite = m_HighPingSprite;
                break;

            case PingState.Medium:
                pingSprite = m_MediumPingSprite;
                break;

            case PingState.Low:
                pingSprite = m_LowPingSprite;
                break;
        }

        return pingSprite;
    }

    private Sprite GetPingSprite(int i_Ping)
    {
        PingState pingState = GetPingState(i_Ping);
        Sprite pingSprite = GetPingSprite(pingState);
        return pingSprite;
    }

    private Color GetPingColor(PingState i_PingState)
    {
        Color pingColor = Color.white;

        switch (i_PingState)
        {
            case PingState.High:
                pingColor = m_HighPingColor;
                break;

            case PingState.Medium:
                pingColor = m_MediumPingColor;
                break;

            case PingState.Low:
                pingColor = m_LowPingColor;
                break;
        }

        return pingColor;
    }

    private Color GetPingColor(int i_Ping)
    {
        PingState pingState = GetPingState(i_Ping);
        Color pingColor = GetPingColor(pingState);
        return pingColor;
    }
}