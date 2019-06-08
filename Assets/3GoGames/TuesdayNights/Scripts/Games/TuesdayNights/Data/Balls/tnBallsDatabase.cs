using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnBallDataEntry
{
    [SerializeField]
    private string m_Id = "";
    [SerializeField]
    private tnBallDataDescriptor m_Descriptor = null;

    public string id
    {
        get { return m_Id; }
    }

    public tnBallDataDescriptor descriptor
    {
        get { return m_Descriptor; }
    }
}

public class tnBallsDatabase : BaseScriptableObject
{
    [SerializeField]
    private ResourcePath m_BallPrefabPath = null;
    [SerializeField]
    private List<tnBallDataEntry> m_Balls = new List<tnBallDataEntry>();

    public string ballPrefabPath
    {
        get { return m_BallPrefabPath; }
    }

    public int ballsCount
    {
        get { return m_Balls.Count; }
    }

    public tnBallDataEntry GetBallDataEntry(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Balls.Count)
        {
            return null;
        }

        return m_Balls[i_Index];
    }
}
