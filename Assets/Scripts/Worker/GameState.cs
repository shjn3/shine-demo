using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameStateData
{
    public GameStateData Clone()
    {
        string[][] newBottleList = new string[bottleList.Length][];

        for (int i = 0; i < bottleList.Length; i++)
        {
            if (bottleList[i] == null)
            {
                continue;
            }
            string[] colors = new string[bottleList[i].Length];
            bottleList[i].CopyTo(colors, 0);
            newBottleList[i] = colors;
        }

        return new GameStateData()
        {
            coloredBottleCount = coloredBottleCount,
            emptyBottleCount = emptyBottleCount,
            bottleVolume = bottleVolume,
            bottleList = newBottleList,
        };
    }
    public int coloredBottleCount;
    public int emptyBottleCount;
    public int bottleVolume;
    public string[][] bottleList;
}

public struct GameSuccessor
{
    public int fromIdx;
    public int toIdx;
    public GameStateData data;
}

public struct LastColor
{
    public LastColor(string color, int count)
    {
        this.color = color;
        this.count = count;
    }
    public string color;
    public int count;
}


public class GameState
{
    public GameStateData data;
    public GameSuccessor[] gameSuccessors;
    public LastColor[] lastColors;
    private int score;


    public GameState(GameStateData gameStateData)
    {
        data = gameStateData;
        CalculateLastColors();
        ProcessState();
    }

    public void ProcessState()
    {
        this.gameSuccessors = CalculateAllSuccessors();
        this.score = CalculateScore();
    }

    public GameStateData GetStateData()
    {
        return this.data;
    }

    private GameSuccessor[] CalculateAllSuccessors()
    {
        List<GameSuccessor> successors = new();
        for (int i = 0; i < data.bottleList.Length; i++)
        {
            for (int j = 0; j < data.bottleList.Length; j++)
            {
                if (i == j || !IsCanGiveTo(i, j)) continue;
                var newState = Give(i, j);
                successors.Add(new GameSuccessor()
                {
                    fromIdx = i,
                    toIdx = j,
                    data = newState
                });
            }
        }
        return successors.ToArray();
    }

    private GameStateData Give(int from, int to)
    {
        GameStateData cloneData = this.data.Clone();

        var fromLastColor = this.lastColors[from];
        List<string> fromBottle = new(cloneData.bottleList[from]);
        List<string> toBottle = new(cloneData.bottleList[to]);

        for (int i = 0; i < fromLastColor.count; i++)
        {
            string color = fromBottle[^1];
            toBottle.Add(color);
            fromBottle.RemoveAt(fromBottle.Count - 1);
        }

        cloneData.bottleList[from] = fromBottle.ToArray();
        cloneData.bottleList[to] = toBottle.ToArray();

        return cloneData;
    }

    private bool IsCanGiveTo(int fromIdx, int toIdx)
    {
        LastColor fromLastColor = GetLastColor(fromIdx);
        if (fromLastColor.count == 0 || fromLastColor.count == this.data.bottleVolume) return false;
        int toEmpty = GetEmptyCount(toIdx);
        LastColor toLastColor = GetLastColor(toIdx);
        if (fromLastColor.count == this.data.bottleList[fromIdx].Length)
        {
            if (toLastColor.count == 0) return false;
        }
        if (toLastColor.count == 0) return true;
        if (toLastColor.count == this.data.bottleVolume) return false;

        if (toLastColor.color != fromLastColor.color)
            return false;
        if (toEmpty < fromLastColor.count) return false;

        return true;
    }

    public GameSuccessor[] GetAllGameSuccessors()
    {
        return this.gameSuccessors;
    }

    public int GetScore()
    {
        return this.score;
    }

    public bool IsWin()
    {
        for (int i = 0; i < this.data.bottleList.Length; i++)
        {
            if (this.lastColors[i].count == this.data.bottleVolume || this.lastColors[i].count == 0) continue;
            return false;
        }
        return true;
    }

    public bool IsLose()
    {
        return this.gameSuccessors.Length == 0;
    }

    public LastColor GetLastColor(int idx)
    {
        return this.lastColors[idx];
    }

    private void CalculateLastColors()
    {
        lastColors = new LastColor[this.data.bottleList.Length];
        for (int i = 0; i < this.data.bottleList.Length; i++)
        {
            lastColors[i] = CalculateLastColor(i);
        }
    }

    private LastColor CalculateLastColor(int i)
    {

        LastColor lastColor = new LastColor()
        {
            count = 0,
            color = "",
        };

        if (this.data.bottleList[i] == null || this.data.bottleList[i].Length == 0)
        {
            return lastColor;
        }
        string color = this.data.bottleList[i][^1];
        int count = 0;
        for (int j = this.data.bottleList[i].Length - 1; j >= 0; j--)
        {
            if (data.bottleList[i][j] == color)
            {
                count++;
            }
            else
            {
                break;
            }
        }

        lastColor.color = color;
        lastColor.count = count;
        return lastColor;
    }

    private int GetEmptyCount(int idx)
    {
        return this.data.bottleVolume - this.data.bottleList[idx].Length;
    }

    public int CalculateScore()
    {
        if (IsWin()) return 999999999;
        if (IsLose()) return -999999999;
        int score = this.GetAllGameSuccessors().Length;
        for (int i = 0; i < this.data.bottleList.Length; i++)
        {
            if (this.data.bottleList[i] == null || this.data.bottleList[i].Length == 0)
            {
                score += 2;
                continue;
            }
            string lastColor = data.bottleList[i][0];
            for (int j = 1; j < data.bottleList[i].Length; j++)
            {
                if (lastColor == data.bottleList[i][j])
                {
                    score += 2 * (data.bottleVolume - j);
                }
                else
                {
                    score -= 2 * (data.bottleVolume - j);
                    lastColor = data.bottleList[i][j];
                }
            }
        }

        return score;
    }

}