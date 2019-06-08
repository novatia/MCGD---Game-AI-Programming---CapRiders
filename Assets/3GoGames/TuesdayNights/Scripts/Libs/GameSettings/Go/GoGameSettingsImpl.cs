using UnityEngine;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum SettingType
{
    None,
    Bool,
    Int,
    Float,
    String,
}

[Serializable]
public class GoSetting
{
    [SerializeField]
    private int m_Id;
    [SerializeField]
    private SettingType m_Type;
    [SerializeField]
    private bool m_BoolValue;
    [SerializeField]
    private int m_IntValue;
    [SerializeField]
    private float m_FloatValue;
    [SerializeField]
    private string m_StringValue;

    public int id
    {
        get { return m_Id; }
    }

    public SettingType type
    {
        get { return m_Type; }
    }

    public bool boolValue
    {
        get { return m_BoolValue; }
    }

    public int intValue
    {
        get { return m_IntValue; }
    }

    public float floatValue
    {
        get { return m_FloatValue; }
    }

    public string stringValue
    {
        get { return m_StringValue; }
    }

    // CTOR

    public GoSetting()
    {
        m_Id = Hash.s_NULL;
        m_Type = SettingType.None;
        m_BoolValue = false;
        m_IntValue = 0;
        m_FloatValue = 0f;
        m_StringValue = "";
    }

    public GoSetting(int i_Id, int i_Value)
    {
        m_Id = i_Id;
        m_Type = SettingType.Int;
        m_BoolValue = false;
        m_IntValue = i_Value;
        m_FloatValue = 0f;
        m_StringValue = "";
    }

    public GoSetting(int i_Id, bool i_Value)
    {
        m_Id = i_Id;
        m_Type = SettingType.Bool;
        m_BoolValue = i_Value;
        m_IntValue = 0;
        m_FloatValue = 0f;
        m_StringValue = "";
    }

    public GoSetting(int i_Id, float i_Value)
    {
        m_Id = i_Id;
        m_Type = SettingType.Float;
        m_BoolValue = false;
        m_IntValue = 0;
        m_FloatValue = i_Value;
        m_StringValue = "";
    }

    public GoSetting(int i_Id, string i_Value)
    {
        m_Id = i_Id;
        m_Type = SettingType.String;
        m_BoolValue = false;
        m_IntValue = 0;
        m_FloatValue = 0f;
        m_StringValue = i_Value;
    }
}

[Serializable]
public class GoSettingsSave
{
    [SerializeField]
    private List<GoSetting> m_Settings = null;

    public int settingsCount
    {
        get
        {
            return m_Settings.Count;
        }
    }

    // LOGIC

    public void Clear()
    {
        m_Settings.Clear();
    }

    public void AddInt(int i_Id, int i_Value)
    {
        GoSetting setting = new GoSetting(i_Id, i_Value);
        m_Settings.Add(setting);
    }

    public void AddFloat(int i_Id, float i_Value)
    {
        GoSetting setting = new GoSetting(i_Id, i_Value);
        m_Settings.Add(setting);
    }

    public void AddBool(int i_Id, bool i_Value)
    {
        GoSetting setting = new GoSetting(i_Id, i_Value);
        m_Settings.Add(setting);
    }

    public void AddString(int i_Id, string i_Value)
    {
        GoSetting setting = new GoSetting(i_Id, i_Value);
        m_Settings.Add(setting);
    }

    public GoSetting GetSetting(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Settings.Count)
        {
            return null;
        }

        return m_Settings[i_Index];
    }

    // CTOR

    public GoSettingsSave()
    {
        m_Settings = new List<GoSetting>();
    }
}

public class GoGameSettingsImpl : IGameSettingsImpl
{
    // Fields

    private Dictionary<int, bool> m_BoolSettings = null;
    private Dictionary<int, int> m_IntSettings = null;
    private Dictionary<int, float> m_FloatSettings = null;
    private Dictionary<int, string> m_StringSettings = null;

    // IGameSettingsImpls's interface

    public void Initialize()
    {

    }

    public void Save()
    {
        InternalSave();
    }

    public void Load()
    {
        InternalLoad();
    }

    public void Delete(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        Delete(hash);
    }

    public void Delete(int i_Id)
    {
        DeleteInt(i_Id);
        DeleteFloat(i_Id);
        DeleteString(i_Id);
        DeleteBool(i_Id);
    }

    public void DeleteInt(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        DeleteInt(hash);
    }

    public void DeleteInt(int i_Id)
    {
        InternalDeleteIntSetting(i_Id);
    }

    public void DeleteFloat(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        DeleteFloat(hash);
    }

