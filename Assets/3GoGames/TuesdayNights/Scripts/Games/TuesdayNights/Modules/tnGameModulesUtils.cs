using UnityEngine;
using System.Collections;

using TuesdayNights;

public static class tnGameModulesUtils
{
    public static bool IsLocalPlayer(int i_OnlinePlayerIndex)
    {
        if (i_OnlinePlayerIndex < 0)
        {
            return false;
        }

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();

        if (localPartyModule == null)
        {
            return false;
        }

        for (int index = 0; index < localPartyModule.playersCount; ++index)
        {
            int onlinePlayerIndex = localPartyModule.GetOnlinePlayerIndexByIndex(index);
            if (onlinePlayerIndex == i_OnlinePlayerIndex)
            {
                return true;
            }
        }

        return false;
    }

    public static bool LocalToOnlinePlayerIndex(int i_LocalPlayerIndex, out int o_OnlinePlayerIndex)
    {
        o_OnlinePlayerIndex = -1;

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();

        if (localPartyModule == null)
        {
            return false;
        }

        if (i_LocalPlayerIndex < 0 || i_LocalPlayerIndex > localPartyModule.playersCount)
        {
            return false;
        }

        int onlinePlayerIndex = localPartyModule.GetOnlinePlayerIndexByIndex(i_LocalPlayerIndex);
        o_OnlinePlayerIndex = onlinePlayerIndex;

        return true;
    }

    public static bool OnlineToLocalPlayerIndex(int i_OnlinePlayerIndex, out int o_LocalPlayerIndex)
    {
        o_LocalPlayerIndex = -1;

        if (i_OnlinePlayerIndex < 0)
        {
            return false;
        }

        tnLocalPartyModule localPartyModule = GameModulesManager.GetModuleMain<tnLocalPartyModule>();

        if (localPartyModule == null)
        {
            return false;
        }

        for (int index = 0; index < localPartyModule.playersCount; ++index)
        {
            int currentOnlinePlayerIndex = localPartyModule.GetOnlinePlayerIndexByIndex(index);
            if (currentOnlinePlayerIndex == i_OnlinePlayerIndex)
            {
                o_LocalPlayerIndex = index;
                return true;
            }
        }

        return false;
    }

    public static bool GetPhotonPlayerOwnerId(int i_OnlinePlayerIndex, out int o_Id)
    {
        o_Id = -1;

        tnMultiplayerIndexTable indexTable = null;
        if (PhotonUtils.TryGetRoomCustomProperty<tnMultiplayerIndexTable>(PhotonPropertyKey.s_RoomCustomPropertyKey_AssignedIndices, out indexTable))
        {
            if (indexTable != null)
            {
                int id = indexTable.GetIndexOwnerId(i_OnlinePlayerIndex);
                if (id != -1)
                {
                    o_Id = id;
                    return true;
                }
            }
        }

        return false;
    }
}
