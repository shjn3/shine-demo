using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using System;
using Unity.VisualScripting;
using DG.Tweening;

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
    public List<List<Tube>> tubeGrid = null;

    private Tube fromTube;
    private int level = 0;

    private GamePlayState state;

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
        if (tubeGrid != null)
        {
            foreach (List<Tube> tubes in tubeGrid)
            {
                foreach (Tube tube in tubes)
                {
                    Destroy(tube.gameObject);
                }
            }
            tubeGrid = null;
        }

        //Generate new tubes
        int bottleCount = levelData.bottleList.Count + 1;
        bool stagger = bottleCount % 2 == 0;
        int maxTubeInRow = Math.Max(1, Mathf.RoundToInt(bottleCount / GamePlayConfig.MAX_ROW));
        tubeGrid = new();
        float gapVertical = 50f;
        float x = 0;
        float y = (TubeConfig.HEIGHT + gapVertical) / 2;
        float remainingHorizontalSpace = Math.Max(0, GameSceneConfig.WIDTH - maxTubeInRow * TubeConfig.WIDTH);
        float gapHorizontal = remainingHorizontalSpace / (maxTubeInRow + 1);

        int bottleListIdx = 0;
        for (int row = 0; row < GamePlayConfig.MAX_ROW; row++)
        {
            var tubeList = new List<Tube>();
            int tubeInRow = stagger && row % 2 != 0 ? maxTubeInRow - 1 : maxTubeInRow;

            float marginLeft = stagger && row % 2 != 0 ? Math.Max(0, GameSceneConfig.WIDTH - (tubeInRow * TubeConfig.WIDTH + gapHorizontal * (tubeInRow - 1))) / 2f : gapHorizontal;

            x = -GameSceneConfig.WIDTH / 2 + marginLeft + TubeConfig.WIDTH / 2;
            for (int column = 0; column < tubeInRow; column++)
            {

                GameObject tubeObject = Instantiate(tubePrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
                Tube tube = tubeObject.GetComponent<Tube>();
                tubeList.Add(tube);
                tubeObject.transform.localPosition = new Vector3(x, y, 0);
                x += TubeConfig.WIDTH + gapHorizontal;

                List<string> colours = levelData.bottleList[bottleListIdx];
                bottleListIdx += 1;
                foreach (var colour in colours)
                {
                    GameObject ballObject = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
                    ballObject.GetComponent<Ball>().SetColor(colour);
                    tube.PushBall(ballObject.GetComponent<Ball>());
                }
                tube.AlignBallsPosition();
            }
            tubeGrid.Add(tubeList);
            y -= TubeConfig.HEIGHT + gapVertical;
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

    public bool SwapBalls(Tube from, Tube to, Action callback)
    {
        if (!IsSwapBalls(from, to))
        {
            return false;
        }

        state = GamePlayState.Swapping;
        string color = from.GetLastColor();
        List<Ball> balls = new();
        int firstToIdx = Math.Max(0, TubeConfig.VOLUME - to.GetEmptyVolume());
        int firstFromIdx = Math.Max(0, TubeConfig.VOLUME - from.GetEmptyVolume());
        Debug.Log("first_position: " + to.GetEmptyVolume() + " " + firstToIdx + " " + firstFromIdx);
        while (from.GetLastColor() == color)
        {
            var ball = from.PopBall();
            balls.Add(ball);
            to.PushBall(ball);
        }

        Action completedSwap = () =>
        {
            state = GamePlayState.Ready;
            to.AlignBallsPosition();
            from.AlignBallsPosition();
            callback.Invoke();
        };

        if (balls.Count == 0)
        {
            completedSwap.Invoke();
            return false;
        }

        int countBall = 0;
        float maxDuration = 0.3f;


        Debug.Log("from" + from.transform.localPosition + "  " + to.transform.localPosition + " " + (to.transform.localPosition - from.transform.localPosition) + " " + (to.transform.localPosition + from.transform.localPosition));
        for (int i = 0; i < balls.Count; i++)
        {
            var ball = balls[i];
            Vector3 fromTopPosition = -to.transform.localPosition + from.transform.localPosition + Tube.GetTopPosition();

            float duration = 0.3f * ((fromTopPosition.y - ball.transform.localPosition.y) / (fromTopPosition.y - Tube.GetBallPositionY(0)));

            Vector3 toTopPosition = Tube.GetTopPosition();
            Vector3 endPosition = new Vector3(0, Tube.GetBallPositionY(firstToIdx + i), 0);

            ball.MoveTo(fromTopPosition, duration).OnComplete(() =>
            {
                ball.MoveTo(toTopPosition, maxDuration);
            });

            ball.Drop(endPosition, Tube.CalculateDuration(firstToIdx + i), maxDuration + duration)
                  .OnComplete(() =>
                  {
                      SoundManager.Play(SoundKey.BOUND);
                      if (countBall == balls.Count - 1)
                      {
                          completedSwap.Invoke();
                      }
                      countBall++;
                  });
        }
        return true;
    }

    public void HandlePointerDown(Vector3 mousePosition)
    {
        if (state != GamePlayState.Ready)
        {
            return;
        }

        Tube selectedTube = null;
        foreach (var tubeList in tubeGrid)
        {
            foreach (var tube in tubeList)
            {
                if (tube.boxCollider2D.OverlapPoint(mousePosition))
                {
                    selectedTube = tube;
                    break;
                };
            }
            if (selectedTube != null)
            {
                break;
            }
        }

        if (selectedTube == null)
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

        }
        else
        {
            if (SwapBalls(fromTube, selectedTube, () =>
            {
                if (IsWin())
                {
                    HandleWin();
                }
                else
                {
                    if (IsLose())
                    {
                        HandleLose();
                    }
                }
            }))
            {
                Debug.Log("Successfully");
            }
            else
            {
                Debug.Log("UnSuccessfully");
            }
        }


        fromTube = null;
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
        foreach (var tubeList in tubeGrid)
        {
            foreach (var tube in tubeList)
            {
                if (!tube.AreAllBallsValid())
                {
                    return false;
                }
            }

        }
        return true;
    }

    public void HandleWin()
    {
        Pause();

        LevelCompletedScreen screen = ScreenManager.GetScreen<LevelCompletedScreen>();
        screen.onceClose += () =>
        {
            gameManager.gamePlay.NextLevel();
        };
        ScreenManager.OpenScreen(ScreenKey.LEVEL_COMPLETED_SCREEN);
    }

    public bool IsLose()
    {
        List<Tube> remainTubes = new();
        foreach (var tubeList in tubeGrid)
        {
            foreach (var tube in tubeList)
            {
                if (tube.IsEmpty())
                {
                    return false;
                }
                if (!tube.AreAllBallsValid())
                {
                    remainTubes.Add(tube);
                }
            }
        }

        if (remainTubes.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < remainTubes.Count - 1; i++)
        {
            string fromLastColor = remainTubes[i].GetLastColor();
            int fromEmptyVolume = remainTubes[i].GetEmptyVolume();
            int fromMatchingBalls = remainTubes[i].GetMatchingBalls();

            for (int j = i + 1; j < remainTubes.Count; j++)
            {
                if (fromLastColor == remainTubes[j].GetLastColor())
                {
                    int toEmptyVolume = remainTubes[j].GetEmptyVolume();
                    int toMatchingBalls = remainTubes[j].GetMatchingBalls();
                    if (fromEmptyVolume >= toMatchingBalls || fromMatchingBalls <= toEmptyVolume)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void HandleLose()
    {
        Pause();
        LevelFailedScreen screen = ScreenManager.GetScreen<LevelFailedScreen>();
        screen.onceClose += () =>
        {
            gameManager.gamePlay.Retry();
        };
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

}