using UnityEngine;

using System.Collections.Generic;

public class tnMatchData
{
    private List<GameObject> m_Characters = null;

    public int charactersCount
    {
        get
        {
            return m_Characters.Count;
        }
    }

    // LOGIC

    public void AddCharacter(GameObject i_Character)
    {
        m_Characters.Add(i_Character);
    }

    public GameObject GetCharacter(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Characters.Count)
        {
            return null;
        }

        return m_Characters[i_Index];
    }

    // VIRTUALS

    public virtual void Clear()
    {
        m_Characters.Clear();
    }

    // CTOR

    public tnMatchData()
    {
        m_Characters = new List<GameObject>();
    }
}
