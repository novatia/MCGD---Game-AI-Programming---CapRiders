using UnityEngine;

using TrueSyncCommon;

namespace TrueSync
{
    [RequireComponent(typeof(TSCollider2D))]
    public class TSColliderEffector2D : TrueSyncBehaviour
    {
        // Serializabled fields

        [Header("Effector")]

        [SerializeField]
        private FP m_ForceAngle = FP.Zero;
        [SerializeField]
        private FP m_ForceMagnitude = FP.Zero;
        [SerializeField]
        private FP m_Drag = FP.Zero;

        [SerializeField]
        [AddTracking]
        private bool m_Running = true;

        // TrueSyncBehaviour's interface

        public override void OnSyncedTriggerStay(TSCollision2D i_Collision)
        {
            base.OnSyncedCollisionStay(i_Collision);

            GameObject otherGo = i_Collision.gameObject;

            if (otherGo == null)
                return;

            bool valid = ValidateGameObject(otherGo);

            if (!valid)
                return;

            if (m_Running)
            {
                FP deltaTime = TrueSyncManager.deltaTimeMain;

                TSRigidBody2D rigidbody = otherGo.GetComponent<TSRigidBody2D>();
                if (rigidbody != null)
                {
                    // Drag

                    TSVector2 currentVelocity = rigidbody.velocity;
                    currentVelocity *= FP.One / (FP.One + (deltaTime * m_Drag));
                    rigidbody.velocity = currentVelocity;

                    // Force

                    FP angle = MathFP.ClampAngle(m_ForceAngle, FP.Zero, 360f);

                    TSVector2 forceDirection = TSVector2.right;
                    forceDirection = forceDirection.Rotate(angle);

                    rigidbody.AddForce(forceDirection * m_ForceMagnitude);
                }
            }
        }

        // MonoBehaviour's interface

        private void Awake()
        {
            sortOrder = SortOrder.s_SortOrder_ColliderEffector2D; // Set sort order.
        }

        // LOGIC

        public void SetRunning(bool i_Running)
        {
            m_Running = i_Running;
        }

        // INTERNALS

        private bool ValidateGameObject(GameObject i_GameObject)
        {
            if (i_GameObject == null)
            {
                return false;
            }

            return OnValidateGameObject(i_GameObject);
        }

        // Virtual

        protected virtual bool OnValidateGameObject(GameObject i_GameObject)
        {
            return true;
        }
    }
}
