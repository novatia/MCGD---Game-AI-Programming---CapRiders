using UnityEngine;

using System;

namespace GoUI
{
    public abstract class UINavbarCommand : MonoBehaviour
    {
        [SerializeField]
        private string m_IconKey = "";
        [SerializeField]
        private string m_Text = "";
        [SerializeField]
        private int m_SortIndex = -1;

        private int m_IconKeyHash;

        private bool m_LastIsActive = false; 

        public event Action<UINavbarCommand> onCommandStateChanged = null;

        // ACCESSORS

        public string iconKey
        {
            get
            {
                return m_IconKey;
            }
        }

        public int iconKeyHash
        {
            get
            {
                return m_IconKeyHash;
            }
        }

        public string text
        {
            get
            {
                return m_Text;
            }
        }

        public int sortIndex
        {
            get
            {
                return m_SortIndex;
            }
        }

        // UINavbarCommand's INTERFACE

        public virtual bool isActive
        {
            get
            {
                return true;
            }
        }

        // MonoBehaviour's INTERFACE

        protected virtual void Awake()
        {
            m_IconKeyHash = StringUtils.GetHashCode(m_IconKey);
        }

        protected virtual void Start()
        {
            m_LastIsActive = isActive;
        }

        protected virtual void Update()
        {
            bool currentIsActive = isActive;
            if (currentIsActive != m_LastIsActive)
            {
                CommandStateChanged();
            }

            m_LastIsActive = currentIsActive;
        }

        // PROTECTED

        private void CommandStateChanged()
        {
            if (onCommandStateChanged != null)
            {
                onCommandStateChanged(this);
            }
        }
    }
}
