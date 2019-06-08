using UnityEngine;

using System.Collections.Generic;

using PlayerInputEvents;

namespace GoUI
{
    public class UIPanel_Navbar : UIPanel<UIView_Navbar>
    {
        // STATIC

        private static int s_Max_SortIndex = 10;

        // Fields

        private bool m_Dirty = false;
        private List<List<UINavbarCommand>> m_RegisteredEntries = new List<List<UINavbarCommand>>();

        // MonoBehaviour's interface

        protected override void Awake()
        {
            base.Awake();

            // Create entries list.

            for (int index = 0; index < s_Max_SortIndex; ++index)
            {
                List<UINavbarCommand> commands = new List<UINavbarCommand>();
                m_RegisteredEntries.Add(commands);
            }
        }

        // UIPanel's interface

        protected override void OnEnter()
        {
            base.OnEnter();

            Internal_SetupEntries();
            Internal_RegisterOnEvents();

            Internal_RefreshView();
        }

        protected override void OnUpdate(float i_DeltaTime)
        {
            base.OnUpdate(i_DeltaTime);

            if (m_Dirty)
            {
                Internal_RefreshView();
                m_Dirty = false;
            }
        }

        protected override void OnExit()
        {
            base.OnExit();

            Internal_UnregisterFromEvents();
            Internal_ClearEntries();
            Internal_RefreshView();
        }

        // INTERNALS

        private void Internal_ClearEntries()
        {
            for (int index = 0; index < m_RegisteredEntries.Count; ++index)
            {
                List<UINavbarCommand> commands = m_RegisteredEntries[index];

                if (commands == null)
                    continue;

                commands.Clear();
            }
        }

        private void Internal_SetupEntries()
        {
            Internal_ClearEntries();

            int triggersCount = UIEventSystem.triggersCountMain;
            for (int index = 0; index < triggersCount; ++index)
            {
                UIEventTrigger trigger = UIEventSystem.GetUIEventTriggerMain(index);

                if (trigger == null)
                    continue;

                OnTriggerAdded(trigger);
            }

            GameObject currentFocus = UIEventSystem.focusMain;
            OnFocusChanged(null, currentFocus);
        }

        private void Internal_RegisterOnEvents()
        {
            InputSystem.onControllerConnectedEventMain += OnControllerConnected;
            InputSystem.onControllerDisconnectedEventMain += OnControllerDisconnected;

            UIEventSystem.onFocusChangedMain += OnFocusChanged;

            UIEventSystem.onTriggerAddedMain += OnTriggerAdded;
            UIEventSystem.onTriggerRemovedMain += OnTriggerRemoved;
        }

        private void Internal_UnregisterFromEvents()
        {
            UIEventSystem.onFocusChangedMain -= OnFocusChanged;

            UIEventSystem.onTriggerAddedMain -= OnTriggerAdded;
            UIEventSystem.onTriggerRemovedMain -= OnTriggerRemoved;
        }

        private bool Internal_RegisterCommand(UINavbarCommand i_Command)
        {
            if (i_Command == null)
            {
                return false;
            }

            int index = i_Command.sortIndex;

            if (index < 0 || index >= m_RegisteredEntries.Count)
            {
                return false;
            }

            i_Command.onCommandStateChanged += OnCommandStateChanged;

            List<UINavbarCommand> entries = m_RegisteredEntries[index];
            entries.Add(i_Command);

            return true;
        }

        private bool Internal_UnregisterCommand(UINavbarCommand i_Command)
        {
            if (i_Command == null)
            {
                return false;
            }

            int index = i_Command.sortIndex;

            if (index < 0 || index >= m_RegisteredEntries.Count)
            {
                return false;
            }

            i_Command.onCommandStateChanged -= OnCommandStateChanged;

            List<UINavbarCommand> entries = m_RegisteredEntries[index];
            return entries.Remove(i_Command);
        }

        private void Internal_RefreshView()
        {
            if (viewInstance == null)
                return;

            // Clear view.

            viewInstance.ClearAll();
            
            // Set new data.

            for (int index = 0; index < m_RegisteredEntries.Count; ++index)
            {
                List<UINavbarCommand> commands = m_RegisteredEntries[index];

                if (commands == null)
                    continue;

                UINavbarCommand targetCommand = null;

                // Find first not null command.

                for (int commandIndex = 0; commandIndex < commands.Count; ++commandIndex)
                {
                    UINavbarCommand current = commands[commandIndex];

                    if (current != null)
                    {
                        targetCommand = current;
                        break;
                    }
                }

                // Set command.

                viewInstance.SetElement(index, targetCommand);
            }
        }

        // EVENTS

        private void OnControllerConnected(ControllerEventParams i_EventParams)
        {
            m_Dirty = true;
        }

        private void OnControllerDisconnected(ControllerEventParams i_EventParams)
        {
            m_Dirty = true;
        }

        private void OnFocusChanged(GameObject i_PrevFocus, GameObject i_CurrFocus)
        {
            if (i_PrevFocus != null)
            {
                UINavbarCommand[] commands = i_PrevFocus.GetComponents<UINavbarCommand>();
                for (int index = 0; index < commands.Length; ++index)
                {
                    UINavbarCommand command = commands[index];

                    if (command == null)
                        continue;

                    Internal_UnregisterCommand(command);
                }
            }

            if (i_CurrFocus != null)
            {
                UINavbarCommand[] commands = i_CurrFocus.GetComponents<UINavbarCommand>();
                for (int index = 0; index < commands.Length; ++index)
                {
                    UINavbarCommand command = commands[index];

                    if (command == null)
                        continue;

                    Internal_RegisterCommand(command);
                }
            }

            // Set as dirty.

            m_Dirty = true;
        }

        private void OnTriggerAdded(UIEventTrigger i_Trigger)
        {
            if (i_Trigger == null)
                return;

            UINavbarCommand[] commands = i_Trigger.GetComponents<UINavbarCommand>();
            for (int index = 0; index < commands.Length; ++index)
            {
                UINavbarCommand command = commands[index];

                if (command == null)
                    continue;

                Internal_RegisterCommand(command);
            }

            // Set as dirty.

            m_Dirty = true;
        }

        private void OnTriggerRemoved(UIEventTrigger i_Trigger)
        {
            if (i_Trigger == null)
                return;

            UINavbarCommand[] commands = i_Trigger.GetComponents<UINavbarCommand>();
            for (int index = 0; index < commands.Length; ++index)
            {
                UINavbarCommand command = commands[index];

                if (command == null)
                    continue;

                Internal_UnregisterCommand(command);
            }

            // Set as dirty.

            m_Dirty = true;
        }

        private void OnCommandStateChanged(UINavbarCommand i_Command)
        {
            if (i_Command == null)
                return;

            m_Dirty = true;
        }
    }
}