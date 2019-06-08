using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Create ObjectPool.")]
    public class CreateObjectPool : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Title("Prefab")]
        [Tooltip("Pool prefab.")]
        public FsmOwnerDefault prefab;

        [RequiredField]
        public FsmInt size;

        public override void Reset()
        {
            prefab = null;
            size = 0;
        }

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(prefab);
			
			if (go == null)
			{
				return;
			}

            ObjectPool.CreatePoolMain(go, size.Value, true);
            Finish();
        }
    }
}