    public void DeleteFloat(int i_Id)
    {
        InternalDeleteFloatSetting(i_Id);
    }

    public void DeleteString(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        DeleteString(hash);
    }

    public void DeleteString(int i_Id)
    {
        InternalDeleteStringSetting(i_Id);
    }

    public void DeleteBool(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        DeleteBool(hash);
    }

    public void DeleteBool(int i_Id)
    {
        InternalDeleteBoolSetting(i_Id);
    }

    public void DeleteAll()
    {
        ClearAll();
    }

    public void SetInt(string i_Id, int i_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetInt(hash, i_Value);
    }

    public void SetInt(int i_Id, int i_Value)
    {
        m_IntSettings[i_Id] = i_Value;
    }

    public void SetFloat(string i_Id, float i_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetFloat(hash, i_Value);
    }

    public void SetFloat(int i_Id, float i_Value)
    {
        m_FloatSettings[i_Id] = i_Value;
    }

    public void SetString(string i_Id, string i_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetString(hash, i_Value);
    }

    public void SetString(int i_Id, string i_Value)
    {
        m_StringSettings[i_Id] = i_Value;
    }

    public void SetBool(string i_Id, bool i_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        SetBool(hash, i_Value);
    }

    public void SetBool(int i_Id, bool i_Value)
    {
        m_BoolSettings[i_Id] = i_Value;
    }

    public int GetInt(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetInt(hash);
    }

    public int GetInt(int i_Id)
    {
        int value;
        if (m_IntSettings.TryGetValue(i_Id, out value))
        {
            return value;
        }

        return 0;
    }

    public float GetFloat(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetFloat(hash);
    }

    public float GetFloat(int i_Id)
    {
        float value;
        if (m_FloatSettings.TryGetValue(i_Id, out value))
        {
            return value;
        }

        return 0f;
    }

    public string GetString(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetString(hash);
    }

    public string GetString(int i_Id)
    {
        string value;
        if (m_StringSettings.TryGetValue(i_Id, out value))
        {
            return value;
        }

        return "";
    }

    public bool GetBool(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetBool(hash);
    }

    public bool GetBool(int i_Id)
    {
        bool value;
        if (m_BoolSettings.TryGetValue(i_Id, out value))
        {
            return value;
        }

        return false;
    }

    public bool TryGetInt(string i_Id, out int o_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return TryGetInt(hash, out o_Value);
    }

    public bool TryGetInt(int i_Id, out int o_Value)
    {
        return m_IntSettings.TryGetValue(i_Id, out o_Value);
    }

    public bool TryGetFloat(string i_Id, out float o_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return TryGetFloat(hash, out o_Value);
    }

    public bool TryGetFloat(int i_Id, out float o_Value)
    {
        return m_FloatSettings.TryGetValue(i_Id, out o_Value);
    }

    public bool TryGetString(string i_Id, out string o_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return TryGetString(hash, out o_Value);
    }

    public bool TryGetString(int i_Id, out string o_Value)
    {
        return m_StringSettings.TryGetValue(i_Id, out o_Value);
    }

