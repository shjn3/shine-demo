using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterButton : BaseButton
{
    public Image dotImage;
    public Image adsImage;
    public Text countText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCountText(int count)
    {
        this.countText.text = count.ToString();
    }

    public void ShowAdsIcon()
    {
        this.adsImage.gameObject.SetActive(true);
        this.dotImage.gameObject.SetActive(false);
    }

    public void ShowDotIcon()
    {
        this.adsImage.gameObject.SetActive(false);
        this.dotImage.gameObject.SetActive(true);
    }
}
