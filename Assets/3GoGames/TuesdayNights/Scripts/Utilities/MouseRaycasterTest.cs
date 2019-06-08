using UnityEngine;
using System.Collections;

public class MouseRaycasterTest : MonoBehaviour
{
    void OnEnable()
    {
        MouseRaycaster.AddListenerMain(LayerMask.NameToLayer("Items 2"), OnEvent);
    }

    void OnDisable()
    {
        MouseRaycaster.RemoveListenerMain(LayerMask.NameToLayer("Items 2"), OnEvent);
    }

    void OnEvent(MouseRaycasterEventParams i_Params)
    {
        if (i_Params.eventType == MouseRaycasterEventType.OnEnterEvent)
        {
            Debug.Log("ON ENTER : " + i_Params.position + " " + i_Params.gameObject.name);
        }
        else // OnExit
        {
            Debug.Log("ON EXIT : " + i_Params.position + " " + i_Params.gameObject.name);
        }
    }
}
