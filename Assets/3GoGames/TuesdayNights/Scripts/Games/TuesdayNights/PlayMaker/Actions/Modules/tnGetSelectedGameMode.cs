namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Get selected game mode id from MatchSettingsModule")]
    public class tnGetSelectedGameMode : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the setting's value")]
        public FsmInt storeResult;

        public override void Reset()
        {
            storeResult = new FsmInt { UseVariable = true };
        }

        public override void OnEnter()
        {
            tnMatchSettingsModule module = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

            if (module != null)
            {
                int gameModeId = module.gameModeId;
                
                if (storeResult != null && !storeResult.IsNone)
                {
                    storeResult.Value = gameModeId;
                }
            }

            Finish();
        }
    }
}