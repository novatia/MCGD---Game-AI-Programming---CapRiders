#if INPUT_REWIRED

using UnityEngine;
using System.Collections;

using InputUtils;

using Rewired;

public sealed class RewiredPlayerInputImpl : IPlayerInputImpl
{

#region Private fields

    private Player _RewiredPlayer = null;

#endregion

#region Properties

    public int Id 
    {
        get
        {
            if (_RewiredPlayer != null)
            {
                return _RewiredPlayer.id;
            }

            return -1;
        }
    }

    public string Name
    {
        get
        {
            if (_RewiredPlayer != null)
            {
                return _RewiredPlayer.name;
            }

            return "INVALID_NAME";
        }
    }

    public string DescriptiveName
    {
        get
        {
            if (_RewiredPlayer != null)
            {
                return _RewiredPlayer.descriptiveName;
            }

            return "INVALID_NAME";
        }
    }

    public int JoystickCount
    {
        get
        {
            if (_RewiredPlayer != null)
            {
                Player.ControllerHelper controller = _RewiredPlayer.controllers;
                if (controller != null)
                {
                    return controller.joystickCount;
                }
            }

            return -1;
        }
    }

    public bool bIsPlaying
    {
        get
        {
            if (_RewiredPlayer != null)
            {
                return _RewiredPlayer.isPlaying;
            }

            return false;
        }
    }

    public bool HasMouse
    {
        get
        {
            if (_RewiredPlayer != null)
            {
                Player.ControllerHelper controller = _RewiredPlayer.controllers;
                if (controller != null)
                {
                    return controller.hasMouse;
                }
            }

            return false;
        }

        set
        {
            if (_RewiredPlayer != null)
            {
                Player.ControllerHelper controller = _RewiredPlayer.controllers;
                if (controller != null)
                {
                    controller.hasMouse = value;
                }
            }
        }
    }

#endregion // Properties

#region Axis

    public float GetAxis(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxis(i_ActionName);
        }

        return 0f;
    }

    public float GetAxis(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxis(i_ActionId);
        }

        return 0f;
    }

    public float GetAxisPrev(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxisPrev(i_ActionName);
        }

        return 0f;
    }

    public float GetAxisPrev(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxisPrev(i_ActionId);
        }

        return 0f;
    }

    public float GetAxisTimeActive(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxisTimeActive(i_ActionName);
        }

        return 0f;
    }

    public float GetAxisTimeActive(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxisTimeActive(i_ActionId);
        }

        return 0f;
    }

    public float GetAxisTimeInactive(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxisTimeInactive(i_ActionName);
        }

        return 0f;
    }

    public float GetAxisTimeInactive(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxisTimeInactive(i_ActionId);
        }

        return 0f;
    }

    public float GetAxisRaw(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxisRaw(i_ActionName);
        }

        return 0f;
    }

    public float GetAxisRaw(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetAxisRaw(i_ActionId);
        }

        return 0f;
    }

#endregion // Axis

#region Positive Button

    public bool GetButton(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButton(i_ActionName);
        }

        return false;
    }

    public bool GetButton(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButton(i_ActionId);
        }

        return false;
    }

    public bool GetButtonDown(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonDown(i_ActionName);
        }

        return false;
    }

    public bool GetButtonDown(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonDown(i_ActionId);
        }

        return false;
    }

    public bool GetButtonUp(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonUp(i_ActionName);
        }

        return false;
    }

    public bool GetButtonUp(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonUp(i_ActionId);
        }

        return false;
    }

    public bool GetButtonPrev(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonPrev(i_ActionName);
        }

        return false;
    }

    public bool GetButtonPrev(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonPrev(i_ActionId);
        }

        return false;
    }

    public bool GetButtonDoublePressDown(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonDoublePressDown(i_ActionName);
        }

        return false;
    }

    public bool GetButtonDoublePressDown(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonDoublePressDown(i_ActionId);
        }

        return false;
    }

    public bool GetButtonDoublePressHold(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonDoublePressHold(i_ActionName);
        }

        return false;
    }

    public bool GetButtonDoublePressHold(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonDoublePressHold(i_ActionId);
        }

        return false;
    }

    public float GetButtonTimePressed(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonTimePressed(i_ActionName);
        }

        return 0f;
    }

    public float GetButtonTimePressed(int i_ActionId)
    {

        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonTimePressed(i_ActionId);
        }

        return 0f;
    }

    public float GetButtonTimeUnpressed(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonTimeUnpressed(i_ActionName);
        }

        return 0f;
    }

    public float GetButtonTimeUnpressed(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetButtonTimeUnpressed(i_ActionId);
        }

        return 0f;
    }

#endregion // Positive Button

