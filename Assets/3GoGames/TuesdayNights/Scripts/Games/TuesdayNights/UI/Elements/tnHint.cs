using UnityEngine;
using System.Collections;

public class tnHint : MonoBehaviour
{
    [TextArea]
    [SerializeField]
    private string m_Text = "";

    public string text
    {
        get
        {
            return m_Text;
        }
    }
}
