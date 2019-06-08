using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class TriggerAnimator : MonoBehaviour 
{
    public string eventName;
    public string trigger = "";

    private Animator m_Animator = null;

	void Start () 
    {
        m_Animator = (Animator)GetComponent(typeof(Animator));
	}

    void OnEnable()
    {
        Messenger.AddListener(eventName, OnEvent);
    }

    void OnDisable()
    {
        Messenger.RemoveListener(eventName, OnEvent);
    }

    private void OnEvent()
    {
        m_Animator.SetTrigger(trigger);
    }
}
