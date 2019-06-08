using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Create Analytics Manager.")]
    public class tnCreateAnalyticsManager : FsmStateAction
    {
        public override void OnEnter()
        {
            GameObject go = new GameObject("tnAnalyticsManager");
            go.AddComponent<tnAnalyticsManager>();

            GameObject.DontDestroyOnLoad(go);

            Finish();
        }
    }
}
