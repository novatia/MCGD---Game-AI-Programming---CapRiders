using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using GoUI;

using CharacterList = System.Collections.Generic.List<tnUICharacter>;
using SlotList = System.Collections.Generic.List<tnUICharacterSlot>;
using SelectionCache = System.Collections.Generic.List<UnityEngine.GameObject>;

public class tnView_CharacterSelection : GoUI.UIView
{
    private static int s_MaxPlayers = 2;
    private static int s_TeamSize = 11;

    [Header("Prefabs")]

    [SerializeField]
    private tnUICharacter m_CharacterPrefab = null;
    [SerializeField]
    private tnUICharacterSlot m_CharacterSlotPrefab = null;

    [Header("Widgets")]

    [SerializeField]
    private RectTransform m_CharactersRoot = null;
    [SerializeField]
    private RectTransform m_SlotsRoot = null;

    [SerializeField]
    private tnUITeamInfo[] m_TeamInfo = new tnUITeamInfo[s_MaxPlayers];
    [SerializeField]
    private tnUITeamAnchors[] m_TeamAnchorsSets = new tnUITeamAnchors[s_MaxPlayers];
    [SerializeField]
    private tnUIBench[] m_TeamAnchorsBench = new tnUIBench[s_MaxPlayers];
    [SerializeField]
    private tnCharacterPortrait[] m_Portraits = new tnCharacterPortrait[s_MaxPlayers];
    [SerializeField]
    private FacingDir[] m_Facing = new FacingDir[s_MaxPlayers];
    [SerializeField]
    private GameObject[] m_Overlays = new GameObject[s_MaxPlayers];

    [SerializeField]
    private Text m_Timer = null;

    [Header("Triggers")]

    [SerializeField]
    private UIEventTrigger m_ProceedEventTrigger = null;
    [SerializeField]
    private UIEventTrigger m_BackEventTrigger = null;

    [Header("Audio")]

    [SerializeField]
    private SfxDescriptor m_MoveSfx = null;
    [SerializeField]
    private SfxDescriptor m_ConfirmSfx = null;
    [SerializeField]
    private SfxDescriptor m_CancelSfx = null;
    [SerializeField]
    private SfxDescriptor m_ReadySfx = null;

    private SelectionCache[] m_SelectionsCache = new SelectionCache[s_MaxPlayers];
    private bool[] m_Confirmations = new bool[s_MaxPlayers];

    private Matrix<Color> m_PlayersColors = new Matrix<Color>(s_MaxPlayers, s_TeamSize);
    private Matrix<bool> m_Humans = new Matrix<bool>(s_MaxPlayers, s_TeamSize); 
    private Color[] m_TeamsColors = new Color[s_MaxPlayers];
    private Color[] m_CaptainsColors = new Color[s_MaxPlayers];

    private GameObject[] m_CurrentSlots = new GameObject[s_MaxPlayers];
    private tnUIAnchorsSet[] m_TeamAnchors = new tnUIAnchorsSet[s_MaxPlayers];

    private SlotList[] m_LineUp = new SlotList[s_MaxPlayers];
    private SlotList[] m_Bench = new SlotList[s_MaxPlayers];

    private CharacterList m_CharactersPool = new CharacterList();
    private SlotList m_CharactersSlotsPool = new SlotList();

    private enum FacingDir
    {
        FacingRight,
        FacingLeft,
    }

    // MonoBehaviour's interface

    protected override void Awake()
    {
        base.Awake();

        for (int playerIndex = 0; playerIndex < s_MaxPlayers; ++playerIndex)
        {
            m_LineUp[playerIndex] = new SlotList();
            m_Bench[playerIndex] = new SlotList();

            m_SelectionsCache[playerIndex] = new SelectionCache();
        }

        InitializePool();

        ClearAll();
    }

    // UIView's interface

    protected override void OnEnter()
    {
        base.OnEnter();
        ClearAll();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);

        for (int index = 0; index < s_MaxPlayers; ++index)
        {
            UpdatePortrait(index);
        }
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // LOGIC

    public void ClearAll()
    {
        Internal_ClearAll();
    }

