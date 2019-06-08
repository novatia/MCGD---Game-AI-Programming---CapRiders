using WiFiInput.Server;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Initialize WiFi InputSystem.")]
    public class InitWiFiInputSystem : FsmStateAction
    {
        public override void OnEnter()
        {
            WiFiInputSystem.InitializeMain();
            Finish();
        }
    }
}