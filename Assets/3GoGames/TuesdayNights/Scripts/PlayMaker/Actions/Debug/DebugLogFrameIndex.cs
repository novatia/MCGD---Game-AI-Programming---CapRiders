using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Debug")]
    [Tooltip("Log frame count.")]
    public class DebugLogFrameIndex : FsmStateAction
    {
        public override void OnEnter()
        {
            Debug.Log("ENTER [" + State.Name + "]: FRAME " + Time.frameCount);
            Finish();
        }

        public override void OnExit()
        {
            Debug.Log("EXIT [" + State.Name + "]: FRAME " + Time.frameCount);
        }
    }
}