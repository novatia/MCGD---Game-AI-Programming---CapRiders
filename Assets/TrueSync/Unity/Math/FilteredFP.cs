using UnityEngine;
using System.Collections;

namespace TrueSync
{
    public struct FilteredFP
    {
        // Fields

        private FP m_RaiseStepFactor;
        private FP m_LowerStepFactor;

        private FP m_Position;

        // ACCESSORS

        public FP position
        {
            get { return m_Position; }
            set { m_Position = value; }
        }

        // LOGIC

        public FP Step(FP i_Target, FP i_FrameTime)
        {
            FP smoothStepFactor = (i_Target < m_Position) ? m_LowerStepFactor : m_RaiseStepFactor;
            FP smoothFactor = (smoothStepFactor > FP.Zero) ? FP.One - MathFP.Pow(FP.Half, i_FrameTime / smoothStepFactor) : FP.One;

            m_Position += (i_Target - m_Position) * smoothFactor;

            return m_Position;
        }

        public void Reset()
        {
            Reset(FP.Zero);
        }

        public void Reset(FP i_Position)
        {
            m_Position = i_Position;
        }

        // CTOR

        public FilteredFP(FP i_RaiseStepFactor, FP i_LowerStepFactor)
        {
            m_Position = 0.0f;

            m_RaiseStepFactor = MathFP.Max(i_RaiseStepFactor, FP.Zero);
            m_LowerStepFactor = MathFP.Max(i_LowerStepFactor, FP.Zero);
        }
    }
}