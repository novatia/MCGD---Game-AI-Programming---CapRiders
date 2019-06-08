using UnityEngine;

namespace TrueSync
{
    public static class MathFP
    {
        public static FP Lerp(FP i_A, FP i_B, FP i_T)
        {
            FP t = Clamp01(i_T);

            FP size = i_B - i_A;
            FP delta = t * size;

            return i_A + delta;
        }

        public static FP Pow(FP i_Base, FP i_Exp)
        {
            if (i_Base == FP.Zero)
            {
                return FP.Zero;
            }

            FP baseValue = (i_Exp > FP.Zero) ? i_Base : (FP.One / i_Base);

            FP result = FP.One;

            int i = 0;
            while (i < i_Exp)
            {
                result *= baseValue;
                ++i;
            }

            return result;
        }

        public static FP Floor(FP i_Value)
        {
            return FP.Floor(i_Value);
        }

        public static FP Ceil(FP i_Value)
        {
            return FP.Ceiling(i_Value);
        }

        public static FP Abs(FP i_Value)
        {
            if (i_Value < FP.Zero)
            {
                return -i_Value;
            }

            return i_Value;
        }

        public static FP Max(params FP[] i_Values)
        {
            FP max = FP.MinValue;
            for (int index = 0; index < i_Values.Length; ++index)
            {
                FP value = i_Values[index];
                if (value > max)
                {
                    max = value;
                }
            }

            return max;
        }

        public static FP Min(params FP[] i_Values)
        {
            FP min = FP.MaxValue;
            for (int index = 0; index < i_Values.Length; ++index)
            {
                FP value = i_Values[index];
                if (value < min)
                {
                    min = value;
                }
            }

            return min;
        }

        public static FP Clamp01(FP i_Value)
        {
            return Clamp(i_Value, FP.Zero, FP.One);
        }

        public static FP Clamp(FP i_Value, FP i_Min, FP i_Max)
        {
            i_Value = Max(i_Min, i_Value);
            i_Value = Min(i_Max, i_Value);

            return i_Value;
        }

        public static FP GetClampedPercentage(FP value, FP min, FP max)
        {
            if (Abs(max - min) < 0.00001f)
            {
                return FP.Zero;
            }
            else
            {
                return Clamp01((value - min) / (max - min));
            }
        }

        public static bool ApproximatelyEqual(FP i_A, FP i_B, FP i_Tolerance)
        {
            i_Tolerance = Clamp01(i_Tolerance);

            FP minB = i_B - i_B * i_Tolerance;
            FP maxB = i_B + i_B * i_Tolerance;

            return !(i_A < minB || i_A > maxB);
        }

        public static FP Square(FP i_Value)
        {
            return (i_Value * i_Value);
        }

        public static FP GetBellInterpolation2(FP i_Value, FP i_T0, FP i_T1)
        {
            return (FP.One - Square(2f * (GetClampedPercentage(i_Value, i_T0, i_T1)) - FP.One));
        }

        public static FP GetBellInterpolation4(FP i_Value, FP i_T0, FP i_T1)
        {
            return (FP.One - Square(GetBellInterpolation2(i_Value, i_T0, i_T1)));
        }

        public static FP GetBellInterpolation8(FP i_Value, FP i_T0, FP i_T1)
        {
            return (FP.One - Square(GetBellInterpolation4(i_Value, i_T0, i_T1)));
        }

        public static FP InterpolateBetweenThresholds(FP i_Value, FP i_T0, FP i_T1, FP i_T2, FP i_T3)
        {
            if (i_Value <= i_T0)
            {
                return FP.Zero;
            }

            if (i_Value <= i_T1)
            {
                return GetClampedPercentage(i_Value, i_T0, i_T1);
            }

            if (i_Value <= i_T2)
            {
                return FP.One;
            }

            if (i_Value <= i_T3)
            {
                return (FP.One - GetClampedPercentage(i_Value, i_T3, i_T2));
            }

            return FP.Zero;
        }

        public static FP GetNormalizedAngle(FP i_Angle)
        {
            FP normalizedAngle = i_Angle - Floor(i_Angle / 360f) * 360f;
            normalizedAngle = Clamp(normalizedAngle, FP.Zero, 360f);

            return normalizedAngle;
        }

        public static FP ClampAngle(FP i_Angle, FP i_Min, FP i_Max)
        {
            FP result = GetNormalizedAngle(i_Angle);
            result = Clamp(result, i_Min, i_Max);

            return result;
        }

        public static FP SmoothDamp(FP i_Current, FP i_Target, ref FP i_CurrentVelocity, FP i_SmoothTime, FP i_MaxSpeed, FP i_DeltaTime)
        {
            FP smoothTime = Max(0.0001f, i_SmoothTime);

            FP num = 2f / smoothTime;
            FP num2 = num * i_DeltaTime;
            FP num3 = FP.One / (FP.One + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            FP num4 = i_Current - i_Target;
            FP num5 = i_Target;
            FP num6 = i_MaxSpeed * smoothTime;

            num4 = Clamp(num4, -num6, num6);

            i_Target = i_Current - num4;

            FP num7 = (i_CurrentVelocity + num * num4) * i_DeltaTime;

            i_CurrentVelocity = (i_CurrentVelocity - num * num7) * num3;

            FP num8 = i_Target + (num4 + num7) * num3;

            if (num5 - i_Current > FP.Zero == num8 > num5)
            {
                num8 = num5;
                i_CurrentVelocity = (num8 - num5) / i_DeltaTime;
            }

            return num8;
        }

        public static FP SmoothDamp(FP i_Current, FP i_Target, ref FP i_CurrentVelocity, FP i_SmoothTime, FP i_DeltaTime)
        {
            FP smoothTime = Max(0.0001f, i_SmoothTime);

            FP num = 2f / smoothTime;
            FP num2 = num * i_DeltaTime;
            FP num3 = FP.One / (FP.One + num2 + 0.48f * num2 * num2 + 0.235f * num2 * num2 * num2);
            FP num4 = i_Current - i_Target;
            FP num5 = i_Target;

            i_Target = i_Current - num4;

            FP num6 = (i_CurrentVelocity + num * num4) * i_DeltaTime;

            i_CurrentVelocity = (i_CurrentVelocity - num * num6) * num3;

            FP num7 = i_Target + (num4 + num6) * num3;

            if (num5 - i_Current > FP.Zero == num7 > num5)
            {
                num7 = num5;
                i_CurrentVelocity = (num7 - num5) / i_DeltaTime;
            }

            return num7;
        }
    }
}