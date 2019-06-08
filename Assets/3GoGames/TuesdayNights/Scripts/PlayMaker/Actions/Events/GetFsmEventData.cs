using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Get FsmEventData given its key.")]
    public class GetFsmEventData : FsmStateAction
    {
        [CompoundArray("Event Properties", "Key", "Data")]
        public FsmString[] keys;
        [UIHint(UIHint.Variable)]
        public FsmVar[] datas;

        public override void Reset()
        {
            keys = new FsmString[1];
            datas = new FsmVar[1];
        }

        public override void OnEnter()
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].IsNone)
                    continue;

                int keyHash = StringUtils.GetHashCode(keys[i].Value);

                if (PlayMakerEventData.ContainsKey(keyHash))
                {
                    PlayMakerUtils.ApplyValueToFsmVar(this.Fsm, datas[i], PlayMakerEventData.GetValue(keyHash));
                }
            }

            Finish();
        }
    }
}