#if PHOTON

using System.Collections.Generic;

using ExitGames.Client.Photon;

public delegate void OnConnectedToPhoton();
public delegate void OnConnectedToMaster();
public delegate void OnDisconnectedFromPhoton();
public delegate void OnFailedToConnectToPhoton(DisconnectCause cause);
public delegate void OnConnectionFail(DisconnectCause cause);
public delegate void OnJoinedLobby();
public delegate void OnLeftLobby();
public delegate void OnLeftRoom();
public delegate void OnJoinedRoom();
public delegate void OnCreatedRoom();
public delegate void OnPhotonCreateRoomFailed(object[] codeAndMsg);
public delegate void OnPhotonJoinRoomFailed(object[] codeAndMsg);
public delegate void OnPhotonRandomJoinFailed(object[] codeAndMsg);
public delegate void OnReceivedRoomListUpdate();
public delegate void OnMasterClientSwitched(PhotonPlayer newMasterClient);
public delegate void OnPhotonPlayerConnected(PhotonPlayer newPlayer);
public delegate void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer);
public delegate void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged);
public delegate void OnPhotonPlayerPropertiesChanged(PhotonPlayer i_Player, Hashtable i_Properties);
public delegate void OnCustomAuthenticationFailed(string debugMessage);
public delegate void OnCustomAuthenticationResponse(Dictionary<string, object> data);
public delegate void OnPhotonMaxCccuReached();
public delegate void OnUpdatedFriendList();
public delegate void OnWebRpcResponse(OperationResponse response);
public delegate void OnOwnershipRequest(object[] viewAndPlayer);
public delegate void OnLobbyStatisticsUpdate();

public class PhotonCallbacks : Singleton<PhotonCallbacks>
{
    // STATIC

