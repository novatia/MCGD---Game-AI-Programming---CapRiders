using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Broadcast Bool message using Messenger.")]
    public class BroadcastMessageBool : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Message.")]
        public FsmString eventType;

        [RequiredField]
        public FsmBool boolParam;

        public override void Reset()
        {
            eventType = "";
            boolParam = null;
        }

        public override void OnEnter()
        {
            if (!boolParam.IsNone)
            {
                Messenger.Broadcast<bool>(eventType.Value, boolParam.Value);
            }

            Finish();
        }
    }
}