using UnityEngine;
using System.Collections;

public class tnDepth2d : MonoBehaviour
{
    private static float s_GlobalScale = 1f;

    [SerializeField]
    private float m_Scale = 1f;
    [SerializeField]
    private float m_Offset = 0f;

    // BUSINESS LOGIC

    public void SetScale(float i_Scale)
    {
        m_Scale = i_Scale;
    }

    public void SetOffset(float i_Offset)
    {
        m_Offset = i_Offset;
    }

    // MonoBehaviour's INTERFACE

    void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        transform.position = new Vector3(x, y, m_Offset + y * (m_Scale * s_GlobalScale));
    }
}