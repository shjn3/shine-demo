using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMapScroller : ReuseScroller<LevelMapScrollerItemData>
{
    protected override void Start()
    {
        base.Start();
        int column = 4;
        int maxLevel = 200;
        List<LevelMapScrollerItemData> levelItemsData = new();
        int currentLevel = DataStorage.GetInt(Player.PlayerDataKey.LEVEL, 1);

        for (int i = 0; i < maxLevel; i += column)
        {
            List<LevelMapScrollerElementData> levelElementData = new();
            for (int j = 0; j < column && i + j < maxLevel; j++)
            {
                levelElementData.Add(new LevelMapScrollerElementData
                {
                    level = i + j + 1,
                });
            }

            levelItemsData.Add(new LevelMapScrollerItemData()
            {
                elements = levelElementData.ToArray()
            });
        }
        UpdateItemsData(levelItemsData.ToArray());

        ScrollTo(Mathf.CeilToInt(currentLevel / column));
    }
}
