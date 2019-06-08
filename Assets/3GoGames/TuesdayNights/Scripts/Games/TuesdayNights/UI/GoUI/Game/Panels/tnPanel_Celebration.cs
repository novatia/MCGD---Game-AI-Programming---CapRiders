using UnityEngine;

using System;

using GoUI;

public class tnPanel_Celebration : UIPanel<tnView_Celebration>
{
    // UIPanel's interface

    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnUpdate(float i_DeltaTime)
    {
        base.OnUpdate(i_DeltaTime);
    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    // LOGIC

    public void SetCelebrationText(int i_CharacterId)
    {
        string text = "";

        tnCharacterData characterData = tnGameData.GetCharacterDataMain(i_CharacterId);

        if (characterData != null)
        {
            string characterName = characterData.displayName;
            text = characterName + " SCORE";
        }

        SetCelebrationText(text);
    }

    public void SetCelebrationText(string i_Text)
    {
        if (viewInstance != null)
        {
            viewInstance.SetCelebrationText(i_Text);
        }
    }

    public void StartCelebration(Action i_CelebrationCompleted = null)
    {
        if (viewInstance != null)
        {
            viewInstance.StartCelebration(i_CelebrationCompleted);
        }
        else
        {
            if (i_CelebrationCompleted != null)
            {
                i_CelebrationCompleted();
            }
        }
    }
}