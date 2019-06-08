using UnityEngine;
using System.Collections.Generic;

using FullInspector;

public class UserStatsDatabase : BaseScriptableObject
{
    [SerializeField]
    private List<UserStatDescriptor> m_List = new List<UserStatDescriptor>();

    public int statsCount
    {
        get { return m_List.Count; }
    }

    public UserStatDescriptor GetUserStat(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_List.Count)
        {
            return null;
        }

        return m_List[i_Index];
    }
}
