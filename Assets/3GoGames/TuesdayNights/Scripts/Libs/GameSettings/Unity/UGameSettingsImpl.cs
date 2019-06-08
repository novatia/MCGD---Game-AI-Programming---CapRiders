using UnityEngine;

public class UGameSettingsImpl : IGameSettingsImpl
{
    public void Initialize()
    {

    }

    public void Save()
    {
        PlayerPrefs.Save();
    }

    public void Load()
    {

    }

    public void Delete(string i_Id)
    {
        PlayerPrefs.DeleteKey(i_Id);
    }

    public void Delete(int i_Id)
    {
        
    }

    public void DeleteInt(string i_Id)
    {

    }

    public void DeleteInt(int i_Id)
    {

    }

    public void DeleteFloat(string i_Id)
    {

    }

    public void DeleteFloat(int i_Id)
    {

    }

    public void DeleteString(string i_Id)
    {

    }

    public void DeleteString(int i_Id)
    {

    }

    public void DeleteBool(string i_Id)
    {

    }

    public void DeleteBool(int i_Id)
    {

    }

    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void SetInt(string i_Id, int i_Value)
    {
        PlayerPrefs.SetInt(i_Id, i_Value);
    }

    public void SetInt(int i_Id, int i_Value)
    {

    }

    public void SetFloat(string i_Id, float i_Value)
    {
        PlayerPrefs.SetFloat(i_Id, i_Value);
    }

    public void SetFloat(int i_Id, float i_Value)
    {

    }

    public void SetString(string i_Id, string i_Value)
    {
        PlayerPrefs.SetString(i_Id, i_Value);
    }

    public void SetString(int i_Id, string i_Value)
    {

    }

    public void SetBool(string i_Id, bool i_Value)
    {
        string value;
        ConvertBoolToString(i_Value, out value);
        PlayerPrefs.SetString(i_Id, value);
    }

    public void SetBool(int i_Id, bool i_Value)
    {

    }

    public int GetInt(string i_Id)
    {
        return PlayerPrefs.GetInt(i_Id);
    }

    public int GetInt(int i_Id)
    {
        return 0;
    }

    public float GetFloat(string i_Id)
    {
        return PlayerPrefs.GetFloat(i_Id);
    }

    public float GetFloat(int i_Id)
    {
        return 0f;
    }

    public string GetString(string i_Id)
    {
        return PlayerPrefs.GetString(i_Id);
    }

    public string GetString(int i_Id)
    {
        return "";
    }

    public bool GetBool(string i_Id)
    {
        string stringValue = GetString(i_Id);

        bool boolValue;
        ConvertStringToBool(stringValue, out boolValue);

        return boolValue;
    }

    public bool GetBool(int i_Id)
    {
        return false;
    }

    public bool TryGetInt(string i_Id, out int o_Value)
    {
        o_Value = 0;

        if (HasKey(i_Id))
        {
            o_Value = GetInt(i_Id);
            return true;
        }

        return false;
    }

    public bool TryGetInt(int i_Id, out int o_Value)
    {
        o_Value = 0;
        return false;
    }

    public bool TryGetFloat(string i_Id, out float o_Value)
    {
        o_Value = 0f;

        if (HasKey(i_Id))
        {
            o_Value = GetFloat(i_Id);
            return true;
        }

        return false;
    }

    public bool TryGetFloat(int i_Id, out float o_Value)
    {
        o_Value = 0f;
        return false;
    }

    public bool TryGetString(string i_Id, out string o_Value)
    {
        o_Value = "";

        if (HasKey(i_Id))
        {
            o_Value = GetString(i_Id);
            return true;
        }

        return false;
    }

    public bool TryGetString(int i_Id, out string o_Value)
    {
        o_Value = "";
        return false;
    }

    public bool TryGetBool(string i_Id, out bool o_Value)
    {
        o_Value = false;

        string stringValue;
        if (TryGetString(i_Id, out stringValue))
        {
            bool boolValue;
            if (ConvertStringToBool(stringValue, out boolValue))
            {
                o_Value = boolValue;
                return true;
            }
        }

        return false;
    }

    public bool TryGetBool(int i_Id, out bool o_Value)
    {
        o_Value = false;
        return false;
    }

    public bool HasKey(string i_Id)
    {
        return PlayerPrefs.HasKey(i_Id);

    }

    public bool HasKey(int i_Id)
    {
        return false;
    }

    public bool HasIntKey(string i_Id)
    {
        int v;
        return TryGetInt(i_Id, out v);
    }

    public bool HasIntKey(int i_Id)
    {
        return false;
    }

    public bool HasFloatKey(string i_Id)
    {
        float v;
        return TryGetFloat(i_Id, out v);
    }

    public bool HasFloatKey(int i_Id)
    {
        return false;
    }

    public bool HasStringKey(string i_Id)
    {
        string v;
        return TryGetString(i_Id, out v);
    }

    public bool HasStringKey(int i_Id)
    {
        return false;
    }

    public bool HasBoolKey(string i_Id)
    {
        bool v;
        return TryGetBool(i_Id, out v);
    }

    public bool HasBoolKey(int i_Id)
    {
        return false;
    }

    // INTERNALS

    private bool ConvertBoolToString(bool i_Value, out string o_Value)
    {
        string value = (i_Value) ? "ON" : "OFF";
        o_Value = value;
        return true;
    }

    private bool ConvertStringToBool(string i_Value, out bool o_Value)
    {
        bool value = false;
        if (i_Value == "ON")
        {
            value = true;
            o_Value = value;
            return true;
        }
        else
        {
            if (i_Value == "OFF")
            {
                value = false;
                o_Value = value;
                return true;
            }
        }

        o_Value = value;
        return false;
    }
}
