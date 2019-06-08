using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using TuesdayNights;

[Serializable]
public struct AnchorsGridLayout
{
    [SerializeField]
    private int m_ControllersPerRow;

    [SerializeField]
    private float m_DistanceFromTop;
    [SerializeField]
    private float m_DistanceFromBot;
    [SerializeField]
    private float m_DistanceFromLeft;
    [SerializeField]
    private float m_DistanceFromRight;

    public int controllersPerRow
    {
        get { return m_ControllersPerRow; }
        set { m_ControllersPerRow = value; }
    }

    public float distanceFromTop
    {
        get { return m_DistanceFromTop; }
        set { m_DistanceFromTop = value; }
    }

    public float distanceFromBot
    {
        get { return m_DistanceFromBot; }
        set { m_DistanceFromBot = value; }
    }

    public float distanceFromLeft
    {
        get { return m_DistanceFromLeft; }
        set { m_DistanceFromLeft = value; }
    }

    public float distanceFromRight
    {
        get { return m_DistanceFromRight; }
        set { m_DistanceFromRight = value; }
    }
}

public class GridEntry
{
    private ControllerAnchor m_ControllerAnchor = null;
    private tnUIDevice m_Device = null;

    private bool m_IsBot = false;

    public ControllerAnchor controllerAnchor
    {
        get { return m_ControllerAnchor; }
    }

    public bool isFree
    {
        get
        {
            bool free = (m_ControllerAnchor != null && m_ControllerAnchor.rectTransform != null) && (m_Device == null) && (!m_IsBot);
            return free;
        }
    }

    public bool isBot
    {
        get { return m_IsBot; }
        set { m_IsBot = value; }
    }

    public tnUIDevice device
    {
        get { return m_Device; }
        set
        {
            m_Device = value;
        }
    }

    // LOGIC

    public void SetFree()
    {
        m_Device = null;
    }

    // CTOR

    public GridEntry(ControllerAnchor i_ControllerAnchor)
    {
        m_ControllerAnchor = i_ControllerAnchor;
    }
}

public class tnUITeam : MonoBehaviour
{
    [SerializeField]
    private int m_EntriesCount = 11;
    [SerializeField]
    private RectTransform m_GridPivotPrefab = null;
    [SerializeField]
    private tnUIBot m_UIBotPrefab = null;
    [SerializeField]
    private AnchorsGridLayout m_Layout;

    [SerializeField]
    private RectTransform m_BotControls = null;

    private List<tnUIBot> m_BotsPool = null;

    private List<GridEntry> m_GridEntries = null;
    private List<tnUIBot> m_Bots = null;

    private bool m_CanAddBot = true;

    private static float s_AlphaEnabled = 1f;
    private static float s_AlphaDisabled = 0.5f;

    public int entriesCount
    {
        get { return m_GridEntries.Count; }
    }

    // MonoBehaviour's INTERFACE

    void OnEnable()
    {
        string value = tnGameData.GetGameSettingsValueMain(GlobalSettings.s_EnableAI);
        m_CanAddBot = (value == null || (value == "" || value == "TRUE"));

        RefreshBotControls();
    }

    void OnDisable()
    {

    }

    void Update()
    {
        RefreshBotControls();
    }

    // BUSINESS LOGIC

    public void CreateGrid()
    {
        InternalCreateGrid();
    }

    public void Clear()
    {
        if (m_GridEntries == null || m_Bots == null)
            return;

        while (m_Bots.Count > 0)
        {
            RemoveBot();
        }
    }

    public GridEntry GetFirstAvailableEntry()
    {
        if (m_GridEntries == null)
        {
            return null;
        }

        for (int index = 0; index < m_GridEntries.Count; ++index)
        {
            GridEntry entry = m_GridEntries[index];
            if (entry != null)
            {
                bool isFree = entry.isFree;
                if (isFree)
                {
                    return entry;
                }
            }
        }

        return null;
    }

    public GridEntry GetLastAvailableEntry()
    {
        if (m_GridEntries == null)
        {
            return null;
        }

        for (int index = m_GridEntries.Count - 1; index >= 0; --index)
        {
            GridEntry entry = m_GridEntries[index];
            if (entry != null)
            {
                bool isFree = entry.isFree;
                if (isFree)
                {
                    return entry;
                }
            }
        }

        return null;
    }

    public GridEntry GetEntryByIndex(int i_Index)
    {
        if (m_GridEntries == null)
        {
            return null;
        }

        if (i_Index < 0 || i_Index >= m_GridEntries.Count)
        {
            return null;
        }

        return m_GridEntries[i_Index];
    }

    public GridEntry GetFirstBotEntry()
    {
        if (m_GridEntries == null)
        {
            return null;
        }

        for (int index = 0; index < m_GridEntries.Count; ++index)
        {
            GridEntry entry = m_GridEntries[index];
            if (entry != null)
            {
                bool isBot = entry.isBot;
                if (isBot)
                {
                    return entry;
                }
            }
        }

        return null;
    }

    public GridEntry GetLastBotEntry()
    {
        if (m_GridEntries == null)
        {
            return null;
        }

        for (int index = m_GridEntries.Count - 1; index >= 0; --index)
        {
            GridEntry entry = m_GridEntries[index];
            if (entry != null)
            {
                bool isBot = entry.isBot;
                if (isBot)
                {
                    return entry;
                }
            }
        }

        return null;
    }

