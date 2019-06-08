namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Settings")]
    [Tooltip("Set String Game Setting.")]
    public class SetStringGameSetting : FsmStateAction
    {
        [RequiredField]
        public FsmString id;
        [RequiredField]
        public FsmString value;

        public override void Reset()
        {
            id = "";
            value = null;
        }

        public override void OnEnter()
        {
            GameSettings.SetStringMain(id.Value, value.Value);

            Finish();
        }
    }
}