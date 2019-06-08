using System;
using UnityEngine;
using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Push AudioMixer Snapshot.")]
    public class AudioMixerPush : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(AudioMixerSnapshot))]
        public FsmObject snapshot;
        [RequiredField]
        public FsmFloat fadeTime;
        [RequiredField]
        public FsmFloat priority;

        public bool popOnExit = false;

        public override void Reset()
        {
            snapshot = null;
            fadeTime = null;
            priority = null;

            popOnExit = false;
        }

        public override void OnEnter()
        {
            if (!snapshot.IsNone && snapshot.Value != null)
            {
                AudioMixerManager.SetSnapshotMain((AudioMixerSnapshot)snapshot.Value, fadeTime.Value, priority.Value);
            }

            if (!popOnExit)
            {
                Finish();
            }
        }

        public override void OnExit()
        {
            if (popOnExit)
            {
                if (!snapshot.IsNone && snapshot.Value != null)
                {
                    AudioMixerManager.RemoveMain((AudioMixerSnapshot)snapshot.Value);
                }
            }
        }
    }
}