namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Set Bool Game Setting.")]
    public class SetBoolGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        public FsmBool value;

        public override void Reset()
        {
            id = "";
            value = null;
        }

        public override void OnEnter()
        {
            GameSettings.SetBoolMain(id.Value, value.Value);

            Finish();
        }
    }
}