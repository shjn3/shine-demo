using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ShineCore;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    public Image handImage;
    // Start is called before the first frame update
    void Awake()
    {
        handImage.rectTransform.pivot = handImage.sprite.pivot.normalized;
    }

    Sequence sequence;
    public Promise Show(Suggestion suggestion)
    {
        if (handImage == null)
        {
            Debug.Log("handImage null");
            return new Promise();
        }


        sequence?.Complete();
        handImage.gameObject.SetActive(true);
        return new Promise(resolve =>
        {
            Color color = handImage.color;
            color.a = 0;
            Vector3 downScale = new Vector3(0.7f, 0.7f, 0.7f);
            Vector3 upScale = new Vector3(1f, 1f, 1f);

            handImage.color = color;
            handImage.transform.position = suggestion.fromWorldPosition;
            handImage.transform.localScale = upScale;

            // fade in, scale down
            sequence = DOTween.Sequence();
            sequence.Append(handImage.DOFade(1, 0.4f).SetEase(Ease.OutCubic));
            sequence.Join(handImage.transform.DOScale(downScale, 0.4f));

            //move and scale up
            sequence.Append(handImage.transform.DOMove(suggestion.toWorldPosition, 0.4f).SetEase(Ease.OutCubic).SetDelay(0.2f));
            sequence.Join(handImage.transform.DOScale(upScale, 0.4f).SetEase(Ease.OutCubic).SetDelay(0.2f));

            //scale down
            sequence.Append(handImage.transform.DOScale(downScale, 0.4f).SetEase(Ease.OutCubic).SetDelay(0.2f));

            //Fade out + scale up
            sequence.Append(handImage.DOFade(0, 0.2f).SetEase(Ease.OutCubic).SetDelay(0.2f));
            sequence.Join(handImage.transform.DOScale(upScale, 0.2f).SetEase(Ease.OutCubic).SetDelay(0.2f));
            sequence.Append(DOVirtual.Float(0, 1, 0.2f, (v) => { }));
            sequence.SetLoops(2, LoopType.Restart);


            sequence.OnComplete(() =>
            {
                this.handImage.gameObject.SetActive(false);
                handImage.color = color;
                handImage.transform.position = suggestion.fromWorldPosition;
                handImage.transform.localScale = upScale;
                resolve();
            });
        });
    }

    public void Hide()
    {
        sequence?.Kill();
        handImage.gameObject.SetActive(false);
    }
}