    public bool AddBot()
    {
        if (!CanAddBot())
        {
            return false;
        }

        if (m_UIBotPrefab == null)
        {
            return false;
        }

        GridEntry gridEntry = GetLastAvailableEntry();

        if (gridEntry == null)
        {
            return false;
        }

        tnUIBot bot = SpawnBot();
        if (bot == null)
        {
            return false;
        }

        gridEntry.isBot = true;

        RectTransform botTransform = (RectTransform)bot.transform;
        botTransform.position = gridEntry.controllerAnchor.rectTransform.position;
        botTransform.sizeDelta = new Vector2(gridEntry.controllerAnchor.width, gridEntry.controllerAnchor.height);

        m_Bots.Add(bot);

        return true;
    }

    public bool RemoveBot()
    {
        GridEntry gridEntry = GetFirstBotEntry();

        if (gridEntry == null)
        {
            return false;
        }

        gridEntry.isBot = false;

        tnUIBot lastBot = m_Bots[m_Bots.Count - 1];
        m_Bots.RemoveAt(m_Bots.Count - 1);

        RecycleBot(lastBot);

        return true;
    }

    // INTERNALS

    private void CheckLayoutConfig()
    {
        m_Layout.controllersPerRow = Mathf.Max(1, m_Layout.controllersPerRow);
        m_Layout.distanceFromTop = Mathf.Max(0f, m_Layout.distanceFromTop);
        m_Layout.distanceFromBot = Mathf.Max(0f, m_Layout.distanceFromBot);
        m_Layout.distanceFromLeft = Mathf.Max(0f, m_Layout.distanceFromLeft);
        m_Layout.distanceFromRight = Mathf.Max(0f, m_Layout.distanceFromRight);
    }

    private void InternalCreateGrid()
    {
        if (m_GridPivotPrefab == null)
            return;

        CreateBotsPool();

        CheckLayoutConfig();

        m_GridEntries = new List<GridEntry>();
        m_Bots = new List<tnUIBot>();

        RectTransform rectTransform = (RectTransform)transform;

        float totalWidth = rectTransform.sizeDelta.x - m_Layout.distanceFromLeft - m_Layout.distanceFromRight;
        float totalHeigth = rectTransform.sizeDelta.y - m_Layout.distanceFromTop - m_Layout.distanceFromBot;

        int rows = m_EntriesCount / m_Layout.controllersPerRow;
        rows += (m_EntriesCount % m_Layout.controllersPerRow != 0) ? 1 : 0;
        int columns = m_Layout.controllersPerRow;

        float slotDistanceX = (totalWidth / columns);
        float slotDistanceY = (totalHeigth / rows);

        float firstX = slotDistanceX / 2f;
        float firstY = slotDistanceY / 2f;

        for (int index = 0; index < m_EntriesCount; ++index)
        {
            int row = index / columns;
            int column = index % columns;

            RectTransform rt = GameObject.Instantiate(m_GridPivotPrefab);
            rt.gameObject.name = "Anchor_" + index;
            rt.SetParent(rectTransform);

            float x = m_Layout.distanceFromLeft + firstX + column * slotDistanceX;
            float y = -m_Layout.distanceFromTop - firstY - row * slotDistanceY;

            rt.anchoredPosition = new Vector2(x, y);

            ControllerAnchor controllerAnchor = new ControllerAnchor(rt);
            controllerAnchor.SetWidth(slotDistanceX);
            controllerAnchor.SetHeight(slotDistanceY);

            GridEntry gridEntry = new GridEntry(controllerAnchor);
            m_GridEntries.Add(gridEntry);
        }
    }

    private void CreateBotsPool()
    {
        m_BotsPool = new List<tnUIBot>();

        if (m_UIBotPrefab == null)
            return;

        for (int index = 0; index < m_EntriesCount; ++index)
        {
            tnUIBot bot = GameObject.Instantiate<tnUIBot>(m_UIBotPrefab);
            bot.transform.SetParent(transform, false);
            bot.gameObject.SetActive(false);
            m_BotsPool.Add(bot);
        }
    }

    private tnUIBot SpawnBot()
    {
        if (m_BotsPool != null && m_BotsPool.Count > 0)
        {
            tnUIBot bot = m_BotsPool[m_BotsPool.Count - 1];
            m_BotsPool.RemoveAt(m_BotsPool.Count - 1); 
            bot.gameObject.SetActive(true);
            return bot;
        }
        else
        {
            if (m_UIBotPrefab == null)
            {
                return null;
            }

            tnUIBot bot = GameObject.Instantiate<tnUIBot>(m_UIBotPrefab);
            return bot;
        }
    }

    private void RecycleBot(tnUIBot i_Bot)
    {
        if (i_Bot == null)
            return;

        i_Bot.gameObject.SetActive(false);
        m_BotsPool.Add(i_Bot);
    }

    private bool CanAddBot()
    {
        return m_CanAddBot;
    }

    private void RefreshBotControls()
    {
        int humansCount = 0;

        for (int gridEntryIndex = 0; gridEntryIndex < m_GridEntries.Count; ++gridEntryIndex)
        {
            GridEntry gridEntry = m_GridEntries[gridEntryIndex];

            if (gridEntry != null)
            {
                if (!gridEntry.isFree && !gridEntry.isBot)
                {
                    ++humansCount;
                }
            }
        }

        bool enableControls = m_CanAddBot && (humansCount > 0);

        if (enableControls)
        {
            if (m_BotControls != null)
            {
                CanvasGroup canvasGroup = m_BotControls.GetComponent<CanvasGroup>();

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = s_AlphaEnabled;
                }
            }
        }
        else
        {
            CanvasGroup canvasGroup = m_BotControls.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.alpha = s_AlphaDisabled;
            }
        }
    }
}
