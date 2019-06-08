using System.Collections.Generic;

public class tnProceedRequestHandler
{
    // Fields

    private Dictionary<int, List<int>> m_Players = null;

    private List<int> m_ReceivedProcessRequest = null;

    // ACCESSORS

    public int playerCount
    {
        get { return m_Players.Count; }
    }

    // LOGIC

    public void AddPlayer(int i_PlayerId)
    {
        if (HasPlayer(i_PlayerId))
            return;

        List<int> list = new List<int>();
        m_Players.Add(i_PlayerId, list);
    }

    public bool RemovePlayer(int i_PlayerId)
    {
        return m_Players.Remove(i_PlayerId);
    }

    public bool HasPlayer(int i_PlayerId)
    {
        return m_Players.ContainsKey(i_PlayerId);
    }

    public void SetPlayerReady(int i_PlayerId, int i_ProceedRequestId)
    {
        if (!HasPlayer(i_PlayerId))
            return;

        List<int> list = m_Players[i_PlayerId];
        list.Add(i_ProceedRequestId);
    }

    public bool GetPlayerReady(int i_PlayerId, int i_ProceedRequestId)
    {
        if (!HasPlayer(i_PlayerId))
            return false;

        List<int> list = m_Players[i_PlayerId];
        return list.Contains(i_ProceedRequestId);
    }

    public void ClearPlayer(int i_PlayerId)
    {
        if (!HasPlayer(i_PlayerId))
            return;

        List<int> list = m_Players[i_PlayerId];
        list.Clear();
    }

    public void ClearAllPlayers()
    {
        foreach (int key in m_Players.Keys)
        {
            List<int> list = m_Players[key];
            list.Clear();
        }
    }

    public void ClearAll()
    {
        m_Players.Clear();
    }

    public bool IsReceivedProceedRequest(int i_ProceedRequestId)
    {
        return m_ReceivedProcessRequest.Contains(i_ProceedRequestId);
    }

    public void AddReceivedProceedRequest(int i_ProceedRequestId)
    {
        m_ReceivedProcessRequest.Add(i_ProceedRequestId);
    }

    public void RemoveReceivedProceedRequest(int i_ProceedRequestId)
    {
        m_ReceivedProcessRequest.Remove(i_ProceedRequestId);
    }

    // CTOR

    public tnProceedRequestHandler()
    {
        m_Players = new Dictionary<int, List<int>>();
        m_ReceivedProcessRequest = new List<int>();
    }
}