using UnityEngine;

using System.Xml;
using System.Collections.Generic;

public class TextManager : Singleton<TextManager>
{
    // Fields

    private Dictionary<int, string> m_Texts = new Dictionary<int, string>();

    // STATIC METHODS

    public static void InitializeMain(string i_FilePath)
    {
        if (Instance != null)
        {
            Instance.Initialize(i_FilePath);
        }
    }

    public static string GetTextMain(string i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetText(i_Id);
        }

        return "";
    }

    public static string GetTextMain(int i_Id)
    {
        if (Instance != null)
        {
            return Instance.GetText(i_Id);
        }

        return "";
    }

    public static bool TryGetTextMain(string i_Id, out string o_Text)
    {
        o_Text = "";

        if (Instance != null)
        {
            return Instance.TryGetText(i_Id, out o_Text);
        }

        return false;
    }

    public static bool TryGetTextMain(int i_Id, out string o_Text)
    {
        o_Text = "";

        if (Instance != null)
        {
            return Instance.TryGetText(i_Id, out o_Text);
        }

        return false;
    }

    // LOGIC

    public void Initialize(string i_ResourcePath)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>(i_ResourcePath);

        if (xmlAsset == null)
        {
            Debug.LogError(i_ResourcePath + ": Failed to load. Missing file or directory.");
            return;
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml (xmlAsset.text);

        XmlNodeList textEntries = xmlDoc.GetElementsByTagName("TextEntry");

        foreach (XmlNode textEntry in textEntries)
        {
            XmlAttribute attribureId = textEntry.Attributes["ID"];

            if (attribureId == null)
                return;

            string textId = attribureId.InnerText;

            string text = textEntry.InnerText;
            text = text.Replace("_NEWLINE_", "\n");

            int hash = StringUtils.GetHashCode(textId);
            if (!m_Texts.ContainsKey(hash))
            {
                m_Texts.Add(hash, text);
            }
            else
            {
                Debug.LogWarning("A text with ID '" + textId + "' already exists in dictionary. It will not be added.");
            }
        }
    }

    public string GetText(string i_Id)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return GetText(hash);
    }

    public string GetText(int i_Id)
    {
        string textValue;
        m_Texts.TryGetValue(i_Id, out textValue);
        return textValue;
    }

    public bool TryGetText(string i_Id, out string o_Text)
    {
        int hash = StringUtils.GetHashCode(i_Id);
        return TryGetText(hash, out o_Text);
    }

    public bool TryGetText(int i_Id, out string o_Text)
    {
        return m_Texts.TryGetValue(i_Id, out o_Text);
    }
}
