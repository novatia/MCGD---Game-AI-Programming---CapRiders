using UnityEngine;
using System.Collections;

using InputUtils;

public interface IPlayerInputImpl
{

	#region Properties

    int             Id                  { get; }
    string          Name                { get; }
    string          DescriptiveName     { get; }
    int             JoystickCount       { get; }

    bool            bIsPlaying          { get; }

    bool            HasMouse            { get; set; }

    #endregion // Properties

    #region Axis

    float GetAxis(string i_ActionName);
	float GetAxis(int i_ActionId);

	float GetAxisPrev(string i_ActionName);
	float GetAxisPrev(int i_ActionId);

	float GetAxisTimeActive(string i_ActionName);
	float GetAxisTimeActive(int i_ActionId);

	float GetAxisTimeInactive(string i_ActionName);
	float GetAxisTimeInactive(int i_ActionId);

    float GetAxisRaw(string i_ActionName);
    float GetAxisRaw(int i_ActionId);

	#endregion // Axis

	#region Positive Button

	bool GetButton(string i_ActionName);
	bool GetButton(int i_ActionId);

	bool GetButtonDown(string i_ActionName);
	bool GetButtonDown(int i_ActionId);

	bool GetButtonUp(string i_ActionName);
	bool GetButtonUp(int i_ActionId);

	bool GetButtonPrev(string i_ActionName);
	bool GetButtonPrev(int i_ActionId);

	bool GetButtonDoublePressDown(string i_ActionName);
	bool GetButtonDoublePressDown(int i_ActionId);

	bool GetButtonDoublePressHold(string i_ActionName);
	bool GetButtonDoublePressHold(int i_ActionId);

	float GetButtonTimePressed(string i_ActionName);
	float GetButtonTimePressed(int i_ActionId);

	float GetButtonTimeUnpressed(string i_ActionName);
	float GetButtonTimeUnpressed(int i_ActionId);

	#endregion // Positive Button

	#region Negative Button

	bool GetNegativeButton(string i_ActionName);
	bool GetNegativeButton(int i_ActionId);

	bool GetNegativeButtonDown(string i_ActionName);
	bool GetNegativeButtonDown(int i_ActionId);

	bool GetNegativeButtonUp(string i_ActionName);
	bool GetNegativeButtonUp(int i_ActionId);

	bool GetNegativeButtonPrev(string i_ActionName);
	bool GetNegativeButtonPrev(int i_ActionId);

	bool GetNegativeButtonDoublePressDown(string i_ActionName);
	bool GetNegativeButtonDoublePressDown(int i_ActionId);

	bool GetNegativeButtonDoublePressHold(string i_ActionName);
	bool GetNegativeButtonDoublePressHold(int i_ActionId);

	float GetNegativeButtonTimePressed(string i_ActionName);
	float GetNegativeButtonTimePressed(int i_ActionId);

	float GetNegativeButtonTimeUnpressed(string i_ActionName);
	float GetNegativeButtonTimeUnpressed(int i_ActionId);

    #endregion // Negative Button

    #region Rumble

    void SetVibration(float i_Left, float i_Right);
    void StopVibration();

    #endregion

    #region Maps

    void EnableAllMaps();
	void EnableAllMapsFor(InputSourceType i_Type);

	void EnableMaps(int i_Category);
	void EnableMaps(string i_Category);

	void EnableMaps(string i_Category, string i_Layout);

	void EnableMaps(InputSourceType i_Type, int i_Category);
	void EnableMaps(InputSourceType i_Type, string i_Category);

	void EnableMaps(InputSourceType i_Type, string i_Category, string i_Layout);

	void DisableAllMaps();
	void DisableAllMapsFor(InputSourceType i_Type);

	void DisableMaps(int i_Category);
	void DisableMaps(string i_Category);

	void DisableMaps(string i_Category, string i_Layout);

	void DisableMaps(InputSourceType i_Type, int i_Category);
	void DisableMaps(InputSourceType i_Type, string i_Category);

	void DisableMaps(InputSourceType i_Type, string i_Category, string i_Layout);

	#endregion // Maps

}