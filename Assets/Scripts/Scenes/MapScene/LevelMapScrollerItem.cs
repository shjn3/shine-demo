using System;
using Shine.Scroller;

public class LevelMapScrollerItemData
{
    public LevelMapScrollerElementData[] elements;
}

public class LevelMapScrollerItem : BaseReuseScrollerItem<LevelMapScrollerItemData>
{
    public LevelMapElement[] levelMapElements;
    public override void UpdateData(LevelMapScrollerItemData data)
    {
        base.UpdateData(data);
        for (int i = 0; i < levelMapElements.Length; i++)
        {
            levelMapElements[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < Math.Min(data.elements.Length, levelMapElements.Length); i++)
        {
            levelMapElements[i].gameObject.SetActive(true);
            levelMapElements[i].UpdateStatus(data.elements[i]);
        }
    }
}
