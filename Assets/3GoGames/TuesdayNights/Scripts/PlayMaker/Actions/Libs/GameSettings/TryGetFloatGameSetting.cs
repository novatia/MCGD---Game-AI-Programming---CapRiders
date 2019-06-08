namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Try Get Float Game Setting.")]
    public class TryGetFloatGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmBool found;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmFloat outValue;

        public override void Reset()
        {
            id = "";
            found = null;
            outValue = null;
        }

        public override void OnEnter()
        {
            float value;
            bool foundSetting = GameSettings.TryGetFloatMain(id.Value, out value);

            found.Value = foundSetting;
            outValue.Value = value;

            Finish();
        }
    }
}
