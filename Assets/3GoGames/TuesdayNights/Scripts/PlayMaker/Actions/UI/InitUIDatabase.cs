using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Initialize UI Database.")]
    public class InitUIDatabase : FsmStateAction
    {
        public override void OnEnter()
        {
            UIIconsDatabaseManager.InitializeMain();
            Finish();
        }
    }
}