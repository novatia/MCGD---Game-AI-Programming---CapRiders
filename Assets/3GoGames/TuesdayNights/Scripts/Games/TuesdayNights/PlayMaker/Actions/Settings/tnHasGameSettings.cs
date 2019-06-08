namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Check if a specific game settings Id is present.")]
    public class tnHasGameSettingsKey : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Setting Id.")]
        public FsmString settingId;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public override void Reset()
        {
            settingId = "";
            storeResult = null;
        }

        public override void OnEnter()
        {
            bool outValue = tnGameData.HasGameSettingsKeyMain(settingId.Value);
            storeResult.Value = outValue;

            Finish();
        }
    }
}