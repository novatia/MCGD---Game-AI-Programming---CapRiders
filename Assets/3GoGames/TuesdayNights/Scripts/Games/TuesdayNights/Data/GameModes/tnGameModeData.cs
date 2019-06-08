using UnityEngine;

using System;
using System.Collections.Generic;

[Serializable]
public class tnGameModeData
{
    private string m_Name = "";
    private string m_Descriptrion = "";

    private string m_CharacterPrefabPath = "";
    private tnMatchController m_MatchController = null;

    private IntRange m_TeamsRange = null;
    private IntRange m_PlayersPerTeamRange = null;
    private IntRange m_OnlinePlayersPerTeamRange = null;

    private int m_OptionsConfigId;

    private int m_CamerasSetId;

    private List<int> m_FieldsExcludersTags = null;

    private bool m_Hidden = false;

    // ACCESSORS

    public string name
    {
        get { return m_Name; }
    }

    public string description
    {
        get { return m_Descriptrion; }
    }

    public string characterPrefabPath
    {
        get { return m_CharacterPrefabPath; }
    }

    public tnMatchController matchController
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

    public int optionsConfigId
    {
        get { return m_OptionsConfigId; }
    }

    public int camerasSetId
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

    public int GetFieldExcluderTag(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_FieldsExcludersTags.Count)
        {
            return Hash.s_NULL;
        }

        return m_FieldsExcludersTags[i_Index];
    }

    public GameObject LoadAndGetCharacterPrefabPath()
    {
        GameObject characterPrefab = Resources.Load<GameObject>(m_CharacterPrefabPath);
        return characterPrefab;
    }

    // CTOR

    public tnGameModeData(tnGameModeDataDescriptor i_Descriptor)
    {
        m_FieldsExcludersTags = new List<int>();

        if (i_Descriptor != null)
        {
            m_Name = i_Descriptor.gameModeName;
            m_Descriptrion = i_Descriptor.description;

            m_CharacterPrefabPath = i_Descriptor.characterPrefabPath;
            m_MatchController = i_Descriptor.multiplayerController;

            m_TeamsRange = new IntRange(i_Descriptor.teamsRange.min, i_Descriptor.teamsRange.max);
            m_PlayersPerTeamRange = new IntRange(i_Descriptor.playersPerTeamRange.min, i_Descriptor.playersPerTeamRange.max);
            m_OnlinePlayersPerTeamRange = new IntRange(i_Descriptor.onlinePlayersPerTeamRange.min, i_Descriptor.playersPerTeamRange.max);

            int optionsConfigId = StringUtils.GetHashCode(i_Descriptor.optionsConfigId);
            m_OptionsConfigId = optionsConfigId;

            int camerasSetId = StringUtils.GetHashCode(i_Descriptor.camerasSetId);
            m_CamerasSetId = camerasSetId;

            for (int tagIndex = 0; tagIndex < i_Descriptor.fieldsExcludersTagsCount; ++tagIndex)
            {
                string tag = i_Descriptor.GetFieldExcluderTag(tagIndex);
                if (tag != "" && tag != "NULL")
                {
                    int hash = StringUtils.GetHashCode(tag);
                    m_FieldsExcludersTags.Add(hash);
                }
            }

            m_Hidden = i_Descriptor.hidden;
        }
    }
}
