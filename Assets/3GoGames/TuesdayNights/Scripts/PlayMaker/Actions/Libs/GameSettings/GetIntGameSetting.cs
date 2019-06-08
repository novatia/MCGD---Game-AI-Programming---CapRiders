namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Get Int Game Setting.")]
    public class GetIntGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt storeResult;

        public override void Reset()
        {
            id = "";
            storeResult = null;
        }

        public override void OnEnter()
        {
            int value = GameSettings.GetIntMain(id.Value);
            storeResult.Value = value;

            Finish();
        }
    }
}