    public bool TryGetBool(string i_Id, out bool o_Value)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return TryGetBool(hash, out o_Value);
    }

    public bool TryGetBool(int i_Id, out bool o_Value)
    {
        return m_BoolSettings.TryGetValue(i_Id, out o_Value);
    }

    public bool HasKey(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return HasKey(hash);
    }

    public bool HasKey(int i_Id)
    {
        bool intKey = HasIntKey(i_Id);
        bool floatKey = HasFloatKey(i_Id);
        bool stringKey = HasStringKey(i_Id);
        bool boolKey = HasBoolKey(i_Id);

        return (intKey || floatKey || stringKey || boolKey);
    }

    public bool HasIntKey(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return HasIntKey(hash);
    }

    public bool HasIntKey(int i_Id)
    {
        return m_IntSettings.ContainsKey(i_Id);
    }

    public bool HasFloatKey(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return HasFloatKey(hash);
    }

    public bool HasFloatKey(int i_Id)
    {
        return m_FloatSettings.ContainsKey(i_Id);
    }

    public bool HasStringKey(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return HasStringKey(hash);
    }

    public bool HasStringKey(int i_Id)
    {
        return m_StringSettings.ContainsKey(i_Id);
    }

    public bool HasBoolKey(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return HasBoolKey(hash);
    }

    public bool HasBoolKey(int i_Id)
    {
        return m_BoolSettings.ContainsKey(i_Id);
    }

    // INTERNALS

    private void InternalDeleteIntSetting(int i_Id)
    {
        m_IntSettings.Remove(i_Id);
    }

    private void InternalDeleteFloatSetting(int i_Id)
    {
        m_FloatSettings.Remove(i_Id);
    }

    private void InternalDeleteStringSetting(int i_Id)
    {
        m_StringSettings.Remove(i_Id);
    }

    private void InternalDeleteBoolSetting(int i_Id)
    {
        m_BoolSettings.Remove(i_Id);
    }

    private void ClearAll()
    {
        ClearBoolSettings();
        ClearFloatSettings();
        ClearIntSettings();
        ClearStringSettings();
    }

    private void ClearBoolSettings()
    {
        m_BoolSettings.Clear();
    }

    private void ClearIntSettings()
    {
        m_IntSettings.Clear();
    }

    private void ClearFloatSettings()
    {
        m_FloatSettings.Clear();
    }

    private void ClearStringSettings()
    {
        m_StringSettings.Clear();
    }

    private void InternalSave()
    {
        GoSettingsSave save = CreateSave();

        string filePath = Application.persistentDataPath + "/Settings.sav";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(filePath);

        LogManager.Log(this, LogContexts.Saves, "File created at '" + filePath + "'. The saved will be writed.");

        bf.Serialize(file, save);

        file.Close();
    }

    private void InternalLoad()
    {
        GoSettingsSave save = null;

        string filePath = Application.persistentDataPath +  "/Settings.sav";

        if (File.Exists(Application.persistentDataPath + "/Settings.sav"))
        {
            LogManager.Log(this, LogContexts.Saves, "Found file at '" + filePath + "'. The file will be loaded.");

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filePath, FileMode.Open);
            save = (GoSettingsSave)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            LogManager.Log(this, LogContexts.Saves, "Cannot load from '" + filePath + "'. File not found.");
        }

        ApplyState(save);
    }

    private void ApplyState(GoSettingsSave i_Save)
    {
        LogManager.Log(this, LogContexts.Saves, "Start load save.");

        ClearAll();

        if (i_Save == null)
            return;

        for (int index = 0; index < i_Save.settingsCount; ++index)
        {
            GoSetting setting = i_Save.GetSetting(index);

            if (setting == null)
                continue;

            int id = setting.id;

            switch (setting.type)
            {
                case SettingType.Bool:
                    SetBool(id, setting.boolValue);
                    LogManager.Log(this, LogContexts.Saves, "Loaded: (bool)" + id + " --> " + setting.boolValue);
                    break;

                case SettingType.Float:
                    SetFloat(id, setting.floatValue);
                    LogManager.Log(this, LogContexts.Saves, "Loaded: (float)" + id + " --> " + setting.floatValue);
                    break;

                case SettingType.Int:
                    SetInt(id, setting.intValue);
                    LogManager.Log(this, LogContexts.Saves, "Loaded: (int)" + id + " --> " + setting.intValue);
                    break;

                case SettingType.String:
                    SetString(id, setting.stringValue);
                    LogManager.Log(this, LogContexts.Saves, "Loaded: (string)" + id + " --> " + setting.stringValue);
                    break;
            }
        }

        LogManager.Log(this, LogContexts.Saves, "Save loaded.");
    }

    private GoSettingsSave CreateSave()
    {
        GoSettingsSave save = new GoSettingsSave();

        LogManager.Log(this, LogContexts.Saves, "Start create save.");

        // Int

        {
            foreach (int id in m_IntSettings.Keys)
            {
                int value = m_IntSettings[id];
                save.AddInt(id, value);

                LogManager.Log(this, LogContexts.Saves, "(int) " + id + " --> " + value);
            }
        }

        // Float

        {
            foreach (int id in m_FloatSettings.Keys)
            {
                float value = m_FloatSettings[id];
                save.AddFloat(id, value);

                LogManager.Log(this, LogContexts.Saves, "(float) " + id + " --> " + value);
            }
        }

        // Bool

        {
            foreach (int id in m_BoolSettings.Keys)
            {
                bool value = m_BoolSettings[id];
                save.AddBool(id, value);

                LogManager.Log(this, LogContexts.Saves, "(bool) " + id + " --> " + value);
            }
        }

        // String

        {
            foreach (int id in m_StringSettings.Keys)
            {
                string value = m_StringSettings[id];
                save.AddString(id, value);

                LogManager.Log(this, LogContexts.Saves, "(string) " + id + " --> " + value);
            }
        }

        LogManager.Log(this, LogContexts.Saves, "Save created.");

        return save;
    }

    // CTOR

    public GoGameSettingsImpl()
    {
        m_BoolSettings = new Dictionary<int, bool>();
        m_IntSettings = new Dictionary<int, int>();
        m_FloatSettings = new Dictionary<int, float>();
        m_StringSettings = new Dictionary<int, string>();
    }
}
