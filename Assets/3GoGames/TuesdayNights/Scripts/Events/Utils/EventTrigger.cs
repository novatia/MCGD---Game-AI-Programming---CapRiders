using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class EventTrigger : MonoBehaviour 
{
    public enum TriggerEvents { OnTriggerEnter, /*OnTriggerStay,*/ OnTriggerExit };

    public string eventName;
    
    public TriggerEvents raiseOn = TriggerEvents.OnTriggerEnter;
    public bool destroyAfterEvent = false;

    private Collider2D coll2D = null;

    void Awake()
    {
        coll2D = (Collider2D)GetComponent(typeof(Collider2D));
        coll2D.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (raiseOn == TriggerEvents.OnTriggerEnter)
        {
            if (eventName != "")
            {
                Messenger.Broadcast(eventName);

                if (destroyAfterEvent)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (raiseOn == TriggerEvents.OnTriggerExit)
        {
            if (eventName != "")
            {
                Messenger.Broadcast(eventName);

                if (destroyAfterEvent)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
