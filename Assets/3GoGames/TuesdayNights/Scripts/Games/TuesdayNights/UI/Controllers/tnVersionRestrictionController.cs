using UnityEngine;
using UnityEngine.UI;

using TuesdayNights;

public class tnVersionRestrictionController : UIViewController
{
    [SerializeField]
    private Text m_Textbox = null;

    void Start()
    {
        if (m_Textbox == null)
            return;

        string dateLimit = tnGameData.GetGameSettingsValueMain(GlobalSettings.s_TimeRestriction);
        m_Textbox.text = dateLimit;
    }
}
