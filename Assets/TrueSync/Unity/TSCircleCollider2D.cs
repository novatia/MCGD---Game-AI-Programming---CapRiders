using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync
{
    /**
     *  @brief Collider with a circle shape. 
     **/
    [AddComponentMenu("TrueSync/Physics/CircleCollider2D", 0)]
    public class TSCircleCollider2D : TSCollider2D
    {
        [FormerlySerializedAs("radius")]
        [SerializeField]
        private FP m_Radius = 0.5f;

        /**
         *  @brief Radius of the sphere. 
         **/
        public FP radius
        {
            get
            {
                if (body != null)
                {
                    return ((Physics2D.CircleShape)body.FixtureList[0].Shape).Radius;
                }

                return m_Radius;
            }

            set
            {
                m_Radius = value;

                if (body != null)
                {
                    Physics2D.CircleShape cShape = (Physics2D.CircleShape)body.FixtureList[0].Shape;
                    if (cShape.Radius != m_Radius)
                    {
                        cShape.Radius = m_Radius;
                    }
                }
            }
        }

        /**
         *  @brief Create the internal shape used to represent a TSSphereCollider.
         **/
        public override Physics2D.Shape CreateShape()
        {
            return new Physics2D.CircleShape(radius, 1);
        }

        // TSCollider2D's INTERFACE

        protected override void DrawGizmos()
        {
            Gizmos.DrawWireSphere(Vector3.zero, 1);
        }

        protected override Vector3 GetGizmosSize()
        {
            return Vector3.one * radius.AsFloat();
        }
    }
}