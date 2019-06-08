using UnityEngine;
using System;

using StateMachine;

public abstract class tnFSM<T> : tnRunnableFSM where T : struct, IConvertible, IComparable
{
    private StateMachine<T> m_Fsm = null;

    protected StateMachine<T> fsm
    {
        get
        {
            return m_Fsm;
        }
    }

    // MonoBehaviour's INTERFACE

    protected virtual void Awake()
    {
        m_Fsm = StateMachine<T>.Initialize(this, default(T));
    }

    // btIRunnableFSM's INTERFACE

    private OnFsmReturn m_FsmReturnedEvent = null;

    public override event OnFsmReturn fsmReturnedEvent
    {
        add
        {
            m_FsmReturnedEvent += value;
        }

        remove
        {
            m_FsmReturnedEvent -= value;
        }
    }

    public override void StartFSM()
    {
        m_Fsm.ChangeState(startingState);
        OnFSMStarted();
    }

    // PROTECTED

    protected void Return()
    {
        fsm.ChangeState(default(T));

        OnFSMReturn();

        if (m_FsmReturnedEvent != null)
        {
            m_FsmReturnedEvent();
        }
    }

    // btFSM<T>'s INTERFACE

    protected abstract T startingState { get; }

    protected virtual void OnFSMStarted()
    {

    }

    protected virtual void OnFSMReturn()
    {

    }
}
