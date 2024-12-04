using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
public class SettingsPopup : Popup
{
    public CanvasGroup canvasGroup;
    public Image dashImage;
    public override void Open()
    {
        base.Open();
        SettingsSound();
        RunOpenAnimation();
    }


    public override void Close()
    {
        base.Close();
        RunCloseAnimation();
    }

    public void RunOpenAnimation()
    {
        float duration = 0.5f;
        transform.localPosition = new Vector3(0, -70, 0);
        transform.localScale = new Vector3(0.85f, 0.85f, 0.9f);
        canvasGroup.alpha = 0;

        transform.DOLocalMove(Vector3.zero, duration).SetEase(Ease.OutBack);
        canvasGroup.DOFade(1, 1).SetEase(Ease.OutBack);
        transform.DOScale(1, duration).SetEase(Ease.OutElastic);
    }

    public void RunCloseAnimation()
    {
        canvasGroup.DOFade(0, 0.2f).SetEase(Ease.OutSine);
    }


    public void OnSoundButtonClick()
    {
        DataStorage.SetBool(Player.PlayerDataKey.SOUND, !DataStorage.GetBool(Player.PlayerDataKey.SOUND));
        SettingsSound();

    }

    public void SettingsSound()
    {
        bool sound = DataStorage.GetBool(Player.PlayerDataKey.SOUND);
        this.dashImage.gameObject.SetActive(!sound);
        if (sound)
        {
            SoundManager.Continue();
        }
        else
        {

            SoundManager.Pause();
        }
    }
}