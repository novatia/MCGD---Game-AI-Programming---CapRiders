using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

namespace GoUI
{
    public class UIPanelsManager : MonoBehaviour
    {
        private Dictionary<UIGroup, List<UIBasePanel>> m_OpenedPanels = new Dictionary<UIGroup, List<UIBasePanel>>();
        private Dictionary<UIGroup, Action> m_Callbacks = new Dictionary<UIGroup, Action>();
        private Dictionary<UIGroup, Coroutine> m_Coroutines = new Dictionary<UIGroup, Coroutine>();

        // LOGIC

        public void Initialize()
        {
            for (int groupIndex = 0; groupIndex < (int)UIGroup.GroupsCount; ++groupIndex)
            {
                UIGroup group = (UIGroup)groupIndex;
                List<UIBasePanel> panels = new List<UIBasePanel>();
                m_OpenedPanels.Add(group, panels);
            }
        }

        public void SwitchPanels(UIGroup i_Group, UIBasePanel i_Panel)
        {
            UIBasePanel[] panels = new UIBasePanel[] { i_Panel };
            SwitchPanels(i_Group, panels);
        }

        public void SwitchPanels(UIGroup i_Group, UIBasePanel i_First, UIBasePanel i_Second)
        {
            UIBasePanel[] panels = new UIBasePanel[] { i_First, i_Second };
            SwitchPanels(i_Group, panels);
        }

        public void SwitchPanels(UIGroup i_Group, UIBasePanel[] i_Panels)
        {
            if (i_Panels == null)
                return;

            List<UIBasePanel> openedPanels = m_OpenedPanels[i_Group];

            if (openedPanels == null)
                return;

            // Close panels

            for (int panelIndex = 0; panelIndex < openedPanels.Count; ++panelIndex)
            {
                UIBasePanel panel = openedPanels[panelIndex];

                if (panel == null)
                    continue;

                bool shouldBeClosed = true;
                for (int index = 0; index < i_Panels.Length; ++index)
                {
                    UIBasePanel current = i_Panels[index];
                    if (current == panel)
                    {
                        shouldBeClosed = false;
                        break;
                    }
                }

                if (shouldBeClosed)
                {
                    UI.ClosePanel(panel);
                }
            }

            // Remove closed panels

            for (int panelIndex = 0; panelIndex < openedPanels.Count;)
            {
                UIBasePanel panel = openedPanels[panelIndex];

                if (panel == null)
                    continue;

                if (!panel.isOpen)
                {
                    openedPanels.Remove(panel);
                    panelIndex = 0;
                }
                else
                {
                    ++panelIndex;
                }
            }

            // Open new panels.

            for (int panelIndex = 0; panelIndex < i_Panels.Length; ++panelIndex)
            {
                UIBasePanel panel = i_Panels[panelIndex];

                if (panel == null)
                    continue;

                if (!openedPanels.Contains(panel))
                {
                    UI.OpenPanel(panel);
                    openedPanels.Add(panel);
                }
            }
        }

        public void SequentialSwitchPanels(UIGroup i_Group, UIBasePanel i_Panel, Action i_Callback = null)
        {
            UIBasePanel[] panels = new UIBasePanel[] { i_Panel };
            SequentialSwitchPanels(i_Group, panels, i_Callback);
        }

        public void SequentialSwitchPanels(UIGroup i_Group, UIBasePanel i_First, UIBasePanel i_Second, Action i_Callback = null)
        {
            UIBasePanel[] panels = new UIBasePanel[] { i_First, i_Second};
            SequentialSwitchPanels(i_Group, panels, i_Callback);
        }

        public void SequentialSwitchPanels(UIGroup i_Group, UIBasePanel[] i_Panels, Action i_Callback = null)
        {
            Coroutine currentCoroutine;
            m_Coroutines.TryGetValue(i_Group, out currentCoroutine);
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                m_Coroutines.Remove(i_Group);
            }

            m_Callbacks.Remove(i_Group);
            if (i_Callback != null)
            {
                m_Callbacks.Add(i_Group, i_Callback);
            }

            IEnumerator switchRoutine = Switch(i_Group, i_Panels);
            Coroutine coroutine = StartCoroutine(switchRoutine);
            m_Coroutines.Add(i_Group, coroutine);
        }

        public void ClearGroup(UIGroup i_Group)
        {
            List<UIBasePanel> openedPanels = m_OpenedPanels[i_Group];

            if (openedPanels == null)
                return;

            for (int panelIndex = 0; panelIndex < openedPanels.Count; ++panelIndex)
            {
                UIBasePanel panel = openedPanels[panelIndex];

                if (panel == null)
                    continue;

                UI.ClosePanel(panel);
            }

            openedPanels.Clear();
        }

        public void ClearAll()
        {
            for (int groupIndex = 0; groupIndex < (int)UIGroup.GroupsCount; ++groupIndex)
            {
                UIGroup group = (UIGroup)groupIndex;

                ClearGroup(group);
            }
        }

        // INTERNALS

        private IEnumerator Switch(UIGroup i_Group, UIBasePanel[] i_Panels)
        {
            if (i_Panels != null)
            {
                List<UIBasePanel> openedPanels = m_OpenedPanels[i_Group];
                if (openedPanels != null)
                {
                    // Close panels

                    int closingPanelsCount = 0;
                    int closedPanelsCount = 0;
                    Action panelClosedCallback = () => { ++closedPanelsCount; };

                    for (int panelIndex = 0; panelIndex < openedPanels.Count; ++panelIndex)
                    {
                        UIBasePanel panel = openedPanels[panelIndex];

                        if (panel == null)
                            continue;

                        bool shouldBeClosed = true;
                        for (int index = 0; index < i_Panels.Length; ++index)
                        {
                            UIBasePanel current = i_Panels[index];
                            if (current == panel)
                            {
                                shouldBeClosed = false;
                                break;
                            }
                        }

                        if (shouldBeClosed)
                        {
                            ++closingPanelsCount;   
                            UI.ClosePanel(panel, panelClosedCallback);
                        }
                    }

                    // Remove closed panels

                    for (int panelIndex = 0; panelIndex < openedPanels.Count;)
                    {
                        UIBasePanel panel = openedPanels[panelIndex];

                        if (panel == null)
                            continue;

                        if (!panel.isOpen)
                        {
                            openedPanels.Remove(panel);
                            panelIndex = 0;
                        }
                        else
                        {
                            ++panelIndex;
                        }
                    }

                    // Wait animations.

                    yield return new WaitUntil(() => (closedPanelsCount == closingPanelsCount));

                    // Open new panels.

                    int openingPanelsCount = 0;
                    int openedPanelsCount = 0;
                    Action panelOpenedCallback = () => { ++openedPanelsCount; };

                    for (int panelIndex = 0; panelIndex < i_Panels.Length; ++panelIndex)
                    {
                        UIBasePanel panel = i_Panels[panelIndex];

                        if (panel == null)
                            continue;

                        if (!openedPanels.Contains(panel))
                        {
                            ++openingPanelsCount;
                            UI.OpenPanel(panel, panelOpenedCallback);
                            openedPanels.Add(panel);
                        }
                    }

                    // Wait animations.

                    yield return new WaitUntil(() => (openedPanelsCount == openingPanelsCount));
                }
            }

            Callback(i_Group);
        }

        private void Callback(UIGroup i_Group)
        {
            Action callback;
            if (m_Callbacks.TryGetValue(i_Group, out callback))
            {
                if (callback != null)
                {
                    callback();
                }

                m_Callbacks.Remove(i_Group);
            }
        }
    }
}