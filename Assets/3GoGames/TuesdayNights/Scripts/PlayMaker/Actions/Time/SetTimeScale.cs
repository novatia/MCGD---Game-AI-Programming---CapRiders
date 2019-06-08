using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Time")]
    [Tooltip("Set time scale.")]
    public class SetTimeScale : FsmStateAction
    {
        [RequiredField]
        public FsmFloat timeScale;

        public bool restoreOnExit = false;

        private float m_Cache = 1f;

        public override void Reset()
        {
            timeScale = 1f;
            restoreOnExit = false;
        }

        public override void OnEnter()
        {
            m_Cache = Time.timeScale;

            Time.timeScale = timeScale.Value;

            if (!restoreOnExit)
            {
                Finish();
            }
        }

        public override void OnExit()
        {
            if (restoreOnExit)
            {
                Time.timeScale = m_Cache;
            }
        }
    }
}