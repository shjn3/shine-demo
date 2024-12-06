using UnityEngine;

public class GameSceneFooter : MonoBehaviour
{
    public GameScene gameScene;
    public BaseButton addTubeButton;
    public BaseButton backButton;
    public BaseButton hintButton;

    void Awake()
    {
        //
    }
    void Start()
    {
        //
    }

    void Update()
    {
        //
    }


    public void OnBackButtonClick()
    {
        // gameManager.Retry();
    }


    public void OnAddTubeButtonClick()
    {
        // gameManager.Retry();
    }

    public void OnHintButtonClick()
    {
        // gameManager.Retry();
    }

    public float GetHeight()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        return rectTransform.anchoredPosition.y + rectTransform.sizeDelta.y / 2;
    }
}