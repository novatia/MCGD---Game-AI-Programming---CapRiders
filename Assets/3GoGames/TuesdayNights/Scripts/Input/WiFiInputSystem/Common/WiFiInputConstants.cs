namespace WiFiInput.Common
{
    public enum ControlType
    {
        Button,
        Axis,
    }

    public enum CURRENT_CLIENT_STATE { Broadcasting = 0, ServerFound = 1, SendingInventory = 2, SendingControllerData = 3, NotConnected = 4 };
    public enum CURRENT_SERVER_STATE { Listening = 0, Disconnecting = 1 };
    public enum PLAYER_NUMBER { Player1 = 0, Player2 = 1, Player3 = 2, Player4 = 3, Player5 = 4, Player6 = 5, Player7 = 6, Player8 = 7, Player9 = 8, Player10 = 9, Player11 = 10, Player12 = 11, Player13 = 12, Player14 = 13, Player15 = 14, Player16 = 15, AnyPlayer = 999 };
    public enum AXIS { XAxis = 0, YAxis = 1, ZAxis = 2, NegativeXAxis = 3, NegativeYAxis = 4, NegativeZAxis = 5, None = 6 };
    public enum SECOND_TOUCH_TYPE { Tilt = 0, Pan = 1 };
    public enum PEER_TYPE { Server = 0, Client = 1 };
    public enum UNITY_UI_INPUT_TYPE { Joystick = 0 };
    public enum UNITY_UI_SELECTION_TYPE { Button = 0 };
    public enum SLIDER_TYPE { Horizonal = 0, Vertical = 1 };
    public enum CALL_TYPE { Every_Frame = 0, Only_When_Changed = 1 };
    public enum CONTROLLERDATA_MAX_SEND_RATE { CapAt90Hz = 0, CapAt60Hz = 1, CapAt30Hz = 2, SendAsFastAsPossible = 3, SendInfrequentlyOncePerSecond = 4 };

    public static class WiFiInputConstants
    {
        //peer types
        public const string PEERTYPE_CLIENT = "Client";
        public const string PEERTYPE_SERVER = "Server";

        //message types
        public const string MESSAGETYPE_BROADCAST = "Client_Broadcast";
        public const string MESSAGETYPE_SERVER_RESPONSE_BROADCAST = "Broadcast_ServerResponse";
        public const string MESSAGETYPE_CLIENT_INVENTORY = "Client_Inventory";
        public const string MESSAGETYPE_SERVER_RESPONSE_INVENTORY = "Inventory_ServerResponse";
        public const string MESSAGETYPE_CONTROLLER_DATA = "Client_ControllerData";
        public const string MESSAGETYPE_BACKCHANNEL_DATA = "Server_BackchannelData";
        public const string MESSAGETYPE_HEARTBEAT = "Server_Heartbeat";
        public const string MESSAGETYPE_DISCONNECT = "Client_Disconnect";
        public const string MESSAGETYPE_CLIENTLOG = "Client_Log";

        //controller types
        public const string CONTROLLERTYPE_AXIS = "Axis";
        public const string CONTROLLERTYPE_BUTTON = "Button";

        //other constants
        public const int ROLLOVER_PACKET_NUMBER = 10000;
        public const int CHECK_FOR_ROLLOVER_HIGH = 9000;
        public const int CHECK_FOR_ROLLOVER_LOW = 1000;
        public const string BACKCHANNEL_FILTER = "Backchannel_";

        //common message splits(string)
        public const string SPLITMESSAGE_POUND = "#";
        public const string SPLITMESSAGE_COLON = ":";
        public const string SPLITMESSAGE_COMMA = ",";
        public const string SPLITMESSAGE_NEWLINE = "\n";

        //split arrays
        public static readonly string[] SPLITARRAY_POUND = { WiFiInputConstants.SPLITMESSAGE_POUND };
        public static readonly string[] SPLITARRAY_COLON = { WiFiInputConstants.SPLITMESSAGE_COLON };
        public static readonly string[] SPLITARRAY_COMMA = { WiFiInputConstants.SPLITMESSAGE_COMMA };
        public static readonly string[] SPLITARRAY_NEWLINE = { WiFiInputConstants.SPLITMESSAGE_NEWLINE };

        //player number
        public static int PLAYERNUMBER_DISCONNECTED = -99;
        public static int PLAYERNUMBER_ANY = 999;
        public static float SIMULATED_TOUCH = -99f;

        //number of duplicate controllers allowed
        public static int MAX_CONTROLLERS = 16;
    }

}
