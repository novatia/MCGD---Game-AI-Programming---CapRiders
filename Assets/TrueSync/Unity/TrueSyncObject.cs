using UnityEngine;

namespace TrueSync
{
    public class TrueSyncObject : MonoBehaviour, ITrueSyncBehaviour
    {
        private TrueSyncBehaviour[] m_Behaviours = null;

        private TSTransform2D[] m_Transforms = null;
        private TSCollider2D[] m_Colliders = null;

        private TSRigidBody2D[] m_RigidBodies = null;

        private int m_RegisteredBehaviourCount = 0;

        private int m_RegisteredTransformCount = 0;
        private int m_RegisteredColliderCount = 0;

        private int m_RegisteredRigidBodyCount = 0;

        // ACCESSORS

        public int behaviourCount
        {
            get
            {
                if (m_Behaviours != null)
                {
                    return m_Behaviours.Length;
                }

                return 0;
            }
        }

        public int transformCount
        {
            get
            {
                if (m_Transforms != null)
                {
                    return m_Transforms.Length;
                }

                return 0;
            }
        }

        public int colliderCount
        {
            get
            {
                if (m_Colliders != null)
                {
                    return m_Colliders.Length;
                }

                return 0;
            }
        }

        public int rigidbodyCount
        {
            get
            {
                if (m_RigidBodies != null)
                {
                    return m_RigidBodies.Length;
                }

                return 0;
            }
        }

        public int registeredBehaviourCount
        {
            get
            {
                return m_RegisteredBehaviourCount;
            }
        }

        public int registeredTransformCount
        {
            get
            {
                return m_RegisteredTransformCount;
            }
        }

        public int registeredColliderCount
        {
            get
            {
                return m_RegisteredColliderCount;
            }
        }

        public int registeredRigidBodyCount
        {
            get
            {
                return m_RegisteredRigidBodyCount;
            }
        }

        // MonoBehaviour's INTERFACE

        private void Awake()
        {
            m_Behaviours = GetComponentsInChildren<TrueSyncBehaviour>();

            m_Transforms = GetComponentsInChildren<TSTransform2D>();
            m_Colliders = GetComponentsInChildren<TSCollider2D>();

            m_RigidBodies = GetComponentsInChildren<TSRigidBody2D>();
        }

        // LOGIC

        public TrueSyncBehaviour GetTrueSyncBehaviourByIndex(int i_Index)
        {
            if (m_Behaviours == null || (i_Index < 0 || i_Index >= m_Behaviours.Length))
            {
                return null;
            }

            return m_Behaviours[i_Index];
        }

        public TSTransform2D GetTransformByIndex(int i_Index)
        {
            if (m_Transforms == null || (i_Index < 0 || i_Index >= m_Transforms.Length))
            {
                return null;
            }

            return m_Transforms[i_Index];
        }

        public TSCollider2D GetColliderByIndex(int i_Index)
        {
            if (m_Colliders == null || i_Index < 0 || i_Index >= m_Colliders.Length)
            {
                return null;
            }

            return m_Colliders[i_Index];
        }

        public TSRigidBody2D GetRigidBodyByIndex(int i_Index)
        {
            if (m_RigidBodies == null || i_Index < 0 || i_Index >= m_RigidBodies.Length)
            {
                return null;
            }

            return m_RigidBodies[i_Index];
        }

        public void SetOwnerId(int i_Id)
        {
            for (int index = 0; index < behaviourCount; ++index)
            {
                TrueSyncBehaviour behaviour = GetTrueSyncBehaviourByIndex(index);

                if (behaviour == null)
                    continue;

                behaviour.ownerIndex = i_Id;
            }
        }

        public void OnRegistration()
        {
            m_RegisteredBehaviourCount = behaviourCount;

            m_RegisteredTransformCount = transformCount;
            m_RegisteredColliderCount = colliderCount;

            m_RegisteredRigidBodyCount = rigidbodyCount;
        }

        // ITrueSyncBehaviour's interface

        public void SetGameInfo(TSPlayerInfo localOwner, int numberOfPlayers)
        {
            // Nothing to do. Exactly like a TrueSyncBehaviour.
        }
    }
}