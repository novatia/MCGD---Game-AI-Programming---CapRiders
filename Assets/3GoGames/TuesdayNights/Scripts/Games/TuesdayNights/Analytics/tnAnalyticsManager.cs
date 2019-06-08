using UnityEngine;
using UnityEngine.Analytics;

using System.Collections.Generic;

using BaseMatchEvents;

public class tnAnalyticsManager : MonoBehaviour
{
    // Fields

    private Dictionary<string, object> m_Data = new Dictionary<string, object>();

    // MonoBehaviour's interface

    void OnEnable()
    {
        Messenger.AddListener<tnStartMatchEventParams>("MatchStarted", OnMatchStarted);
    }

    void OnDisable()
    {
        Messenger.RemoveListener<tnStartMatchEventParams>("MatchStarted", OnMatchStarted);
    }

    // EVENTS

    private void OnMatchStarted(tnStartMatchEventParams i_Params)
    {
        if (!IsValidMatch(i_Params))
            return;

        bool raiseEvent = (i_Params.online && PhotonNetwork.isMasterClient) || (i_Params.offline);

        if (!raiseEvent)
            return;

        string gameModeName;
        string stadiumName;
        string ballName;
       
        GetBallName(i_Params.ballId, out ballName);
        GetGameModeName(i_Params.gameModeId, out gameModeName);
        GetStadiumName(i_Params.stadiumId, out stadiumName);

        bool referee = i_Params.hasReferee;
        bool goldenGoal = i_Params.goldenGoalEnabled;

        string totalPlayers = i_Params.teamASize + "v" + i_Params.teamBSize;
        string humanPlayers = i_Params.teamAHumanPlayers + "v" + i_Params.teamBHumanPlayers;
        string aiPlayers = i_Params.teamAAIPlayers + "v" + i_Params.teamBAIPlayers;

        string eventName = (i_Params.online) ? "GameStarted_Online" : "GameStarted_Offline";
        SendCustomEvent(eventName, gameModeName, stadiumName, ballName, referee, goldenGoal, totalPlayers, humanPlayers, aiPlayers);
    }

    // INTERNALS

    private void SendCustomEvent(string i_Event, string i_GameMode, string i_Stadium, string i_Ball, bool i_Referee, bool i_GoldenGoal, string i_TotalPlayers, string i_HumanPlayers, string i_AIPlayers)
    {
        m_Data.Clear();

        m_Data.Add("GameMode", i_GameMode);
        m_Data.Add("Stadium", i_Stadium);
        m_Data.Add("Ball", i_Ball);
        m_Data.Add("Referee", i_Referee);
        m_Data.Add("GoldenGoal", i_GoldenGoal);
        m_Data.Add("TotalPlayers", i_TotalPlayers);
        m_Data.Add("HumanPlayers", i_HumanPlayers);
        m_Data.Add("AIPlayers", i_HumanPlayers);

        Analytics.CustomEvent(i_Event, m_Data);
    }

    // UTILS

    private void GetBallName(int i_BallId, out string o_Name)
    {
        o_Name = "";

        tnBallData ballData = tnGameData.GetBallDataMain(i_BallId);

        if (ballData != null)
        {
            o_Name = ballData.name;
        }
    }

    private void GetGameModeName(int i_GameModeId, out string o_Name)
    {
        o_Name = "";

        tnGameModeData gameModeData = tnGameData.GetGameModeDataMain(i_GameModeId);

        if (gameModeData != null)
        {
            o_Name = gameModeData.name;
        }
    }

    private void GetStadiumName(int i_StadiumId, out string o_Name)
    {
        o_Name = "";

        tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(i_StadiumId);

        if (stadiumData != null)
        {
            o_Name = stadiumData.name;
        }
    }

    private bool IsValidMatch(tnStartMatchEventParams i_Params)
    {
        int teamAHumanPlayers = i_Params.teamAHumanPlayers;
        int teamBHumanPlayers = i_Params.teamBHumanPlayers;

        return (teamAHumanPlayers > 0 || teamBHumanPlayers > 0);
    }
}
