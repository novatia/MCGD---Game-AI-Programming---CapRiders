using UnityEngine;

using TrueSync;

public class tnBallView : MonoBehaviour
{
    [SerializeField]
    private float m_TrailMinSpeed = 0.1f;
    [SerializeField]
    private float m_TrailMaxSpeed = 2.0f;

    [SerializeField]
    private float m_ParticlesMinSpeed = 1.0f;

    [SerializeField]
    private Color m_ColorMinSpeed = Color.white;
    [SerializeField]
    private Color m_ColorMaxSpeed = Color.white;

    private MeshRenderer m_MeshRenderer = null;

    private TrailRenderer m_Trail = null;
    private ParticleSystem m_Particles = null;

    private TSRigidBody2D m_Rididbody2D = null;

    private tnRespawn m_Respawn = null;

    // MonoBehaviour's interface

    void Awake()
    {
        m_MeshRenderer = GetComponentInChildren<MeshRenderer>();
        m_Trail = GetComponentInChildren<TrailRenderer>();

        m_Rididbody2D = GetComponent<TSRigidBody2D>();

        m_Respawn = GetComponent<tnRespawn>();
    }

    void OnEnable()
    {
        ForceStop();

        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccurred;
        }
    }

    void OnDisable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccurred;
        }
    }

    void Start()
    {
        ForceStop();
    }

    void Update()
    {
        // Update effects.

        FP speed2 = 0;

        if (m_Rididbody2D != null)
        {
            TSVector2 velocity = m_Rididbody2D.velocity;
            speed2 = velocity.LengthSquared();
        }

        float speedSqr = FP.ToFloat(speed2);

        UpdateTrailColor(speedSqr);
        UpdateParticleEffect(speedSqr);
    }

    // LOGIC

    public void SetTexture(Texture i_Texture)
    {
        if (m_MeshRenderer == null)
            return;

        Material material = m_MeshRenderer.material;

        if (material != null)
        {
            material.mainTexture = i_Texture;
        }
    }

    public void SetTrailMaterial(Material i_Material)
    {
        if (m_Trail == null)
            return;

        if (i_Material == null)
        {
            m_Trail.enabled = false;
        }
        else
        {
            m_Trail.sharedMaterial = i_Material;
        }
    }

    public void SetParticleEffect(ParticleSystem i_Effect)
    {
        if (i_Effect == null)
            return;

        ParticleSystem effectInstance = (ParticleSystem)Instantiate(i_Effect, Vector3.zero, Quaternion.identity);
        effectInstance.transform.SetParent(transform, false);

        m_Particles = effectInstance;
    }

    // EVENTS

    private void OnRespawnOccurred()
    {
        ForceStop();
    }

    // INTERNALS

    private void UpdateTrailColor(float i_Speed2)
    {
        if (m_Trail == null)
            return;

        float speedFactor = MathUtils.GetClampedPercentage(i_Speed2, m_TrailMinSpeed * m_TrailMinSpeed, m_TrailMaxSpeed * m_TrailMaxSpeed);
        Color color = Color.Lerp(m_ColorMinSpeed, m_ColorMaxSpeed, speedFactor);

        Material material = m_Trail.material;

        if (material != null)
        {
            material.SetColor("_TintColor", color);
        }
    }

    private void UpdateParticleEffect(FP i_Speed2)
    {
        if (m_Particles == null)
            return;

        ParticleSystem.EmissionModule particlesEmission = m_Particles.emission;
        particlesEmission.enabled = (i_Speed2 > m_ParticlesMinSpeed * m_ParticlesMinSpeed);
    }

    private void StopTrail()
    {
        if (m_Trail == null)
            return;

        m_Trail.Clear();
    }

    private void ForceStop()
    {
        // Refresh effects status.

        UpdateParticleEffect(0f);

        UpdateTrailColor(0f);
        StopTrail();
    }
}
