using UnityEngine;

public static class AudioUtils
{
    public static float LinearToDecibel(float i_Linear)
    {
        float dB;

        i_Linear = Mathf.Clamp01(i_Linear);

        if (i_Linear > 0.0001f)
        {
            dB = Mathf.Max(20f * Mathf.Log10(i_Linear), -80f);
        }
        else
        {
            dB = -80f;
        }

        return dB;
    }

    public static float DecibelToLinear(float i_dB)
    {
        i_dB = Mathf.Max(i_dB, -80f);

        float linear = Mathf.Pow(10f, i_dB / 20f);
        linear = Mathf.Clamp01(linear);

        return linear;
    }
}
