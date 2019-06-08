namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Try Get Bool Game Setting.")]
    public class TryGetBoolGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmBool found;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmBool outValue;

        public override void Reset()
        {
            id = "";
            found = null;
            outValue = null;
        }

        public override void OnEnter()
        {
            bool value;
            bool foundSetting = GameSettings.TryGetBoolMain(id.Value, out value);

            found.Value = foundSetting;
            outValue.Value = value;

            Finish();
        }
    }
}
