using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Show/Hide MouseCursor.")]
    public class SetMouseCursorVisible : FsmStateAction
    {
        [RequiredField]
        public FsmBool visible = false;

        public override void Reset()
        {
            visible = false;
        }

        public override void OnEnter()
        {
#if !UNITY_EDITOR
            Cursor.visible = (!visible.IsNone && visible.Value);
#endif // !UNITY_EDITOR

            Finish();
        }
    }
}