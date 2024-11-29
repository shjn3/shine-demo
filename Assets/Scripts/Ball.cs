using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] sprites;

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
}
