namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Has Float Key Game Setting.")]
    public class HasFloatKeyGameSetting : FsmStateAction
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
            bool value = GameSettings.HasFloatKeyMain(id.Value);
            storeResult.Value = value;

            Finish();
        }
    }
}
