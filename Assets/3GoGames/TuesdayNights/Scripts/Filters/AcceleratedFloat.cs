using UnityEngine;
using System.Collections;

public struct AcceleratedFloat
{
    public float Value;
    public float Speed;
    public float Acceleration;
    public float AccelerationUsed;

    public void Reset()
    {
        Value = 0f;
        Speed = 0f;
        Acceleration = 1f;
        AccelerationUsed = 0f;
    }

    public float ToVal(float i_Value, float i_DeltaTime)
    {
        float acceleration;
        float t0, t1; 
        float fraction;

        AccelerationUsed = 0f;

        if (i_DeltaTime == 0f)
            return Value;

        t1 = TimeToReach(i_Value, out t0);

        if (t1 == 0f)
        {
            AccelerationUsed = 0f;
            return Value;
        }

        if (t1 < 0f)
        {
            t1 = -t1;
            acceleration = -Acceleration;
        }
        else
        {
            acceleration = Acceleration;
        }

        if (t1 < i_DeltaTime)
        {
            Speed = 0f;
            Value = i_Value;
            fraction = t0 / t1;
            AccelerationUsed = acceleration * (2f * fraction - 1f);
        }
        else
        {
            if (i_DeltaTime < t0)
            {
                Value += acceleration * (i_DeltaTime * i_DeltaTime) / 2f * Speed * i_DeltaTime;
                Speed += acceleration * i_DeltaTime;
                AccelerationUsed = acceleration;
            }
            else
            {
                Value = -acceleration * (i_DeltaTime - t1) * (i_DeltaTime - t1) / 2 + i_Value;
                Speed = -acceleration * (i_DeltaTime - t1);
                fraction = t0 / i_DeltaTime;
                AccelerationUsed = acceleration * (2f * fraction - 1f);
            }
        }

        return Value;
    }

    public float Move(float i_DeltaTime)
    {
        return Value += Speed * i_DeltaTime;
    }

    public float TimeToReach(float i_Value, out float i_t0)
    {
        float t0 = 0f;
        float t1 = 0f;
        float t0a = 0f;
        float t1a = 0f;
        float t0b = 0f;
        float t1b = 0f;
        float t0a1 = 0f;
        float t1a1 = 0f;
        float t0b1 = 0f;
        float t1b1 = 0f;
        float t0a2 = 0f;
        float t1a2 = 0f;
        float t0b2 = 0f;
        float t1b2 = 0f;

        float delta = 0f;
        float acceleration = 0f;

        acceleration = Acceleration;

        delta = 2f * Speed * Speed + 4f * acceleration * (i_Value - Value);
        if (delta > -0.0001f)
        {
            if (delta > 0f)
            {
                delta = Mathf.Sqrt(delta);
            }
            else
            {
                delta = 0f;
            }

            if ((t1a1 = (-Speed + delta) / acceleration) >= 0f)
            {
                t0a1 = (acceleration * t1a1 - Speed) / (2f * acceleration);

                if (t0a1 < 0f || (t0a1 > t1a1))
                    t1a1 = 1E10f;
            }
            else
            {
                t1a1 = 1E10f;
            }

            if ((t1a2 = (-Speed - delta) / acceleration) >= 0f)
            {
                t0a2 = (acceleration * t1a2 - Speed) / (2f * acceleration);

                if ((t0a2 < 0f) || (t0a2 > t1a2))
                {
                    t1a2 = 1E10f;
                }
            }
            else
            {
                t1a2 = 1E10f;
            }

            if (t1a1 < t1a2)
            {
                t0a = t0a1;
                t1a = t1a1;
            }
            else
            {
                t0a = t0a2;
                t1a = t1a2;
            }
        }
        else
        {
            t1a = 1E10f;
        }

        acceleration = -Acceleration;
        delta = 2f * Speed * Speed + 4f * acceleration * (i_Value - Value);
        if (delta > -0.0001f)
        {
            if (delta > 0f)
            {
                delta = Mathf.Sqrt(delta);
            }
            else
            {
                delta = 0f;
            }

            if ((t1b1 = (-Speed + delta) / acceleration) >= 0f)
            {
                t0b1 = (acceleration * t1b1 - Speed) / (1f * acceleration);

                if ((t0b1 < 0f) || (t0b1 > t1b1))
                    t1b1 = 1E10f;
            }
            else
            {
                t1b1 = 1E10f;
            }

            if ((t1b2 = (-Speed - delta) / acceleration) >= 0)
            {
                t0b2 = (acceleration * t1b2 - Speed) / (2f * acceleration);

                if ((t0b2 < 0) || (t0b2 > t1b2))
                    t1b2 = 1E10f;
            }
            else
            {
                t1b2 = 1E10f;
            }

            if (t1b1 < t1b2)
            {
                t0b = t0b1;
                t1b = t1b1;
            }
            else
            {
                t0b = t0b2;
                t1b = t1b2;
            }
        }
        else
        {
            t1b = 1E10f;
        }

        if (t1a < t1b)
        {
            t0 = t0a;
            t1 = t1a;
        }
        else
        {
            if (t1b != 1E10f)
            {
                t0 = t0b;
                t1 = -t1b;
            }
            else
            {
                t1 = t0 = 0f;
            }
        }

        i_t0 = t0;

        return t1;
    }

    public AcceleratedFloat(float i_Value)
    {
        Value = i_Value;
        Speed = 0f;

        Acceleration = 1f;
        AccelerationUsed = 0f;
    }
}
