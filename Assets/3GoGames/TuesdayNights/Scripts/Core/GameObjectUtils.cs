using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameObjectUtils 
{
    // Extensions

    public static T GetComponentOnlyInChildren<T>(this GameObject go) where T : MonoBehaviour
    {
        if (go == null)
        {
            return null;
        }

        T[] childrenComponents = go.GetComponentsInChildren<T>();

        for (int i = 0; i < childrenComponents.Length; ++i)
        {
            T component = childrenComponents[i];

            if (component == null)
                continue;

            if (component.gameObject != go)
            {
                return component;
            }
        }

        return null;
    }

    public static GameObject FindChild(this GameObject go, string name)
    {
        Transform child = go.transform.Find(name);

        if (child == null)
        {
            return null;
        }

        return child.gameObject;
    }

    public static GameObject FindChildWithTag(this GameObject go, string tag)
    {
        if (go.CompareTag(tag))
        {
            return go;
        }
        else
        {
            for (int childIndex = 0; childIndex < go.GetChildCount(); ++childIndex)
            {
                GameObject child = go.GetChild(childIndex);

                GameObject result = child.FindChildWithTag(tag);
                if (result != null)
                {
                    return result;
                }
            }
        }

        return null;
    }

    public static int GetChildCount(this GameObject go)
    {
        return go.transform.childCount;
    }

    public static GameObject Find(this GameObject go, string name)
    {
        Transform child = go.transform.Find(name);

        if (child == null)
        {
            return null;
        }

        return child.gameObject;
    }

    public static GameObject GetChild(this GameObject go, int index)
    {
        Transform child = go.transform.GetChild(index);

        if (child == null)
        {
            return null;
        }

        return child.gameObject;
    }

    public static GameObject GetParent(this GameObject go)
    {
        if (go.transform.parent != null)
        {
            return go.transform.parent.gameObject;
        }

        return null;
    }

    public static void SetParent(this GameObject go, GameObject parent, bool worldPositionStays = true)
    {
        if (parent == null)
        {
            go.transform.SetParent(null, worldPositionStays);
        }
        else
        {
            go.transform.SetParent(parent.transform, worldPositionStays);
        }
    }

    public static bool CheckLayerMask(this GameObject go, int layerMask)
    {
        int goLayer = go.layer;
        int goLayerMask = (1 << goLayer);

        int maskComparison = goLayerMask & layerMask;

        return (maskComparison > 0);
    }

    // STATIC

    public static T FindObjectWithTag<T>(string i_Tag) where T : Component
    {
        T result = null;

        GameObject go = GameObject.FindGameObjectWithTag(i_Tag);
        if (go != null)
        {
            T component = go.GetComponent<T>();
            result = component;
        }

        return result;
    }

}
