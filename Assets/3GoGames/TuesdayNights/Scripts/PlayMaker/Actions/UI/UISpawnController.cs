using UnityEngine;

using System;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Spawn UI page controller.")]
    public class UISpawnController : FsmStateAction
    {
        [RequiredField]
        public FsmString name;

        [RequiredField]
        [CheckForComponent(typeof(UIController))]
        [Tooltip("Controller prefab.")]
        public FsmGameObject prefab;

        [UIHint(UIHint.Variable)]
        [Tooltip("Owner.")]
        public FsmOwnerDefault owner;

        public bool dontDestroyOnLoad = false;

        [UIHint(UIHint.Variable)]
        public FsmGameObject storeObject;

        public override void Reset()
        {
            name = "";
            prefab = null;
            owner = null;
            storeObject = null;

            dontDestroyOnLoad = false;
        }

        public override void OnEnter()
        {
            GameObject go = prefab.Value;

            if (go != null)
            {
                GameObject uiControllerGo = (GameObject)GameObject.Instantiate(go, Vector3.zero, Quaternion.identity);
                uiControllerGo.name = name.Value;

                UIController uiController = uiControllerGo.GetComponent<UIController>();
                if (uiController != null)
                {
                    GameObject ownerGo = null;

                    if (owner.OwnerOption == OwnerDefaultOption.UseOwner)
                    {
                        ownerGo = Owner;
                    }
                    else
                    {
                        if (!owner.GameObject.IsNone && owner.GameObject != null)
                        {
                            ownerGo = owner.GameObject.Value;
                        }
                    }

                    uiController.Bind(ownerGo);
                }

                if (dontDestroyOnLoad)
                {
                    GameObject.DontDestroyOnLoad(uiControllerGo);
                }

                storeObject.Value = uiControllerGo;
            }

            Finish();
        }
    }
}