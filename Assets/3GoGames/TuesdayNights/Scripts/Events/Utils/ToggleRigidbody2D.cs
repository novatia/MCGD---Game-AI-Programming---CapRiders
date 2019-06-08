using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public sealed class ToggleRigidbody2D : MonoBehaviour 
{
    public string eventName;

    private Rigidbody2D targetRigidbody2D;

    void OnEnable()
    {
        targetRigidbody2D = GetComponent<Rigidbody2D>();

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

        targetRigidbody2D = null;
    }

    void OnEvent()
    {
        if (targetRigidbody2D != null)
        {
            targetRigidbody2D.isKinematic = !targetRigidbody2D.isKinematic;
        }
    }
}
