using UnityEngine;

using System;
using System.Collections.Generic;

using WiFiInput.Server;

[Serializable]
public struct ControllersGridLayoutConfig
{
    // FIELDS

    [SerializeField]
    private int m_PadsPerRow;

    [SerializeField]
    private float m_PadPanelHeight;

    [SerializeField]
    private float m_PadDistanceFromTop;
    [SerializeField]
    private float m_PadDistanceFromLeft;
    [SerializeField]
    private float m_PadDistanceFromRight;
    [SerializeField]
    private float m_PadXSpacing;
    [SerializeField]
    private float m_PadYSpacing;

    [SerializeField]
    private float m_DistancePadsPhones;

    [SerializeField]
    private int m_PhonesPerRow;

    [SerializeField]
    private float m_PhoneDistanceFromLeft;
    [SerializeField]
    private float m_PhoneDistanceFromRight;
    [SerializeField]
    private float m_PhoneDistanceFromBot;
    [SerializeField]
    private float m_PhoneXSpacing;
    [SerializeField]
    private float m_PhoneYSpacing;

    // ACCESSORS

    public int padsPerRow
    {
        get { return m_PadsPerRow; }
        set { m_PadsPerRow = value; }
    }

    public float padPanelHeight
    {
        get { return m_PadPanelHeight; }
        set { m_PadPanelHeight = value; }
    }

    public float padDistanceFromTop
    {
        get { return m_PadDistanceFromTop; }
        set { m_PadDistanceFromTop = value; }
    }

    public float padDistanceFromLeft
    {
        get { return m_PadDistanceFromLeft; }
        set { m_PadDistanceFromLeft = value; }
    }

    public float padDistanceFromRight
    {
        get { return m_PadDistanceFromRight; }
        set { m_PadDistanceFromRight = value; }
    }

    public float padXSpacing
    {
        get { return m_PadXSpacing; }
        set { m_PadXSpacing = value; }
    }

    public float padYSpacing
    {
        get { return m_PadYSpacing; }
        set { m_PadYSpacing = value; }
    }

    public float distancePadsPhones
    {
        get { return m_DistancePadsPhones; }
        set { m_DistancePadsPhones = value; }
    }

    public int phonesPerRow
    {
        get { return m_PhonesPerRow; }
        set { m_PhonesPerRow = value; }
    }

    public float phoneDistanceFromBot
    {
        get { return m_PhoneDistanceFromBot; }
        set { m_PhoneDistanceFromBot = value; }
    }

    public float phoneDistanceFromLeft
    {
        get { return m_PhoneDistanceFromLeft; }
        set { m_PhoneDistanceFromLeft = value; }
    }

    public float phoneDistanceFromRight
    {
        get { return m_PhoneDistanceFromRight; }
        set { m_PhoneDistanceFromRight = value; }
    }

    public float phoneXSpacing
    {
        get { return m_PhoneXSpacing; }
        set { m_PhoneXSpacing = value; }
    }

    public float phoneYSpacing
    {
        get { return m_PhoneYSpacing; }
        set { m_PhoneYSpacing = value; }
    }
}

public class ControllerAnchor
{
    private RectTransform m_Transform = null;

    private float m_Width = 0f;
    private float m_Height = 0f;

    public RectTransform rectTransform
    {
        get { return m_Transform; }
    }

    public float width
    {
        get { return m_Width; }
    }

    public float height
    {
        get { return m_Height; }
    }

    // LOGIC

    public void SetWidth(float i_Width)
    {
        m_Width = i_Width;
    }

    public void SetHeight(float i_Height)
    {
        m_Height = i_Height;
    }

    // CTOR

    public ControllerAnchor(RectTransform i_Transform)
    {
        m_Transform = i_Transform;
    }
}

