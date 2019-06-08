using UnityEngine;

using System;
using System.Collections;

public class StaticCoroutine : Singleton<StaticCoroutine>
{
    // STATIC

    public static void DoCoroutineMain(IEnumerator i_Coroutine, Action i_OnComplete = null)
    {
        if (Instance != null)
        {
            Instance.DoCoroutine(i_Coroutine, i_OnComplete);
        }
    }

    public static Coroutine RunMain(IEnumerator i_Coroutine)
    {
        if (Instance != null)
        {
            Instance.Run(i_Coroutine);
        }

        return null;
    }

    // LOGIC

    public Coroutine Run(IEnumerator i_Coroutine)
    {
        return StartCoroutine(i_Coroutine);
    }

    public void DoCoroutine(IEnumerator i_Coroutine, Action i_OnComplete = null)
    {
        StartCoroutine(Perform(i_Coroutine, i_OnComplete));
    }

    // INTERNALS

    private IEnumerator Perform(IEnumerator i_Coroutine, Action i_OnComplete = null)
    {
        if (i_Coroutine != null)
        {
            yield return StartCoroutine(i_Coroutine);
        }

        if (i_OnComplete != null)
        {
            i_OnComplete();
        }
    }
}
