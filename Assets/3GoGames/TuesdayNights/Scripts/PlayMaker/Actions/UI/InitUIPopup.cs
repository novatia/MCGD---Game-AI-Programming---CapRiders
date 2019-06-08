using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Initialize UI Popup.")]
    public class InitUIPopup : FsmStateAction
    {
        public override void OnEnter()
        {
            Popup.InitializeMain();
            Finish();
        }
    }
}