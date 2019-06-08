using UnityEngine;

using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public delegate void HoleOccuredCallback(Collider2D i_Entity);

public class tnCarambolaHole : MonoBehaviour
{
    [Serializable]
    public class RespawnPoint
    {
        [SerializeField]
        private Transform m_Transform = null;
        [SerializeField]
        private Vector2 m_ForceDirection = Vector2.zero;

        [SerializeField]
        private float m_ErrorAngle = 0f;

        [SerializeField]
        private float m_ForceIntensity = 0f;

        public Transform transform
        {
            get { return m_Transform; }
        }

        public Vector2 forceDirection
        {
            get { return m_ForceDirection; }
        }

        public float errorAngle
        {
            get { return m_ErrorAngle; }
        }

        public float forceIntensity
        {
            get { return m_ForceIntensity; }
        }
    }

    [SerializeField]
    private float m_Threshold = 0.25f;

    [SerializeField]
    private float m_RespawnTime = 2f;

    [SerializeField]
    private LayerMask m_LayerMask = 0;

    [SerializeField]
    private RespawnPoint[] m_RespawnPoints = null;

    [SerializeField]
    private Effect m_InEffect = null;
    [SerializeField]
    private Effect m_OutEffect = null;

    public HoleOccuredCallback onPreHoleOccuredEvent;
    public HoleOccuredCallback onPostHoleOccuredEvent;

    private tnCarambolaHolesManager m_Manager = null;

    private List<RespawnPoint> m_RespawnPointsCache = new List<RespawnPoint>();
    private List<Collider2D> m_TargetsCache = new List<Collider2D>();

    public float threshold
    {
        get
        {
            return m_Threshold;
        }
        set
        {
            m_Threshold = value;
        }
    }

    // MonoBehaviour's interface

    void OnEnable()
    {
        Clear();
    }

