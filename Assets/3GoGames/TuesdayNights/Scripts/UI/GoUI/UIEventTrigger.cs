using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

using System;
using System.Collections.Generic;

using WiFiInput.Server;

namespace GoUI
{
    public class UIEventTrigger : MonoBehaviour
    {
        [Serializable]
        public class UIEvent : UnityEvent { }

        [SerializeField]
        private string m_Action = "";
        [SerializeField]
        private string m_WifiAction = "";

        [SerializeField]
        private List<string> m_PlayerNames = new List<string>();
        [SerializeField]
        private List<string> m_WifiPlayerNames = new List<string>();

        [SerializeField]
        [DisallowEditInPlayMode]
        private bool m_UseInputModule = false;

        [SerializeField]
        private bool m_UseWifi = false;

        [FormerlySerializedAs("onEvent")]
        [SerializeField]
        private UIEvent m_OnEvent = new UIEvent();

        public UIEvent onEvent
        {
            get { return m_OnEvent; }
            set { m_OnEvent = value; }
        }

        private bool m_CanSend = true;

        public bool canSend
        {
            get { return m_CanSend; }
            set { m_CanSend = value; }
        }

        private List<PlayerInput> m_Players = new List<PlayerInput>();
        private List<WiFiPlayerInput> m_WifiPlayers = new List<WiFiPlayerInput>();

        // MonoBehaviour's interface

        void Awake()
        {
            // Get players references.

            {
                for (int playerIndex = 0; playerIndex < m_PlayerNames.Count; ++playerIndex)
                {
                    string playerName = m_PlayerNames[playerIndex];
                    PlayerInput playerInput = InputSystem.GetPlayerByNameMain(playerName);
                    if (playerInput != null)
                    {
                        m_Players.Add(playerInput);
                    }
                }
            }

            if (m_UseWifi)
            {
                // Get wifi players references.

                for (int playerIndex = 0; playerIndex < m_WifiPlayerNames.Count; ++playerIndex)
                {
                    string playerName = m_WifiPlayerNames[playerIndex];
                    WiFiPlayerInput playerInput = WiFiInputSystem.GetPlayerByNameMain(playerName);
                    if (playerInput != null)
                    {
                        m_WifiPlayers.Add(playerInput);
                    }
                }
            }
        }

        void OnEnable()
        {
            if (m_UseInputModule)
            {
                InputModule inputModule = UIEventSystem.inputModuleMain;
                if (inputModule != null)
                {
                    inputModule.playersChangedEvent += OnInputModulePlayersChangedEvent;
                    RefreshPlayers();
                }
            }

            UIEventSystem.RegisterTriggerMain(this);
        }

        void OnDisable()
        {
            UIEventSystem.UnregisterTriggerMain(this);

            if (m_UseInputModule)
            {
                InputModule inputModule = UIEventSystem.inputModuleMain;
                if (inputModule != null)
                {
                    inputModule.playersChangedEvent -= OnInputModulePlayersChangedEvent;
                }
            }
        }

        // BUSINESS LOGIC

        public void UpdateTrigger()
        {
            if (!m_CanSend)
                return;

            // Update players action.

            {
                for (int playerInputIndex = 0; playerInputIndex < m_Players.Count; ++playerInputIndex)
                {
                    PlayerInput playerInput = m_Players[playerInputIndex];
                    if (playerInput != null)
                    {
                        if (playerInput.GetButtonDown(m_Action))
                        {
                            m_OnEvent.Invoke();
                        }
                    }
                }
            }

            if (m_UseWifi)
            {
                // Update wifi players action.

                for (int playerInputIndex = 0; playerInputIndex < m_WifiPlayers.Count; ++playerInputIndex)
                {
                    WiFiPlayerInput playerInput = m_WifiPlayers[playerInputIndex];
                    if (playerInput != null)
                    {
                        if (playerInput.GetButtonDown(m_WifiAction))
                        {
                            m_OnEvent.Invoke();
                        }
                    }
                }
            }
        }

        public void Invoke()
        {
            if (!m_CanSend)
                return;

            m_OnEvent.Invoke();
        }

        // INTERNALS

        private void RefreshPlayers()
        {
            m_Players.Clear();
            m_WifiPlayers.Clear();

            InputModule inputModule = UIEventSystem.inputModuleMain;

            if (inputModule == null)
                return;

            for (int playerIndex = 0; playerIndex < inputModule.playersCount; ++playerIndex)
            {
                PlayerInput playerInput = inputModule.GetPlayerInput(playerIndex);
                m_Players.Add(playerInput);
            }

            if (m_UseWifi)
            {
                for (int playerIndex = 0; playerIndex < inputModule.wifiPlayersCount; ++playerIndex)
                {
                    WiFiPlayerInput playerInput = inputModule.GetWifiPlayerInput(playerIndex);
                    m_WifiPlayers.Add(playerInput);
                }
            }
        }

        // EVENTS

        private void OnInputModulePlayersChangedEvent()
        {
            RefreshPlayers();
        }
    }
}
