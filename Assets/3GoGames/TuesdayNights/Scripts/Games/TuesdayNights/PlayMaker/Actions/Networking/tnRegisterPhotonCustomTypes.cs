#if PHOTON

using ExitGames.Client.Photon;

using BaseMatchEvents;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Register photon custom types.")]
    public class tnRegisterPhotonCustomTypes : FsmStateAction
    {
        public override void OnEnter()
        {
            PhotonPeer.RegisterType(typeof(tnMultiplayerIndexTable), (byte)'M', tnMultiplayerIndexTable.PhotonSerialize, tnMultiplayerIndexTable.PhotonDeserialize);

            Finish();
        }
    }
}

#endif // PHOTON