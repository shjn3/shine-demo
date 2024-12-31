using System;
using UnityEngine;
using Shine.Ads;
using Shine.Utils;

public class GameSceneFooter : MonoBehaviour
{
    public GameScene gameScene;
    public FooterButton addTubeButton;
    public FooterButton undoButton;
    public FooterButton hintButton;

    void Awake()
    {
        DateTime hintCountUpdateTime = DataStorage.GetDateTime(Player.PlayerDataKey.HINT_COUNT_UPDATE_AT);
        DateTime undoCountUpdateTime = DataStorage.GetDateTime(Player.PlayerDataKey.UNDO_COUNT_UPDATE_AT);

        if (!DateTimeUtils.IsToday(hintCountUpdateTime))
        {
            // DataStorage.SetInt(Player.PlayerDataKey.HINT_COUNT, Core.instance.configs.hintCount);
            DataStorage.SetDateTime(Player.PlayerDataKey.HINT_COUNT_UPDATE_AT, DateTime.Now);
            DataStorage.SetInt(Player.PlayerDataKey.HINT_COUNT, 5);

        }

        if (!DateTimeUtils.IsToday(undoCountUpdateTime))
        {
            // DataStorage.SetInt(Player.PlayerDataKey.UNDO_COUNT, Core.instance.configs.undoCount);
            DataStorage.SetDateTime(Player.PlayerDataKey.UNDO_COUNT_UPDATE_AT, DateTime.Now);
            DataStorage.SetInt(Player.PlayerDataKey.UNDO_COUNT, 5);

        }
        var position = transform.localPosition;
        position.y += GameSceneConfig.GetBannerHeight();
        transform.localPosition = position;
        int undoCount = DataStorage.GetInt(Player.PlayerDataKey.UNDO_COUNT, 0);
        int hintCount = DataStorage.GetInt(Player.PlayerDataKey.HINT_COUNT, 0);

        if (undoCount == 0)
        {
            undoButton.ShowAdsIcon();
        }
        else
        {
            undoButton.ShowDotIcon();
        }

        if (hintCount == 0)
        {
            hintButton.ShowAdsIcon();
        }
        else
        {
            hintButton.ShowDotIcon();
        }

        hintButton.SetCountText(hintCount);
        undoButton.SetCountText(undoCount);

    }
    void Start()
    {

    }

    void Update()
    {
        //
    }


    public void OnUndoButtonClick()
    {
        if (DataStorage.GetInt(Player.PlayerDataKey.UNDO_COUNT, 0) == 0)
        {
            AdsManager.ShowRewardedAd((bool isRewarded) =>
            {
                if (!isRewarded)
                {
                    return;
                }
                DataStorage.SetInt(Player.PlayerDataKey.UNDO_COUNT, 1);
                this.undoButton.ShowDotIcon();
                this.undoButton.SetCountText(1);
            });
        }
        if (!gameScene.gameManager.gamePlay.IsCanUndo())
        {
            return;
        }

        gameScene.gameManager.gamePlay.Undo();
        int count = Math.Max(0, DataStorage.GetInt(Player.PlayerDataKey.UNDO_COUNT, 0) - 1);
        DataStorage.SetInt(Player.PlayerDataKey.UNDO_COUNT, count);
        if (count == 0)
        {
            this.undoButton.ShowAdsIcon();
        }
        this.undoButton.SetCountText(count);
    }


    public void OnAddTubeButtonClick()
    {
        this.addTubeButton.SetDisable(true);
        AdsManager.ShowRewardedAd((isRewarded) =>
        {
            this.addTubeButton.SetDisable(false);
            if (isRewarded)
            {
                this.gameScene.gameManager.gamePlay.AddTube();
            }
        });
    }

    public void OnHintButtonClick()
    {
        this.hintButton.SetDisable(true);
        if (DataStorage.GetInt(Player.PlayerDataKey.HINT_COUNT, 0) == 0)
        {
            AdsManager.ShowRewardedAd((bool isRewarded) =>
            {
                if (!isRewarded) return;
                DataStorage.SetInt(Player.PlayerDataKey.HINT_COUNT, 1);
                hintButton.ShowDotIcon();
                hintButton.SetCountText(1);
            });
        }
        Action onCompleted = () =>
        {
            hintButton.SetDisable(false);
        };

        Action<bool> onStarted = (bool isCanShow) =>
        {
            if (!isCanShow)
                return;

            int count = DataStorage.GetInt(Player.PlayerDataKey.HINT_COUNT, 0);
            DataStorage.SetInt(Player.PlayerDataKey.HINT_COUNT, Math.Max(0, count - 1));

            if (count == 1)
                hintButton.ShowAdsIcon();
            else
                hintButton.SetCountText(count - 1);

        };

        gameScene.gameManager.ShowHint(onCompleted, onStarted);
    }

    public float GetHeight()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        return rectTransform.anchoredPosition.y + rectTransform.sizeDelta.y / 2;
    }

    public void SetDisableButtons(bool isDisabled)
    {
        this.hintButton.SetDisable(isDisabled);
        this.undoButton.SetDisable(isDisabled);
        this.addTubeButton.SetDisable(isDisabled);
    }
}