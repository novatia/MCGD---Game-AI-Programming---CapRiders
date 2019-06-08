using UnityEngine;

using System;
using System.Collections.Generic;

public static class CSharpUtils
{
    public static void Swap<T>(ref T i_A, ref T i_B) where T : class
    {
        T temp = i_A;
        i_A = i_B;
        i_B = temp;
    }

    public static T Cast<T>(object i_Object)
    {
        T castedObj;
        TryCast<T>(i_Object, out castedObj);
        return castedObj;
    }

    public static bool TryCast<T>(object i_Object, out T o_CastedObj)
    {
        o_CastedObj = default(T);

        if (i_Object == null)
        {
            return false;
        }

        bool canCast = i_Object is T;

        if (!canCast)
        {
            return false;
        }

        T castedObj = (T) i_Object;
        o_CastedObj = castedObj;

        return true;
    }
}

public static class CollectionsUtils
{
	public static void InsertionSort<T>(IList<T> i_List, Comparison<T> i_Comparison)
	{
		if (i_List == null || i_Comparison == null)
			return;

		int count = i_List.Count;
		for (int j = 1; j < count; j++) {
			T key = i_List [j];

			int i = j - 1;
			for (; i >= 0 && i_Comparison (i_List [i], key) > 0; i--) {
				i_List [i + 1] = i_List [i];
			}

			i_List [i + 1] = key;
		}
	}
}