using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class tnUIAnchorsSet : BaseBehavior
{
    [SerializeField]
    private List<RectTransform> m_Anchors = new List<RectTransform>();
    [SerializeField]
    private Color m_GizmosColor = Color.white;
    [SerializeField]
    private float m_GizmosRadius = 0.1f;
    [SerializeField]
    private bool m_DrawGizmos = false;
    [SerializeField]
    private bool m_DrawSegment = false;

    public int anchorsCount
    {
        get { return m_Anchors.Count; }
    }

    // MonoBehaviour's interface

    void OnDrawGizmos()
    {
        if (!m_DrawGizmos)
            return;

        Gizmos.color = m_GizmosColor;

        for (int index = 0; index < m_Anchors.Count; ++index)
        {
            RectTransform rt = m_Anchors[index];
            if (rt != null)
            {
                Gizmos.DrawSphere(rt.position, m_GizmosRadius);

                if (m_DrawSegment)
                {
                    if (index > 0)
                    {
                        RectTransform prevRt = m_Anchors[index - 1];
                        if (prevRt != null)
                        {
                            Gizmos.DrawLine(rt.position, prevRt.position);
                        }
                    }
                }
            }
        }
    }

    // LOGIC

    public RectTransform GetAnchorByIndex(int i_Index)
    {
       if (i_Index < 0 || i_Index >= m_Anchors.Count)
        {
            return null;
        }

        return m_Anchors[i_Index];
    }
}
