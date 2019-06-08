using UnityEngine;
using System.Collections;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _Instance = null;

    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<T>();
                DontDestroyOnLoad(_Instance.gameObject);
            }

            return _Instance;
        }
    }
     
    void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _Instance)
            {
                Destroy(this.gameObject);
            }
        }

        OnAwake();
    }

    public virtual void OnAwake() { }

}
