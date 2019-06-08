using UnityEngine;
using System.Collections;

public class PopupTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Popup.ShowMessageMain("THIS IS A TEST POPUP. ENJOY!");
        }
    }
}
