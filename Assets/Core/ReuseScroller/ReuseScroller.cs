using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ReuseScroller<T> : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public enum ScrollerDirection
    {
        VERTICAL = 0,
        HORIZONTAL = 1
    }

    public enum ScrollerType
    {
        NORMAL = 0,
        REVERSAL = 1
    }

    public Action onBeginDrag = () => { };
    public Action onEndDrag = () => { };

    public Action onDrag = () => { };


    [HideInInspector]
    public ScrollRect scrollRect;
    [HideInInspector]
    public RectTransform content;
    [HideInInspector]
    public RectTransform viewport;
    public Transform itemsParent;
    public BaseReuseScrollerItem<T> prefabItem;
    public ScrollerDirection direction = ScrollerDirection.HORIZONTAL;
    public ScrollerType type = ScrollerType.NORMAL;

    [HideInInspector]
    public LinkedList<BaseReuseScrollerItem<T>> items = new();

    public Stack<BaseReuseScrollerItem<T>> poolItem = new();
    public T[] itemsData;
    private float totalHeight = 0;

    public bool IsEnable
    {
        get
        {
            if (scrollRect == null)
            {
                return false;
            }

            return true;
        }
    }

    public float sizeItem = 0;
    public float padding = 0;
    public Vector2 margin = new Vector2(0, 0);

    protected virtual void Awake()
    {
        InitSettings();
    }

    protected virtual void Start()
    {

    }


    void InitSettings()
    {
        scrollRect = GetComponent<ScrollRect>();
        if (scrollRect == null)
        {
            return;
        }
        content = scrollRect.content;
        viewport = scrollRect.viewport;

        scrollRect.vertical = direction == ScrollerDirection.VERTICAL;
        scrollRect.horizontal = direction == ScrollerDirection.HORIZONTAL;


        switch (direction)
        {
            case ScrollerDirection.HORIZONTAL:
                content.anchorMin = type == ScrollerType.NORMAL ? Vector2.zero : Vector2.right;
                content.anchorMax = type == ScrollerType.NORMAL ? Vector2.up : Vector2.one;
                break;
            default:
                content.anchorMin = type == ScrollerType.NORMAL ? Vector2.zero : Vector2.up;
                content.anchorMax = type == ScrollerType.NORMAL ? Vector2.right : Vector2.one;
                content.pivot = type == ScrollerType.NORMAL ? new Vector2(0.5f, 0) : new Vector2(0.5f, 1);
                break;
        }

        scrollRect.onValueChanged.AddListener(OnValueChanged);
    }

    public void UpdateItemsData(T[] data)
    {
        itemsData = data;
        totalHeight = sizeItem * data.Length + Math.Max(0, padding * (data.Length - 1)) + margin.x + margin.y;
        content.anchoredPosition = Vector3.zero;
        content.sizeDelta = new Vector2(0, totalHeight);
        UpdateItems();
    }

    protected void CreateLastItem(int idx)
    {
        if (items.Last.Value.idx == itemsData.Length - 1 || idx >= itemsData.Length || idx < 0)
        {
            Debug.Log("Don't add last item!!");
            return;
        }

        var item = InstantiateItem(idx, itemsData[idx]);

        items.AddLast(item);
    }

    protected void CreateFirstItem(int idx)
    {
        if (items.Count != 0 && items.First.Value.idx == 0 || idx >= itemsData.Length || idx < 0)
        {
            Debug.Log("Don't add first item!!");
            return;
        }

        var item = InstantiateItem(idx, itemsData[idx]);


        items.AddFirst(item);
    }

    protected void RemoveFirstItem()
    {
        if (items.Count == 0)
        {
            return;
        }

        var item = items.First.Value;
        items.RemoveFirst();
        item.gameObject.SetActive(false);
        poolItem.Push(item);
    }

    protected void RemoveLastItem()
    {
        if (items.Count == 0)
        {
            return;
        }

        var item = items.Last.Value;
        items.RemoveLast();

        item.gameObject.SetActive(false);
        poolItem.Push(item);
    }


    public BaseReuseScrollerItem<T> InstantiateItem(int idx, T elementData)
    {
        BaseReuseScrollerItem<T> item = null;
        if (poolItem.Count == 0)
        {
            var go = Instantiate(prefabItem);
            go.transform.SetParent(itemsParent ? itemsParent : content.transform);
            item = go.GetComponent<BaseReuseScrollerItem<T>>();
        }
        else
        {
            item = poolItem.Pop();
            item.gameObject.SetActive(true);
        }

        item.rect.anchorMax = content.anchorMax;
        item.rect.anchorMin = content.anchorMin;
        item.rect.pivot = content.pivot;
        item.idx = idx;
        item.UpdateData(elementData);
        AlignItem(item);

        return item;
    }

    public void AlignItem(BaseReuseScrollerItem<T> item)
    {
        if (item.idx == -1)
        {
            return;
        }

        if (direction == ScrollerDirection.VERTICAL)
        {
            if (type == ScrollerType.NORMAL)
            {
                item.AnchoredPositionY = margin.x + item.idx * (sizeItem + padding);
            }
            else
            {
                item.AnchoredPositionY = -(item.idx * (sizeItem + padding) + margin.x);
            }
        }
        else
        {
            item.AnchoredPosition = Vector3.zero;
        }

    }

    public void AlignItems()
    {
        foreach (var item in items)
        {
            AlignItem(item);
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        // UpdateItems();

    }

    public void OnValueChanged(Vector2 pos)
    {
        UpdateItems();
    }

    public void UpdateItems()
    {
        if (itemsData == null || itemsData.Length == 0)
        {
            return;
        }

        RemoveItems();
        AddItems();
    }

    public void AddItems()
    {
        if (items.Count == 0)
        {
            CreateFirstItem(GetIdx(content.anchoredPosition.y));
        }

        if (direction == ScrollerDirection.VERTICAL)
        {
            if (type == ScrollerType.NORMAL)
            {
                if (items.Count > 0)
                {
                    while (items.First.Value.Bottom > -content.anchoredPosition.y - (sizeItem * 0.5f + padding))
                    {
                        if (items.First.Value.idx == 0)
                        {
                            break;
                        }
                        CreateFirstItem(items.First.Value.idx - 1);
                    }

                    while (items.Last.Value.Top < -content.anchoredPosition.y + (viewport.rect.size.y + (sizeItem * 0.5f + padding)))
                    {
                        if (items.Last.Value.idx == itemsData.Length - 1)
                        {
                            break;
                        }
                        CreateLastItem(items.Last.Value.idx + 1);
                    }
                }
            }
            else
            {
                if (items.Count > 0)
                {
                    while (-items.First.Value.Top > content.anchoredPosition.y - (sizeItem * 0.5f + padding))
                    {
                        if (items.First.Value.idx == 0)
                        {
                            break;
                        }
                        CreateFirstItem(items.First.Value.idx - 1);
                    }

                    while (-items.Last.Value.Bottom < content.anchoredPosition.y + (viewport.rect.size.y + (sizeItem * 0.5f + padding)))
                    {
                        if (items.Last.Value.idx == itemsData.Length - 1)
                        {
                            break;
                        }
                        CreateLastItem(items.Last.Value.idx + 1);
                    }
                }

            }
        }

    }

    public void RemoveItems()
    {
        if (items.Count == 0)
        {
            return;
        }
        if (direction == ScrollerDirection.VERTICAL)
        {
            if (type == ScrollerType.NORMAL)
            {
                while (items.Count > 0 && items.First.Value.Bottom <= -content.anchoredPosition.y - (sizeItem * 0.5f + padding))
                {
                    RemoveFirstItem();
                }

                while (items.Count > 0 && items.Last.Value.Top >= -content.anchoredPosition.y + (viewport.rect.size.y + (sizeItem * 0.5f + padding)))
                {
                    RemoveLastItem();
                }
            }
            else
            {
                while (items.Count > 0 && -items.First.Value.Top <= content.anchoredPosition.y - (sizeItem * 1.5f + padding))
                {
                    RemoveFirstItem();
                }

                while (items.Count > 0 && -items.Last.Value.Bottom >= content.anchoredPosition.y + (viewport.rect.size.y + (sizeItem * 1.5f + padding)))
                {
                    RemoveLastItem();
                }
            }
        }
    }

    public void ScrollTo(int idx)
    {
        float y = GetPosY(idx);
        Vector2 anchoredPosition = content.anchoredPosition;
        anchoredPosition.y = y - viewport.rect.size.y / 2 + sizeItem / 2;
        scrollRect.content.anchoredPosition = anchoredPosition;
        UpdateItems();
    }


    public float GetPosY(int idx)
    {
        float posY = 0;
        if (direction == ScrollerDirection.VERTICAL)
        {
            if (type == ScrollerType.NORMAL)
            { }
            else
            {
                posY = idx * (sizeItem + padding);
            }
        }
        return posY;
    }
    public int GetIdx(float posY)
    {
        int idx = 0;
        if (direction == ScrollerDirection.VERTICAL)
        {
            if (type == ScrollerType.NORMAL)
            { }
            else
            {
                idx = Mathf.FloorToInt(posY / (sizeItem + padding));
            }
        }
        return idx;
    }
}
