using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnGameModeDataDescriptor : BaseScriptableObject
{
    [SerializeField]
    private string m_GameModeName = "";

    [SerializeField]
    private string m_Description = "";

    [SerializeField]
    private ResourcePath m_CharacterPrefabPath = null;
    [SerializeField]
    private tnMatchController m_MatchController = null;

    [SerializeField]
    private IntRange m_TeamsRange = null;
    [SerializeField]
    private IntRange m_PlayersPerTeamRange = null;
    [SerializeField]
    private IntRange m_OnlinePlayersPerTeamRange = null;
    
    [SerializeField]
    private string m_OptionsConfigId = "";

    [SerializeField]
    private string m_CamerasSetId = "";

    [SerializeField]
    private List<string> m_FieldsExcludersTags = new List<string>();

    [SerializeField]
    private bool m_Hidden = false;

    public string gameModeName
    {
        get { return m_GameModeName; }
    }

    public string description
    {
        get { return m_Description; }
    }

    public string characterPrefabPath
    {
        get { return m_CharacterPrefabPath; }
    }

    public tnMatchController multiplayerController
    {
        get
        {
            return m_MatchController;
        }
    }

    public IntRange teamsRange
    {
        get { return m_TeamsRange; }
    }

    public IntRange playersPerTeamRange
    {
        get { return m_PlayersPerTeamRange; }
    }

    public IntRange onlinePlayersPerTeamRange
    {
        get
        {
            return m_OnlinePlayersPerTeamRange;
        }
    }

    public string optionsConfigId
    {
        get { return m_OptionsConfigId; }
    }

    public string camerasSetId
    {
        get { return m_CamerasSetId; }
    }

    public int fieldsExcludersTagsCount
    {
        get { return m_FieldsExcludersTags.Count; }
    }

    public bool hidden
    {
        get { return m_Hidden; }
    }

    // LOGIC

    public string GetFieldExcluderTag(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_FieldsExcludersTags.Count)
        {
            return string.Empty;
        }

        return m_FieldsExcludersTags[i_Index];
    }
}
