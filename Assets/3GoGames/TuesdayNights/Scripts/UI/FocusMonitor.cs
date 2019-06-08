using UnityEngine;

using GoUI;

public class FocusMonitor : MonoBehaviour
{
	void Awake()
    {
        DontDestroyOnLoad(gameObject);
	}
	
	void Update()
    {
        if (UIEventSystem.focusMain != null)
        {
            Debug.Log(UIEventSystem.focusMain.name);
        }
    }
}
