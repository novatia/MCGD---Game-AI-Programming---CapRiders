using UnityEngine;

using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights - Camera")]
    [Tooltip("Set target camera position.")]
    public class tnSetCameraPosition : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(tnGameCamera))]
        [UIHint(UIHint.Variable)]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        public FsmVector3 position;

        private GameObject m_CachedGameObject = null;
        private tnGameCamera m_Target = null;

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
                    m_Target.SetPosition(position.Value);
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
                m_Target = i_Go.GetComponent<tnGameCamera>();
                m_CachedGameObject = i_Go;
            }

            return (m_Target != null);
        }
    }
}