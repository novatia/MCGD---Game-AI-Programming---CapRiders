#if PHOTON

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Photon")]
    [Tooltip("Set PhotonNetwork offline mode")]
    public class SetPhotonNetworkOfflineMode : FsmStateAction
    {
        [RequiredField]
        public FsmBool offlineMode;

        public override void Reset()
        {
            offlineMode = false;
        }

        public override void OnEnter()
        {
            PhotonNetwork.offlineMode = offlineMode.Value;

            Finish();
        }
    }
}

#endif // PHOTON