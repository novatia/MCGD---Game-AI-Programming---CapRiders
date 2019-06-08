using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Create User Stats Updater.")]
    public class tnCreateUserStatsUpdater : FsmStateAction
    {
        public override void OnEnter()
        {
            GameObject go = new GameObject("tnUserStatsUpdater");
            go.AddComponent<tnUserStatsUpdater>();

            GameObject.DontDestroyOnLoad(go);

            Finish();
        }
    }
}
