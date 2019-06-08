using UnityEngine;

public static class tnGameModeFactory
{
    public static tnMatchController CreateMatchController(int i_Id)
    {
        tnMatchController controllerInstance = null;

        tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(i_Id);
        if (gameModeData != null)
        {
            tnMatchController controllerPrefab = gameModeData.matchController;
            if (controllerPrefab != null)
            {
                controllerInstance = GameObject.Instantiate<tnMatchController>(controllerPrefab);
            }
        }

        return controllerInstance;
    }
}
