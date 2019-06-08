using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineEraser : MonoBehaviour
{
    private Renderer m_Renderer = null;
    private OutlineCamera m_OutlineCamera = null;

    private bool m_Binded = false;

    // MonoBehaviour 's interface

    void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            m_OutlineCamera = cam.GetComponent<OutlineCamera>();
        }

        Bind();
    }

    void OnDisable()
    {
        Unbind();

        m_OutlineCamera = null;
    }

    // LOGIC

    private void Bind()
    {
        if (m_OutlineCamera != null)
        {
            if (!m_Binded)
            {
                m_OutlineCamera.RegisterEraseRenderer(m_Renderer);
                m_Binded = true;
            }
        }
    }

    private void Unbind()
    {
        if (m_OutlineCamera != null)
        {
            if (m_Binded)
            {
                m_OutlineCamera.UnregisterEraseRenderer(m_Renderer);
                m_Binded = false;
            }
        }
    } 
}
