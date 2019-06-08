using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using System;
using System.Collections;
using System.Reflection;

public sealed class EventHandler : MonoBehaviour, IEventSystemHandler
{
    public string eventName = "";

    [SerializeField]
    private InternalCallback onEvent = new InternalCallback();
 
    public void OnEnable()
    {
        if (eventName != "")
        {
            Messenger.AddListener(eventName, OnEvent);
        }
    }

    public void OnDisable()
    {
        if (eventName != "")
        {
            Messenger.RemoveListener(eventName, OnEvent);
        }
    }
 
    private void OnEvent()
    {
        onEvent.Invoke();
    }
 
    [Serializable]
    public class InternalCallback : UnityEvent
    {
    }
}