using UnityEngine;
using UnityEngine.UI;

public class tnUITeamInfo : MonoBehaviour
{
    [SerializeField]
    private Image m_TeamFlag = null;
    [SerializeField]
    private Text m_TeamName = null;

    // LOGIC

    public void SetFlag(Sprite i_Sprite)
    {
        if (m_TeamFlag != null)
        {
            m_TeamFlag.sprite = i_Sprite;
        }
    }

    public void SetName(string i_Name)
    {
        if (m_TeamName != null)
        {
            m_TeamName.text = i_Name;
        }
    }
}
