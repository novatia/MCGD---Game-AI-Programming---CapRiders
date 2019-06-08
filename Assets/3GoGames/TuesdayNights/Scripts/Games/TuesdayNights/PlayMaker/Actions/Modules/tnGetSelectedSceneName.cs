namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Get scene name from MatchSettingsModule")]
    public class tnGetSelectedSceneName : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString storeResult;

        public override void Reset()
        {
            storeResult = new FsmString { UseVariable = true };
        }

        public override void OnEnter()
        {
            tnMatchSettingsModule module = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
            if (module != null)
            {
                int stadiumId = module.stadiumId;
                tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);

                if (stadiumData != null)
                {
                    if (storeResult != null && !storeResult.IsNone)
                    {
                        storeResult.Value = stadiumData.sceneName;
                    }
                }
            }

            Finish();
        }
    }
}