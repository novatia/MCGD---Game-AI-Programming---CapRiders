using UnityEngine;

using System.Collections.Generic;

public class UIIconsDatabaseManager : DatabaseManager<UIIconsDatabaseManager>
{
    private UIIconsDatabase m_Database = null;

    // STATIC METHODS

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static Sprite GetIconMain(string i_GroupId, string i_IconId)
    {
        if (Instance != null)
        {
            return Instance.GetIcon(i_GroupId, i_IconId);
        }

        return null;
    }

    // MonoBehaviour's interface

    protected override void OnAwake()
    {
        base.OnAwake();

        m_Database = Resources.Load<UIIconsDatabase>("Database/UI/IconsDatabase");
    }

    // LOGIC

    public void Initialize()
    {

    }

    public Sprite GetIcon(string i_GroupId, string i_IconId)
    {
        UIIconsGroup iconsGroup = m_Database.GetGroup(i_GroupId);
        if (iconsGroup != null)
        {
            Sprite icon = iconsGroup.GetIcon(i_IconId);
            return icon;
        }

        return null;
    }
}