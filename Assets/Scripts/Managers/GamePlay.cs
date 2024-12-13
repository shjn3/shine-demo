using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Linq;

public struct BallsMove
{

    public int fromLayer;
    public int toLayer;
    public int count;

    public BallsMove Reverse()
    {
        return new BallsMove()
        {
            fromLayer = toLayer,
            toLayer = fromLayer,
            count = count
        };
    }
}

public class GamePlay : MonoBehaviour
{
    public enum GamePlayState
    {
        Ready = 0,
        Swapping = 1,
        Pause = 2,
    }
    public GameObject tubePrefab;
    public GameObject ballPrefab;
    public GameManager gameManager;
    public List<Tube> tubes = null;

    private Tube fromTube;
    private int level = 0;

    private GamePlayState state;

    private float ratioScale = 1f;
    public Stack<BallsMove> moveStack = new();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DataStorage.SetInt(Player.PlayerDataKey.LEVEL, 1);
            Retry();
        }
    }

    void Awake()
    {
        level = DataStorage.GetInt(Player.PlayerDataKey.LEVEL, 1);
        gameManager.inputManager.pointerDownLeftCallback += HandlePointerDown;
        Generate(level);
    }

    void Destroy()
    {
        gameManager.inputManager.pointerDownLeftCallback -= HandlePointerDown;
    }



    void Generate(int level)
    {
        gameManager.levelPlugin.LoadLevel(level);

        Generate(gameManager.levelPlugin.GetLevelData(level));
    }

    void Generate(LevelPlugin.LevelData levelData)
    {
        //Destroy old objects
        if (tubes != null)
        {

            foreach (Tube tube in tubes)
            {
                Destroy(tube.gameObject);
            }

            tubes = null;
        }
        tubes = new();
        //Generate new tubes
        for (int i = 0; i < levelData.bottleList.Count; i++)
        {
            Tube tube = Instantiate(tubePrefab, Vector3.zero, Quaternion.identity, gameObject.transform).GetComponent<Tube>();
            tube.GenerateBalls(levelData.bottleList[i]);
            tube.idx = i;
            tubes.Add(tube);
        }
        AlignTubes();
    }

    private void AlignTubes()
    {
        float padding = 0;
        float maxColumn = Mathf.CeilToInt(tubes.Count / 2f);

        float width = TubeConfig.WIDTH * ratioScale;
        if (tubes.Count > 1)
        {
            padding = (GameSceneConfig.WIDTH - maxColumn * width) / (maxColumn + 1);
        }

        float xTop = -GameSceneConfig.WIDTH / 2 + width / 2 + padding;
        float xBottom = tubes.Count % 2 == 0 ? xTop : -GameSceneConfig.WIDTH / 2 + width + padding * 1.5f;

        for (int i = 0; i < tubes.Count; i += 2)
        {
            tubes[i].transform.localPosition = new Vector3(xTop, GamePlayConfig.TUBE_GAP_VERTICAL, 0);
            if (i + 1 < tubes.Count)
                tubes[i + 1].transform.localPosition = new Vector3(xBottom, -GamePlayConfig.TUBE_GAP_VERTICAL, 0);

            xTop += padding + width;
            xBottom += padding + width;
        }
    }

    private bool IsSwapBalls(Tube from, Tube to)
    {
        if (from.IsEmpty())
        {
            return false;
        }

        if (to.IsEmpty())
        {
            return true;
        }
        int emptyVolume = to.GetEmptyVolume();
        int ballVolume = from.GetMatchingBalls();

        return from.GetLastColor() == to.GetLastColor() && emptyVolume >= ballVolume;
    }
    public void HandlePointerDown(Vector3 mousePosition)
    {
        if (state != GamePlayState.Ready) return;
        Tube selectedTube = tubes.Find(tube => tube.boxCollider2D.OverlapPoint(mousePosition));

        if (selectedTube == null) return;
        if (selectedTube.AreAllBallsValid())
        {
            return;
        }
        if (fromTube == null)
        {
            if (!selectedTube.IsEmpty())
            {
                selectedTube.Select();
                fromTube = selectedTube;
            }
            return;
        }

        if (fromTube == selectedTube)
        {
            selectedTube.UnSelect();
            fromTube = null;
            return;
        }

        Action swapBallCallback = () =>
        {
            if (IsWin()) HandleWin();
            else if (IsLose()) HandleLose();

            fromTube = null;
        };


        if (SwapBall(fromTube, selectedTube, swapBallCallback))
            Debug.Log("Successfully");
        else
            Debug.Log("UnSuccessfully");
    }

    public float[][] delayArrArr = { new float[] { 0f},
                               new float[] { 0f, 0.077f},
                               new float[] { 0f, 0.077f, 0.22f }, };


    public bool SwapBall(Tube from, Tube to, Action callback)
    {
        if (!IsSwapBalls(from, to))
        {
            return false;
        }
        string color = from.GetLastColor();
        List<Ball> balls = new();
        while (from.GetLastColor() == color)
        {
            var ball = from.PopBall();
            to.PushBall(ball);
            balls.Add(ball);
        }

        Action completedSwap = () =>
        {
            to.AlignBallsPosition();
            from.AlignBallsPosition();
            callback.Invoke();
            if (to.AreAllBallsValid())
            {
                SoundManager.Play(SoundKey.CONFETTI);
            }
        };

        if (balls.Count == 0)
        {
            completedSwap.Invoke();
            return false;
        }
        BallsMove move = new()
        {
            fromLayer = from.idx,
            toLayer = to.idx,
            count = balls.Count
        };
        moveStack.Push(move);
        RunSwapBallsAnimation(move, completedSwap);

        return true;
    }


    void RunSwapBallsAnimation(BallsMove ballsMove, Action callback)
    {

        state = GamePlayState.Swapping;
        Tube from = tubes[ballsMove.fromLayer];
        Tube to = tubes[ballsMove.toLayer];
        Ball[] lastBalls = to.getLastBalls();
        Ball[] balls = new Ball[ballsMove.count];
        for (int i = ballsMove.count - 1; i >= 0; i--)
        {
            balls[ballsMove.count - 1 - i] = lastBalls[i];
        }

        int firstToIdx = Math.Max(0, TubeConfig.VOLUME - (to.GetEmptyVolume() + ballsMove.count));
        int firstFromIdx = Math.Max(0, TubeConfig.VOLUME - (from.GetEmptyVolume() - ballsMove.count));
        int countBall = 0;
        float maxDuration = 0.3f;

        float[] delayArr = delayArrArr[ballsMove.count - 1];

        for (int i = 0; i < ballsMove.count; i++)
        {
            var ball = balls[i];
            Vector3 fromTopPosition = -to.transform.localPosition + from.transform.localPosition + Tube.GetTopPosition();

            float duration = maxDuration * Math.Abs((fromTopPosition.y - ball.transform.localPosition.y) / (Tube.GetTopPosition().y - Tube.GetBallPositionY(0)));

            Vector3 toTopPosition = Tube.GetTopPosition();
            Vector3 endPosition = new Vector3(0, Tube.GetBallPositionY(firstToIdx + i), 0);
            Action onDropCompleted = () =>
            {
                if (countBall == ballsMove.count - 1)
                {
                    state = GamePlayState.Ready;
                    callback.Invoke();
                }
                countBall++;
            };
            float delay = Mathf.Max(0, delayArr[i] - delayArr[i] * duration / maxDuration);
            ball.MoveTo(fromTopPosition, duration, 0, () =>
            {
                ball.spriteRenderer.sortingLayerName = "BallOutsideTube";
            });

            ball.MoveTo(toTopPosition, maxDuration, duration, () =>
            { ball.spriteRenderer.sortingLayerName = "Default"; });

            ball.Drop(toTopPosition, endPosition, Tube.CalculateDuration(firstToIdx + i), maxDuration + duration, onDropCompleted);
        }
    }

    public void Retry()
    {
        Debug.Log("RETRY");
        SceneTransition.Transition("GameScene", 1);
    }

    public void NextLevel()
    {
        Debug.Log("Next Level");
        SceneTransition.Transition("GameScene", 1);
    }

    public bool IsWin()
    {
        return tubes.Find(tube => !tube.AreAllBallsValid() && !tube.IsEmpty()) == null;
    }

    public void HandleWin()
    {
        Pause();
        LevelCompletedScreen screen = ScreenManager.GetScreen<LevelCompletedScreen>();
        screen.onceClose += () => { gameManager.gamePlay.NextLevel(); };
        ScreenManager.OpenScreen(ScreenKey.LEVEL_COMPLETED_SCREEN);
    }

    public bool IsLose()
    {
        List<Tube> remainTubes = new();

        foreach (var tube in tubes)
        {
            if (tube.IsEmpty()) return false;
            if (!tube.AreAllBallsValid() && !tube.IsEmpty())
                remainTubes.Add(tube);
        }


        if (remainTubes.Count == 0) return false;

        for (int i = 0; i < remainTubes.Count - 1; i++)
        {
            string fromLastColor = remainTubes[i].GetLastColor();
            int fromEmptyVolume = remainTubes[i].GetEmptyVolume();
            int fromMatchingBalls = remainTubes[i].GetMatchingBalls();

            for (int j = i + 1; j < remainTubes.Count; j++)
            {
                if (fromLastColor != remainTubes[j].GetLastColor())
                {
                    continue;
                }

                if (fromEmptyVolume >= remainTubes[j].GetMatchingBalls() || fromMatchingBalls <= remainTubes[j].GetEmptyVolume())
                    return false;

            }
        }

        return true;
    }

    public void HandleLose()
    {
        Pause();
        LevelFailedScreen screen = ScreenManager.GetScreen<LevelFailedScreen>();
        screen.onceClose += () => gameManager.gamePlay.Retry();

        ScreenManager.OpenScreen(ScreenKey.LEVEL_FAILED_SCREEN);
    }

    public void Pause()
    {
        state = GamePlayState.Pause;
    }

    public void Continue()
    {
        state = GamePlayState.Ready;
    }

    public void UpdateRatio(float ratioScale = 1f)
    {
        this.ratioScale = ratioScale;
        gameObject.transform.localScale = new Vector3(ratioScale, ratioScale, ratioScale);
        AlignTubes();
    }

    public void AddTube()
    {
        Tube tube = Instantiate(tubePrefab, Vector3.zero, Quaternion.identity, gameObject.transform).GetComponent<Tube>();
        tube.idx = tubes.Count;

        tubes.Add(tube);
        AlignTubes();
    }

    public bool IsCanUndo()
    {
        return moveStack.Count != 0 && state == GamePlayState.Ready;
    }

    public void Undo()
    {
        if (!IsCanUndo())
        {
            Debug.Log("can't undo");
            return;
        }
        BallsMove move = moveStack.Pop().Reverse();
        Debug.Log("undo: " + moveStack.Count);

        Tube from = tubes[move.fromLayer];
        Tube to = tubes[move.toLayer];

        for (int i = 0; i < move.count; i++)
        {
            to.PushBall(from.PopBall());
        }

        RunSwapBallsAnimation(move, () =>
        {

        });
    }
}