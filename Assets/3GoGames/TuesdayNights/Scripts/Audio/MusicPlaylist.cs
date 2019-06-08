using UnityEngine;
using System.Collections;

public class MusicPlaylist : ScriptableObject
{
    [SerializeField]
    private AudioClip[] m_Tracks = null;

    public int tracksCount
    {
        get
        {
			if (m_Tracks == null)
			{
				return 0;
			}
			
            return m_Tracks.Length;
        }
    }

    public AudioClip GetTrack(int i_Index)
    {
		if (m_Tracks == null)
		{
			return null;
		}
		
        if (i_Index < 0 || i_Index >= m_Tracks.Length)
        {
            return null;
        }

        return m_Tracks[i_Index];
    }
}
