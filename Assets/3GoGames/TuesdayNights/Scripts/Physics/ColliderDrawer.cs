using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class ColliderDrawer : MonoBehaviour
{
    [SerializeField]
    private bool m_DrawGizmos = false;
    [SerializeField]
    private Color m_Color = Color.white;

    private Collider2D m_Collider = null;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_Collider = GetComponent<Collider2D>();
    }

    void OnDrawGizmos()
    {
        if (!m_DrawGizmos)
            return;

        if (m_Collider is CircleCollider2D)
        {
            CircleCollider2D circleCollider2D = (CircleCollider2D)m_Collider;

            Gizmos.color = m_Color;
            Gizmos.DrawWireSphere(circleCollider2D.transform.position, circleCollider2D.radius);
        }
        else
        {
            // TODO: Handle other shapes.
        }
    }
}
