namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Set Int Game Setting.")]
    public class SetIntGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        public FsmInt value;

        public override void Reset()
        {
            id = "";
            value = null;
        }

        public override void OnEnter()
        {
            GameSettings.SetIntMain(id.Value, value.Value);

            Finish();
        }
    }
}