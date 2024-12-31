using UnityEngine.UI;
using Shine.Promise;

public class AdsScreen : BaseScreen
{
    string title = "";
    public Text titleText;
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
        base.OnOpen();
        this.titleText.text = title;
    }
    protected override Promise RunFadeInBackground()
    {
        return new Promise();
    }

    protected override Promise RunFadeOutBackground()
    {
        return new Promise();
    }

    public void OnCloseButtonClick()
    {
        ScreenManager.CloseScreen(screenName);
    }

    public void PassData(string title)
    {
        this.title = title;
    }

}
