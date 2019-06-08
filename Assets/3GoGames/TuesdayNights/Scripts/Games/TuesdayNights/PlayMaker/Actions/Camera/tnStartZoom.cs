using UnityEngine;

using System;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights - Camera")]
    [Tooltip("Zoom on target character.")]
    public class tnStartZoom : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [ObjectType(typeof(tnCameraZoom))]
        public FsmObject controller;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmInt targetIndex;

        public override void Reset()
        {
            controller = null;
        }

        public override void OnEnter()
        {
            if (controller != null && controller.Value != null)
            {
                tnCameraZoom cameraZoom = (tnCameraZoom)controller.Value;
                cameraZoom.SetHighlighted(targetIndex.Value, OnAnimationCompleted);
            }
            else
            {
                OnAnimationCompleted();
            }
        }

        // INTERNALS

        private void OnAnimationCompleted()
        {
            Finish();
        }
    }
}