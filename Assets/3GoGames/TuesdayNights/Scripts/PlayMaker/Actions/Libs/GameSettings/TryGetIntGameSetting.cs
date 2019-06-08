namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Try Get Int Game Setting.")]
    public class TryGetIntGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmBool found;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt outValue;

        public override void Reset()
        {
            id = "";
            found = null;
            outValue = null;
        }

        public override void OnEnter()
        {
            int value;
            bool foundSetting = GameSettings.TryGetIntMain(id.Value, out value);

            found.Value = foundSetting;
            outValue.Value = value;

            Finish();
        }
    }
}