    public static event OnConnectedToPhoton onConnectedToPhotonMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onConnectedToPhoton += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onConnectedToPhoton -= value;
            }
        }
    }

    public static event OnConnectedToMaster onConnectedToMasterMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onConnectedToMaster += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onConnectedToMaster -= value;
            }
        }
    }

    public static event OnDisconnectedFromPhoton onDisconnectedFromPhotonMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onDisconnectedFromPhoton += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onDisconnectedFromPhoton -= value;
            }
        }
    }

    public static event OnFailedToConnectToPhoton onFailedToConnectToPhotonMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onFailedToConnectToPhoton += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onFailedToConnectToPhoton -= value;
            }
        }
    }

    public static event OnConnectionFail onConnectionFailMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onConnectionFail += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onConnectionFail -= value;
            }
        }
    }

    public static event OnJoinedLobby onJoinedLobbyMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onJoinedLobby += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onJoinedLobby -= value;
            }
        }
    }

    public static event OnLeftLobby onLeftLobbyMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onLeftLobby += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onLeftLobby -= value;
            }
        }
    }

    public static event OnLeftRoom onLeftRoomMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onLeftRoom += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onLeftRoom -= value;
            }
        }
    }

    public static event OnJoinedRoom onJoinedRoomMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onJoinedRoom += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onJoinedRoom -= value;
            }
        }
    }

    public static event OnCreatedRoom onCreatedRoomMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onCreatedRoom += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onCreatedRoom -= value;
            }
        }
    }

    public static event OnPhotonCreateRoomFailed onPhotonCreateRoomFailedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onPhotonCreateRoomFailed += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onPhotonCreateRoomFailed -= value;
            }
        }
    }

    public static event OnPhotonJoinRoomFailed onPhotonJoinRoomFailedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onPhotonJoinRoomFailed += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onPhotonJoinRoomFailed -= value;
            }
        }
    }

    public static event OnPhotonRandomJoinFailed onPhotonRandomJoinFailedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onPhotonRandomJoinFailed += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onPhotonRandomJoinFailed -= value;
            }
        }
    }

    public static event OnReceivedRoomListUpdate onReceivedRoomListUpdateMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onReceivedRoomListUpdate += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onReceivedRoomListUpdate -= value;
            }
        }
    }

    public static event OnMasterClientSwitched onMasterClientSwitchedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onMasterClientSwitched += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onMasterClientSwitched -= value;
            }
        }
    }

    public static event OnPhotonPlayerConnected onPhotonPlayerConnectedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onPhotonPlayerConnected += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onPhotonPlayerConnected -= value;
            }
        }
    }

    public static event OnPhotonPlayerDisconnected onPhotonPlayerDisconnectedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onPhotonPlayerDisconnected += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onPhotonPlayerDisconnected -= value;
            }
        }
    }

    public static event OnPhotonCustomRoomPropertiesChanged onPhotonCustomRoomPropertiesChangedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onPhotonCustomRoomPropertiesChanged += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onPhotonCustomRoomPropertiesChanged -= value;
            }
        }
    }

    public static event OnPhotonPlayerPropertiesChanged onPhotonPlayerPropertiesChangedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onPhotonPlayerPropertiesChanged += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onPhotonPlayerPropertiesChanged -= value;
            }
        }
    }

    public static event OnCustomAuthenticationFailed onCustomAuthenticationFailedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onCustomAuthenticationFailed += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onCustomAuthenticationFailed -= value;
            }
        }
    }

    public static event OnCustomAuthenticationResponse onCustomAuthenticationResponseMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onCustomAuthenticationResponse += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onCustomAuthenticationResponse -= value;
            }
        }
    }

    public static event OnPhotonMaxCccuReached onPhotonMaxCccuReachedMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onPhotonMaxCccuReached += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onPhotonMaxCccuReached -= value;
            }
        }
    }

    public static event OnUpdatedFriendList onUpdatedFriendListMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onUpdatedFriendList += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onUpdatedFriendList -= value;
            }
        }
    }

    public static event OnWebRpcResponse onWebRpcResponseMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onWebRpcResponse += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onWebRpcResponse -= value;
            }
        }
    }

    public static event OnOwnershipRequest onOwnershipRequestMain
    {
        add
        {
            if (Instance != null)
            {
                Instance.onOwnershipRequest += value;
            }
        }

        remove
        {
            if (Instance != null)
            {
                Instance.onOwnershipRequest -= value;
            }
        }
    }

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    // Fields

    private event OnConnectedToPhoton m_OnConnectedToPhoton = null;
    private event OnConnectedToMaster m_OnConnectedToMaster = null;
    private event OnDisconnectedFromPhoton m_OnDisconnectedFromPhoton = null;
    private event OnFailedToConnectToPhoton m_OnFailedToConnectToPhoton = null;
    private event OnConnectionFail m_OnConnectionFail = null;
    private event OnJoinedLobby m_OnJoinedLobby = null;
    private event OnLeftLobby m_OnLeftLobby = null;
    private event OnLeftRoom m_OnLeftRoom = null;
    private event OnJoinedRoom m_OnJoinedRoom = null;
    private event OnCreatedRoom m_OnCreatedRoom = null;
    private event OnPhotonCreateRoomFailed m_OnPhotonCreateRoomFailed = null;
    private event OnPhotonJoinRoomFailed m_OnPhotonJoinRoomFailed = null;
    private event OnPhotonRandomJoinFailed m_OnPhotonRandomJoinFailed = null;
    private event OnReceivedRoomListUpdate m_OnReceivedRoomListUpdate = null;
    private event OnMasterClientSwitched m_OnMasterClientSwitched = null;
    private event OnPhotonPlayerConnected m_OnPhotonPlayerConnected = null;
    private event OnPhotonPlayerDisconnected m_OnPhotonPlayerDisconnected = null;
    private event OnPhotonCustomRoomPropertiesChanged m_OnPhotonCustomRoomPropertiesChanged = null;
    private event OnPhotonPlayerPropertiesChanged m_OnPhotonPlayerPropertiesChanged = null;
    private event OnCustomAuthenticationFailed m_OnCustomAuthenticationFailed = null;
    private event OnCustomAuthenticationResponse m_OnCustomAuthenticationResponse = null;
    private event OnPhotonMaxCccuReached m_OnPhotonMaxCccuReached = null;
    private event OnUpdatedFriendList m_OnUpdatedFriendList = null;
    private event OnWebRpcResponse m_OnWebRpcResponse = null;
    private event OnOwnershipRequest m_OnOwnershipRequest = null;
    private event OnLobbyStatisticsUpdate m_OnLobbyStatisticsUpdate = null;

    // ACCESSORS

    public event OnConnectedToPhoton onConnectedToPhoton
    {
        add
        {
            m_OnConnectedToPhoton += value;
        }

        remove
        {
            m_OnConnectedToPhoton -= value;
        }
    }

    public event OnConnectedToMaster onConnectedToMaster
    {
        add
        {
            m_OnConnectedToMaster += value;
        }

        remove
        {
            m_OnConnectedToMaster -= value;
        }
    }

    public event OnDisconnectedFromPhoton onDisconnectedFromPhoton
    {
        add
        {
            m_OnDisconnectedFromPhoton += value;
        }

        remove
        {
            m_OnDisconnectedFromPhoton -= value;
        }
    }

    public event OnFailedToConnectToPhoton onFailedToConnectToPhoton
    {
        add
        {
            m_OnFailedToConnectToPhoton += value;
        }

        remove
        {
            m_OnFailedToConnectToPhoton -= value;
        }
    }

    public event OnConnectionFail onConnectionFail
    {
        add
        {
            m_OnConnectionFail += value;
        }

        remove
        {
            m_OnConnectionFail -= value;
        }
    }

    public event OnJoinedLobby onJoinedLobby
    {
        add
        {
            m_OnJoinedLobby += value;
        }

        remove
        {
            m_OnJoinedLobby -= value;
        }
    }

    public event OnLeftLobby onLeftLobby
    {
        add
        {
            m_OnLeftLobby += value;
        }

        remove
        {
            m_OnLeftLobby -= value;
        }
    }
    
    public event OnLeftRoom onLeftRoom
    {
        add
        {
            m_OnLeftRoom += value;
        }

        remove
        {
            m_OnLeftRoom -= value;
        }
    }

    public event OnJoinedRoom onJoinedRoom
    {
        add
        {
            m_OnJoinedRoom += value;
        }

        remove
        {
            m_OnJoinedRoom -= value;
        }
    }

    public event OnCreatedRoom onCreatedRoom
    {
        add
        {
            m_OnCreatedRoom += value;
        }

        remove
        {
            m_OnCreatedRoom -= value;
        }
    }

    public event OnPhotonCreateRoomFailed onPhotonCreateRoomFailed
    {
        add
        {
            m_OnPhotonCreateRoomFailed += value;
        }

        remove
        {
            m_OnPhotonCreateRoomFailed -= value;
        }
    }

    public event OnPhotonJoinRoomFailed onPhotonJoinRoomFailed
    {
        add
        {
            m_OnPhotonJoinRoomFailed += value;
        }

        remove
        {
            m_OnPhotonJoinRoomFailed -= value;
        }
    }

    public event OnPhotonRandomJoinFailed onPhotonRandomJoinFailed
    {
        add
        {
            m_OnPhotonRandomJoinFailed += value;
        }

        remove
        {
            m_OnPhotonRandomJoinFailed -= value;
        }
    }

    public event OnReceivedRoomListUpdate onReceivedRoomListUpdate
    {
        add
        {
            m_OnReceivedRoomListUpdate += value;
        }

        remove
        {
            m_OnReceivedRoomListUpdate -= value;
        }
    }

    public event OnMasterClientSwitched onMasterClientSwitched
    {
        add
        {
            m_OnMasterClientSwitched += value;
        }

        remove
        {
            m_OnMasterClientSwitched -= value;
        }
    }

    public event OnPhotonPlayerConnected onPhotonPlayerConnected
    {
        add
        {
            m_OnPhotonPlayerConnected += value;
        }

        remove
        {
            m_OnPhotonPlayerConnected -= value;
        }
    }

    public event OnPhotonPlayerDisconnected onPhotonPlayerDisconnected
    {
        add
        {
            m_OnPhotonPlayerDisconnected += value;
        }

        remove
        {
            m_OnPhotonPlayerDisconnected -= value;
        }
    }

    public event OnPhotonCustomRoomPropertiesChanged onPhotonCustomRoomPropertiesChanged
    {
        add
        {
            m_OnPhotonCustomRoomPropertiesChanged += value;
        }

        remove
        {
            m_OnPhotonCustomRoomPropertiesChanged -= value;
        }
    }

    public event OnPhotonPlayerPropertiesChanged onPhotonPlayerPropertiesChanged
    {
        add
        {
            m_OnPhotonPlayerPropertiesChanged += value;
        }

        remove
        {
            m_OnPhotonPlayerPropertiesChanged -= value;
        }
    }

    public event OnCustomAuthenticationFailed onCustomAuthenticationFailed
    {
        add
        {
            m_OnCustomAuthenticationFailed += value;
        }

        remove
        {
            m_OnCustomAuthenticationFailed -= value;
        }
    }

    public event OnCustomAuthenticationResponse onCustomAuthenticationResponse
    {
        add
        {
            m_OnCustomAuthenticationResponse += value;
        }

        remove
        {
            m_OnCustomAuthenticationResponse -= value;
        }
    }

    public event OnPhotonMaxCccuReached onPhotonMaxCccuReached
    {
        add
        {
            m_OnPhotonMaxCccuReached += value;
        }

        remove
        {
            m_OnPhotonMaxCccuReached -= value;
        }
    }

    public event OnUpdatedFriendList onUpdatedFriendList
    {
        add
        {
            m_OnUpdatedFriendList += value;
        }

        remove
        {
            m_OnUpdatedFriendList -= value;
        }
    }

    public event OnWebRpcResponse onWebRpcResponse
    {
        add
        {
            m_OnWebRpcResponse += value;
        }

        remove
        {
            m_OnWebRpcResponse -= value;
        }
    }

    public event OnOwnershipRequest onOwnershipRequest
    {
        add
        {
            m_OnOwnershipRequest += value;
        }

        remove
        {
            m_OnOwnershipRequest -= value;
        }
    }

    public event OnLobbyStatisticsUpdate onLobbyStatisticsUpdate
    {
        add
        {
            m_OnLobbyStatisticsUpdate += value;
        }

        remove
        {
            m_OnLobbyStatisticsUpdate -= value;
        }
    }

    // LOGIC

    public void Initialize()
    {

    }

    // Photon callbacks

    private void OnConnectedToPhoton()
    {
        if (m_OnConnectedToPhoton != null)
        {
            m_OnConnectedToPhoton();
        }
    }

    private void OnConnectedToMaster()
    {
        if (m_OnConnectedToMaster != null)
        {
            m_OnConnectedToMaster();
        }
    }

    private void OnDisconnectedFromPhoton()
    {
        if (m_OnDisconnectedFromPhoton != null)
        {
            m_OnDisconnectedFromPhoton();
        }
    }

    private void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        if (m_OnFailedToConnectToPhoton != null)
        {
            m_OnFailedToConnectToPhoton(cause);
        }
    }

    private void OnConnectionFail(DisconnectCause cause)
    {
        if (m_OnConnectionFail != null)
        {
            m_OnConnectionFail(cause);
        }
    }

    private void OnJoinedLobby()
    {
        if (m_OnJoinedLobby != null)
        {
            m_OnJoinedLobby();
        }
    }

    private void OnLeftLobby()
    {
        if (m_OnLeftLobby != null)
        {
            m_OnLeftLobby();
        }
    }

    private void OnLeftRoom()
    {
        if (m_OnLeftRoom != null)
        { 
            m_OnLeftRoom();
        }
    }

    private void OnJoinedRoom()
    {
        if (m_OnJoinedRoom != null)
        {
            m_OnJoinedRoom();
        }
    }

    private void OnCreatedRoom()
    {
        if (m_OnCreatedRoom != null)
        {
            m_OnCreatedRoom();
        }
    }

    private void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        if (m_OnPhotonCreateRoomFailed != null)
        {
            m_OnPhotonCreateRoomFailed(codeAndMsg);
        }
    }

    private void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        if (m_OnPhotonJoinRoomFailed != null)
        {
            m_OnPhotonJoinRoomFailed(codeAndMsg);
        }
    }

    private void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        if (m_OnPhotonRandomJoinFailed != null)
        {
            m_OnPhotonRandomJoinFailed(codeAndMsg);
        }
    }

    private void OnReceivedRoomListUpdate()
    {
        if (m_OnReceivedRoomListUpdate != null)
        {
            m_OnReceivedRoomListUpdate();
        }
    }

    private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (m_OnMasterClientSwitched != null)
        {
            m_OnMasterClientSwitched(newMasterClient);
        }
    }

    private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (m_OnPhotonPlayerConnected != null)
        {
            m_OnPhotonPlayerConnected(newPlayer);
        }
    }

    private void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (m_OnPhotonPlayerDisconnected != null)
        {
            m_OnPhotonPlayerDisconnected(otherPlayer);
        }
    }

    private void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged)
    {
        if (m_OnPhotonCustomRoomPropertiesChanged != null)
        {
            m_OnPhotonCustomRoomPropertiesChanged(propertiesThatChanged);
        }
    }

    private void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        if (m_OnPhotonPlayerPropertiesChanged != null)
        {
            PhotonPlayer photonPlayer = (PhotonPlayer) playerAndUpdatedProps[0];
            Hashtable properties = (Hashtable)playerAndUpdatedProps[1];

            m_OnPhotonPlayerPropertiesChanged(photonPlayer, properties);
        }
    }

    private void OnCustomAuthenticationFailed(string debugMessage)
    {
        if (m_OnCustomAuthenticationFailed != null)
        {
            m_OnCustomAuthenticationFailed(debugMessage);
        }
    }

    private void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        if (m_OnCustomAuthenticationResponse != null)
        {
            m_OnCustomAuthenticationResponse(data);
        }
    }

    private void OnPhotonMaxCccuReached()
    {
        if (m_OnPhotonMaxCccuReached != null)
        {
            m_OnPhotonMaxCccuReached();
        }
    }

    private void OnUpdatedFriendList()
    {
        if (m_OnUpdatedFriendList != null)
        {
            m_OnUpdatedFriendList();
        }
    }

    private void OnWebRpcResponse(OperationResponse response)
    {
        if (m_OnWebRpcResponse != null)
        {
            m_OnWebRpcResponse(response);
        }
    }

    private void OnOwnershipRequest(object[] viewAndPlayer)
    {
        if (m_OnOwnershipRequest != null)
        {
            m_OnOwnershipRequest(viewAndPlayer);
        }
    }

    private void OnLobbyStatisticsUpdate()
    {
        if (m_OnLobbyStatisticsUpdate != null)
        {
            m_OnLobbyStatisticsUpdate();
        }
    }
}