public class tnUIControllersGrid : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_GridPivotPrefab = null;
    [SerializeField]
    private ControllersGridLayoutConfig m_Layout;

    private RectTransform m_Root = null;
    private List<ControllerAnchor> m_Anchors = null;

    public int anchorsCount
    {
        get
        {
            if (m_Anchors != null)
            {
                return m_Anchors.Count;
            }

            return 0;
        }
    }

    // LOGIC

    public void CreateGrid()
    {
        m_Anchors = new List<ControllerAnchor>();
        m_Root = (RectTransform)transform;

        InternalSpawnAnchors();
    }

    public ControllerAnchor GetAnchorByIndex(int i_Index)
    {
        if (m_Anchors == null)
        {
            return null;
        }

        if (i_Index < 0 || i_Index >= m_Anchors.Count)
        {
            return null;
        }

        return m_Anchors[i_Index];
    }

    // INTERNALS

    private void CheckLayoutConfig()
    {
        m_Layout.padsPerRow = Mathf.Max(1, m_Layout.padsPerRow);

        m_Layout.padPanelHeight = Mathf.Max(1f, m_Layout.padPanelHeight);

        m_Layout.padDistanceFromTop = Mathf.Max(0f, m_Layout.padDistanceFromTop);
        m_Layout.padDistanceFromLeft = Mathf.Max(0f, m_Layout.padDistanceFromLeft);
        m_Layout.padDistanceFromRight = Mathf.Max(0f, m_Layout.padDistanceFromRight);

        m_Layout.padXSpacing = Mathf.Max(0f, m_Layout.padXSpacing);
        m_Layout.padYSpacing = Mathf.Max(0f, m_Layout.padYSpacing);

        m_Layout.distancePadsPhones = Mathf.Max(0f, m_Layout.distancePadsPhones);

        m_Layout.phonesPerRow = Mathf.Max(1, m_Layout.phonesPerRow);

        m_Layout.phoneDistanceFromLeft = Mathf.Max(0f, m_Layout.phoneDistanceFromLeft);
        m_Layout.phoneDistanceFromRight = Mathf.Max(0f, m_Layout.phoneDistanceFromRight);
        m_Layout.phoneDistanceFromBot = Mathf.Max(0f, m_Layout.phoneDistanceFromBot);

        m_Layout.phoneXSpacing = Mathf.Max(0f, m_Layout.phoneXSpacing);
        m_Layout.phoneYSpacing = Mathf.Max(0f, m_Layout.phoneYSpacing);
    }

    private void InternalSpawnAnchors()
    {
        CheckLayoutConfig();

        SpawnPadsAnchors();
        SpawnPhonesAnchors();
    }

    private void SpawnPadsAnchors()
    {
        if (m_GridPivotPrefab == null || m_Root == null || m_Anchors == null)
            return;

        int playersCount = InputSystem.numPlayersMain;

        float width = m_Root.sizeDelta.x - m_Layout.padDistanceFromLeft - m_Layout.padDistanceFromRight;
        float height = m_Layout.padPanelHeight;

        float startY = -m_Layout.padDistanceFromTop;

        int rows = playersCount / m_Layout.padsPerRow;
        rows += (playersCount % m_Layout.padsPerRow != 0) ? 1 : 0;
        int columns = m_Layout.padsPerRow;

        float padWidth = (width - (m_Layout.padXSpacing * (columns + 1))) / columns;
        float padHeight = (height - (m_Layout.padYSpacing * (rows + 1))) / rows;

        for (int playerIndex = 0; playerIndex < playersCount; ++playerIndex)
        {
            int row = playerIndex / columns;
            int column = playerIndex % columns;

            RectTransform pivot = GameObject.Instantiate(m_GridPivotPrefab);
            pivot.gameObject.name = "Anchor_" + playerIndex;
            pivot.SetParent(m_Root);

            float x = m_Layout.padDistanceFromLeft + ((column + 1) * m_Layout.padXSpacing) + (column * padWidth) + (padWidth / 2f);
            float y = startY - ((row + 1) * m_Layout.padYSpacing) - (row * padHeight) - (padHeight / 2f);

            pivot.anchoredPosition = new Vector2(x, y);

            ControllerAnchor anchor = new ControllerAnchor(pivot);
            anchor.SetWidth(padWidth);
            anchor.SetHeight(padHeight);

            m_Anchors.Add(anchor);
        }
    }

    private void SpawnPhonesAnchors()
    {
        if (m_GridPivotPrefab == null || m_Root == null || m_Anchors == null)
            return;

        int playersCount = WiFiInputSystem.playersCountMain;

        float width = m_Root.sizeDelta.x - m_Layout.phoneDistanceFromLeft - m_Layout.phoneDistanceFromRight;
        float height = m_Root.sizeDelta.y - m_Layout.padDistanceFromTop - m_Layout.padPanelHeight - m_Layout.distancePadsPhones - m_Layout.phoneDistanceFromBot;

        float startY = -m_Layout.padDistanceFromTop - m_Layout.padPanelHeight - m_Layout.distancePadsPhones;

        int rows = playersCount / m_Layout.phonesPerRow;
        rows += (playersCount % m_Layout.phonesPerRow != 0) ? 1 : 0;
        int columns = m_Layout.phonesPerRow;

        float phoneWidth = (width - (m_Layout.phoneXSpacing * (columns + 1))) / columns;
        float phoneHeight = (height - (m_Layout.phoneYSpacing * (rows + 1))) / rows;

        for (int playerIndex = 0; playerIndex < playersCount; ++playerIndex)
        {
            int row = playerIndex / columns;
            int column = playerIndex % columns;

            RectTransform pivot = GameObject.Instantiate(m_GridPivotPrefab);
            pivot.gameObject.name = "Anchor_" + playerIndex;
            pivot.SetParent(m_Root);

            float x = m_Layout.phoneDistanceFromLeft + ((column + 1) * m_Layout.phoneXSpacing) + (column * phoneWidth) + (phoneWidth / 2f);
            float y = startY - ((row + 1) * m_Layout.phoneYSpacing) - (row * phoneHeight) - (phoneHeight / 2f);

            pivot.anchoredPosition = new Vector2(x, y);

            ControllerAnchor anchor = new ControllerAnchor(pivot);
            anchor.SetWidth(phoneWidth);
            anchor.SetHeight(phoneHeight);

            m_Anchors.Add(anchor);
        }
    }
}
