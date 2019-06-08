using UnityEngine;

public abstract class Effect : MonoBehaviour, IEffect
{
    private bool m_IsPlaying = false;
    private AnimCompletedCallback m_AnimCompletedCallback;

    private bool m_FollowTransform = false;
    private Transform m_TargetTransform = null;

    private Vector3 m_LocalPosition = Vector3.zero;
    private Quaternion m_LocalRotation = Quaternion.identity;

    // IEffect interface

    public bool isPlaying
    {
        get { return m_IsPlaying; }
    }

    public void Play(AnimCompletedCallback i_AnimCompletedCallback = null, AnimEventCallback i_AnimEventCallback = null)
    {
        if (m_IsPlaying)
            return;

        m_IsPlaying = true;
        m_AnimCompletedCallback = i_AnimCompletedCallback;

        OnPlay(i_AnimEventCallback);
        NotifyPlay(i_AnimEventCallback);
    }

    public void Stop()
    {
        if (!m_IsPlaying)
            return;

        NotifyStop();
        OnStop();

        m_TargetTransform = null;
        m_FollowTransform = false;
        m_AnimCompletedCallback = null;
        m_IsPlaying = false;
    }

    // BUSINESS LOGIC

    public void SetTargetTransform(Vector3 i_WorldPosition, Quaternion i_WorldRotation, Transform i_Anchor = null)
    {
        if (i_Anchor == null)
        {
            m_TargetTransform = null;
            m_FollowTransform = false;

            m_LocalPosition = i_WorldPosition;
            m_LocalRotation = i_WorldRotation;
        }
        else
        {
            m_TargetTransform = i_Anchor;
            m_FollowTransform = true;

            m_LocalPosition = i_WorldPosition - i_Anchor.position;
            m_LocalRotation = i_WorldRotation * Quaternion.Inverse(i_Anchor.rotation);
        }

        UpdatePosition();
    }

    // MonoBehaviour 's interface

    void Awake()
    {
        OnAwake();
    }

    void OnEnable()
    {
        // I can call Clear here (I also call it in OnDisable). It is redundance.
    }

    void OnDisable()
    {
        if (!m_IsPlaying)
            return;

        OnStop();

        m_TargetTransform = null;
        m_FollowTransform = false;
        m_AnimCompletedCallback = null;
        m_IsPlaying = false;
    }

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        if (m_IsPlaying)
        {
            if (m_FollowTransform)
            {
                if (m_TargetTransform == null || !m_TargetTransform.gameObject.activeSelf)
                {
                    Stop();
                    return;
                }

                UpdatePosition();
            }

            OnUpdate(Time.deltaTime);
        }
    }
       
    // Abstract methods

    protected abstract void OnPlay(AnimEventCallback i_EventCallback = null);
    protected abstract void OnStop();

    // Virtual methods

    protected virtual void OnAwake() { }
    protected virtual void OnStart() { }
    protected virtual void OnUpdate(float i_DeltaTime) { }
    protected virtual void NotifyPlay(AnimEventCallback i_EventCallback = null) { }
    protected virtual void NotifyStop() { }

    // Methods

    protected void Finish()
    {
        OnStop();

        if (m_AnimCompletedCallback != null)
        {
            m_AnimCompletedCallback();
            m_AnimCompletedCallback = null;
        }

        m_IsPlaying = false;
    }

    // INTERNALS

    private void UpdatePosition()
    {
        Vector3 localPosition = Vector3.zero;
        Quaternion localRotation = Quaternion.identity;

        if (m_FollowTransform)
        {
            if (m_TargetTransform != null && m_TargetTransform.gameObject.activeSelf)
            {
                localPosition += m_TargetTransform.position;
                localRotation *= m_TargetTransform.rotation;
            }
        }

        localPosition += m_LocalPosition;
        localRotation *= m_LocalRotation;

        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
    }
}