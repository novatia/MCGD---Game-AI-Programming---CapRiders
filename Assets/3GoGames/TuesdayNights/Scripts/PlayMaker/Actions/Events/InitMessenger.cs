using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Initialize Messenger.")]
    public class InitMessenger : FsmStateAction
    {
        public override void OnEnter()
        {
            Messenger.Cleanup();
            Finish();
        }
    }
}