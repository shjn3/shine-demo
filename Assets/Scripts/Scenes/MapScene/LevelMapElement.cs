using UnityEngine;
using UnityEngine.UI;
using Shine;
public class LevelMapScrollerElementData
{
    public int level;
}

public class LevelMapElement : MonoBehaviour
{
    public Image background;
    public Sprite[] backgroundSprites;
    public Image checkIcon;
    public Image lockIcon;
    public Text levelText;
    public Button button;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateStatus(LevelMapScrollerElementData data)
    {
        int currentLevel = DataStorage.GetInt(Player.PlayerDataKey.LEVEL, 1);
        levelText.text = data.level.ToString();

        if (currentLevel == data.level)
        {
            SetStatusReady();
        }
        else
        {
            if (currentLevel > data.level)
            {
                SetStatusCompleted();
            }
            else
            {
                SetStatusLocked();
            }
        }
        //  
    }

    public void SetStatusReady()
    {
        levelText.color = Color.white;
        checkIcon.gameObject.SetActive(false);
        background.sprite = backgroundSprites[1];
        lockIcon.gameObject.SetActive(false);
        button.interactable = true;
    }

    public void SetStatusLocked()
    {
        levelText.color = Color.white;
        checkIcon.gameObject.SetActive(false);
        background.sprite = backgroundSprites[1];
        lockIcon.gameObject.SetActive(true);
        button.interactable = false;
    }

    public void SetStatusCompleted()
    {
        levelText.color = Color.black;
        checkIcon.gameObject.SetActive(true);
        background.sprite = backgroundSprites[0];
        lockIcon.gameObject.SetActive(false);
        button.interactable = false;
    }

    public void OnButtonClick()
    {
        SceneTransition.Transition("GameScene");
    }
}
