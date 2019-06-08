using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System;
using System.Collections.Generic;

[Serializable]
public class SelectorItem
{
    [SerializeField]
    private int m_Id;
    [SerializeField]
    private string m_Label;
    [SerializeField]
    private string m_Description;
    [SerializeField]
    private Sprite m_Image;
    [SerializeField]
    private bool m_Locked;
    [SerializeField]
    private string m_LockedText;

    public int id
    {
        get
        {
            return m_Id;
        }
    }

    public string label
    {
        get
        {
            return m_Label;
        }
    }

    public string description
    {
        get
        {
            return m_Description;
        }
    }

    public Sprite image
    {
        get
        {
            return m_Image;
        }
    }

    public bool locked
    {
        get { return m_Locked; }
    }

    public string lockedText
    {
        get { return m_LockedText; }
    }

    // CTOR

    public SelectorItem(int i_Id, string i_Label, string i_Description, Sprite i_Image, bool i_Locked = false, string i_LockedText = "")
    {
        m_Id = i_Id;
        m_Label = i_Label;

        m_Description = i_Description;

        m_Image = i_Image;

        m_Locked = i_Locked;
        m_LockedText = i_LockedText;
    }
}

[Serializable]
public class SelectorData
{
    [SerializeField]
    private List<SelectorItem> m_Items;

    public int itemsCount
    {
        get
        {
            return m_Items.Count;
        }
    }

    public SelectorItem GetItem(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Items.Count)
        {
            return null;
        }

        return m_Items[i_Index];
    }

    public void AddItem(SelectorItem i_Entry)
    {
        m_Items.Add(i_Entry);
    }

    public void Clear()
    {
        m_Items.Clear();
    }

    // CTOR

    public SelectorData()
    {
        m_Items = new List<SelectorItem>();
    }
}

[RequireComponent(typeof(RectTransform))]
public sealed class UISelector : Selectable, ICanvasElement, ISubmitHandler, IPointerClickHandler
{
    [Serializable]
    public class SelectorEvent : UnityEvent<SelectorItem> { }
    [Serializable]
    public class NavigationEvent : UnityEvent { }

    [SerializeField]
    private SelectorEvent m_OnSubmit = new SelectorEvent();
    public SelectorEvent onSubmit { get { return m_OnSubmit; } set { m_OnSubmit = value; } }

    [SerializeField]
    private SelectorEvent m_OnChangeSelection = new SelectorEvent();
    public SelectorEvent onChangeSelection { get { return m_OnChangeSelection; } set { m_OnChangeSelection = value; } }

    [SerializeField]
    private SelectorEvent m_OnSelectorMove = new SelectorEvent();
    public SelectorEvent onSelectorMove { get { return m_OnSelectorMove; } set { m_OnSelectorMove = value; } }

    [SerializeField]
    private NavigationEvent m_OnMove = new NavigationEvent();
    public NavigationEvent onMove { get { return m_OnMove; } set { m_OnMove = value; } }

    private SelectorData m_Data = null;

    [SerializeField]
    private Text m_FillText;
    public Text filltext { get { return m_FillText; } set { m_FillText = value; } }

    [SerializeField]
    private Text m_DescriptionText;
    public Text descriptionText { get { return m_DescriptionText; } set { m_DescriptionText = value; } }

    [SerializeField]
    private Image m_FillImage;
    public Image fillImage { get { return m_FillImage; } set { m_FillImage = value; } }

    [SerializeField]
    private Image m_LockedImage = null;
    [SerializeField]
    private Text m_LockedText = null;

    [SerializeField]
    private Image m_LeftArrow = null;
    [SerializeField]
    private Image m_RightArrow = null;

    private int m_CurrentIndex = -1;

    public SelectorItem currentItem
    {
        get
        {
            if (m_Data != null)
            {
                return m_Data.GetItem(m_CurrentIndex);
            }

            return null;
        }
    }

    public int itemsCount
    {
        get
        {
            if (m_Data == null)
            {
                return 0;
            }

            return m_Data.itemsCount;
        }
    }

    [SerializeField]
    private float m_MoveDelay = 0.25f;
    public float moveDelay { get { return m_MoveDelay; } set { m_MoveDelay = value; } }

    private float m_ElapsedTime = 0f;

    // BUSINESS LOGIC

    public SelectorItem GetSelectorItem(int i_Index)
    {
        if (m_Data == null)
        {
            return null;
        }

        if (i_Index < 0 || i_Index >= m_Data.itemsCount)
        {
            return null;
        }

        return m_Data.GetItem(i_Index);
    }

