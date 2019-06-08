using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Core")]
    [Tooltip("Setup Application properties.")]
    public class SetupApplication : FsmStateAction
    {
        public enum VSyncType
        {
            Disabled,
            HalfRate,
            FullRate,
        }

        public VSyncType vsyncType = VSyncType.FullRate;
        public int targetFrameRate = 60;

        public override void OnEnter()
        {
            if (vsyncType == VSyncType.Disabled)
            {
                QualitySettings.vSyncCount = 0;
            }
            else
            {
                if (vsyncType == VSyncType.HalfRate)
                {
                    QualitySettings.vSyncCount = 2;
                }
                else
                {
                    QualitySettings.vSyncCount = 1;
                }
            }

            Application.targetFrameRate = targetFrameRate; // Setup framerate target.

            Finish();
        }
    }
}