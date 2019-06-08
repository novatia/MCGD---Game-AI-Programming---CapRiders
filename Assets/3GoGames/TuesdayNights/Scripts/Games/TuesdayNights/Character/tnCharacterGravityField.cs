using UnityEngine;

public class tnCharacterGravityField : MonoBehaviour
{
    // Fields

    [SerializeField]
    private LayerMask m_LayerMask = 0;

    [SerializeField]
    private float m_GravityStartMinDistance = 0.75f;
    [SerializeField]
    private float m_GravityStartMaxDistance = 0.85f;

    [SerializeField]
    private float m_GravityEndMinDistance = 1.15f;
    [SerializeField]
    private float m_GravityEndMaxDistance = 1.25f;

    [SerializeField]
    private float m_GravityIntensity = 300f;

    [SerializeField]
    private bool m_DrawGizmos = false;

    private Vector3 m_TargetCache = Vector3.zero;
    private Vector3 m_ForceCache = Vector3.zero;

    // MonoBehaviour's interface

    void OnDrawGizmos()
    {
        if (!m_DrawGizmos)
            return;

        Vector3 center = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, m_GravityStartMinDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, m_GravityStartMaxDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, m_GravityEndMinDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, m_GravityEndMaxDistance);

        // Draw force cache.

        Gizmos.color = Color.white;

        Vector3 from = m_TargetCache;
        Vector3 to = from + m_ForceCache * 0.05f;

        Gizmos.DrawLine(from, to);
    }

    void FixedUpdate()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, m_GravityEndMaxDistance, m_LayerMask);

        if (collider == null)
            return;

        Rigidbody2D rigidbody = collider.GetComponent<Rigidbody2D>();

        if (rigidbody == null)
        {
            m_TargetCache = Vector3.zero;
            m_ForceCache = Vector3.zero;
        }
        else
        {
            float distance = Vector2.Distance(transform.position, collider.transform.position);

            float fT = MathUtils.InterpolateBetweenThresholds(distance, m_GravityStartMinDistance, m_GravityStartMaxDistance, m_GravityEndMinDistance, m_GravityEndMaxDistance);
            float intensity = Mathf.Lerp(0f, m_GravityIntensity, fT);

            Vector2 direction = collider.transform.position - transform.position;
            direction.Normalize();

            Vector2 force = direction * (-intensity);
            rigidbody.AddForce(force);

            m_TargetCache = collider.transform.position;
            m_ForceCache = force;
        }
    }
}
