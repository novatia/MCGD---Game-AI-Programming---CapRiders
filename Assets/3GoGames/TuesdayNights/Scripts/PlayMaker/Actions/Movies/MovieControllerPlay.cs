using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Movies")]
    [Tooltip("Play target MovieTexture.")]
    public class MovieControllerPlay : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(MovieController))]
        [UIHint(UIHint.Variable)]
        [Tooltip("Movie Controller.")]
        public FsmOwnerDefault gameObject;

        public override void Reset()
        {
            gameObject = null;
        }

        public override void OnEnter()
        {
            GameObject go = gameObject.OwnerOption == OwnerDefaultOption.UseOwner ? Owner : gameObject.GameObject.Value;
            if (go != null)
            {
                MovieController movieController = go.GetComponent<MovieController>();
                movieController.Play();
            }

            Finish();
        }
    }
}