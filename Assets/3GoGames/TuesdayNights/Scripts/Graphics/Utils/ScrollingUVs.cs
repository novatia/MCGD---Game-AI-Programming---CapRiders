using UnityEngine;
using UnityEngine.UI;

using System.Collections;

[RequireComponent(typeof(Renderer))]
public class ScrollingUVs : MonoBehaviour
{
    public float scrollSpeedX = 0.05f;
    public float scrollSpeedY = 0.05f;

    private Vector2 m_SavedOffset;

    private Renderer m_Renderer = null;

    void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        m_SavedOffset = m_Renderer.material.GetTextureOffset("_MainTex");
    }

    void Update()
    {
        float y = Mathf.Repeat(Time.time * scrollSpeedY, 1f);

        Vector2 offset = new Vector2(m_SavedOffset.x, y);
        m_Renderer.material.SetTextureOffset("_MainTex", offset);
    }

    void OnDisable()
    {
        m_Renderer.material.SetTextureOffset("_MainTex", m_SavedOffset);
    }
}