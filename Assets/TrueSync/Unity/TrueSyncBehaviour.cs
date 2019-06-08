using UnityEngine;

namespace TrueSync
{
    /**
     *  @brief Represents each player's behaviour simulated on every machine connected to the game.
     */
    public abstract class TrueSyncBehaviour : MonoBehaviour, ITrueSyncBehaviourGamePlay, ITrueSyncBehaviourCallbacks
    {
        /**
         * @brief Number of players connected to the game.
         **/
        private int m_NumberOfPlayers;

        public int numberOfPlayers
        {
            get
            {
                return m_NumberOfPlayers;
            }

            set
            {
                m_NumberOfPlayers = value;
            }
        }

        /**
         *  @brief Index of the owner at initial players list.
         **/
        [SerializeField]
        private int m_OwnerIndex = -1;

        public int ownerIndex
        {
            get
            {
                return m_OwnerIndex;
            }

            set
            {
                m_OwnerIndex = value;
            }
        }

        /**
         *  @brief Sort order.
         **/
        [SerializeField]
        private int m_SortOrder = 0;

        public int sortOrder
        {
            get
            {
                return m_SortOrder;
            }

            set
            {
                m_SortOrder = value;
            }
        }

        /**
         *  @brief Basic info about the owner of this behaviour.
         **/
        private TSPlayerInfo m_Owner;

        public TSPlayerInfo owner
        {
            get
            {
                return m_Owner;
            }

            set
            {
                m_Owner = value;
            }
        }

        /**
         *  @brief Basic info about the local player.
         **/
        private TSPlayerInfo m_LocalOwner;

        public TSPlayerInfo localOwner
        {
            get
            {
                return m_LocalOwner;
            }

            set
            {
                m_LocalOwner = value;
            }
        }

        /**
         *  @brief Return if this object is contorlled by local player.
         **/
        public bool isMine
        {
            get
            {
                return ((m_Owner != null && m_LocalOwner != null) ? (m_Owner.Id == m_LocalOwner.Id) : (false));
            }
        }

        /**
         *  @brief Object transform.
         **/
        private TSTransform2D m_Transform;

        public TSTransform2D tsTransform2D
        {
            get
            {
                if (m_Transform == null)
                {
                    m_Transform = this.GetComponent<TSTransform2D>();
                }

                return m_Transform;
            }
        }

        /**
         * @brief It is not called for instances of {@link TrueSyncBehaviour}.
         **/
        public void SetGameInfo(TSPlayerInfo i_LocalOwner, int i_NumberOfPlayers) { }

        /**
         * @brief Called once when the object becomes active.
         **/
        public virtual void OnSyncedStart() { }

        /**
         * @brief Called once on instances owned by the local player after the object becomes active.
         **/
        public virtual void OnSyncedStartLocalPlayer() { }

        /**
         * @brief Called when the game has paused.
         **/
        public virtual void OnGamePaused() { }

        /**
         * @brief Called when the game has unpaused.
         **/
        public virtual void OnGameUnPaused() { }

        /**
         * @brief Called when the game has ended.
         **/
        public virtual void OnGameEnded() { }

        /**
         *  @brief Called before {@link #OnSyncedUpdate}.
         *  
         *  Called once every lockstepped frame.
         */
        public virtual void OnPreSyncedUpdate() { }

        /**
         *  @brief Game updates goes here.
         *  
         *  Called once every lockstepped frame.
         */
        public virtual void OnSyncedUpdate() { }

        /**
         *  @brief Game updates goes here.
         *  
         *  Called once every lockstepped frame.
         */
        public virtual void OnLateSyncedUpdate() { }

        /**
         *  @brief Get local player data.
         *  
         *  Called once every lockstepped frame.
         */
        public virtual void OnSyncedInput() { }

        /**
         * @brief OnSyncedCollisionEnter.
         **/
        public virtual void OnSyncedCollisionEnter(TSCollision2D i_Collision) { }

        /**
         * @brief OnSyncedCollisionStay.
         **/
        public virtual void OnSyncedCollisionStay(TSCollision2D i_Collision) { }

        /**
         * @brief OnSyncedCollisionExit.
         **/
        public virtual void OnSyncedCollisionExit(TSCollision2D i_Collision) { }

        /**
         * @brief OnSyncedTriggerEnter.
         **/
        public virtual void OnSyncedTriggerEnter(TSCollision2D i_Collision) { }

        /**
         * @brief OnSyncedTriggerStay.
         **/
        public virtual void OnSyncedTriggerStay(TSCollision2D i_Collision) { }

        /**
         * @brief OnSyncedTriggerExit.
         **/
        public virtual void OnSyncedTriggerExit(TSCollision2D i_Collision) { }

        /**
         * @brief Callback called when a player get disconnected.
         **/
        public virtual void OnPlayerDisconnection(int i_PlayerId) { }
    }
}