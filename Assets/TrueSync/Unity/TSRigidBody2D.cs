using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync
{
    /**
     *  @brief Represents a physical 2D rigid body.
     **/
    [RequireComponent(typeof(TSTransform2D))]
    [RequireComponent(typeof(TSCollider2D))]
    [AddComponentMenu("TrueSync/Physics/TSRigidBody2D", 11)]
    public class TSRigidBody2D : MonoBehaviour
    {
        public enum InterpolateMode { None, Interpolate, Extrapolate };

        [FormerlySerializedAs("mass")]
        [SerializeField]
        private FP m_Mass = 1;

        /**
         *  @brief Mass of the body. 
         **/
        public FP mass
        {
            get
            {
                if (tsCollider.isBodyInitialized)
                {
                    return tsCollider.body.Mass;
                }

                return m_Mass;
            }

            set
            {
                m_Mass = value;

                if (tsCollider.isBodyInitialized)
                {
                    tsCollider.body.Mass = value;
                }
            }
        }

        [SerializeField]
        private bool m_UseGravity = true;

        /**
         *  @brief If true it uses gravity force. 
         **/
        public bool useGravity
        {
            get
            {
                if (tsCollider.isBodyInitialized)
                {
                    return tsCollider.body.TSAffectedByGravity;
                }

                return m_UseGravity;
            }

            set
            {
                m_UseGravity = value;

                if (tsCollider.isBodyInitialized)
                {
                    tsCollider.body.TSAffectedByGravity = m_UseGravity;
                }
            }
        }

        [SerializeField]
        private bool m_IsKinematic = false;

        /**
         *  @brief If true it doesn't get influences from external forces. 
         **/
        public bool isKinematic
        {
            get
            {
                if (tsCollider.isBodyInitialized)
                {
                    return tsCollider.body.TSIsKinematic;
                }

                return m_IsKinematic;
            }

            set
            {
                m_IsKinematic = value;

                if (tsCollider.isBodyInitialized)
                {
                    tsCollider.body.TSIsKinematic = m_IsKinematic;
                }
            }
        }

        [SerializeField]
        private FP m_LinearDrag = FP.Zero;

        public FP linearDrag
        {
            get
            {
                if (tsCollider.isBodyInitialized)
                {
                    return tsCollider.body.TSLinearDrag;
                }

                return m_LinearDrag;
            }

            set
            {
                m_LinearDrag = value;

                if (tsCollider.isBodyInitialized)
                {
                    tsCollider.body.TSLinearDrag = m_LinearDrag;
                }
            }
        }

        [SerializeField]
        private FP m_AngularDrag = FP.Zero;

        public FP angularDrag
        {
            get
            {
                if (tsCollider.isBodyInitialized)
                {
                    return tsCollider.body.TSAngularDrag;
                }

                return m_AngularDrag;
            }

            set
            {
                m_AngularDrag = value;

                if (tsCollider.isBodyInitialized)
                {
                    tsCollider.body.TSAngularDrag = m_AngularDrag;
                }
            }
        }

        [SerializeField]
        private InterpolateMode m_Interpolation = InterpolateMode.None;

        /**
         *  @brief Interpolation mode that should be used. 
         **/
        public InterpolateMode interpolation
        {
            get
            {
                return m_Interpolation;
            }

            set
            {
                m_Interpolation = value;
            }
        }

        /**
         *  @brief If true it freezes Z rotation of the RigidBody (it only appears when in 2D Physics).
         **/
        [SerializeField]
        private bool m_FreezeZAxis = false;

        public bool freezeZAxis
        {
            get
            {
                return m_FreezeZAxis;
            }

            set
            {
                m_FreezeZAxis = value;
            }
        }

        private TSCollider2D m_TSCollider = null;

        public TSCollider2D tsCollider
        {
            get
            {
                if (m_TSCollider == null)
                {
                    m_TSCollider = GetComponent<TSCollider2D>();
                }

                return m_TSCollider;
            }
        }

        private TSTransform2D m_TSTransform = null;

        private TSTransform2D tsTransform
        {
            get
            {
                if (m_TSTransform == null)
                {
                    m_TSTransform = GetComponent<TSTransform2D>();
                }

                return m_TSTransform;
            }
        }

        /**
         *  @brief Position of the body. 
         **/
        public TSVector2 position
        {
            get
            {
                if (m_TSCollider.isBodyInitialized)
                {
                    return m_TSCollider.body.TSPosition - m_TSCollider.center;
                }

                return TSVector2.zero;
            }

            set
            {
                tsTransform.position = value;
            }
        }

        /**
         *  @brief Orientation of the body. 
         **/
        public FP rotation
        {
            get
            {
                if (m_TSCollider.isBodyInitialized)
                {
                    return m_TSCollider.body.TSOrientation * FP.Rad2Deg;
                }

                return FP.Zero;
            }

            set
            {
                tsTransform.rotation = value;
            }
        }

        public TSVector2 velocity
        {
            get
            {
                if (tsCollider.isBodyInitialized)
                {
                    return tsCollider.body.TSLinearVelocity;
                }

                return TSVector2.zero;
            }

            set
            {
                if (tsCollider.isBodyInitialized)
                {
                    tsCollider.body.TSLinearVelocity = value;
                }
            }
        }

        public FP angularVelocity
        {
            get
            {
                if (tsCollider.isBodyInitialized)
                {
                    return tsCollider.body.TSAngularVelocity;
                }

                return FP.Zero;
            }

            set
            {
                if (tsCollider.isBodyInitialized)
                {
                    tsCollider.body.TSAngularVelocity = value;
                }
            }
        }

        [AddTracking]
        private Vector2 m_PrevCachedPosition = Vector2.zero;
        [AddTracking]
        private Vector2 m_LastCachedPosition = Vector2.zero;
        [AddTracking]
        private float m_PrevCachedRotation = 0f;
        [AddTracking]
        private float m_LastCachedRotation = 0f;

        [AddTracking]
        private float m_PrevCachedTime = 0f;
        [AddTracking]
        private float m_LastCachedTime = 0f;

        public void Initialize()
        {
            StateTracker.AddTracking(this);
            InitTransformCache();
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         **/
        public void AddForce(TSVector2 i_Force)
        {
            AddForce(i_Force, ForceMode.Force);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         *  @param mode Indicates how the force should be applied.
         **/
        public void AddForce(TSVector2 i_Force, ForceMode i_Mode)
        {
            if (!tsCollider.isBodyInitialized)
                return;

            if (i_Mode == ForceMode.Force)
            {
                tsCollider.body.TSApplyForce(i_Force);
            }
            else if (i_Mode == ForceMode.Impulse)
            {
                tsCollider.body.TSApplyImpulse(i_Force);
            }
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForce(TSVector2 i_Force, TSVector2 i_Position)
        {
            AddForce(i_Force, i_Position, ForceMode.Impulse);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForce(TSVector2 i_Force, TSVector2 i_Position, ForceMode i_Mode)
        {
            if (!tsCollider.isBodyInitialized)
                return;

            if (i_Mode == ForceMode.Force)
            {
                tsCollider.body.TSApplyForce(i_Force, i_Position);
            }
            else if (i_Mode == ForceMode.Impulse)
            {
                tsCollider.body.TSApplyImpulse(i_Force, i_Position);
            }
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForceAtPosition(TSVector2 i_Force, TSVector2 i_Position)
        {
            AddForceAtPosition(i_Force, i_Position, ForceMode.Impulse);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForceAtPosition(TSVector2 i_Force, TSVector2 i_Position, ForceMode i_Mode)
        {
            if (i_Mode == ForceMode.Force)
            {
                tsCollider.body.TSApplyForce(i_Force, i_Position);
            }
            else if (i_Mode == ForceMode.Impulse)
            {
                tsCollider.body.TSApplyImpulse(i_Force, i_Position);
            }
        }

        /**
         *  @brief Simulates the provided tourque in the body. 
         *  
         *  @param torque A {@link TSVector2} representing the torque to be applied.
         **/
        public void AddTorque(TSVector2 i_Torque)
        {
            if (!tsCollider.isBodyInitialized)
                return;

            tsCollider.body.TSApplyTorque(i_Torque);
        }

        /**
         *  @brief Moves the body to a new position. 
         **/
        public void MovePosition(TSVector2 i_Position)
        {
            this.position = i_Position;

            // This should reset interpolation data.

            Vector2 newPosition = position.ToVector();
            m_PrevCachedPosition = m_LastCachedPosition = newPosition;
        }

        /**
         *  @brief Rotates the body to a provided rotation. 
         **/
        public void MoveRotation(FP i_Rotation)
        {
            this.rotation = i_Rotation;

            // This should reset interpolation data.

            float newRotation = rotation.AsFloat();
            m_PrevCachedRotation = m_LastCachedRotation = newRotation;
        }

        /**
         *  @brief Interpolation logic.
         **/
        public Vector2 ComputeTransformPosition(float i_Time)
        {
            if (m_Interpolation == InterpolateMode.Interpolate)
            {
                float elapsedTime = i_Time - m_LastCachedTime;
                float interval = m_LastCachedTime - m_PrevCachedTime;
                float lerpFactor = (interval > 0f) ? Mathf.Clamp01(elapsedTime / interval) : 1f;

                Vector2 outputPosition = Vector2.Lerp(m_PrevCachedPosition, m_LastCachedPosition, lerpFactor);

                return outputPosition;
            }
            else // NOTE: Actually we don't support Extrapolation.
            {
                return position.ToVector();
            }
        }

        /**
         *  @brief Interpolation logic.
         **/
        public float ComputeTransformRotation(float i_Time)
        {
            if (m_Interpolation == InterpolateMode.Interpolate)
            {
                float elapsedTime = i_Time - m_LastCachedTime;
                float interval = m_LastCachedTime - m_PrevCachedTime;
                float lerpFactor = (interval > 0f) ? Mathf.Clamp01(elapsedTime / interval) : 1f;

                float outputRotation = Mathf.Lerp(m_PrevCachedRotation, m_LastCachedRotation, lerpFactor);

                return outputRotation;
            }
            else // NOTE: Actually we don't support Extrapolation.
            {
                return rotation.AsFloat();
            }
        }

        /**
         *  @brief Interpolation logic.
         **/
        public void InitTransformCache()
        {
            Vector2 newPosition = position.ToVector();
            float newRotation = rotation.AsFloat();

            m_PrevCachedPosition = m_LastCachedPosition = newPosition;
            m_PrevCachedRotation = m_LastCachedRotation = newRotation;

            m_LastCachedTime = 0f;
            m_PrevCachedTime = 0f;
        }

        /**
         *  @brief Interpolation logic.
         **/
        public void UpdateTransformCache(float i_Time)
        {
            // Cache time.

            m_PrevCachedTime = m_LastCachedTime;
            m_LastCachedTime = i_Time;

            // Update position and rotation.

            m_PrevCachedPosition = m_LastCachedPosition;
            m_PrevCachedRotation = m_LastCachedRotation;

            Vector2 newPosition = position.ToVector();
            float newRotation = rotation.AsFloat();

            m_LastCachedPosition = newPosition;
            m_LastCachedRotation = newRotation;

            // TODO: Scale?
        }
    }
}