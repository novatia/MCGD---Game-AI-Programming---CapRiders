using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using GoUI;

public class tnView_Countdown : GoUI.UIView
{
    [SerializeField]
    private Image m_Image = null;

    [SerializeField]
    private List<Sprite> m_Sprites = new List<Sprite>();
    [SerializeField]
    private List<SfxDescriptor> m_Sfxs = new List<SfxDescriptor>();

    // UIView's interface

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

    public void SetIndex(int i_Index)
    {
        InternalSetIndex(i_Index);
    }

    public void Clear()
    {
        InternalClear();
    }

    // INTERNALS

    private void InternalSetIndex(int i_Index)
    {
        // Play SFX.

        SfxDescriptor sfx = InternalGetSfx(i_Index);
        SfxPlayer.PlayMain(sfx);

        // Select frame image.

        Internal_SetImageActive(true);

        Sprite nextSprite = InternalGetSprite(i_Index);
        Internal_SetImage(nextSprite);
    }

    private void InternalClear()
    {
        Internal_SetImage(null);
        Internal_SetImageActive(false);
    }

    private Sprite InternalGetSprite(int i_Index)
    {
        if (m_Sprites.Count == 0)
        {
            return null;
        }

        int index = Mathf.Clamp(i_Index, 0, m_Sprites.Count - 1);
        Sprite s = m_Sprites[index];
        return s;
    }

    private SfxDescriptor InternalGetSfx(int i_Index)
    {
        if (m_Sfxs.Count == 0)
        {
            return null;
        }

        int index = Mathf.Clamp(i_Index, 0, m_Sfxs.Count - 1);
        SfxDescriptor s = m_Sfxs[index];
        return s;
    }

    private void Internal_SetImageActive(bool i_Active)
    {
        if (m_Image != null)
        {
            m_Image.gameObject.SetActive(i_Active);
        }
    }

    private void Internal_SetImage(Sprite i_Sprite)
    {
        if (m_Image != null)
        {
            m_Image.sprite = i_Sprite;
        }
    }
}
