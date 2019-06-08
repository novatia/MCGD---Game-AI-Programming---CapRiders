using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Camera")]
    [Tooltip("Initialize Fader.")]
    public class InitFader : FsmStateAction
    {
        public override void OnEnter()
        {
            FadeInOut.InitializeMain();
            Finish();
        }
    }
}