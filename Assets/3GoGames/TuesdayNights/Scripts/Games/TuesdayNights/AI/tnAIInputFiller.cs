using UnityEngine;

using TuesdayNights;

public abstract class tnAIInputFiller : tnInputFiller
{
    private GameObject m_Self = null;

    public GameObject self
    {
        get { return m_Self; }
    }

    // PROTECTED

    protected void ResetInputData(tnInputData i_Data)
    {
        if (i_Data == null)
            return;

        i_Data.SetAxis(InputActions.s_HorizontalAxis, 0f);
        i_Data.SetAxis(InputActions.s_VerticalAxis, 0f);

        i_Data.SetButton(InputActions.s_PassButton, false);
        i_Data.SetButton(InputActions.s_ShotButton, false);
        i_Data.SetButton(InputActions.s_AttractButton, false);
        i_Data.SetButton(InputActions.s_TauntButton, false);
        i_Data.SetButton(InputActions.s_StartButton, false);
    }

    // CTOR

    public tnAIInputFiller(GameObject i_Self)
    {
        m_Self = i_Self;
    }
}
