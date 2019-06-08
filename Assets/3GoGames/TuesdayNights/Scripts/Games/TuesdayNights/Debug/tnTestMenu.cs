using UnityEngine;
using UnityEngine.SceneManagement;

using TuesdayNights;

public enum GameMode
{
    Standard = 0,
    Chargers = 1,
}

public enum Stadium
{
    CompetitiveArena = 0,
    KonigPark = 1,
    ClayStade = 2,
    FlipperArena = 3,
    PoolPalace = 4,
    CircleGarden = 5,
    InfiniteStadium = 6,
}

public enum Ball
{
    Royale = 0,
    Gireiro = 1,
    Standard = 2,
    Luxury = 3,
    Ball8 = 4,
    Armadillo = 5,
    Beach = 6,
    Blossom = 7,
    Candy = 8,
}

public enum Team
{
    Italy = 0,
    Spain = 1,
    France = 2,
    England = 3,
    Germany = 4,
    Portugual = 5,
    Sweden = 6,
    Belgium = 7,
    Russia = 8,
    Croatia = 9,
    Poland = 10,
    Ireland = 11,
}

public enum BooleanOption
{
    OFF = 0,
    ON = 1,
}

public enum MatchDurationOption
{
    OneMinute,
    TwoMinutes,
    ThreeMinutes,
    FiveMinutes,
    TenMinutes,
}

public class tnTestMenu : MonoBehaviour
{
    // STATIC

    private static string s_GameScene = "Game";

    private static string[] s_PlayerIds = new string[]
    {
        "Player0",
        "Player1",
        "Player2",
        "Player3",
        "Player4",
        "Player5",
        "Player6",
        "Player7",
        "Player8",
        "Player9",
        "Player10",
        "Player12",
        "Player13",
        "Player14",
        "Player15",
        "Player16",
        "Player17",
        "Player18",
        "Player19",
        "Player20",
        "Player21",
    };

private static string[] s_BallIds = new string[]
    {
        "royale",
        "gireiro",
        "standard",
        "luxury",
        "8ball",
        "armadillo",
        "beach",
        "blossom",
        "candy",
    };

    private static string[] s_StadiumIds = new string[]
    {
        "competitive_arena",
        "konig_park",
        "clay_stade",
        "flipper_arena",
        "pool_palace",
        "circle_garden",
        "inifinite_stadium",
    };

    private static string[] s_TeamIds = new string[]
    {
        "ITA",
        "SPA",
        "FRA",
        "ENG",
        "GER",
        "POR",
        "SWE",
        "BEL",
        "RUS",
        "CRO",
        "POL",
        "IRE",
    };

    private static string[] s_GameModeIds = new string[]
    {
        "GM_STANDARD",
        "GM_SUBBUTEO",
    };

    private static string[] s_RefereeOptionIds = new string[]
    {
        "OFF",
        "ON",
    };

    private static string[] s_GoldenGoalOptionIds = new string[]
    {
        "OFF",
        "ON",
    };

    private static string[] s_MatchDurationOptionIds = new string[]
    {
        "MD_60",
        "MD_120",
        "MD_180",
        "MD_300",
        "MD_600",
    };

    // Serializeable fields

    [SerializeField]
    private GameMode m_GameMode = GameMode.Standard;
    [SerializeField]
    private Ball m_Ball = Ball.Standard;
    [SerializeField]
    private Stadium m_Stadium = Stadium.KonigPark;
    [SerializeField]
    private MatchDurationOption m_MatchDuration = MatchDurationOption.ThreeMinutes;
    [SerializeField]
    private BooleanOption m_GoldenGoal = BooleanOption.ON;
    [SerializeField]
    private BooleanOption m_Referee = BooleanOption.OFF;

    [SerializeField]
    private Team m_TeamA = Team.Italy;
    [SerializeField]
    private Team m_TeamB = Team.Spain;

    [SerializeField]
    private int m_PlayersPerTeam = 1;

    [SerializeField]
    private bool m_AllHumans = false;
    [SerializeField]
    private bool m_AllCPUs = false;

    // MonoBehaviour's interface

    private void Start()
    {
        PhotonNetwork.offlineMode = true;

        m_AllCPUs = (m_AllHumans) ? false : m_AllCPUs;

        FillModules();

        LoadGameScene();
    }

    // LOGIC

    public void FillModules()
    {
        Internal_FillModules();
    }

    public string GetStadiumId()
    {
        return GetStadiumId(m_Stadium);
    }

