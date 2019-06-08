using System;

using WiFiInput.Common;

namespace WiFiInput.Client
{
    //Here we define the structure of the controller types "over the wire" or in another words the network representation of all

    public class AxisControllerType : BaseControllerType
    {
        //A Joystick is a simple control with a float for two axis going between -1 and 1 (normalized
        //just a simple string representation on both ends

        public float AXIS_VALUE;

        public override string mapStructureToNetworkData()
        {
            string message = "";

            message += Convert.ToDecimal(AXIS_VALUE).ToString() + WiFiInputConstants.SPLITMESSAGE_NEWLINE;

            return message;
        }
    }
}
