#if PHOTON

using UnityEngine;

using ExitGames.Client.Photon;

public static class PhotonEventsProxy
{
    private static bool offlineMode
    {
        get { return PhotonNetwork.offlineMode; }
    }

    public delegate void PhotonEventCallback(byte i_EventCode, object i_Content, int i_SenderId);

    public static PhotonEventCallback onEvent;

    // BUSINESS LOGIC

    public static void Initialize()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }

    public static void CleanUp()
    {
        PhotonNetwork.OnEventCall -= OnEvent;
    }

    public static void RaiseEvent(byte i_EventCode, Hashtable i_Params, bool i_SendReliable, RaiseEventOptions i_Options, bool i_Immediate = false)
    {
        if (!offlineMode)
        {
            PhotonNetwork.RaiseEvent(i_EventCode, i_Params, i_SendReliable, i_Options);
            if (i_Immediate)
            {
                PhotonNetwork.SendOutgoingCommands();
            }

            return;
        }

        RaiseLocalEvent(i_EventCode, i_Params, i_Options);
    }

    // INTERNALS

    private static void OnEvent(byte i_Eventcode, object i_Content, int i_SenderId)
    {
        if (onEvent != null)
        {
            onEvent(i_Eventcode, i_Content, i_SenderId);
        }   
    }

    private static void RaiseLocalEvent(byte i_EventCode, Hashtable i_Params, RaiseEventOptions i_Options)
    {
        if (i_Options.Receivers != ReceiverGroup.Others)
        {
            if (onEvent != null)
            {
                onEvent(i_EventCode, i_Params, PhotonNetwork.player.ID);
            }
        }
    }
}

#endif // PHOTON
