using UnityEngine;
using System.Collections;

using HutongGames.PlayMaker;

[RequireComponent(typeof(PlayMakerFSM))]
public class UIController : MonoBehaviour 
{
    public UIView viewPrefab = null;

    private PlayMakerFSM m_Fsm = null;
    private UIView m_ViewInstance = null;

    private const string s_InitEvent = "UICONTROLLER / INIT";

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_Fsm = GetComponent<PlayMakerFSM>();

        // Create UI View.

        if (viewPrefab == null)
            return;

        GameObject mainCanvasGo = GameObject.FindGameObjectWithTag("MainCanvas");

        if (mainCanvasGo == null)
            return;

        Canvas mainCanvas = mainCanvasGo.GetComponent<Canvas>();

        if (mainCanvas == null)
            return;

        UIView viewInstance = (UIView)Instantiate(viewPrefab);
        if (viewInstance != null)
        {
            viewInstance.transform.SetParent(mainCanvas.transform, false);
            viewInstance.Bind(this);

            m_ViewInstance = viewInstance;

            // Set a reference to the View on Controller's FSM.

            FsmGameObject viewReference = m_Fsm.FsmVariables.FindFsmGameObject("UIView");

            if (viewReference != null)
            {
                viewReference.Value = viewInstance.gameObject;
            }
        }
    }

    void OnDestroy()
    {
        // Destroy UI View.

        if (m_ViewInstance != null)
        {
            Destroy(m_ViewInstance.gameObject);
            m_ViewInstance = null;
        }
    }

    // BUSINESS LOGIC

    public bool isViewOpened
    {
        get
        {
            if (m_ViewInstance == null)
            {
                return false;
            }

            return m_ViewInstance.isOpened;
        }
    }

    public bool isViewClosed
    {
        get
        {
            if (m_ViewInstance == null)
            {
                return false;
            }

            return m_ViewInstance.isClosed;
        }
    }

    public void Bind(GameObject i_Owner)
    {
        Fsm.EventData = new FsmEventData();
        Fsm.EventData.GameObjectData = i_Owner != null ? i_Owner.gameObject : null;

        m_Fsm.Fsm.Event(s_InitEvent);
    }
    
    public void Present()
    {
        if (m_ViewInstance != null)
        {
            m_ViewInstance.Present();
        }
    }

    public void Dismiss()
    {
        if (m_ViewInstance != null)
        {
            m_ViewInstance.Dismiss();
        }
    }
}

