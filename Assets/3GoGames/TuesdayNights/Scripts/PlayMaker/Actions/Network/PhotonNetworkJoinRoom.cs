#if PHOTON

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Photon")]
    [Tooltip("Join photon room.")]
    public class PhotonNetworkJoinRoom : FsmStateAction
    {
        [RequiredField]
        public FsmString roomName; 

        public override void Reset()
        {
            roomName = "";
        }

        public override void OnEnter()
        {
            string room = (roomName != null && !roomName.IsNone) ? roomName.Value : "";

            if (room == "")
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                PhotonNetwork.JoinRoom(room);
            }

            Finish();
        }
    }
}

#endif // PHOTON