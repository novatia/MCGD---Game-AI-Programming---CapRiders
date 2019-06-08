using UnityEngine;

using System;
using System.Collections;

namespace GoUI
{
    public class UIView : MonoBehaviour
    {
        private static int s_ClosedStateId = Animator.StringToHash("Closed");
        private static int s_OpenBoolId = Animator.StringToHash("Open");

        [SerializeField]
        private GameObject m_Root = null;

        private UIBasePanel m_Panel = null;

        private RectTransform m_RectTransform = null;
        private CanvasGroup m_CanvasGroup = null;
        private Animator m_Animator = null;

        private Coroutine m_Coroutine = null;
        private bool m_IsCoroutineRunning = false;

        private bool m_IsLogicallyOpen = false;
        private bool m_IsOpen = false;

        private Action m_Callback = null;

        // ACCESSORS

        protected GameObject root
        {
            get
            {
                return m_Root;
            }
        }

        public UIBasePanel panel
        {
            get
            {
                return m_Panel;
            }
        }

        public bool isLogicallyOpen
        {
            get
            {
                return m_IsLogicallyOpen;
            }
        }

        public bool isOpen
        {
            get
            {
                return m_IsOpen;
            }
        }

        // MonoBehaviour's interface

        protected virtual void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();

            if (m_Root != null)
            {
                m_CanvasGroup = m_Root.GetComponent<CanvasGroup>();
                if (m_CanvasGroup == null)
                {
                    m_CanvasGroup = m_Root.AddComponent<CanvasGroup>();
                }
            }

            DisableCanvasGroup();

            m_Animator = GetComponent<Animator>();
            if (m_Animator != null)
            {
                m_Animator.CrossFade(s_ClosedStateId, 0f);
            }

            m_IsOpen = false;
            m_IsLogicallyOpen = false;
        }

        void Update()
        {
            if (m_IsOpen)
            {
                float deltaTime = Time.deltaTime;
                OnUpdate(deltaTime);
            }
        }

        // LOGIC

        public void Setup(UIBasePanel i_Owner)
        {
            m_Panel = i_Owner;
            UI.DeactivateEventTriggers(gameObject);

            SetRootActive(false);
        }

        public void Show(Action i_Callback = null)
        {
            if (m_IsLogicallyOpen)
                return;

            m_IsLogicallyOpen = true;

            if (m_IsCoroutineRunning)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
                m_IsCoroutineRunning = false;
                m_Callback = null;
            }

            m_Callback = i_Callback;

            m_Coroutine = StartCoroutine(ShowView());
        }

        public void Hide(Action i_Callback = null)
        {
            if (!m_IsLogicallyOpen)
                return;

            m_IsLogicallyOpen = false;

            if (m_IsCoroutineRunning)
            {
                StopCoroutine(m_Coroutine);
                m_Coroutine = null;
                m_IsCoroutineRunning = false;
                m_Callback = null;
            }

            m_Callback = i_Callback;

            m_Coroutine = StartCoroutine(HideView());
        }

        // VIRTUALS

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

        private IEnumerator ShowView()
        {
            m_IsCoroutineRunning = true;

            yield return null;

            SetRootActive(true);

            MoveOutView();

            yield return null;

            MoveInView();

            if (m_Animator != null)
            {
                m_Animator.SetBool(s_OpenBoolId, true);

                yield return null;

                AnimatorClipInfo[] animatorClipsInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
                if (animatorClipsInfo != null && animatorClipsInfo.Length > 0)
                {
                    AnimatorClipInfo animatorClipInfo = animatorClipsInfo[0];
                    AnimationClip clip = animatorClipInfo.clip;
                    if (clip != null)
                    {
                        if (clip.length > 0f)
                        {
                            AnimatorStateInfo animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
                            float normalizedTime = animatorStateInfo.normalizedTime;
                            while (normalizedTime < 1f)
                            {
                                yield return null;

                                animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
                                normalizedTime = animatorStateInfo.normalizedTime;
                            }
                        }
                    }
                }
            }

            EnableCanvasGroup();

            m_IsOpen = true;

            UI.ActivateEventTriggers(gameObject);

            GameObject focusGo = GetFocusGameObject(gameObject);
            if (focusGo != null)
            {
                UIEventSystem.SetFocusMain(focusGo);
            }

            OnEnter();

            m_IsCoroutineRunning = false;

            Callback();
        }

        private IEnumerator HideView()
        {
            m_IsCoroutineRunning = true;

            UI.DeactivateEventTriggers(gameObject);
            UI.RemoveFocusFrom(gameObject);

            OnExit();

            DisableCanvasGroup();

            if (m_Animator != null)
            {
                m_Animator.SetBool(s_OpenBoolId, false);

                yield return null;

                AnimatorClipInfo[] animatorClipsInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
                if (animatorClipsInfo != null && animatorClipsInfo.Length > 0)
                {
                    AnimatorClipInfo animatorClipInfo = animatorClipsInfo[0];
                    AnimationClip clip = animatorClipInfo.clip;
                    if (clip != null)
                    {
                        if (clip.length > 0f)
                        {
                            AnimatorStateInfo animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
                            float normalizedTime = animatorStateInfo.normalizedTime;
                            while (normalizedTime < 1f)
                            {
                                yield return null;

                                animatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
                                normalizedTime = animatorStateInfo.normalizedTime;
                            }
                        }
                    }
                }
            }

            MoveOutView();

            yield return null;

            // MoveInView();

            SetRootActive(false);

            yield return null;

            m_IsOpen = false;

            m_IsCoroutineRunning = false;

            Callback();
        }

        private GameObject GetFocusGameObject(GameObject i_Root)
        {
            if (i_Root == null)
            {
                return null;
            }

            GameObject focus = i_Root.FindChildWithTag("Focus");
            return focus;
        }

        private void Callback()
        {
            if (m_Callback != null)
            {
                m_Callback();
                m_Callback = null;
            }
        }

        // PROTECTED

        protected T GetPanel<T>() where T : UIBasePanel
        {
            if (m_Panel == null)
            {
                return null;
            }

            if (m_Panel is T)
            {
                T typedPanel = (T)m_Panel;
                return typedPanel;
            }

            return null;
        }

        // UTILS

        private void EnableCanvasGroup()
        {
            if (m_CanvasGroup == null)
                return;

            m_CanvasGroup.blocksRaycasts = true;
            //m_CanvasGroup.interactable = true;
        }

        private void DisableCanvasGroup()
        {
            if (m_CanvasGroup == null)
                return;

            m_CanvasGroup.blocksRaycasts = false;
            //m_CanvasGroup.interactable = false;
        }

        private void SetRootActive(bool i_Active)
        {
            if (m_Root != null)
            {
                m_Root.SetActive(i_Active);
            }
        }

        private void MoveOutView()
        {
            m_RectTransform.SetAnchor(-1f, -1f, 0f, 0f);
            m_RectTransform.sizeDelta = Vector2.zero;
        }

        private void MoveInView()
        {
            m_RectTransform.SetAnchor(UIAnchor.s_Stretch);
            m_RectTransform.sizeDelta = Vector2.zero;
        }
    }
}