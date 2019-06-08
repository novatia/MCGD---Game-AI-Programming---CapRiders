namespace HutongGames.PlayMaker.Actions
{
    public abstract class AddModule<T> : FsmStateAction where T : GameModule, new()
    {
        public override void OnEnter()
        {
            GameModulesManager.AddModuleMain<T>();
            Finish();
        }
    }
}
