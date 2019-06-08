using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class tnCharacterAnimation : MonoBehaviour
{
    private Animator m_Animator = null;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.speed = Random.Range(0.5f, 2f);
    }
}
