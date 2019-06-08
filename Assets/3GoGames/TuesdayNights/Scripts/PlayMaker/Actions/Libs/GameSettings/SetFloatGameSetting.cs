namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Set Float Game Setting.")]
    public class SetFloatGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        public FsmFloat value;

        public override void Reset()
        {
            id = "";
            value = null;
        }

        public override void OnEnter()
        {
            GameSettings.SetFloatMain(id.Value, value.Value);

            Finish();
        }
    }
}
