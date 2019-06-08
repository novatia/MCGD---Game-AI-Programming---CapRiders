using System;

using WiFiInput.Common;

namespace WiFiInput.Server
{
    //Here we define the structure of the controller types "over the wire" or in another words the network representation of all

    public class AxisControllerType : BaseControllerType
    {
        //A Joystick is a simple control with a float for two axis going between -1 and 1 (normalized
        //just a simple string representation on both ends

        public float AXIS_VALUE;

        public override void mapNetworkDataToStructure(int packetNumber, string line)
        {
            //if we've received a packet on connection we thought was disconnected take action
            if (logicalPlayerNumber == WiFiInputConstants.PLAYERNUMBER_DISCONNECTED)
            {
                reuseOrGetAnotherConnection(previousConnectionPlayerNumber);
            }

            //remember when receiving the data the serverkey has already been parsed out to get the right
            //server instance in the data structure so we're left with datapoint,datapoint,datapoint...etc.
            if (isNewPacket(packetNumber))
            {
                lastPacketNumber = packetNumber;

                if (line != null && !line.Equals(string.Empty))
                {
                    AXIS_VALUE = (float)Convert.ToDecimal(line);
                }
            }
        }
    }
}
