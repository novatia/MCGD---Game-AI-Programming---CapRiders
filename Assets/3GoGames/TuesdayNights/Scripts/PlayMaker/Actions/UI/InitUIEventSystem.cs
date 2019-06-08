using System;

using UnityEngine;
using UnityEngine.UI;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Initialize UIEventSystem.")]
    public class InitUIEventSystem : FsmStateAction
    {
        public override void OnEnter()
        {
            UIEventSystem.InitializeMain();
            Finish();
        }
    }
}