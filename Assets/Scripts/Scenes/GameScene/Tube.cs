using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Tube : MonoBehaviour
{
    private Stack<Ball> ballStack = new();
    public BoxCollider2D boxCollider2D;
    public bool isSelected = false;
    public int idx;
    public GameObject ballPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool PushBall(Ball ball)
    {
        if (ballStack.Count == TubeConfig.VOLUME)
        {
            return false;
        }

        ballStack.Push(ball);
        ball.SetTube(this);
        ball.transform.SetParent(transform);
        ball.idx = ballStack.Count - 1;
        return true;
    }

    public Ball PopBall()
    {
        Ball ball = ballStack.Pop();
        ball.SetTube(null);
        ball.idx = -1;
        return ball;
    }


    public void GenerateBalls(List<string> colours)
    {
        foreach (var colour in colours)
        {
            GameObject ballObject = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            ballObject.GetComponent<Ball>().SetColor(colour);
            PushBall(ballObject.GetComponent<Ball>());
        }
        AlignBallsPosition();
    }
    public string GetLastColor()
    {
        if (IsEmpty())
        {
            return "";
        }
        return ballStack.Peek().color;
    }

    public int GetEmptyVolume()
    {
        return TubeConfig.VOLUME - ballStack.Count;
    }

    public bool IsEmpty()
    {
        return ballStack.Count == 0;
    }

    public int GetMatchingBalls()
    {
        if (IsEmpty())
        {
            return 0;
        }

        int count = 0;
        string color = GetLastColor();
        foreach (Ball ball in ballStack)
        {
            if (color == ball.color)
            {
                count++;
            }
            else
            {
                break;
            }
        }

        return count;
    }

    public void AlignBallsPosition()
    {
        int i = 0;
        foreach (Ball ball in ballStack.Reverse())
        {
            ball.transform.localPosition = new Vector3(0, GetBallPositionY(i), 0);
            i++;
        }
    }

    public static float GetBallPositionY(int idx)
    {
        return -TubeConfig.HEIGHT / 2 + TubeConfig.BOTTOM_HEIGHT + BallConfig.HEIGHT * (0.5f + idx);
    }

    public static Vector3 GetTopPosition()
    {
        return new(0, TubeConfig.HEIGHT / 2 + TubeConfig.TOP_HEIGHT + BallConfig.HEIGHT / 2, 0);
    }

    public void Select()
    {
        if (IsEmpty())
        {
            return;
        }
        Vector3 to = GetTopPosition();
        Ball ball = ballStack.Peek();
        ball.MoveTo(to, 0.3f * CalculateDuration(ball.idx));
        isSelected = true;
        SoundManager.Play(SoundKey.HIGHLIGHT);
        Debug.Log("Select " + ballStack.Count);
    }

    public void UnSelect()
    {
        if (IsEmpty())
        {
            return;
        }
        SoundManager.Play(SoundKey.UN_HIGHLIGHT);
        Vector3 to = new(0, GetBallPositionY(ballStack.Count - 1), 0);
        Ball ball = ballStack.Peek();
        ball.Drop(to, CalculateDuration(ball.idx));
        isSelected = false;
        Debug.Log("Un Select " + ballStack.Count);
    }

    public static float CalculateDuration(int idx)
    {
        return Math.Abs((GetTopPosition().y - GetBallPositionY(idx)) / (GetTopPosition().y - GetBallPositionY(0)));
    }

    public bool AreAllBallsValid()
    {
        if (GetMatchingBalls() == TubeConfig.VOLUME)
        {
            return true;
        }

        return false;
    }
}
