using UnityEngine;

using System.Collections.Generic;

namespace TrueSync
{
    /**
     *  @brief Collider with a polygon 2D shape. 
     **/
    [AddComponentMenu("TrueSync/Physics/PolygonCollider2D", 0)]
    public class TSPolygonCollider2D : TSCollider2D
    {
        [SerializeField]
        private TSVector2[] m_Points;

        public TSVector2[] points
        {
            get
            {
                return m_Points;
            }

            set
            {
                if (body == null)
                {
                    m_Points = value;
                }
            }
        }

        /**
         *  @brief Create the internal shape used to represent a TSBoxCollider.
         **/
        public override Physics2D.Shape[] CreateShapes()
        {
            if (m_Points == null || m_Points.Length == 0)
            {
                return null;
            }

            Physics2D.Vertices v = new Physics2D.Vertices();
            for (int index = 0, length = m_Points.Length; index < length; index++)
            {
                v.Add(m_Points[index]);
            }

            List<Physics2D.Vertices> convexShapeVs = Physics2D.BayazitDecomposer.ConvexPartition(v);
            Physics2D.Shape[] result = new Physics2D.Shape[convexShapeVs.Count];
            for (int index = 0, length = result.Length; index < length; index++)
            {
                result[index] = new Physics2D.PolygonShape(convexShapeVs[index], 1);
            }

            return result;
        }

        // TSCollider2D's INTERFACE

        protected override void DrawGizmos()
        {
            DrawPolygon(m_Points);
        }

        private void DrawPolygon(TSVector2[] i_AllPoints)
        {
            if (i_AllPoints == null || i_AllPoints.Length == 0)
            {
                return;
            }

            for (int index = 0, length = i_AllPoints.Length - 1; index < length; index++)
            {
                Gizmos.DrawLine(i_AllPoints[index].ToVector(), i_AllPoints[index + 1].ToVector());
            }

            Gizmos.DrawLine(i_AllPoints[i_AllPoints.Length - 1].ToVector(), i_AllPoints[0].ToVector());
        }


        protected override Vector3 GetGizmosSize()
        {
            return Vector3.one;
        }
    }
}