    public void SetupTeam(int i_TeamIndex, tnTeamDescription i_TeamDescription)
    {
        Internal_SetupTeam(i_TeamIndex, i_TeamDescription);
    }

    public void Move(int i_TeamIndex, UINavigationDirection i_Direction)
    {
        if (IsValidIndex(i_TeamIndex))
        {
            GameObject selected = m_CurrentSlots[i_TeamIndex];
            if (selected != null)
            {
                GameObject nearest = GetNearest(selected, i_Direction, i_TeamIndex);
                if (nearest != null)
                {
                    Select(i_TeamIndex, nearest);

                    SfxPlayer.PlayMain(m_MoveSfx);
                }
            }
        }
    }

    public void Confirm(int i_TeamIndex)
    {
        if (IsReady(i_TeamIndex))
            return;

        if (IsValidIndex(i_TeamIndex))
        {
            GameObject current = m_CurrentSlots[i_TeamIndex];
            if (current != null)
            {
                SelectionCache cache = m_SelectionsCache[i_TeamIndex];

                GameObject currentSelection = m_CurrentSlots[i_TeamIndex];

                if (currentSelection != null)
                {
                    if (cache.IsEmpty())
                    {
                        cache.Add(currentSelection);

                        tnUICharacterSlot slot = currentSelection.GetComponent<tnUICharacterSlot>();
                        if (slot != null)
                        {
                            slot.Select();
                        }
                    }
                    else
                    {
                        cache.Add(currentSelection);

                        GameObject first = cache[0];
                        GameObject second = cache[1];

                        if (first == null || second == null)
                            return;

                        tnUICharacterSlot firstSlot = first.GetComponent<tnUICharacterSlot>();
                        tnUICharacterSlot secondSlot = second.GetComponent<tnUICharacterSlot>();

                        if (firstSlot == null || secondSlot == null)
                            return;

                        if (cache.Count == 2)
                        {
                            Swap(firstSlot, secondSlot);

                            firstSlot.Deselect();
                            secondSlot.Deselect();

                            cache.Clear();
                        }
                    }

                    SfxPlayer.PlayMain(m_ConfirmSfx);
                }
            }
        }
    }

    public void Cancel(int i_TeamIndex)
    {
        if (IsValidIndex(i_TeamIndex))
        {
            if (IsReady(i_TeamIndex))
            {
                m_Confirmations[i_TeamIndex] = false;
                UpdateOverlay(i_TeamIndex);

                SfxPlayer.PlayMain(m_CancelSfx);
            }
            else
            {
                SelectionCache cache = m_SelectionsCache[i_TeamIndex];

                if (!cache.IsEmpty())
                {
                    GameObject lastSelection = cache[0];
                    if (lastSelection != null)
                    {
                        tnUICharacterSlot slot = lastSelection.GetComponent<tnUICharacterSlot>();
                        if (slot != null)
                        {
                            slot.Deselect();

                            SfxPlayer.PlayMain(m_CancelSfx);
                        }
                    }

                    cache.Clear();
                }
            }
        }
    }

    public void Ready(int i_TeamIndex)
    {
        if (IsValidIndex(i_TeamIndex))
        {
            if (!IsReady(i_TeamIndex))
            {
                // Cancel current selection, if any.

                SelectionCache cache = m_SelectionsCache[i_TeamIndex];

                if (!cache.IsEmpty())
                {
                    GameObject lastSelection = cache[0];
                    if (lastSelection != null)
                    {
                        tnUICharacterSlot slot = lastSelection.GetComponent<tnUICharacterSlot>();
                        if (slot != null)
                        {
                            slot.Deselect();
                        }
                    }

                    cache.Clear();
                }

                m_Confirmations[i_TeamIndex] = true;
                UpdateOverlay(i_TeamIndex);

                SfxPlayer.PlayMain(m_ReadySfx);
            }
        }
    }

    public bool GetTeamReady(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
        {
            return false;
        }

        return m_Confirmations[i_TeamIndex];
    }

