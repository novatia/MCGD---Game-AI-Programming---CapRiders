using System;
using UnityEngine;
using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Set Sfx Volume.")]
    public class PlaySfx : FsmStateAction
    {
        [ObjectType(typeof(AudioClip))]
        [Tooltip("Audio clip.")]
        public FsmObject audioClip;

        public AudioMixerGroup audioMixerGroup = null;
        [Range(0f, 1f)]
        public float volume = 1f;

        public override void Reset()
        {
            audioClip = null;
        }

        public override void OnEnter()
        {
            if (!audioClip.IsNone && audioClip.Value != null)
            {
                SfxPlayer.PlayMain((AudioClip)audioClip.Value, audioMixerGroup, volume);
            }

            Finish();
        }
    }
}