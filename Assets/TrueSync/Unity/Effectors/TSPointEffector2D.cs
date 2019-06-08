using UnityEngine;

using TrueSyncCommon;

namespace TrueSync
{
    [RequireComponent(typeof(TSTransform2D))]
    public sealed class TSPointEffector2D : TrueSyncBehaviour
    {
        // Serializeable fields

        [Header("Effector")]

        [SerializeField]
        private FP m_Radius = FP.FromFloat(0.85f);
        [SerializeField]
        private LayerMask m_LayerMask = 0;
        [SerializeField]
        private FP m_ForceMagnitude = FP.Zero;
        [SerializeField]
        private FP m_Drag = FP.Zero;

        [SerializeField]
        [AddTracking]
        private bool m_Running = false;

        // ACCESSORS

        public FP forceMagnitude
        {
            get { return m_ForceMagnitude; }
            set { m_ForceMagnitude = value; }
        }

        public FP drag
        {
            get { return m_Drag; }
            set { m_Drag = value; }
        }

        public FP radius
        {
            get
            {
                return m_Radius;
            }

            set
            {
                m_Radius = value;
            }
        }

        public bool isRunning
        {
            get
            {
                return m_Running;
            }
        }

        // TrueSyncBehaviour's interface

        public override void OnSyncedUpdate()
        {
            base.OnSyncedUpdate();

            if (!m_Running)
                return;

            FP deltaTime = TrueSyncManager.deltaTimeMain;

            TSCollider2D[] colliders = TSPhysics2D.OverlapCircleAll(tsTransform2D.position, radius, m_LayerMask);

            if (colliders != null)
            {
                if (colliders.Length > 0)
                {
                    for (int index = 0; index < colliders.Length; ++index)
                    {
                        TSCollider2D currentCollider = colliders[index];

                        if (currentCollider == null)
                            continue;

                        TSTransform2D transform2d = currentCollider.tsTransform;

                        TSVector2 direction = transform2d.position - tsTransform2D.position;
                        direction = direction.normalized;

                        TSRigidBody2D rigidbody = currentCollider.GetComponent<TSRigidBody2D>();
                        if (rigidbody != null)
                        {
                            TSVector2 currentVelocity = rigidbody.velocity;
                            currentVelocity *= FP.One / (FP.One + (deltaTime * m_Drag));
                            rigidbody.velocity = currentVelocity;

                            rigidbody.AddForce(direction * m_ForceMagnitude);
                        }
                    }
                }
            }
        }

        // MonoBehaviour's interface

        private void Awake()
        {
            sortOrder = SortOrder.s_SortOrder_PointEffector2D; // Set sort order.
        }

        // LOGIC

        public void SetRunning(bool i_Running)
        {
            m_Running = i_Running;
        }
    }
}