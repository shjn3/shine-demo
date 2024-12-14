using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Ball : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    public Tube tube;
    public int idx = 0;
    public string color = "";
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

    public void Drop(Vector3 to, float duration = 1, float delay = 0)
    {
        Vector3 position = transform.localPosition;
        Drop(transform.localPosition, to, duration, delay, () => { });
    }

    public void Drop(Vector3 from, Vector3 to, float duration, float delay, Action onCompeted = null)
    {
        Vector3 position = from;
        int bounceTime = BallConfig.BOUNCE_COUNT;
        bool isAllowBounce = true;
        float prevValue = from.y;
        bool IsBallDirectionReversed(float delta) => to.y < from.y ? delta > 0 : delta < 0;

        DOVirtual.Float(from.y, to.y, duration, (value) =>
            {
                position.y = value;
                transform.localPosition = position;
                var delta = value - prevValue;
                if (bounceTime > 0 && isAllowBounce)
                {
                    if (IsBallDirectionReversed(delta))
                    {
                        Debug.Log("Sound");
                        SoundManager.Play(SoundKey.BOUND, bounceTime * 1f / BallConfig.BOUNCE_COUNT);
                        isAllowBounce = false;
                        bounceTime -= 1;
                    }
                }
                else if (!IsBallDirectionReversed(delta)) isAllowBounce = true;
                prevValue = value;
            })
            .SetEase(Ease.OutBounce)
            .SetDelay(delay)
            .OnComplete(() =>
            {
                SoundManager.Play(SoundKey.BOUND, 0.2f);
                onCompeted?.Invoke();
            });
    }

    public void MoveTo(Vector3 to, float duration = 0.3f, float delay = 0, Action onCompleted = null
    )
    {
        transform.DOLocalMove(to, duration).SetEase(Ease.InOutSine).SetDelay(delay).OnComplete(() =>
        {
            onCompleted?.Invoke();
        });
    }
}
