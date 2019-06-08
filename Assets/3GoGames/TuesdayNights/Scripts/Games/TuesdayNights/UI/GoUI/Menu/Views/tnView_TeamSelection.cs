using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using GoUI;

public class tnView_TeamSelection : GoUI.UIView
{
    [Header("Widgets")]

    [SerializeField]
    private Transform m_FlagsContent = null;
    [SerializeField]
    private tnTeamFlag m_TeamEntryPrefab = null;

    [SerializeField]
    private Text m_Timer = null;

    [Header("Triggers")]

    [SerializeField]
    private UIEventTrigger m_ConfirmTrigger = null;

    [Header("Audio")]

    [SerializeField]
    private SfxDescriptor m_SelectionChangedSfx = null;
    [SerializeField]
    private SfxDescriptor m_ConfirmSfx = null;
    [SerializeField]
    private SfxDescriptor m_CancelSfx = null;

    private List<GameObject> m_Slots = new List<GameObject>();
    private static int s_MaxPlayers = 2;
    private GameObject[] m_Selections = new GameObject[s_MaxPlayers];
    private Color[] m_PlayersColors = new Color[s_MaxPlayers];

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        for (int index = 0; index < s_MaxPlayers; ++index)
        {
            m_PlayersColors[index] = Color.white;
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
        for (int index = 0; index < m_Slots.Count; ++index)
        {
            GameObject slot = m_Slots[index];
            if (slot != null)
            {
                tnTeamFlag team = slot.GetComponent<tnTeamFlag>();
                if (team != null)
                {
                    team.Recycle();
                }
            }
        }

        m_Slots.Clear();

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_Selections[playerIndex] = null;
        }
    }

    public void SetPlayerColor(int i_Index, Color i_Color)
    {
        if (i_Index < 0 || i_Index >= s_MaxPlayers)
            return;

        m_PlayersColors[i_Index] = i_Color;

        GameObject currentSelection = m_Selections[i_Index];
        if (currentSelection != null)
        {
            tnTeamFlag teamFlag = currentSelection.GetComponent<tnTeamFlag>();
            if (teamFlag != null)
            {
                teamFlag.SetColor(i_Color);
            }
        }
    }

    public void AddTeamFlag(tnTeamData i_TeamData)
    {
        if (i_TeamData != null)
        {
            Sprite sprite = i_TeamData.flag;
            string name = i_TeamData.name;

            if (m_TeamEntryPrefab != null && m_FlagsContent != null)
            {
                tnTeamFlag teamFlag = m_TeamEntryPrefab.Spawn<tnTeamFlag>();

                teamFlag.transform.SetParent(m_FlagsContent, false);

                if (sprite != null)
                {
                    teamFlag.SetImage(sprite);
                }

                teamFlag.SetLabel(name);
                teamFlag.SetAvailable();

                m_Slots.Add(teamFlag.gameObject);
            }
        }
    }

    public void Move(int i_TeamIndex, UINavigationDirection i_Direction)
    {
        if (IsValidIndex(i_TeamIndex))
        {
            GameObject selected = m_Selections[i_TeamIndex];
            if (selected != null)
            {
                GameObject nearest = GetNearest(selected, i_Direction);
                if (nearest != null)
                {
                    SfxPlayer.PlayMain(m_SelectionChangedSfx);
                    Select(i_TeamIndex, nearest);
                }
            }
        }
    }

    public void ForceTeamSelection(int i_TeamIndex, int i_SlotIndex)
    {
        Select(i_TeamIndex, i_SlotIndex);
    }

    public void ConfirmTeam(int i_TeamIndex)
    {
        if (IsValidIndex(i_TeamIndex))
        {
            GameObject selection = m_Selections[i_TeamIndex];
            if (selection != null)
            {
                tnTeamFlag teamFlag = selection.GetComponent<tnTeamFlag>();
                if (teamFlag != null)
                {
                    SfxPlayer.PlayMain(m_ConfirmSfx);

                    Color color = m_PlayersColors[i_TeamIndex];
                    teamFlag.SetSelected(color);
                }
            }
        }
    }

    public void CancelConfirmedTeam(int i_TeamIndex)
    {
        if (IsValidIndex(i_TeamIndex))
        {
            GameObject selection = m_Selections[i_TeamIndex];
            if (selection != null)
            {
                tnTeamFlag teamSelection = selection.GetComponent<tnTeamFlag>();
                if (teamSelection != null)
                {
                    SfxPlayer.PlayMain(m_CancelSfx);

                    Color color = m_PlayersColors[i_TeamIndex];
                    teamSelection.SetHighlighted(color);
                }
            }
        }
    }

    public int GetSelectedFlagIndex(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
        {
            return -1;
        }

        GameObject selection = m_Selections[i_TeamIndex];

        if (selection == null)
        {
            return -1;
        }

        return m_Slots.IndexOf(selection);
    }

    public void SetTimer(double i_Time)
    {
        Internal_SetTimer(i_Time);
    }

    public void SetTimer(float i_Time)
    {
        Internal_SetTimer(i_Time);
    }

    // TRIGGERS LOGIC

    public void SetConfirmTriggerCanSend(bool i_CanSend)
    {
        if (m_ConfirmTrigger != null)
        {
            m_ConfirmTrigger.canSend = i_CanSend;
        }
    }

    // INTERNALS

    private void Internal_StartSelectionTeam()
    {
        for (int index = 0; index < m_Slots.Count; ++index)
        {
            GameObject slot = m_Slots[index];
            if (slot != null)
            {
                for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
                {
                    if (m_Selections[playerIndex] == null)
                    {
                        m_Selections[playerIndex] = slot;
                        break;
                    }
                }
            }
        }
    }

    private GameObject GetNearest(GameObject i_Source, UINavigationDirection i_Direction)
    {
        if (i_Source == null)
        {
            return null;
        }

        GameObject nextSelection = null;

        switch (i_Direction)
        {
            case UINavigationDirection.Left:
                nextSelection = GetNearestOnLeft(i_Source);
                break;
            case UINavigationDirection.Right:
                nextSelection = GetNearestOnRight(i_Source);
                break;
            case UINavigationDirection.Up:
                nextSelection = GetNearestOnUp(i_Source);
                break;
            case UINavigationDirection.Down:
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

        while (selectable != null && !IsAvailable(selectable.gameObject))
        {
            selectable = selectable.FindSelectableOnLeft();
        }

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

        while (selectable != null && !IsAvailable(selectable.gameObject))
        {
            selectable = selectable.FindSelectableOnRight();
        }

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

        while (selectable != null && !IsAvailable(selectable.gameObject))
        {
            selectable = selectable.FindSelectableOnUp();
        }

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

        while (selectable != null && !IsAvailable(selectable.gameObject))
        {
            selectable = selectable.FindSelectableOnDown();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private bool IsAvailable(GameObject i_Slot)
    {
        if (i_Slot == null)
        {
            return false;
        }

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            GameObject currentSelection = m_Selections[playerIndex];
            if (i_Slot == currentSelection)
            {
                return false;
            }
        }

        return true;
    }

    private bool IsValidIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_MaxPlayers)
        {
            return false;
        }

        return true;
    }

    private void Select(int i_TeamIndex, GameObject i_Slot)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        if (i_Slot == null)
            return;

        // Release previous selection.

        {
            GameObject currentSelection = m_Selections[i_TeamIndex];

            if (currentSelection != null)
            {
                tnTeamFlag teamFlag = currentSelection.GetComponent<tnTeamFlag>();
                if (teamFlag != null)
                {
                    teamFlag.SetAvailable();
                }

                // Raise event.

                // if (m_OnNavigate != null)
                {
                    //      m_OnNavigate.Invoke();
                }
            }
        }

        m_Selections[i_TeamIndex] = i_Slot;

        // Update new selection.

        {
            tnTeamFlag teamFlag = i_Slot.GetComponent<tnTeamFlag>();
            if (teamFlag != null)
            {
                teamFlag.SetHighlighted(m_PlayersColors[i_TeamIndex]);
            }
        }
    }

    private void Select(int i_TeamIndex, int i_SlotIndex)
    {
        if (i_SlotIndex < 0 || i_SlotIndex >= m_Slots.Count)
        {
            return;
        }

        GameObject slotInstance = m_Slots[i_SlotIndex];
        Select(i_TeamIndex, slotInstance);
    }

    private void Internal_SetTimer(double i_Time)
    {
        if (m_Timer == null)
            return;

        string timerString = TimeUtils.TimeToString(i_Time, true, true);
        m_Timer.text = timerString;
    }

    private void Internal_SetTimer(float i_Time)
    {
        if (m_Timer == null)
            return;

        string timerString = TimeUtils.TimeToString(i_Time, true, true);
        m_Timer.text = timerString;
    }
}