#region Negative Button

    public bool GetNegativeButton(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButton(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButton(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButton(i_ActionId);
        }

        return false;
    }

    public bool GetNegativeButtonDown(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonDown(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonDown(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonDown(i_ActionId);
        }

        return false;
    }

    public bool GetNegativeButtonUp(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonUp(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonUp(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonUp(i_ActionId);
        }

        return false;
    }

    public bool GetNegativeButtonPrev(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonPrev(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonPrev(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonPrev(i_ActionId);
        }

        return false;
    }

    public bool GetNegativeButtonDoublePressDown(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonDoublePressDown(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonDoublePressDown(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonDoublePressDown(i_ActionId);
        }

        return false;
    }

    public bool GetNegativeButtonDoublePressHold(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonDoublePressHold(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonDoublePressHold(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonDoublePressHold(i_ActionId);
        }

        return false;
    }

    public float GetNegativeButtonTimePressed(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonTimePressed(i_ActionName);
        }

        return 0f;
    }

    public float GetNegativeButtonTimePressed(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonTimePressed(i_ActionId);
        }

        return 0f;
    }

    public float GetNegativeButtonTimeUnpressed(string i_ActionName)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonTimeUnpressed(i_ActionName);
        }

        return 0f;
    }

    public float GetNegativeButtonTimeUnpressed(int i_ActionId)
    {
        if (_RewiredPlayer != null)
        {
            return _RewiredPlayer.GetNegativeButtonTimeUnpressed(i_ActionId);
        }

        return 0f;
    }

#endregion // Negative Button

#region Rumble

    public void SetVibration(float i_Left, float i_Right)
    {
        if (!ReInput.isReady)
            return;

        if (_RewiredPlayer != null)
        {
            for (int index = 0; index < _RewiredPlayer.controllers.Joysticks.Count; ++index)
            {
                Joystick joystick = _RewiredPlayer.controllers.Joysticks[index];
                if (joystick != null)
                {
                    if (!joystick.supportsVibration)
                        continue;

                    joystick.SetVibration(i_Left, i_Right);
                }
            }
        }
    }

    public void StopVibration()
    {
        if (!ReInput.isReady)
            return;

        if (_RewiredPlayer != null)
        {
            for (int index = 0; index < _RewiredPlayer.controllers.Joysticks.Count; ++index)
            {
                Joystick joystick = _RewiredPlayer.controllers.Joysticks[index];
                if (joystick != null)
                {
                    if (!joystick.supportsVibration)
                        continue;

                    joystick.StopVibration();
                }
            }
        }
    }

#endregion

#region Maps

    public void EnableAllMaps()
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetAllMapsEnabled(true);
                }
            }
        }
    }

    public void EnableAllMapsFor(InputSourceType i_Type)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetAllMapsEnabled(true, RewiredUtils.Types.GetControllerType(i_Type));
                }
            }
        }
    }

    public void EnableMaps(int i_Category)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(true, i_Category);
                }
            }
        }
    }

    public void EnableMaps(string i_Category)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(true, i_Category);
                }
            }
        }
    }

    public void EnableMaps(string i_Category, string i_Layout)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(true, i_Category, i_Layout);
                }
            }
        }
    }

    public void EnableMaps(InputSourceType i_Type, int i_Category)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(true, RewiredUtils.Types.GetControllerType(i_Type), i_Category);
                }
            }
        }
    }

    public void EnableMaps(InputSourceType i_Type, string i_Category)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(true, RewiredUtils.Types.GetControllerType(i_Type), i_Category);
                }
            }
        }
    }

    public void EnableMaps(InputSourceType i_Type, string i_Category, string i_Layout)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(true, RewiredUtils.Types.GetControllerType(i_Type), i_Category, i_Layout);
                }
            }
        }
    }

    public void DisableAllMaps()
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetAllMapsEnabled(false);
                }
            }
        }
    }

    public void DisableAllMapsFor(InputSourceType i_Type)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetAllMapsEnabled(false, RewiredUtils.Types.GetControllerType(i_Type));
                }
            }
        }
    }

    public void DisableMaps(int i_Category)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(false, i_Category);
                }
            }
        }
    }

    public void DisableMaps(string i_Category)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(false, i_Category);
                }
            }
        }
    }

    public void DisableMaps(string i_Category, string i_Layout)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(false, i_Category, i_Layout);
                }
            }
        }
    }

    public void DisableMaps(InputSourceType i_Type, int i_Category)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(false, RewiredUtils.Types.GetControllerType(i_Type), i_Category);
                }
            }
        }
    }

    public void DisableMaps(InputSourceType i_Type, string i_Category)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(false, RewiredUtils.Types.GetControllerType(i_Type), i_Category);
                }
            }
        }
    }

    public void DisableMaps(InputSourceType i_Type, string i_Category, string i_Layout)
    {
        if (_RewiredPlayer != null)
        {
            Player.ControllerHelper controllers = _RewiredPlayer.controllers;
            if (controllers != null)
            {
                Player.ControllerHelper.MapHelper maps = controllers.maps;
                if (maps != null)
                {
                    maps.SetMapsEnabled(false, RewiredUtils.Types.GetControllerType(i_Type), i_Category, i_Layout);
                }
            }
        }
    }

#endregion // Maps

#region Constructor

    public RewiredPlayerInputImpl(string i_Name)
    {
        if (ReInput.players != null)
        {
            _RewiredPlayer = ReInput.players.GetPlayer(i_Name);
        }
    }

#endregion // Constructor

}

#endif // INPUT_REWIRED