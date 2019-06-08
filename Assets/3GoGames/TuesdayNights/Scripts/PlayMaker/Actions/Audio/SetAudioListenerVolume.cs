using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Set Audio Listener Volume.")]
    public class SetSfxVolume : FsmStateAction
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
            AudioListener.volume = volume.Value;
            Finish();
        }
    }
}