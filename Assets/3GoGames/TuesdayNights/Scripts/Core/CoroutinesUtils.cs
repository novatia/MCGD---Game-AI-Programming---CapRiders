using UnityEngine;

using System;
using System.Collections;

public static class CoroutinesUtils
{
    public static IEnumerator DoCoroutine(MonoBehaviour i_Executor, IEnumerator i_Enumerator, Action i_Callback)
    {
        if (i_Executor != null)
        {
            yield return i_Executor.StartCoroutine(i_Enumerator);
        }

        if (i_Callback != null)
        {
            i_Callback();
        }
    }
}
