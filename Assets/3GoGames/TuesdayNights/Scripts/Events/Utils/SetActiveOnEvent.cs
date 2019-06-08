using UnityEngine;
using System.Collections;

public class SetActiveOnEvent : MonoBehaviour 
{
    public GameObject target = null;
    public string eventName = "";
    public bool active = true;

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
            target.SetActive(active);
        }
    }
}
