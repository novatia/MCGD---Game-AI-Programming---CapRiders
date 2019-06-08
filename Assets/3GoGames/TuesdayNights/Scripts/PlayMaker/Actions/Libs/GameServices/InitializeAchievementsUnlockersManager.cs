using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Game Services")]
    [Tooltip("Initialize Achievements Unlockers Manager.")]
    public class InitializeAchievementsUnlockersManager : FsmStateAction
    {
        public override void OnEnter()
        {
            GameObject go = new GameObject("AchievementsUnlockersManager");
            go.AddComponent<AchievementsUnlockersManager>();

            Finish();
        }
    }
}