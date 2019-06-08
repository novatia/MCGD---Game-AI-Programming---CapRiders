using UnityEngine;

using System.Collections;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Remove EventSystem focus.")]
    public class UIRemoveFocus : FsmStateAction
    {
        public override void OnEnter()
        {
            UIEventSystem.SetFocusMain(null);
            Finish();
        }
    }
}