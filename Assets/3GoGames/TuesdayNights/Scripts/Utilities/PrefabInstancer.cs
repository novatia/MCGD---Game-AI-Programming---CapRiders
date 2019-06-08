using UnityEngine;
using System.Collections;

public class PrefabInstancer : MonoBehaviour 
{
    public GameObject prefab = null;

    void Awake()
    {
        if (prefab != null)
        {
            GameObject instance = (GameObject)Instantiate(prefab, transform.position, transform.rotation);
            instance.transform.SetParent(this.transform, true);
        }
    }
}
