using UnityEngine;

using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights - Camera")]
    [Tooltip("Stop any Screen Shake.")]
    public class tnStopScreenShake : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmOwnerDefault gameObject;

        private GameObject m_CachedGameObject = null;
        private tnScreenShake m_Target = null;

        public override void Reset()
        {
            gameObject = null;
        }

        public override void OnEnter()
        {
            GameObject go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
            if (go != null)
            {
                if (UpdateCache(go))
                {
                    m_Target.ForceStop();
                }
            }

            Finish();
        }

        private bool UpdateCache(GameObject i_Go)
        {
            if (i_Go == null)
            {
                return false;
            }

            if (m_Target == null || m_CachedGameObject != i_Go)
            {
                m_Target = i_Go.GetComponentInChildren<tnScreenShake>();
                m_CachedGameObject = i_Go;
            }

            return (m_Target != null);
        }
    }
}