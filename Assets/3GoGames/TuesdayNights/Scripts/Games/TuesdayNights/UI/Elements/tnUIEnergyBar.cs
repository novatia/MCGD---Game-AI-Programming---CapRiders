using UnityEngine;

public class tnUIEnergyBar : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Root = null;

    [SerializeField]
    private tnUIEnergySlot m_SlotPrefab = null;
    [SerializeField]
    private tnUIEnergySlotSeparator m_SeparatorPrefab = null;
    [SerializeField]
    private RectTransform m_SlotsRoot = null;
    [SerializeField]
    private RectTransform m_SeparatorsRoot = null;
    [SerializeField]
    private int m_SlotsCount = 3;
    [SerializeField]
    private Color m_SeparatorFullColor = Color.black;
    [SerializeField]
    private Color m_SeparatorEmptyColor = Color.white;
    [SerializeField]
    private Color m_StartingBarColor = Color.white;

    private tnEnergy m_Energy = null;

    private tnUIEnergySlot[] m_Slots = null;
    private tnUIEnergySlotSeparator[] m_Separators = null;

    // MonoBehaviour's interface

    void Awake()
    {
        m_Energy = GetComponentInParent<tnEnergy>();

        if (m_SlotPrefab == null || m_SlotsRoot == null)
            return;

        m_SlotsCount = Mathf.Max(1, m_SlotsCount);

        m_Slots = new tnUIEnergySlot[m_SlotsCount];

        // Spawn slots.

        for (int slotIndex = 0; slotIndex < m_Slots.Length; ++slotIndex)
        {
            tnUIEnergySlot slotInstance = Instantiate<tnUIEnergySlot>(m_SlotPrefab);
            slotInstance.Clear();

            slotInstance.transform.SetParent(m_SlotsRoot, false);

            slotInstance.SetColor(m_StartingBarColor);

            m_Slots[slotIndex] = slotInstance;
        }

        // Spawn separators

        if (m_SeparatorsRoot == null || m_SeparatorPrefab == null)
            return;

        m_Separators = new tnUIEnergySlotSeparator[m_SlotsCount - 1];

        float barLenght = m_SlotsRoot.rect.width;
        float slotLenght = barLenght / m_Slots.Length;

        for (int separatorIndex = 0; separatorIndex < m_Separators.Length; ++separatorIndex)
        {
            tnUIEnergySlotSeparator separatorInstance = Instantiate<tnUIEnergySlotSeparator>(m_SeparatorPrefab);

            separatorInstance.transform.SetParent(m_SeparatorsRoot, false);
            RectTransform separatorTransform = (RectTransform)separatorInstance.transform;
            separatorTransform.anchoredPosition = new Vector2(slotLenght * (separatorIndex + 1), 0f);

            m_Separators[separatorIndex] = separatorInstance;
        }
    }

    void Start()
    {
        UpdateVisual();
    }

    void Update()
    {
        UpdateVisual();
    }

    // LOGIC

    public void SetVisible(bool i_Visible)
    {
        if (m_SlotsRoot == null)
            return;

        m_Root.SetActive(i_Visible);
    }

    public void SetColor(Color i_Color)
    {
        if (m_Slots == null)
            return;

        for (int slotIndex = 0; slotIndex < m_Slots.Length; ++slotIndex)
        {
            tnUIEnergySlot slot = m_Slots[slotIndex];
            if (slot != null)
            {
                slot.SetColor(i_Color);
            }
        }
    }

    // INTERNALS

    private void UpdateVisual()
    {
        if (m_Slots == null || m_Energy == null)
            return;

        float energyPercentage = m_Energy.energy.AsFloat();
        float slotPercentage = 1f / m_Slots.Length;

        for (int slotIndex = 0; slotIndex < m_Slots.Length; ++slotIndex)
        {
            tnUIEnergySlot slot = m_Slots[slotIndex];
            if (slot != null)
            {
                slot.Clear();

                float fullThreshold = slotIndex * slotPercentage + slotPercentage;
                if (energyPercentage >= fullThreshold)
                {
                    slot.Fill();
                }
                else
                {
                    float minLimit = slotIndex * slotPercentage;
                    float maxLimit = slotIndex * slotPercentage + slotPercentage;

                    float slotFillPercentage = Mathf.Clamp01((energyPercentage - minLimit) / (maxLimit - minLimit)); 
                    slot.SetValue(slotFillPercentage);
                }
            }
        }

        if (m_Separators == null)
            return;

        // Set Separators color

        for (int index = 0; index < m_Separators.Length; ++index)
        {
            tnUIEnergySlotSeparator separator = m_Separators[index];
            tnUIEnergySlot slot = m_Slots[index];

            if (slot != null && separator != null)
            {
                Color color = (slot.isFull) ? m_SeparatorFullColor : m_SeparatorEmptyColor;
                separator.SetColor(color);
            }
        }
    }
}
