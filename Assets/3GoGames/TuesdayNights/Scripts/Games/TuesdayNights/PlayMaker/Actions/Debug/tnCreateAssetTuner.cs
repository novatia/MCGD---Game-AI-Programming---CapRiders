using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    public class tnCreateAssetTuner : FsmStateAction
    {
        public override void OnEnter()
        {
            base.OnEnter();

#if !(BUILD_RELEASE || BUILD_TEST) 
            tnAssetTuner assetTunerPrefab = Resources.Load<tnAssetTuner>("Debug/p_AssetTuner");
            if (assetTunerPrefab != null)
            {
                GameObject.Instantiate<tnAssetTuner>(assetTunerPrefab);
            }
#endif

            Finish(); 
        }
    }
}