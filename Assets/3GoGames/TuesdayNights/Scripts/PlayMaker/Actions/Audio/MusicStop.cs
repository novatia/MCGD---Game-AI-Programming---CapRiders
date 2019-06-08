using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("MusicPlayer - Stop.")]
    public class MusicStop : FsmStateAction
    {
        public override void OnEnter()
        {
            MusicPlayer.StopMain();
            Finish();
        }
    }
}