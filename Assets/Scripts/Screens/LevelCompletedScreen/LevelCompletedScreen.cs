using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class LevelCompletedScreen : BaseScreen
{
    public LevelCompletedPopup popup;
    public override void OnOpen()
    {
        popup.Open();
        SoundManager.Play(SoundKey.YOU_WIN);

    }

    public override void OnClose()
    {
        popup.Close();
    }
}
