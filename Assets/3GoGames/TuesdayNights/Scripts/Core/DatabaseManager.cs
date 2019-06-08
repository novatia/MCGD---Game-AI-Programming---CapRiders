using UnityEngine;

public class DatabaseManager<T> : Singleton<T> where T : MonoBehaviour
{
    void Awake()
    {
        GameObject root = GameObject.Find("Database");
        if (root == null)
        {
            root = new GameObject("Database");
            DontDestroyOnLoad(root);
        }

        gameObject.SetParent(root);

        tag = "Database";

        OnAwake();
    }

    protected virtual void OnAwake() { }
}
