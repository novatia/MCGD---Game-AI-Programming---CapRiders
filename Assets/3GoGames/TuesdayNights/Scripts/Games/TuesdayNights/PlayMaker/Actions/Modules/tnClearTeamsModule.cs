namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Clear tnTeamsModule instance.")]
    public class tnClearTeamsModule : FsmStateAction
    {
        public override void OnEnter()
        {
            tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();

            if (teamsModule != null)
            {
                teamsModule.Clear();
            }

            Finish();
        }
    }
}