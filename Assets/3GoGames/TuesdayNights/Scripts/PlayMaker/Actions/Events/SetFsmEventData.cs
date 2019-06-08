using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Events")]
    [Tooltip("Set FsmEventData given its key.")]
    public class SetFsmEventData : FsmStateAction
    {
        [CompoundArray("Event Properties", "Key", "Data")]
        public FsmString[] keys;
        public FsmVar[] datas;

        public override void Reset()
        {
            keys = new FsmString[1];
            datas = new FsmVar[1];
        }

        public override void OnEnter()
        {
            PlayMakerEventParams eventParams = new PlayMakerEventParams();

            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].IsNone)
                    continue;

                int keyHash = StringUtils.GetHashCode(keys[i].Value);
                object value = PlayMakerUtils.GetValueFromFsmVar(this.Fsm, datas[i]);

                eventParams.AddValue(keyHash, value);
            }

            PlayMakerEventData.SetValues(eventParams);

            Finish();
        }
    }
}