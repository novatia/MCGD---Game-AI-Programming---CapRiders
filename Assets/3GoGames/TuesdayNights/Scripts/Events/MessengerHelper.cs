using UnityEngine;

public sealed class MessengerHelper : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // OnLevelWasLoaded is deprecated and will be removed in a later versione of Unity. If you want, you can use SceneManager.sceneLoaded callback.

    //// Clean up eventTable every time a new level loads.
    //void OnLevelWasLoaded(int unused)
    //{
    //    // Messenger.Cleanup();
    //}
}
