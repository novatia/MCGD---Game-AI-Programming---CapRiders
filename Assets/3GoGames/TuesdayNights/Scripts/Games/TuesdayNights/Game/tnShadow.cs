using UnityEngine;
using System.Collections;

public class tnShadow : MonoBehaviour
{
    [SerializeField]
    private Sprite m_Sprite = null;
    [SerializeField]
    [Range(0f, 0.1f)]
    private float m_Offset = 0.005f;

    private Transform m_Anchor = null;

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        GameObject anchorGo = new GameObject("Shadow");

        SpriteRenderer spriteRenderer = anchorGo.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = m_Sprite;

        m_Anchor = anchorGo.transform;

        float targetZ;

        GameObject fieldGo = GameObject.FindGameObjectWithTag("Field");
        if (fieldGo != null)
        {
            targetZ = fieldGo.transform.position.z - m_Offset;
        }
        else
        {
            targetZ = 10f - m_Offset;
        }

        m_Anchor.position = new Vector3(0f, 0f, targetZ);

        // Initialize position.

        AnchorPosition();
    }

    void OnEnable()
    {
        if (m_Anchor != null)
        {
            m_Anchor.gameObject.SetActive(true);
        }
    }

    void OnDisable()
    {
        if (m_Anchor != null)
        {
            m_Anchor.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        AnchorPosition();
    }

    // INTERNALS

    private void AnchorPosition()
    {
        Vector3 newPosition = m_Anchor.position;
        newPosition.x = transform.position.x;
        newPosition.y = transform.position.y;

        m_Anchor.localScale = transform.localScale;

        m_Anchor.position = newPosition;
    }
}
