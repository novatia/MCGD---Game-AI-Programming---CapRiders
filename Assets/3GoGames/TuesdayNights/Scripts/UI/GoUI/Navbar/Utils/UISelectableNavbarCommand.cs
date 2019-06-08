using UnityEngine;
using UnityEngine.UI;

namespace GoUI
{
    [RequireComponent(typeof(Selectable))]
    public class UISelectableNavbarCommand : UINavbarCommand
    {
        private Selectable m_Selectable = null;

        // MonoBehaviour's INTERFACE

        protected override void Awake()
        {
            base.Awake();

            m_Selectable = GetComponent<Selectable>();
        }

        // UINavbarEntry's INTERFACE

        public override bool isActive
        {
            get
            {
                return m_Selectable.IsInteractable();
            }
        }
    }
}
