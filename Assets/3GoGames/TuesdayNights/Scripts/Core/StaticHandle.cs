using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class StaticHandle<V, T> : Singleton<V>  where T : MonoBehaviour 
                                                where V : StaticHandle<V, T>
{
    private List<T> m_Handles = new List<T>();

    public T handle
    {
        get { return GetHandle(0); }
    }

    // MonoBehaviour's INTERFACE

    void Awake()
    {
        GameObject root = GameObject.Find("Handles");
        if (root == null)
        {
            root = new GameObject("Handles");
            DontDestroyOnLoad(root);
        }

        gameObject.SetParent(root);
    }

    // BUSINESS LOGIC

    public void Initialize()
    {

    }

    public void RegisterHandle(T i_Handle)
    {
        if (Contains(i_Handle))
            return;

        m_Handles.Add(i_Handle);
    }

    public void UnregisterHandle(T i_Handle)
    {
        m_Handles.Remove(i_Handle);
    }

    public T GetHandle(int i_Index)
    {
        if (i_Index < 0 || i_Index >= m_Handles.Count)
        {
            return null;
        }

        return m_Handles[i_Index];
    }

    public bool Contains(T i_Handle)
    {
        return m_Handles.Contains(i_Handle);
    }

    // STATIC METHODS

    public static T handleMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.handle;
            }

            return null;
        }
    }

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    public static void RegisterHandleMain(T i_Handle)
    {
        if (Instance != null)
        {
            Instance.RegisterHandle(i_Handle);
        }
    }

    public static void UnregisterHandleMain(T i_Handle)
    {
        if (Instance != null)
        {
            Instance.UnregisterHandle(i_Handle);
        }
    }

    public static T GetHandleMain(int i_Index)
    {
        if (Instance != null)
        {
            Instance.GetHandle(i_Index);
        }

        return null;
    }

    public static bool ContainsMain(T i_Handle)
    {
        if (Instance != null)
        {
            Instance.Contains(i_Handle);
        }

        return false;
    }
}
