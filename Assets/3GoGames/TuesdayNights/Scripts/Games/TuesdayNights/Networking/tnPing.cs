using UnityEngine;

using TuesdayNights;

public class tnPing : MonoBehaviour
{
    // Serializable fields

    [Header("Player")]

    [SerializeField]
    private float m_MinPlayerUpdateTime = 0f;

    [SerializeField]
    private int m_PlayerRisingThreshold = 10;
    [SerializeField]
    private int m_PlayerFallingThreshold = 10;

    [Header("Room")]

    [SerializeField]
    private float m_MinRoomUpdateTime = 0f;

    [SerializeField]
    private int m_RoomRisingThreshold = 10;
    [SerializeField]
    private int m_RoomFallingThreshold = 10;

    // Fields

    private float m_PlayerPropertyTimer = 0f;
    private float m_RoomPropertyTimer = 0f;

    private int m_LastPlayerPing = 0;
    private int m_LastRoomPing = 0;

    // ACCESSORS

    public float minPlayerUpdateTime
    {
        get
        {
            return Mathf.Max(0f, m_MinPlayerUpdateTime);
        }

        set
        {
            m_MinPlayerUpdateTime = Mathf.Max(0f, value);
        }
    }

    public int playerRisingThreshold
    {
        get
        {
            return Mathf.Max(0, m_PlayerRisingThreshold);
        }

        set
        {
            m_PlayerRisingThreshold = Mathf.Max(0, value);
        }
    }

    public int playerFallingThreshold
    {
        get
        {
            return Mathf.Max(0, m_PlayerFallingThreshold);
        }

        set
        {
            m_PlayerFallingThreshold = Mathf.Max(0, value);
        }
    }

    public float minRoomUpdateTime
    {
        get
        {
            return Mathf.Max(0f, m_MinPlayerUpdateTime);
        }

        set
        {
            m_MinPlayerUpdateTime = Mathf.Max(0f, value);
        }
    }

    public int roomRisingThreshold
    {
        get
        {
            return Mathf.Max(0, m_RoomRisingThreshold);
        }

        set
        {
            m_RoomRisingThreshold = Mathf.Max(0, value);
        }
    }

    public int roomFallingThreshold
    {
        get
        {
            return Mathf.Max(0, m_RoomFallingThreshold);
        }

        set
        {
            m_RoomFallingThreshold = Mathf.Max(0, value);
        }
    }

    // MonoBehaviour's interface

    private void Start()
    {
        m_LastPlayerPing = 0;
        m_LastRoomPing = 0;
    }

    private void Update()
    {
        if (PhotonNetwork.offlineMode || !PhotonNetwork.connectedAndReady)
        {
            m_LastPlayerPing = 0;
            m_LastRoomPing = 0;

            m_PlayerPropertyTimer = 0f;
            m_RoomPropertyTimer = 0f;
        }
        else
        {
            // You're connected. Refresh player's ping.

            if (m_PlayerPropertyTimer == 0f)
            {
                CheckPlayerPing();
                m_PlayerPropertyTimer = m_MinPlayerUpdateTime;
            }
            else
            {
                m_PlayerPropertyTimer = Mathf.Max(0f, m_PlayerPropertyTimer - Time.deltaTime);
            }

            // Check if you're in a valid room and if you're MasterClient.

            if (PhotonNetwork.room == null || !PhotonNetwork.isMasterClient)
            {
                m_LastRoomPing = 0;

                m_RoomPropertyTimer = 0f;
            }
            else
            { 
                // You're MasterClient. Refresh room's ping.

                if (m_RoomPropertyTimer == 0f)
                {
                    CheckRoomPing();
                    m_RoomPropertyTimer = m_MinRoomUpdateTime;
                }
                else
                {
                    m_RoomPropertyTimer = Mathf.Max(0f, m_RoomPropertyTimer - Time.deltaTime);
                }
            }
        }
    }

    // INTERNALS

    private void CheckPlayerPing()
    {
        int currentPing = PhotonNetwork.GetPing();

        int diff = currentPing - m_LastPlayerPing;
        int absDiff = Mathf.Abs(diff);

        if (diff < 0)
        {
            if (absDiff > m_PlayerFallingThreshold)
            {
                WritePlayerProperty(currentPing);
            }
        }
        else
        {
            if (diff > 0)
            {
                if (absDiff > m_PlayerRisingThreshold)
                {
                    WritePlayerProperty(currentPing);
                }
            }
        }

        m_LastPlayerPing = currentPing;
    }

    private void WritePlayerProperty(int i_Ping)
    {
        PhotonUtils.SetPlayerCustomProperty(PhotonPropertyKey.s_PlayerCustomPropertyKey_Ping, i_Ping);
    }

    private void CheckRoomPing()
    {
        int currentPing = 0;
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        if (photonPlayers != null)
        {
            if (photonPlayers.Length > 0)
            {
                for (int index = 0; index < photonPlayers.Length; ++index)
                {
                    PhotonPlayer photonPlayer = photonPlayers[index];

                    if (photonPlayer == null)
                        continue;

                    int playerPing;
                    PhotonUtils.TryGetPlayerCustomProperty<int>(PhotonPropertyKey.s_PlayerCustomPropertyKey_Ping, out playerPing);

                    currentPing += playerPing;
                }

                currentPing /= photonPlayers.Length;
            }
        }

        int diff = currentPing - m_LastRoomPing;
        int absDiff = Mathf.Abs(diff);

        if (diff < 0)
        {
            if (absDiff > m_RoomFallingThreshold)
            {
                WriteRoomProperty(currentPing);
            }
        }
        else
        {
            if (diff > 0)
            {
                if (absDiff > m_RoomRisingThreshold)
                {
                    WriteRoomProperty(currentPing);
                }
            }
        }

        m_LastRoomPing = currentPing;
    }

    private void WriteRoomProperty(int i_Ping)
    {
        PhotonUtils.SetRoomCustomProperty(PhotonPropertyKey.s_RoomCustomPropertyKey_AvgPing, i_Ping);
    }
}