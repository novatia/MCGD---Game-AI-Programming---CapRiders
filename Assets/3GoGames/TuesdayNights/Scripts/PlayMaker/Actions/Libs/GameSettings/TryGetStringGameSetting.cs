namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Try Get String Game Setting.")]
    public class TryGetStringGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmBool found;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString outValue;

        public override void Reset()
        {
            id = "";
            found = null;
            outValue = null;
        }

        public override void OnEnter()
        {
            string value;
            bool foundSetting = GameSettings.TryGetStringMain(id.Value, out value);

            found.Value = foundSetting;
            outValue.Value = value;

            Finish();
        }
    }
}
