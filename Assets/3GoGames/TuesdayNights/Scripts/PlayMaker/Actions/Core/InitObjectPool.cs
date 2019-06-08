using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Initialize ObjectPool.")]
    public class InitObjectPool : FsmStateAction
    {
        public override void OnEnter()
        {
            ObjectPool.InitializeMain();
            Finish();
        }
    }
}