using System;
using System.Collections.Generic;

public delegate int Sorter<T>(T i_First, T i_Second);

public class SortableList<T>
{
    private List<T> m_List = null;
    private List<int> m_SortedIndexes = null;

    public int count
    {
        get { return m_List.Count; }
    }

    public void Clear()
    {
        m_List.Clear();
        m_SortedIndexes.Clear();
    }

    public void Add(T i_Element)
    {
        m_List.Add(i_Element);
        m_SortedIndexes.Add(m_List.Count - 1);
    }

    public void Remove(T i_Element)
    {
        for (int index = 0; index < m_List.Count; ++index)
        {
            T element = m_List[index];
            if (element.Equals(i_Element))
            {
                m_List.RemoveAt(index);
                m_SortedIndexes.Remove(index);
            }
        }
    }

    public void RemoveAt(int i_Index)
    {
        m_List.RemoveAt(i_Index);
        m_SortedIndexes.Remove(i_Index);
    }

    public T GetByIndex(int i_Index)
    {
        return m_List[i_Index];
    }

    public T GetByPosition(int i_Position)
    {
        int index = m_SortedIndexes[i_Position];
        return m_List[index];
    }

    public void Sort(Comparison<T> i_Comparison)
    {
        for (int i = 0; i < m_List.Count; ++i)
        {
            for (int j = i + 1; j < m_List.Count; ++j)
            {
                T a = m_List[i];
                T b = m_List[j];

                int result = i_Comparison(a, b);

                if (result > 0)
                {
                    int temp = m_SortedIndexes[i];
                    m_SortedIndexes[i] = m_SortedIndexes[j];
                    m_SortedIndexes[j] = temp;
                }
            }
        }
    }

    // CTOR

    public SortableList()
    {
        m_List = new List<T>();
        m_SortedIndexes = new List<int>();
    }
}
