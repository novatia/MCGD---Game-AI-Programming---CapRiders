using UnityEngine;

public class Comment : MonoBehaviour
{
    [SerializeField]
    [Multiline]
    private string m_Comment = "";

    public string comment
    {
        get
        {
            return m_Comment;
        }
    }
}
