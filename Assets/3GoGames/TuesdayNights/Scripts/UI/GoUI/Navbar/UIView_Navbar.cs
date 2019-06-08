using UnityEngine;

namespace GoUI
{
    public class UIView_Navbar : UIView
    {
        // STATIC

        private static string s_IconsGroup_Keyboard = "GROUP_KEYBOARD";
        private static string s_IconsGroup_Pad = "GROUP_PAD";

        // Serializeable fields

        [SerializeField]
        private UINavbarElement[] m_NavbarElements = null;
        [SerializeField]
        private RuntimeAnimatorController m_ElementsAnimatorController = null;

        // MonoBehaviour's interface

        protected override void Awake()
        {
            base.Awake();

            // Clear elements.

            if (m_NavbarElements != null)
            {
                for (int elementIndex = 0; elementIndex < m_NavbarElements.Length; ++elementIndex)
                {
                    UINavbarElement element = m_NavbarElements[elementIndex];

                    if (element == null)
                        continue;

                    element.isVisible = false;

                    if (m_ElementsAnimatorController != null)
                    {
                        Animator elementAnimator = element.GetComponent<Animator>();
                        if (elementAnimator == null)
                        {
                            elementAnimator = element.gameObject.AddComponent<Animator>();
                        }

                        elementAnimator.runtimeAnimatorController = m_ElementsAnimatorController;
                    }
                }
            }
        }

        // UIView's interface

        protected override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void OnUpdate(float i_DeltaTime)
        {
            base.OnUpdate(i_DeltaTime);
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        // LOGIC

        public void ClearAll()
        {
            if (m_NavbarElements == null)
                return;

            for (int index = 0; index < m_NavbarElements.Length; ++index)
            {
                SetElement(index, null);
            }
        }

        public void SetElement(int i_Index, UINavbarCommand i_Command)
        {
            if (m_NavbarElements == null)
                return;

            if (i_Index < 0 || i_Index >= m_NavbarElements.Length)
                return;

            UINavbarElement element = m_NavbarElements[i_Index];

            if (element == null)
                return;

            element.Clear();
            element.isVisible = false;

            if (i_Command == null)
                return;

            element.isVisible = true;

            Sprite sprite = Internal_GetSprite(i_Command.iconKey);
            element.SetIcon(sprite);

            element.SetText(i_Command.text);

            element.isActive = i_Command.isActive;
        }

        // INTERNALS

        private Sprite Internal_GetSprite(string i_IconId)
        {
            Sprite sprite = null;

            if (InputSystem.player0Main != null && InputSystem.player0Main.JoystickCount > 0)
            {
                sprite = UIIconsDatabaseManager.GetIconMain(s_IconsGroup_Pad, i_IconId);
            }
            else
            {
                sprite = UIIconsDatabaseManager.GetIconMain(s_IconsGroup_Keyboard, i_IconId);
            }

            return sprite;
        }
    }
}