#pragma warning disable 618

using UnityEngine;

using System;

public class FadeInOut : Singleton<FadeInOut> 
{
    private GUITexture m_GuiTexture = null;

    private float m_TargetAlpha = 0.5f;

    private float m_FadeDuration = 1f;
    private float m_ElapsedTime = 0f;

    private Action m_Callback = null;

    // STATIC METHODS

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void FadeToBlackMain(float i_Duration, Action i_Callback = null)
    {
        if (Instance != null)
        {
            Instance.FadeToBlack(i_Duration, i_Callback);
        }
    }

    public static void ClearMain(float i_Duration, Action i_Callback = null)
    {
        if (Instance != null)
        {
            Instance.Clear(i_Duration, i_Callback);
        }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        m_GuiTexture = gameObject.AddComponent<GUITexture>();
        m_GuiTexture.gameObject.layer = LayerMask.NameToLayer("Default");

        Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.SetPixel(0, 0, Color.black);
        texture.Apply();

        m_GuiTexture.texture = texture;

        m_GuiTexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
        m_GuiTexture.color = new Color(m_GuiTexture.color.r, m_GuiTexture.color.g, m_GuiTexture.color.b, 0f);

        m_GuiTexture.enabled = false;
    }

    void Update()
    {
        if (m_GuiTexture == null || !m_GuiTexture.enabled)
            return;

        float percentage = m_ElapsedTime / m_FadeDuration;
        percentage = Mathf.Clamp01(percentage);

        Color targetColor = new Color(m_GuiTexture.color.r, m_GuiTexture.color.g, m_GuiTexture.color.b, m_TargetAlpha);
        m_GuiTexture.color = Color.Lerp(m_GuiTexture.color, targetColor, percentage);

        if (Mathf.Abs(m_GuiTexture.color.a - m_TargetAlpha) < Mathf.Epsilon)
        {
            m_GuiTexture.color = targetColor;
            m_GuiTexture.enabled = false;

            m_Callback();
        }
        else
        {
            m_ElapsedTime += Time.deltaTime;
        }
    }

    // BUSINESS LOGIC

    public void Initialize()
    {

    }

    public void FadeToBlack(float i_Duration, Action i_Callback = null)
    {
        FadeTo(1f, i_Duration, i_Callback);
    }

    public void Clear(float i_Duration, Action i_Callback = null)
    {
        FadeTo(0f, i_Duration, i_Callback);
    }

    // INTERNALS

    private void FadeTo(float i_TargetAlpha, float i_Duration, Action i_Callback = null)
    {
        m_TargetAlpha = i_TargetAlpha;
        m_ElapsedTime = 0f;
        m_FadeDuration = i_Duration;

        m_Callback = i_Callback;

        m_GuiTexture.enabled = true;
    }
}

#pragma warning restore 618