using System;
using UnityEngine;

namespace UnityStandardAssets.Cameras
{
    public abstract class AbstractTargetFollower : MonoBehaviour
    {
        public enum UpdateType      // The available methods of updating are:
        {
            FixedUpdate,            // Update in FixedUpdate (for tracking rigid bodies).
            LateUpdate,             // Update in LateUpdate. (for tracking objects that are moved in Update)
            ManualUpdate,           // User must call to update camera
        }

        [SerializeField] protected Transform m_Target = null;            // The target object to follow
        [SerializeField] private bool m_AutoTargetPlayer = true;  // Whether the rig should automatically target the player.
        [SerializeField] private UpdateType m_UpdateType = UpdateType.LateUpdate;         // stores the selected update type

        protected Rigidbody targetRigidbody = null;

        public virtual void SetTarget(Transform newTransform)
        {
            m_Target = newTransform;
        }

        public Transform Target
        {
            get { return m_Target; }
        }

        protected virtual void Start()
        {
            // If auto targeting is used, find the object tagged "Player"
            // any class inheriting from this should call base.Start() to perform this action!
            if (m_AutoTargetPlayer)
            {
                FindAndTargetPlayer();
            }

            if (m_Target == null) 
                return;
            
            targetRigidbody = m_Target.GetComponent<Rigidbody>();
        }

        protected abstract void FollowTarget(float deltaTime);

        private void FixedUpdate()
        {
            if (m_AutoTargetPlayer && (m_Target == null || !m_Target.gameObject.activeSelf))
            {
                FindAndTargetPlayer();
            }

            if (m_UpdateType == UpdateType.FixedUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (m_AutoTargetPlayer && (m_Target == null || !m_Target.gameObject.activeSelf))
            {
                FindAndTargetPlayer();
            }

            if (m_UpdateType == UpdateType.LateUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        public void ManualUpdate()
        {
            if (m_AutoTargetPlayer && (m_Target == null || !m_Target.gameObject.activeSelf))
            {
                FindAndTargetPlayer();
            }

            if (m_UpdateType == UpdateType.ManualUpdate)
            {
                FollowTarget(Time.deltaTime);
            }
        }

        private void FindAndTargetPlayer()
        {
            // auto target an object tagged player, if no target has been assigned
            var targetObj = GameObject.FindGameObjectWithTag("Player");
            if (targetObj)
            {
                SetTarget(targetObj.transform);
            }
        }
    }
}
