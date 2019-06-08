using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class BezierSpline : MonoBehaviour
{
    [SerializeField]
    private Transform m_NodeA = null;
    [SerializeField]
    private Transform m_HandleA = null;

    [SerializeField]
    private Transform m_NodeB = null;
    [SerializeField]
    private Transform m_HandleB = null;

    // BUSINESS LOGIC

    public bool GetDataAtTime(float i_Time, out Vector3 o_Position, out Vector3 o_Tangent)
    {
        if (m_NodeA == null || m_HandleA == null)
        {
            o_Position = Vector3.zero;
            o_Tangent = Vector3.forward;

            return false;
        }

        if (m_NodeB == null || m_HandleB == null)
        {
            o_Position = Vector3.zero;
            o_Tangent = Vector3.forward;

            return false;
        }

        Vector3 p0 = m_NodeA.position;
        Vector3 p3 = m_NodeB.position;

        Vector3 p1 = m_HandleA.position;
        Vector3 p2 = m_HandleB.position;

        float t = i_Time;
        float t2 = t * t;
        float t3 = t2 * t;

        float u = (1 - t);
        float u2 = u * u;
        float u3 = u2 * u;

        Vector3 pos = p0 * u3 + 3f * p1 * t * u2 + 2f * p2 * t2 * u + p3 * t3;
        o_Position = pos;

        Vector3 tangent = 3f * u2 * (p1 - p0) + 6f * u * t * (p2 - p1) + 3f * t2 * (p3 - p2);
        o_Tangent = tangent;

        return true;
    }

    public bool GetPositionAtTime(float i_Time, out Vector3 o_Position)
    {
        if (m_NodeA == null || m_HandleA == null)
        {
            o_Position = Vector3.zero;
            return false;
        }

        if (m_NodeB == null || m_HandleB == null)
        {
            o_Position = Vector3.zero;
            return false;
        }

        Vector3 p0 = m_NodeA.position;
        Vector3 p3 = m_NodeB.position;

        Vector3 p1 = m_HandleA.position;
        Vector3 p2 = m_HandleB.position;

        float t = i_Time;
        float t2 = t * t;
        float t3 = t2 * t;

        float u = (1 - t);
        float u2 = u * u;
        float u3 = u2 * u;

        Vector3 pos = p0 * u3 + 3f * p1 * t * u2 + 2f * p2 * t2 * u + p3 * t3;
        o_Position = pos;

        return true;
    }

    public bool GetTangentAtTime(float i_Time, out Vector3 o_Tangent)
    {
        if (m_NodeA == null || m_HandleA == null)
        {
            o_Tangent = Vector3.forward;
            return false;
        }

        if (m_NodeB == null || m_HandleB == null)
        {
            o_Tangent = Vector3.forward;
            return false;
        }

        Vector3 p0 = m_NodeA.position;
        Vector3 p3 = m_NodeB.position;

        Vector3 p1 = m_HandleA.position;
        Vector3 p2 = m_HandleB.position;

        float t = i_Time;
        float t2 = t * t;

        float u = (1 - t);
        float u2 = u * u;

        Vector3 tangent = 3f * u2 * (p1 - p0) + 6f * u * t * (p2 - p1) + 3f * t2 * (p3 - p2);
        o_Tangent = tangent;

        return true;
    }

    // INTERNALS

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if (m_NodeA == null || m_HandleA == null)
            return;

        if (m_NodeB == null || m_HandleB == null)
            return;

        Vector3 p0 = m_NodeA.position;
        Vector3 p3 = m_NodeB.position;

        Vector3 p1 = m_HandleA.position;
        Vector3 p2 = m_HandleB.position;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p2, p3);

        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, HandleUtility.GetHandleSize(Vector3.zero) * 0.25f);
    }

#endif // UNITY_EDITOR
}
