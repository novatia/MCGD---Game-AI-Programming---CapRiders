using UnityEngine;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Deactivate Triggers for a specified UIView.")]
    public class UIViewDeactivateTriggers : FsmStateAction
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
                    DeactivateTriggerFor(uiView.gameObject);
                }
            }

            Finish();
        }

        private void DeactivateTriggerFor(GameObject i_Root)
        {
            if (i_Root == null)
                return;

            UIEventTrigger[] uiEventTriggers = i_Root.GetComponentsInChildren<UIEventTrigger>(true);

            for (int uiEventTriggerIndex = 0; uiEventTriggerIndex < uiEventTriggers.Length; ++uiEventTriggerIndex)
            {
                UIEventTrigger uiEventTriggerInstance = uiEventTriggers[uiEventTriggerIndex];
                uiEventTriggerInstance.enabled = false;
            }
        }
    }
}