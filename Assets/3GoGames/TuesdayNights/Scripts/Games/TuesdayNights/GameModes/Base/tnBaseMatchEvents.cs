using System.Collections.Generic;

using TrueSync;

namespace BaseMatchEvents
{
    public struct tnGoalEventParams
    {
        private int m_TeamId;

        private int m_ScorerId;
        private int m_ScorerTeamId;

        private bool m_IsHumanScorer;
        private bool m_IsLocalScorer;

        private bool m_HasValidScorer;

        public int teamId
        {
            get { return m_TeamId; }
        }

        public int scorerId
        {
            get { return m_ScorerId; }
        }

        public int scorerTeamId
        {
            get { return m_ScorerTeamId; }
        }

        public bool isHumanScorer
        {
            get
            {
                return m_IsHumanScorer;
            }
        }

        public bool isLocalScorer
        {
            get { return m_IsLocalScorer; }
        }

        public bool hasValidScorer
        {
            get
            {
                return m_HasValidScorer;
            }
        }

        public void SetTeamId(int i_TeamId)
        {
            m_TeamId = i_TeamId;
        }

        public void SetScorerId(int i_ScorerId)
        {
            m_ScorerId = i_ScorerId;
        }

        public void SetScorerTeamId(int i_ScorerTeamId)
        {
            m_ScorerTeamId = i_ScorerTeamId;
        }

        public void SetHasValidScorer(bool i_HasValidScorer)
        {
            m_HasValidScorer = i_HasValidScorer;
        }

        public void SetIsHumanScorer(bool i_IsHumanScorer)
        {
            m_IsHumanScorer = i_IsHumanScorer;
        }

        public void SetIsLocalScorer(bool i_IsLocalScorer)
        {
            m_IsLocalScorer = i_IsLocalScorer;
        }

        public bool isOwnGoal
        {
            get
            {
                return m_HasValidScorer && (m_TeamId == m_ScorerTeamId);
            } 
        }
    }

    public struct tnTouch
    {
        private int m_CharacterId;
        private int m_TeamId;

        private bool m_IsHuman;
        private bool m_IsLocal;

        private FP m_Timestamp;

        public int characterId
        {
            get { return m_CharacterId; }
        }

        public int teamId
        {
            get { return m_TeamId; }
        }

        public bool isHuman
        {
            get { return m_IsHuman; }
        }

        public bool isLocal
        {
            get { return m_IsLocal; }
        }

        public FP timestamp
        {
            get { return m_Timestamp; }
        }

        public void SetCharacterId(int i_CharacterId)
        {
            m_CharacterId = i_CharacterId;
        }

        public void SetTeamId(int i_TeamId)
        {
            m_TeamId = i_TeamId;
        }

        public void SetIsHuman(bool i_IsHuman)
        {
            m_IsHuman = i_IsHuman;
        }

        public void SetIsLocal(bool i_IsLocal)
        {
            m_IsLocal = i_IsLocal;
        }

        public void SetTimestamp(FP i_Timestamp)
        {
            m_Timestamp = i_Timestamp;
        }
    }

    public struct tnStartMatchEventParams
    {
        private bool m_Online;

        private int m_GameModeId;

        private int m_BallId;
        private int m_StadiumId;

        private int m_TeamAId;
        private int m_TeamBId;

        private bool m_HasReferee;

        private int m_TeamASize;
        private int m_TeamBSize;

        private int m_TeamAHumanPlayers;
        private int m_TeamBHumanPlayers;

        private bool m_GoldenGoalEnabled;

        private List<int> m_LocalTeamIds;
        private List<int> m_LocalCharacterIds;

        // GETTERS

        public bool online
        {
            get
            {
                return m_Online;
            }
        }

        public bool offline
        {
            get
            {
                return !m_Online;
            }
        }

        public int gameModeId
        {
            get
            {
                return m_GameModeId;
            }
        }

        public int ballId
        {
            get
            {
                return m_BallId;
            }
        }

        public int stadiumId
        {
            get
            {
                return m_StadiumId;
            }
        }

        public int teamAId
        {
            get
            {
                return m_TeamAId;
            }
        }

        public int teamBId
        {
            get
            {
                return m_TeamBId;
            }
        }

        public bool hasReferee
        {
            get
            {
                return m_HasReferee;
            }
        }

        public int teamASize
        {
            get
            {
                return m_TeamASize;
            }
        }

        public int teamBSize
        {
            get
            {
                return m_TeamBSize;
            }
        }

        public int teamAHumanPlayers
        {
            get
            {
                return m_TeamAHumanPlayers;
            }
        }

        public int teamBHumanPlayers
        {
            get
            {
                return m_TeamBHumanPlayers;
            }
        }

        public int teamAAIPlayers
        {
            get
            {
                return m_TeamASize - m_TeamAHumanPlayers;
            }
        }

        public int teamBAIPlayers
        {
            get
            {
                return m_TeamBSize - m_TeamBHumanPlayers;
            }
        }

