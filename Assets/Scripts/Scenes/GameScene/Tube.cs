using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Shine.Sound;

public class Tube : MonoBehaviour, IPointerClickHandler
{
    private Stack<Ball> ballStack = new();
    public BoxCollider2D boxCollider2D;
    [SerializeField]
    private ParticleSystem confettiParticle;
    [HideInInspector]
    public int idx;
    [SerializeField]
    private GameObject ballPrefab;

    public event Action<Tube> onClick = (tube) => { };

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

    public void GenerateBalls(string[] colors, Transform ballsParent)
    {
        if (colors == null || colors.Length == 0)
        {
            return;
        }

        foreach (var color in colors)
        {
            GameObject ballObject = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, ballsParent);
            ballObject.GetComponent<Ball>().SetColor(color);
            PushBall(ballObject.GetComponent<Ball>());
        }
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
            ball.transform.localPosition = new Vector3(0, GetBallPositionY(i), 0) + transform.localPosition;
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

    public static Vector3 GetTopPosition(Tube tube)
    {
        return new Vector3(0, TubeConfig.HEIGHT / 2 + TubeConfig.TOP_HEIGHT + BallConfig.HEIGHT / 2, 0) + tube.transform.localPosition;
    }

    public void Select()
    {
        if (IsEmpty())
        {
            return;
        }
        Vector3 to = GetTopPosition();
        Ball[] balls = GetLastBalls();

        foreach (var ball in balls)
        {
            ball.PlayHighlightAnimation(to + transform.localPosition);
            to.y -= BallConfig.HEIGHT;
        }

        SoundManager.Play(SoundKey.HIGHLIGHT);
    }

    public void UnSelect()
    {
        if (IsEmpty())
        {
            return;
        }
        SoundManager.Play(SoundKey.UN_HIGHLIGHT);

        Ball[] balls = GetLastBalls();
        foreach (var ball in balls)
        {
            Vector3 to = new Vector3(0, GetBallPositionY(ball.idx), 0) + transform.localPosition;
            ball.PlayUnHighlightAnimation(to);
        }
    }

    public static float CalculateDuration(int idx)
    {
        return Math.Abs((GetTopPosition().y - GetBallPositionY(idx)) / (GetTopPosition().y - GetBallPositionY(0)));
    }

    public bool IsCanReceiveBalls()
    {
        return GetEmptyVolume() > 0;
    }

    public bool IsCanGiveBalls()
    {
        return this.ballStack.Count > 0;
    }

    public Ball[] GetLastBalls(int maxCount = 0)
    {
        List<Ball> balls = new();
        string color = GetLastColor();

        foreach (var ball in ballStack)
        {
            if (ball.color != color)
                break;
            balls.Add(ball);
        }

        return balls.ToArray();
    }

    public int GetLastBallCount()
    {
        int count = 0;
        string color = GetLastColor();

        foreach (var ball in ballStack)
        {
            if (ball.color != color)
                break;
            count++;
        }

        return count;
    }

    public int GetBallCount()
    {
        return this.ballStack.Count;
    }


    public Vector3 GetLastBallWorldPosition()
    {
        return transform.TransformPoint(new Vector3(0, GetBallPositionY(Math.Max(0, ballStack.Count - 1)), 0));
    }

    public string[] GetColors()
    {
        List<string> colors = new();

        foreach (var b in ballStack.Reverse())
        {
            colors.Add(b.color);
        }

        return colors.ToArray();
    }

    public void RunConfettiParticle()
    {
        this.confettiParticle.Play();
    }

    public void StopConfettiParticle()
    {
        confettiParticle.Stop();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick.Invoke(this);
    }
}
