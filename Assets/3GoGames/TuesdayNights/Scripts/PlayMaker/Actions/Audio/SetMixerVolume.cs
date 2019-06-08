using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Set Mixer Volume.")]
    public class SetMixerVolume : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(AudioMixer))]
        public FsmObject targetMixer;
        [RequiredField]
        public FsmString paramId;
        [RequiredField]
        public FsmFloat volume;

        public override void Reset()
        {
            paramId = "";
            targetMixer = null;
            volume = 1f;
        }

        public override void OnEnter()
        {
            AudioMixer mixer = (AudioMixer) targetMixer.Value;

            float volumeDb = AudioUtils.LinearToDecibel(volume.Value);
            mixer.SetFloat(paramId.Value, volumeDb);

            Finish();
        }
    }
}