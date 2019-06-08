using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the Parent of a Game Object. It uses the Transform.SetParent method")]
	public class SetTransformParent : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The Game Object to parent.")]
		public FsmOwnerDefault gameObject;
		
		[Tooltip("The new parent for the Game Object.")]
		public FsmGameObject parent;
		
		[Tooltip("If true, the parent-relative position, scale and rotation is modified such that the object keeps the same world space position, rotation and scale as before.")]
		public FsmBool worldPositionStays;
		
		public override void Reset()
		{
			gameObject = null;
			parent = null;
			worldPositionStays = true;
		}

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);

            GameObject parentGo = parent.Value;
            Transform parentTransform = null;

            if (parentGo != null)
            {
                parentTransform = parentGo.transform;
            }

            if (go != null)
            {
                go.transform.SetParent(parentTransform, worldPositionStays.Value);
            }

            Finish();
        }
	}
}