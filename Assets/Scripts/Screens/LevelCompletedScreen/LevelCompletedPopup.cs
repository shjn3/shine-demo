using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class LevelCompletedPopup : Popup
{
    public Image cupImage;
    public Text levelText;
    public ParticleSystem leftConfettiParticle;
    public ParticleSystem rightConfettiParticle;

    public CanvasGroup nextLevelButtonCanvasGroup;
    private Vector3 cupImageOriginalPosition;
    private Vector3 levelTextOriginalPosition;
    private Vector3 nextLevelButtonOriginalPosition;
    public void Awake()
    {
        cupImageOriginalPosition = cupImage.transform.localPosition;
        levelTextOriginalPosition = levelText.transform.localPosition;
        nextLevelButtonOriginalPosition = nextLevelButtonCanvasGroup.transform.localPosition;
    }

    public override void Open()
    {
        base.Open();

        levelText.text = "Level " + DataStorage.GetInt(Player.PlayerDataKey.LEVEL, 1);
        RunOpenAnimation();
        leftConfettiParticle.Play();
        rightConfettiParticle.Play();
    }

    public void RunOpenAnimation()
    {
        float duration = 0.3f;
        cupImage.color = new Color(1, 1, 1, 0);
        cupImage.DOFade(1, duration);

        cupImage.transform.localPosition = cupImageOriginalPosition - new Vector3(0, 50, 0);
        cupImage.transform.DOLocalMove(cupImageOriginalPosition, duration).SetEase(Ease.OutBack);

        levelText.color = new Color(1, 1, 1, 0);
        levelText.DOFade(1, duration).SetDelay(0.2f);
        levelText.transform.localPosition = levelTextOriginalPosition - new Vector3(0, 50, 0);
        levelText.transform.DOLocalMove(levelTextOriginalPosition, duration).SetDelay(0.2f).SetEase(Ease.OutBack);

        nextLevelButtonCanvasGroup.alpha = 0;
        nextLevelButtonCanvasGroup.DOFade(1, duration).SetDelay(0.4f);
        nextLevelButtonCanvasGroup.transform.localPosition = nextLevelButtonOriginalPosition - new Vector3(0, 50, 0);
        nextLevelButtonCanvasGroup.transform.DOLocalMove(nextLevelButtonOriginalPosition, duration).SetDelay(0.4f).SetEase(Ease.OutBack);
    }

    public void OnNextLevelButtonClick()
    {

        ScreenManager.CloseScreen(ScreenKey.LEVEL_COMPLETED_SCREEN);
        SceneTransition.Transition("GameScene");
    }
}