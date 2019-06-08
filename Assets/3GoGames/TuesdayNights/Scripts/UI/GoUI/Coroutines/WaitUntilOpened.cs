using UnityEngine;
using System.Collections;

namespace GoUI
{

public class WaitUntilOpened : CustomYieldInstruction
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

            return m_Panel.isViewOpening;
        }
    }

    public WaitUntilOpened(UIBasePanel i_Panel)
    {
        m_Panel = i_Panel;
    }
}

}