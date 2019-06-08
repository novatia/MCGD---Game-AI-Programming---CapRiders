using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Is a given Player active?.")]
    public class GetPlayerActive : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Player index.")]
        public FsmInt playerIndex;

        [Tooltip("Event to send if the GameObject is visible.")]
        public FsmEvent trueEvent;

        [Tooltip("Event to send if the GameObject is NOT visible.")]
        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a bool variable.")]
        public FsmBool storeResult;

        public bool everyFrame;

        public override void Reset()
        {
            playerIndex = 0;
            trueEvent = null;
            falseEvent = null;
            storeResult = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoIsActive();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoIsActive();
        }

        void DoIsActive()
        {
            PlayerInput playerInput = InputSystem.GetPlayerByIndexMain(0);

            if (playerInput != null && playerInput.bIsActive)
            {
                storeResult.Value = true;
                Fsm.Event(trueEvent);
            }
            else
            {
                storeResult.Value = false;
                Fsm.Event(falseEvent);
            }
        }
    }
}