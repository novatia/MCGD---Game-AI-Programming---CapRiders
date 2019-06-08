using UnityEngine;
using UnityEditor;

using System.Collections;

public class ShowHiddenObjects : MonoBehaviour
{
    [MenuItem("GameObject/ShowAll", false, 10000)]
    static void ShowAll()
    {
        Transform[] list = GameObject.FindObjectsOfType<Transform>();
        foreach (Transform t in list)
        {
            if ((t.gameObject.hideFlags & HideFlags.HideInHierarchy) != 0)
            {
                t.gameObject.hideFlags &= ~HideFlags.HideInHierarchy;

                Debug.Log("Removed HideInHierarchy flag from " + t.gameObject.name);
            }
        }
    }
}
