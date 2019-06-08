using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class tnCredits : MonoBehaviour
{
    [SerializeField]
    private RectTransform m_Root = null;

    [SerializeField]
    private Color m_Color = Color.white;

    [SerializeField]
    private Font m_Font = null;
    [SerializeField]
    private int m_FontSize = 22;

    [SerializeField]
    private float m_ScrollSpeed = 15f;

    void Awake()
    {
        if (m_Root == null)
            return;

        for (int entryIndex = 0; entryIndex < tnGameData.specialThanksCountMain; ++entryIndex)
        {
            tnCreditsTextEntry entry = tnGameData.GetCreditsTextMain(entryIndex);

            GameObject entryGo = new GameObject("Entry");
            entryGo.transform.SetParent(m_Root, false);

            Text textComponent = entryGo.AddComponent<Text>();

            textComponent.text = entry.label;

            if (entry.overrideProperties)
            {
                textComponent.color = entry.color;

                textComponent.font = entry.font;
                textComponent.fontSize = entry.fontSize;
            }
            else
            {
                textComponent.color = m_Color;

                textComponent.font = m_Font;
                textComponent.fontSize = m_FontSize;
            }
        }
    }

    void OnEnable()
    {
        if (m_Root == null)
            return;

        m_Root.anchorMin = new Vector2(m_Root.anchorMin.x, 0f);
        m_Root.anchorMax = new Vector2(m_Root.anchorMax.x, 0f);

        m_Root.pivot = new Vector2(m_Root.pivot.x, 1f);

        Vector2 pos = m_Root.anchoredPosition;
        pos.y = 0f;

        m_Root.anchoredPosition = pos;
    }

    void Update()
    {
        if (m_Root == null)
            return;

        Vector2 pos = m_Root.anchoredPosition;
        pos.y += Time.deltaTime * m_ScrollSpeed;
        pos.y = Mathf.Min(pos.y, 10000f);

        m_Root.anchoredPosition = pos;
    }
}
