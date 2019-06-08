using UnityEngine;
using System.Collections;

public class tnAnchorToField : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 0.1f)]
    private float m_Offset = 0.005f;

    private Transform m_Anchor = null;

	void Awake()
    {
        GameObject fieldGo = GameObject.FindGameObjectWithTag("Field");
        if (fieldGo != null)
        {
            m_Anchor = fieldGo.transform;
        }

        AnchorPosition();
	}

    void Update()
    {
        AnchorPosition();
    }

    // INTERNALS

    private void AnchorPosition()
    {
        if (m_Anchor == null)
            return;

        Vector3 newPosition = transform.position;
        newPosition.z = m_Anchor.position.z;
        newPosition.z -= m_Offset; // Add a small offset towards the camera.

        transform.position = newPosition;
    }
}
