namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Load Game Settings.")]
    public class LoadGameSettings : FsmStateAction
    {
        public override void OnEnter()
        {
            GameSettings.LoadMain();

            Finish();
        }
    }
}
