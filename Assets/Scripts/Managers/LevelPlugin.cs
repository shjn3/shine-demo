using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.Linq;
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
        public LevelData()
        {
            //
        }
        public LevelData(LevelData lData)
        {
            level = lData.level;
            coloredBottleCount = lData.coloredBottleCount;
            emptyBottleCount = lData.emptyBottleCount;
            bottleVolume = lData.bottleVolume;
            solution = lData.solution.Select(i => new List<int>(i)).ToList();
            bottleList = lData.bottleList.Select(i => new List<string>(i)).ToList();
        }
    }

    private Dictionary<int, LevelData> levelDataByLevel = new Dictionary<int, LevelData>();
    public int currentLevel = 0;
    public LevelPlugin()
    {
        //
    }

    private LevelData LoadLevel(int level)
    {
        if (levelDataByLevel.ContainsKey(level))
        {
            return levelDataByLevel[level];
        }
        string path = "levels/level_" + level.ToString("D5");
        TextAsset myAsset = Resources.Load<TextAsset>(path);
        levelDataByLevel.Add(level, JsonConvert.DeserializeObject<LevelData>(myAsset.text));
        return levelDataByLevel[level];
    }

    public LevelData GetLevelData(int level)
    {
        if (levelDataByLevel.ContainsKey(level))
        {
            return levelDataByLevel[level];
        }

        return LoadLevel(level);
    }
}