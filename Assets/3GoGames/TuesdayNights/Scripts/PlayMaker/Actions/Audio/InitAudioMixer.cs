using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Initialize AudioMixer.")]
    public class InitAudioMixer : FsmStateAction
    {
        [ObjectType(typeof(AudioMixerSnapshot))]
        public FsmObject defaultSnapshot;

        public override void Reset()
        {
            defaultSnapshot = null;
        }

        public override void OnEnter()
        {
            AudioMixerManager.InitializeMain();

            if (!defaultSnapshot.IsNone && defaultSnapshot.Value != null)
            {
                AudioMixerManager.SetSnapshotMain((AudioMixerSnapshot)defaultSnapshot.Value, 0f);
            }

            Finish();
        }
    }
}