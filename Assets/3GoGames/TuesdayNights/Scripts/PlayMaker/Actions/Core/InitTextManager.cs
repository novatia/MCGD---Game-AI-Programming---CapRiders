using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Initialize TextManager.")]
    public class InitTextManager : FsmStateAction
    {
        [RequiredField]
        public FsmString filePath = "";

        public override void Reset()
        {
            filePath = "";
        }

        public override void OnEnter()
        {
            string path = (!filePath.IsNone) ? filePath.Value : "";
            TextManager.InitializeMain(path);
            Finish();
        }
    }
}