using UnityEngine;
using System.Collections;

using WiFiInput.Server;

public static class tnInputUtils
{
    public static bool GetButtonDown(int i_PlayerId, string i_ActionId)
    {
        return GetButtonDown(i_PlayerId, i_ActionId, i_ActionId);
    }

    public static bool GetButtonDown(int i_PlayerId, int i_ActionId)
    {
        return GetButtonDown(i_PlayerId, i_ActionId, i_ActionId);
    }

    public static bool GetButtonDown(int i_PlayerId, string i_ActionId, string i_WiFiActionId)
    {
        PlayerInput playerInput;
        WiFiPlayerInput wifiPlayerInput;
        GetPlayersInputs(i_PlayerId, out playerInput, out wifiPlayerInput);

        bool action = false;

        if (playerInput != null)
        {
            action = playerInput.GetButtonDown(i_ActionId);
        }
        else
        {
            if (wifiPlayerInput != null)
            {
                action = wifiPlayerInput.GetButtonDown(i_WiFiActionId);
            }
        }

        return action;
    }

    public static bool GetButtonDown(int i_PlayerId, int i_ActionId, int i_WiFiActionId)
    {
        PlayerInput playerInput;
        WiFiPlayerInput wifiPlayerInput;
        GetPlayersInputs(i_PlayerId, out playerInput, out wifiPlayerInput);

        bool action = false;

        if (playerInput != null)
        {
            action = playerInput.GetButtonDown(i_ActionId);
        }
        else
        {
            if (wifiPlayerInput != null)
            {
                action = wifiPlayerInput.GetButtonDown(i_WiFiActionId);
            }
        }

        return action;
    }

    public static bool GetPositiveButtonDown(int i_PlayerId, string i_ActionId)
    {
        return GetPositiveButtonDown(i_PlayerId, i_ActionId, i_ActionId);
    }

    public static bool GetPositiveButtonDown(int i_PlayerId, int i_ActionId)
    {
        return GetPositiveButtonDown(i_PlayerId, i_ActionId, i_ActionId);
    }

    public static bool GetPositiveButtonDown(int i_PlayerId, string i_ActionId, string i_WiFiActionId)
    {
        PlayerInput playerInput;
        WiFiPlayerInput wifiPlayerInput;
        GetPlayersInputs(i_PlayerId, out playerInput, out wifiPlayerInput);

        bool action = false;

        if (playerInput != null)
        {
            action = playerInput.GetButtonDown(i_ActionId);
        }
        else
        {
            if (wifiPlayerInput != null)
            {
                action = wifiPlayerInput.GetPositiveButtonDown(i_WiFiActionId);
            }
        }

        return action;
    }

    public static bool GetPositiveButtonDown(int i_PlayerId, int i_ActionId, int i_WiFiActionId)
    {
        PlayerInput playerInput;
        WiFiPlayerInput wifiPlayerInput;
        GetPlayersInputs(i_PlayerId, out playerInput, out wifiPlayerInput);

        bool action = false;

        if (playerInput != null)
        {
            action = playerInput.GetButtonDown(i_ActionId);
        }
        else
        {
            if (wifiPlayerInput != null)
            {
                action = wifiPlayerInput.GetPositiveButtonDown(i_WiFiActionId);
            }
        }

        return action;
    }

    public static bool GetNegativeButtonDown(int i_PlayerId, string i_ActionId)
    {
        return GetNegativeButtonDown(i_PlayerId, i_ActionId, i_ActionId);
    }

    public static bool GetNegativeButtonDown(int i_PlayerId, int i_ActionId)
    {
        return GetNegativeButtonDown(i_PlayerId, i_ActionId, i_ActionId);
    }

    public static bool GetNegativeButtonDown(int i_PlayerId, string i_ActionId, string i_WiFiActionId)
    {
        PlayerInput playerInput;
        WiFiPlayerInput wifiPlayerInput;
        GetPlayersInputs(i_PlayerId, out playerInput, out wifiPlayerInput);

        bool action = false;

        if (playerInput != null)
        {
            action = playerInput.GetButtonDown(i_ActionId);
        }
        else
        {
            if (wifiPlayerInput != null)
            {
                action = wifiPlayerInput.GetNegativeButtonDown(i_WiFiActionId);
            }
        }

        return action;
    }

    public static bool GetNegativeButtonDown(int i_PlayerId, int i_ActionId, int i_WiFiActionId)
    {
        PlayerInput playerInput;
        WiFiPlayerInput wifiPlayerInput;
        GetPlayersInputs(i_PlayerId, out playerInput, out wifiPlayerInput);

        bool action = false;

        if (playerInput != null)
        {
            action = playerInput.GetNegativeButtonDown(i_ActionId);
        }
        else
        {
            if (wifiPlayerInput != null)
            {
                action = wifiPlayerInput.GetNegativeButtonDown(i_WiFiActionId);
            }
        }

        return action;
    }

    public static bool GetPlayersInputs(string i_PlayerId, out PlayerInput o_PlayerInput, out WiFiPlayerInput o_WifiPlayerInput)
    {
        o_PlayerInput = null;
        o_WifiPlayerInput = null;

        tnPlayerData playerData = tnGameData.GetPlayerDataMain(i_PlayerId);

        if (playerData == null)
        {
            return false;
        }

        string playerInputName = playerData.playerInputName;
        string wifiPlayerInputName = playerData.wifiPlayerInputName;

        PlayerInput playerInput = InputSystem.GetPlayerByNameMain(playerInputName);
        WiFiPlayerInput wifiPlayerInput = WiFiInputSystem.GetPlayerByNameMain(wifiPlayerInputName);

        o_PlayerInput = playerInput;
        o_WifiPlayerInput = wifiPlayerInput;

        return true;
    }

    public static bool GetPlayersInputs(int i_PlayerId, out PlayerInput o_PlayerInput, out WiFiPlayerInput o_WifiPlayerInput)
    {
        o_PlayerInput = null;
        o_WifiPlayerInput = null;

        if (Hash.IsNullOrEmpty(i_PlayerId))
        {
            return false;
        }

        tnPlayerData playerData = tnGameData.GetPlayerDataMain(i_PlayerId);

        if (playerData == null)
        {
            return false;
        }

        string playerInputName = playerData.playerInputName;
        string wifiPlayerInputName = playerData.wifiPlayerInputName;

        PlayerInput playerInput = InputSystem.GetPlayerByNameMain(playerInputName);
        WiFiPlayerInput wifiPlayerInput = WiFiInputSystem.GetPlayerByNameMain(wifiPlayerInputName);

        o_PlayerInput = playerInput;
        o_WifiPlayerInput = wifiPlayerInput;

        return true;
    }
}