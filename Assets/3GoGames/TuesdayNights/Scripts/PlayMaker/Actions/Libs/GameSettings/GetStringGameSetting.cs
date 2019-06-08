namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Get String Game Setting.")]
    public class GetStringGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmString storeResult;

        public override void Reset()
        {
            id = "";
            storeResult = null;
        }

        public override void OnEnter()
        {
            string value = GameSettings.GetStringMain(id.Value);
            storeResult.Value = value;

            Finish();
        }
    }
}
