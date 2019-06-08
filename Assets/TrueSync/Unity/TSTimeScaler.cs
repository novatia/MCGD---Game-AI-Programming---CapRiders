using UnityEngine;

namespace TrueSync
{
    public class TSTimeScaler
    {
        // Fields

        [AddTracking]
        private FP m_TimeScale = FP.One;

        // ACCESSORS

        public FP timeScale
        {
            get
            {
                return m_TimeScale;
            }
        }

        // LOGIC

        public void Init()
        {
            StateTracker.AddTracking(this);
        }

        public void Sync()
        {
            InternalSetTimeScale(m_TimeScale);
        }

        public void SetTimeScale(FP i_Value)
        {
            InternalSetTimeScale(i_Value);
        }

        // INTERNALS

        private void InternalSetTimeScale(FP i_Value)
        {
            m_TimeScale = MathFP.Max(FP.Zero, i_Value);

            if (m_TimeScale == FP.One)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = m_TimeScale.AsFloat();
            }
        }
    }
}