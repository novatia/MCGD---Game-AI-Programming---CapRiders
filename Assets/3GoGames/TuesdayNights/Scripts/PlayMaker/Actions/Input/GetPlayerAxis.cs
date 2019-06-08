using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Gets the value of the specified Input Axis and stores it in a Float Variable. See Unity Input Manager docs.")]
    public class GetPlayerAxis : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The name of the axis. Set in the Unity Input Manager.")]
        public FsmString axisName;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Player index.")]
        public FsmInt playerIndex;

        [Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
        public FsmFloat multiplier;

        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a float variable.")]
        public FsmFloat store;

        [Tooltip("Repeat every frame. Typically this would be set to True.")]
        public bool everyFrame;

        public override void Reset()
        {
            axisName = "";
            playerIndex = 0;
            multiplier = 1.0f;
            store = null;
            everyFrame = true;
        }

        public override void OnEnter()
        {
            DoGetAxis();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoGetAxis();
        }

        void DoGetAxis()
        {
            PlayerInput playerInput = InputSystem.GetPlayerByIndexMain(playerIndex.Value);

            if (playerInput == null)
                return;

            float axisValue = playerInput.GetAxis(axisName.Value);

            Debug.Log(axisValue);

            if (!multiplier.IsNone)
            {
                axisValue *= multiplier.Value;
            }

            store.Value = axisValue;
        }
    }
}