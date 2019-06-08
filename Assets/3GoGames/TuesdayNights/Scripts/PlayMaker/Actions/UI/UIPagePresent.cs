using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Open UI page.")]
    public class UIPagePresent : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(UIController))]
        [UIHint(UIHint.Variable)]
        [Tooltip("Page controller.")]
        public FsmOwnerDefault gameObject;

        public bool waitForAnimation = false;
        public bool dismissOnExit = false;

        private GameObject m_CachedGameObject = null;
        private UIController m_Controller = null;

        public override void Reset()
        {
            gameObject = null;
            dismissOnExit = false;
        }

        public override void OnUpdate()
        {
            if (m_Controller == null || m_Controller.isViewOpened)
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
                    m_Controller.Present();
                }
            }

            if (!waitForAnimation && !dismissOnExit)
            {
                Finish();
            }
        }

        public override void OnExit()
        {
            if (dismissOnExit)
            {
                GameObject go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
                if (go == null)
                    return;

                if (UpdateCache(go))
                {
                    m_Controller.Dismiss();
                }
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