    public void SetData(SelectorData i_Data)
    {
        m_Data = i_Data;

        if (!IndexValid(m_CurrentIndex))
        {
            SelectItemByIndex(0); // Reset selection.
        }
        else
        {
            SelectItemByIndex(m_CurrentIndex); // Refresh view.
        }
    }

    public void Clear()
    {
        m_Data = null;
        SelectItemByIndex(-1);
    }

    public void Select(int i_Dir)
    {
        if (i_Dir > 0)
        {
            SelectNext();
        }
        else
        {
            SelectPrev();
        }
    }

    public void SelectPrev()
    {
        InternalSelectPrev();
    }

    public void SelectNext()
    {
        InternalSelectNext();
    }

    public void SelectItem(int i_Id)
    {
        InternalSelectItem(GetItemIndex(i_Id));
    }

    public void SelectItemByIndex(int i_ItemIndex)
    {
        InternalSelectItem(i_ItemIndex);
    }

    public void SelectFirstUnlockedItem()
    {
        InternalSelectFirstUnlockedItem();
    }

    // MonoBehaviour INTERFACE

    protected override void OnEnable()
    {
        base.OnEnable();

        SelectItemByIndex(m_CurrentIndex);

        UpdateVisual();

        m_ElapsedTime = 0f;
    }

    protected override void OnDisable()
    {
        m_ElapsedTime = 0f;

        base.OnDisable();
    }

    void Update()
    {
        m_ElapsedTime += Time.unscaledDeltaTime;

        if (!IsInteractable())
        {
            if (m_LeftArrow != null)
            {
                m_LeftArrow.gameObject.SetActive(false);
            }

            if (m_RightArrow != null)
            {
                m_RightArrow.gameObject.SetActive(false);
            }
        }
        else
        {
            if (m_LeftArrow != null)
            {
                m_LeftArrow.gameObject.SetActive(true);
            }

            if (m_RightArrow != null)
            {
                m_RightArrow.gameObject.SetActive(true);
            }
        }
    }

    // IMoveHandler INTERFACE

    public override void OnMove(AxisEventData i_EventData)
    {
        if (!IsActive() || !IsInteractable())
        {
            base.OnMove(i_EventData);
            return;
        }

        switch (i_EventData.moveDir)
        {
            case MoveDirection.Left:
                
                // Select previous.
            
                if (m_ElapsedTime > m_MoveDelay)
                {
                    SelectPrev();
                    m_ElapsedTime = 0f;
                }

                break;

            case MoveDirection.Right:

                // Select next.
                
                if (m_ElapsedTime > m_MoveDelay)
                {
                    SelectNext();
                    m_ElapsedTime = 0f;
                }

                break;
            
            case MoveDirection.Up:
            case MoveDirection.Down:

                base.OnMove(i_EventData);

                if (m_OnMove != null)
                {
                    m_OnMove.Invoke();
                }

                break;
        }
    }

    // Selectable INTERFACE

    public override Selectable FindSelectableOnLeft()
    {
        return null; // Disable navigation.
    }

    public override Selectable FindSelectableOnRight()
    {
        return null; // Disable navigation.
    }

    public override Selectable FindSelectableOnUp()
    {
        return base.FindSelectableOnUp(); // Automatic navigation.
    }

    public override Selectable FindSelectableOnDown()
    {
        return base.FindSelectableOnDown(); // Automatic navigation.
    }

    // ICanvasElement INTERFACE

    public void Rebuild(CanvasUpdate i_Executing)
    {

    }

    // TODO: Override OnValidate
    // TODO: Override OnRectTransformDimensionsChange

    // ISubmitHandler interface

    public void OnSubmit(BaseEventData i_EventData)
    {
        if (!IsActive() || !IsInteractable())
            return;

        if (m_OnSubmit != null)
        {
            if (currentItem != null)
            {
                if (!currentItem.locked)
                {
                    m_OnSubmit.Invoke(currentItem);
                }
            }
        }
    }

    public void LayoutComplete()
    {

    }

    public void GraphicUpdateComplete()
    {

    }

    // INTERNALS

    private void InternalSelectPrev()
    {
        InternalSelectItem(m_CurrentIndex - 1);

        if (m_OnSelectorMove != null)
        {
            if (currentItem != null)
            {
                m_OnSelectorMove.Invoke(currentItem);
            }
            else
            {
                m_OnSelectorMove.Invoke(null);
            }
        }
    }

