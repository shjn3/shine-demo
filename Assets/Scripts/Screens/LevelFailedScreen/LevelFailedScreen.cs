using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ShineCore;
using UnityEngine;
using UnityEngine.UI;

public class LevelFailedScreen : BaseScreen
{
    public CanvasGroup popupCanvasGroup;
    public Text levelText;
    public CanvasGroup footer;
    public Image ribbon;
    public Text outOfMovesText;

    private Vector3 levelTextOriginalPosition;
    private Vector3 footerOriginalPosition;
    private Vector3 ribbonOriginalPosition;
    private Vector3 outOfMovesTextOriginalPosition;

    protected override void Awake()
    {
        base.Awake();
        levelTextOriginalPosition = levelText.transform.localPosition;
        footerOriginalPosition = footer.transform.localPosition;
        ribbonOriginalPosition = ribbon.transform.localPosition;
        outOfMovesTextOriginalPosition = outOfMovesText.transform.localPosition;
    }


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
        levelText.text = "Level " + DataStorage.GetInt(Player.PlayerDataKey.LEVEL, 1);
        RunOpenAnimation();
        SoundManager.Play(SoundKey.YOU_LOSE);
    }

    public override void OnClose()
    {
        RunHidePopupAnimation();
    }

    public void RunOpenAnimation()
    {
        popupCanvasGroup.alpha = 1;
        Color color = new Color(1, 1, 1, 0);
        Vector3 scale = new Vector3(0.85f, 0.85f, 0.85f);
        color.a = 0;
        ribbon.color = color;
        footer.alpha = 0;
        levelText.color = color;
        outOfMovesText.gameObject.SetActive(true);
        outOfMovesText.color = color;
        outOfMovesText.transform.localPosition = outOfMovesTextOriginalPosition + new Vector3(0, -150, 0);
        outOfMovesText.transform.localScale = scale;

        levelText.transform.localPosition = levelTextOriginalPosition + new Vector3(0, -150, 0);
        levelText.transform.localScale = scale;

        footer.transform.localPosition = footerOriginalPosition + new Vector3(0, -150, 0);
        footer.transform.localScale = scale;

        ribbon.transform.localPosition = ribbonOriginalPosition + new Vector3(0, -150, 0);
        ribbon.transform.localScale = scale;


        RunShowOutOfMovesTextAnimation().Then(() =>
        {
            RunHideOutOfMovesTextAnimation().Then(() =>
            {
                RunShowPopup();
            });
        });
    }

    public Promise RunShowOutOfMovesTextAnimation()
    {
        return new Promise(resolve =>
        {
            outOfMovesText.transform.DOMove(outOfMovesTextOriginalPosition, 0.5f).SetEase(Ease.OutBack);
            outOfMovesText.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
            outOfMovesText.transform.DOScale(1, 1f).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                resolve();
            });
        });
    }

    public Promise RunHideOutOfMovesTextAnimation()
    {
        return new Promise(resolve =>
        {
            outOfMovesText.color = new Color(1, 1, 1, 1);
            outOfMovesText.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                outOfMovesText.gameObject.SetActive(false);
                resolve();
            });
        });
    }

    public void RunShowPopup()
    {
        ribbon.transform.DOMove(ribbonOriginalPosition, 0.5f).SetEase(Ease.OutBack);
        ribbon.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
        ribbon.transform.DOScale(1, 1f).SetEase(Ease.OutElastic);


        Sleeper.instance.WaitForSeconds(0.1f).Then(() =>
        {
            levelText.transform.DOMove(levelTextOriginalPosition, 0.5f).SetEase(Ease.OutBack);
            levelText.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
            levelText.transform.DOScale(1, 1f).SetEase(Ease.OutElastic);
        });

        Sleeper.instance.WaitForSeconds(0.2f).Then(() =>
        {
            footer.transform.DOMove(footerOriginalPosition, 0.5f).SetEase(Ease.OutBack);
            footer.DOFade(1, 0.5f).SetEase(Ease.OutCubic);
            footer.transform.DOScale(1, 1f).SetEase(Ease.OutElastic);
        });
    }

    public void RunHidePopupAnimation()
    {
        popupCanvasGroup.alpha = 1;
        popupCanvasGroup.DOFade(0, 0.2f).SetEase(Ease.OutQuad);
    }

    public void OnRetryButtonClick()
    {
        ScreenManager.CloseScreen(screenName);
    }
}
