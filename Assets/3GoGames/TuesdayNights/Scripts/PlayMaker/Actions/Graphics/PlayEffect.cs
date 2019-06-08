using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - Graphics")]
    [Tooltip("Play Effect.")]
    public class PlayEffect : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof(Effect))]
        public FsmGameObject effect;

        [UIHint(UIHint.Variable)]
        public FsmGameObject parent;

        public FsmVector3 position;
        public FsmQuaternion rotation;

        public override void Reset()
        {
            effect = null;

            parent = null;

            position = null;
            rotation = null;
        }

        public override void OnEnter()
        {
            if (!effect.IsNone && effect.Value != null)
            {
                Effect effectPrefab = effect.Value.GetComponent<Effect>();
                EffectUtils.PlayEffect(effectPrefab, position.Value, rotation.Value, parent.Value != null ? parent.Value.transform : null);
            }

            Finish();
        }
    }
}