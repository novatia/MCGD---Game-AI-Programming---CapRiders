using System.Collections.Generic;

public abstract class tnGameModeOption<T>
{
    private Dictionary<int, T> m_Values = null;
    private List<int> m_Keys = null;

    // LOGIC

    public bool TryGetValue(int i_Id, out T o_Value)
    {
        return m_Values.TryGetValue(i_Id, out o_Value);    
    }

    public List<int> GetKeys()
    {
        List<int> list = new List<int>(m_Keys);
        return list;
    }

    // PROTECTED

    protected void InternalAdd(int i_Id, T i_Value)
    {
        m_Values.Add(i_Id, i_Value);
        m_Keys.Add(i_Id);
    }

    // CTOR

    public tnGameModeOption()
    {
        m_Values = new Dictionary<int, T>();
        m_Keys = new List<int>();
    }
}
