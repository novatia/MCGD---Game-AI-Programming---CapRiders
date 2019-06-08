using UnityEngine;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Remove EventSystem focus if it's under a specific UIVIew.")]
    public class UIViewRemoveFocus : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(UIView))]
        [Tooltip("The GameObject with an UIView component.")]
        public FsmOwnerDefault gameObject;

        public override void Reset()
        {
            gameObject = null;
        }

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go != null)
            {
                UIView uiView = go.GetComponent<UIView>();
                if (uiView != null)
                {
                    RemoveFocusFrom(uiView.gameObject);
                }
            }

            Finish();
        }

        private void RemoveFocusFrom(GameObject i_Root)
        {
            if (i_Root == null)
                return;

            GameObject currentFocus = UIEventSystem.focusMain;
            if (currentFocus != null)
            {
                if (i_Root.transform.HasChild(currentFocus))
                {
                    UIEventSystem.SetFocusMain(null);
                }
            }
        }
    }
}