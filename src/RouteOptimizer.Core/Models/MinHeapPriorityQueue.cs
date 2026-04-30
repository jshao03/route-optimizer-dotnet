namespace RouteOptimizer.Core.DataStructures;

public sealed class MinHeapPriorityQueue
{
    #region Fields
    private readonly List<(string NodeId, double Priority)> heap = [];

    public int Count => heap.Count;
    #endregion

    #region Methods
    public void Enqueue(string nodeId, double priority)
    {
        heap.Add((nodeId, priority));
        HeapifyUp(heap.Count - 1);
    }

    /// <summary>
    /// remove the root node of the heap and return it
    /// make the last node as the new root and heapify down to maintain the heap property
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public (string NodeId, double Priority) Dequeue()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("The priority queue is empty.");

        var root = heap[0];
        var last = heap[^1];

        heap.RemoveAt(heap.Count - 1);

        if (heap.Count > 0)
        {
            heap[0] = last;
            HeapifyDown(0);
        }

        return root;
    }

    /// <summary>
    /// Returns the node with the smallest priority without removing it from the queue.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public (string NodeId, double Priority) Peek()
    {
        if (heap.Count == 0)
            throw new InvalidOperationException("The priority queue is empty.");

        return heap[0];
    }

    /// <summary>
    /// Moves the node at the specified index up the heap until the heap property is restored.
    /// </summary>
    /// <param name="index"></param>
    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            var parentIndex = GetParentIndex(index);

            if (heap[index].Priority >= heap[parentIndex].Priority)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    /// <summary>
    /// Moves the node at the specified index down the heap until the heap property is restored.
    /// </summary>
    /// <param name="index"></param>
    private void HeapifyDown(int index)
    {
        while (true)
        {
            var leftChildIndex = GetLeftChildIndex(index);
            var rightChildIndex = GetRightChildIndex(index);
            var smallestIndex = index;

            if (leftChildIndex < heap.Count &&
                heap[leftChildIndex].Priority < heap[smallestIndex].Priority)
            {
                smallestIndex = leftChildIndex;
            }

            if (rightChildIndex < heap.Count &&
                heap[rightChildIndex].Priority < heap[smallestIndex].Priority)
            {
                smallestIndex = rightChildIndex;
            }

            if (smallestIndex == index)
                break;

            Swap(index, smallestIndex);
            index = smallestIndex;
        }
    }

    private static int GetParentIndex(int index) => (index - 1) / 2;

    private static int GetLeftChildIndex(int index) => (2 * index) + 1;

    private static int GetRightChildIndex(int index) => (2 * index) + 2;

    private void Swap(int firstIndex, int secondIndex)
    {
        (heap[firstIndex], heap[secondIndex]) = (heap[secondIndex], heap[firstIndex]);
    }
    #endregion
}