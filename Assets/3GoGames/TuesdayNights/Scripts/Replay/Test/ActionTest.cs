using UnityEngine;

using System;

public class ActionTest : MonoBehaviour
{
    [SerializeField]
    private KeyCode m_ActionKey = KeyCode.A;
    [SerializeField]
    private Effect m_ActionEffect = null;

    public Action onAction = null;

    // MonoBehaviour's interface

    void Update()
    {
        if (Input.GetKeyDown(m_ActionKey))
        {
            DoAction();
        }
    }

    // LOGIC

    public void ForceEffect()
    {
        EffectUtils.PlayEffect2D(m_ActionEffect, transform);
    }

    // INTERNALS

    private void DoAction()
    {
        EffectUtils.PlayEffect2D(m_ActionEffect, transform);

        if (onAction != null)
        {
            onAction();
        }
    }
}
