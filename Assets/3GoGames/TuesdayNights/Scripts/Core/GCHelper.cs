using UnityEngine;

using System;

public class GCHelper : MonoBehaviour
{
    public int frameInterval = 30;

	void Update ()
    {
#if !UNITY_EDITOR

        int frameCount = Time.frameCount;
	    if (frameCount % frameInterval == 0)
        {
            GC.Collect();
        }
#endif
	}
}
