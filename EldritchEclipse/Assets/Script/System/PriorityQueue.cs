using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<(T, int)> heap;
    private IComparer<int> comparer;

    public PriorityQueue() : this(Comparer<int>.Default) { }

    public PriorityQueue(IComparer<int> comparer)
    {
        this.heap = new List<(T, int)>();
        this.comparer = comparer;
    }

    public int Count => heap.Count;

    public void Enqueue(T item, int priority)
    {
        heap.Add((item, priority));
        int index = heap.Count - 1;
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (comparer.Compare(heap[parentIndex].Item2, heap[index].Item2) <= 0)
                break;
            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    public (T, int) Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Priority queue is empty");

        (T item, int priority) = heap[0];
        int lastIndex = heap.Count - 1;
        heap[0] = heap[lastIndex];
        heap.RemoveAt(lastIndex);
        lastIndex--;

        int parentIndex = 0;
        while (true)
        {
            int leftChildIndex = parentIndex * 2 + 1;
            if (leftChildIndex > lastIndex)
                break;

            int rightChildIndex = leftChildIndex + 1;
            int minChildIndex = (rightChildIndex <= lastIndex && comparer.Compare(heap[rightChildIndex].Item2, heap[leftChildIndex].Item2) < 0)
                ? rightChildIndex
                : leftChildIndex;

            if (comparer.Compare(heap[parentIndex].Item2, heap[minChildIndex].Item2) <= 0)
                break;

            Swap(parentIndex, minChildIndex);
            parentIndex = minChildIndex;
        }

        return (item, priority);
    }

    public (T, int) Peek()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("Priority queue is empty");

        return heap[0];
    }

    private void Swap(int i, int j)
    {
        (T, int) temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }

    public bool IsEmpty => !(heap.Count > 0);
}
