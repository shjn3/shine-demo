using DG.Tweening;
using UnityEngine;
using Shine.Promise;

public class MapScene : MonoBehaviour
{
    public CanvasGroup canvasGroupPopup;
    public LevelMapScroller levelMapScroller;
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        levelMapScroller.GenerateLevelMapItems();
        RunPopupEntranceAnimation();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GoToGameScene()
    {
        SceneTransition.Transition("GameScene");
    }

    public Promise RunPopupEntranceAnimation()
    {
        return new Promise((resolve) =>
        {
            float duration = 0.5f;
            canvasGroupPopup.transform.localPosition = new Vector3(0, -70, 0);
            canvasGroupPopup.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);

            canvasGroupPopup.transform.DOLocalMove(Vector3.zero, duration).SetEase(Ease.OutBack);
            canvasGroupPopup.transform.DOScale(1, duration).SetEase(Ease.OutElastic).OnComplete(() =>
            {
                resolve();
            });
        });
    }

    public void OnDestroy()
    {
        DataStorage.SetBool("history.isUse", true);
    }
}