    public bool HasCharacterSelected(int i_TeamIndex)
    {
        if (IsValidIndex(i_TeamIndex))
        {
            if (!IsReady(i_TeamIndex))
            {
                SelectionCache cache = m_SelectionsCache[i_TeamIndex];
                if (!cache.IsEmpty())
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void ForceSelection(int i_TeamIndex, int i_SlotIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        Select(i_TeamIndex, i_SlotIndex);
    }

    public void SetTeamColor(int i_TeamIndex, Color i_Color)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        m_TeamsColors[i_TeamIndex] = i_Color;
    }

    public void SetCaptainColor(int i_TeamIndex, Color i_Color)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        m_CaptainsColors[i_TeamIndex] = i_Color;
    }

    public void SetPlayerColor(int i_TeamIndex, int i_PlayerIndex, Color i_Color)
    {
        m_PlayersColors[i_TeamIndex, i_PlayerIndex] = i_Color;    
    }

    public void SetPlayerIsHuman(int i_TeamIndex, int i_PlayerIndex, bool i_Human)
    {
        m_Humans[i_TeamIndex, i_PlayerIndex] = i_Human;
    }

    public List<int> GetLineUpIds(int i_TeamIndex)
    {
        List<int> charactersIds = new List<int>();

        if (IsValidIndex(i_TeamIndex))
        {
            List<tnUICharacterSlot> slots = m_LineUp[i_TeamIndex];
            if (slots != null)
            {
                for (int characterIndex = 0; characterIndex < slots.Count; ++characterIndex)
                {
                    tnUICharacterSlot slot = slots[characterIndex];

                    if (slot == null)
                        continue;

                    int id = slot.characterId;
                    charactersIds.Add(id);
                }
            }
        }

        return charactersIds;
    }

    public void SetTimer(double i_Time)
    {
        Internal_SetTimer(i_Time);
    }

    public void SetTimer(float i_Time)
    {
        Internal_SetTimer(i_Time);
    }

    public void SetProceedTriggerCanSend(bool i_CanSend)
    {
        if (m_ProceedEventTrigger != null)
        {
            m_ProceedEventTrigger.canSend = i_CanSend;
        }
    }

    public void SetBackTriggerCanSend(bool i_CanSend)
    {
        if (m_BackEventTrigger != null)
        {
            m_BackEventTrigger.canSend = i_CanSend;
        }
    }

    // INTERNAL

    private void Internal_SetupTeam(int i_TeamIndex, tnTeamDescription i_TeamDescription)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        if (i_TeamDescription == null)
            return;

        int teamId = i_TeamDescription.teamId;
        tnTeamData teamData = tnGameData.GetTeamDataMain(teamId);

        if (teamData == null)
            return;

        List<int> lineUp = teamData.GetDefaultLineUp(i_TeamDescription.charactersCount);

        if (lineUp == null)
            return;

        SetTeamInfo(i_TeamIndex, teamData);

        // Lineup.

        {
            tnUITeamAnchors teamAnchors = m_TeamAnchorsSets[i_TeamIndex];

            tnUIAnchorsSet anchorsSet = null;

            if (teamAnchors != null)
            {
                anchorsSet = teamAnchors.GetAnchorsSetBySize(i_TeamDescription.charactersCount);
            }

            if (anchorsSet != null)
            {
                for (int index = 0; index < anchorsSet.anchorsCount && index < lineUp.Count; ++index)
                {
                    RectTransform anchor = anchorsSet.GetAnchorByIndex(index);
                    if (anchor != null)
                    {
                        int characterId = lineUp[index];

                        if (!teamData.Contains(characterId))
                            continue;

                        tnCharacterData characterData = tnGameData.GetCharacterDataMain(characterId);

                        if (characterData == null)
                            continue;

                        FacingDir facingDir = m_Facing[i_TeamIndex];

                        Color teamColor = i_TeamDescription.teamColor;
                        Sprite flag = teamData.baseSprite;

                        tnUICharacter character = SpawnCharacter(characterData, facingDir, teamColor, flag);

                        tnUICharacterSlot slot = SpawnCharacterSlot();
                        slot.transform.position = anchor.position;
                        slot.character = character;
                        slot.characterId = characterId;

                        bool isHuman = m_Humans[i_TeamIndex, index];
                        if (isHuman)
                        {
                            Color playerColor = m_PlayersColors[i_TeamIndex, index];
                            slot.SetPlayerColor(playerColor);
                        }
                        else
                        {
                            slot.ClearPlayerColor();
                        }

                        SlotList slotList = m_LineUp[i_TeamIndex];
                        slotList.Add(slot);
                    }
                }
            }

            m_TeamAnchors[i_TeamIndex] = anchorsSet;
        }

        // Bench.

        {
            tnUIBench bench = m_TeamAnchorsBench[i_TeamIndex];

            int lastBenchIndexUsed = -1;
            for (int index = 0; index < bench.entriesCount && index < teamData.charactersCount; ++index)
            {
                tnUIBenchEntry benchEntry = bench.GetEntryByIndex(index);
                if (benchEntry != null && benchEntry.anchor != null)
                {
                    int characterId = Hash.s_NULL;

                    for (int characterIndex = lastBenchIndexUsed + 1; characterIndex < teamData.charactersCount; ++characterIndex)
                    {
                        int id = teamData.GetCharacterKey(characterIndex);
                        if (!lineUp.Contains(id))
                        {
                            characterId = id;
                            lastBenchIndexUsed = characterIndex;
                            break;
                        }
                    }

                    tnCharacterData characterData = tnGameData.GetCharacterDataMain(characterId);

                    if (characterData == null)
                        continue;

                    FacingDir facingDir = m_Facing[i_TeamIndex];

                    Color teamColor = i_TeamDescription.teamColor;
                    Sprite flag = teamData.baseSprite;

                    tnUICharacter character = SpawnCharacter(characterData, facingDir, teamColor, flag);

                    tnUICharacterSlot slot = SpawnCharacterSlot();
                    slot.transform.position = benchEntry.anchor.position;
                    slot.character = character;
                    slot.characterId = characterId;
                    slot.ClearPlayerColor();

                    SlotList slotList = m_Bench[i_TeamIndex];
                    slotList.Add(slot);
                }
            }
        }
    }

