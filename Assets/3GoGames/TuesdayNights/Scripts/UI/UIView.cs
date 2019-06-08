using UnityEngine;
using System.Collections;

using HutongGames.PlayMaker;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(PlayMakerFSM))]
public class UIView : MonoBehaviour 
{
    private PlayMakerFSM m_Fsm = null;

    private const string s_InitEvent = "UIVIEW / INIT";

    private const string s_PresentEvent = "UIVIEW / PRESENT";
    private const string s_DismissEvent = "UIVIEW / DISMISS";

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_Fsm = GetComponent<PlayMakerFSM>();
    }

    void OnDestroy()
    {

    }

    // BUSINESS LOGIC

    public bool isOpened
    {
        get
        {
            return (m_Fsm.ActiveStateName == "Opened");
        }
    }

    public bool isClosed
    {
        get
        {
            return (m_Fsm.ActiveStateName == "Closed");
        }
    }

    public void Bind(UIController i_Controller)
    {
        Fsm.EventData = new FsmEventData();
        Fsm.EventData.GameObjectData = i_Controller.gameObject;

        m_Fsm.Fsm.Event(s_InitEvent);
    }

    public void Present()
    {
        /*
       
        FsmEventData eventData = new FsmEventData();
        FirePlayMakerEvent(s_PresentEvent, eventData);

        */

        m_Fsm.SendEvent(s_PresentEvent);
    }

    public void Dismiss()
    {
        /*
        
        FsmEventData eventData = new FsmEventData();
        FirePlayMakerEvent(s_DismissEvent, eventData);
        
        */

        m_Fsm.SendEvent(s_DismissEvent);
    }
}
