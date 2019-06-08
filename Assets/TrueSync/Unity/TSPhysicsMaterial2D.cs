using UnityEngine;

using TrueSync;

public class TSPhysicsMaterial2D : ScriptableObject
{
    [SerializeField]
    private FP m_Friction = FP.Zero;

    public FP friction
    {
        get
        {
            return m_Friction;
        }

        set
        {
            m_Friction = value;
        }
    }

    [SerializeField]
    private FP m_Restitution = FP.Zero;

    public FP restitution
    {
        get
        {
            return m_Restitution;
        }

        set
        {
            m_Restitution = value;
        }
    }
}
