using TuesdayNights;

public class tnPlayerInputFiller : tnInputFiller
{
    private PlayerInput m_PlayerInput = null;

    private FilteredFloat m_HorizontalFilter;       // Horizontal axis filter - It avoids mechanical errors of joypad's stick.
    private FilteredFloat m_VerticalFilter;         // Vertical axis filter - It avoids mechanical errors of joypad's stick.

    // tnInputFiller's INTERFACE

    public override void Fill(float i_FrameTime, tnInputData i_Data)
    {
        if (m_PlayerInput == null)
        {
            return;
        }

        float horizontalAxisRaw = m_PlayerInput.GetAxis("MoveHorizontal");
        float verticalAxisRaw = m_PlayerInput.GetAxis("MoveVertical");

        m_HorizontalFilter.Step(horizontalAxisRaw, i_FrameTime);
        m_VerticalFilter.Step(verticalAxisRaw, i_FrameTime);

        i_Data.SetAxis(InputActions.s_HorizontalAxis, m_HorizontalFilter.position);
        i_Data.SetAxis(InputActions.s_VerticalAxis, m_VerticalFilter.position);

        i_Data.SetButton(InputActions.s_ShotButton, m_PlayerInput.GetButton("Shot"));
        i_Data.SetButton(InputActions.s_PassButton, m_PlayerInput.GetButton("Pass"));
        i_Data.SetButton(InputActions.s_AttractButton, m_PlayerInput.GetButton("Attract"));

        i_Data.SetButton(InputActions.s_TauntButton, m_PlayerInput.GetButton("Taunt"));
    }

    public override void Clear()
    {
        m_HorizontalFilter.Reset(0f);
        m_VerticalFilter.Reset(0f);
    }

    public override void DrawGizmos()
    {

    }

    public override void DrawGizmosSelected()
    {

    }
    
    // CTOR

    public tnPlayerInputFiller(PlayerInput i_PlayerInput)
    {
        m_PlayerInput = i_PlayerInput;

        m_HorizontalFilter = new FilteredFloat(0.01f, 0.01f);
        m_VerticalFilter = new FilteredFloat(0.01f, 0.01f);
    }
}
