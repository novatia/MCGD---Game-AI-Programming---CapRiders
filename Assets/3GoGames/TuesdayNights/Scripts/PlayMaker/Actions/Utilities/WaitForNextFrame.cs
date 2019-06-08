using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Utilities")]
    [Tooltip("Sends an Event in the next frame. Useful if you want to loop states every frame.")]
    public class WaitForNextFrame : FsmStateAction
    {
        [RequiredField]
        public FsmEvent sendEvent;

        private int m_EnterFrame = 0;

        public override void Reset()
        {
            sendEvent = null;
        }

        public override void OnEnter()
        {
            m_EnterFrame = Time.frameCount;
        }

        public override void OnUpdate()
        {
            int currentFrame = Time.frameCount;

            if (currentFrame > m_EnterFrame)
            {
                Finish();
                Fsm.Event(sendEvent);
            }
        }
    }
}