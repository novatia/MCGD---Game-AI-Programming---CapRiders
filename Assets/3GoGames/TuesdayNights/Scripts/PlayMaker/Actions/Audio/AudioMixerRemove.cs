using System;
using UnityEngine;
using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Remove AudioMixer Snapshot.")]
    public class AudioMixerRemove : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(AudioMixerSnapshot))]
        public FsmObject defaultSnapshot;

        public override void Reset()
        {
            defaultSnapshot = null;
        }

        public override void OnEnter()
        {
            if (!defaultSnapshot.IsNone && defaultSnapshot.Value != null)
            {
                AudioMixerManager.RemoveMain((AudioMixerSnapshot)defaultSnapshot.Value);
            }

            Finish();
        }
    }
}