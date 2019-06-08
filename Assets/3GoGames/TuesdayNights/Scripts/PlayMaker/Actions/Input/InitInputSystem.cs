using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Initialize InputSystem.")]
    public class InitInputSystem : FsmStateAction
    {
        public override void OnEnter()
        {
            InputSystem.InitializeMain();
            Finish();
        }
    }
}