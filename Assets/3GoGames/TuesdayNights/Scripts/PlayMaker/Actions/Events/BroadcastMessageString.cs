using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Broadcast String message using Messenger.")]
    public class BroadcastMessageString : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Message.")]
        public FsmString eventType;

        [RequiredField]
        public FsmString stringParam;

        public override void Reset()
        {
            eventType = "";
            stringParam = null;
        }

        public override void OnEnter()
        {
            if (!stringParam.IsNone)
            {
                Messenger.Broadcast<string>(eventType.Value, stringParam.Value);
            }

            Finish();
        }
    }
}