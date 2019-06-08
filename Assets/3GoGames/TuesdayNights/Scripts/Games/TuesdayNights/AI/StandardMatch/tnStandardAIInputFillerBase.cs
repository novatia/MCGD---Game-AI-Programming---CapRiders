using UnityEngine;

using TuesdayNights;

public abstract class tnStandardAIInputFillerBase : tnBaseAIInputFiller
{
    private tnEnergy m_Energy = null;

    // UTILS

    protected bool CheckEnergy(float i_Energy)
    {
        if (m_Energy == null)
        {
            return false;
        }

        return (m_Energy.energy > i_Energy);
    }

    // CTOR

    public tnStandardAIInputFillerBase(GameObject i_Self)
        : base (i_Self)
    {
        m_Energy = i_Self.GetComponent<tnEnergy>();
    }
}
