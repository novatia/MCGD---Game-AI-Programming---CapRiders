using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Close UI page.")]
    public class UIPageDismiss : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(UIController))]
        [UIHint(UIHint.Variable)]
        [Tooltip("Page controller.")]
        public FsmOwnerDefault gameObject;

        public bool waitForAnimation = false;

        private GameObject m_CachedGameObject = null;
        private UIController m_Controller = null;

        public override void Reset()
        {
            gameObject = null;
        }

        public override void OnUpdate()
        {
            if (m_Controller == null || m_Controller.isViewClosed)
            {
                Finish();
            }
        }

        public override void OnEnter()
        {
            GameObject go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
            if (go != null)
            {
                if (UpdateCache(go))
                {
                    m_Controller.Dismiss();
                }
            }

            if (!waitForAnimation)
            {
                Finish();
            }
        }

        private bool UpdateCache(GameObject i_Go)
        {
            if (i_Go == null)
            {
                return false;
            }

            if (m_Controller == null || m_CachedGameObject != i_Go)
            {
                m_Controller = i_Go.GetComponent<UIController>();
                m_CachedGameObject = i_Go;
            }

            return (m_Controller != null);
        }
    }
}