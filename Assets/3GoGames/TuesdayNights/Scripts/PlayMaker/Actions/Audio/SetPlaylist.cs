using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Audio")]
    [Tooltip("Set target playlist.")]
    public class SetPlaylist : FsmStateAction
    {
        [RequiredField]
        [ObjectType(typeof(MusicPlaylist))]
        [Tooltip("Music Bg.")]
        public FsmObject playlist;

        public override void Reset()
        {
            playlist = null;
        }

        public override void OnEnter()
        {
            if (!playlist.IsNone && playlist.Value != null)
            {
                MusicPlayer.SetPlaylistMain((MusicPlaylist)playlist.Value);
            }

            Finish();
        }
    }
}