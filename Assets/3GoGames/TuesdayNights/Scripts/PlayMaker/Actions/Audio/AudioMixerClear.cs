namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Clear AudioMixer Queue.")]
    public class AudioMixerClear : FsmStateAction
    {
        public override void OnEnter()
        {
            AudioMixerManager.ClearMain();
            Finish();
        }
    }
}