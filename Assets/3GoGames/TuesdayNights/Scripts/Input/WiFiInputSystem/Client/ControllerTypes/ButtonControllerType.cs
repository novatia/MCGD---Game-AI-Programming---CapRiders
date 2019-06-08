using WiFiInput.Common;

namespace WiFiInput.Client
{
    //Here we define the structure of the controller types "over the wire" or in another words the network representation of all

    public class ButtonControllerType : BaseControllerType
    {
        //A button is a simple control and is really just pressed/not pressed in a bool
        //just a simple string representation on both ends so I'll just be using "0" for off and "1" for on

        public bool BUTTON_STATE_IS_PRESSED;

        public override string mapStructureToNetworkData()
        {
            string message = "";

            if (BUTTON_STATE_IS_PRESSED == true)
            {
                message += "1" + WiFiInputConstants.SPLITMESSAGE_NEWLINE;
            }
            else
            {
                message += "0" + WiFiInputConstants.SPLITMESSAGE_NEWLINE;
            }

            return message;
        }
    }
}