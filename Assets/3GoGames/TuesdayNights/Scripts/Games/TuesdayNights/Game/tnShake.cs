using UnityEngine;

using TuesdayNights;

using TrueSync;

public class tnShake : TrueSyncBehaviour
{
    [SerializeField]
    private float m_ShakeTimeMin = 0f;
    [SerializeField]
    private float m_ShakeTimeMax = 0f;

    [SerializeField]
    private float m_ShakeAmountMin = 0f;
    [SerializeField]
    private float m_ShakeAmountMax = 0f;

    [SerializeField]
    private float m_VelocityThresholdMin = 0f;
    [SerializeField]
    private float m_VelocityThresholdMax = 0f;

    [SerializeField]
    private ShakeMode m_ShakeMode = ShakeMode.DontInterrupt;

    [SerializeField]
    private LayerMask m_LayerMask = 0;

    private tnScreenShake m_ScreenShake = null;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        GameObject gameCameraGo = GameObject.FindGameObjectWithTag("GameCamera");

        if (gameCameraGo != null)
        {
            m_ScreenShake = gameCameraGo.GetComponentInChildren<tnScreenShake>();
        }

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Shake;
    }

    // TrueSyncBehaviour's INTERFACE

    public override void OnSyncedCollisionEnter(TSCollision2D i_Collision)
    {
        if (m_ScreenShake == null)
            return;

        GameObject colliderGo = i_Collision.collider.gameObject;

        bool validObject = colliderGo.CheckLayerMask(m_LayerMask);

        if (!validObject)
            return;

        float velocity2 = i_Collision.relativeVelocity.LengthSquared().AsFloat();

        float minThreshold2 = m_VelocityThresholdMin * m_VelocityThresholdMin;
        float maxThreshold2 = m_VelocityThresholdMax * m_VelocityThresholdMax;

        float velocityFactor = MathUtils.GetClampedPercentage(velocity2, minThreshold2, maxThreshold2);

        float shakeTime = Mathf.Lerp(m_ShakeTimeMin, m_ShakeTimeMax, velocityFactor);
        float shakeAmount = Mathf.Lerp(m_ShakeAmountMin, m_ShakeAmountMax, velocityFactor);

        if (shakeTime < Mathf.Epsilon)
            return;

        if (shakeAmount < Mathf.Epsilon)
            return;

        m_ScreenShake.ForceShake(shakeTime, shakeAmount, m_ShakeMode, null);
    }
}
