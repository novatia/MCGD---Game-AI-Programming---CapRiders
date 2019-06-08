using UnityEngine.Audio;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Initialize MusicPlayer.")]
    public class InitMusicPlayer : FsmStateAction
    {
        [ObjectType(typeof(AudioMixerGroup))]
        public FsmObject outputChannel;

        public override void Reset()
        {
            outputChannel = null;
        }

        public override void OnEnter()
        {
            MusicPlayer.InitializeMain();

            if (!outputChannel.IsNone && outputChannel.Value != null)
            {
                MusicPlayer.SetChannelMain((AudioMixerGroup)outputChannel.Value);
            }

            Finish();
        }
    }
}