using UnityEngine;

using WiFiInput.Common;

namespace WiFiInput.Server
{
    public class AxisServerController : WiFiServerController
    {
        private AxisControllerType m_Controller = null;

        // LOGIC

        public void Initialize()
        {
            m_Controller = WiFiInputUtilities.checkForClient<AxisControllerType>(controlName, (int)playerNumber);
        }

        public float GetValue()
        {
            if (m_Controller != null)
            {
                float value = m_Controller.AXIS_VALUE;
                value = Mathf.Clamp(value, -1f, 1f);
                return value;
            }

            return 0f;
        }

        // WiFiServerController's interface

        public override void OnConnectionsChanged()
        {
            m_Controller = WiFiInputUtilities.checkForClient<AxisControllerType>(controlName, (int)playerNumber);
        }

        // CTOR

        public AxisServerController(string i_ControlName, PLAYER_NUMBER i_PlayerNumber)
            : base(i_ControlName, i_PlayerNumber)
        {

        }
    }
}