    private void SetTeamInfo(int i_TeamIndex, tnTeamData i_TeamData)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        if (i_TeamData == null)
            return;

        tnUITeamInfo teamInfo = m_TeamInfo[i_TeamIndex];

        if (teamInfo != null)
        {
            teamInfo.SetFlag(i_TeamData.flag);
            teamInfo.SetName(i_TeamData.name);
        }
    }

    private void Swap(tnUICharacterSlot i_A, tnUICharacterSlot i_B)
    {
        if (i_A == null || i_B == null || i_A == i_B)
            return;

        int tempCharacterId = i_A.characterId;
        i_A.characterId = i_B.characterId;
        i_B.characterId = tempCharacterId;

        tnUICharacter tempCharacter = i_A.character;
        i_A.character = i_B.character;
        i_B.character = tempCharacter;
    }

    private void ClearLineUp(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        SlotList slotList = m_LineUp[i_TeamIndex];

        for (int index = 0; index < slotList.Count; ++index)
        {
            tnUICharacterSlot slot = slotList[index];
            if (slot != null)
            {
                tnUICharacter character = slot.character;
                RecycleCharacter(character);

                slot.Clear();
            }

            RecycleCharacterSlot(slot);
        }

        slotList.Clear();
    }

    private void ClearBench(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        SlotList slotList = m_Bench[i_TeamIndex];

        for (int index = 0; index < slotList.Count; ++index)
        {
            tnUICharacterSlot slot = slotList[index];
            if (slot != null)
            {
                tnUICharacter character = slot.character;
                RecycleCharacter(character);

                slot.Clear();
            }

            RecycleCharacterSlot(slot);
        }

        slotList.Clear();
    }

    private void SelectFirst(int i_TeamIndex)
    {
        Select(i_TeamIndex, GetFirstSlot(i_TeamIndex));
    }

    private void Select(int i_TeamIndex, int i_SlotIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        SlotList lineUp = m_LineUp[i_TeamIndex];
        SlotList bench = m_Bench[i_TeamIndex];

        if (i_SlotIndex < 0 || i_SlotIndex >= lineUp.Count + bench.Count)
        {
            return;
        }
        else
        {
            if (i_SlotIndex < lineUp.Count)
            {
                tnUICharacterSlot slot = lineUp[i_SlotIndex];
                if (slot != null)
                {
                    Select(i_TeamIndex, slot.gameObject);
                }
            }
            else
            {
                i_SlotIndex = i_SlotIndex % lineUp.Count;
                tnUICharacterSlot slot = bench[i_SlotIndex];
                if (slot != null)
                {
                    Select(i_TeamIndex, slot.gameObject);
                }
            }
        }
    }

    private void Select(int i_TeamIndex, GameObject i_Slot)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        if (i_Slot == null)
            return;

        // Release previous selection.

        {
            GameObject currentSlot = m_CurrentSlots[i_TeamIndex];

            if (currentSlot != null)
            {
                tnUICharacterSlot characterSlot = currentSlot.GetComponent<tnUICharacterSlot>();
                if (characterSlot != null)
                {
                    characterSlot.SetAvailable();
                }
            }
        }

        m_CurrentSlots[i_TeamIndex] = i_Slot;

        // Update new selection.

        {
            tnUICharacterSlot characterSlot = i_Slot.GetComponent<tnUICharacterSlot>();
            if (characterSlot != null)
            {
                Color captainColor = m_CaptainsColors[i_TeamIndex];
                characterSlot.SetHighlighted(captainColor);
            }
        }

        UpdatePortrait(i_TeamIndex);
    }

    private GameObject GetFirstSlot(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
        {
            return null;
        }

        SlotList slotList = m_LineUp[i_TeamIndex];

        if (slotList.Count > 0)
        {
            tnUICharacterSlot characterSlot = slotList[0];

            if (characterSlot != null)
            {
                return characterSlot.gameObject;
            }
        }

        return null;
    }

    private GameObject GetSlot(int i_TeamIndex, int i_SlotIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
        {
            return null;
        }

        SlotList slotList = m_LineUp[i_TeamIndex];

        if (i_SlotIndex < 0 || i_SlotIndex >= slotList.Count)
        {
            return null;
        }

        tnUICharacterSlot characterSlot = slotList[i_SlotIndex];

        if (characterSlot != null)
        {
            return characterSlot.gameObject;
        }

        return null;
    }

    private void Internal_ClearAll()
    {
        // Clear selection cache.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            m_SelectionsCache[teamIndex].Clear();
        }

        // Clear current slots.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            m_CurrentSlots[teamIndex] = null;
        }

        // Clear confirmations.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            m_Confirmations[teamIndex] = false;
        }

        // Clear anchors.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            m_TeamAnchors[teamIndex] = null;
        }

        // Clear line-up.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            ClearLineUp(teamIndex);
        }

        // Clear bench.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            ClearBench(teamIndex);
        }

        // Disable overlays.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            ClearOverlay(teamIndex);
        }

        // Clear potraits.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            ClearPortrait(teamIndex);
            SetPortraitEnable(teamIndex, false);
        }

        // Clear team colors.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            m_TeamsColors[teamIndex] = Color.white;
        }

        // Clear captains colors.

        for (int teamIndex = 0; teamIndex < s_MaxPlayers; ++teamIndex)
        {
            m_CaptainsColors[teamIndex] = Color.white;
        }

        // Clear player colors.

        m_PlayersColors.Reset(Color.white);
        m_Humans.Reset(false);
    }

    private bool IsReady(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
        {
            return false;
        }

        return m_Confirmations[i_TeamIndex];
    }

    private bool IsAvailable(GameObject i_Slot, int i_PlayerIndex)
    {
        if (i_Slot == null)
        {
            return false;
        }

        tnUICharacterSlot slot = i_Slot.GetComponent<tnUICharacterSlot>();
        if (slot != null)
        {
            SlotList lineUp = m_LineUp[i_PlayerIndex];
            SlotList bench = m_Bench[i_PlayerIndex];

            if (lineUp.Contains(slot) || bench.Contains(slot))
            {
                if (slot.character != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsValidIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_MaxPlayers)
        {
            return false;
        }

        return true;
    }

    private void UpdateOverlay(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        GameObject panelGo = m_Overlays[i_TeamIndex];

        if (panelGo != null)
        {
            bool isActive = m_Confirmations[i_TeamIndex];
            panelGo.SetActive(isActive);
        }
    }

    private void ClearOverlay(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        GameObject panelGo = m_Overlays[i_TeamIndex];

        if (panelGo != null)
        {
            panelGo.SetActive(false);
        }
    }

    private void UpdatePortrait(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        tnCharacterPortrait portrait = m_Portraits[i_TeamIndex];
        GameObject currentSelection = m_CurrentSlots[i_TeamIndex];

        ClearPortrait(i_TeamIndex);
        SetPortraitEnable(i_TeamIndex, false);

        if (portrait != null && currentSelection != null)
        {
            tnUICharacterSlot slot = currentSelection.GetComponent<tnUICharacterSlot>();
            if (slot != null)
            {
                int characterId = slot.characterId;
                if (characterId != Hash.s_NULL)
                {
                    tnCharacterData characterData = tnGameData.GetCharacterDataMain(characterId);
                    if (characterData != null)
                    {
                        SetPortraitEnable(i_TeamIndex, true);

                        FacingDir facing = m_Facing[i_TeamIndex];

                        Sprite characterSprite = (facing == FacingDir.FacingLeft) ? characterData.uiIconFacingLeft : characterData.uiIconFacingRight;

                        portrait.SetCharacterPortrait(characterSprite);
                        portrait.SetName(characterData.displayName);
                    }
                }
            }
        }
    }

    private void ClearPortrait(int i_TeamIndex)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        tnCharacterPortrait portrait = m_Portraits[i_TeamIndex];

        if (portrait != null)
        {
            portrait.Clear();
        }
    }

    private void SetPortraitEnable(int i_TeamIndex, bool i_Enable)
    {
        if (!IsValidIndex(i_TeamIndex))
            return;

        tnCharacterPortrait portrait = m_Portraits[i_TeamIndex];

        if (portrait != null)
        {
            portrait.gameObject.SetActive(i_Enable);
        }
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

    // UTILITIES

    private void InitializePool()
    {
        int poolSize = 2 * s_TeamSize;

        if (m_CharacterPrefab != null && m_CharactersRoot != null)
        {
            for (int index = 0; index < poolSize; ++index)
            {
                tnUICharacter characterInstance = GameObject.Instantiate<tnUICharacter>(m_CharacterPrefab);
                characterInstance.transform.SetParent(m_CharactersRoot, false);
                characterInstance.gameObject.SetActive(false);

                m_CharactersPool.Add(characterInstance);
            }
        }

        if (m_CharacterSlotPrefab != null && m_SlotsRoot != null)
        {
            for (int index = 0; index < poolSize; ++index)
            {
                tnUICharacterSlot characterSlot = GameObject.Instantiate<tnUICharacterSlot>(m_CharacterSlotPrefab);
                characterSlot.transform.SetParent(m_SlotsRoot, false);
                characterSlot.gameObject.SetActive(false);

                m_CharactersSlotsPool.Add(characterSlot);
            }
        }
    }

    private tnUICharacter SpawnCharacter(tnCharacterData i_CharacterData, FacingDir i_Facing, Color i_TeamColor, Sprite i_Flag)
    {
        tnUICharacter character = null;
        if (m_CharactersPool.Count > 0)
        {
            character = m_CharactersPool[m_CharactersPool.Count - 1];
            m_CharactersPool.RemoveAt(m_CharactersPool.Count - 1);
            character.gameObject.SetActive(true);
        }
        else
        {
            if (m_CharacterPrefab == null)
            {
                return null;
            }

            character = GameObject.Instantiate<tnUICharacter>(m_CharacterPrefab);
            character.transform.SetParent(transform, false);
        }

        character.SetBaseColor(i_TeamColor);
        character.SetFlagSprite(i_Flag);

        if (i_Facing == FacingDir.FacingRight)
        {
            character.SetCharacterSprite(i_CharacterData.uiIconFacingRight);
        }
        else // Facing Left
        {
            character.SetCharacterSprite(i_CharacterData.uiIconFacingLeft);
        }

        character.SetName(i_CharacterData.displayName);

        character.SetAvailable();
        character.Deselect();

        return character;
    }

    private tnUICharacterSlot SpawnCharacterSlot()
    {
        if (m_CharactersSlotsPool.Count > 0)
        {
            tnUICharacterSlot slot = m_CharactersSlotsPool[m_CharactersSlotsPool.Count - 1];
            m_CharactersSlotsPool.RemoveAt(m_CharactersSlotsPool.Count - 1);
            slot.gameObject.SetActive(true);
            return slot;
        }
        else
        {
            if (m_CharacterSlotPrefab == null)
            {
                return null;
            }

            tnUICharacterSlot slot = GameObject.Instantiate<tnUICharacterSlot>(m_CharacterSlotPrefab);
            slot.transform.SetParent(transform, false);
            return slot;
        }
    }

    private void RecycleCharacter(tnUICharacter i_Character)
    {
        if (i_Character == null)
            return;

        i_Character.Clear();

        i_Character.gameObject.SetActive(false);
        m_CharactersPool.Add(i_Character);
    }

    private void RecycleCharacterSlot(tnUICharacterSlot i_Slot)
    {
        if (i_Slot == null)
            return;

        i_Slot.Clear();

        i_Slot.gameObject.SetActive(false);
        m_CharactersSlotsPool.Add(i_Slot);
    }

    private GameObject GetNearest(GameObject i_Source, UINavigationDirection i_Direction, int i_PlayerIndex)
    {
        if (i_Source == null)
        {
            return null;
        }

        GameObject nextSelection = null;

        switch (i_Direction)
        {
            case UINavigationDirection.Left:
                nextSelection = GetNearestOnLeft(i_Source, i_PlayerIndex);
                break;
            case UINavigationDirection.Right:
                nextSelection = GetNearestOnRight(i_Source, i_PlayerIndex);
                break;
            case UINavigationDirection.Up:
                nextSelection = GetNearestOnUp(i_Source, i_PlayerIndex);
                break;
            case UINavigationDirection.Down:
                nextSelection = GetNearestOnDown(i_Source, i_PlayerIndex);
                break;
        }

        return nextSelection;
    }

    private GameObject GetNearestOnLeft(GameObject i_Source, int i_PlayerIndex)
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

        while (selectable != null && !IsAvailable(selectable.gameObject, i_PlayerIndex))
        {
            selectable = selectable.FindSelectableOnLeft();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnRight(GameObject i_Source, int i_PlayerIndex)
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

        while (selectable != null && !IsAvailable(selectable.gameObject, i_PlayerIndex))
        {
            selectable = selectable.FindSelectableOnRight();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnUp(GameObject i_Source, int i_PlayerIndex)
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

        while (selectable != null && !IsAvailable(selectable.gameObject, i_PlayerIndex))
        {
            selectable = selectable.FindSelectableOnUp();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private GameObject GetNearestOnDown(GameObject i_Source, int i_PlayerIndex)
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

        while (selectable != null && !IsAvailable(selectable.gameObject, i_PlayerIndex))
        {
            selectable = selectable.FindSelectableOnDown();
        }

        if (selectable == null)
        {
            return null;
        }

        return selectable.gameObject;
    }

    private void UpdatePortraits()
    {
        if (m_Portraits != null)
        {
            for (int index = 0; index < m_Portraits.Length; ++index)
            {
                UpdatePortrait(index);
            }
        }
    }

    private void SetPortraitsVisible(bool i_Visible)
    {
        if (m_Portraits != null)
        {
            for (int index = 0; index < m_Portraits.Length; ++index)
            {
                SetPortraitEnable(index, i_Visible);
            }
        }
    }

    private void ClearPortraits()
    {
        if (m_Portraits != null)
        {
            for (int index = 0; index < m_Portraits.Length; ++index)
            {
                ClearPortrait(index);
            }
        }
    }
}
