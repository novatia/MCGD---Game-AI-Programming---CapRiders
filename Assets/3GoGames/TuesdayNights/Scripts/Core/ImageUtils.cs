using UnityEngine;
using UnityEngine.UI;

public static class ImageUtils
{
    public static void SetColorWithoutAlpha(this Image image, float i_R, float i_G, float i_B)
    {
        if (image == null)
            return;

        Color c = new Color(i_R, i_G, i_B, 0f);
        image.SetColorWithoutAlpha(c);
    }

    public static void SetColorWithoutAlpha(this Image image, Color i_Color)
    {
        if (image == null)
            return;

        Color currentColor = image.color;
        float currentAlpha = currentColor.a;

        Color targetColor = new Color(i_Color.r, i_Color.g, i_Color.b, currentAlpha);
        image.color = targetColor;
    }

    public static void SetColorAlpha(this Image image, float i_Alpha)
    {
        if (image == null)
            return;

        Color currentColor = image.color;
        Color targetColor = new Color(currentColor.r, currentColor.g, currentColor.b, i_Alpha);

        image.color = targetColor;
    }
}