using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using HutongGames.PlayMaker;

public static class FsmUtils
{
    public static void SetReference(PlayMakerFSM i_Target, string i_Name, GameObject i_Reference)
    {
        if (i_Target == null)
            return;

        FsmGameObject reference = i_Target.FsmVariables.FindFsmGameObject(i_Name);
        if (reference != null)
        {
            reference.Value = i_Reference;
        }
    }

    public static void SendEvent(GameObject i_Target, string i_EventName, FsmEventData i_EventData = null)
    {
        if (i_Target == null)
            return;

        if (i_EventData != null)
        {
            HutongGames.PlayMaker.Fsm.EventData = i_EventData;
        }

        NotifyEvent(i_Target, i_EventName);
    }

    public static void SendEvent(GameObject i_Target, string i_EventName, PlayMakerEventParams i_Params)
    {
        if (i_Target == null)
            return;

        if (i_Params != null)
        {
            PlayMakerEventData.SetValues(i_Params);
        }

        NotifyEvent(i_Target, i_EventName);
    }

    public static void FirePlayMakerEvent(PlayMakerFSM i_Source, string i_EventName, GameObject i_Target, FsmEventData i_EventData)
    {
        if (i_EventData != null)
        {
            HutongGames.PlayMaker.Fsm.EventData = i_EventData;
        }

        if (i_Source == null)
        {
            return;
        }

        FsmEventTarget eventTarget = new FsmEventTarget();
        eventTarget.excludeSelf = false;
        eventTarget.sendToChildren = true;

        eventTarget.target = FsmEventTarget.EventTarget.GameObject;

        FsmOwnerDefault owner = new FsmOwnerDefault();
        owner.OwnerOption = OwnerDefaultOption.SpecifyGameObject;
        owner.GameObject = new FsmGameObject();
        owner.GameObject.Value = i_Target;

        eventTarget.gameObject = owner;

        i_Source.Fsm.Event(eventTarget, i_EventName);
    }

    // INTERNALS

    private static void NotifyEvent(GameObject i_Target, string i_EventName)
    {
        if (i_Target == null)
            return;

        PlayMakerFSM[] targetFSMs = i_Target.GetComponents<PlayMakerFSM>();

        for (int fsmIndex = 0; fsmIndex < targetFSMs.Length; ++fsmIndex)
        {
            PlayMakerFSM targetFSM = targetFSMs[fsmIndex];
            targetFSM.SendEvent(i_EventName);
        }
    }
}

public static class PlayMakerFsmExtensions
{
    public static void SetReference(this PlayMakerFSM i_Target, string i_Name, GameObject i_Reference)
    {
        FsmUtils.SetReference(i_Target, i_Name, i_Reference);
    }
}