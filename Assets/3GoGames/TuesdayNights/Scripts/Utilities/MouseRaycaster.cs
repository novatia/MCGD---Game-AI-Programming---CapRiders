using UnityEngine;

using System;
using System.Collections;
using UnityEngine.EventSystems;

public enum MouseRaycasterEventType
{
    OnEnterEvent,
    OnExitEvent
}

public struct MouseRaycasterEventParams
{ 
    private MouseRaycasterEventType m_EventType;
    public MouseRaycasterEventType eventType
    {
        get
        {
            return m_EventType;
        }
    }

    private Vector2 m_Position;
    public Vector2 position
    {
        get
        {
            return m_Position;
        }
    }

    private GameObject m_GameObject;
    public GameObject gameObject
    {
        get
        {
            return m_GameObject;
        }
    }

    public MouseRaycasterEventParams(MouseRaycasterEventType i_EventType, Vector2 i_Position, GameObject i_GameObject)
    {
        m_EventType = i_EventType;

        m_Position = i_Position;
        m_GameObject = i_GameObject;
    }
}

public class MouseRaycaster : Singleton<MouseRaycaster>
{
    private class Raycaster
    {
        private GameObject m_PrevSelectedGO = null;
        private LayerMask m_LayerMask = 0;

        public Action<MouseRaycasterEventParams> onEvent;

        // BUSINESS LOGIC

        public void Update(Vector2 i_MousePosition, bool i_Valid)
        {
            if (onEvent == null)
                return; // Empty listener list, we can skip this raycaster.

            GameObject selectedGo = null;

            if (i_Valid)
            {
                Collider2D selectedCollider = Physics2D.OverlapCircle(i_MousePosition, 0.05f, m_LayerMask);
                if (selectedCollider != null)
                {
                    selectedGo = selectedCollider.gameObject;
                }
            }

            if (selectedGo != m_PrevSelectedGO)
            {
                if (m_PrevSelectedGO != null)
                {
                    if (onEvent != null)
                    {
                        MouseRaycasterEventParams eventParams = new MouseRaycasterEventParams(MouseRaycasterEventType.OnExitEvent, i_MousePosition, m_PrevSelectedGO);
                        onEvent(eventParams);
                    }
                }

                if (selectedGo != null)
                {
                    if (onEvent != null)
                    {
                        MouseRaycasterEventParams eventParams = new MouseRaycasterEventParams(MouseRaycasterEventType.OnEnterEvent, i_MousePosition, selectedGo);
                        onEvent(eventParams);
                    }
                }
            }

            m_PrevSelectedGO = selectedGo;
        }

        public void Clear()
        {
            // TODO: Raise 'OnExit' event.

            m_PrevSelectedGO = null;
        }

        // CTOR

        public Raycaster(int i_LayerIndex)
        {
            m_LayerMask = (1 << i_LayerIndex);
        }
    }

    private Raycaster[] m_Raycasters = new Raycaster[32];

    // STATIC METHODS

    public static void InitializeMain()
    {
        if (MouseRaycaster.Instance != null)
        {
            MouseRaycaster.Instance.Initialize();
        }
    }
   
    public static void AddListenerMain(int i_LayerIndex, Action<MouseRaycasterEventParams> i_OnEvent)
    {
        if (MouseRaycaster.Instance != null)
        {
            MouseRaycaster.Instance.AddListener(i_LayerIndex, i_OnEvent);
        }
    }

    public static void RemoveListenerMain(int i_LayerIndex, Action<MouseRaycasterEventParams> i_OnEvent)
    {
        if (MouseRaycaster.Instance != null)
        {
            MouseRaycaster.Instance.RemoveListener(i_LayerIndex, i_OnEvent);
        }
    }

    // BUSINESS LOGIC

    public void Initialize()
    {

    }

    public void AddListener(int i_LayerIndex, Action<MouseRaycasterEventParams> i_OnEvent)
    {
        if (i_LayerIndex < 0 || i_LayerIndex >= 32)
            return;

        Raycaster raycaster = m_Raycasters[i_LayerIndex];

        if (i_OnEvent != null)
        {
            raycaster.onEvent += i_OnEvent;
        }
    }

    public void RemoveListener(int i_LayerIndex, Action<MouseRaycasterEventParams> i_OnEvent)
    {
        if (i_LayerIndex < 0 || i_LayerIndex >= 32)
            return;

        Raycaster raycaster = m_Raycasters[i_LayerIndex];

        if (i_OnEvent != null)
        {
            raycaster.onEvent -= i_OnEvent;
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        for (int layerIndex = 0; layerIndex < m_Raycasters.Length; ++layerIndex)
        {
            m_Raycasters[layerIndex] = new Raycaster(layerIndex);
        }
    }

    void Update()
    {
        if (Camera.main == null)
            return;

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        bool valid = (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject());

        for (int layerIndex = 0; layerIndex < m_Raycasters.Length; ++layerIndex)
        {
            Raycaster raycaster = m_Raycasters[layerIndex];
            raycaster.Update(mousePosition, valid);
        }
    }
}
