using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Popup : Singleton<Popup>
{
    private List<PopupPanel> m_Panels = new List<PopupPanel>();

    // STATIC METHODS

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Init();
        }
    }

    public static void RegisterPanelMain(PopupPanel i_Panel)
    {
        if (Instance != null)
        {
            Instance.RegisterPanel(i_Panel);
        }
    }

    public static void UnregisterPanelMain(PopupPanel i_Panel)
    {
        if (Instance != null)
        {
            Instance.UnregisterPanel(i_Panel);
        }
    }

    public static void ShowMessageMain(string i_Message)
    {
        if (Instance != null)
        {
            Instance.ShowMessage(i_Message);
        }
    }

    // BUSINESS LOGIC

    public void Init()
    {

    }

    public void RegisterPanel(PopupPanel i_Panel)
    {
        m_Panels.Add(i_Panel);
    }

    public void UnregisterPanel(PopupPanel i_Panel)
    {
        m_Panels.Remove(i_Panel);
    }

    public void ShowMessage(string i_Message)
    {
        for (int panelIndex = 0; panelIndex < m_Panels.Count; ++panelIndex)
        {
            PopupPanel popupPanel = m_Panels[panelIndex];
            if (popupPanel != null)
            {
                popupPanel.ShowMessage(i_Message);
            }
        }
    }
}