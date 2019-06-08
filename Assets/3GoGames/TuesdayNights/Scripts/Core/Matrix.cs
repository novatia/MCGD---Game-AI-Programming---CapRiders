public class Matrix<T>
{
    private T[] m_Values = null;

    private int m_Rows;
    private int m_Columns;

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= m_Rows * m_Columns)
                return default(T);

            return m_Values[index];
        }

        set
        {
            if (index < 0 || index >= m_Rows * m_Columns)
                return;

            m_Values[index] = value;
        }
    }

    public T this[int row, int column]
    {
        get
        {
            if (!(row < m_Rows) || !(row >= 0))
                return default(T);

            if (!(column < m_Columns) || !(column >= 0))
                return default(T);

            return m_Values[row * m_Columns + column];
        }

        set
        {
            if (!(row < m_Rows) || !(row >= 0))
                return;

            if (!(column < m_Columns) || !(column >= 0))
                return;

            m_Values[row * m_Columns + column] = value;
        }
    }

    public int Rows
    {
        get { return m_Rows; }
    }

    public int Columns
    {
        get { return m_Columns; }
    }

    public int Size
    {
        get { return m_Rows * m_Columns; }
    }

    // LOGIC

    public void Reset(T i_Value = default(T))
    {
        for (int index = 0; index < m_Values.Length; ++index)
        {
            m_Values[index] = i_Value;
        }
    }

    // CTOR

    public Matrix(int i_Rows, int i_Columns)
    {
        m_Values = new T[i_Rows * i_Columns];

        m_Rows = i_Rows;
        m_Columns = i_Columns;
    }
}
