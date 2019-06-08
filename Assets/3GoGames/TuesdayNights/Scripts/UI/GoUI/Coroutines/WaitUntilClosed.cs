using UnityEngine;
using System.Collections;

namespace GoUI
{

public class WaitUntilClosed : CustomYieldInstruction
{
    private UIBasePanel m_Panel = null;

    public override bool keepWaiting
    {
        get
        {
            if (m_Panel == null)
            {
                return false;
            }

            return m_Panel.isViewClosing;
        }
    }

    public WaitUntilClosed(UIBasePanel i_Panel)
    {
        m_Panel = i_Panel;
    }
}

}