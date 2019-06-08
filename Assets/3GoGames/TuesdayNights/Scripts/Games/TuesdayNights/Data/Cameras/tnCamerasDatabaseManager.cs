using UnityEngine;
using System.Collections.Generic;

public class tnCamerasSet
{
    [SerializeField]
    private Dictionary<int, GameObject> m_Cameras = null;

    public int count
    {
        get { return m_Cameras.Count; }
    }

    public Dictionary<int, GameObject>.KeyCollection keys
    {
        get { return m_Cameras.Keys; }
    }

    public GameObject GetCamera(string i_CameraId)
    {
        int hash = StringUtils.GetHashCode(i_CameraId);
        return GetCamera(hash);
    }

    public GameObject GetCamera(int i_CameraId)
    {
        GameObject camera = null;
        m_Cameras.TryGetValue(i_CameraId, out camera);
        return camera;
    }

    public tnCamerasSet(tnCameraSetDescriptor i_Descriptor)
    {
        m_Cameras = new Dictionary<int, GameObject>();

        if (i_Descriptor != null)
        {
            foreach (string id in i_Descriptor.keys)
            {
                GameObject camera = i_Descriptor.GetCamera(id);

                if (camera == null)
                    continue;

                int hash = StringUtils.GetHashCode(id);
                m_Cameras.Add(hash, camera);
            }
        }
    }
}

public class tnCameraDatabaseManager
{
    private Dictionary<int, tnCamerasSet> m_Data = null;

    public int dataCount
    {
        get { return m_Data.Count; }
    }

    // LOGIC

    public void Initialize(string i_DatabasePath)
    {
        tnCamerasDatabase database = Resources.Load<tnCamerasDatabase>(i_DatabasePath);
        if (database != null)
        {
            foreach (string id in database.keys)
            {
                tnCameraSetDescriptor cameraSetDescriptor = database.GetCameraSet(id);
                if (cameraSetDescriptor != null)
                {
                    int hash = StringUtils.GetHashCode(id);

                    tnCamerasSet cameraSet = new tnCamerasSet(cameraSetDescriptor);
                    m_Data.Add(hash, cameraSet);
                }
            }
        }
        else
        {
            LogManager.LogWarning(this, "Database not loaded.");
        }
    }

    public tnCamerasSet GetData(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetData(hash);
    }

    public tnCamerasSet GetData(int i_Id)
    {
        tnCamerasSet data = null;
        m_Data.TryGetValue(i_Id, out data);
        return data;
    }

    // CTOR

    public tnCameraDatabaseManager()
    {
        m_Data = new Dictionary<int, tnCamerasSet>();
    }
}
