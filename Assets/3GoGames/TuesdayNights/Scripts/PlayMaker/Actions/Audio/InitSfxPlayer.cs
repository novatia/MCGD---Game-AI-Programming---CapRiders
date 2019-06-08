using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Initialize SfxPlayer.")]
    public class InitSfxPlayer : FsmStateAction
    {
        public override void OnEnter()
        {
            SfxPlayer.InitializeMain();
            Finish();
        }
    }
}