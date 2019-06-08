namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Initialize game Database.")]
    public class tnInitializeDatabase : FsmStateAction
    {
        public override void OnEnter()
        {
            tnGameData.InitializeMain();

            Finish();
        }
    }
}