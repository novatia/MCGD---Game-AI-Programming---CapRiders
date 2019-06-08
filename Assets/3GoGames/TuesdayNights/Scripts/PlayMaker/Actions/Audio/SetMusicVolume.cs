using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Set Music Volume.")]
    public class SetMusicVolume : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Target volume.")]
        public FsmFloat volume;

        public override void Reset()
        {
            volume = null;
        }

        public override void OnEnter()
        {
            MusicPlayer.SetVolumeMain(volume.Value);
            Finish();
        }
    }
}