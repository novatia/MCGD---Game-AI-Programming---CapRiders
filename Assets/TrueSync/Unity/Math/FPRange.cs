using UnityEngine;

using System;

namespace TrueSync
{
    [Serializable]
    public class FPRange
    {
        [SerializeField]
        private FP m_Min = 0;
        [SerializeField]
        private FP m_Max = 1;

        public FP min
        {
            get { return m_Min; }
            set { m_Min = value; }
        }

        public FP max
        {
            get { return m_Max; }
            set { m_Max = value; }
        }

        // LOGIC

        public bool IsValueValid(FP i_Value)
        {
            return (m_Min <= i_Value && i_Value <= m_Max);
        }

        public FP GetValueAt(FP i_Percentage)
        {
            FP p = MathFP.Clamp01(i_Percentage);

            FP value = m_Min + p * (m_Max - m_Min);
            return value;
        }

        // CTOR

        public FPRange(FP i_Min, FP i_Max)
        {
            m_Min = i_Min;
            m_Max = i_Max;
        }
    }
}