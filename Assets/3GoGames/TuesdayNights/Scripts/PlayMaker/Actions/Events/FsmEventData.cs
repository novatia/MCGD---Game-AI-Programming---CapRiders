using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System;

public class PlayMakerEventParams : IEnumerable, IEnumerable<KeyValuePair<int, object>>
{
    private Dictionary<int, object> m_Params = null;

    // PUBLIC INTERFACE

    public void AddValue(int i_Key, object i_Value)
    {
        m_Params.Add(i_Key, i_Value);
    }

    public bool ContainsKey(int i_Key)
    {
        return m_Params.ContainsKey(i_Key);
    }

    public bool TryGetValue(int i_Key, out object o_Value)
    {
        return m_Params.TryGetValue(i_Key, out o_Value);
    }

    public object GetValue(int i_Key)
    {
        return m_Params[i_Key];
    }

    // IEnumerable INTERFACE

    public IEnumerator<KeyValuePair<int, object>> GetEnumerator()
    {
        return m_Params.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // CTOR

    public PlayMakerEventParams()
    {
        m_Params = new Dictionary<int, object>();
    }
}

public static class PlayMakerEventData
{
    private static Dictionary<int, object> m_Params = new Dictionary<int, object>();

    // BUSINESS LOGIC

    public static bool ContainsKey(int i_Key)
    {
        return m_Params.ContainsKey(i_Key);
    }

    public static bool TryGetValue(int i_Key, out object o_Value)
    {
        return m_Params.TryGetValue(i_Key, out o_Value);
    }
    
    public static object GetValue(int i_Key)
    {
        return m_Params[i_Key];
    }

    public static void SetValues(PlayMakerEventParams i_Params)
    {
        if (i_Params == null)
            return;

        m_Params.Clear();

        foreach (KeyValuePair<int, object> p in i_Params)
        {
            AddValue(p.Key, p.Value);
        }
    }

    // INTERNALS

    private static void AddValue(int i_Key, object i_Value)
    {
        m_Params.Add(i_Key, i_Value);
    }

    private static void Clear()
    {
        m_Params.Clear();
    }
}
