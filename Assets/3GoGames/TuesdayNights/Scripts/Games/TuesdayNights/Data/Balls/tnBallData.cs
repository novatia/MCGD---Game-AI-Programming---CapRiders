using UnityEngine;

public class tnBallData
{
    private string m_Name = "";
    private Texture m_Texture = null;
    private Material m_TrailMaterial = null;
    private ParticleSystem m_ParticleEffect = null;
    private Sprite m_Icon = null;
    private bool m_CanRotate = true;

    public string name
    {
        get { return m_Name; }
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

    // CTOR

    public tnBallData(tnBallDataDescriptor i_Descriptor)
    {
        if (i_Descriptor != null)
        {
            m_Name = i_Descriptor.ballName;
            m_Texture = i_Descriptor.texture;
            m_TrailMaterial = i_Descriptor.trailMaterial;
            m_ParticleEffect = i_Descriptor.particleEffect;
            m_Icon = i_Descriptor.icon;
            m_CanRotate = i_Descriptor.canRotate;
        }
    }
}

