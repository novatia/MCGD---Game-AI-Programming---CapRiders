using UnityEngine;
using UnityEngine.EventSystems;

using WiFiInput.Server;

using GoUI;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Configure InputModule.")]
    public class ConfigureInputModule : FsmStateAction
    {
        public FsmBool useAllPlayers;
        public FsmBool useAllWifiPlayers;

        public FsmString[] players;
        public FsmString[] wifiPlayers;

        public override void Reset()
        {
            useAllPlayers = new FsmBool { Value = false };
            useAllWifiPlayers = new FsmBool { Value = false };

            players = null;
            wifiPlayers = null;
        }

        public override void OnEnter()
        {
            InputModule inputModule = UIEventSystem.inputModuleMain;
            if (inputModule != null)
            {
                // Clear input module.

                inputModule.Clear();

                // Handle players.

                if (useAllPlayers != null && useAllPlayers.Value)
                {
                    for (int playerIndex = 0; playerIndex < InputSystem.numPlayersMain; ++playerIndex)
                    {
                        PlayerInput playerInput = InputSystem.GetPlayerByIndexMain(playerIndex);
                        inputModule.AddPlayer(playerInput);
                    }
                }
                else
                {
                    if (players != null)
                    {
                        for (int playerIndex = 0; playerIndex < players.Length; ++playerIndex)
                        {
                            FsmString str = players[playerIndex];
                            if (!str.IsNone && str.Value != "")
                            {
                                inputModule.AddPlayer(str.Value);
                            }
                        }
                    }
                }

                // Handle wifi players.

                if (useAllWifiPlayers != null && useAllWifiPlayers.Value)
                {
                    for (int playerIndex = 0; playerIndex < WiFiInputSystem.playersCountMain; ++playerIndex)
                    {
                        WiFiPlayerInput playerInput = WiFiInputSystem.GetPlayerByIndexMain(playerIndex);
                        inputModule.AddWifiPlayer(playerInput);
                    }
                }
                else
                {
                    if (wifiPlayers != null)
                    {
                        for (int playerIndex = 0; playerIndex < wifiPlayers.Length; ++playerIndex)
                        {
                            FsmString str = wifiPlayers[playerIndex];
                            if (!str.IsNone && str.Value != "")
                            {
                                inputModule.AddWifiPlayer(str.Value);
                            }
                        }
                    }
                }
            }

            Finish();
        }
    }
}