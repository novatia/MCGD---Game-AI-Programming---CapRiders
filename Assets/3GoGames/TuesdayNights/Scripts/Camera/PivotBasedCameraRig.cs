using System;
using UnityEngine;


namespace UnityStandardAssets.Cameras
{
    public abstract class PivotBasedCameraRig : AbstractTargetFollower
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 game objects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        protected Transform m_Cam = null;       // Transform of the camera
        protected Transform m_Pivot = null;     // Point at which the camera pivots around
        protected Vector3 m_LastTargetPosition = Vector3.zero;

        protected virtual void Awake()
        {
            Camera cam = GetComponentInChildren<Camera>();

            if (cam != null)
            {
                m_Cam = cam.transform; // Find the camera in the object hierarchy
                m_Pivot = m_Cam.parent;
            }
        }
    }
}
