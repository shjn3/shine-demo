using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;

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
        Pause = 2
    }

    private float[][] delayArrArr = { new float[] { 0f},
                               new float[] { 0f, 0.077f},
                               new float[] { 0f, 0.077f, 0.22f }, };

    public GameObject tubePrefab;
    public GameObject ballPrefab;
    public GameManager gameManager;
    public List<Tube> tubes = null;

    private Tube selectedTube;
    private int level = 0;
    private LevelPlugin.LevelData levelData;

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

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                AutoPlay();
            }
        }
    }

    void Awake()
    {
        level = DataStorage.GetInt(Player.PlayerDataKey.LEVEL, 1);
        gameManager.inputManager.pointerDownLeftCallback += HandlePointerDown;
        BuildLevel(level);
    }

    void Destroy()
    {
        gameManager.inputManager.pointerDownLeftCallback -= HandlePointerDown;
    }

    void HandlePointerDown(Vector3 mousePosition)
    {
        if (state != GamePlayState.Ready) return;
        Tube toTube = tubes.Find(tube => tube.boxCollider2D.OverlapPoint(mousePosition));
        if (toTube == null) return;
        //Select
        if (selectedTube == null)
        {
            if (toTube.IsCanGiveBalls() && toTube.GetLastBallsCount() < levelData.bottleVolume)
            {
                toTube.Select();
                selectedTube = toTube;
            }
            return;
        }

        //Unselected
        if (selectedTube == toTube)
        {
            toTube.UnSelect();
            selectedTube = null;
            return;
        }

        //Check to swap
        if (!toTube.IsCanReceiveBalls()) return;

        SwapBall(selectedTube, toTube, () =>
        {
            CheckGameState();
        });


        selectedTube = null;
    }



    void BuildLevel(int level)
    {
        levelData = gameManager.levelPlugin.GetLevelData(level);
        BuildLevel(levelData);
    }

    void BuildLevel(LevelPlugin.LevelData levelData)
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

        if (!from.IsCanGiveBalls()) return false;
        if (!to.IsCanReceiveBalls()) return false;
        if (from.GetLastBallsCount() == levelData.bottleVolume) return false;

        if (to.GetLastBallsCount() == 0) return true;

        if (from.GetLastColor() != to.GetLastColor()) return false;
        if (from.GetLastBallsCount() > to.GetEmptyVolume()) return false;

        return true;
    }

    private bool IsSwapBalls(int fromIdx, int toIdx)
    {
        if (!IsValidTubeIdx(fromIdx) || !IsValidTubeIdx(toIdx)) return false;
        return IsSwapBalls(this.tubes[fromIdx], this.tubes[toIdx]);
    }

    private void CheckGameState()
    {
        if (IsWin())
        {
            HandleWin();
            return;
        }

        if (IsLose())
        {
            HandleLose();
            return;
        }
    }

    bool SwapBall(int fromIdx, int toIdx, Action callback)
    {
        if (!IsSwapBalls(fromIdx, toIdx)) return false;
        return SwapBall(this.tubes[fromIdx], this.tubes[toIdx], callback);
    }

    bool IsValidTubeIdx(int idx)
    {
        return idx >= 0 && idx < this.tubes.Count;
    }

    bool SwapBall(Tube from, Tube to, Action callback)
    {
        if (!IsSwapBalls(from, to)) return false;
        if (from.GetLastBallsCount() == 0) return false;

        string color = from.GetLastColor();

        List<Ball> balls = new();
        while (from.GetLastColor() == color)
        {
            var ball = from.PopBall();
            to.PushBall(ball);
            balls.Add(ball);
        }

        Action completedSwapBall = () =>
        {
            callback.Invoke();
            to.AlignBallsPosition();
            from.AlignBallsPosition();
            if (to.GetLastBallsCount() == levelData.bottleVolume) SoundManager.Play(SoundKey.CONFETTI);
        };

        //Save move history
        BallsMove move = new()
        {
            fromLayer = from.idx,
            toLayer = to.idx,
            count = balls.Count
        };
        moveStack.Push(move);

        RunSwapBallsAnimation(move, completedSwapBall);
        return true;
    }

    void RunSwapBallsAnimation(BallsMove ballsMove, Action callback)
    {
        state = GamePlayState.Swapping;
        Tube from = tubes[ballsMove.fromLayer];
        Tube to = tubes[ballsMove.toLayer];
        Ball[] lastBalls = to.GetLastBalls();
        Ball[] balls = new Ball[ballsMove.count];
        for (int i = ballsMove.count - 1; i >= 0; i--)
        {
            balls[ballsMove.count - 1 - i] = lastBalls[i];
        }

        int firstToIdx = Math.Max(0, TubeConfig.VOLUME - (to.GetEmptyVolume() + ballsMove.count));
        int firstFromIdx = Math.Max(0, TubeConfig.VOLUME - (from.GetEmptyVolume() - ballsMove.count));

        float maxDuration = 0.3f;

        float[] delayArr = delayArrArr[ballsMove.count - 1];
        Action onDropCompleted = () =>
        {
            state = GamePlayState.Ready;
            callback.Invoke();
        };

        for (int i = 0; i < ballsMove.count; i++)
        {
            var ball = balls[i];
            Vector3 fromTopPosition = -to.transform.localPosition + from.transform.localPosition + Tube.GetTopPosition();

            float duration = maxDuration * Math.Abs((fromTopPosition.y - ball.transform.localPosition.y) / (Tube.GetTopPosition().y - Tube.GetBallPositionY(0)));

            Vector3 toTopPosition = Tube.GetTopPosition();
            Vector3 endPosition = new Vector3(0, Tube.GetBallPositionY(firstToIdx + i), 0);

            float delay = Mathf.Max(0, delayArr[i] - delayArr[i] * duration / maxDuration);
            ball.MoveTo(fromTopPosition, duration, 0, () =>
            {
                ball.spriteRenderer.sortingLayerName = "BallOutsideTube";
            });

            ball.MoveTo(toTopPosition, maxDuration, duration, () =>
            { ball.spriteRenderer.sortingLayerName = "Default"; });

            ball.Drop(toTopPosition, endPosition, Tube.CalculateDuration(firstToIdx + i), maxDuration + duration, i == ballsMove.count - 1 ? onDropCompleted : null);
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

    private bool IsWin()
    {
        return tubes.Find(tube => tube.IsCanGiveBalls() && tube.GetLastBallsCount() != levelData.bottleVolume) == null;
    }

    public void HandleWin()
    {
        Pause();
        LevelCompletedScreen screen = ScreenManager.GetScreen<LevelCompletedScreen>();
        screen.onceClose += () => { gameManager.gamePlay.NextLevel(); };
        ScreenManager.OpenScreen(ScreenKey.LEVEL_COMPLETED_SCREEN);
    }

    private bool IsLose()
    {
        for (int i = 0; i < tubes.Count; i++)
        {
            for (int j = 0; j < tubes.Count; j++)
            {
                if (i == j) continue;
                if (IsSwapBalls(i, j)) return false;
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

        for (int i = 0; i < move.count; i++)
            tubes[move.toLayer].PushBall(tubes[move.fromLayer].PopBall());


        RunSwapBallsAnimation(move, () => { });
    }

    public void AutoPlay()
    {
        GameSolution solution = GameLogic.Solve(GetGameStateData());
        Debug.Log("moves Count: " + solution.moves?.Length);
        if (solution.moves?.Length > 0)
        {
            // string debug = "move:\n";
            // foreach (var m in solution.moves)
            // {
            //     debug += m.x + "|" + m.y + "\n";

            // }
            // Debug.Log(debug);
            RecursionSwapBall(0, solution.moves);
        }
    }

    public GameStateData GetGameStateData()
    {
        string[][] bottleList = new string[tubes.Count][];
        for (int i = 0; i < tubes.Count; i++)
            bottleList[i] = tubes[i].GetColors();


        GameStateData data = new()
        {
            coloredBottleCount = levelData.coloredBottleCount,
            emptyBottleCount = levelData.emptyBottleCount,
            bottleVolume = levelData.bottleVolume,
            bottleList = bottleList
        };


        return data;

    }

    private void RecursionSwapBall(int idx, Vector2[] moves)
    {
        if (idx >= moves.Length || idx < 0)
        {
            CheckGameState();
            this.state = GamePlayState.Ready;
            return;
        }

        SwapBall((int)moves[idx].x, (int)moves[idx].y, () =>
        {
            this.state = GamePlayState.Swapping;
            Sleeper.WaitForSeconds(0.5f).Then(() =>
            {
                RecursionSwapBall(idx + 1, moves);
            });
        });
    }
}