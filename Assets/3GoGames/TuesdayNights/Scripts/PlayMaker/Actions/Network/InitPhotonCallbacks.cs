#if PHOTON

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Photon")]
    public class InitPhotonCallbacks : FsmStateAction
    {
        public override void OnEnter()
        {
            PhotonCallbacks.InitializeMain();
            Finish();
        }
    }
}

#endif // PHOTON