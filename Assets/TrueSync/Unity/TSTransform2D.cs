using UnityEngine;

namespace TrueSync
{
    /**
    *  @brief A deterministic version of Unity's Transform component for 2D physics. 
    **/
    [ExecuteInEditMode]
    public class TSTransform2D : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        [AddTracking]
        private TSVector2 m_Position = TSVector2.zero;

        /**
         *  @brief Property access to position. 
         *  
         *  It works as proxy to a Body's position when there is a collider attached.
         **/
        public TSVector2 position
        {
            get
            {
                if (m_TSCollider != null && m_TSCollider.body != null)
                {
                    return m_TSCollider.body.TSPosition - m_TSCollider.center;
                }

                return m_Position;
            }

            set
            {
                m_Position = value;

                if (m_TSCollider != null && m_TSCollider.body != null)
                {
                    m_TSCollider.body.TSPosition = m_Position + m_TSCollider.center;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [AddTracking]
        private FP m_Rotation = FP.Zero;

        /**
         *  @brief Property access to rotation. 
         *  
         *  It works as proxy to a Body's rotation when there is a collider attached.
         **/
        public FP rotation
        {
            get
            {
                if (m_TSCollider != null && m_TSCollider.body != null)
                {
                    return m_TSCollider.body.TSOrientation * FP.Rad2Deg;
                }

                return m_Rotation;
            }

            set
            {
                m_Rotation = value;

                if (m_TSCollider != null && m_TSCollider.body != null)
                {
                    m_TSCollider.body.TSOrientation = m_Rotation * FP.Deg2Rad;
                }
            }
        }

        [SerializeField]
        [HideInInspector]
        [AddTracking]
        private TSVector m_Scale = TSVector.one;

        /**
         *  @brief Property access to scale. 
         **/
        public TSVector scale
        {
            get
            {
                return m_Scale;
            }

            set
            {
                m_Scale = value;
            }
        }

        [SerializeField]
        [HideInInspector]
        private bool m_Serialized;

        public bool serialized
        {
            get { return m_Serialized; }
        }

        [HideInInspector]
        public TSCollider2D m_TSCollider = null;

        [HideInInspector]
        public TSTransform2D m_TSParent = null;

        private bool m_Initialized = false;

        private TSRigidBody2D m_Rigidbody;

        /**
         *  @brief Initializes internal properties based on whether there is a {@link TSCollider2D} attached.
         **/
        public void Initialize()
        {
            if (m_Initialized)
            {
                return;
            }

            m_TSCollider = GetComponent<TSCollider2D>();

            if (transform.parent != null)
            {
                m_TSParent = transform.parent.GetComponent<TSTransform2D>();
            }

            if (!m_Serialized)
            {
                UpdateEditMode();
            }

            if (m_TSCollider != null)
            {
                if (m_TSCollider.isBodyInitialized)
                {
                    m_TSCollider.body.TSPosition = m_Position + m_TSCollider.center;
                    m_TSCollider.body.TSOrientation = m_Rotation * FP.Deg2Rad;
                }
            }
            else
            {
                StateTracker.AddTracking(this);
            }

            m_Initialized = true;
        }

        // MonoBehaviour's interface

        public void Start()
        {
            if (!Application.isPlaying)
                return;

            m_Rigidbody = GetComponent<TSRigidBody2D>();
        }

        public void Update()
        {
            if (Application.isPlaying)
            {
                if (m_Initialized)
                {
                    UpdatePlayMode();
                }
            }
            else
            {
                UpdateEditMode();
            }
        }

        // INTERNALS

        private void UpdatePlayMode()
        {
            if (m_Rigidbody != null)
            {
                float elapsedTime = Time.time;

                Vector2 newPosition = m_Rigidbody.ComputeTransformPosition(elapsedTime);
                float newRotation = m_Rigidbody.ComputeTransformRotation(elapsedTime);

                transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
                transform.rotation = Quaternion.Euler(0f, 0f, newRotation);

                if (scale == TSVector.one)
                {
                    transform.localScale = Vector3.one;
                }
                else
                {
                    transform.localScale = scale.ToVector();
                }
            }
            else
            {
                Vector2 newPosition = position.ToVector();
                float newRotation = rotation.AsFloat();

                transform.localPosition = new Vector3(newPosition.x, newPosition.y, transform.localPosition.z);
                transform.localRotation = Quaternion.Euler(0f, 0f, newRotation);

                if (scale == TSVector.one)
                {
                    transform.localScale = Vector3.one;
                }
                else
                {
                    transform.localScale = scale.ToVector();
                }

            }
        }

        private void UpdateEditMode()
        {
            if (transform.hasChanged)
            {
                m_Position = transform.position.ToTSVector2();
                m_Rotation = transform.rotation.eulerAngles.z;

                m_Scale = transform.localScale.ToTSVector();

                m_Serialized = true;
            }
        }
    }
}