using UnityEngine;

using FullInspector;

public class tnBallDataDescriptor : BaseScriptableObject
{
    [SerializeField]
    private string m_BallName = "";
    [SerializeField]
    private Texture m_Texture = null;
    [SerializeField]
    private Material m_TrailMaterial = null;
    [SerializeField]
    private ParticleSystem m_ParticleEffect = null;
    [SerializeField]
    private Sprite m_Icon = null;
    [SerializeField]
    private bool m_CanRotate = true;

    public string ballName
    {
        get { return m_BallName; }
    }

    public Texture texture
    {
        get { return m_Texture; }
    }

    public Material trailMaterial
    {
        get { return m_TrailMaterial; }
    }

    public ParticleSystem particleEffect
    {
        get { return m_ParticleEffect; }
    }

    public Sprite icon
    {
        get { return m_Icon; }
    }

    public bool canRotate
    {
        get { return m_CanRotate; }
    }
}
