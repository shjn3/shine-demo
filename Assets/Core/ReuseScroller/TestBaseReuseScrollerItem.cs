using UnityEngine.UI;

public class TestBaseReuseScrollerItem : BaseReuseScrollerItem<TestData>
{
    public Text idxText;

    public override void UpdateData(TestData data)
    {
        this.data = data;

        idxText.text = data.idx.ToString();
    }
}