    public string GetBallId()
    {
        return GetBallId(m_Ball);
    }

    public string GetTeamA()
    {
        return GetTeamId(m_TeamA);
    }

    public string GetTeamB()
    {
        return GetTeamId(m_TeamB);
    }

    public Color GetTeamAColor()
    {
        string teamAId = GetTeamA();
        string teamBId = GetTeamB();

        int teamAIdHashed = StringUtils.GetHashCode(teamAId);
        int teamBIdHashed = StringUtils.GetHashCode(teamBId);

        int[] ids = new int[] { teamAIdHashed, teamBIdHashed };
        Color[] colors = Utils.ComputeTeamColors(ids);

        if (colors != null && colors.Length > 0)
        {
            return colors[0];
        }

        return Color.white;
    }

    public Color GetTeamBColor()
    {
        string teamAId = GetTeamA();
        string teamBId = GetTeamB();

        int teamAIdHashed = StringUtils.GetHashCode(teamAId);
        int teamBIdHashed = StringUtils.GetHashCode(teamBId);

        int[] ids = new int[] { teamAIdHashed, teamBIdHashed };
        Color[] colors = Utils.ComputeTeamColors(ids);

        if (colors != null && colors.Length > 1)
        {
            return colors[1];
        }

        return Color.white;
    }

    public int GetMinPlayers()
    {
        tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(GetStadiumId());

        if (stadiumData == null)
        {
            return 0;
        }

        IntRange teamSizeRange = stadiumData.teamSize;
        if (teamSizeRange == null)
        {
            return 0;
        }

        return teamSizeRange.min * 2;
    }

    public int GetMaxPlayers()
    {
        tnStadiumData stadiumData = tnGameData.GetStadiumDataMain(GetStadiumId());

        if (stadiumData == null)
        {
            return int.MaxValue;
        }

        IntRange teamSizeRange = stadiumData.teamSize;
        if (teamSizeRange == null)
        {
            return int.MaxValue;
        }

        return teamSizeRange.max * 2;
    }

    public bool WillPlayerPerTeamBeClamped()
    {
        int min = GetMinPlayers();
        int max = GetMaxPlayers();

        int totalPlayers = m_PlayersPerTeam * 2;

        return !(min <= totalPlayers && totalPlayers <= max);
    }

    // INTERNALS

    private void Internal_FillModules()
    {
        Internal_FillMatchSettingsModule();
        Internal_FillTeamsModule();
    }

    private void Internal_FillMatchSettingsModule()
    {
        tnMatchSettingsModule module = GameModulesManager.GetModuleMain<tnMatchSettingsModule>();

        if (module == null)
            return;

        string gameModeId = GetGameModeId(m_GameMode);
        string ballId = GetBallId(m_Ball);
        string stadiumId = GetStadiumId(m_Stadium);
        string matchDurationOptionId = GetMatchDurationOptionId(m_MatchDuration);
        string goldenGoalOptionId = GetGoldenGoalOptionId(m_GoldenGoal);
        string refereeOptionId = GetRefereeOptionId(m_Referee);

        module.SetGameModeId(gameModeId);
        module.SetBallId(ballId);
        module.SetStadiumId(stadiumId);
        module.SetMatchDurationOption(matchDurationOptionId);
        module.SetGoldenGoalOption(goldenGoalOptionId);
        module.SetRefereeOption(refereeOptionId);
    }

    private void Internal_FillTeamsModule()
    {
        tnTeamsModule teamsModule = GameModulesManager.GetModuleMain<tnTeamsModule>();
        if (teamsModule != null)
        {
            int minPlayers = GetMinPlayers();
            int maxPlayers = GetMaxPlayers();

            int total = m_PlayersPerTeam * 2;
            int clampedTotal = Mathf.Clamp(total, minPlayers, maxPlayers);
            int characterCount = clampedTotal / 2;
            
            Internal_AddTeamA(teamsModule, characterCount);
            Internal_AddTeamB(teamsModule, characterCount);
        }
    }

