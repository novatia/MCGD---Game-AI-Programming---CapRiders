using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using FullInspector;

public class tnGameSettingsDatabase : BaseScriptableObject
{
    [SerializeField]
    private Dictionary<string, string> m_Settings = new Dictionary<string, string>();

    public int count
    {
        get { return m_Settings.Count; }
    }

    public Dictionary<string, string>.KeyCollection keys
    {
        get { return m_Settings.Keys; }
    }

    public string GetValue(string i_Key)
    {
        string value;
        m_Settings.TryGetValue(i_Key, out value);
        return value;
    }
}