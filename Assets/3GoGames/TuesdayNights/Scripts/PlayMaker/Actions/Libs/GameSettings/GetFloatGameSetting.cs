namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Get Float Game Setting.")]
    public class GetFloatGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeResult;

        public override void Reset()
        {
            id = "";
            storeResult = null;
        }

        public override void OnEnter()
        {
            float value = GameSettings.GetFloatMain(id.Value);
            storeResult.Value = value;

            Finish();
        }
    }
}
