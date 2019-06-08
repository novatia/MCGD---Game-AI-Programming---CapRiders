using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour 
{
    [SerializeField]
    private float m_TargetAspectRatio = 1920f / 1080f;

    private Camera m_Camera = null;

    private int m_Width = -1;
    private int m_Height = -1;

    private bool m_Fullscreen = true;

    void Awake()
    {
        m_Camera = (Camera)GetComponent(typeof(Camera));
    }

	void Start() 
    {
        Refresh(true); // Force refresh.
	}

    void LateUpdate()
    {
        Refresh();
    }

    // BUSINESS LOGIC

    public void SetRatio(float i_Ratio)
    {
        m_TargetAspectRatio = i_Ratio;
    }

    // INTERNALS

    private void Refresh(bool i_Forced = false)
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        bool isFullscreen = Screen.fullScreen;

        if (i_Forced || (screenWidth != m_Width || screenHeight != m_Height || isFullscreen != m_Fullscreen))
        {
            float currentAspectRatio = (float)screenWidth / screenHeight;

            float scaleHeight = currentAspectRatio / m_TargetAspectRatio;

            if (scaleHeight < 1f) // Letterbox
            {
                Rect rect = m_Camera.rect;

                rect.x = 0f;
                rect.y = (1f - scaleHeight) / 2f;
                rect.width = 1f;
                rect.height = scaleHeight;

                m_Camera.rect = rect;
            }
            else // Pillarbox
            {
                float scaleWidth = 1f / scaleHeight;

                Rect rect = m_Camera.rect;

                rect.x = (1f - scaleWidth) / 2f;
                rect.y = 0f;
                rect.width = scaleWidth;
                rect.height = 1f;

                m_Camera.rect = rect;
            }

            // LogManager.Log(this, LogContexts.Camera, "REFRESH RECT: " + "[" + m_Width + "x" + m_Height + "@" + m_Fullscreen + "]" + " --> " + "[" + screenWidth + "x" + screenHeight + "@" + isFullscreen + "]");

            m_Width = screenWidth;
            m_Height = screenHeight;

            m_Fullscreen = isFullscreen;

        }
    }
}
