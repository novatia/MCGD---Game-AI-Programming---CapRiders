using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Create PlayMaker Proxy.")]
    public class CreatePlayMakerProxy : FsmStateAction
    {
        public override void OnEnter()
        {
            GameObject playmakerProxy = new GameObject("PlayMaker Proxy");

            GameObject playmakerGUIPrefab = (GameObject)Resources.Load("Core/PlayMakerGUI");
            if (playmakerGUIPrefab != null)
            {
                GameObject playmakerGUI = (GameObject)GameObject.Instantiate(playmakerGUIPrefab, Vector3.zero, Quaternion.identity);
                playmakerGUI.SetParent(playmakerProxy);
            }

            GameObject playmakerUGUIProxyPrefab = (GameObject)Resources.Load("Core/PlayMakerUGUI");
            if (playmakerUGUIProxyPrefab != null)
            {
                GameObject playmakerUGUIProxy = (GameObject)GameObject.Instantiate(playmakerUGUIProxyPrefab, Vector3.zero, Quaternion.identity);
                playmakerUGUIProxy.SetParent(playmakerProxy);
            }

            GameObject.DontDestroyOnLoad(playmakerProxy);

            Finish();
        }
    }
}