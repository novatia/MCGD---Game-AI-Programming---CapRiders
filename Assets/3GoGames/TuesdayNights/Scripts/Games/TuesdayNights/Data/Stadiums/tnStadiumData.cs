using UnityEngine;

using System.Collections.Generic;

public class tnStadiumData
{
    private string m_Name = "";

    private string m_Description = "";

    private string m_SceneName = "";
    private int m_CameraId;

    private string m_GoalPrefabPath = "";

    private Sprite m_Icon = null;

    private bool m_HiddenOnline = false;

    private IntRange m_TeamSize;
    private IntRange m_OnlineTeamSize;

    private List<int> m_Tags = null;

    private Dictionary<int, string> m_ScorePanelsPrefabsPaths = null;

    // ACCESSORS

    public string name
    {
        get { return m_Name; }
    }

    public string description
    {
        get { return m_Description; }
    }

    public string sceneName
    {
        get { return m_SceneName; }
    }

    public int cameraId
    {
        get { return m_CameraId; }
    }

    public string goalPrefabPath
    {
        get { return m_GoalPrefabPath; }
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

    public IntRange teamSize
    {
        get { return m_TeamSize; }
    }

    public IntRange onlineTeamSize
    {
        get
        {
            return m_OnlineTeamSize;
        }
    }

    // LOGIC

    public bool HasTag(string i_Tag)
    {
        int hash = StringUtils.GetHashCode(i_Tag);
        return HasTag(hash);
    }

    public bool HasTag(int i_Tag)
    {
        return m_Tags.Contains(i_Tag);
    }

    public tnGoal LoadAndGetGoalPrefab()
    {
        tnGoal prefab = Resources.Load<tnGoal>(m_GoalPrefabPath);
        return prefab;
    }

    public bool TryGetScorePanelPrefabPath(int i_Id, out string o_Path)
    {
        return m_ScorePanelsPrefabsPaths.TryGetValue(i_Id, out o_Path);
    }

    public GameObject LoadAndGetScorePanelPrefab(int i_Id)
    {
        string path = "";
        if (m_ScorePanelsPrefabsPaths.TryGetValue(i_Id, out path))
        {
            GameObject prefab = Resources.Load<GameObject>(path);
            return prefab;
        }

        return null;
    }

    // CTOR

    public tnStadiumData(tnStadiumDataDescriptor i_Descriptor)
    {
        m_Tags = new List<int>();

        m_ScorePanelsPrefabsPaths = new Dictionary<int, string>();

        if (i_Descriptor != null)
        {
            m_Name = i_Descriptor.stadiumName;

            m_Description = i_Descriptor.description;

            m_SceneName = i_Descriptor.sceneName;
            m_GoalPrefabPath = i_Descriptor.goalPrefabPath;

            m_Icon = i_Descriptor.icon;

            m_HiddenOnline = i_Descriptor.hiddenOnline;

            int cameraId = StringUtils.GetHashCode(i_Descriptor.cameraId);
            m_CameraId = cameraId;

            m_TeamSize = new IntRange(i_Descriptor.minTeamSize, i_Descriptor.maxTeamSize);
            m_OnlineTeamSize = new IntRange(i_Descriptor.minOnlineTeamSize, i_Descriptor.maxOnlineTeamSize);

            for (int tagIndex = 0; tagIndex < i_Descriptor.tagsCount; ++tagIndex)
            {
                string tag = i_Descriptor.GetTag(tagIndex);
                if (tag != "" && tag != "NULL")
                {
                    int hash = StringUtils.GetHashCode(tag);
                    m_Tags.Add(hash);
                }
            }

            foreach (string id in i_Descriptor.scorePanelsPrefabsPathsKeys)
            {
                string path = "";
                if (i_Descriptor.TryGetScorePanelPrefabPath(id, out path))
                {
                    int hash = StringUtils.GetHashCode(id);
                    m_ScorePanelsPrefabsPaths.Add(hash, path);
                }
            }
        }
    }
}
