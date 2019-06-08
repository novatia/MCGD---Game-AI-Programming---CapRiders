using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Enable/disable XInput.")]
    public class SetUseXInput : FsmStateAction
    {
        [RequiredField]
        public FsmBool useXInput;

        public override void Reset()
        {
            useXInput = true;
        }

        public override void OnEnter()
        {
            InputSystem.useXInputMain = useXInput.Value;
            Finish();
        }
    }
}