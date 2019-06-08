using System;
using UnityEngine;

namespace TrueSync
{
    /**
     *  @brief Abstract collider for 2D shapes.
     **/
    [RequireComponent(typeof(TSTransform2D))]
    [Serializable]
    [ExecuteInEditMode]
    public abstract class TSCollider2D : MonoBehaviour, ICollider
    {
        [SerializeField]
        private TSPhysicsMaterial2D m_PhysicsMaterial = null;

        [SerializeField]
        private TSVector2 m_Center = TSVector2.zero;

        [SerializeField]
        public bool m_IsTrigger = false;

        private TSTransform2D m_TSTransform = null;
        private TSRigidBody2D m_TSRigidBody = null;

        private TSMaterial m_TSMaterial = null;

        private Physics2D.Body m_Body = null;
        private Physics2D.Shape m_Shape = null;

        public TSPhysicsMaterial2D tsPhysicsMaterial
        {
            get
            {
                return m_PhysicsMaterial;
            }
        }

        public TSMaterial tsMaterial
        {
            get
            {
                return m_TSMaterial;
            }
        }

        /**
         *  @brief Center of the collider shape.
         **/
        public TSVector2 center
        {
            get
            {
                return m_Center;
            }

            set
            {
                m_Center = value;
            }
        }

        /**
         *  @brief Returns the {@link TSTransform2D} attached.
         **/
        public TSTransform2D tsTransform
        {
            get
            {
                return m_TSTransform;
            }
        }

        /**
         *  @brief Returns the {@link TSRigidBody} attached.
         **/
        public TSRigidBody2D tsRigidBody
        {
            get
            {
                return m_TSRigidBody;
            }
        }

        public Physics2D.Body body
        {
            get
            {
                return m_Body;
            }
        }

        /**
         *  @brief Returns true if the body was already initialized.
         **/
        public bool isBodyInitialized
        {
            get
            {
                return (m_Body != null);
            }
        }

        /**
         *  @brief Shape used by a collider.
         **/
        public Physics2D.Shape shape
        {
            get
            {
                return m_Shape;
            }
        }

        /**
         *  @brief Creates the shape related to a concrete implementation of TSCollider.
         **/
        public virtual Physics2D.Shape CreateShape()
        {
            return null;
        }

        public virtual Physics2D.Shape[] CreateShapes()
        {
            return new Physics2D.Shape[0];
        }

        // MonoBehaviour's INTERFACE

        public void Awake()
        {
            m_TSTransform = GetComponent<TSTransform2D>();
            m_TSRigidBody = GetComponent<TSRigidBody2D>();
        }

        public virtual void OnDrawGizmos()
        {
            if (!enabled)
            {
                return;
            }

            Vector3 position = (m_Body != null) ? (m_Body.TSPosition.ToVector()) : (transform.position + center.ToVector());
            Quaternion rotation = (m_Body != null) ? (Quaternion.Euler(0f, 0f, (m_Body.Rotation * FP.Rad2Deg).AsFloat())) : (transform.rotation);

            Gizmos.color = Color.yellow;

            Matrix4x4 cubeTransform = Matrix4x4.TRS(position, rotation, GetGizmosSize());
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;

            Gizmos.matrix *= cubeTransform;

            DrawGizmos();

            Gizmos.matrix = oldGizmosMatrix;
        }

        // BUSINESS LOGIC

        public void SetCollisionLayer(int i_Layer)
        {
            if (m_Body != null)
            {
                int layerMask = (1 << i_Layer);
                int collisionMask = LayerCollisionMatrix.ComputeCollisionMask(i_Layer);

                m_Body.CollisionCategories = (Physics2D.Category)layerMask;
                m_Body.CollidesWith = (Physics2D.Category)collisionMask;

                m_Body.CollisionGroup = 0; // Forced to zero, unused.
            }
        }

        public void SetIsSensor(bool i_IsSensor)
        {
            if (m_Body != null)
            {
                m_Body.IsSensor = i_IsSensor;
            }
        }

        // INTERNALS

        private void CreateBodyFixture(Physics2D.Body i_Body, Physics2D.Shape i_Shape, FP i_Friction, FP i_Restitution)
        {
            Physics2D.Fixture fixture = i_Body.CreateFixture(i_Shape);

            fixture.Friction = i_Friction;
            fixture.Restitution = i_Restitution;
        }

        private void CreateBody(Physics2D.World i_World)
        {
            Physics2D.Body body = Physics2D.BodyFactory.CreateBody(i_World);

            // Get collision/restitution data from physics material.

            FP materialFriction = FP.Zero;
            FP materialRestitution = FP.Zero;

            if (m_PhysicsMaterial != null)
            {
                materialFriction = m_PhysicsMaterial.friction;
                materialRestitution = m_PhysicsMaterial.restitution;
            }
            else
            {
                // Check for a TSMaterial2D. Note: this is deprecated.

                m_TSMaterial = GetComponent<TSMaterial>();

                if (m_TSMaterial != null)
                {
                    materialFriction = m_TSMaterial.friction;
                    materialRestitution = m_TSMaterial.restitution;
                }
            }

            // Create collision shape(s).

            Physics2D.Shape shape = CreateShape();
            if (shape != null)
            {
                CreateBodyFixture(body, shape, materialFriction, materialRestitution);
            }
            else
            {
                Physics2D.Shape[] shapes = CreateShapes();
                for (int index = 0, length = shapes.Length; index < length; index++)
                {
                    CreateBodyFixture(body, shapes[index], materialFriction, materialRestitution);
                }
            }

            // Setup RigidBody.

            if (m_TSRigidBody == null)
            {
                body.BodyType = Physics2D.BodyType.Static;
            }
            else
            {
                if (m_TSRigidBody.isKinematic)
                {
                    body.BodyType = Physics2D.BodyType.Kinematic;
                }
                else
                {
                    body.BodyType = Physics2D.BodyType.Dynamic;
                    body.IgnoreGravity = !m_TSRigidBody.useGravity;
                }

                if (m_TSRigidBody.mass <= 0)
                {
                    m_TSRigidBody.mass = 1;
                }

                body.FixedRotation = m_TSRigidBody.freezeZAxis;
                body.Mass = m_TSRigidBody.mass;

                body.TSLinearDrag = m_TSRigidBody.linearDrag;
                body.TSAngularDrag = m_TSRigidBody.angularDrag;
            }

            m_Body = body;
            m_Shape = shape;

            SetIsSensor(m_IsTrigger);
            SetCollisionLayer(gameObject.layer);
        }

        /**
         *  @brief Initializes Shape and RigidBody and sets initial values to position and orientation based on Unity's transform.
         **/
        public void Initialize(Physics2D.World i_World)
        {
            CreateBody(i_World);
        }

        /**
         *  @brief Returns the gizmos size.
         **/
        protected abstract Vector3 GetGizmosSize();

        /**
         *  @brief Draws the specific gizmos of concrete collider (for example "Gizmos.DrawWireCube" for a {@link TSBoxCollider}).
         **/
        protected abstract void DrawGizmos();
    }
}