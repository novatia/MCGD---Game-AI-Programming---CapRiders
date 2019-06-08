using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class tnSupporterArea : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1000)]
    private int m_MaxSupporters = 100;

    [SerializeField]
    private bool m_UseStaticResources = true;

    [SerializeField]
    [Range(0f, 1f)]
    private float m_MaxAnimators = 0.5f;

    private Vector2 m_BoundsMin = Vector2.zero;
    private Vector2 m_BoundsMax = Vector2.zero;

    private Vector2[] m_Points = null;
    private int m_NumPoints = 0;

    private Collider2D m_Collider = null;

    void Awake()
    {
        // Setup BoxCollider.

        m_Collider = GetComponent<Collider2D>();
        m_Collider.isTrigger = true;

        // Get bounds.

        m_BoundsMin = m_Collider.bounds.min;
        m_BoundsMax = m_Collider.bounds.max;

        // Compute points.

        {
            m_Points = new Vector2[m_MaxSupporters];

            int nextPointIndex = 0;

            for (int index = 0; index < m_MaxSupporters; ++index)
            {
                float pointX;
                float pointY;
                if (TryGetRandomPoint(out pointX, out pointY))
                {
                    m_Points[nextPointIndex] = new Vector2(pointX, pointY);
                    ++nextPointIndex;
                }
            }

            m_NumPoints = nextPointIndex;
        }

        // Disable collider.

        m_Collider.enabled = false;
    }

    // BUSINESS LOGIC

    public Vector2 boundsMin
    {
        get
        {
            return m_BoundsMin;
        }
    }

    public Vector2 boundsMax
    {
        get
        {
            return m_BoundsMax;
        }
    }

    public int maxSupporters
    {
        get
        {
            return m_MaxSupporters;
        }
    }

    public int maxAnimators
    {
        get
        {
            if (m_UseStaticResources)
            {
                return (int)(m_MaxAnimators * m_MaxSupporters);
            }
            else
            {
                return m_MaxSupporters; // Allow any number of animators.
            }
        }
    }

    public int numPoints
    {
        get
        {
            return m_NumPoints;
        }
    }

    public Vector2 GetPoint(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_NumPoints)
        {
            return Vector2.zero;
        }

        return m_Points[i_Index];
    }

    // INERNALS

    private bool TryGetRandomPoint(out float o_PointX, out float o_PointY)
    {
        o_PointX = 0f;
        o_PointY = 0f;

        bool validPoint = false;

        int attempt = 0;

        do
        {
            attempt++;

            float x = Random.Range(m_BoundsMin.x, m_BoundsMax.x);
            float y = Random.Range(m_BoundsMin.y, m_BoundsMax.y);

            Vector2 p = new Vector2(x, y);

            if (m_Collider.OverlapPoint(p))
            {
                validPoint = true;

                o_PointX = x;
                o_PointY = y;
            }
        } while (!validPoint && attempt <= 100);

        return validPoint;
    }
}
