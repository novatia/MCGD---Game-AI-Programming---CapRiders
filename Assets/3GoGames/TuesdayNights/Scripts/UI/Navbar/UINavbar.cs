using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using GoUI;

using PlayerInputEvents;

public class UINavbar : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_Root = null;
    [SerializeField]
    private UINavbarItem[] m_NavbarItems = null;
    [SerializeField]
    private bool m_AutomaticGenerateItems = false;
    [SerializeField]
    private int m_MaxItems = 8;
    [SerializeField]
    private UINavbarItem m_NavbarItemPrefab = null;

    private List<List<UINavbarEntry>> m_RegisteredEntries = new List<List<UINavbarEntry>>();

    private bool m_Dirty = false;

    private static string s_IconsGroup_Keyboard = "GROUP_KEYBOARD";
    private static string s_IconsGroup_Pad = "GROUP_PAD";

    // MonoBehaviour's interface

    void Awake()
    {
        if (m_AutomaticGenerateItems && m_Root != null)
        {
            // Cleanup Root children.

            for (int childIndex = 0; childIndex < m_Root.childCount; ++childIndex)
            {
                Transform childTransform = m_Root.GetChild(childIndex);
                Destroy(childTransform.gameObject);
            }

            // Create an Horizontal Layout Group in order to arrange items.

            HorizontalLayoutGroup horizontalLayoutGroup = m_Root.gameObject.AddComponent<HorizontalLayoutGroup>();
            horizontalLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            horizontalLayoutGroup.spacing = 0f;
            horizontalLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
            horizontalLayoutGroup.childForceExpandHeight = false;
            horizontalLayoutGroup.childForceExpandWidth = true;

            // Create prefab items.

            if (m_NavbarItemPrefab != null)
            {
                m_NavbarItems = new UINavbarItem[m_MaxItems];

                for (int itemIndex = 0; itemIndex < m_NavbarItems.Length; ++itemIndex)
                {
                    UINavbarItem navbarItem = Instantiate<UINavbarItem>(m_NavbarItemPrefab);
                    navbarItem.transform.SetParent(m_Root, false);
                    navbarItem.transform.SetAsLastSibling();

                    m_NavbarItems[itemIndex] = navbarItem;
                }
            }
        }

        // Clear items.

        for (int itemIndex = 0; itemIndex < m_NavbarItems.Length; ++itemIndex)
        {
            UINavbarItem navbarItem = m_NavbarItems[itemIndex];
            if (navbarItem != null)
            {
                navbarItem.isVisible = false;
            }
        }

        // Build registration map.

        for (int index = 0; index < m_NavbarItems.Length; ++index)
        {
            m_RegisteredEntries.Add(new List<UINavbarEntry>());
        }
    }

    void OnEnable()
    {
        InputSystem.onControllerConnectedEventMain += InternalControllerConnected;
        InputSystem.onControllerDisconnectedEventMain += InternalControllerConnected;

        UIEventSystem.onFocusChangedMain += InternalFocusChaged;
        UIEventSystem.onTriggerAddedMain += InternalTriggerAdded;
        UIEventSystem.onTriggerRemovedMain += InternalTriggerRemoved;
    }

    void OnDisable()
    {
        InputSystem.onControllerConnectedEventMain -= InternalControllerConnected;
        InputSystem.onControllerDisconnectedEventMain -= InternalControllerConnected;

        UIEventSystem.onFocusChangedMain -= InternalFocusChaged;
        UIEventSystem.onTriggerAddedMain -= InternalTriggerAdded;
        UIEventSystem.onTriggerRemovedMain -= InternalTriggerRemoved;
    }

    void Update()
    {
        InternalRefreshNavbar();
    }

    // LOGIC

    public void Show()
    {
        if (m_Root != null)
        {
            m_Root.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (m_Root != null)
        {
            m_Root.gameObject.SetActive(false);
        }
    }

    // INTERNALS

    private bool InternalRegister(UINavbarEntry i_Entry)
    {
        int index = i_Entry.sortIndex;

        if (index < 0 || index >= m_RegisteredEntries.Count)
        {
            return false;
        }

        List<UINavbarEntry> entries = m_RegisteredEntries[index];
        entries.Add(i_Entry);

        return true;
    }

    private bool InternalUnregister(UINavbarEntry i_Entry)
    {
        int index = i_Entry.sortIndex;

        if (index < 0 || index >= m_RegisteredEntries.Count)
        {
            return false;
        }

        List<UINavbarEntry> entries = m_RegisteredEntries[index];
        return entries.Remove(i_Entry);
    }

    private void InternalRefreshNavbar()
    {
        for (int index = 0; index < m_NavbarItems.Length; ++index)
        {
            UINavbarItem navbarItem = m_NavbarItems[index];
            if (navbarItem != null)
            {
                List<UINavbarEntry> entries = m_RegisteredEntries[index];
                if (entries.Count > 0)
                {
                    // Set this item visible.

                    navbarItem.isVisible = true;

                    // Set activation state.

                    UINavbarEntry firstEntry = entries[0];

                    if (m_Dirty)
                    {
                        Sprite sprite = GetSprite(firstEntry.iconKey);

                        navbarItem.SetIcon(sprite);
                        navbarItem.SetText(firstEntry.text);
                    }

                    navbarItem.isActive = firstEntry.isActive;
                }
                else
                {
                    // Set this item hidden.

                    navbarItem.isVisible = false;
                }
            }
        }

        m_Dirty = false;
    }

    // UTILS

    private Sprite GetSprite(string i_IconId)
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

    // EVENTS

    private void InternalControllerConnected(ControllerEventParams i_EventParams)
    {
        m_Dirty = true;
    }

    private void InternalControllerDisconnected(ControllerEventParams eventParams)
    {
        m_Dirty = true;
    }

    private void InternalFocusChaged(GameObject i_CurrFocus, GameObject i_PrevFocus)
    {
        if (i_PrevFocus != null)
        {
            UINavbarEntry[] navbarEntries = i_PrevFocus.GetComponents<UINavbarEntry>();
            for (int entryIndex = 0; entryIndex < navbarEntries.Length; ++entryIndex)
            {
                UINavbarEntry navbarEntry = navbarEntries[entryIndex];
                InternalUnregister(navbarEntry);
            }
        }

        if (i_CurrFocus != null)
        {
            UINavbarEntry[] navbarEntries = i_CurrFocus.GetComponents<UINavbarEntry>();
            for (int entryIndex = 0; entryIndex < navbarEntries.Length; ++entryIndex)
            {
                UINavbarEntry navbarEntry = navbarEntries[entryIndex];
                InternalRegister(navbarEntry);
            }
        }

        m_Dirty = true;
    }

    private void InternalTriggerAdded(UIEventTrigger i_Trigger)
    {
        UINavbarEntry[] navbarEntries = i_Trigger.GetComponents<UINavbarEntry>();

        for (int entryIndex = 0; entryIndex < navbarEntries.Length; ++entryIndex)
        {
            UINavbarEntry navbarEntry = navbarEntries[entryIndex];
            InternalRegister(navbarEntry);
        }

        m_Dirty = true;
    }

    private void InternalTriggerRemoved(UIEventTrigger i_Trigger)
    {
        UINavbarEntry[] navbarEntries = i_Trigger.GetComponents<UINavbarEntry>();

        for (int entryIndex = 0; entryIndex < navbarEntries.Length; ++entryIndex)
        {
            UINavbarEntry navbarEntry = navbarEntries[entryIndex];
            InternalUnregister(navbarEntry);
        }

        m_Dirty = true;
    }
}