    private void InternalSelectNext()
    {
        InternalSelectItem(m_CurrentIndex + 1);

        if (m_OnSelectorMove != null)
        {
            if (currentItem != null)
            {
                m_OnSelectorMove.Invoke(currentItem);
            }
            else
            {
                m_OnSelectorMove.Invoke(null);
            }
        }
    }

    private void InternalSelectItem(int i_ItemIndex)
    {
        int normalizedIndex = ModIndex(i_ItemIndex);

        if (IndexValid(normalizedIndex))
        {
            if (m_CurrentIndex != normalizedIndex)
            {
                m_CurrentIndex = normalizedIndex;
            }
        }
        else
        {
            m_CurrentIndex = -1;
        }

        UpdateVisual();

        if (m_OnChangeSelection != null)
        {
            if (currentItem != null)
            {
                m_OnChangeSelection.Invoke(currentItem);
            }
            else
            {
                m_OnChangeSelection.Invoke(null);
            }
        }
    }

    private void InternalSelectFirstUnlockedItem()
    {
        if (m_Data == null)
            return;

        int firstValidIndex = -1;

        for (int itemIndex = 0; itemIndex < m_Data.itemsCount; ++itemIndex)
        {
            SelectorItem selectorItem = m_Data.GetItem(itemIndex);

            if (selectorItem == null)
                continue;

            if (!selectorItem.locked)
            {
                firstValidIndex = itemIndex;
                break;
            }
        }

        if (firstValidIndex >= 0)
        {
            SelectItemByIndex(firstValidIndex);
        }
    }

    private void UpdateVisual()
    {
        if (filltext != null)
        {
            if (currentItem == null)
            {
                filltext.text = "";
            }
            else
            {
                filltext.text = currentItem.label;
            }
        }

        if (descriptionText != null)
        {
            if (currentItem == null)
            {
                descriptionText.text = "";
            }
            else
            {
                descriptionText.text = currentItem.description;
            }
        }

        if (fillImage != null)
        {
            if (currentItem == null)
            {
                fillImage.sprite = null;
            }
            else
            {
                fillImage.sprite = currentItem.image;
            }
        }

        if (currentItem != null)
        {
            if (m_LockedImage != null)
            {
                m_LockedImage.enabled = currentItem.locked;
            }

            if (currentItem.locked)
            {
                if (m_LockedText != null)
                {
                    m_LockedText.text = currentItem.lockedText;
                }
            }
            else
            {
                if (m_LockedText != null)
                {
                    m_LockedText.text = "";
                }
            }
        }
        else
        {
            if (m_LockedImage != null)
            {
                m_LockedImage.enabled = false;
            }

            if (m_LockedText != null)
            {
                m_LockedText.text = "";
            }
        }
    }

    private bool IndexValid(int i_ItemIndex)
    {
        return (m_Data != null && i_ItemIndex >= 0 && i_ItemIndex < m_Data.itemsCount);
    }

    private int ModIndex(int i_ItemIndex)
    {
        if (m_Data == null || m_Data.itemsCount <= 0)
        {
            return i_ItemIndex;
        }

        int result = i_ItemIndex % m_Data.itemsCount;

        if (result != 0)
        {
            if (i_ItemIndex < 0)
            {
                result += m_Data.itemsCount;
            }
        }

        return result;
    }

    private int GetItemIndex(int i_Id)
    {
        if (m_Data != null)
        {
            for (int itemIndex = 0; itemIndex < m_Data.itemsCount; ++itemIndex)
            {
                SelectorItem selectorItem = m_Data.GetItem(itemIndex);
                if (selectorItem != null && selectorItem.id == i_Id)
                {
                    return itemIndex;
                }
            }
        }

        return -1;
    }

    // IPointerClickHandler INTERFACE

    public void OnPointerClick(PointerEventData i_EventData)
    {
        // Check left.

        if (m_LeftArrow != null && m_LeftArrow.gameObject.activeSelf)
        {
            if (m_LeftArrow.rectTransform.rect.size.x > 0f && m_LeftArrow.rectTransform.rect.size.y > 0f)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(m_LeftArrow.rectTransform, i_EventData.position, i_EventData.pressEventCamera))
                {
                    SelectPrev();
                }
            }
        }

        // Check right.

        if (m_RightArrow != null && m_RightArrow.gameObject.activeSelf)
        {
            if (m_RightArrow.rectTransform.rect.size.x > 0f && m_RightArrow.rectTransform.rect.size.y > 0f)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(m_RightArrow.rectTransform, i_EventData.position, i_EventData.pressEventCamera))
                {
                    SelectNext();
                }
            }
        }
    }
}
