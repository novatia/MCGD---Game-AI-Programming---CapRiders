using UnityEngine;
using System.Collections;

public class OnClickEvent : MonoBehaviour 
{
    public string eventName;

    void OnClick()
    {
        if (eventName != "")
        {
            Messenger.Broadcast(eventName);
        }
	}
}
