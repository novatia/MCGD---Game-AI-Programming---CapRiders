namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Get Bool Game Setting.")]
    public class GetBoolGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public override void Reset()
        {
            id = "";
            storeResult = null;
        }

        public override void OnEnter()
        {
            bool value = GameSettings.GetBoolMain(id.Value);
            storeResult.Value = value;

            Finish();
        }
    }
}
