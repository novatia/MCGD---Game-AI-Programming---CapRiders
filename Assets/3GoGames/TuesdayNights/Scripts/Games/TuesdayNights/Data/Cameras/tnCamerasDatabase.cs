using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

[Serializable]
public class tnCameraSetDescriptor
{
    [SerializeField]
    private Dictionary<string, GameObject> m_Cameras = new Dictionary<string, GameObject>();

    public int count
    {
        get { return m_Cameras.Count; }
    }

    public Dictionary<string, GameObject>.KeyCollection keys
    {
        get { return m_Cameras.Keys; }
    }

    public GameObject GetCamera(string i_CameraId)
    {
        GameObject camera = null;
        m_Cameras.TryGetValue(i_CameraId, out camera);
        return camera;
    }
}

public class tnCamerasDatabase : BaseScriptableObject
{
    [SerializeField]
    private Dictionary<string, tnCameraSetDescriptor> m_CamerasSets = new Dictionary<string, tnCameraSetDescriptor>();

    public int count
    {
        get { return m_CamerasSets.Count; }
    }

    public Dictionary<string, tnCameraSetDescriptor>.KeyCollection keys
    {
        get { return m_CamerasSets.Keys; }
    }

    public tnCameraSetDescriptor GetCameraSet(string i_CameraSetId)
    {
        tnCameraSetDescriptor camerSet = null;
        m_CamerasSets.TryGetValue(i_CameraSetId, out camerSet);
        return camerSet;
    }
}
