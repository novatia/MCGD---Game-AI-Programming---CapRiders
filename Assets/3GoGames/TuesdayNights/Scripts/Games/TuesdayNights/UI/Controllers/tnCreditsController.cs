using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System;
using System.Collections;
using System.Collections.Generic;

using WiFiInput.Server;

using GoUI;

public class tnCreditsController : MonoBehaviour
{
    private enum MoveDir
    {
        None,
        Left,
        Right,
        Up,
        Down,
    }

    private static string s_PlayerInput_Left = "HorizontalLeft";
    private static string s_PlayerInput_Right = "HorizontalRight";
    private static string s_PlayerInput_Up = "VerticalUp";
    private static string s_PlayerInput_Down = "VerticalDown";

    private static string s_WiFiPlayerInput_Horizontal = "Horizontal";
    private static string s_WiFiPlayerInput_Vertical = "Vertical";

    [SerializeField]
    private tnUICreditsEntry m_Prefab = null;
    [SerializeField]
    private tnCreditsPortrait m_Portrait = null;

    [SerializeField]
    private RectTransform[] m_Anchors = null;

    [Serializable]
    public class NavigationEvent : UnityEvent { }

    [SerializeField]
    private NavigationEvent m_OnNavigate = new NavigationEvent();
    public NavigationEvent onNavigate { get { return m_OnNavigate; } set { m_OnNavigate = value; } }

    private List<tnUICreditsEntry> m_Entries = new List<tnUICreditsEntry>();

    private List<PlayerInput> m_Players = new List<PlayerInput>();
    private List<WiFiPlayerInput> m_WiFiPlayers = new List<WiFiPlayerInput>();

