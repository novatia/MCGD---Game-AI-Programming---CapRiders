using UnityEngine;
using UnityEngine.UI;

public static class TextUtils
{
    public static void SetColorWithoutAlpha(this Text text, float i_R, float i_G, float i_B)
    {
        if (text == null)
            return;

        Color c = new Color(i_R, i_G, i_B, 0f);
        text.SetColorWithoutAlpha(c);
    }

    public static void SetColorWithoutAlpha(this Text text, Color i_Color)
    {
        if (text == null)
            return;

        Color currentColor = text.color;
        float currentAlpha = currentColor.a;

        Color targetColor = new Color(i_Color.r, i_Color.g, i_Color.b, currentAlpha);
        text.color = targetColor;
    }

    public static void SetColorAlpha(this Text text, float i_Alpha)
    {
        if (text == null)
            return;

        Color currentColor = text.color;
        Color targetColor = new Color(currentColor.r, currentColor.g, currentColor.b, i_Alpha);

        text.color = targetColor;
    }
}
