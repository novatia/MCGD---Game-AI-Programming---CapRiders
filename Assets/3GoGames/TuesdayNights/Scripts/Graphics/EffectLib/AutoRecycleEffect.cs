using UnityEngine;

using System.Collections;

[RequireComponent(typeof( Effect ))]
public class AutoRecycleEffect : MonoBehaviour
{
    private Effect m_Effect = null;

    void Awake()
    {
        m_Effect = GetComponent<Effect>();
    }

    void Update()
    {
        if (!m_Effect.isPlaying)
        {
            DestroyEffect();
        }
    }

    private void DestroyEffect()
    {
        m_Effect.Recycle<Effect>();
    }
}