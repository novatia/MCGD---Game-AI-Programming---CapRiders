using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Sends an Event when a Key is released.")]
    public class GetKeyUpMain : FsmStateAction
    {
        [RequiredField]
        public KeyCode key;
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public override void Reset()
        {
            sendEvent = null;
            key = KeyCode.None;
            storeResult = null;
        }

        public override void OnUpdate()
        {
            bool keyUp = InputSystem.GetKeyUpMain(key);

            if (keyUp)
            {
                Fsm.Event(sendEvent);
            }

            storeResult.Value = keyUp;
        }
    }
}