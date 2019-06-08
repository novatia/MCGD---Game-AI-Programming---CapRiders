using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Get Camera prefab from MatchSettingsModule")]
    public class tnGetSelectedCamera : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeResult;

        public override void Reset()
        {
            storeResult = null;
        }

        public override void OnEnter()
        {
            tnMatchSettingsModule module = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();
            if (module != null)
            {
                int gameModeId = module.gameModeId;
                tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(gameModeId);

                if (gameModeData != null)
                {
                    int camerasSetId = gameModeData.camerasSetId;
                    tnCamerasSet camerasSet = tnGameData.GetCameraSetMain(camerasSetId);

                    if (camerasSet != null)
                    {
                        int stadiumId = module.stadiumId;
                        tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(stadiumId);

                        if (stadiumData != null)
                        {
                            GameObject cameraPrefab = camerasSet.GetCamera(stadiumData.cameraId);

                            if (cameraPrefab != null)
                            {
                                if (storeResult != null && !storeResult.IsNone)
                                {
                                    storeResult.Value = cameraPrefab.gameObject;
                                }
                            }
                        }
                    }
                }
            }

            Finish();
        }
    }
}