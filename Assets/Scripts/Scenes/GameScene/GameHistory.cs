using System.Collections;
using System.Collections.Generic;

public class GameHistoryData
{
    public int level;
    public string[][] bottleList;
    public BallsMove[] moves;
}

public class GameHistoryManager
{
    static GameHistoryData history;
    public static void Save(GameHistoryData data)
    {
        history = data;
    }

    public static GameHistoryData Get()
    {
        return history;
    }
}