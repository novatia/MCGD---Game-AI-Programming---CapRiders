using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Initialize GameModulesManager.")]
    public class InitGameModulesManager : FsmStateAction
    {
        public override void OnEnter()
        {
            GameModulesManager.InitializeMain();
            Finish();
        }
    }
}