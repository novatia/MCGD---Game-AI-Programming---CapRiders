using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Clear MusicPlayer playlist.")]
    public class ClearPlaylist : FsmStateAction
    {
        public override void OnEnter()
        {
            MusicPlayer.SetPlaylistMain(null);
            Finish();
        }
    }
}