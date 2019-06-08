using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Sends an Event when the specified Mouse Button is pressed. Optionally store the button state in a bool variable.")]
    public class GetMouseButtonDownMain : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The mouse button to test.")]
        public MouseButton button;

        [Tooltip("Event to send if the mouse button is down.")]
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the button state in a Bool Variable.")]
        public FsmBool storeResult;

        public override void Reset()
        {
            button = MouseButton.Left;
            sendEvent = null;
            storeResult = null;
        }

        public override void OnEnter()
        {
            DoGetMouseButtonDown();
        }

        public override void OnUpdate()
        {
            DoGetMouseButtonDown();
        }

        void DoGetMouseButtonDown()
        {
            bool buttonDown = InputSystem.GetMouseButtonDownMain((int)button);
            if (buttonDown)
            {
                Fsm.Event(sendEvent);
            }

            storeResult.Value = buttonDown;
        }
    }
}