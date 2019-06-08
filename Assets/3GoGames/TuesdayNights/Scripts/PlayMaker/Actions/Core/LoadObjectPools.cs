using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Load a list of pools specified in a given ObjectPoolController.")]
    public class LoadObjectPools : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(ObjectPoolController))]
        public FsmOwnerDefault gameObject;

        public override void Reset()
        {
            gameObject = null;
        }

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
            if (go != null)
            {
                ObjectPoolController poolController = go.GetComponent<ObjectPoolController>();
                if (poolController != null)
                {
                    poolController.LoadAll();
                }
            }

            Finish();
        }
    }
}