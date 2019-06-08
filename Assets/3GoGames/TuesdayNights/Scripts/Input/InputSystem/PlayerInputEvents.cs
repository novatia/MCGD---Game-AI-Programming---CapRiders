using UnityEngine;
using System.Collections;

using InputUtils;

namespace PlayerInputEvents
{
    public struct ControllerEventParams
    {
        private string _name;
        private int _controllerId;
        private InputSourceType _controllerType;

        public string name { get { return _name; } }
        public int controllerId { get { return _controllerId; } }
        public InputSourceType controllerType { get { return _controllerType; } }

        public ControllerEventParams(string name, int controllerId, InputSourceType controllerType)
        {
            _name = name;
            _controllerId = controllerId;
            _controllerType = controllerType;
        }
    }

    public delegate void OnControllerConnected(ControllerEventParams i_Params);
    public delegate void OnControllerDisconnected(ControllerEventParams i_Params);

    public delegate void OnEditorRecompileEvent();

    public delegate void OnInputSystemReset();
}
