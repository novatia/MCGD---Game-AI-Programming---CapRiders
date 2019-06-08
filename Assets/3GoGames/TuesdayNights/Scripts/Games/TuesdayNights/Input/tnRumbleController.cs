public class tnRumbleController
{
    private PlayerInput m_PlayerInput = null;

    // LOGIC

    public void SetVibration(float i_Left, float i_Right)
    {
        if (m_PlayerInput != null)
        {
            m_PlayerInput.SetVibration(i_Left, i_Right);
        }
    }

    public void StopVibration()
    {
        if (m_PlayerInput != null)
        {
            m_PlayerInput.StopVibration();
        }
    }

    // CTOR

    public tnRumbleController(PlayerInput i_PlayerInput)
    {
        m_PlayerInput = i_PlayerInput;
    }
}
