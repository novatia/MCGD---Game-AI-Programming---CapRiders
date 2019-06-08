using UnityEngine;

public struct TransformData
{
    private Vector3 m_Position;
    private Quaternion m_Rotation;
    private Vector3 m_Scale;

    // ACCESSORS

    public Vector3 position
    {
        get
        {
            return m_Position;
        }
    }

    public Quaternion rotation
    {
        get
        {
            return m_Rotation;
        }
    }

    public Vector3 scale
    {
        get
        {
            return m_Scale;
        }
    }

    // LOGIC

    public void SetPosition(Vector3 i_Position)
    {
        m_Position = i_Position;
    }

    public void SetRotation(Quaternion i_Rotation)
    {
        m_Rotation = i_Rotation;
    }

    public void SetScale(Vector3 i_Scale)
    {
        m_Scale = i_Scale;
    }

    // CTOR

    public TransformData(Vector3 i_Position, Quaternion i_Rotation, Vector3 i_Scale)
    {
        m_Position = i_Position;
        m_Rotation = i_Rotation;
        m_Scale = i_Scale;
    }
}

[RequireComponent(typeof(Transform))]
public class ReplayTransformView : MonoBehaviour, IReplayable
{
    // Fields

    [SerializeField]
    [DisallowEditInPlayMode]
    private int m_SamplePerSecond = 18;

    private Transform m_TargetTransform = null;
    private TimedBuffer<TransformData> m_Buffer = null;

    private TransformData m_Temp;

    private float m_SampleInterval = 0f;
    private float m_SampleTimer = 0f;

    private float m_TimeAccumulator = 0f;

    // MonoBehaviour's interface

    void Awake()
    {
        m_TargetTransform = GetComponent<Transform>();

        int bufferSize = Mathf.CeilToInt(ReplayConfig.s_RecordTime * m_SamplePerSecond);
        m_Buffer = new TimedBuffer<TransformData>(bufferSize);

        m_SampleInterval = 1f / m_SamplePerSecond;
    }

    // IReplayable's interface

    // RECORD

    public void StartRecord()
    {
        m_Buffer.Clear();

        m_SampleTimer = 0f;
        m_TimeAccumulator = 0f;

        TransformData data;
        RecordSample(out data);

        m_Buffer.Push(0f, data);
    }

    public void StopRecord()
    {
        TransformData data;
        RecordSample(out data);

        m_Buffer.Push(m_TimeAccumulator, data);
    }

    public void UpdateRecord(float i_DeltaTime)
    {
        m_TimeAccumulator += i_DeltaTime;
        m_SampleTimer += i_DeltaTime;

        if (m_SampleTimer >= m_SampleInterval)
        {
            TransformData data;
            RecordSample(out data);

            m_Buffer.Push(m_TimeAccumulator, data);

            m_SampleTimer = 0f;
        }
    }

    // PLAY

    public void StartPlay(float i_Time)
    {
        RecordSample(out m_Temp);

        TransformData data;
        Seek(i_Time, out data);

        ApplyData(data);
    }

    public void StopPlay()
    {
        ApplyData(m_Temp);
    }

    public void UpdatePlay(float i_LastPlayedTime, float i_PlayTime)
    {
        TransformData data;
        Seek(i_PlayTime, out data);

        ApplyData(data);
    }

    // INTERNALS

    public void Seek(float i_Time, out TransformData o_Data)
    {
        o_Data = default(TransformData);

        int index;
        float timestamp;
        if (m_Buffer.TryGetIndex(i_Time, out index, out timestamp))
        {
            TransformData data;
            if (m_Buffer.TryGetData(index, out data, out timestamp))
            {
                TransformData nextData;
                float nextTimestamp;
                if (!m_Buffer.TryGetData(index + 1, out nextData, out nextTimestamp))
                {
                    nextTimestamp = timestamp;
                    nextData = data;
                }

                float p = MathUtils.GetClampedPercentage(i_Time, timestamp, nextTimestamp);

                Vector3 targetPosition = Vector3.Lerp(data.position, nextData.position, p);
                Quaternion targetRotation = Quaternion.Lerp(data.rotation, nextData.rotation, p);
                Vector3 targetScale = Vector3.Lerp(data.scale, nextData.scale, p);

                TransformData targetData = new TransformData(targetPosition, targetRotation, targetScale);
                o_Data = targetData;
            }
        }
    }

    private void RecordSample(out TransformData o_Data)
    {
        Vector3 currentPosition = m_TargetTransform.localPosition;
        Quaternion currentRotation = m_TargetTransform.localRotation;
        Vector3 currentScale = m_TargetTransform.localScale;

        o_Data = new TransformData();

        o_Data.SetPosition(currentPosition);
        o_Data.SetRotation(currentRotation);
        o_Data.SetScale(currentScale);
    }

    // UTILS

    private void ApplyData(TransformData i_Data)
    {
        Vector3 position = i_Data.position;
        Quaternion rotation = i_Data.rotation;
        Vector3 scale = i_Data.scale;

        m_TargetTransform.localPosition = position;
        m_TargetTransform.localRotation = rotation;
        m_TargetTransform.localScale = scale;
    }
}
