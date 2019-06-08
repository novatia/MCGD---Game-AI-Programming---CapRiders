using WiFiInput.Common;

namespace WiFiInput.Server
{
    public abstract class WiFiServerController
    {
        private string m_ControlName = "";
        private PLAYER_NUMBER m_PlayerNumber = PLAYER_NUMBER.Player1;

        public string controlName
        {
            get { return m_ControlName; }
        }

        public PLAYER_NUMBER playerNumber
        {
            get { return m_PlayerNumber; }
        }

        // ABSTRACTS

        public abstract void OnConnectionsChanged();

        // CTOR

        public WiFiServerController(string i_ControlName, PLAYER_NUMBER i_PlayerNumber)
        {
            m_ControlName = i_ControlName;
            m_PlayerNumber = i_PlayerNumber;
        }
    }
}
