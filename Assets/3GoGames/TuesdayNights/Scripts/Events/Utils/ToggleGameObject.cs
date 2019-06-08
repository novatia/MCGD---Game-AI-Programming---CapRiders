using UnityEngine;
using System.Collections;

public sealed class ToggleGameObject : MonoBehaviour 
{
    public GameObject target = null;
    
    public string eventName;

    void OnEnable()
    {
        if (eventName != "")
        {
            Messenger.AddListener(eventName, OnEvent);
        }
    }

    void OnDisable()
    {
        if (eventName != "")
        {
            Messenger.RemoveListener(eventName, OnEvent);
        }
    }

    void OnEvent()
    {
        if (target != null)
        {
            target.SetActive(!target.activeSelf);
        }
    }
}
