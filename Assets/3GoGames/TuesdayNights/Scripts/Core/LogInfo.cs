using UnityEngine;
using System.Collections;

public class LogInfo : MonoBehaviour 
{
	void Update () 
    {
        Debug.Log(gameObject.name + " - " + "[" + transform.position + "]");
	}
}
