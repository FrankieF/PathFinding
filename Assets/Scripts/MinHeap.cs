using System;
using System.Collections.Generic;

public class MinHeap<T>
{
    private const int InitialCapacity = 4;
    private int lastIndex;
    private T[] heap;
    private IComparer<T> comparer;

    public MinHeap() : this(InitialCapacity, Comparer<T>.Default) {}
    public MinHeap(int capacity) : this(capacity, Comparer<T>.Default) {}
    public MinHeap(Comparison<T> comparison) : this(InitialCapacity, Comparer<T>.Create(comparison)) {}

    public MinHeap(int capacity, IComparer<T> comparer)
    {
        heap = new T[capacity];
        lastIndex = -1;
        this.comparer = comparer;
    }

    public int Count => lastIndex + 1;

    public void Add(T element)
    {
        if (lastIndex == heap.Length - 1)
            Resize();
        lastIndex++;
        heap[lastIndex] = element;
        HeapifyUp(lastIndex);
    }

    public T Remove()
    {
        if (lastIndex == -1)
            return default;
        var element = heap[0];
        heap[0] = heap[lastIndex];
        lastIndex--;
        HeapifyDown(0);
        return element;
    }

    public T Peek()
    {
        if (lastIndex == -1)
            return default;
        return heap[0];
    }

    public void Clear()
    {
        lastIndex = -1;
    }

    private void HeapifyUp(int index)
    {
        if (index == 0)
            return;
        int childIndex = index;
        int parentIndex = (index - 1) / 2;
        if (comparer.Compare(heap[childIndex], heap[parentIndex]) < 0)
        {
            var temp = heap[childIndex];
            heap[childIndex] = heap[parentIndex];
            heap[parentIndex] = temp;
            HeapifyUp(parentIndex);
        }
    }

    private void HeapifyDown(int index)
    {
        int leftIndex = index * 2 + 1;
        int rightIndex = index * 2 + 2;
        int smallestIndex = index;

        if (leftIndex <= lastIndex && comparer.Compare(heap[leftIndex], heap[smallestIndex]) < 0)
            smallestIndex = leftIndex;
        if (rightIndex <= lastIndex && comparer.Compare(heap[rightIndex], heap[smallestIndex]) < 0)
            smallestIndex = rightIndex;
        if (smallestIndex != index)
        {
            var temp = heap[index];
            heap[index] = heap[smallestIndex];
            heap[smallestIndex] = temp;
            HeapifyDown(smallestIndex);
        }
    }

    private void Resize()
    {
        Array.Resize<T>(ref heap, heap.Length * 2);
    }
}
