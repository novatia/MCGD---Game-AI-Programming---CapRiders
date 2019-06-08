using System.Collections.Generic;

public class DictionaryList<V, T> where T : class
{
    private Dictionary<V, T> m_Dictionary = null;
    private List<V> m_List = null;

    // GETTERS

    public T this[V i_Id]
    {
        get
        {
            T value;
            TryGetValue(i_Id, out value);
            return value;
        }
    }

    public int count
    {
        get
        {
            return m_List.Count;
        }
    }

    // LOGIC

    public bool Add(V i_Key, T i_Value)
    {
        if (!m_Dictionary.ContainsKey(i_Key))
        {
            m_Dictionary.Add(i_Key, i_Value);
            m_List.Add(i_Key);

            return true;
        }

        return false;
    }

    public bool Remove(V i_Key)
    {
        T item = null;
        if (m_Dictionary.TryGetValue(i_Key, out item))
        {
            m_Dictionary.Remove(i_Key);
            m_List.Remove(i_Key);

            return true;
        }

        return false;
    }

    public void Clear()
    {
        m_Dictionary.Clear();
        m_List.Clear();
    }

    public T GetValue(V i_Key)
    {
        T value;
        TryGetValue(i_Key, out value);
        return value;
    }

    public bool TryGetValue(V i_Key, out T o_Value)
    {
        return m_Dictionary.TryGetValue(i_Key, out o_Value);
    }

    public T GetItem(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_List.Count)
        {
            return null;
        }

        V id = m_List[i_Index];
        return m_Dictionary[id];
    }

    public V GetKey(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_List.Count)
        {
            return default(V);
        }

        return m_List[i_Index];
    }

    public bool ContainsKey(V i_Key)
    {
        return m_Dictionary.ContainsKey(i_Key);
    }

    public bool ContainsValue(T i_Value)
    {
        return m_Dictionary.ContainsValue(i_Value);
    }

    // CTOR

    public DictionaryList()
    {
        m_Dictionary = new Dictionary<V, T>();
        m_List = new List<V>();
    }
}
