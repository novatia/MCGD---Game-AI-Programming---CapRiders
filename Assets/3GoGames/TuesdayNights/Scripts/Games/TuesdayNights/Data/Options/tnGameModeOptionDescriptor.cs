using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public abstract class tnGameModeOptionDescriptor<T> : BaseScriptableObject
{
    [SerializeField]
    private Dictionary<string, T> m_Values = new Dictionary<string, T>();

    public int count
    {
        get { return m_Values.Count; }
    }

    public Dictionary<string, T>.KeyCollection keys
    {
        get { return m_Values.Keys; }
    }

    public bool TryGetValue(string i_Key, out T o_Value)
    {
        return m_Values.TryGetValue(i_Key, out o_Value);
    }
}
