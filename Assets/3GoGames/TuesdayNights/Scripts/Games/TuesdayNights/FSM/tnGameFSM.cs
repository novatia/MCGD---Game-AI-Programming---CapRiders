using System;

using GoUI;

public abstract class tnGameFSM<T> : tnFSM<T> where T : struct, IConvertible, IComparable
{
    private UIPanelsManager m_PanelsManager = null;

    // MonoBehaviour's INTERFACE

    protected override void Awake()
    {
        base.Awake();

        UIPanelsManager panelsManager = GetComponent<UIPanelsManager>();
        if (panelsManager == null)
        {
            panelsManager = gameObject.AddComponent<UIPanelsManager>();
        }

        panelsManager.Initialize();

        m_PanelsManager = panelsManager;
    }

    // btFSM's INTERFACE

    protected override void OnFSMReturn()
    {
        base.OnFSMReturn();

        ClearAllGroups();
    }

    // UIPanelsManager's methods

    public void OpenPanel(UIBasePanel i_Panel)
    {
        if (i_Panel != null)
        {
            i_Panel.Open();
        }
    }

    public void ClosePanel(UIBasePanel i_Panel)
    {
        if (i_Panel != null)
        {
            i_Panel.Close();
        }
    }

    public void SwitchPanels(UIGroup i_Group, UIBasePanel i_Panel)
    {
        m_PanelsManager.SwitchPanels(i_Group, i_Panel);
    }

    public void SwitchPanels(UIGroup i_Group, UIBasePanel i_First, UIBasePanel i_Second)
    {
        m_PanelsManager.SwitchPanels(i_Group, i_First, i_Second);
    }

    public void SwitchPanels(UIGroup i_Group, UIBasePanel[] i_Panels)
    {
        m_PanelsManager.SwitchPanels(i_Group, i_Panels);
    }

    public void SequentialSwitchPanels(UIGroup i_Group, UIBasePanel i_Panel, Action i_Callback = null)
    {
        m_PanelsManager.SequentialSwitchPanels(i_Group, i_Panel, i_Callback);
    }

    public void SequentialSwitchPanels(UIGroup i_Group, UIBasePanel i_First, UIBasePanel i_Second, Action i_Callback = null)
    {
        m_PanelsManager.SequentialSwitchPanels(i_Group, i_First, i_Second, i_Callback);
    }

    public void SequentialSwitchPanels(UIGroup i_Group, UIBasePanel[] i_Panels, Action i_Callback = null)
    {
        m_PanelsManager.SequentialSwitchPanels(i_Group, i_Panels, i_Callback);
    }

    public void ClearGroup(UIGroup i_Group)
    {
        m_PanelsManager.ClearGroup(i_Group);
    }

    public void ClearAllGroups()
    {
        m_PanelsManager.ClearAll();
    }
}