#endif // PHOTON

// PUN CALLBACKS LIST

// void OnConnectedToPhoton();
// void OnConnectedToMaster();

// void OnDisconnectedFromPhoton();

// void OnFailedToConnectToPhoton(DisconnectCause cause);
// void OnConnectionFail(DisconnectCause cause);

// void OnJoinedLobby();
// void OnLeftLobby();

// void OnLeftRoom();
// void OnJoinedRoom();
// void OnCreatedRoom();

// void OnPhotonCreateRoomFailed(object[] codeAndMsg);
// void OnPhotonJoinRoomFailed(object[] codeAndMsg);
// void OnPhotonRandomJoinFailed(object[] codeAndMsg);

// void OnReceivedRoomListUpdate();

// void OnMasterClientSwitched(PhotonPlayer newMasterClient);

// void OnPhotonPlayerConnected(PhotonPlayer newPlayer);
// void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer);

// void OnPhotonInstantiate(PhotonMessageInfo info);

// void OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged);
// void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps);

// void OnCustomAuthenticationFailed(string debugMessage);
// void OnCustomAuthenticationResponse(Dictionary<string, object> data);

// void OnPhotonMaxCccuReached();
// void OnUpdatedFriendList();
// void OnWebRpcResponse(OperationResponse response);
// void OnOwnershipRequest(object[] viewAndPlayer);
// void OnLobbyStatisticsUpdate();