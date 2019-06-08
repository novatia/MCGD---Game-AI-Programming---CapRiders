using UnityEngine;
using System.Collections;

using InputUtils;

public sealed class UPlayerInputImpl : IPlayerInputImpl
{
    #region Private fields

    private string m_Name = "INVALID_NAME";

    #endregion

    #region Properties

    public int Id
    {
        get
        {
            return -1;
        }
    }

    public string Name
    {
        get
        {
            return m_Name;
        }
    }

    public string DescriptiveName
    {
        get
        {
            return "INVALID_NAME";
        }
    }

    public int JoystickCount
    {
        get
        {
            return -1;
        }
    }

    public bool bIsPlaying
    {
        get
        {
            return false;
        }
    }

    public bool HasMouse
    {
        get
        {
            return true;
        }

        set
        {

        }
    }

    #endregion // Properties

    #region Axis

    public float GetAxis(string i_ActionName)
    {
        return 0f;
    }

    public float GetAxis(int i_ActionId)
    {
        return 0f;
    }

    public float GetAxisPrev(string i_ActionName)
    {
        return 0f;
    }

    public float GetAxisPrev(int i_ActionId)
    {
        return 0f;
    }

    public float GetAxisTimeActive(string i_ActionName)
    {
        return 0f;
    }

    public float GetAxisTimeActive(int i_ActionId)
    {
        return 0f;
    }

    public float GetAxisTimeInactive(string i_ActionName)
    {
        return 0f;
    }

    public float GetAxisTimeInactive(int i_ActionId)
    {
        return 0f;
    }

    public float GetAxisRaw(string i_ActionName)
    {
        return 0f;
    }

    public float GetAxisRaw(int i_ActionId)
    {
        return 0f;
    }

    #endregion // Axis

    #region Positive Button

    public bool GetButton(string i_ActionName)
    {
        return false;
    }

    public bool GetButton(int i_ActionId)
    {
        return false;
    }

    public bool GetButtonDown(string i_ActionName)
    {
        return false;
    }

    public bool GetButtonDown(int i_ActionId)
    {
        return false;
    }

    public bool GetButtonUp(string i_ActionName)
    {
        return false;
    }

    public bool GetButtonUp(int i_ActionId)
    {
        return false;
    }

    public bool GetButtonPrev(string i_ActionName)
    {
        return false;
    }

    public bool GetButtonPrev(int i_ActionId)
    {
        return false;
    }

    public bool GetButtonDoublePressDown(string i_ActionName)
    {
        return false;
    }

    public bool GetButtonDoublePressDown(int i_ActionId)
    {
        return false;
    }

    public bool GetButtonDoublePressHold(string i_ActionName)
    {
        return false;
    }

    public bool GetButtonDoublePressHold(int i_ActionId)
    {
        return false;
    }

    public float GetButtonTimePressed(string i_ActionName)
    {
        return 0f;
    }

    public float GetButtonTimePressed(int i_ActionId)
    {
        return 0f;
    }

    public float GetButtonTimeUnpressed(string i_ActionName)
    {
        return 0f;
    }

    public float GetButtonTimeUnpressed(int i_ActionId)
    {
        return 0f;
    }

    #endregion // Positive Button

    #region Negative Button

    public bool GetNegativeButton(string i_ActionName)
    {
        return false;
    }

    public bool GetNegativeButton(int i_ActionId)
    {
        return false;
    }

    public bool GetNegativeButtonDown(string i_ActionName)
    {
        return false;
    }

    public bool GetNegativeButtonDown(int i_ActionId)
    {
        return false;
    }

    public bool GetNegativeButtonUp(string i_ActionName)
    {
        return false;
    }

    public bool GetNegativeButtonUp(int i_ActionId)
    {
        return false;
    }

    public bool GetNegativeButtonPrev(string i_ActionName)
    {
        return false;
    }

    public bool GetNegativeButtonPrev(int i_ActionId)
    {
        return false;
    }

    public bool GetNegativeButtonDoublePressDown(string i_ActionName)
    {
        return false;
    }

    public bool GetNegativeButtonDoublePressDown(int i_ActionId)
    {
        return false;
    }

    public bool GetNegativeButtonDoublePressHold(string i_ActionName)
    {
        return false;
    }

    public bool GetNegativeButtonDoublePressHold(int i_ActionId)
    {
        return false;
    }

    public float GetNegativeButtonTimePressed(string i_ActionName)
    {
        return 0f;
    }

    public float GetNegativeButtonTimePressed(int i_ActionId)
    {
        return 0f;
    }

    public float GetNegativeButtonTimeUnpressed(string i_ActionName)
    {
        return 0f;
    }

    public float GetNegativeButtonTimeUnpressed(int i_ActionId)
    {
        return 0f;
    }

    #endregion // Negative Button

    #region Rumble

    public void SetVibration(float i_Left, float i_Right)
    {
    }

    public void StopVibration()
    {
    }

    #endregion

    #region Maps

    public void EnableAllMaps()
    {
    }

    public void EnableAllMapsFor(InputSourceType i_Type)
    {
    }

    public void EnableMaps(int i_Category)
    {
    }

    public void EnableMaps(string i_Category)
    {
    }

    public void EnableMaps(string i_Category, string i_Layout)
    {
    }

    public void EnableMaps(InputSourceType i_Type, int i_Category)
    {
    }

    public void EnableMaps(InputSourceType i_Type, string i_Category)
    {
    }

    public void EnableMaps(InputSourceType i_Type, string i_Category, string i_Layout)
    {
    }

    public void DisableAllMaps()
    {
    }

    public void DisableAllMapsFor(InputSourceType i_Type)
    {
    }

    public void DisableMaps(int i_Category)
    {
    }

    public void DisableMaps(string i_Category)
    {
    }

    public void DisableMaps(string i_Category, string i_Layout)
    {
    }

    public void DisableMaps(InputSourceType i_Type, int i_Category)
    {
    }

    public void DisableMaps(InputSourceType i_Type, string i_Category)
    {
    }

    public void DisableMaps(InputSourceType i_Type, string i_Category, string i_Layout)
    {
    }

    #endregion // Maps

    #region Constructor

    public UPlayerInputImpl(string i_Name)
    {
        m_Name = i_Name;
    }

    #endregion // Constructor
}