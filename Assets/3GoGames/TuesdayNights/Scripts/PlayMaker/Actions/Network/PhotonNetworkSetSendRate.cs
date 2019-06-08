#if PHOTON

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Photon")]
    [Tooltip("Set PhotonNetwork send rate (sendRate and sendRateOnSerialize fields)")]
    public class PhotonNetworkSetSendRate : FsmStateAction
    {
        [RequiredField]
        public FsmInt sendRate;
        [RequiredField]
        public FsmInt sendRateOnSerialize;

        public override void Reset()
        {
            sendRate = 20;
            sendRateOnSerialize = 20;
        }

        public override void OnEnter()
        {
            PhotonNetwork.sendRate = sendRate.Value;
            PhotonNetwork.sendRateOnSerialize = sendRateOnSerialize.Value;

            Finish();
        }
    }
}

#endif // PHOTON