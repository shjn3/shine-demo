using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
public class LevelPlugin
{
    public class LevelData
    {
        public int level;
        public int coloredBottleCount;
        public int emptyBottleCount;
        public int bottleVolume;
        public List<List<string>> bottleList;
        public List<List<int>> solution;
    }

    private Dictionary<int, LevelData> levelDataByLevel = new Dictionary<int, LevelData>();
    public int currentLevel = 0;

    public LevelPlugin()
    {
        //
    }

    public bool LoadLevel(int level)
    {
        if (levelDataByLevel.ContainsKey(level))
        {
            return true;
        }
        string path = "levels/level_" + level.ToString("D5");
        TextAsset myAsset = Resources.Load<TextAsset>(path);
        levelDataByLevel.Add(level, JsonConvert.DeserializeObject<LevelData>(myAsset.text));
        return false;
    }

    public LevelData GetLevelData(int level)
    {
        if (levelDataByLevel.ContainsKey(level))
        {
            return levelDataByLevel[level];
        }

        return null;
    }
}