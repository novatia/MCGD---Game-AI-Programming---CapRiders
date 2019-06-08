using WiFiInput.Common;

namespace WiFiInput.Server
{
    public class ButtonServerController : WiFiServerController
    {
        ButtonControllerType m_Controller = null;

        // LOGIC

        public void Initialize()
        {
            m_Controller = WiFiInputUtilities.checkForClient<ButtonControllerType>(controlName, (int)playerNumber);
        }

        public bool GetButton()
        {
            if (m_Controller != null)
            {
                return m_Controller.BUTTON_STATE_IS_PRESSED;
            }

            return false;
        }

        // WiFiServerController's interface

        public override void OnConnectionsChanged()
        {
            m_Controller = WiFiInputUtilities.checkForClient<ButtonControllerType>(controlName, (int)playerNumber);
        }

        // CTOR

        public ButtonServerController(string i_ControlName, PLAYER_NUMBER i_PlayerNumber)
            : base (i_ControlName, i_PlayerNumber)
        {

        }
    }
}
