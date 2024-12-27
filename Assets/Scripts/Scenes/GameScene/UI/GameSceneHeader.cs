using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneHeader : MonoBehaviour
{
    public GameScene gameScene;
    public Text levelText;
    public Button pauseButton;
    public Button retryButton;
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
        gameScene.gameManager.Retry();
    }


    public void OnPauseButtonClick()
    {
        gameScene.gameManager.gamePlay.Pause();
        SettingsScreen screen = ScreenManager.GetScreen<SettingsScreen>();
        if (screen == null)
        {
            return;
        }
        screen.onceClose += () =>
        {
            gameScene.gameManager.gamePlay.Continue();
        };
        ScreenManager.OpenScreen(ScreenKey.SETTINGS_SCREEN);
    }

    public float GetHeight()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        return math.abs(rectTransform.anchoredPosition.y) + rectTransform.sizeDelta.y / 2;
    }
}