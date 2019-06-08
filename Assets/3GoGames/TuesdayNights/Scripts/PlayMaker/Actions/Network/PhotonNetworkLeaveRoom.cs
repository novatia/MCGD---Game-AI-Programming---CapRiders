#if PHOTON

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Photon")]
    [Tooltip("Leave current photon room.")]
    public class PhotonNetworkLeaveRoom : FsmStateAction
    {
        public override void OnEnter()
        {
            PhotonNetwork.LeaveRoom();
            Finish();
        }
    }
}

#endif // PHOTON