namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Save Game Settings.")]
    public class SaveGameSettings : FsmStateAction
    {
        public override void OnEnter()
        {
            GameSettings.SaveMain();

            Finish();
        }
    }
}