        public bool goldenGoalEnabled
        {
            get
            {
                return m_GoldenGoalEnabled;
            }
        }

        public int localTeamCount
        {
            get
            {
                return m_LocalTeamIds.Count;
            }
        }

        public int localCharacterCount
        {
            get
            {
                return m_LocalCharacterIds.Count;
            }
        }

        public bool isTeamALocal
        {
            get { return IsLocalTeam(m_TeamAId); }
        }

        public bool isTeamBLocal
        {
            get { return IsLocalTeam(m_TeamBId); }
        }

        // LOGIC

        public void SetGameModeId(int i_GameModeId)
        {
            m_GameModeId = i_GameModeId;
        }

        public void SetBallId(int i_BallId)
        {
            m_BallId = i_BallId;
        }

        public void SetStadiumId(int i_StadiumId)
        {
            m_StadiumId = i_StadiumId;
        }

        public void SetTeamAId(int i_TeamId)
        {
            m_TeamAId = i_TeamId;
        }

        public void SetTeamBId(int i_TeamId)
        {
            m_TeamBId = i_TeamId;
        }

        public void SetHasReferee(bool i_HasReferee)
        {
            m_HasReferee = i_HasReferee;
        }

        public void SetTeamASize(int i_TeamSize)
        {
            m_TeamASize = i_TeamSize;
        }

        public void SetTeamBSize(int i_TeamSize)
        {
            m_TeamBSize = i_TeamSize;
        }

        public void SetGoldenGoalEnabled(bool i_GoldenGoalEnabled)
        {
            m_GoldenGoalEnabled = i_GoldenGoalEnabled;
        }

        public void SetTeamAHumanPlayers(int i_HumanPlayers)
        {
            m_TeamAHumanPlayers = i_HumanPlayers;
        }

        public void SetTeamBHumanPlayers(int i_HumanPlayers)
        {
            m_TeamBHumanPlayers = i_HumanPlayers;
        }

        public void AddLocalTeam(int i_TeamId)
        {
            if (!m_LocalTeamIds.Contains(i_TeamId))
            {
                m_LocalTeamIds.Add(i_TeamId);
            }
        }

        public void AddLocalCharacter(int i_CharacterId)
        {
            if (!m_LocalCharacterIds.Contains(i_CharacterId))
            {
                m_LocalCharacterIds.Add(i_CharacterId);
            }
        }

        public int GetLocalTeamId(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_LocalTeamIds.Count)
            {
                return Hash.s_NULL;
            }

            return m_LocalTeamIds[i_Index];
        }

        public int GetLocalCharacterId(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_LocalCharacterIds.Count)
            {
                return Hash.s_NULL;
            }

