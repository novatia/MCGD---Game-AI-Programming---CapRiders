using UnityEngine;
using System.Collections.Generic;

public class tnGameData : Singleton<tnGameData>
{
    // STATIC

    public static void InitializeMain()
    {
        if (Instance != null)
        {
            Instance.Initialize();
        }
    }

    // Game Modes

    public static int gameModesCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.gameModesCount;
            }

            return 0;
        }
    }

    public static List<int> GetGameModesKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetGameModesKeys();
        }

        return null;
    }

    public static tnGameModeData GetGameModeDataMain(string i_GameModeId)
    {
        if (Instance != null)
        {
            return Instance.GetGameModeData(i_GameModeId);
        }

        return null;
    }

    public static tnGameModeData GetGameModeDataMain(int i_GameModeId)
    {
        if (Instance != null)
        {
            return Instance.GetGameModeData(i_GameModeId);
        }

        return null;
    }

    // AI

    public static int aiLevelCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.aiLevelCount;
            }

            return 0;
        }
    }

    public static tnAILevel GetAILevelMain(int i_Index)
    {
        if (Instance != null)
        {
            return Instance.GetAILevel(i_Index);
        }

        return null;
    }

    // Characters

    public static string defaultCharacterPrefabPathMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.defaultCharacterPrefabPath;
            }

            return string.Empty;
        }
    }

    public static int charactersCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.charactersCount;
            }

            return 0;
        }
    }

    public static List<int> GetCharactersKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetCharactersKeys();
        }

        return null;
    }

    public static GameObject LoadAndGetDefaultCharacterPrefabMain()
    {
        if (Instance != null)
        {
            return Instance.LoadAndGetDefaultCharacterPrefab();
        }

        return null;
    }

    public static tnCharacterData GetCharacterDataMain(string i_CharacterId)
    {
        if (Instance != null)
        {
            return Instance.GetCharacterData(i_CharacterId);
        }

        return null;
    }

    public static tnCharacterData GetCharacterDataMain(int i_CharacterId)
    {
        if (Instance != null)
        {
            return Instance.GetCharacterData(i_CharacterId);
        }

        return null;
    }

    // Teams

    public static int teamsCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.teamsCount;
            }

            return 0;
        }
    }

    public static List<int> GetTeamsKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetTeamsKeys();
        }

        return null;
    }

    public static tnTeamData GetTeamDataMain(string i_TeamId)
    {
        if (Instance != null)
        {
            return Instance.GetTeamData(i_TeamId);
        }

        return null;
    }

    public static tnTeamData GetTeamDataMain(int i_TeamId)
    {
        if (Instance != null)
        {
            return Instance.GetTeamData(i_TeamId);
        }

        return null;
    }

    // Stadiums

    public static int stadiumsCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.stadiumsCount;
            }

            return 0;
        }
    }

    public static List<int> GetStadiumsKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetStadiumsKeys();
        }

        return null;
    }

    public static tnStadiumData GetStadiumDataMain(string i_StadiumId)
    {
        if (Instance != null)
        {
            return Instance.GetStadiumData(i_StadiumId);
        }

        return null;
    }

    public static tnStadiumData GetStadiumDataMain(int i_StadiumId)
    {
        if (Instance != null)
        {
            return Instance.GetStadiumData(i_StadiumId);
        }

        return null;
    }

    // Balls

    public static string ballPrefabPathMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.ballPrefabPath;
            }

            return string.Empty;
        }
    }

    public static int ballsCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.ballsCount;
            }

            return 0;
        }
    }

    public static List<int> GetBallsKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetBallsKeys();
        }

        return null;
    }

    public static tnBall LoadAndGetBallPrefabMain()
    {
        if (Instance != null)
        {
            return Instance.LoadAndGetBallPrefab();
        }

        return null;
    }

    public static tnBallData GetBallDataMain(string i_BallId)
    {
        if (Instance != null)
        {
            return Instance.GetBallData(i_BallId);
        }

        return null;
    }

    public static tnBallData GetBallDataMain(int i_BallId)
    {
        if (Instance != null)
        {
            return Instance.GetBallData(i_BallId);
        }

        return null;
    }

    // Players

    public static int playersCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.playersCount;
            }

            return 0;
        }
    }

    public static List<int> GetPlayersKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetPlayersKeys();
        }

        return null;
    }

    public static tnPlayerData GetPlayerDataMain(string i_PlayerId)
    {
        if (Instance != null)
        {
            return Instance.GetPlayerData(i_PlayerId);
        }

        return null;
    }

    public static tnPlayerData GetPlayerDataMain(int i_PlayerId)
    {
        if (Instance != null)
        {
            return Instance.GetPlayerData(i_PlayerId);
        }

        return null;
    }

    // Online players

    public static int onlinePlayersCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.onlinePlayersCount;
            }

            return 0;
        }
    }

    public static List<int> GetOnlinePlayersKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetOnlinePlayersKeys();
        }

        return null;
    }

    public static tnOnlinePlayerData GetOnlinePlayerDataMain(string i_PlayerId)
    {
        if (Instance != null)
        {
            return Instance.GetOnlinePlayerData(i_PlayerId);
        }

        return null;
    }

    public static tnOnlinePlayerData GetOnlinePlayerDataMain(int i_PlayerId)
    {
        if (Instance != null)
        {
            return Instance.GetOnlinePlayerData(i_PlayerId);
        }

        return null;
    }

    public static tnOnlinePlayerData GetOnlinePlayerDataByIndexMain(int i_Index)
    {
        if (Instance != null)
        {
            return Instance.GetOnlinePlayerDataByIndex(i_Index);
        }

        return null;
    }

    // Cameras

    public static int camerasSetsCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.camerasSetsCount;
            }

            return 0;
        }
    }

    public static tnCamerasSet GetCameraSetMain(string i_CameraSetId)
    {
        if (Instance != null)
        {
            return Instance.GetCameraSet(i_CameraSetId);
        }

        return null;
    }

    public static tnCamerasSet GetCameraSetMain(int i_CameraSetId)
    {
        if (Instance != null)
        {
            return Instance.GetCameraSet(i_CameraSetId);
        }

        return null;
    }

    // Options

    public static List<int> GetMatchDurationOptionKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetMatchDurationOptionKeys();
        }

        return null;
    }

    public static bool TryGetMatchDurationValueMain(string i_Id, out float o_Value)
    {
        o_Value = 0f;

        if (Instance != null)
        {
            return Instance.TryGetMatchDurationValue(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetMatchDurationValueMain(int i_Id, out float o_Value)
    {
        o_Value = 0f;

        if (Instance != null)
        {
            return Instance.TryGetMatchDurationValue(i_Id, out o_Value);
        }

        return false;
    }

    public static List<int> GetRefereeOptionKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetRefereeOptionKeys();
        }

        return null;
    }

    public static bool TryGetRefereeValueMain(string i_Id, out string o_Value)
    {
        o_Value = "";

        if (Instance != null)
        {
            return Instance.TryGetRefereeValue(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetRefereeValueMain(int i_Id, out string o_Value)
    {
        o_Value = "";

        if (Instance != null)
        {
            return Instance.TryGetRefereeValue(i_Id, out o_Value);
        }

        return false;
    }

    public static List<int> GetGoldenGoalOptionKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetGoldenGoalOptionKeys();
        }

        return null;
    }

    public static bool TryGetGoldenGoalValueMain(string i_Id, out string o_Value)
    {
        o_Value = "";

        if (Instance != null)
        {
            return Instance.TryGetGoldenGoalValue(i_Id, out o_Value);
        }

        return false;
    }

    public static bool TryGetGoldenGoalValueMain(int i_Id, out string o_Value)
    {
        o_Value = "";

        if (Instance != null)
        {
            return Instance.TryGetGoldenGoalValue(i_Id, out o_Value);
        }

        return false;
    }

    // Configs

    public static tnGameModeConfig GetConfigDataMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetConfigData(i_Id);
        }

        return null;
    }

    public static tnGameModeConfig GetConfigDataMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetConfigData(i_Id);
        }

        return null;
    }

    // GameSettings

    public int gameSettingsCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.gameSettingsCount;
            }

            return 0;
        }
    }

    public List<int> GetGameSettingsKeysMain()
    {
        if (Instance != null)
        {
            return Instance.GetGameSettingsKeys();
        }

        return null;
    }

    public static bool HasGameSettingsKeyMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasGameSettingsKey(i_Id);
        }

        return false;
    }

    public static bool HasGameSettingsKeyMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.HasGameSettingsKey(i_Id);
        }

        return false;
    }

    public static string GetGameSettingsValueMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetGameSettingsValue(i_Id);
        }

        return null;
    }

    public static string GetGameSettingsValueMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetGameSettingsValue(i_Id);
        }

        return null;
    }

    // Credits

    public static int creditsCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.creditsCount;
            }

            return 0;
        }
    }
    
    public static int specialThanksCountMain
    {
        get
        {
            if (Instance != null)
            {
                return Instance.specialThanksCount;
            }

            return 0;
        }
    }

    public static tnCreditsData GetCreditsDataMain(int i_Index)
    {
        if (Instance != null)
        {
            return Instance.GetCreditsData(i_Index);
        }

        return null;
    }

    public static tnCreditsTextEntry GetCreditsTextMain(int i_Index)
    {
        if (Instance != null)
        {
            return Instance.GetCreditsText(i_Index);
        }

        return null;
    }

    // FIELDS

    private tnGameModesDatabaseManager m_GameModesManager = null;

    private tnAIDatabaseManager m_AIManager = null;
    private tnCharactersDatabaseManager m_CharactersManager = null;
    private tnTeamsDatabaseManager m_TeamsManager = null;
    private tnStadiumsDatabaseManager m_StadiumsManager = null;
    private tnBallsDatabaseManager m_BallsManager = null;
    private tnPlayerDatabaseManager m_PlayersManager = null;
    private tnOnlinePlayersDatabaseManager m_OnlinePlayersManager = null;
    private tnCameraDatabaseManager m_CamerasManager = null;

    private tnGameModeOptionsManager m_OptionsManager = null;
    private tnGameModeConfigsDatabaseManager m_ConfigsManager = null;

    private tnGameSettingsDatabaseManager m_GameSettingsManager = null;

    private tnCreditsDatabaseManager m_CreditsManager = null;

    // MonoBehaviour's interface

    void Awake()
    {
        m_GameModesManager = new tnGameModesDatabaseManager();

        m_AIManager = new tnAIDatabaseManager();
        m_CharactersManager = new tnCharactersDatabaseManager();
        m_TeamsManager = new tnTeamsDatabaseManager();
        m_StadiumsManager = new tnStadiumsDatabaseManager();
        m_BallsManager = new tnBallsDatabaseManager();
        m_PlayersManager = new tnPlayerDatabaseManager();
        m_OnlinePlayersManager = new tnOnlinePlayersDatabaseManager();
        m_CamerasManager = new tnCameraDatabaseManager();

        m_OptionsManager = new tnGameModeOptionsManager();
        m_ConfigsManager = new tnGameModeConfigsDatabaseManager();

        m_GameSettingsManager = new tnGameSettingsDatabaseManager();

        m_CreditsManager = new tnCreditsDatabaseManager();
    }

    // LOGIC

    public void Initialize()
    {
        m_GameModesManager.Initialize("Database/Game/GameModesDatabase");

        m_AIManager.Initialize("Database/Game/AIDatabase");
        m_CharactersManager.Initialize("Database/Game/CharactersDatabase");
        m_TeamsManager.Initialize("Database/Game/TeamsDatabase");
        m_StadiumsManager.Initialize("Database/Game/StadiumsDatabase");
        m_BallsManager.Initialize("Database/Game/BallsDatabase");
        m_PlayersManager.Initialize("Database/Game/PlayersDatabase");
        m_OnlinePlayersManager.Initialize("Database/Game/OnlinePlayersDatabase");
        m_CamerasManager.Initialize("Database/Game/CamerasDatabase");

        m_OptionsManager.Initialize("Database/Game/GameModeOptions");
        m_ConfigsManager.Initialize("Database/Game/GameModeConfigsDatabase");

        m_GameSettingsManager.Initialize("Database/Game/GameSettingsDatabase");

        m_CreditsManager.Initialize("Database/Credits/CreditsDatabase");
    }

    // Game Modes

    public int gameModesCount
    {
        get { return m_GameModesManager.dataCount; }
    }

    public List<int> GetGameModesKeys()
    {
        return m_GameModesManager.GetKeys();
    }

    public tnGameModeData GetGameModeData(string i_GameModeId)
    {
        return m_GameModesManager.GetData(i_GameModeId);
    }

    public tnGameModeData GetGameModeData(int i_GameModeId)
    {
        return m_GameModesManager.GetData(i_GameModeId);
    }

    // AI

    public int aiLevelCount
    {
        get
        {
            return m_AIManager.aiLevelCount;
        }
    }

    public tnAILevel GetAILevel(int i_Index)
    {
        return m_AIManager.GetAILevel(i_Index);
    }

    // Characters

    public string defaultCharacterPrefabPath
    {
        get
        {
            return m_CharactersManager.defaultCharacterPrefabPath;
        }
    }

    public int charactersCount
    {
        get { return m_CharactersManager.dataCount; }
    }

    public List<int> GetCharactersKeys()
    {
        return m_CharactersManager.GetKeys();
    }

    public GameObject LoadAndGetDefaultCharacterPrefab()
    {
        return m_CharactersManager.LoadAndGetDefaultCharacterPrefab();
    }

    public tnCharacterData GetCharacterData(string i_CharacterId)
    {
        return m_CharactersManager.GetData(i_CharacterId);
    }

    public tnCharacterData GetCharacterData(int i_CharacterId)
    {
        return m_CharactersManager.GetData(i_CharacterId);
    }

    // Teams

    public int teamsCount
    {
        get { return m_TeamsManager.dataCount; }
    }

    public List<int> GetTeamsKeys()
    {
        return m_TeamsManager.GetKeys();
    }

    public tnTeamData GetTeamData(string i_TeamId)
    {
        return m_TeamsManager.GetData(i_TeamId);
    }

    public tnTeamData GetTeamData(int i_TeamId)
    {
        return m_TeamsManager.GetData(i_TeamId);
    }

    // Stadiums

    public int stadiumsCount
    {
        get { return m_StadiumsManager.dataCount; }
    }

    public List<int> GetStadiumsKeys()
    {
        return m_StadiumsManager.GetKeys();
    }

    public tnStadiumData GetStadiumData(string i_StadiumId)
    {
        return m_StadiumsManager.GetData(i_StadiumId);
    }

    public tnStadiumData GetStadiumData(int i_StadiumId)
    {
        return m_StadiumsManager.GetData(i_StadiumId);
    }

    // Balls

    public string ballPrefabPath
    {
        get
        {
            return m_BallsManager.ballPrefabPath;
        }
    }

    public int ballsCount
    {
        get { return m_BallsManager.dataCount; }
    }

    public List<int> GetBallsKeys()
    {
        return m_BallsManager.GetKeys();
    }

    public tnBall LoadAndGetBallPrefab()
    {
        return m_BallsManager.LoadAndGetBallPrefab();
    }

    public tnBallData GetBallData(string i_BallId)
    {
        return m_BallsManager.GetData(i_BallId);
    }

    public tnBallData GetBallData(int i_BallId)
    {
        return m_BallsManager.GetData(i_BallId);
    }

    // Players

    public int playersCount
    {
        get { return m_PlayersManager.dataCount; }
    }

    public List<int> GetPlayersKeys()
    {
        return m_PlayersManager.GetKeys();
    }

    public tnPlayerData GetPlayerData(string i_PlayerId)
    {
        return m_PlayersManager.GetData(i_PlayerId);
    }

    public tnPlayerData GetPlayerData(int i_PlayerId)
    {
        return m_PlayersManager.GetData(i_PlayerId);
    }

    // Online players

    public int onlinePlayersCount
    {
        get
        {
            return m_OnlinePlayersManager.dataCount;
        }
    }

    public List<int> GetOnlinePlayersKeys()
    {
        return m_OnlinePlayersManager.GetKeys();
    }

    public tnOnlinePlayerData GetOnlinePlayerData(string i_PlayerId)
    {
        return m_OnlinePlayersManager.GetData(i_PlayerId);
    }

    public tnOnlinePlayerData GetOnlinePlayerData(int i_PlayerId)
    {
        return m_OnlinePlayersManager.GetData(i_PlayerId);
    }

    public tnOnlinePlayerData GetOnlinePlayerDataByIndex(int i_Index)
    {
        return m_OnlinePlayersManager.GetDataByIndex(i_Index);
    }

    // Cameras

    public int camerasSetsCount
    {
        get { return m_CamerasManager.dataCount; }
    }

    public tnCamerasSet GetCameraSet(string i_CameraSetId)
    {
        return m_CamerasManager.GetData(i_CameraSetId);
    }

    public tnCamerasSet GetCameraSet(int i_CameraSetId)
    {
        return m_CamerasManager.GetData(i_CameraSetId);
    }

    // Options

    public List<int> GetMatchDurationOptionKeys()
    {
        return m_OptionsManager.GetMatchDurationOptionKeys();
    }

    public bool TryGetMatchDurationValue(string i_Id, out float o_Value)
    {
        return m_OptionsManager.TryGetMatchDurationValue(i_Id, out o_Value);
    }

    public bool TryGetMatchDurationValue(int i_Id, out float o_Value)
    {
        return m_OptionsManager.TryGetMatchDurationValue(i_Id, out o_Value);
    }

    public List<int> GetRefereeOptionKeys()
    {
        return m_OptionsManager.GetRefereeOptionKeys();
    }

    public bool TryGetRefereeValue(string i_Id, out string o_Value)
    {
        return m_OptionsManager.TryGetRefereeValue(i_Id, out o_Value);
    }

    public bool TryGetRefereeValue(int i_Id, out string o_Value)
    {
        return m_OptionsManager.TryGetRefereeValue(i_Id, out o_Value);
    }

    public List<int> GetGoldenGoalOptionKeys()
    {
        return m_OptionsManager.GetGoldenGoalOptionKeys();
    }

    public bool TryGetGoldenGoalValue(string i_Id, out string o_Value)
    {
        return m_OptionsManager.TryGetGoldenGoalValue(i_Id, out o_Value);
    }

    public bool TryGetGoldenGoalValue(int i_Id, out string o_Value)
    {
        return m_OptionsManager.TryGetGoldenGoalValue(i_Id, out o_Value);
    }

    // Configs

    public tnGameModeConfig GetConfigData(string i_Id)
    {
        return m_ConfigsManager.GetData(i_Id);
    }

    public tnGameModeConfig GetConfigData(int i_Id)
    {
        return m_ConfigsManager.GetData(i_Id);
    }

    // GameSettings

    public int gameSettingsCount
    {
        get
        {
            return m_GameSettingsManager.dataCount;
        }
    }

    public List<int> GetGameSettingsKeys()
    {
        return m_GameSettingsManager.GetKeys();
    }

    public bool HasGameSettingsKey(string i_Id)
    {
        return m_GameSettingsManager.HasKey(i_Id);
    }

    public bool HasGameSettingsKey(int i_Id)
    {
        return m_GameSettingsManager.HasKey(i_Id);
    }

    public string GetGameSettingsValue(string i_Id)
    {
        return m_GameSettingsManager.GetValue(i_Id);
    }

    public string GetGameSettingsValue(int i_Id)
    {
        return m_GameSettingsManager.GetValue(i_Id);
    }

    // Credits

    public int creditsCount
    {
        get
        {
            return m_CreditsManager.dataCount;
        }
    }

    public int specialThanksCount
    {
        get
        {
            return m_CreditsManager.textsCount;
        }
    }

    public tnCreditsData GetCreditsData(int i_Index)
    {
        return m_CreditsManager.GetData(i_Index);
    }

    public tnCreditsTextEntry GetCreditsText(int i_Index)
    {
        return m_CreditsManager.GetText(i_Index);
    }
}
