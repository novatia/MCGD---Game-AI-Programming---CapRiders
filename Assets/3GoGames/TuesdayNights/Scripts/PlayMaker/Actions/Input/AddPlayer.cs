using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Input")]
    [Tooltip("Add player to InputSystem.")]
    public class AddPlayer : FsmStateAction
    {
        [RequiredField]
        [Tooltip("The name of the player.")]
        public FsmString playerName;

        public override void Reset()
        {
            playerName = "";
        }

        public override void OnEnter()
        {
            InputSystem.AddPlayerMain(playerName.Value);

            InputSystem.RefreshMapsMain();

            Finish();
        }
    }
}