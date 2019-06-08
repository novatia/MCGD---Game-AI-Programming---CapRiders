namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Initialize Game Settings.")]
    public class InitializeGameSettings : FsmStateAction
    {
        public override void OnEnter()
        {
            GameSettings.InitializeMain();

            Finish();
        }
    }
}
