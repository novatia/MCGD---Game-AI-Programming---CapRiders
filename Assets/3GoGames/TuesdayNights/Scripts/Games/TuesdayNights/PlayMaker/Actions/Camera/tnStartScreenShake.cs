using UnityEngine;

using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights - Camera")]
    [Tooltip("Start a Screen Shake.")]
    public class tnStartScreenShake : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        public FsmFloat shakeTime = 0.5f;
        [RequiredField]
        public FsmFloat shakeAmount = 0.1f;

        public ShakeMode shakeMode = ShakeMode.Interruput;

        private GameObject m_CachedGameObject = null;
        private tnScreenShake m_Target = null;

        public override void Reset()
        {
            gameObject = null;

            shakeTime = 0.5f;
            shakeAmount = 0.1f;
        }

        public override void OnEnter()
        {
            GameObject go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
            if (go != null)
            {
                if (UpdateCache(go))
                {
                    m_Target.ForceShake(shakeTime.Value, shakeAmount.Value, shakeMode, null);
                }
            }

            Finish();
        }

        // INTERNALS

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