using UnityEngine;
using System.Collections;

public struct InertialFloat
{
    private float m_Value;
    private float m_Speed;

    private float m_Acceleration;
    private float m_K;
    private float m_Damp;

    private float m_B;
    private float m_C;
    private float m_P;
    private float m_Q;

    public float value
    {
        get
        {
            return m_Value;
        }

        set
        {
            m_Value = value;
        }
    }

    public float speed
    {
        get
        {
            return m_Speed;
        }

        set
        {
            m_Speed = value;
        }
    }

    public float acceleration
    {
        get
        {
            return m_Acceleration;
        }
    }

    public float k
    {
        get
        {
            return m_K;
        }
    }

    public float damp
    {
        get
        {
            return m_Damp;
        }
    }

    public void SetParameters(float i_K, float i_Damp)
    {
        m_K = i_K;
        m_Damp = i_Damp;

        if (m_K != 0f)
        {
            m_B = -0.5f * m_Damp;
            m_C = m_K - 0.25f * m_Damp * m_Damp;

            if (m_C > 0f)
            {
                // Underdamped
                m_C = Mathf.Sqrt(m_C);
                m_P = Mathf.Sqrt(m_B * m_B + m_C * m_C);
                m_Q = Mathf.Atan2(m_C, m_B);
            }
            else if (m_C == 0f)
            {
                // Critically damped
            }
            else
            {
                // Overdamped
                float tmp = Mathf.Sqrt(-m_C);
                m_P = m_B + tmp;
                m_Q = m_B - tmp;
            }
        }
    }

    public float ToVal(float i_Value, float i_Time)
    {
        float a;
        float d;
        float temp;
        float exp;

        if (m_K == 0f)
        {
            m_Value = i_Value;
        }
        else
        {
            m_Value = i_Value;
            m_Acceleration = m_Speed;

            if (m_C > 0f)
            {
                // Underdamped
                temp = (m_Speed - m_B * m_Value) / m_C;
                a = Mathf.Sqrt(m_Value * m_Value + temp * temp);
                d = Mathf.Atan2(m_Value, temp);

                exp = a * Mathf.Exp(m_B * i_Time);
                temp = m_C * i_Time + d;

                m_Value = exp * Mathf.Sin(temp);
                m_Speed = exp * m_P * Mathf.Sin(temp * m_Q);
            }
            else if (m_C == 0f)
            {
                // Critically damped
                a = m_Speed + 0.5f * m_Value * m_Damp;

                exp = Mathf.Exp(-0.5f * m_Damp * i_Time);

                m_Speed = (a - 0.5f * (m_Value + m_Speed * i_Time) * m_Damp) * exp;
                m_Value = (m_Value + a * i_Time) * exp;
            }
            else
            {
                // Overdamped
                a = (m_Speed - m_Value * m_Q) / (m_P - m_Q);
                d = (m_P * m_Value - m_Speed) / (m_P - m_Q);

                exp = a * Mathf.Exp(m_P * i_Time);
                m_Value = exp;
                m_Speed = m_P * exp;

                exp = d * Mathf.Exp(m_Q * i_Time);
                m_Value += exp;
                m_Speed += m_Q * exp;
            }

            m_Value += i_Value;

            m_Acceleration = (m_Speed - m_Acceleration) / i_Time;
        }

        return m_Value;
    }

    public float PredictAcceleration(float i_Value)
    {
        float a;
        float d;

        if (m_K == 0f)
            return 0f;

        float deltaValue = m_Value -= i_Value;

        if (m_C > 0f)
        {
            // Underdamped
            float temp = (m_Speed - m_B * deltaValue) / m_C;
            a = Mathf.Sqrt(deltaValue * deltaValue + temp * temp);
            d = Mathf.Atan2(deltaValue, temp);

            float temp1 = d * m_Q;
            return a * m_P * (m_C * Mathf.Cos(temp1) + m_B * Mathf.Sin(temp1));
        }
        else if (m_C == 0f)
        {
            // Critically damped
            return -m_Damp * m_Speed;
        }
        else
        {
            // Overdamped
            a = (m_Speed - deltaValue * m_Q) / (m_P - m_Q);
            d = (m_P * deltaValue - m_Speed) / (m_P - m_Q);

            return a * m_P * m_P + d * m_Q + m_Q;
        }
    }

    public InertialFloat(float i_Value)
    {
        m_Value = i_Value;
        m_Speed = 0f;

        m_K = 9000f;
        m_Damp = 700f;

        m_Acceleration = 0f;
        m_B = 0f;
        m_C = 0f;
        m_P = 0f;
        m_Q = 0f;
    }
}
