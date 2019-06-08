using System.Collections.Generic;

public static class ListExtensions
{
    public static bool IsValidIndex<T>(this List<T> list, int i_Index)
    {
        if (list == null)
        {
            return false;
        }

        return (i_Index >= 0 && i_Index < list.Count);
    }

    public static bool IsEmpty<T>(this List<T> list)
    {
        return (list.Count == 0);
    }

    public static bool TryGetFirst<T>(this List<T> list, out T o_Element)
    {
        o_Element = default(T);

        if (list.Count > 0)
        {
            o_Element = list[0];
            return true;
        }

        return false;
    }

    public static bool TryGetLast<T>(this List<T> list, out T o_Element)
    {
        o_Element = default(T);

        if (list.Count > 0)
        {
            o_Element = list[list.Count - 1];
            return true;
        }

        return false;
    }

    public static T GetFirst<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            return list[0];
        }

        return default(T);
    }

    public static T GetLast<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            return list[list.Count - 1];
        }

        return default(T);
    }

    public static T Pop<T>(this List<T> list)
    {
        if (list.Count > 0)
        {
            T lastElement = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return lastElement;
        }

        return default(T);
    }

    public static bool TryGetAt<T>(this List<T> list, int i_Index, out T o_Value)
    {
        o_Value = default(T);

        if (list == null)
        {
            return false;
        }

        if (i_Index < 0 || i_Index >= list.Count)
        {
            return false;
        }

        o_Value = list[i_Index];
        return true;
    }

    public static T GetAt<T>(this List<T> list, int i_Index)
    {
        if (list == null)
        {
            return default(T);
        }

        T result;
        list.TryGetAt<T>(i_Index, out result);
        return result;
    }
}