    private GameObject m_CurrentSelection = null;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        SetupPage();
    }

    void OnEnable()
    {
        // Update players list from InputModule.

        {
            m_Players.Clear();

            InputModule inputModule = UIEventSystem.inputModuleMain;

            if (inputModule != null)
            {
                for (int playerIndex = 0; playerIndex < inputModule.playersCount; ++playerIndex)
                {
                    PlayerInput playerInput = inputModule.GetPlayerInput(playerIndex);
                    m_Players.Add(playerInput);
                }
            }
        }

        // Update wifi players list from InputModule.

        {
            m_WiFiPlayers.Clear();

            InputModule inputModule = UIEventSystem.inputModuleMain;

            if (inputModule != null)
            {
                for (int playerIndex = 0; playerIndex < inputModule.wifiPlayersCount; ++playerIndex)
                {
                    WiFiPlayerInput playerInput = inputModule.GetWifiPlayerInput(playerIndex);
                    m_WiFiPlayers.Add(playerInput);
                }
            }
        }

        // Clear slots.

        {
            for (int entryIndex = 0; entryIndex < m_Entries.Count; ++entryIndex)
            {
                tnUICreditsEntry entry = m_Entries[entryIndex];
                entry.SetHighlighted(false);
            }
        }

        // Clear portrait.

        {
            if (m_Portrait != null)
            {
                m_Portrait.Clear();
            }
        }

        // Find first available entry.

        GameObject firstEntry = null;

        {
            for (int entryIndex = 0; entryIndex < m_Entries.Count; ++entryIndex)
            {
                tnUICreditsEntry creditsEntry = m_Entries[entryIndex];

                if (creditsEntry != null)
                {
                    firstEntry = creditsEntry.gameObject;
                }
            }
        }

        // Update selection.

        {
            Select(firstEntry);
        }
    }

    void OnDisable()
    {

    }

    void Update()
    {
        // Update selection from players.

        {
            for (int playerIndex = 0; playerIndex < m_Players.Count; ++playerIndex)
            {
                PlayerInput playerInput = m_Players[playerIndex];

                if (playerInput == null)
                    continue;

                MoveDir moveDir = GetMoveDirection(playerInput);
                UpdateSelection(moveDir);
            }
        }

        // Update selection from wifi players.

        {
            for (int wifiPlayerIndex = 0; wifiPlayerIndex < m_WiFiPlayers.Count; ++wifiPlayerIndex)
            {
                WiFiPlayerInput wifiPlayerInput = m_WiFiPlayers[wifiPlayerIndex];

                if (wifiPlayerInput == null)
                    continue;

                MoveDir moveDir = GetMoveDirection(wifiPlayerInput);
                UpdateSelection(moveDir);
            }
        }
    }

    // INTERNALS

    private void SetupPage()
    {
        if (m_Prefab == null)
            return;

        for (int anchorIndex = 0; anchorIndex < m_Anchors.Length; ++anchorIndex)
        {
            RectTransform anchor = m_Anchors[anchorIndex];

            if (anchor == null)
                continue;

            // Try to get a credits data entry.

            tnCreditsData creditsData = tnGameData.GetCreditsDataMain(anchorIndex);

            if (creditsData == null)
                continue;

            // Create credits entry.

            tnUICreditsEntry creditsEntry = Instantiate<tnUICreditsEntry>(m_Prefab);
            creditsEntry.transform.SetParent(anchor, false);

            // Configure entry.

            creditsEntry.SetIndex(anchorIndex);

            creditsEntry.SetBaseImage(creditsData.baseSprite);
            creditsEntry.SetCharacterAnimator(creditsData.animatorController);
            creditsEntry.SetCharacterName(creditsData.nickname);
            creditsEntry.SetRole(creditsData.role);

            creditsEntry.SetHighlighted(false);

            m_Entries.Add(creditsEntry);
        }
    }

    private void UpdateSelection(MoveDir i_MoveDir)
    {
        if (i_MoveDir == MoveDir.None)
            return;

        GameObject newSelection = GetNearest(m_CurrentSelection, i_MoveDir);
        Select(newSelection);
    }

    private void Select(GameObject i_NewSelection)
    {
        if (i_NewSelection == null)
            return;

        // Release previous selection.

        {
            if (m_CurrentSelection != null)
            {
                tnUICreditsEntry creditsEntry = m_CurrentSelection.GetComponent<tnUICreditsEntry>();

                if (creditsEntry != null)
                {
                    creditsEntry.SetHighlighted(false);

                    // Play SFX.

                    if (m_OnNavigate != null)
                    {
                        m_OnNavigate.Invoke();
                    }
                }
            }
        }

        // Update new selection.

        {
            m_CurrentSelection = i_NewSelection;

            {
                tnUICreditsEntry creditsEntry = i_NewSelection.GetComponent<tnUICreditsEntry>();

                if (creditsEntry != null)
                {
                    creditsEntry.SetHighlighted(true);

                    // Refresh portrait.

                    int entryIndex = creditsEntry.index;
                    UpdatePortrait(entryIndex);
                }
            }
        }
    }

    private void UpdatePortrait(int i_Index)
    {
        if (m_Portrait == null)
            return;

        tnCreditsData creditsData = tnGameData.GetCreditsDataMain(i_Index);

        if (creditsData == null)
            return;

        m_Portrait.SetCharacterSprite(creditsData.characterSprite);

        string name = "";
        name += creditsData.firstName;
        name += " ";
        name += creditsData.lastName;

        m_Portrait.SetName(name);

        m_Portrait.SetRole(creditsData.role);
    }

    private void ClearPortrait()
    {
        if (m_Portrait != null)
        {
            m_Portrait.Clear();
        }
    }

    // UTILS

    private MoveDir GetMoveDirection(PlayerInput i_PlayerInput)
    {
        if (i_PlayerInput == null)
        {
            return MoveDir.None;
        }
        else if (i_PlayerInput.GetButtonDown(s_PlayerInput_Left))
        {
            return MoveDir.Left;
        }
        else if (i_PlayerInput.GetButtonDown(s_PlayerInput_Right))
        {
            return MoveDir.Right;
        }
        else if (i_PlayerInput.GetButtonDown(s_PlayerInput_Up))
        {
            return MoveDir.Up;
        }
        else if (i_PlayerInput.GetButtonDown(s_PlayerInput_Down))
        {
            return MoveDir.Down;
        }

        return MoveDir.None; // Something went wrong.
    }

    private MoveDir GetMoveDirection(WiFiPlayerInput i_PlayerInput)
    {
        if (i_PlayerInput == null)
        {
            return MoveDir.None;
        }
        else if (i_PlayerInput.GetNegativeButtonDown(s_WiFiPlayerInput_Horizontal))
        {
            return MoveDir.Left;
        }
        else if (i_PlayerInput.GetPositiveButtonDown(s_WiFiPlayerInput_Horizontal))
        {
            return MoveDir.Right;
        }
        else if (i_PlayerInput.GetPositiveButtonDown(s_WiFiPlayerInput_Vertical))
        {
            return MoveDir.Up;
        }
        else if (i_PlayerInput.GetNegativeButtonDown(s_WiFiPlayerInput_Vertical))
        {
            return MoveDir.Down;
        }

        return MoveDir.None; // Something went wrong.
    }

    private GameObject GetNearest(GameObject i_Source, MoveDir i_Direction)
    {
        if (i_Source == null)
        {
            return null;
        }

        GameObject nextSelection = null;

        switch (i_Direction)
        {
            case MoveDir.Left:
                nextSelection = GetNearestOnLeft(i_Source);
                break;
            case MoveDir.Right:
                nextSelection = GetNearestOnRight(i_Source);
                break;
            case MoveDir.Up:
                nextSelection = GetNearestOnUp(i_Source);
                break;
            case MoveDir.Down:
                nextSelection = GetNearestOnDown(i_Source);
                break;
        }

        return nextSelection;
    }

    private GameObject GetNearestOnLeft(GameObject i_Source)
    {
        if (i_Source == null)
        {
            return null;
        }

        Selectable sourceSelectable = i_Source.GetComponent<Selectable>();

        if (sourceSelectable == null)
        {
            return null;
        }

        Selectable selectable = sourceSelectable.FindSelectableOnLeft();

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnRight(GameObject i_Source)
    {
        if (i_Source == null)
        {
            return null;
        }

        Selectable sourceSelectable = i_Source.GetComponent<Selectable>();

        if (sourceSelectable == null)
        {
            return null;
        }

        Selectable selectable = sourceSelectable.FindSelectableOnRight();

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnUp(GameObject i_Source)
    {
        if (i_Source == null)
        {
            return null;
        }

        Selectable sourceSelectable = i_Source.GetComponent<Selectable>();

        if (sourceSelectable == null)
        {
            return null;
        }

        Selectable selectable = sourceSelectable.FindSelectableOnUp();

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnDown(GameObject i_Source)
    {
        if (i_Source == null)
        {
            return null;
        }

        Selectable sourceSelectable = i_Source.GetComponent<Selectable>();

        if (sourceSelectable == null)
        {
            return null;
        }

        Selectable selectable = sourceSelectable.FindSelectableOnDown();

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }
}
