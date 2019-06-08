#if PHOTON

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("Photon")]
    [Tooltip("Disconnect from Photon.")]
    public class PhotonNetworkDisconnect : FsmStateAction
    {
        public FsmEvent disconnectedEvent;

        public override void Reset()
        {
            disconnectedEvent = null;
        }

        public override void OnEnter()
        {
            PhotonCallbacks.onDisconnectedFromPhotonMain += OnDisconnectedFromPhoton;

            DisconnectFromPhoton();

            Finish();
        }

        public override void OnExit()
        {
            PhotonCallbacks.onDisconnectedFromPhotonMain -= OnDisconnectedFromPhoton;
        }

        private void DisconnectFromPhoton()
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.Disconnect();
            }
            else
            {
                OnDisconnectedFromPhoton();
            }
        }

        private void OnDisconnectedFromPhoton()
        {
            Fsm.Event(disconnectedEvent);
        }
    }
}

#endif // PHOTON