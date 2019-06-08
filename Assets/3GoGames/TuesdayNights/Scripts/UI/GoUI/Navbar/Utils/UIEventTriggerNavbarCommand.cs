using UnityEngine;

namespace GoUI
{
    [RequireComponent(typeof(UIEventTrigger))]
    public class UIEventTriggerNavbarCommand : UINavbarCommand
    {
        private UIEventTrigger m_Trigger = null;

        // MonoBehaviour's INTERFACE

        protected override void Awake()
        {
            base.Awake();

            m_Trigger = GetComponent<UIEventTrigger>();
        }

        // UINavbarEntry's INTERFACE

        public override bool isActive
        {
            get
            {
                return m_Trigger.canSend;
            }
        }
    }
}