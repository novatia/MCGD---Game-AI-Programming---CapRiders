using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Initialize MouseRaycaster.")]
    public class InitMouseRaycaster : FsmStateAction
    {
        public override void OnEnter()
        {
            MouseRaycaster.InitializeMain();
            Finish();
        }
    }
}