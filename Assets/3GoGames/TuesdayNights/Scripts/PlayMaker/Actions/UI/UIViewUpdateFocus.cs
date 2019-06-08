using UnityEngine;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Update focus finding a valid Selectable from a specified UIView's children.")]
    public class UIViewUpdateFocus : FsmStateAction
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
                    UpdateFocusTo(uiView.gameObject);
                }
            }

            Finish();
        }

        private void UpdateFocusTo(GameObject i_Root)
        {
            if (i_Root == null)
                return;

            GameObject newFocus = i_Root.FindChildWithTag("Focus");
            if (newFocus != null)
            {
                UIEventSystem.SetFocusMain(newFocus);
            }
        }
    }
}