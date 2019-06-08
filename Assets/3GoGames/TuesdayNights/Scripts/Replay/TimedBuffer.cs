public class TimedBuffer<T>
{
    private int m_Size = 0;

    private float[] m_Timestamps = null;
    private T[] m_Data = null;

    private int m_FirstFreeIndex = 0;

    public int capacity
    {
        get
        {
            return m_Size;
        }
    }

    public int count
    {
        get
        {
            return m_FirstFreeIndex;
        }
    }

    // LOGIC

    public void Push(float i_Timestamp, T i_Data)
    {
        if (!(m_FirstFreeIndex < m_Size))
            return;

        m_Timestamps[m_FirstFreeIndex] = i_Timestamp;
        m_Data[m_FirstFreeIndex] = i_Data;

        ++m_FirstFreeIndex;
    }

    public bool TryGetIndex(float i_Timestamp, out int o_Index, out float o_Timestamp)
    {
        o_Index = -1;
        o_Timestamp = 0f;

        if (count <= 0)
        {
            return false;
        }

        float first = m_Timestamps[0];
        float last = m_Timestamps[count - 1];

        if (i_Timestamp <= first)
        {
            o_Index = 0;
            o_Timestamp = first;

            return true;
        }
        
        if (i_Timestamp >= last)
        {
            o_Index = count - 1;
            o_Timestamp = last;

            return true;
        }

        int left = 0;
        int right = count - 1;

        while (left <= right)
        {
            int middleIndex = (left + right) / 2;

            float t = m_Timestamps[middleIndex];

            if (i_Timestamp >= t)
            {
                int next = middleIndex + 1;
                if (IsValidIndex(next))
                {
                    float nextT = m_Timestamps[next];
                    if (i_Timestamp < nextT)
                    {
                        o_Index = middleIndex;
                        o_Timestamp = t;

                        return true;
                    }
                }
                else
                {
                    o_Index = middleIndex;
                    o_Timestamp = t;

                    return true;
                }
            }

            if (i_Timestamp < t)
            {
                right = middleIndex - 1;
            }

            if (i_Timestamp > t)
            {
                left = middleIndex + 1;
            }
        }

        return false;
    }

    public bool TryGetData(int i_Index, out T o_Data, out float o_Timestamp)
    {
        o_Data = default(T);
        o_Timestamp = 0f;

        if (i_Index < 0 || i_Index >= count)
        {
            return false;
        }

        float t = m_Timestamps[i_Index];
        T data = m_Data[i_Index];

        o_Timestamp = t;
        o_Data = data;

        return true;
    }

    public T GetData(int i_Index, out float o_Timestamp)
    {
        o_Timestamp = m_Timestamps[i_Index];
        return m_Data[i_Index];
    }

    public void Clear()
    {
        m_FirstFreeIndex = 0;
    }

    // UTILS

    private bool IsValidIndex(int i_Index)
    {
        return !(i_Index < 0 || i_Index >= count);
    }

    // CTOR

    public TimedBuffer(int i_Size)
    {
        m_Size = i_Size;

        m_Timestamps = new float[i_Size];
        m_Data = new T[i_Size];
    }
}
