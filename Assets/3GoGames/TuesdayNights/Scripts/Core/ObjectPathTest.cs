using UnityEngine;

public class ObjectPathTest : MonoBehaviour
{
    public ResourcePath obj = null;

    void Start()
    {
        if (obj != null)
        {
            Debug.Log("PATH: " + obj);
        }
    }
}
