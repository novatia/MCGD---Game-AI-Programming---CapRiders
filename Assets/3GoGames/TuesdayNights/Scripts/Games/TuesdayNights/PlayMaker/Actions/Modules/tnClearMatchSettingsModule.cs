namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Clear tnMatchSettingsModule instance.")]
    public class tnClearMatchSettingsModule : FsmStateAction
    {
        public override void OnEnter()
        {
            tnMatchSettingsModule matchSettingsModule = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

            if (matchSettingsModule != null)
            {
                matchSettingsModule.Clear();
            }

            Finish();
        }
    }
}