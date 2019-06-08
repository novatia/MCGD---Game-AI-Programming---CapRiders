using System;

using WiFiInput.Common;

namespace WiFiInput.Client
{
    public class AxisClientController : WiFiClientController
    {
        private string m_ControlName = "";
        
        public string controlName
        {
            get { return m_ControlName; }
        }

        // LOGIC

        public void Initialize()
        {

        }

        // WiFiClientController's interface

        public override void OnUpdate()
        {

        }

        protected override void mapInputToDataStream()
        {

        }

        // CTOR

        public AxisClientController(string i_ControlName)
        {
            m_ControlName = i_ControlName;
        }
    }
}