    void OnDisable()
    {
        Clear();
    }

    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, m_Threshold, m_LayerMask);

        if (colliders == null)
            return;

        for (int colliderIndex = 0; colliderIndex < colliders.Length; ++colliderIndex)
        {
            Collider2D collider = colliders[colliderIndex];

            if (m_Manager == null || !m_Manager.IsValidCollision(collider))
                continue;

            Vector2 colliderPosition = collider.transform.position;
            Vector2 myPosition = transform.position;

            Vector2 positionDelta = colliderPosition - myPosition;

            float distance2 = positionDelta.sqrMagnitude;

            if (distance2 < m_Threshold * m_Threshold)
            {
                RespawnPoint respawnPoint = GetRandomRespawnPoint();

                if (respawnPoint == null || respawnPoint.transform == null)
                    continue;

                Transform otherTransform = collider.transform;

                // Snap to my position.

                otherTransform.position = transform.position;

                // Make the object kinematic.

                Rigidbody2D rb2d = otherTransform.GetComponent<Rigidbody2D>();
                if (rb2d != null)
                {
                    rb2d.isKinematic = true;
                    rb2d.velocity = Vector2.zero;
                }

                RaisePreHoleOccuredEvent(collider);

                m_RespawnPointsCache.Add(respawnPoint);
                m_TargetsCache.Add(collider);

                EffectUtils.PlayEffect(m_InEffect, transform.position);

                if (m_RespawnTime == 0f)
                {
                    Teleport();
                }
                else
                {
                    Invoke("Teleport", m_RespawnTime);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        for (int respawnPointIndex = 0; respawnPointIndex < m_RespawnPoints.Length; ++respawnPointIndex)
        {
            RespawnPoint respawnPoint = m_RespawnPoints[respawnPointIndex];

            if (respawnPoint == null)
                continue;

            Vector2 normalizedDir = respawnPoint.forceDirection.normalized;

            // Draw main direction.

            {
                Gizmos.color = Color.yellow;

                Vector3 from = respawnPoint.transform.position;
                Vector3 to = from + new Vector3(normalizedDir.x, normalizedDir.y, 0f);
                Gizmos.DrawLine(from, to);
            }

            if (Mathf.Abs(respawnPoint.errorAngle) > Mathf.Epsilon)
            {
                // Draw error range.

                {
                    Gizmos.color = Color.red;

                    Vector2 normalizedDirLeft = normalizedDir.Rotate(-respawnPoint.errorAngle);

                    Vector3 from = respawnPoint.transform.position;
                    Vector3 to = from + new Vector3(normalizedDirLeft.x, normalizedDirLeft.y, 0f);
                    Gizmos.DrawLine(from, to);
                }

                {
                    Gizmos.color = Color.red;

                    Vector2 normalizedDirRight = normalizedDir.Rotate(respawnPoint.errorAngle);

                    Vector3 from = respawnPoint.transform.position;
                    Vector3 to = from + new Vector3(normalizedDirRight.x, normalizedDirRight.y, 0f);
                    Gizmos.DrawLine(from, to);
                }
            }
        }
    }

    // BUSINESS LOGIC

    public void SetManager(tnCarambolaHolesManager i_Manager)
    {
        m_Manager = i_Manager;
    }

    public bool IsCollidingWith(Collider2D i_Collider2D)
    {
        if (i_Collider2D == null)
        {
            return false;
        }

        Vector2 colliderPosition = i_Collider2D.transform.position;
        Vector2 myPosition = transform.position;

        Vector2 positionDelta = colliderPosition - myPosition;

        float distance2 = positionDelta.sqrMagnitude;

        return (distance2 < m_Threshold * m_Threshold);
    }

    public void Clear()
    {
        for (int targetIndex = 0; targetIndex < m_TargetsCache.Count; ++targetIndex)
        {
            Collider2D target = m_TargetsCache[targetIndex];

            if (target == null)
                continue;

            Rigidbody2D rb2d = target.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.isKinematic = false;
            }
        }

        m_TargetsCache.Clear();
        m_RespawnPointsCache.Clear();
    }

    // INTERNALS

    private RespawnPoint GetRandomRespawnPoint()
    {
        if (m_RespawnPoints == null  || m_RespawnPoints.Length == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, m_RespawnPoints.Length);
        return m_RespawnPoints[randomIndex]; 
    }

    private void RaisePreHoleOccuredEvent(Collider2D i_Entity)
    {
        if (i_Entity == null)
            return;

        if (onPreHoleOccuredEvent != null)
        {
            onPreHoleOccuredEvent(i_Entity);
        }
    }

    private void RaisePostHoleOccuredEvent(Collider2D i_Entity)
    {
        if (i_Entity == null)
            return;

        if (onPostHoleOccuredEvent != null)
        {
            onPostHoleOccuredEvent(i_Entity);
        }
    }

    // EVENTS

    private void Teleport()
    {
        if (m_TargetsCache.Count < 1 || m_RespawnPointsCache.Count < 1)
            return;

        Collider2D target = m_TargetsCache[0];
        m_TargetsCache.RemoveAt(0);

        RespawnPoint respawnPoint = m_RespawnPointsCache[0];
        m_RespawnPointsCache.RemoveAt(0);

        if (target != null && respawnPoint != null && respawnPoint.transform != null)
        {
            target.transform.position = respawnPoint.transform.position;

            Rigidbody2D rb2d = target.GetComponent<Rigidbody2D>();
            if (rb2d != null)
            {
                rb2d.isKinematic = false;
                rb2d.velocity = Vector2.zero;

                Vector2 forceDirection = respawnPoint.forceDirection;
                forceDirection.Normalize();

                if (Mathf.Abs(respawnPoint.errorAngle) > Mathf.Epsilon)
                {
                    float randomError = Random.Range(-respawnPoint.errorAngle, respawnPoint.errorAngle);
                    forceDirection = forceDirection.Rotate(randomError);
                }

                Vector2 outForce = forceDirection * respawnPoint.forceIntensity;
                rb2d.AddForce(outForce);
            }

            EffectUtils.PlayEffect(m_OutEffect, respawnPoint.transform.position);
            RaisePostHoleOccuredEvent(target);
        }
    }
}
