namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Services")]
    [Tooltip("Initialize Game Services.")]
    public class InitializeGameServices : FsmStateAction
    {
        public override void OnEnter()
        {
            GameServices.InitializeMain();

            Finish();
        }
    }
}
