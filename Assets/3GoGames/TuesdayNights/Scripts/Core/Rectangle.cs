using UnityEngine;
using System.Collections;

public class Rectangle
{
    private float m_Top;
    private float m_Left;

    private float m_Width;
    private float m_Height;

    public Vector2 center
    {
        get { return new Vector2((m_Left + m_Width / 2f), (m_Top - m_Height / 2f)); }
    }

    public float top
    {
        get { return m_Top; }
    }

    public float left
    {
        get { return m_Left; }
    }

    public float right
    {
        get { return m_Left + m_Width; }
    }

    public float bottom
    {
        get { return m_Top - m_Height; }
    }

    public float width
    {
        get { return m_Width; }
    }

    public float height
    {
        get { return m_Height; }
    }

    public float area
    {
        get { return m_Width * m_Height; }
    }

    public bool Contains(Vector2 i_Point)
    {
        return !(i_Point.x < m_Left || i_Point.x > right) && !(i_Point.y > m_Top || i_Point.y < bottom);
    }

    public bool Contains(Rectangle i_Rectangle)
    {
        Vector2 upperLeft = new Vector2(i_Rectangle.left, i_Rectangle.top);
        Vector2 bottomRight = new Vector2(i_Rectangle.right, i_Rectangle.bottom);

        return Contains(upperLeft) && Contains(bottomRight);
    }

    public Rectangle(float i_Top, float i_Left, float i_Width, float i_Height)
    {
        m_Top = i_Top;
        m_Left = i_Left;
        m_Width = i_Width;
        m_Height = i_Height;
    }
}
