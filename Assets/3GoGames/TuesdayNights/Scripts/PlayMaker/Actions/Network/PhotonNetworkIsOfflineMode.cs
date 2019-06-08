#if PHOTON

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Photon")]
    [Tooltip("Check weather photon is in offline mode or not.")]
    public class PhotonNetworkIsOfflineMode : FsmStateAction
    {
        [Tooltip("Event to send if player is in room.")]
        public FsmEvent isTrue;
        [Tooltip("Event to send if player isn't in room.")]
        public FsmEvent isFalse;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void Reset()
        {
            isTrue = null;
            isFalse = null;

            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoCheck();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoCheck();
        }

        // INTERNALS

        private void DoCheck()
        {
            Fsm.Event(PhotonNetwork.offlineMode ? isTrue : isFalse);
        }
    }
}

#endif // PHOTON