using UnityEngine;
using System.Collections;

public class tnStandardAIInputFillerParams : ScriptableObject
{
    // Seek-and-flee behaviour.

    [SerializeField]
    private FloatRange m_MinFleeDistanceFactor = null;
    [SerializeField]
    private FloatRange m_MaxFleeDistanceFactor = null;

    // Separation.

    [SerializeField]
    private FloatRange m_SeparationThreshold = null;

    // Energy thresholds.

    [SerializeField]
    private FloatRange m_MinDashEnergy = null;
    [SerializeField]
    private FloatRange m_MinKickEnergy = null;
    [SerializeField]
    private FloatRange m_MinTackleEnergy = null;
    [SerializeField]
    private FloatRange m_MinAttractEnergy = null;

    // Cooldown timers.

    [SerializeField]
    private FloatRange m_DashCooldown = null;
    [SerializeField]
    private FloatRange m_KickCooldown = null;
    [SerializeField]
    private FloatRange m_TackleCooldown = null;
    [SerializeField]
    private FloatRange m_AttractCooldown = null;

    // Dash behaviour.

    // This value should be an upper bound of the distance covered by a dash.

    [SerializeField]
    private FloatRange m_DashDistance = null;           // Used during charges.
    [SerializeField]
    private FloatRange m_ForcedDashDistance = null;     // Used during recovers.

    // Kick behaviour.

    [SerializeField]
    private FloatRange m_KickPrecision = null;

    // Tackle behaviour.

    [SerializeField]
    private FloatRange m_TackleRadius = null;
    [SerializeField]
    private FloatRange m_BallDistanceThreshold = null; // Your character will not tackle if the ball is into this range.

    // Attract behaviour.

    [SerializeField]
    private FloatRange m_AttractMinRadius = null;
    [SerializeField]
    private FloatRange m_AttractMaxRadius = null;

    [SerializeField]
    private FloatRange m_AttractTimeThreshold = null;

    // Extra parameters.

    [SerializeField]
    private FloatRange m_RecoverRadius = null;
    [SerializeField]
    private FloatRange m_RecoverTimeThreshold = null; // When the ball has zero-speed for this time threshold, your character will be forced to kick away the ball.

    // GETTERS

    public FloatRange minFleeDistanceFactor
    {
        get
        {
            return m_MinFleeDistanceFactor;
        }
    }

    public FloatRange maxFleeDistanceFactor
    {
        get
        {
            return m_MaxFleeDistanceFactor;
        }
    }

    public FloatRange separationThreshold
    {
        get
        {
            return m_SeparationThreshold;
        }
    }

    public FloatRange minDashEnergy
    {
        get
        {
            return m_MinDashEnergy;
        }
    }

    public FloatRange minKickEnergy
    {
        get
        {
            return m_MinKickEnergy;
        }
    }

    public FloatRange minTackleEnergy
    {
        get
        {
            return m_MinTackleEnergy;
        }
    }

    public FloatRange minAttractEnergy
    {
        get
        {
            return m_MinAttractEnergy;
        }
    }

    public FloatRange dashCooldown
    {
        get
        {
            return m_DashCooldown;
        }
    }

    public FloatRange kickCooldown
    {
        get
        {
            return m_KickCooldown;
        }
    }

    public FloatRange tackleCooldown
    {
        get
        {
            return m_TackleCooldown;
        }
    }

    public FloatRange attractCooldown
    {
        get
        {
            return m_AttractCooldown;
        }
    }

    public FloatRange dashDistance
    {
        get
        {
            return m_DashDistance;
        }
    }

    public FloatRange forcedDashDistance
    {
        get
        {
            return m_ForcedDashDistance;
        }
    }

    public FloatRange kickPrecision
    {
        get
        {
            return m_KickPrecision;
        }
    }

    public FloatRange tackleRadius
    {
        get
        {
            return m_TackleRadius;
        }
    }

    public FloatRange ballDistanceThreshold
    {
        get
        {
            return m_BallDistanceThreshold;
        }
    }

    public FloatRange attractMinRadius
    {
        get
        {
            return m_AttractMinRadius;
        }
    }

    public FloatRange attractMaxRadius
    {
        get
        {
            return m_AttractMaxRadius;
        }
    }

    public FloatRange attractTimeThreshold
    {
        get
        {
            return m_AttractTimeThreshold;
        }
    }

    public FloatRange recoverRadius
    {
        get
        {
            return m_RecoverRadius;
        }
    }

    public FloatRange recoverTimeThreshold
    {
        get
        {
            return m_RecoverTimeThreshold;
        }
    }

    // BUSINESS LOGIC

    public void GetMinFleeDistanceFactor(out float o_Value)
    {
        if (m_MinFleeDistanceFactor != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_MinFleeDistanceFactor.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetMaxFleeDistanceFactor(out float o_Value)
    {
        if (m_MaxFleeDistanceFactor != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_MaxFleeDistanceFactor.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetSeparationThreshold(out float o_Value)
    {
        if (m_SeparationThreshold != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_SeparationThreshold.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetMinDashEnergy(out float o_Value)
    {
        if (m_MinDashEnergy != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_MinDashEnergy.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetMinKickEnergy(out float o_Value)
    {
        if (m_MinKickEnergy != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_MinKickEnergy.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetMinTackleEnergy(out float o_Value)
    {
        if (m_MinTackleEnergy != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_MinTackleEnergy.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetMinAttractEnergy(out float o_Value)
    {
        if (m_MinAttractEnergy != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_MinAttractEnergy.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetDashCooldown(out float o_Value)
    {
        if (m_DashCooldown != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_DashCooldown.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetKickCooldown(out float o_Value)
    {
        if (m_KickCooldown != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_KickCooldown.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetTackleCooldown(out float o_Value)
    {
        if (m_TackleCooldown != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_TackleCooldown.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetAttractCooldown(out float o_Value)
    {
        if (m_AttractCooldown != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_AttractCooldown.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetDashDistance(out float o_Value)
    {
        if (m_DashDistance != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_DashDistance.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetForcedDashDistance(out float o_Value)
    {
        if (m_ForcedDashDistance != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_ForcedDashDistance.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetKickPrecision(out float o_Value)
    {
        if (m_KickPrecision != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_KickPrecision.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetTackleRadius(out float o_Value)
    {
        if (m_TackleRadius != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_TackleRadius.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetBallDistanceThreshold(out float o_Value)
    {
        if (m_BallDistanceThreshold != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_BallDistanceThreshold.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetAttractMinRadius(out float o_Value)
    {
        if (m_AttractMinRadius != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_AttractMinRadius.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetAttractMaxRadius(out float o_Value)
    {
        if (m_AttractMaxRadius != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_AttractMaxRadius.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetAttractTimeThreshold(out float o_Value)
    {
        if (m_AttractTimeThreshold != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_AttractTimeThreshold.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetRecoverRadius(out float o_Value)
    {
        if (m_RecoverRadius != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_RecoverRadius.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }

    public void GetRecoverTimeThreshold(out float o_Value)
    {
        if (m_RecoverTimeThreshold != null)
        {
            float t = Random.Range(0f, 1f);
            o_Value = m_RecoverTimeThreshold.GetValueAt(t);
        }
        else
        {
            o_Value = 0f;
        }
    }
}
