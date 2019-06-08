using UnityEditor;

public static class tnDataEditorUtils
{
    // Teams

    [MenuItem("Assets/Create/TuesdayNights/Database/Teams Database")]
    public static void CreateTeamsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnTeamsDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Data/Team")]
    public static void CreateTeam()
    {
        ScriptableObjectUtility.CreateAsset<tnTeamDataDescriptor>();
    }

    // Characters

    [MenuItem("Assets/Create/TuesdayNights/Database/Characters Database")]
    public static void CreateCharactersDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnCharactersDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Data/Character")]
    public static void CreateCharacter()
    {
        ScriptableObjectUtility.CreateAsset<tnCharacterDataDescriptor>();
    }

    // Stadiums

    [MenuItem("Assets/Create/TuesdayNights/Database/Stadiums Database")]
    public static void CreateStadiumsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnStadiumsDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Data/Stadium")]
    public static void CreateStadium()
    {
        ScriptableObjectUtility.CreateAsset<tnStadiumDataDescriptor>();
    }

    // Balls

    [MenuItem("Assets/Create/TuesdayNights/Database/Balls Database")]
    public static void CreateBallsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnBallsDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Data/Ball")]
    public static void CreateBall()
    {
        ScriptableObjectUtility.CreateAsset<tnBallDataDescriptor>();
    }

    // Players

    [MenuItem("Assets/Create/TuesdayNights/Database/Players Database")]
    public static void CreatePlayersDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnPlayersDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Data/Player")]
    public static void CreatePlayer()
    {
        ScriptableObjectUtility.CreateAsset<tnPlayerDataDescriptor>();
    }

    // Online Players

    [MenuItem("Assets/Create/TuesdayNights/Database/Online Players Database")]
    public static void CreateOnlinePlayersDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnOnlinePlayersDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Data/Online Player")]
    public static void CreateOnlinePlayer()
    {
        ScriptableObjectUtility.CreateAsset<tnOnlinePlayerDataDescriptor>();
    }

    // Cameras

    [MenuItem("Assets/Create/TuesdayNights/Database/Cameras Sets Database")]
    public static void CreateCamerasDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnCamerasDatabase>();
    }

    // Credits

    [MenuItem("Assets/Create/TuesdayNights/Database/Credits Database")]
    public static void CreateCreditsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnCreditsDatabase>();
    }

    [MenuItem("Assets/Create/TuesdayNights/Database/Credits Data")]
    public static void CreateCreditsData()
    {
        ScriptableObjectUtility.CreateAsset<tnCreditsDataDescriptor>();
    }

    // Game settings

    [MenuItem("Assets/Create/TuesdayNights/Database/GameSettings Database")]
    public static void CreateSettingsDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnGameSettingsDatabase>();
    }

    // AI

    [MenuItem("Assets/Create/TuesdayNights/Database/AI Database")]
    public static void CreateAIDatabase()
    {
        ScriptableObjectUtility.CreateAsset<tnAIDatabase>();
    }
}
