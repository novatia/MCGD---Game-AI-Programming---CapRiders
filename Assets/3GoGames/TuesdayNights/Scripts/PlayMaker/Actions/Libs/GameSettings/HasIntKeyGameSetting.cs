namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Has Int Key Game Setting.")]
    public class HasIntKeyGameSetting : FsmStateAction
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
            bool value = GameSettings.HasIntKeyMain(id.Value);
            storeResult.Value = value;

            Finish();
        }
    }
}
