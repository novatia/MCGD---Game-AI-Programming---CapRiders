using UnityEngine;

using System;
using System.Collections;

using HutongGames.PlayMaker;

using GoUI;

public class PlayMakerUGUIEventTriggerProxy : MonoBehaviour
{
    public enum PlayMakerProxyEventTarget { Owner, GameObject, BroadCastAll };

    [Serializable]
    public struct FsmEventSetup
    {
        public PlayMakerProxyEventTarget target;
        public GameObject gameObject;
        public string eventName;
    }

    [SerializeField]
    private UIEventTrigger m_UITarget = null;
    [SerializeField]
    private FsmEventSetup m_FsmEventSetup = new FsmEventSetup();

    private FsmEventTarget m_FsmEventTarget = null;

    // MonoBehaviour's interface

    void Awake()
    {
        InternalSetupEvent();
    }

    void Start()
    {
        InternalSetupCallback();
    }

    // INTERNALS

    private void InternalSetupEvent()
    {
        if (m_FsmEventTarget == null)
        {
            m_FsmEventTarget = new FsmEventTarget();
        }

        // BROADCAST
        if (m_FsmEventSetup.target == PlayMakerProxyEventTarget.BroadCastAll)
        {
            m_FsmEventTarget.target = FsmEventTarget.EventTarget.BroadcastAll;
            m_FsmEventTarget.excludeSelf = false;
        }

        // GAMEOBJECT
        else if (m_FsmEventSetup.target == PlayMakerProxyEventTarget.GameObject)
        {
            m_FsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
            m_FsmEventTarget.gameObject = new FsmOwnerDefault();
            m_FsmEventTarget.gameObject.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
            m_FsmEventTarget.gameObject.GameObject.Value = m_FsmEventSetup.gameObject;
        }

        // OWNER
        else if (m_FsmEventSetup.target == PlayMakerProxyEventTarget.Owner)
        {
            m_FsmEventTarget.ResetParameters();
            m_FsmEventTarget.target = FsmEventTarget.EventTarget.GameObject;
            m_FsmEventTarget.gameObject = new FsmOwnerDefault();
            m_FsmEventTarget.gameObject.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
            m_FsmEventTarget.gameObject.GameObject.Value = this.gameObject;
        }
    }

    private void InternalSetupCallback()
    {
        if (m_UITarget == null)
            return;

        m_UITarget.onEvent.AddListener(OnEvent);
    }

    private void FireEvent(FsmEventData i_EventData)
    {
        if (i_EventData == null)
            return;

        Fsm.EventData = i_EventData;

        if (PlayMakerUGuiSceneProxy.fsm == null)
        {
            Debug.LogError("Missing 'PlayMaker UGui' prefab in scene.");
            return;
        }

        m_FsmEventTarget.excludeSelf = false; // Why? 

        Fsm fsm = PlayMakerUGuiSceneProxy.fsm.Fsm;
        fsm.Event(m_FsmEventTarget, m_FsmEventSetup.eventName);
    }

    // CALLBACK

    private void OnEvent()
    {
        FsmEventData fsmEventData = new FsmEventData();
        FireEvent(fsmEventData);
    }
}
