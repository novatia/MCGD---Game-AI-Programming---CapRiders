#if PHOTON

using UnityEngine;

public class PunPrefabPool : MonoBehaviour, IPunPrefabPool
{
    public void Destroy(GameObject gameObject)
    {
        gameObject.Recycle();
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        GameObject go = (GameObject) Resources.Load(prefabId);

        if (go != null)
        {
            GameObject instance = go.Spawn(position, rotation);
            return instance;
        }

        return null;      
    }
}

#endif