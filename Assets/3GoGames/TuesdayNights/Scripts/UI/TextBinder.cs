using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class TextBinder : MonoBehaviour 
{
    public string textId = "";

    private Text m_TextComponent;

    void Awake()
    {
        m_TextComponent = GetComponent<Text>();
    }

    void Start()
    {
        if (textId != "")
        {
            m_TextComponent.text = TextManager.GetTextMain(textId);
        }
    }
}