    private void Internal_AddTeamA(tnTeamsModule i_TeamsModule, int i_CharacterCount)
    {
        if (i_TeamsModule == null)
            return;

        tnTeamDescription teamDescription = new tnTeamDescription();

        string teamId = GetTeamA();
        teamDescription.SetTeamId(teamId);

        Color teamColor = GetTeamAColor();
        teamDescription.SetTeamColor(teamColor);

        if (i_CharacterCount >= 0)
        {
            tnTeamData teamData = tnGameData.GetTeamDataMain(teamId);
            if (teamData != null)
            {
                int characterCount = Mathf.Min(i_CharacterCount, teamData.charactersCount);
                for (int index = 0; index < characterCount; ++index)
                {
                    tnCharacterDescription characterDescription = new tnCharacterDescription();
                    characterDescription.SetCharacterId(teamData.GetCharacterKey(index));
                    characterDescription.SetSpawnOrder(index);

                    if (!m_AllCPUs && (index == 0 || m_AllHumans))
                    {
                        characterDescription.SetPlayerId(GetPlayerIdByIndex(index));
                    }

                    teamDescription.AddCharacterDescription(characterDescription);
                }
            }
        }

        i_TeamsModule.AddTeamDescription(teamDescription);
    }

    private void Internal_AddTeamB(tnTeamsModule i_TeamsModule, int i_CharacterCount)
    {
        if (i_TeamsModule == null)
            return;

        tnTeamDescription teamDescription = new tnTeamDescription();

        string teamId = GetTeamB();
        teamDescription.SetTeamId(teamId);

        Color teamColor = GetTeamBColor();
        teamDescription.SetTeamColor(teamColor);

        if (i_CharacterCount >= 0)
        {
            tnTeamData teamData = tnGameData.GetTeamDataMain(teamId);
            if (teamData != null)
            {
                int characterCount = Mathf.Min(i_CharacterCount, teamData.charactersCount);
                for (int index = 0; index < characterCount; ++index)
                {
                    tnCharacterDescription characterDescription = new tnCharacterDescription();
                    characterDescription.SetCharacterId(teamData.GetCharacterKey(index));
                    characterDescription.SetSpawnOrder(index);

                    if (m_AllHumans)
                    {
                        characterDescription.SetPlayerId(GetPlayerIdByIndex(index + characterCount));
                    }

                    teamDescription.AddCharacterDescription(characterDescription);
                }
            }
        }

        i_TeamsModule.AddTeamDescription(teamDescription);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(s_GameScene, LoadSceneMode.Single);
    }

    // UTILS

    private string GetGameModeId(GameMode i_GameMode)
    {
        int index = (int)i_GameMode;
        return GetGameModeIdByIndex(index);
    }

    private string GetBallId(Ball i_Ball)
    {
        int index = (int)i_Ball;
        return GetBallIdByIndex(index);
    }

    private string GetStadiumId(Stadium i_Stadium)
    {
        int index = (int)i_Stadium;
        return GetStadiumIdByIndex(index);
    }

    private string GetTeamId(Team i_Team)
    {
        int index = (int)i_Team;
        return GetTeamIdByIndex(index);
    }

    private string GetRefereeOptionId(BooleanOption i_Option)
    {
        int index = (int)i_Option;
        return GetRefereeOptionIdByIndex(index);
    }

    private string GetGoldenGoalOptionId(BooleanOption i_Option)
    {
        int index = (int)i_Option;
        return GetGoldenGoalOptionIdByIndex(index);
    }

    private string GetMatchDurationOptionId(MatchDurationOption i_MatchDuration)
    {
        int index = (int)i_MatchDuration;
        return GetMatchDurationOptionIdByIndex(index);
    }

    private string GetGameModeIdByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_GameModeIds.Length)
        {
            return "";
        }

        return s_GameModeIds[i_Index];
    }

    private string GetBallIdByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_BallIds.Length)
        {
            return "";
        }

        return s_BallIds[i_Index];
    }

    private string GetStadiumIdByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_StadiumIds.Length)
        {
            return "";
        }

        return s_StadiumIds[i_Index];
    }

    private string GetTeamIdByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_TeamIds.Length)
        {
            return "";
        }

        return s_TeamIds[i_Index];
    }

    private string GetRefereeOptionIdByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_RefereeOptionIds.Length)
        {
            return "";
        }

        return s_RefereeOptionIds[i_Index];
    }

    private string GetGoldenGoalOptionIdByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_GoldenGoalOptionIds.Length)
        {
            return "";
        }

        return s_GoldenGoalOptionIds[i_Index];
    }

    private string GetMatchDurationOptionIdByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_MatchDurationOptionIds.Length)
        {
            return "";
        }

        return s_MatchDurationOptionIds[i_Index];
    }

    private string GetPlayerIdByIndex(int i_Index)
    {
        if (i_Index < 0 || i_Index >= s_PlayerIds.Length)
        {
            return "";
        }

        return s_PlayerIds[i_Index];
    }
}
