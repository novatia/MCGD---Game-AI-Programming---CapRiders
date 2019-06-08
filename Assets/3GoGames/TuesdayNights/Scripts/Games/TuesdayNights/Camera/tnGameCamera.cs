using UnityEngine;

using System.Collections.Generic;

using TuesdayNights;

public class tnGameCamera : MonoBehaviour
{
    [SerializeField]
    private bool m_FollowTarget = false;

    [SerializeField]
    private Vector2 m_Origin = new Vector2(0f, 0f);

    [SerializeField]
    private Vector2 m_RangeInf = new Vector2(-10f, -10f);
    [SerializeField]
    private Vector2 m_RangeSup = new Vector2(10f, 10f);

    [SerializeField]
    private Vector2 m_OffsetInf = new Vector2(-1f, -1f);
    [SerializeField]
    private Vector2 m_OffsetSup = new Vector2(1f, 1f);

    [SerializeField]
    private float m_StepFactorX = 0.5f;
    [SerializeField]
    private float m_StepFactorY = 0.5f;

    private FilteredFloat m_FilterX;
    private FilteredFloat m_FilterY;

    private Vector3 m_TargetPosition = Vector3.zero;
    private Vector3 m_CurrentPosition = Vector3.zero;

    private Vector3 m_OriginalPositon = Vector3.zero;

    private Transform m_Target = null;

    private Camera m_Cam = null;

    public bool m_AutoMove = true;
    
    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    public float size
    {
        get
        {
            if (m_Cam != null)
            {
                return m_Cam.orthographicSize;
            }

            return -1f;
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_FilterX = new FilteredFloat(m_StepFactorX, m_StepFactorX);
        m_FilterY = new FilteredFloat(m_StepFactorY, m_StepFactorY);

        m_Cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        m_OriginalPositon = transform.position;
    }

    void OnEnable()
    {
        SetPosition(transform.position); // Refresh internal status.
    }

    void LateUpdate()
    {
        if (!m_FollowTarget)
            return;

        if (!m_AutoMove)
            return;

        // Update target position.

        {
            m_TargetPosition = m_OriginalPositon;

            bool enableCameraMovement;
            GameSettings.TryGetBoolMain(Settings.s_CameraMovementSetting, out enableCameraMovement);

            if (enableCameraMovement)
            {
                float offsetX;
                float offsetY;

                ComputeTargetOffsetX(out offsetX);
                ComputeTargetOffsetY(out offsetY);

                m_TargetPosition += new Vector3(offsetX, offsetY, 0f);
            }
        }

        // Step.

        {
            m_FilterX.Step(m_TargetPosition.x, Time.deltaTime);
            m_FilterY.Step(m_TargetPosition.y, Time.deltaTime);
        }

        // Update current position.

        {
            float currentPositionX = m_FilterX.position;
            float currentPositionY = m_FilterY.position;

            m_CurrentPosition = new Vector3(currentPositionX, currentPositionY, position.z);
        }

        // Move camera.

        {
            Vector3 newPosition = new Vector3(m_CurrentPosition.x, m_CurrentPosition.y, m_CurrentPosition.z);
            transform.position = newPosition;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw target.

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(m_TargetPosition, 0.2f);

        // Draw current position.

        Gizmos.color = Color.grey;
        Gizmos.DrawSphere(m_CurrentPosition, 0.2f);
    }

    // BUSINESS LOGIC

    public void SetPosition(Vector3 i_Position)
    {
        m_FilterX.position = i_Position.x;
        m_FilterY.position = i_Position.y;

        m_TargetPosition = i_Position;
        m_CurrentPosition = i_Position;

        transform.position = i_Position;
    }

    public void SetPosition(Vector2 i_Position)
    {
        float z = position.z;
        Vector3 pos = new Vector3(i_Position.x, i_Position.y, z);
        SetPosition(pos);
    }

    public void SetTarget(Transform i_Target)
    {
        m_Target = i_Target;
    }

    public void SetSize(float i_Size)
    {
        if (m_Cam != null)
        {
            m_Cam.orthographicSize = i_Size;
        }
    }

    public void SetAutoMove(bool i_Value)
    {
        m_AutoMove = i_Value;
    }

    public void ResetPosition()
    {
        SetPosition(m_OriginalPositon);
    }

    // INTERNALS

    private void ComputeTargetOffsetX(out float o_OffsetX)
    {
        float offset = 0f;

        if (m_Target != null)
        {
            if (m_Target.position.x < m_Origin.x)
            {
                float xPerc = MathUtils.GetClampedPercentage(m_Target.position.x, m_RangeInf.x, m_Origin.x);
                offset = (1f - xPerc) * m_OffsetInf.x;
            }
            else
            {
                float xPerc = MathUtils.GetClampedPercentage(m_Target.position.x, m_Origin.x, m_RangeSup.x);
                offset = xPerc *m_OffsetSup.x;
            }
        }

        o_OffsetX = offset;
    }

    private void ComputeTargetOffsetY(out float o_OffsetY)
    {
        float offset = 0f;

        if (m_Target != null)
        {
            if (m_Target.position.y < m_Origin.y)
            {
                float yPerc = MathUtils.GetClampedPercentage(m_Target.position.y, m_RangeInf.y, m_Origin.y);
                offset = (1f - yPerc) * m_OffsetInf.y;
            }
            else
            {
                float yPerc = MathUtils.GetClampedPercentage(m_Target.position.y, m_Origin.y, m_RangeSup.y);
                offset = yPerc * m_OffsetSup.y;
            }
        }

        o_OffsetY = offset;
    }
}
