using System;
using System.Collections.Generic;
using UnityEngine;

public class MaxNode<T>
{
    public T data;
    public float score;
    public MaxNode()
    {

    }

    public MaxNode(T data, float score)
    {
        this.data = data;
        this.score = score;
    }

    public static implicit operator float(MaxNode<T> obj)
    {
        return obj.score;
    }

    public static implicit operator T(MaxNode<T> obj)
    {
        return obj.data;
    }
}

public class MaxHeap<T>
{
    public List<MaxNode<T>> list = new();

    public bool IsEmpty()
    {
        return list.Count == 0;
    }
    public int Parent(int idx)
    {
        return Mathf.FloorToInt((idx - 1) / 2f);
    }

    public int LeftChild(int idx)
    {
        return idx * 2 + 1;
    }

    public int RightChild(int idx)
    {
        return idx * 2 + 2;
    }

    public bool IsLeaf(int idx)
    {
        return idx >= Mathf.FloorToInt(this.list.Count / 2f) && idx < this.list.Count;
    }

    public void Swap(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || toIdx < 0 || fromIdx >= list.Count || toIdx >= list.Count)
        {
            return;
        }

        (list[fromIdx], list[toIdx]) = (list[toIdx], list[fromIdx]);
    }

    public void HeapifyDown(int idx)
    {
        if (IsLeaf(idx)) return;

        int leftChildIdx = LeftChild(idx);
        int rightChildIdx = RightChild(idx);
        int nodeIdx = idx;

        if (leftChildIdx <= this.list.Count - 1)
        {
            if (list[leftChildIdx].score > this.list[nodeIdx].score) nodeIdx = leftChildIdx;
        }

        if (rightChildIdx <= this.list.Count - 1)
        {
            if (list[rightChildIdx].score > list[nodeIdx].score) nodeIdx = rightChildIdx;
        }

        if (nodeIdx == idx) return;

        Swap(nodeIdx, idx);
        HeapifyDown(nodeIdx);
    }

    public void HeapifyUp(int idx)
    {
        if (idx <= 0 || idx >= this.list.Count) return;
        int parent = Parent(idx);

        if (this.list[parent].score < this.list[idx])
        {
            Swap(parent, idx);
            HeapifyUp(parent);
        }
    }

    public void Add(MaxNode<T> node)
    {
        this.list.Add(node);
        this.HeapifyUp(this.list.Count - 1);
    }

    public MaxNode<T> Peek()
    {
        return this.list[0];
    }

    public MaxNode<T> ExtractMax()
    {
        if (this.list.Count == 0)
        {
            return null;
        }
        var max = this.list[0];
        if (this.list.Count == 1)
        {
            list.RemoveAt(0);
            return max;
        }
        var last = this.list[^1];
        list[0] = last;
        list.RemoveAt(this.list.Count - 1);
        this.HeapifyDown(0);

        return max;
    }

    public void BuildHeap(List<MaxNode<T>> values)
    {
        this.list = values;
        for (int i = Mathf.FloorToInt((this.list.Count) / 2f); i >= 0; i--)
        {
            HeapifyDown(i);
        }
    }

    public void Print()
    {
        int i = 0;
        while (!this.IsLeaf(i))
        {
            string debugLog = $"Parent {i}: " + this.list[i].score;
            var leftIdx = LeftChild(i);
            var rightIdx = RightChild(i);
            if (leftIdx < this.list.Count)
            {
                debugLog += $"\nLeft child {leftIdx}: " + this.list[leftIdx].score;
            }

            if (rightIdx < this.list.Count)
            {
                debugLog += $"\nRight child {rightIdx}: " + this.list[rightIdx].score;
            }
            Debug.Log(debugLog);
            i++;
        }
    }
}