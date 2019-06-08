using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Resources")]
	[Tooltip("UnLoads unused Resources. Be careful to use it only once at a time as it crates hickups especially on mobile.")]
	public class ResourcesUnLoadUnusedAssets : FsmStateAction
	{
		public FsmEvent UnloadDoneEvent;
		
		AsyncOperation asyncOperation;
		
		public override void Reset()
		{
			UnloadDoneEvent = null;
		}
		
		public override void OnEnter()
		{
            asyncOperation = Resources.UnloadUnusedAssets();
		}
		
		public override void OnUpdate()
		{
		    if (asyncOperation != null)
            {
			    if (asyncOperation.isDone)
				{
					Fsm.Event(UnloadDoneEvent);
					Finish();
				}
			}
		}
	}
}

