using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class UIPageDescriptor : BaseBehavior
{
    [SerializeField]
    private Dictionary<string, GameObject> m_Widgets = new Dictionary<string, GameObject>();

    // LOGIC

    public GameObject GetWidget(string i_WidgetId)
    {
        GameObject widget = null;
        m_Widgets.TryGetValue(i_WidgetId, out widget);
        return widget;
    }

    public T GetWidget<T>(string i_Id) where T : Component
    {
        GameObject widgetGo = GetWidget(i_Id);
        if (widgetGo != null)
        {
            return widgetGo.GetComponent<T>();
        }

        return null;
    }
}
