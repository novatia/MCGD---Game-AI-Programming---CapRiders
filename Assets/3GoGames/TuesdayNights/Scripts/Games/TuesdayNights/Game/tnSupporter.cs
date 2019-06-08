using UnityEngine;
using System.Collections;

public class tnSupporter : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer[] m_Sprites = null;

    // BUSINESS LOGIC

    public void SetColor(Color i_Color)
    {
        foreach (SpriteRenderer s in m_Sprites)
        {
            if (s != null)
            {
                s.color = i_Color;
            }
        }
    }
}
