using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Sends an Event when a Button is pressed.")]
    public class GetPlayerButtonDown : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The name of the button. Set in the Unity Input Manager.")]
        public FsmString buttonName;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Player index.")]
        public FsmInt playerIndex;

        [Tooltip("Event to send if the button is pressed.")]
        public FsmEvent sendEvent;

        [Tooltip("Set to True if the button is pressed.")]
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public override void Reset()
        {
            buttonName = "Fire1";
            playerIndex = 0;
            sendEvent = null;
            storeResult = null;
        }

        public override void OnUpdate()
        {
            PlayerInput playerInput = InputSystem.GetPlayerByIndexMain(playerIndex.Value);

            if (playerInput == null)
                return;

            bool buttonDown = playerInput.GetButtonDown(buttonName.Value);

            if (buttonDown)
            {
                Fsm.Event(sendEvent);
            }

            storeResult.Value = buttonDown;
        }
    }
}