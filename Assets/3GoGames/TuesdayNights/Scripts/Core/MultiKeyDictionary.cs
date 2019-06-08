using System.Collections.Generic;

public class MultiKeyDictionary<K1, K2, T>
{
    private Dictionary<K1, Dictionary<K2, T>> m_Dictionary = null;
    
    // LOGIC

    public void Clear()
    {
        foreach (K1 key in m_Dictionary.Keys)
        {
            Dictionary<K2, T> d = null;
            if (m_Dictionary.TryGetValue(key, out d))
            {
                d.Clear();
            }
        }

        m_Dictionary.Clear();
    }

    public void Add(K1 i_Key1, K2 i_Key2, T i_Value)
    {
        Dictionary<K2, T> d = null;
        if (m_Dictionary.TryGetValue(i_Key1, out d))
        {
            d.Add(i_Key2, i_Value);
        }
        else
        {
            d = new Dictionary<K2, T>();
            d.Add(i_Key2, i_Value);
            m_Dictionary.Add(i_Key1, d);
        }
    }

    public bool Remove(K1 i_Key1, K2 i_Key2)
    {
        Dictionary<K2, T> d = null;
        if (m_Dictionary.TryGetValue(i_Key1, out d))
        {
            return d.Remove(i_Key2);
        }

        return false;
    }

    public bool TryGetValue(K1 i_Key1, K2 i_Key2, out T o_Value)
    {
        o_Value = default(T);

        Dictionary<K2, T> d = null;
        if (m_Dictionary.TryGetValue(i_Key1, out d))
        {
            return d.TryGetValue(i_Key2, out o_Value);
        }

        return false;
    }

    // CTOR

    public MultiKeyDictionary()
    {
        m_Dictionary = new Dictionary<K1, Dictionary<K2, T>>();
    }
}
