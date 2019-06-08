using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UISelectableInstruction : UINavbarEntry
{
    private Selectable m_Selectable = null;

    // MonoBehaviour's INTERFACE

    protected override void Awake()
    {
        base.Awake();

        m_Selectable = GetComponent<Selectable>();
    }

    // UINavbarEntry's INTERFACE

    public override bool isActive
    {
        get
        {
            return m_Selectable.IsInteractable();
        }
    }
}
