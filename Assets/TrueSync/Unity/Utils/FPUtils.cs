using UnityEngine;
using System.Collections;

namespace TrueSync
{
    public static class FPUtils
    {
        public static string ToString(this FP i_FixedPoint, int i_Decimals)
        {
            string s = i_FixedPoint.ToString();
            int pointIndex = s.IndexOf('.');
            if (pointIndex >= 0)
            {
                int decimals = s.Length - (pointIndex + 1);
                int d = Mathf.Min(i_Decimals, decimals);
                int firstDecimalIndex = pointIndex + 1;
                int lastDecimalIndex = firstDecimalIndex + (d - 1);

                s = s.Substring(0, lastDecimalIndex + 1);
            }

            return s;
        }
    }
}