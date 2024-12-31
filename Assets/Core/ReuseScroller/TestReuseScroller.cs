using Shine.Scroller;
public class TestData
{
    public int idx;
}


public class TestReuseScroller : ReuseScroller<TestData>
{
    protected override void Awake()
    {
        base.Awake();

        UpdateItemsData(new TestData[]{
        new TestData() { idx = 1 },
            new TestData() { idx = 2 },
            new TestData() { idx = 3 },
            new TestData() { idx = 4 },
            new TestData() { idx = 5 },
            new TestData() { idx = 6 },
            new TestData() { idx = 7 },
            new TestData() { idx = 8 },
            new TestData() { idx = 9 },
        });

        ScrollTo(0);
        UpdateItems();
    }
}

