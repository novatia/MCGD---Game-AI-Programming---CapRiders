using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("MusicPlayer - Play.")]
    public class MusicPlay : FsmStateAction
    {
        public override void OnEnter()
        {
            MusicPlayer.PlayMain();
            Finish();
        }
    }
}