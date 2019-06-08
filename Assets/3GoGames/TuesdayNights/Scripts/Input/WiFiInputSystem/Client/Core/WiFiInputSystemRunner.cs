using UnityEngine;

namespace WiFiInput.Client
{
    public class WiFiInputSystemRunner : MonoBehaviour
    {
        void Start()
        {
            if (!WiFiInputSystem.isRunningMain)
            {
                WiFiInputSystem.RunMain();
            }
        }
    }
}
