using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Start a tnRunnableFSM.")]
    public class tnStartRunnableFSM : FsmStateAction
    {
        [ObjectType(typeof(tnRunnableFSM))]
        public FsmObject m_RunnableFSM = null;

        public override void Reset()
        {
            m_RunnableFSM = null;
        }

        public override void OnEnter()
        {
            if (m_RunnableFSM != null)
            {
                if (!m_RunnableFSM.IsNone)
                {
                    if (m_RunnableFSM.Value != null)
                    {
                        tnRunnableFSM runnableFSM = (tnRunnableFSM)m_RunnableFSM.Value;
                        if (runnableFSM)
                        {
                            runnableFSM.StartFSM();
                        }
                    }
                }
            }
        }
    }
}
