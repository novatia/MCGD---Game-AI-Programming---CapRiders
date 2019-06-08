using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Broadcast message using Messenger.")]
    public class BroadcastMessage : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Message.")]
        public FsmString eventType;

        public override void Reset()
        {
            eventType = "";
        }

        public override void OnEnter()
        {
            Messenger.Broadcast(eventType.Value);
            Finish();
        }
    }
}