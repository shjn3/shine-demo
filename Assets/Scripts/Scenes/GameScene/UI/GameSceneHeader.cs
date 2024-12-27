using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Shine.Ads;
public class GameSceneHeader : MonoBehaviour
{
    public GameScene gameScene;
    public Text levelText;
    public BaseButton pauseButton;
    public BaseButton retryButton;
    public SafeArea safeArea;

    void Awake()
    {
        int level = DataStorage.GetInt(Player.PlayerDataKey.LEVEL, 1);
        levelText.text = "Level " + level;
    }


    void Start()
    {
        var position = transform.localPosition;
        position.y += safeArea.GetComponent<RectTransform>().sizeDelta.y;
        transform.localPosition = position;
    }

    void Update()
    {
        //
    }

    public void OnRetryButtonClick()
    {
        if (!gameScene.gameManager.gamePlay.IsCanRetry())
        {
            return;
        }
        gameScene.SetDisableButtons(true);
        AdsManager.ShowInterstitialAd((isShowed) =>
        {
            gameScene.SetDisableButtons(false);
            gameScene.gameManager.Retry();
        });
    }


    public void OnPauseButtonClick()
    {
        gameScene.gameManager.gamePlay.SetStatusPause();
        SettingsScreen screen = ScreenManager.GetScreen<SettingsScreen>();
        if (screen == null)
        {
            return;
        }
        screen.onceClose += () =>
        {
            gameScene.gameManager.gamePlay.SetStatusReady();
        };
        ScreenManager.OpenScreen(ScreenKey.SETTINGS_SCREEN);
    }

    public float GetHeight()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        return math.abs(rectTransform.anchoredPosition.y) + rectTransform.sizeDelta.y / 2;
    }

    public void SetDisableButtons(bool isDisabled)
    {
        this.pauseButton.SetDisable(isDisabled);
        this.retryButton.SetDisable(isDisabled);
    }
}