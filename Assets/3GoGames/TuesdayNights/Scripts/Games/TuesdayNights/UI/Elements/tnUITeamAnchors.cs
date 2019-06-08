using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class tnUITeamAnchors : BaseBehavior
{
    [SerializeField]
    private List<tnUIAnchorsSet> m_AnchorsSets = new List<tnUIAnchorsSet>();

    public int anchorsSetsCount
    {
        get { return m_AnchorsSets.Count; }
    }

    public tnUIAnchorsSet GetAnchorsSetByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_AnchorsSets.Count)
        {
            return null;
        }

        return m_AnchorsSets[i_Index];
    }

    public tnUIAnchorsSet GetAnchorsSetBySize(int i_Size)
    {
        for (int setIndex = 0; setIndex < m_AnchorsSets.Count; ++setIndex)
        {
            tnUIAnchorsSet anchorsSet = m_AnchorsSets[setIndex];
            if (anchorsSet != null)
            {
                if (anchorsSet.anchorsCount == i_Size)
                {
                    return anchorsSet;
                }
            }
        }

        return null;
    }
}
