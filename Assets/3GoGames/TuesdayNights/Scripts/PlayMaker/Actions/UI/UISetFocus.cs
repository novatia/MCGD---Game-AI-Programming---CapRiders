using UnityEngine;
using UnityEngine.UI;

using System.Collections;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Set EventSystem focus.")]
    public class UISetFocus : FsmStateAction
    {
        public GameObject target = null;

        public override void OnEnter()
        {
            UIEventSystem.SetFocusMain(FindFirstEnabledSelectable(target));
            Finish();
        }

        private static GameObject FindFirstEnabledSelectable(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return null; // Invalid game object.
            }

            GameObject go = null;

            var selectables = gameObject.GetComponentsInChildren<Selectable>(true);
            foreach (var selectable in selectables)
            {
                if (selectable.IsActive() && selectable.IsInteractable())
                {
                    go = selectable.gameObject;
                    break;
                }
            }

            return go;
        }
    }
}