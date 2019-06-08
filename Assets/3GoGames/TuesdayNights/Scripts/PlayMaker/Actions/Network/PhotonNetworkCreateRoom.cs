#if PHOTON

using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Photon")]
    [Tooltip("Create photon room.")]
    public class PhotonNetworkCreateRoom : FsmStateAction
    {
        public FsmString roomName;

        [RequiredField]
        public FsmBool isOpen;
        [RequiredField]
        public FsmBool isVisible;
        [RequiredField]
        public FsmInt maxPlayers;

        public override void Reset()
        {
            roomName = "";

            isOpen = true;
            isVisible = true;

            maxPlayers = 8;
        }

        public override void OnEnter()
        {
            string room = (roomName != null && !roomName.IsNone) ? roomName.Value : "";
            bool open = (isOpen != null && !isOpen.IsNone) ? isOpen.Value : true;
            bool visible = (isVisible != null && !isVisible.IsNone) ? isVisible.Value : true;
            int players = (maxPlayers != null && !maxPlayers.IsNone) ? maxPlayers.Value : 8;

            RoomOptions options = new RoomOptions();
            options.IsOpen = open;
            options.IsVisible = visible;
            options.MaxPlayers = Convert.ToByte(players);

            PhotonNetwork.CreateRoom(room, options, TypedLobby.Default);

            Finish();
        }
    }
}

#endif // PHOTON