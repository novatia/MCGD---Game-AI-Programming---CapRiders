using UnityEngine;
using System.Collections;

public class BringTransformToFront : MonoBehaviour 
{
    void OnEnable()
    {
        transform.SetAsLastSibling();
    }
}
