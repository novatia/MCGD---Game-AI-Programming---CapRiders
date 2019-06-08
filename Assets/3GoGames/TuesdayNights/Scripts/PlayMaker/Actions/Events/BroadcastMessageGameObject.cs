using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Broadcast GameObject message using Messenger.")]
    public class BroadcastMessageGameObject : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Message.")]
        public FsmString eventType;

        [RequiredField]
        public FsmGameObject gameObjectParam;

        public override void Reset()
        {
            eventType = "";
            gameObjectParam = null;
        }

        public override void OnEnter()
        {
            if (!gameObjectParam.IsNone)
            {
                Messenger.Broadcast<GameObject>(eventType.Value, gameObjectParam.Value);
            }

            Finish();
        }
    }
}