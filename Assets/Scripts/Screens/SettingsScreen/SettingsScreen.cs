using System.Collections;
using System.Collections.Generic;
using ShineCore;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : BaseScreen
{
    public SettingsPopup popup;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnOpen()
    {
        popup.Open();
    }


    public override void OnClose()
    {
        popup.Close();
    }

    public void OnCloseButtonClick()
    {
        CloseScreen();
    }

    public void OnMapButtonClick()
    {
        CloseScreen();
        SceneTransition.Transition("MapScene");
    }

    public void CloseScreen()
    {
        ScreenManager.CloseScreen(this.screenName);
    }
}
