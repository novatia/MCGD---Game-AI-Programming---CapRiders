using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineObject : MonoBehaviour
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

    // INTERNALS

    private void Bind()
    {
        if (m_OutlineCamera != null)
        {
            if (!m_Binded)
            {
                m_OutlineCamera.RegisterOutlineRenderer(m_Renderer);
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
                m_OutlineCamera.UnregisterOutlineRenderer(m_Renderer);
                m_Binded = false;
            }
        }
    }
}
