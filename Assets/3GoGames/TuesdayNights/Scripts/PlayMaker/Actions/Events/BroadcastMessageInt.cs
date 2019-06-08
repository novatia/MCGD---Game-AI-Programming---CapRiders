using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Broadcast Int message using Messenger.")]
    public class BroadcastMessageInt : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Message.")]
        public FsmString eventType;

        [RequiredField]
        public FsmInt intParam;

        public override void Reset()
        {
            eventType = "";
            intParam = null;
        }

        public override void OnEnter()
        {
            if (!intParam.IsNone)
            {
                Messenger.Broadcast<int>(eventType.Value, intParam.Value);
            }

            Finish();
        }
    }
}