using UnityEngine;
using UnityEngine.UI;

using System;

namespace GoUI
{
    public static class UI
    {
        public static void OpenPanel(UIBasePanel i_Panel, Action i_Callback = null)
        {
            if (i_Panel == null)
                return;

            if (i_Panel.isClose)
            {
                i_Panel.Open(i_Callback);
            }
        }

        public static void ClosePanel(UIBasePanel i_Panel, Action i_Callback = null)
        {
            if (i_Panel == null)
                return;

            if (i_Panel.isOpen)
            {
                i_Panel.Close(i_Callback);
            }
        }

        public static GameObject GetFirstSelectableGameObject(GameObject i_Root)
        {
            if (i_Root == null)
            {
                return null;
            }

            Selectable[] selectables = i_Root.GetComponentsInChildren<Selectable>();
            for (int index = 0; index < selectables.Length; ++index)
            {
                Selectable selectable = selectables[index];

                if (selectable == null)
                    continue;

                if (selectable.IsActive() && selectable.IsInteractable())
                {
                    GameObject go = selectable.gameObject;
                    return go;
                }
            }

            return null;
        }

        public static void DeactivateEventTriggers(GameObject i_Root)
        {
            if (i_Root == null)
                return;

            UIEventTrigger[] triggers = i_Root.GetComponentsInChildren<UIEventTrigger>(true);
            for (int index = 0; index < triggers.Length; ++index)
            {
                UIEventTrigger trigger = triggers[index];

                if (trigger == null)
                    continue;

                trigger.enabled = false;
            }
        }

        public static void ActivateEventTriggers(GameObject i_Root)
        {
            if (i_Root == null)
                return;

            UIEventTrigger[] triggers = i_Root.GetComponentsInChildren<UIEventTrigger>(true);
            for (int index = 0; index < triggers.Length; ++index)
            {
                UIEventTrigger trigger = triggers[index];

                if (trigger == null)
                    continue;

                trigger.enabled = true;
            }
        }

        public static void RemoveFocusFrom(GameObject i_Root)
        {
            if (i_Root == null)
                return;

            GameObject currentFocus = UIEventSystem.focusMain;
            if (currentFocus != null)
            {
                if (i_Root.transform.HasChild(currentFocus))
                {
                    UIEventSystem.SetFocusMain(null);
                }
            }
        }

        public static Selectable GetSelectableOn(Selectable i_Current, UINavigationDirection i_NavigationDirection)
        {
            if (i_Current == null)
            {
                return null;
            }

            Selectable selectable = null;

            switch (i_NavigationDirection)
            {
                case UINavigationDirection.Down:
                    selectable = GetSelectableOnDown(i_Current);
                    break;

                case UINavigationDirection.Left:
                    selectable = GetSelectableOnLeft(i_Current);
                    break;

                case UINavigationDirection.Right:
                    selectable = GetSelectableOnRight(i_Current);
                    break;

                case UINavigationDirection.Up:
                    selectable = GetSelectableOnUp(i_Current);
                    break;
            }

            return selectable;
        }

        public static Selectable GetSelectableOnLeft(Selectable i_Current)
        {
            if (i_Current == null)
            {
                return null;
            }

            Navigation navigation = i_Current.navigation;
            return navigation.selectOnLeft;
        }

        public static Selectable GetSelectableOnRight(Selectable i_Current)
        {
            if (i_Current == null)
            {
                return null;
            }

            Navigation navigation = i_Current.navigation;
            return navigation.selectOnRight;
        }

        public static Selectable GetSelectableOnDown(Selectable i_Current)
        {
            if (i_Current == null)
            {
                return null;
            }

            Navigation navigation = i_Current.navigation;
            return navigation.selectOnDown;
        }

        public static Selectable GetSelectableOnUp(Selectable i_Current)
        {
            if (i_Current == null)
            {
                return null;
            }

            Navigation navigation = i_Current.navigation;
            return navigation.selectOnUp;
        }

    }
}