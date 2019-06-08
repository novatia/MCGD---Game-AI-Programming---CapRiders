using UnityEngine;
using System.Collections;

public class ToggleComponent<T> : MonoBehaviour where T : Behaviour 
{
    public string eventName;

    private T component;

    void OnEnable()
    {
        component = GetComponent<T>();

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

        component = null;
    }

    void OnEvent()
    {
        if (component != null)
        {
            component.enabled = !component.enabled;
        }
    }
}
