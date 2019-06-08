using System;

using System.Collections.Generic;

public static class CollectionsUtilities
{
    private static Random s_DefaultRandom = new Random();

    // LOGIC

    public static List<T> CreateList<T>(params T[] i_Objects)
    {
        List<T> list = new List<T>();

        if (i_Objects != null)
        {
            for (int index = 0; index < i_Objects.Length; ++index)
            {
                T item = i_Objects[index];
                list.Add(item);
            }
        }

        return list;
    }

    public static T[] CreateArray<T>(params T[] i_Objects)
    {
        List<T> list = CreateList<T>(i_Objects);
        return list.ToArray();
    }

    public static bool Contains<T>(this IList<T> list, Func<T, bool> i_Predicate)
    {
        for (int index = 0; index < list.Count; ++index)
        {
            T value = list[index];
            bool predicateEvaluated = i_Predicate(value);

            if (predicateEvaluated)
            {
                return true;
            }
        }

        return false;
    }

    public static void Shuffle<T>(this IList<T> list, int i_Seed)
    {
        Random r = new Random(i_Seed);
        InternalShuffle<T>(list, r);
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        InternalShuffle<T>(list, s_DefaultRandom);
    }

    // INTERNALS

    private static void InternalShuffle<T>(IList<T> list, Random i_Random)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = i_Random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
