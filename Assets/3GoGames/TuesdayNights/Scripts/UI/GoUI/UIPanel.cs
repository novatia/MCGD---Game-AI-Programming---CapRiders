using UnityEngine;

using System;

namespace GoUI
{
    public class UIPanel<T> : UIBasePanel where T : UIView
    {
        [SerializeField]
        private T m_ViewPrefab = null;

        private CanvasGroup m_CanvasGroup = null;
        private RectTransform m_Canvas = null;
        private T m_ViewInstance = null;

        protected T viewInstance
        {
            get
            {
                return m_ViewInstance;
            }
        }

        private bool m_IsOpen = false;

        // MonoBehaviours' interface

        protected virtual void Awake()
        {
            GetCanvas();

            if (m_Canvas != null)
            {
                if (m_ViewPrefab != null)
                {
                    T instance = Instantiate<T>(m_ViewPrefab);
                    instance.transform.SetParent(m_Canvas, false);

                    instance.Setup(this);

                    m_CanvasGroup = instance.GetComponent<CanvasGroup>();
                    if (m_CanvasGroup == null)
                    {
                        m_CanvasGroup = instance.gameObject.AddComponent<CanvasGroup>();
                    }

                    Internal_DisableCanvasGroup();

                    m_ViewInstance = instance;
                }
            }
        }

        void Update()
        {
            if (m_IsOpen)
            {
                float deltaTime = Time.deltaTime;
                OnUpdate(deltaTime);
            }
        }

        void OnDestroy()
        {
            if (m_ViewInstance != null)
            {
                Destroy(m_ViewInstance.gameObject);
            }
        }

        // UIBasePanel's interface

        public override bool isOpen
        {
            get
            {
                return m_IsOpen;
            }
        }

        public override bool isClose
        {
            get
            {
                return !m_IsOpen;
            }
        }

        public override bool isViewTotallyOpen
        {
            get
            {
                if (m_ViewInstance != null)
                {
                    return m_ViewInstance.isOpen;
                }

                return false;
            }
        }

        public override bool isViewTotallyClose
        {
            get
            {
                if (m_ViewInstance != null)
                {
                    return !m_ViewInstance.isOpen;
                }

                return false;
            }
        }

        public override bool isViewOpening
        {
            get
            {
                if (m_ViewInstance != null)
                {
                    return m_ViewInstance.isLogicallyOpen && !m_ViewInstance.isOpen;
                }

                return false;
            }
        }

        public override bool isViewClosing
        {
            get
            {
                if (m_ViewInstance != null)
                {
                    return !m_ViewInstance.isLogicallyOpen && m_ViewInstance.isOpen;
                }

                return false;
            }
        }

        public override void Open(Action i_Callback = null)
        {
            if (m_IsOpen)
                return;

            if (m_ViewInstance != null)
            {
                m_ViewInstance.Show(i_Callback);
            }

            Internal_EnableCanvasGroup();

            OnEnter();

            m_IsOpen = true;
        }

        public override void Close(Action i_Callback = null)
        {
            if (!m_IsOpen)
                return;

            m_IsOpen = false;

            OnExit();

            Internal_DisableCanvasGroup();

            if (m_ViewInstance != null)
            {
                m_ViewInstance.Hide(i_Callback);
            }
        }

        // LOGIC

        public void SetInteractable(bool i_Interactable)
        {
            if (m_IsOpen)
            {
                if (m_CanvasGroup != null)
                {
                    m_CanvasGroup.interactable = i_Interactable;
                }
            }
        }

        public void SetBlockRaycasts(bool i_BlockRaycasts)
        {
            if (m_CanvasGroup != null)
            {
                m_CanvasGroup.blocksRaycasts = i_BlockRaycasts;
            }
        }

        // PROTECTED

        protected virtual void OnEnter()
        {

        }

        protected virtual void OnUpdate(float i_DeltaTime)
        {

        }

        protected virtual void OnExit()
        {

        }

        // INTERNALS

        private void GetCanvas()
        {
            GameObject uiCanvas = GameObject.FindGameObjectWithTag("MainCanvas");

            if (uiCanvas != null)
            {
                RectTransform canvasRectTransform = uiCanvas.GetComponent<RectTransform>();
                m_Canvas = canvasRectTransform;
            }
        }

        private void Internal_EnableCanvasGroup()
        {
            if (m_CanvasGroup == null)
                return;

            m_CanvasGroup.blocksRaycasts = true;
            //m_CanvasGroup.interactable = true;
        }

        private void Internal_DisableCanvasGroup()
        {
            if (m_CanvasGroup == null)
                return;

            m_CanvasGroup.blocksRaycasts = false;
            //m_CanvasGroup.interactable = false;
        }
    }
}