            return m_LocalCharacterIds[i_Index];
        }

        public bool IsLocalTeam(int i_TeamId)
        {
            return m_LocalTeamIds.Contains(i_TeamId);
        }

        public bool IsLocalCharacter(int i_CharacterId)
        {
            return m_LocalCharacterIds.Contains(i_CharacterId);
        }

        // CTOR

        public tnStartMatchEventParams(bool i_MatchOnline)
        {
            m_Online = i_MatchOnline;

            m_GameModeId = Hash.s_NULL;

            m_BallId = Hash.s_NULL;
            m_StadiumId = Hash.s_NULL;

            m_TeamAId = Hash.s_NULL;
            m_TeamBId = Hash.s_NULL;

            m_HasReferee = false;

            m_TeamASize = 0;
            m_TeamBSize = 0;

            m_TeamAHumanPlayers = 0;
            m_TeamBHumanPlayers = 0;

            m_GoldenGoalEnabled = false;

            m_LocalTeamIds = new List<int>();
            m_LocalCharacterIds = new List<int>();
        }
    }

    public struct tnEndMatchEventParams
    {
        private bool m_Online;

        private int m_GameModeId;

        private int m_BallId;
        private int m_StadiumId;

        private int m_TeamAId;
        private int m_TeamBId;

        private int m_ScoreA;
        private int m_ScoreB;

        private bool m_HasReferee;

        private int m_TeamASize;
        private int m_TeamBSize;

        private int m_TeamAHumanPlayers;
        private int m_TeamBHumanPlayers;

        private bool m_GoldenGoalEnabled;
        private bool m_GoldenGoal;

        private List<int> m_LocalTeamIds;
        private List<int> m_LocalCharacterIds;

        // GETTERS

        public bool online
        {
            get
            {
                return m_Online;
            }
        }

        public bool offline
        {
            get
            {
                return !m_Online;
            }
        }

        public int gameModeId
        {
            get
            {
                return m_GameModeId;
            }
        }

        public int ballId
        {
            get
            {
                return m_BallId;
            }
        }

        public int stadiumId
        {
            get
            {
                return m_StadiumId;
            }
        }

        public int teamAId
        {
            get
            {
                return m_TeamAId;
            }
        }

        public int teamBId
        {
            get
            {
                return m_TeamBId;
            }
        }

        public int scoreA
        {
            get
            {
                return m_ScoreA;
            }
        }

        public int scoreB
        {
            get
            {
                return m_ScoreB;
            }
        }

        public bool hasReferee
        {
            get
            {
                return m_HasReferee;
            }
        }

        public int teamASize
        {
            get
            {
                return m_TeamASize;
            }
        }

        public int teamBSize
        {
            get
            {
                return m_TeamBSize;
            }
        }

        public int teamAHumanPlayers
        {
            get
            {
                return m_TeamAHumanPlayers;
            }
        }

        public int teamBHumanPlayers
        {
            get
            {
                return m_TeamBHumanPlayers;
            }
        }

        public int teamAAIPlayers
        {
            get
            {
                return m_TeamASize - m_TeamAHumanPlayers;
            }
        }

        public int teamBAIPlayers
        {
            get
            {
                return m_TeamBSize - m_TeamBHumanPlayers;
            }
        }

        public bool goldenGoalEnabled
        {
            get
            {
                return m_GoldenGoalEnabled;
            }
        }

        public bool goldenGoal
        {
            get
            {
                return m_GoldenGoal;
            }
        }

        public int localTeamCount
        {
            get
            {
                return m_LocalTeamIds.Count;
            }
        }

        public int localCharacterCount
        {
            get
            {
                return m_LocalCharacterIds.Count;
            }
        }

        public bool isTeamALocal
        {
            get { return IsLocalTeam(m_TeamAId); }
        }

        public bool isTeamBLocal
        {
            get { return IsLocalTeam(m_TeamBId); }
        }

        // LOGIC

        public void SetGameModeId(int i_GameModeId)
        {
            m_GameModeId = i_GameModeId;
        }

        public void SetBallId(int i_BallId)
        {
            m_BallId = i_BallId;
        }

        public void SetStadiumId(int i_StadiumId)
        {
            m_StadiumId = i_StadiumId;
        }

        public void SetTeamAId(int i_TeamId)
        {
            m_TeamAId = i_TeamId;
        }

        public void SetTeamBId(int i_TeamId)
        {
            m_TeamBId = i_TeamId;
        }

        public void SetScoreA(int i_Score)
        {
            m_ScoreA = i_Score;
        }

        public void SetScoreB(int i_Score)
        {
            m_ScoreB = i_Score;
        }

        public void SetHasReferee(bool i_HasReferee)
        {
            m_HasReferee = i_HasReferee;
        }

        public void SetTeamASize(int i_TeamSize)
        {
            m_TeamASize = i_TeamSize;
        }

        public void SetTeamBSize(int i_TeamSize)
        {
            m_TeamBSize = i_TeamSize;
        }

        public void SetGoldenGoalEnabled(bool i_GoldenGoalEnabled)
        {
            m_GoldenGoalEnabled = i_GoldenGoalEnabled;
        }

        public void SetGoldenGoal(bool i_GoldenGoal)
        {
            m_GoldenGoal = i_GoldenGoal;
        }

        public void SetTeamAHumanPlayers(int i_HumanPlayers)
        {
            m_TeamAHumanPlayers = i_HumanPlayers;
        }

        public void SetTeamBHumanPlayers(int i_HumanPlayers)
        {
            m_TeamBHumanPlayers = i_HumanPlayers;
        }

        public void AddLocalTeam(int i_TeamId)
        {
            m_LocalTeamIds.Add(i_TeamId);
        }

        public void AddLocalCharacter(int i_CharacterId)
        {
            m_LocalCharacterIds.Add(i_CharacterId);
        }

        public int GetLocalTeamId(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_LocalTeamIds.Count)
            {
                return Hash.s_NULL;
            }

            return m_LocalTeamIds[i_Index];
        }

        public int GetLocalCharacterId(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_LocalCharacterIds.Count)
            {
                return Hash.s_NULL;
            }

            return m_LocalCharacterIds[i_Index];
        }

        public bool IsLocalTeam(int i_TeamId)
        {
            return m_LocalTeamIds.Contains(i_TeamId);
        }

        public bool IsLocalCharacter(int i_CharacterId)
        {
            return m_LocalCharacterIds.Contains(i_CharacterId);
        }

        // CTOR

        public tnEndMatchEventParams(bool i_MatchOnline)
        {
            m_Online = i_MatchOnline;

            m_GameModeId = Hash.s_NULL;

            m_BallId = Hash.s_NULL;
            m_StadiumId = Hash.s_NULL;

            m_TeamAId = Hash.s_NULL;
            m_TeamBId = Hash.s_NULL;

            m_ScoreA = 0;
            m_ScoreB = 0;

            m_HasReferee = false;

            m_TeamASize = 0;
            m_TeamBSize = 0;

            m_TeamAHumanPlayers = 0;
            m_TeamBHumanPlayers = 0;

            m_GoldenGoalEnabled = false;
            m_GoldenGoal = false;

            m_LocalTeamIds = new List<int>();
            m_LocalCharacterIds = new List<int>();
        }
    }
}