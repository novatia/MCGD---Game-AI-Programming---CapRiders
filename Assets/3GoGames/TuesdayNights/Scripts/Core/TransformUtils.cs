using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtensions
{
    public static Transform SearchByName(this Transform target, string name)
    {
        if (target.name == name)
        {
            return target;
        }

        for (int i = 0; i < target.childCount; ++i)
        {
            Transform result = SearchByName(target.GetChild(i), name);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public static T SearchByType<T>(this Transform target) where T : MonoBehaviour
    {
        T component = target.GetComponent<T>();
       
        if (component != null)
        {
            return component;
        }

        for (int i = 0; i < target.childCount; ++i)
        {
            T result = SearchByType<T>(target.GetChild(i));

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public static T SearchByTypeInParents<T>(this Transform target) where T : MonoBehaviour
    {
        T component = target.GetComponent<T>();

        if (component != null)
        {
            return component;
        }

        Transform parent = target.parent;

        if (parent != null)
        {
            T result = SearchByTypeInParents<T>(parent);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public static T[] SearchObjectsByType<T>(this Transform target) where T : MonoBehaviour
    {
        List<T> results = new List<T>();
        results.AddRange(target.GetComponents<T>());

        for (int i = 0; i < target.childCount; ++i)
        {
            results.AddRange(SearchObjectsByType<T>(target.GetChild(i)));
        }

        return results.ToArray();
    }

    public static Transform SearchByTag(this Transform target, string tag)
    {
        if (target.tag == tag)
        {
            return target;
        }

        for (int i = 0; i < target.childCount; ++i)
        {
            Transform result = SearchByTag(target.GetChild(i), tag);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    public static Transform[] SearchObjectsByTag(this Transform go, string tag)
    {
        List<Transform> list = new List<Transform>();

        if (go.tag.Equals(tag))
        {
            list.Add(go);
        }

        for (int i = 0; i < go.childCount; ++i)
        {
            list.AddRange(go.GetChild(i).SearchObjectsByTag(tag));
        }

        return list.ToArray();
    }

    public static bool HasChild(this Transform target, GameObject go)
    {
        if (target == go.transform)
        {
            return true;
        }

        for (int i = 0; i < target.childCount; ++i)
        {
            if (HasChild(target.GetChild(i), go))
            {
                return true;
            }
        }

        return false;
    }

    public static void AttachTo(this Transform target, Transform parent)
    {
        Vector3 localPosition = target.localPosition;
        Quaternion localRotation = target.localRotation;
        target.parent = parent;
        target.localPosition = localPosition;
        target.localRotation = localRotation;
    }

    public static bool HasUniformScale(this Transform target)
    {
        return (Mathf.Abs(target.localScale.x) == Mathf.Abs(target.localScale.y)) && (Mathf.Abs(target.localScale.y) == Mathf.Abs(target.localScale.z));
    }

    public static Vector2 GetPositionXY(this Transform target)
    {
        return new Vector2(target.position.x, target.position.y);
    }

    public static Vector2 GetPositionXZ(this Transform target)
    {
        return new Vector2(target.position.x, target.position.z);
    }

    public static Vector2 GetPositionYZ(this Transform target)
    {
        return new Vector2(target.position.y, target.position.z);
    }
}