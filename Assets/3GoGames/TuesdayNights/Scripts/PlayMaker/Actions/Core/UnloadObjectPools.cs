using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Unload all pools in a given ObjectPoolController.")]
    public class UnloadObjectPools : FsmStateAction
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
                    poolController.UnloadAll();
                }
            }

            ObjectPool.ReleaseSpawnedMain(); // Release all spawner objects.

            Finish();
        }
    }
}