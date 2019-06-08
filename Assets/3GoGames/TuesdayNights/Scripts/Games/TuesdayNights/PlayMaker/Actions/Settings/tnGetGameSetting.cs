namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Get settings value.")]
    public class tnGetGameSettings : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Setting Id.")]
        public FsmString settingId;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Setting Value")]
        public FsmString storeResult;

        public override void Reset()
        {
            settingId = "";
            storeResult = "";
        }

        public override void OnEnter()
        {
            string outValue = tnGameData.GetGameSettingsValueMain(settingId.Value);
            storeResult.Value = outValue;

            Finish();
        }
    }
}