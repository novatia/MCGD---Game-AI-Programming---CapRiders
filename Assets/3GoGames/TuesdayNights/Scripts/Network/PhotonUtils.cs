#if PHOTON

using UnityEngine;

using System.Collections;

using ExitGames.Client.Photon;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public static class PhotonUtils
{
    public static void SendEvent(byte i_EventCode, Hashtable i_Params)
    {
        PhotonEventsProxy.RaiseEvent(i_EventCode, i_Params, true, new RaiseEventOptions { Receivers = ReceiverGroup.All }, false);
    }

    public static void SendEventImmediate(byte i_EventCode, Hashtable i_Params)
    {
        PhotonEventsProxy.RaiseEvent(i_EventCode, i_Params, true, new RaiseEventOptions { Receivers = ReceiverGroup.All }, true);
    }

    public static PhotonPlayer GetPhotonPlayer(int i_Id)
    {
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        if (photonPlayers != null)
        {
            for (int index = 0; index < photonPlayers.Length; ++index)
            {
                PhotonPlayer photonPlayer = photonPlayers[index];

                if (photonPlayer == null)
                    continue;

                int photonPlayerId = photonPlayer.ID;
                if (photonPlayerId == i_Id)
                {
                    return photonPlayer;
                }
            }
        }

        return null;
    }

    public static bool IsRoomFull()
    {
        if (PhotonNetwork.room == null)
        {
            return false;
        }

        return (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers);
    }

    public static void SetRoomOpened(bool i_Opened)
    {
        Room room = PhotonNetwork.room;

        if (room == null)
            return;

        room.IsOpen = i_Opened;
    } 

    public static void SetRoomVisible(bool i_Visible)
    {
        Room room = PhotonNetwork.room;

        if (room == null)
            return;

        room.IsVisible = i_Visible;
    }

    public static void ClearRoomCustomProperties()
    {
        Room room = PhotonNetwork.room;

        if (room == null)
            return;

        room.CustomProperties.Clear();
    }

    public static void SetRoomCustomProperty(object i_Key, object i_Value)
    {
        Room room = PhotonNetwork.room;

        if (room == null)
            return;

        Hashtable hashtable = new Hashtable();
        hashtable.Add(i_Key, i_Value);
        room.SetCustomProperties(hashtable);
    }

    public static bool TryGetRoomCustomProperty<T>(object i_Key, out T o_Value)
    {
        o_Value = default(T);

        Room room = PhotonNetwork.room;

        if (room == null)
        {
            return false;
        }

        Hashtable customProperties = room.CustomProperties;

        object valueObject;
        if (customProperties.TryGetValue(i_Key, out valueObject))
        {
            T outValue = (T)valueObject;
            o_Value = outValue;

            return true;
        }

        return false;
    }

    public static bool TryGetRoomCustomProperty<T>(RoomInfo i_Room, object i_Key, out T o_Value)
    {
        o_Value = default(T);

        if (i_Room == null)
        {
            return false;
        }

        Hashtable customProperties = i_Room.CustomProperties;

        object valueObject;
        if (customProperties.TryGetValue(i_Key, out valueObject))
        {
            T outValue = (T)valueObject;
            o_Value = outValue;

            return true;
        }

        return false;
    }

    public static int GetPlayerIndex(PhotonPlayer i_Player = null)
    {
        PhotonPlayer player = (i_Player == null) ? PhotonNetwork.player : i_Player;

        PhotonPlayer[] players = PhotonNetwork.playerList;

        if (players != null)
        {
            for (int index = 0; index < players.Length; ++index)
            {
                PhotonPlayer currentPlayer = players[index];
                if (currentPlayer == player)
                {
                    return index;
                }
            }
        }

        return -1;
    }

    public static void ClearPlayerCustomProperties()
    {
        ClearPlayerCustomProperties(PhotonNetwork.player);
    }

    public static void ClearPlayerCustomProperties(PhotonPlayer i_Player)
    {
        if (i_Player == null)
            return;

        i_Player.CustomProperties.Clear();
    }

    public static bool CheckPropertyOnAllPlayers<T>(object i_Key, T i_Value)
    {
        Room room = PhotonNetwork.room;
        
        if (room == null)
        {
            return false;
        }

        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        if (photonPlayers != null)
        {
            for (int index = 0; index < photonPlayers.Length; ++index)
            {
                PhotonPlayer photonPlayer = photonPlayers[index];

                if (photonPlayer == null)
                    continue;

                T propertyValue;
                if (PhotonUtils.TryGetPlayerCustomProperty<T>(photonPlayer, i_Key, out propertyValue))
                {
                    if (propertyValue.Equals(i_Value))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static void SetPlayerCustomProperty(object i_Key, object i_Value)
    {
        SetPlayerCustomProperty(PhotonNetwork.player, i_Key, i_Value);
    }

    public static void SetPlayerCustomProperty(PhotonPlayer i_Player, object i_Key, object i_Value)
    {
        if (i_Player == null)
            return;

        Hashtable hashtable = new Hashtable();
        hashtable.Add(i_Key, i_Value);
        i_Player.SetCustomProperties(hashtable);
    }

    public static bool TryGetPlayerCustomProperty<T>(object i_Key, out T o_Value)
    {
        return TryGetPlayerCustomProperty<T>(PhotonNetwork.player, i_Key, out o_Value);
    }

    public static bool TryGetPlayerCustomProperty<T>(PhotonPlayer i_Player, object i_Key, out T o_Value)
    {
        o_Value = default(T);

        if (i_Player == null)
        {
            return false;
        }

        Hashtable hashtable = i_Player.CustomProperties;

        object valueObject;
        if (hashtable.TryGetValue(i_Key, out valueObject))
        {
            T outValue = (T)valueObject;
            o_Value = outValue;

            return true;
        }

        return false;
    }

    public static IEnumerator DestroyAndUnallocateViewId(GameObject i_Go)
    {
        if (i_Go != null)
        {
            bool isMine = false;
            int viewId = 0;

            PhotonView photonView = i_Go.GetComponent<PhotonView>();
            if (photonView != null)
            {
                isMine = photonView.isMine;
                viewId = photonView.viewID;
                if (isMine)
                {
                    GameObject.Destroy(i_Go);

                    yield return null;

                    PhotonNetwork.UnAllocateViewID(viewId);
                }
            }
        }
    }
}

#endif // PHOTON