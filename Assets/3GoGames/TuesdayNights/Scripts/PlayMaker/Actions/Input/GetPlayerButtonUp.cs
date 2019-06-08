using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Sends an Event when a Button is released.")]
    public class GetPlayerButtonUpMain : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The name of the button. Set in the Unity Input Manager.")]
        public FsmString buttonName;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Player index.")]
        public FsmInt playerIndex;

        [Tooltip("Event to send if the button is released.")]
        public FsmEvent sendEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Set to True if the button is released.")]
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

            bool buttonUp = playerInput.GetButtonUp(buttonName.Value);

            if (buttonUp)
            {
                Fsm.Event(sendEvent);
            }

            storeResult.Value = buttonUp;
        }
    }
}