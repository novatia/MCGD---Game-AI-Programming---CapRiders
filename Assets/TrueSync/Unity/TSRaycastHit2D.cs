namespace TrueSync
{
    /**
     *  @brief Information about a 2D cast hit.
     **/
    public class TSRaycastHit2D
    {
        public TSCollider2D m_Collider = null;

        private TSVector2 m_Point = TSVector2.zero;
        private TSVector2 m_Normal = TSVector2.zero;

        private FP m_Distance = FP.Zero;
        private FP m_Fraction = FP.Zero;

        public TSCollider2D collider
        {
            get
            {
                return m_Collider;
            }
        }

        public TSVector2 point
        {
            get
            {
                return m_Point;
            }
        }

        public TSVector2 normal
        {
            get
            {
                return m_Normal;
            }
        }

        public FP distance
        {
            get
            {
                return m_Distance;
            }
        }

        public FP fraction
        {
            get
            {
                return m_Fraction;
            }
        }

        public TSRaycastHit2D(TSCollider2D i_Collider, TSVector2 i_Point, TSVector2 i_Normal, FP i_Distance, FP i_Fraction)
        {
            m_Collider = i_Collider;

            m_Point = i_Point;
            m_Normal = i_Normal;

            m_Distance = i_Distance;
            m_Fraction = i_Fraction;
        }
    }
}