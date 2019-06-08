using UnityEngine;

using GoUI;

[RequireComponent(typeof(UIEventTrigger))]
public class UIEventTriggerInstruction : UINavbarEntry
{
    private UIEventTrigger m_Trigger = null;

    // MonoBehaviour's INTERFACE

    protected override void Awake()
    {
        base.Awake();

        m_Trigger = GetComponent<UIEventTrigger>();
    }

    // UINavbarEntry's INTERFACE

    public override bool isActive
    {
        get
        {
            return m_Trigger.canSend;
        }
    }
}
