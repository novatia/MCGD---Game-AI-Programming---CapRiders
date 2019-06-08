using UnityEngine;

using System;

[Serializable]
public class RemoteStatInfo
{
    [SerializeField]
    private string m_SteamId = "";

    public string steamId
    {
        get { return m_SteamId; }
    }
}
