using UnityEngine;
using UnityEngine.Serialization;

namespace TrueSync
{
    /**
     *  @brief Collider with a box 2D shape. 
     **/
    [AddComponentMenu("TrueSync/Physics/BoxCollider2D", 0)]
    public class TSBoxCollider2D : TSCollider2D
    {
        [FormerlySerializedAs("size")]
        [SerializeField]
        private TSVector2 m_Size = TSVector2.one;

        /**
         *  @brief Size of the box. 
         **/
        public TSVector2 size
        {
            get
            {
                if (body != null)
                {
                    TSVector2 halfVector = ((Physics2D.PolygonShape)body.FixtureList[0].Shape).Vertices[0] * 2;
                    return halfVector;
                }

                return m_Size;
            }
            set
            {
                m_Size = value;

                if (body != null)
                {
                    TSVector size3 = new TSVector(m_Size.x, m_Size.y, 1);
                    ((Physics2D.PolygonShape)body.FixtureList[0].Shape).Vertices = Physics2D.PolygonTools.CreateRectangle(size3.x * FP.Half, size3.y * FP.Half);
                }

            }
        }

        /**
         *  @brief Create the internal shape used to represent a TSBoxCollider.
         **/
        public override Physics2D.Shape CreateShape()
        {
            TSVector size3 = new TSVector(size.x, size.y, 1);
            return new Physics2D.PolygonShape(Physics2D.PolygonTools.CreateRectangle(size3.x * FP.Half, size3.y * FP.Half), 1);
        }

        // TSCollider2D's INTERFACE

        protected override void DrawGizmos()
        {
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        protected override Vector3 GetGizmosSize()
        {
            TSVector size3 = new TSVector(size.x, size.y, 1);
            return size3.ToVector();
        }
    }
}