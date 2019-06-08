using UnityEngine;
using System;

[Serializable]
public class ResourcePath
{
    [SerializeField]
    private string m_Path = "";

    public static implicit operator string (ResourcePath i_ResourcePath)
    {
        return i_ResourcePath.m_Path;
    }
}
