using UnityEngine;
using UnityEngine.EventSystems;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Clear InputModule.")]
    public class ClearInputModule : FsmStateAction
    {
        public override void OnEnter()
        {
            InputModule inputModule = UIEventSystem.inputModuleMain;
            if (inputModule != null)
            {
                inputModule.Clear();
            }

            Finish();
        }
    }
}
