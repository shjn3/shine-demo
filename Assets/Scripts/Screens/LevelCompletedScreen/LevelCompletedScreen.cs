using DG.Tweening;
using Shine.Sound;

public class LevelCompletedScreen : BaseScreen
{
    public LevelCompletedPopup popup;
    public override void OnOpen()
    {
        SoundManager.Play(SoundKey.YOU_WIN);
        popup.Open();
        UpdatePlayerData();
    }

    private void UpdatePlayerData()
    {
        DataStorage.SetInt(Player.PlayerDataKey.LEVEL, DataStorage.GetInt(Player.PlayerDataKey.LEVEL, 1) + 1);
    }

    public override void OnClose()
    {
        popup.Close();
    }
}
