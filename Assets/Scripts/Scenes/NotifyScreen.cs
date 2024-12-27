using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Loading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NotifyScreen : BaseScreen
{
    public Image loadingImage;
    public Text messageText;
    string message = "";
    bool isShowLoading = false;
    float duration = 0;
    Tween loadingRotate;

    public void ShowLoading()
    {
        loadingImage.gameObject.SetActive(true);
        loadingRotate = loadingImage.transform.DOLocalRotate(new Vector3(0, 0, 360f), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetRelative().SetEase(Ease.Linear);
    }

    public void HideLoading()
    {
        loadingRotate?.Kill();
        loadingImage.gameObject.SetActive(false);
    }

    public override void OnOpen()
    {
        base.OnOpen();
        this.messageText.gameObject.SetActive(!string.IsNullOrEmpty(message));
        messageText.text = message;
        HideLoading();
        if (isShowLoading) ShowLoading();
        RunEntranceAnimation();

        if (duration >= 0)
        {
            Sleeper.instance.WaitForSeconds(duration, () =>
            {
                ScreenManager.CloseScreen(screenName);
            });
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        RunExitAnimation();
    }

    public void RunEntranceAnimation()
    {
        if (messageText.gameObject.activeSelf)
            messageText.DOFade(1, 0.3f);
        if (loadingImage.gameObject.activeSelf)
        {
            loadingImage.DOFade(1, 0.3f);
        }

    }

    public void RunExitAnimation()
    {
        if (messageText.gameObject.activeSelf)
            messageText.DOFade(0, 0.3f);
        if (loadingImage.gameObject.activeSelf)
            loadingImage.DOFade(0, 0.3f);
    }

    public void PassData(string message, float duration, bool isShowLoading)
    {
        this.message = message;
        this.duration = duration;
        this.isShowLoading = isShowLoading;
    }
}
