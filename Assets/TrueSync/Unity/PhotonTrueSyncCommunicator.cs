using System;

namespace TrueSync
{
    /**
     *  @brief Truesync's {@link ICommunicator} implementation based on PUN. 
     **/
    public class PhotonTrueSyncCommunicator : ICommunicator
    {
        private LoadBalancingPeer m_LoadBalancingPeer;

        private static PhotonNetwork.EventCallback s_LastEventCallback;

        /**
         *  @brief Instantiates a new PhotonTrueSyncCommunicator based on a Photon's LoadbalancingPeer. 
         *  
         *  @param loadBalancingPeer Instance of a Photon's LoadbalancingPeer.
         **/
        internal PhotonTrueSyncCommunicator(LoadBalancingPeer i_LoadBalancingPeer)
        {
            m_LoadBalancingPeer = i_LoadBalancingPeer;
        }

        public int RoundTripTime()
        {
            return m_LoadBalancingPeer.RoundTripTime;
        }

        public void OpRaiseEvent(byte i_EventCode, object i_Message, bool i_Reliable, int[] i_ToPlayers)
        {
            if (m_LoadBalancingPeer.PeerState != ExitGames.Client.Photon.PeerStateValue.Connected)
            {
                return;
            }

            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.TargetActors = i_ToPlayers;

            m_LoadBalancingPeer.OpRaiseEvent(i_EventCode, i_Message, i_Reliable, eventOptions);
        }

        public void AddEventListener(OnEventReceived i_OnEventReceived)
        {
            if (s_LastEventCallback != null)
            {
                PhotonNetwork.OnEventCall -= s_LastEventCallback;
            }

            s_LastEventCallback = delegate (byte eventCode, object content, int senderId) { i_OnEventReceived(eventCode, content); };
            PhotonNetwork.OnEventCall += s_LastEventCallback;
        }
    }
}
