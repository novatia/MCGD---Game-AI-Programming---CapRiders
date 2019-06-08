using UnityEngine;
using System.Collections;

public sealed class DisableOnEvent : MonoBehaviour 
{
	public GameObject target = null;

    //[EventName]
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
			target.SetActive(false);
		}
	}
}
