using UnityEngine;

public class AdjustBoxColliderHelper : MonoBehaviour
{
    public enum Corner { topLeft, topRight, bottomLeft, bottomRight}

    [Header("A")]

    [SerializeField]
    private BoxCollider2D m_A = null;
    [SerializeField]
    private Corner m_CornerA = Corner.topLeft;

    [Header("A")]

    [SerializeField]
    private BoxCollider2D m_B = null;
    [SerializeField]
    private Corner m_CornerB = Corner.topLeft;

    // A

    public Vector2 positionA
    {
        get { return (m_A != null) ? m_A.transform.GetPositionXY() : Vector2.zero; }
    }

    public Vector2 sizeA
    {
        get { return (m_A != null) ? m_A.size : Vector2.zero; }
    }

    public Vector2 centerA
    {
        get { return (m_A != null) ? m_A.offset : Vector2.zero; }
    }

    public float rotationA
    {
        get
        {
            return (m_A != null) ? MathUtils.GetNormalizedAngle(m_A.transform.localEulerAngles.z) : 0f;
        }
    }

    public Vector2 centerPositionA
    {
        get
        {
            return positionA + centerA;
        }
    }

    // A

    public Vector2 sizeB
    {
        get { return (m_B != null) ? m_B.size : Vector2.zero; }
    }

    public Vector2 centerB
    {
        get { return (m_B != null) ? m_B.offset : Vector2.zero; }
    }

    public float rotationB
    {
        get
        {
            return (m_B != null) ? MathUtils.GetNormalizedAngle(m_B.transform.localEulerAngles.z) : 0f;
        }
    }

    // World A

    public Vector2 worldTopRightA
    {
        get
        {
            Vector2 offset = localTopRightA - centerA;
            offset = offset.Rotate(rotationA);
            return positionA + offset;
        }
    }

    public Vector2 worldBottomLeftA
    {
        get
        {
            Vector2 offset = localBottomLeftA - centerA;
            offset = offset.Rotate(rotationA);
            return positionA + offset;
        }
    }

    public Vector2 worldTopLeftA
    {
        get
        {
            return new Vector2(worldBottomLeftA.x, worldTopRightA.y);
        }
    }

    public Vector2 worldBottomRightA
    {
        get
        {
            return new Vector2(worldTopRightA.x, worldBottomLeftA.y);
        }
    }

    // Local A

    public Vector2 localTopRightA
    {
        get
        {
            return centerA + (sizeA / 2f);
        }
    }

    public Vector2 localBottomLeftA
    {
        get
        {
            return centerA - (sizeA / 2f);
        }
    }

    public Vector2 localTopLeftA
    {
        get
        {
            return new Vector2(localBottomLeftA.x, localTopRightA.y);
        }
    }

    public Vector2 localBottomRightA
    {
        get
        {
            return new Vector2(localTopRightA.x, localBottomLeftA.y);
        }
    }

    // Local B

    public Vector2 localTopRightB
    {
        get
        {
            return centerB + (sizeB / 2f);
        }
    }

    public Vector2 localBottomLeftB
    {
        get
        {
            return centerB - (sizeB / 2f);
        }
    }

    public Vector2 localTopLeftB
    {
        get
        {
            return new Vector2(localBottomLeftB.x, localTopRightB.y);
        }
    }

    public Vector2 localBottomRightB
    {
        get
        {
            return new Vector2(localTopRightB.x, localBottomLeftB.y);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Compute();
        }
    }

    public void Compute()
    {
        Vector2 targetPosition = InternalCompute();
        if(m_B != null)
        {
            m_B.transform.position = targetPosition;
        }

        Debug.Log(targetPosition);
    }

    public Vector2 InternalCompute()
    {
        Vector2 startCorner = Vector2.zero;

        switch (m_CornerA)
        {
            case Corner.topLeft:
                startCorner = worldTopLeftA;
                break;

            case Corner.topRight:
                startCorner = worldTopRightA;
                break;

            case Corner.bottomLeft:
                startCorner = worldBottomLeftA;
                break;

            case Corner.bottomRight:
                startCorner = worldBottomRightA;
                break;
        }

        Vector2 localEndCorner = Vector2.zero;

        switch (m_CornerB)
        {
            case Corner.topLeft:
                localEndCorner = localTopLeftB;
                break;

            case Corner.topRight:
                localEndCorner = localTopRightB;
                break;

            case Corner.bottomLeft:
                localEndCorner = localBottomLeftB;
                break;

            case Corner.bottomRight:
                localEndCorner = localBottomRightB;
                break;
        }

        localEndCorner = localEndCorner.Rotate(rotationB);
        Vector2 bPosition = startCorner - localEndCorner;
        return bPosition;
    }
}
