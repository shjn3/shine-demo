using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using ShineCore;

public class Ball : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public Tube tube;
    public int idx = 0;
    public string color = "";
    public float dropDownSpeed = 0.36f;
    public float flySpeed = 1f;
    public Tween highlightTween;
    public Tween unHighlightTween;
    public Tween moveTween;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetColor(string color)
    {
        this.color = color;
        switch (color)
        {
            case BallConfig.Colours.LIME:
                spriteRenderer.sprite = sprites[0];
                break;
            case BallConfig.Colours.VIOLET:
                spriteRenderer.sprite = sprites[1];
                break;
            case BallConfig.Colours.BLUE_DARK:
                spriteRenderer.sprite = sprites[2];
                break;
            case BallConfig.Colours.BROWN:
                spriteRenderer.sprite = sprites[3];
                break;
            case BallConfig.Colours.COTTON:
                spriteRenderer.sprite = sprites[4];
                break;
            case BallConfig.Colours.CYAN:
                spriteRenderer.sprite = sprites[5];
                break;
            case BallConfig.Colours.GREEN:
                spriteRenderer.sprite = sprites[6];
                break;
            case BallConfig.Colours.OCEAN:
                spriteRenderer.sprite = sprites[7];
                break;
            case BallConfig.Colours.ORANGE:
                spriteRenderer.sprite = sprites[8];
                break;
            case BallConfig.Colours.PINK:
                spriteRenderer.sprite = sprites[9];
                break;
            case BallConfig.Colours.RED:
                spriteRenderer.sprite = sprites[10];
                break;
            case BallConfig.Colours.YELLOW:
                spriteRenderer.sprite = sprites[11];
                break;
            default:
                spriteRenderer.sprite = sprites[2];
                break;
        }
    }

    public void SetTube(Tube tube)
    {
        this.tube = tube;
    }
    private bool IsBallDirectionReversed(Vector3 fromPosition, Vector3 to, float delta)
    {
        return to.y < fromPosition.y ? delta > 0 : delta < 0;
    }

    public Promise PlayUnHighlightAnimation(Vector3 to)
    {
        var fromPosition = Tube.GetTopPosition();
        int bounceTime = BallConfig.BOUNCE_COUNT;
        bool isAllowBounce = true;
        float prevValue = fromPosition.y;

        return new Promise(resolve =>
        {
            float duration = Math.Abs((Tube.GetTopPosition().y - Tube.GetBallPositionY(idx)) / (Tube.GetTopPosition().y - Tube.GetBallPositionY(0))) * dropDownSpeed;
            if (unHighlightTween != null && DOTween.IsTweening(unHighlightTween))
                unHighlightTween.onComplete();
            unHighlightTween = DOVirtual.Float(fromPosition.y, to.y, duration, (value) =>
                              {
                                  fromPosition.y = value;
                                  transform.localPosition = fromPosition;
                                  var delta = value - prevValue;
                                  if (bounceTime > 0 && isAllowBounce)
                                  {
                                      if (IsBallDirectionReversed(fromPosition, to, delta))
                                      {
                                          SoundManager.Play(SoundKey.BOUND, bounceTime * 1f / BallConfig.BOUNCE_COUNT);
                                          isAllowBounce = false;
                                          bounceTime -= 1;
                                      }
                                  }
                                  else if (!IsBallDirectionReversed(fromPosition, to, delta)) isAllowBounce = true;
                                  prevValue = value;
                              })

                           .SetEase(Ease.OutBounce)
                           .SetDelay(idx * 0.03f)
                           .OnComplete(() =>
                           {
                               SoundManager.Play(SoundKey.BOUND, 0.2f);
                               resolve();
                           });
        });

    }

    public Promise PlayHighlightAnimation(Vector3 to, float delay = 0)
    {
        return new Promise(resolve =>
        {
            if (highlightTween != null && DOTween.IsTweening(highlightTween))
                highlightTween.onComplete();

            highlightTween = transform.DOLocalMove(to, 0.1f).SetEase(Ease.InOutSine).SetDelay(delay).OnComplete(() =>
            {
                highlightTween = null;
                resolve();
            });
        });
    }

    public Promise PlayMoveAnimation(Vector3 to, float duration = 0.3f, float delay = 0)
    {
        return new Promise(resolve =>
        {
            if (moveTween != null && DOTween.IsTweening(moveTween))
                moveTween.onComplete();
            moveTween = transform.DOLocalMove(to, duration).SetEase(Ease.Linear).SetDelay(delay).OnComplete(() => resolve());
        });
    }
}
