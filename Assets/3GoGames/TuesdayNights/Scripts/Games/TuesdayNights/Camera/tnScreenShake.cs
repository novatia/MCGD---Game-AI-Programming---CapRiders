using UnityEngine;

using System;

using TuesdayNights;

using Random = UnityEngine.Random;

public enum ShakeMode
{
    Interruput,
    DontInterrupt,
}

public class tnScreenShake : MonoBehaviour
{
    [SerializeField]
    private float m_ShakeMultiplier = 1f;

    private float m_ShakeAmount = 0f;
    private float m_Timer = 0f;

    private Action m_Callback = null;

    private bool m_Shake = false;

    private Vector3 m_PrevLocalPosition = Vector3.zero;

    // MonoBehaviour's INTERFACE

    void OnEnable()
    {
        m_PrevLocalPosition = transform.localPosition;
    }

    void OnDisable()
    {

    }

    void Update()
    {
        if (m_Shake)
        { 
            Vector3 randomOffset = Random.insideUnitSphere * m_ShakeAmount * m_ShakeMultiplier;
            randomOffset.z = 0f;

            transform.localPosition = m_PrevLocalPosition + randomOffset;

            m_Timer -= Time.deltaTime;

            if (m_Timer < 0f)
            {
                EndShake();
            }
        }
        else
        {
            transform.localPosition = m_PrevLocalPosition;
        }
    }

    // BUSINESS LOGIC

    public void ForceShake(float i_ShakeTime, float i_Amount, ShakeMode i_ShakeMode, Action i_Callback = null)
    {
        bool screenShakeEnabled;
        GameSettings.TryGetBoolMain(Settings.s_ScreenshakeSetting, out screenShakeEnabled);

        if (!screenShakeEnabled)
            return;

        OnScreenShake(i_ShakeTime, i_Amount, i_ShakeMode, i_Callback);
    }

    public void ForceStop()
    {
        EndShake();
    }

    // INTERNALS

    private void StartShake(float i_ShakeTime, float i_Amount, Action i_Callback)
    {
        if (m_Shake)
        {
            if (i_Callback != null)
            {
                i_Callback();
            }

            LogManager.Log(this, LogContexts.Camera, "Shake ignored");

            return;
        }

        m_ShakeAmount = i_Amount;
        m_Timer = Mathf.Max(0f, i_ShakeTime);

        m_Callback = i_Callback;

        m_Shake = true;

        LogManager.Log(this, LogContexts.Camera, "Shake started [" + m_ShakeAmount  + "]" + "[" + m_Timer + "]");
    }

    private void EndShake()
    {
        if (!m_Shake)
            return;

        m_ShakeAmount = 0f;
        m_Timer = 0f;

        if (m_Callback != null)
        {
            m_Callback();

            m_Callback = null;
        }

        m_Shake = false;

        LogManager.Log(this, LogContexts.Camera, "Shake ended");
    }

    private void OnScreenShake(float i_ShakeTime, float i_Amount, ShakeMode i_ShakeMode, Action i_Callback)
    {
        if (i_ShakeMode == ShakeMode.Interruput)
        {
            EndShake();
        }
        else 
        {
            // DontInterrupt
        }

        StartShake(i_ShakeTime, i_Amount, i_Callback);
    }
}
