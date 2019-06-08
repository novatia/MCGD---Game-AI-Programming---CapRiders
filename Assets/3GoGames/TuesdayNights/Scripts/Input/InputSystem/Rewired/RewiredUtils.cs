#if INPUT_REWIRED

using UnityEngine;
using System.Collections;

using Rewired;
using InputUtils;

namespace RewiredUtils
{
    public static class Types
    {
        public static InputSourceType GetInputSourceType(ControllerType i_Type)
        {
            InputSourceType outputType = InputSourceType.eNone;

            switch (i_Type)
            {
                case ControllerType.Joystick:
                    outputType = InputSourceType.eJoystick;
                    break;

                case ControllerType.Keyboard:
                    outputType = InputSourceType.eKeyboard;
                    break;

                case ControllerType.Mouse:
                    outputType = InputSourceType.eMouse;
                    break;
            }

            return outputType;
        }

        public static ControllerType GetControllerType(InputSourceType i_Type)
        {
            ControllerType outputType = ControllerType.Custom;

            switch (i_Type)
            {
                case InputSourceType.eJoystick:
                    outputType = ControllerType.Joystick;
                    break;

                case InputSourceType.eKeyboard:
                    outputType = ControllerType.Keyboard;
                    break;

                case InputSourceType.eMouse:
                    outputType = ControllerType.Mouse;
                    break;
            }

            return outputType;
        }
    }
}

#endif // INPUT_REWIRED