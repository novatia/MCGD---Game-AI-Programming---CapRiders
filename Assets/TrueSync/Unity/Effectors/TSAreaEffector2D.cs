using UnityEngine;

using TrueSyncCommon;

namespace TrueSync
{
    [RequireComponent(typeof(TSTransform2D))]
    public sealed class TSAreaEffector2D : TrueSyncBehaviour
    {
        // Serializeable fields

        [Header("Area")]

        [SerializeField]
        private FP m_Width = 0;
        [SerializeField]
        private FP m_Height = 0;
        [SerializeField]
        private FP m_Angle = 0;

        [Header("Effector")]

        [SerializeField]
        private LayerMask m_LayerMask = 0;
        [SerializeField]
        private FP m_ForceAngle = FP.Zero;
        [SerializeField]
        private FP m_ForceMagnitude = FP.Zero;
        [SerializeField]
        private FP m_Drag = FP.Zero;

        [SerializeField]
        [AddTracking]
        private bool m_Running = false;

        [Header("Debug")]

        [SerializeField]
        private bool m_DrawGizmos = true;

        // ACCESSORS

        public FP width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        public FP height
        {
            get { return m_Height; }
            set { m_Height = value; }
        }

        public TSVector2 size
        {
            get { return new TSVector2(m_Width, m_Height); }
            set { m_Width = value.x; m_Height = value.y; }
        }

        public FP angle
        {
            get { return m_Angle; }
            set { m_Angle = value; }
        }

        public LayerMask layerMask
        {
            get { return m_LayerMask; }
            set { m_LayerMask = value; }
        }

        public FP forceAngle
        {
            get { return m_ForceAngle; }
            set { m_ForceAngle = value; }
        }

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

        public bool isRunning
        {
            get { return m_Running; }
            set { m_Running = value; }
        }

        // TrueSyncBehaviour's interface

        public override void OnSyncedUpdate()
        {
            base.OnSyncedUpdate();

            if (!m_Running)
                return;

            FP area = m_Width * m_Height;

            if (area <= FP.Zero)
                return;

            FP deltaTime = TrueSyncManager.deltaTimeMain;

            TSVector2 center = tsTransform2D.position;

            TSCollider2D[] colliders = TSPhysics2D.OverlapBoxAll(center, size, m_Angle, m_LayerMask);

            if (colliders != null)
            {
                if (colliders.Length > 0)
                {
                    for (int index = 0; index < colliders.Length; ++index)
                    {
                        TSCollider2D currentCollider = colliders[index];

                        if (currentCollider == null)
                            continue;

                        TSRigidBody2D rigidbody = currentCollider.GetComponent<TSRigidBody2D>();
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
            }
        }

        // MonoBehaviour's interface

        private void Awake()
        {
            sortOrder = SortOrder.s_SortOrder_AreaEffector2D; // Set sort order.
        }

        private void OnDrawGizmosSelected()
        {
            if (!m_DrawGizmos)
                return;

            Color oldColor = Gizmos.color;

            Gizmos.color = Color.white;

            Vector2 center = tsTransform2D.position.ToVector();

            float halfWidth = m_Width.AsFloat() / 2f;
            float halfHeight = m_Height.AsFloat() / 2f;

            Vector2 topLeft = new Vector2(center.x - halfWidth, center.y + halfHeight);
            Vector2 topRight = new Vector2(center.x + halfWidth, center.y + halfHeight);
            Vector2 bottomLeft = new Vector2(center.x - halfWidth, center.y - halfHeight);
            Vector2 bottomRight = new Vector3(center.x + halfWidth, center.y - halfHeight);

            Vector2 localTopLeft = topLeft - center;
            Vector2 localTopRight = topRight - center;
            Vector2 localBottomLeft = bottomLeft - center;
            Vector2 localBottomRight = bottomRight - center;

            if (m_Angle != FP.Zero)
            {
                FP angleFP = MathFP.GetNormalizedAngle(m_Angle);
                float angle = angleFP.AsFloat();

                localTopLeft = localTopLeft.Rotate(angle);
                localTopRight = localTopRight.Rotate(angle);
                localBottomLeft = localBottomLeft.Rotate(angle);
                localBottomRight = localBottomRight.Rotate(angle);
            }

            Gizmos.DrawLine(center + localTopLeft, center + localTopRight);
            Gizmos.DrawLine(center + localTopRight, center + localBottomRight);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(center + localBottomRight, center + localBottomLeft);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(center + localBottomLeft, center + localTopLeft);

            Gizmos.color = oldColor;
        }

        // LOGIC

        public void SetRunning(bool i_Running)
        {
            m_Running = i_Running;
        }
    }
}