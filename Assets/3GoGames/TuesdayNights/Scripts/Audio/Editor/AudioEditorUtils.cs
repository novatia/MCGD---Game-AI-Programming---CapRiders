using UnityEditor;

public static class AudioEditorUtils
{
    [MenuItem("Assets/Create/Audio/Playlist")]
    public static void CreateMusicPlaylist()
    {
        ScriptableObjectUtility.CreateAsset<MusicPlaylist>();
    }
}