using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Broadcast Float message using Messenger.")]
    public class BroadcastMessageFloat : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Message.")]
        public FsmString eventType;

        [RequiredField]
        public FsmFloat floatParam;

        public override void Reset()
        {
            eventType = "";
            floatParam = null;
        }

        public override void OnEnter()
        {
            if (!floatParam.IsNone)
            {
                Messenger.Broadcast<float>(eventType.Value, floatParam.Value);
            }

            Finish();
        }
    }
}