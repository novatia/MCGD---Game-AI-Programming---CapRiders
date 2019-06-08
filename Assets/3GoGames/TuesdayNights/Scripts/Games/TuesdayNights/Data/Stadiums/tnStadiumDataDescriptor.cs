using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class tnStadiumDataDescriptor : BaseScriptableObject
{
    [SerializeField]
    private string m_StadiumName = "";

    [SerializeField]
    [InspectorTextArea]
    [Multiline]
    private string m_Description = "";

    [SerializeField]
    private string m_SceneName = "";

    [SerializeField]
    private string m_CameraId = "";

    [SerializeField]
    private ResourcePath m_GoalPrefab = null;

    [SerializeField]
    private Sprite m_Icon = null;

    [SerializeField]
    private int m_MinTeamSize = 1;
    [SerializeField]
    private int m_MaxTeamSize = 5;

    [SerializeField]
    private bool m_HiddenOnline = false;

    [SerializeField]
    private int m_MinOnlineTeamSize = 1;
    [SerializeField]
    private int m_MaxOnlineTeamSize = 5;

    [SerializeField]
    private List<string> m_Tags = new List<string>();

    [SerializeField]
    private Dictionary<string, ResourcePath> m_ScorePanelsPrefabsPaths = new Dictionary<string, ResourcePath>();

    public string stadiumName
    {
        get { return m_StadiumName; }
    }

    public string description
    {
        get { return m_Description; }
    }

    public string sceneName
    {
        get { return m_SceneName; }
    }

    public string cameraId
    {
        get { return m_CameraId; }
    }

    public string goalPrefabPath
    {
        get { return m_GoalPrefab; }
    }

    public Sprite icon
    {
        get { return m_Icon; }
    }

    public bool hiddenOnline
    {
        get
        {
            return m_HiddenOnline;
        }
    }

    public int minTeamSize
    {
        get { return m_MinTeamSize; }
    }

    public int maxTeamSize
    {
        get { return m_MaxTeamSize; }
    }

    public int minOnlineTeamSize
    {
        get
        {
            return m_MinOnlineTeamSize;
        }
    }

    public int maxOnlineTeamSize
    {
        get
        {
            return m_MaxOnlineTeamSize;
        }
    }

    public int tagsCount
    {
        get { return m_Tags.Count; }
    }

    public string GetTag(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Tags.Count)
        {
           return string.Empty;
        }

        return m_Tags[i_Index];
    }

    public Dictionary<string, ResourcePath>.KeyCollection scorePanelsPrefabsPathsKeys
    {
        get
        {
            return m_ScorePanelsPrefabsPaths.Keys;
        }
    }

    public bool TryGetScorePanelPrefabPath(string i_Id, out string o_Path)
    {
        o_Path = "";

        ResourcePath path = null;
        if (m_ScorePanelsPrefabsPaths.TryGetValue(i_Id, out path))
        {
            o_Path = path;
            return true;
        }

        return false;
    }
}
