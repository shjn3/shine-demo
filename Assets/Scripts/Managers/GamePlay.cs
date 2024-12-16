using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using ShineCore;

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
                TestPlaySuggestion();
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
        if (toTube == null)
        {
            return;
        }
        //Select
        if (selectedTube == null)
        {
            if (toTube.IsCanGiveBalls() && toTube.GetLastBallCount() < levelData.bottleVolume)
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

        if (!IsSwapBalls(selectedTube, toTube))
        {
            return;
        }

        SwapBall(selectedTube, toTube, () =>
        {
            if (CheckGameState()) return;
            if (!IsCanPlayAuto())
            {
                SetStatusReady();
                selectedTube = null;
                return;
            }

            PlayAuto().Then(() =>
               {
                   CheckGameState();
               });
            return;
        });
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
        if (from.GetLastBallCount() == levelData.bottleVolume) return false;

        if (to.GetLastBallCount() == 0) return true;

        if (from.GetLastColor() != to.GetLastColor()) return false;
        if (from.GetLastBallCount() > to.GetEmptyVolume()) return false;

        return true;
    }

    private bool IsSwapBalls(int fromIdx, int toIdx)
    {
        if (!IsValidTubeIdx(fromIdx) || !IsValidTubeIdx(toIdx)) return false;
        return IsSwapBalls(this.tubes[fromIdx], this.tubes[toIdx]);
    }

    private bool CheckGameState()
    {
        if (IsWin())
        {
            HandleWin();
            return true;
        }

        if (IsLose())
        {
            HandleLose();
            return true;
        }

        return false;
    }

    bool SwapBall(int fromIdx, int toIdx, Action callback)
    {
        return SwapBall(this.tubes[fromIdx], this.tubes[toIdx], callback);
    }

    bool IsValidTubeIdx(int idx)
    {
        return idx >= 0 && idx < this.tubes.Count;
    }

    bool SwapBall(Tube from, Tube to, Action callback)
    {
        if (!IsSwapBalls(from, to)) return false;
        if (from.GetLastBallCount() == 0) return false;
        SetStatusSwapping();
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
            to.AlignBallsPosition();
            from.AlignBallsPosition();
            if (to.GetLastBallCount() == levelData.bottleVolume)
            {
                to.RunConfettiParticle();
                SoundManager.Play(SoundKey.CONFETTI);
            }

            callback.Invoke();
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

    void RunSwapBallsAnimation(BallsMove ballsMove, Action callback, float delay = 0)
    {
        Tube from = tubes[ballsMove.fromLayer];
        Tube to = tubes[ballsMove.toLayer];

        Ball[] lastBalls = to.GetLastBalls();
        Ball[] balls = new Ball[ballsMove.count];
        for (int i = ballsMove.count - 1; i >= 0; i--)
            balls[ballsMove.count - 1 - i] = lastBalls[i];

        for (int i = 0; i < ballsMove.count; i++)
        {
            var ball = balls[i];
            Vector3 fromTopPosition = -to.transform.localPosition + from.transform.localPosition + Tube.GetTopPosition();

            float duration = Vector3.Distance(fromTopPosition, to.transform.localPosition + Tube.GetTopPosition()) / 2f;
            duration *= 0.001f;

            var tempI = i;
            var promise = new Promise(resolve =>
             {
                 ball.PlayMoveAnimation(fromTopPosition, 0.1f, delay + i * 0.02f).Then(() =>
                 {
                     ball.spriteRenderer.sortingLayerName = "BallOutsideTube";
                     ball.PlayMoveAnimation(Tube.GetTopPosition(), duration).Then(() =>
                     {
                         ball.spriteRenderer.sortingLayerName = "Default";
                         ball.PlayUnHighlightAnimation(new Vector3(0, Tube.GetBallPositionY(ball.idx), 0)).Then(() =>
                         {
                             resolve();
                         });
                     });
                 });
             });
            if (i == ballsMove.count - 1)
                promise.Then(() => { callback.Invoke(); });
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
        return tubes.Find(tube => tube.IsCanGiveBalls() && tube.GetLastBallCount() != levelData.bottleVolume) == null;
    }

    public void HandleWin()
    {
        SetStatusPause();
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
        SetStatusPause();
        LevelFailedScreen screen = ScreenManager.GetScreen<LevelFailedScreen>();
        screen.onceClose += () => gameManager.gamePlay.Retry();
        ScreenManager.OpenScreen(ScreenKey.LEVEL_FAILED_SCREEN);
    }

    public void SetStatusPause()
    {
        state = GamePlayState.Pause;
    }

    public void SetStatusReady()
    {
        state = GamePlayState.Ready;
        gameManager.gameScene.SetDisableButtons(false);
    }

    private void SetStatusSwapping()
    {
        state = GamePlayState.Swapping;
        gameManager.gameScene.SetDisableButtons(true);
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

    public void TestPlaySuggestion()
    {
        GameSolution solution = GameLogic.Solve(GetGameStateData());
        if (solution.moves?.Length > 0) RecursionSwapBall(0, solution.moves);
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
            if (!CheckGameState())
            {
                SetStatusReady();
            }
            return;
        }
        SwapBall((int)moves[idx].x, (int)moves[idx].y, () =>
        {
            Sleeper.WaitForSeconds(0.5f).Then(() =>
            {
                RecursionSwapBall(idx + 1, moves);
            });
        });
    }

    private bool IsCanPlayAuto()
    {
        if (this.level < 3)
        {
            return false;
        }
        foreach (var tube in tubes)
        {
            if (tube.IsEmpty() || tube.GetLastBallCount() == tube.GetBallCount()) continue;
            return false;
        }

        return true;
    }

    private Promise PlayAuto()
    {
        List<Tube[]> sameColorTubesList = new();
        List<int> visited = new();
        for (int i = 0; i < tubes.Count; i++)
        {
            if (visited.Exists(num => num == i) || tubes[i].IsEmpty()) continue;
            List<Tube> temp = new();
            for (int j = 0; j < tubes.Count; j++)
            {
                if (i == j || tubes[i].GetLastColor() != tubes[j].GetLastColor() || tubes[j].IsEmpty()) continue;
                temp.Add(tubes[j]);
                visited.Add(j);
            }

            if (temp.Count == 0) continue;
            temp.Add(tubes[i]);
            sameColorTubesList.Add(temp.ToArray());
            visited.Add(i);
        }

        Promise[] promises = new Promise[sameColorTubesList.Count];
        for (int i = 0; i < sameColorTubesList.Count; i++)
        {
            var sameColorTubes = sameColorTubesList[i];
            var toTube = sameColorTubes[UnityEngine.Random.Range(0, sameColorTubes.Length)];
            float delay = 0;
            foreach (var tube in sameColorTubes)
            {
                if (toTube == tube || !IsSwapBalls(tube, toTube)) continue;
                promises[i] = new Promise(resolve =>
                {
                    AutoSwapBall(tube, toTube, delay).Then(() => { resolve(); });
                });
                delay += 0.1f;
            }
        };
        return Promise.All(promises);
    }

    Promise AutoSwapBall(Tube from, Tube to, float delay = 0)
    {
        string color = from.GetLastColor();
        List<Ball> balls = new();
        while (from.GetLastColor() == color)
        {
            var ball = from.PopBall();
            to.PushBall(ball);
            balls.Add(ball);
        }

        return new Promise(resolve =>
        {
            //Save move history
            BallsMove move = new()
            {
                fromLayer = from.idx,
                toLayer = to.idx,
                count = balls.Count
            };
            RunSwapBallsAnimation(move, resolve, delay);
        });
    }
}