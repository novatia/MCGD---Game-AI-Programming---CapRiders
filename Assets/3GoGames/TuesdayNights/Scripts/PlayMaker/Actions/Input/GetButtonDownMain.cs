using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Sends an Event when a Button is pressed.")]
    public class GetButtonDownMain : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The name of the button. Set in the Unity Input Manager.")]
        public FsmString buttonName;

        [Tooltip("Event to send if the button is pressed.")]
        public FsmEvent sendEvent;

        [Tooltip("Set to True if the button is pressed.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public override void Reset()
        {
            buttonName = "Fire1";
            sendEvent = null;
            storeResult = null;
        }

        public override void OnUpdate()
        {
            var buttonDown = InputSystem.GetButtonDownMain(buttonName.Value);

            if (buttonDown)
            {
                Fsm.Event(sendEvent);
            }

            storeResult.Value = buttonDown;
        }
    }
}