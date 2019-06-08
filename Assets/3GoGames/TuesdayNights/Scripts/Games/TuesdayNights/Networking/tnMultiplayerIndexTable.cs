using UnityEngine;

using System.Collections.Generic;

#if PHOTON
using ExitGames.Client.Photon;
#endif // PHOTON

public class tnMultiplayerIndexTable
{
    private static int s_MaxPlayers_Limit = 22;

    private Dictionary<int, List<int>> m_Table = null;
    private int m_MaxPlayers = 0;

    // ACCESSORS

    public int maxPlayers
    {
        get
        {
            return m_MaxPlayers;
        }
    }

    public int keyCount
    {
        get
        {
            return m_Table.Count;
        }
    }

    public Dictionary<int, List<int>>.KeyCollection keys
    {
        get
        {
            return m_Table.Keys;
        }
    }

    // LOGIC

    public List<int> GetAssignedIndicesFor(int i_Id)
    {
        List<int> indices = new List<int>();

        List<int> list;
        if (m_Table.TryGetValue(i_Id, out list))
        {
            if (list != null)
            {
                for (int index = 0; index < list.Count; ++index)
                {
                    int i = list[index];
                    indices.Add(i);
                }
            }
        }

        return indices;
    }

    public List<int> GetAvailableIndices()
    {
        List<int> indices = new List<int>();

        for (int index = 0; index < m_MaxPlayers; ++index)
        {
            indices.Add(index);
        }

        foreach (int id in m_Table.Keys)
        {
            List<int> list = m_Table[id];

            if (list == null)
                continue;

            indices.RemoveAll(list.Contains);
        }

        return indices;
    }

    public void AssignIndicesTo(int i_PlayerId, List<int> i_Indices)
    {
        if (i_Indices == null)
            return;

        m_Table[i_PlayerId] = i_Indices;
    }

    public void ClearIndicesFor(int i_Id)
    {
        m_Table.Remove(i_Id);
    }

    public int GetIndexOwnerId(int i_PlayerIndex)
    {
        foreach (int playerId in m_Table.Keys)
        {
            List<int> indices = m_Table[playerId];
            if (indices != null)
            {
                if (indices.Contains(i_PlayerIndex))
                {
                    return playerId;
                }
            }
        }

        return -1;
    }

    public int GetGuestIndex(int i_PlayerIndex)
    {
        foreach (int playerId in m_Table.Keys)
        {
            List<int> indices = m_Table[playerId];
            if (indices != null)
            {
                for (int index = 0; index < indices.Count; ++index)
                {
                    int current = indices[index];

                    if (current == i_PlayerIndex)
                    {
                        return index;
                    }
                }
            }
        }

        return -1;
    }

    public bool IsIndexAssigned(int i_PlayerIndex, out int o_PlayerId)
    {
        o_PlayerId = -1;

        if (i_PlayerIndex < 0)
        {
            return false;
        }

        foreach (int playerId in keys)
        {
            List<int> list = m_Table[playerId];
            if (list != null)
            {
                if (list.Contains(i_PlayerIndex))
                {
                    o_PlayerId = playerId;
                    return true;
                }
            }
        }

        return false;
    }

#if PHOTON

    // PHOTON Serialization

    public static readonly byte[] memTnMultiplayerIndexTable = new byte[4 + (22 * 4) + (22 * 4)];

    public static short PhotonSerialize(StreamBuffer i_OutStream, object i_Customobject)
    {
        tnMultiplayerIndexTable table = (tnMultiplayerIndexTable)i_Customobject;

        int index = 0;
        lock (memTnMultiplayerIndexTable)
        {
            byte[] bytes = memTnMultiplayerIndexTable;
            Protocol.Serialize(table.maxPlayers, bytes, ref index);
            Protocol.Serialize(table.keyCount, bytes, ref index);
            foreach (int key in table.keys)
            {
                List<int> assignedIndices = table.GetAssignedIndicesFor(key);
                Protocol.Serialize(key, bytes, ref index);
                Protocol.Serialize(assignedIndices.Count, bytes, ref index);
                for (int i = 0; i < assignedIndices.Count; ++i)
                {
                    int assignedIndex = assignedIndices[i];
                    Protocol.Serialize(assignedIndex, bytes, ref index);
                }
            }

            i_OutStream.Write(bytes, 0, (4 + (22 * 4) + (22 * 4)));
        }

        return (4 + (22 * 4) + (22 * 4));
    }

    public static object PhotonDeserialize(StreamBuffer i_InStream, short i_Length)
    {
        tnMultiplayerIndexTable table = null;

        int index = 0;
        lock (memTnMultiplayerIndexTable)
        {
            i_InStream.Read(memTnMultiplayerIndexTable, 0, (4 + (22 * 4) + (22 * 4)));

            int maxPlayers;
            Protocol.Deserialize(out maxPlayers, memTnMultiplayerIndexTable, ref index);

            table = new tnMultiplayerIndexTable(maxPlayers);

            int keyCount;
            Protocol.Deserialize(out keyCount, memTnMultiplayerIndexTable, ref index);

            for (int i = 0; i < keyCount; ++i)
            {
                int key;
                Protocol.Deserialize(out key, memTnMultiplayerIndexTable, ref index);

                int count;
                Protocol.Deserialize(out count, memTnMultiplayerIndexTable, ref index);

                List<int> list = new List<int>();
                for (int j = 0; j < count; ++j)
                {
                    int assignedIndex;
                    Protocol.Deserialize(out assignedIndex, memTnMultiplayerIndexTable, ref index);

                    list.Add(assignedIndex);
                }

                table.AssignIndicesTo(key, list);
            }
        }

        return table;
    }

#endif // PHOTON

    // CTOR

    public tnMultiplayerIndexTable(int i_MaxPlayers)
    {
        m_Table = new Dictionary<int, List<int>>();
        m_MaxPlayers = Mathf.Clamp(i_MaxPlayers, 0, s_MaxPlayers_Limit);
    }
}
