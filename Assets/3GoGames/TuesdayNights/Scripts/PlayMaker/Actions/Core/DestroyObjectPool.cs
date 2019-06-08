using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Destroy ObjectPool.")]
    public class DestroyObjectPool : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Title("Prefab")]
        [Tooltip("Pool prefab.")]
        public FsmOwnerDefault prefab;

        public FsmBool recycleAll = false;

        public override void Reset()
        {
            prefab = null;
            recycleAll = false;
        }

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(prefab);

            if (go == null)
            {
                return;
            }

            if (recycleAll.Value)
            {
                ObjectPool.DestroyAllMain(go);
            }
            else
            {
                ObjectPool.DestroyPooledMain(go);
            }

            Finish();
        }
    }
}