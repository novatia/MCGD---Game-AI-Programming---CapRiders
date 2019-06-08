using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class UIIconsGroup
{
    [SerializeField]
    private Dictionary<string, Sprite> m_Icons = new Dictionary<string, Sprite>();

    public Sprite GetIcon(string i_IconId)
    {
        Sprite icon = null;
        m_Icons.TryGetValue(i_IconId, out icon);
        return icon;
    }
}

public class UIIconsDatabase : BaseScriptableObject
{
    [SerializeField]
    private Dictionary<string, UIIconsGroup> m_Groups = new Dictionary<string, UIIconsGroup>();

    public UIIconsGroup GetGroup(string i_GroupId)
    {
        UIIconsGroup group = null;
        m_Groups.TryGetValue(i_GroupId, out group);
        return group;
    }
}
