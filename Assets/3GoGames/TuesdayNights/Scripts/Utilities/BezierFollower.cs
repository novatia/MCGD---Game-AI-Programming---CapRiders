using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

public class BezierFollower : MonoBehaviour 
{
    public Transform nodeA = null;
    public Transform handleA = null;

    public Transform nodeB = null;
    public Transform handleB = null;

    private float m_Time = 0f;

    public float position
    {
        get
        {
            return m_Time;
        }

        set
        {
            m_Time = Mathf.Clamp01(value);
        }
    }

	void Update() 
    {
        if (nodeA == null || handleA == null)
            return;

        if (nodeB == null || handleB == null)
            return;

        Vector3 p0 = nodeA.position;
        Vector3 p3 = nodeB.position;
        
        Vector3 p1 = handleA.position;
        Vector3 p2 = handleB.position;

        float t = m_Time;
        float t2 = t * t;
        float t3 = t2 * t;

        float u = (1 - t);
        float u2 = u * u;
        float u3 = u2 * u;

        Vector3 pos = p0 * u3 + 3f * p1 * t * u2 + 2f * p2 * t2 * u + p3 * t3;

        transform.position = pos;
	}

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        if (nodeA == null || handleA == null)
            return;

        if (nodeB == null || handleB == null)
            return;

        Vector3 p0 = nodeA.position;
        Vector3 p3 = nodeB.position;

        Vector3 p1 = handleA.position;
        Vector3 p2 = handleB.position;

        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p2, p3);

        Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, HandleUtility.GetHandleSize(Vector3.zero) * 0.25f);
    }

#endif // UNITY_EDITOR
}
