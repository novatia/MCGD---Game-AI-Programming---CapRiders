using UnityEngine;
using System;
using System.Collections;

using InputUtils;

public class PlayerInput
{
    #region Private Field

    private IPlayerInputImpl m_Impl = null;

    private bool _bIsActive = false;

    #endregion

    #region Getters and Setters

    public int Id
    {
        get
        {
            return m_Impl.Id;
        }
    }

    public bool bIsActive
    {
        get
        {
            return _bIsActive;
        }
    }

    public bool bIsPlaying
    {
        get
        {
            return m_Impl.bIsPlaying;
        }
    }

    public string Name
    {
        get
        {
            return m_Impl.Name;
        }
    }

    public string DescriptiveName
    {
        get
        {
            return m_Impl.DescriptiveName;
        }
    }

    public int JoystickCount
    {
        get
        {
            return m_Impl.JoystickCount;
        }
    }

    public bool HasMouse
    {
        get
        {
            return m_Impl.HasMouse;
        }

        set
        {
            m_Impl.HasMouse = value;
        }
    }

    public void SetActive(bool i_bActive)
    {
        _bIsActive = i_bActive;
    }

    #endregion

    #region Axis

    #region GetAxis

    public float GetAxis(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxis(i_ActionName);
        }

        return 0f;
    }

    public float GetAxis(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxis(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #region GetAxisPrev

    public float GetAxisPrev(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxisPrev(i_ActionName);
        }

        return 0f;
    }

    public float GetAxisPrev(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxisPrev(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #region GetAxisTimeActive

    public float GetAxisTimeActive(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxisTimeActive(i_ActionName);
        }

        return 0f;
    }

    public float GetAxisTimeActive(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxisTimeActive(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #region GetAxisTimeInactive

    public float GetAxisTimeInactive(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxisTimeInactive(i_ActionName);
        }

        return 0f;
    }

    public float GetAxisTimeInactive(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxisTimeInactive(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #region GetAxisRaw

    public float GetAxisRaw(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxisRaw(i_ActionName);
        }

        return 0f;
    }

    public float GetAxisRaw(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetAxisRaw(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #endregion

    #region Positive Button

    #region GetButton

    public bool GetButton(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButton(i_ActionName);
        }

        return false;
    }

    public bool GetButton(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButton(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetButtonDown

    public bool GetButtonDown(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonDown(i_ActionName);
        }

        return false;
    }

    public bool GetButtonDown(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonDown(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetButtonUp

    public bool GetButtonUp(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonUp(i_ActionName);
        }

        return false;
    }

    public bool GetButtonUp(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonUp(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetButtonPrev

    public bool GetButtonPrev(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonPrev(i_ActionName);
        }

        return false;
    }

    public bool GetButtonPrev(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonPrev(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetButtonDoublePressDown

    public bool GetButtonDoublePressDown(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonDoublePressDown(i_ActionName);
        }

        return false;
    }

    public bool GetButtonDoublePressDown(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonDoublePressDown(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetButtonDoublePressHold

    public bool GetButtonDoublePressHold(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonDoublePressHold(i_ActionName);
        }

        return false;
    }

    public bool GetButtonDoublePressHold(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonDoublePressHold(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetButtonTimePressed

    public float GetButtonTimePressed(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonTimePressed(i_ActionName);
        }

        return 0f;
    }

    public float GetButtonTimePressed(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonTimePressed(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #region GetButtonTimeUnpressed

    public float GetButtonTimeUnpressed(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonTimeUnpressed(i_ActionName);
        }

        return 0f;
    }

    public float GetButtonTimeUnpressed(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetButtonTimeUnpressed(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #endregion

    #region Negative Button

    #region GetNegativeButton

    public bool GetNegativeButton(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButton(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButton(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButton(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetNegativeButtonDown

    public bool GetNegativeButtonDown(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonDown(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonDown(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonDown(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetNegativeButtonUp

    public bool GetNegativeButtonUp(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonUp(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonUp(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonUp(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetNegativeButtonPrev

    public bool GetNegativeButtonPrev(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonPrev(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonPrev(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonPrev(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetNegativeButtonDoublePressDown

    public bool GetNegativeButtonDoublePressDown(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonDoublePressDown(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonDoublePressDown(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonDoublePressDown(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetNegativeButtonDoublePressHold

    public bool GetNegativeButtonDoublePressHold(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonDoublePressHold(i_ActionName);
        }

        return false;
    }

    public bool GetNegativeButtonDoublePressHold(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonDoublePressHold(i_ActionId);
        }

        return false;
    }

    #endregion

    #region GetNegativeButtonTimePressed

    public float GetNegativeButtonTimePressed(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonTimePressed(i_ActionName);
        }

        return 0f;
    }

    public float GetNegativeButtonTimePressed(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonTimePressed(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #region GetNegativeButtonTimeUnpressed

    public float GetNegativeButtonTimeUnpressed(string i_ActionName)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonTimeUnpressed(i_ActionName);
        }

        return 0f;
    }

    public float GetNegativeButtonTimeUnpressed(int i_ActionId)
    {
        if (_bIsActive)
        {
            return m_Impl.GetNegativeButtonTimeUnpressed(i_ActionId);
        }

        return 0f;
    }

    #endregion

    #endregion

    #region Rumble

    public void SetVibration(float i_Left, float i_Right)
    {
        m_Impl.SetVibration(i_Left, i_Right);
    }

    public void StopVibration()
    {
        m_Impl.StopVibration();
    }

    #endregion

    #region Maps Handling

    public void EnableAllMaps()
    {
        m_Impl.EnableAllMaps();
    }

    public void EnableAllMapsFor(InputSourceType i_Type)
    {
        m_Impl.EnableAllMapsFor(i_Type);
    }

    public void EnableMaps(int i_Category)
    {
        m_Impl.EnableMaps(i_Category);
    }

    public void EnableMaps(string i_Category)
    {
        m_Impl.EnableMaps(i_Category);
    }

    public void EnableMaps(string i_Category, string i_Layout)
    {
        m_Impl.EnableMaps(i_Category, i_Layout);
    }

    public void EnableMaps(InputSourceType i_Type, int i_Category)
    {
        m_Impl.EnableMaps(i_Type, i_Category);
    }

    public void EnableMaps(InputSourceType i_Type, string i_Category)
    {
        m_Impl.EnableMaps(i_Type, i_Category);
    }

    public void EnableMaps(InputSourceType i_Type, string i_Category, string i_Layout)
    {
        m_Impl.EnableMaps(i_Type, i_Category, i_Layout);
    }

    public void DisableAllMaps()
    {
        m_Impl.DisableAllMaps();
    }

    public void DisableAllMapsFor(InputSourceType i_Type)
    {
        m_Impl.DisableAllMapsFor(i_Type);
    }

    public void DisableMaps(int i_Category)
    {
        m_Impl.DisableMaps(i_Category);
    }

    public void DisableMaps(string i_Category)
    {
        m_Impl.DisableMaps(i_Category);
    }

    public void DisableMaps(string i_Category, string i_Layout)
    {
        m_Impl.DisableMaps(i_Category, i_Layout);
    }

    public void DisableMaps(InputSourceType i_Type, int i_Category)
    {
        m_Impl.DisableMaps(i_Type, i_Category);
    }

    public void DisableMaps(InputSourceType i_Type, string i_Category)
    {
        m_Impl.DisableMaps(i_Type, i_Category);
    }

    public void DisableMaps(InputSourceType i_Type, string i_Category, string i_Layout)
    {
        m_Impl.DisableMaps(i_Type, i_Category, i_Layout);
    }

    #endregion

    #region Constructor

    public PlayerInput(string i_Name)
    {
#if INPUT_STANDARD
        m_Impl = new UPlayerInputImpl(i_Name);
#elif INPUT_REWIRED
        m_Impl = new RewiredPlayerInputImpl(i_Name);
#else
        m_Impl = new NullPlayerInputImpl(i_Name);
#endif
    }

    #endregion
}
