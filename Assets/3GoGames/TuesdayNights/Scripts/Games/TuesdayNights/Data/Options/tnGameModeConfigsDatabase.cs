using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class tnGameModeConfigsDatabase : BaseScriptableObject
{
    [SerializeField]
    private Dictionary<string, tnGameModeConfigDescriptor> m_Configs = new Dictionary<string, tnGameModeConfigDescriptor>();

    public int count
    {
        get { return m_Configs.Count; }
    }

    public Dictionary<string, tnGameModeConfigDescriptor>.KeyCollection keys
    {
        get { return m_Configs.Keys; }
    }

    public tnGameModeConfigDescriptor GetConfig(string i_Key)
    {
        tnGameModeConfigDescriptor config = null;
        m_Configs.TryGetValue(i_Key, out config);
        return config;
    }
}
