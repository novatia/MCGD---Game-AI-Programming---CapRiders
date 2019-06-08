using UnityEngine;
using System.Collections;

public class RaiseEventOnStart : MonoBehaviour 
{
    public string eventName;

	void Start() 
    {
        if (eventName != "")
        {
            Messenger.Broadcast(eventName);
        }
	}
}
