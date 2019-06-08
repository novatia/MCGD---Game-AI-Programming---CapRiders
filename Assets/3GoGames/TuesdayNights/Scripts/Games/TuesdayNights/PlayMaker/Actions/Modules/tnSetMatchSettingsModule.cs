namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Set MatchSettings module.")]
    public class tnSetMatchSettingsModule : FsmStateAction
    {
        [RequiredField]
        public FsmString gameModeId;
        [RequiredField]
        public FsmString stadiumId;
        [RequiredField]
        public FsmString ballId;
        [RequiredField]
        public FsmString matchDurationOption;
        [RequiredField]
        public FsmString refereeOption;
        [RequiredField]
        public FsmString goldenGoalOption;

        public override void OnEnter()
        {
            tnMatchSettingsModule module = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

            if (module == null)
            {
                module = GameModulesManager.AddModuleMain<tnMatchSettingsModule>();
            }

            module.Clear();

            module.SetGameModeId(gameModeId.Value);
            module.SetStadiumId(stadiumId.Value);
            module.SetBallId(ballId.Value);
            module.SetMatchDurationOption(matchDurationOption.Value);
            module.SetRefereeOption(refereeOption.Value);
            module.SetGoldenGoalOption(goldenGoalOption.Value);

            Finish();
        }
    }
}