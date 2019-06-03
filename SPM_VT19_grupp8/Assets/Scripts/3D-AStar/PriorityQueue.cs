/// <summary>
/// A priority queue for <see cref="BoxCompareNode"/> used by the 3D AStar pathfinding algorithm.
/// </summary>
public class PriorityQueue
{
    private const int DEFAULT_CAPACITY = 11;

    private int currentSize; // Number of elements in heap
    private BoxCompareNode[] array; // The heap array

    public PriorityQueue()
    {
        currentSize = 0;
        array = new BoxCompareNode[DEFAULT_CAPACITY];
    }

    public PriorityQueue(int size)
    {
        currentSize = 0;
        array = new BoxCompareNode[size];
    }

    public void Insert(BoxCompareNode box)
    {
        if (currentSize == array.Length - 1)
            EnlargeArray(array.Length * 2 + 1);

        // Percolate up
        int hole = ++currentSize;
        for (array[0] = box; hole > 1 && box.CompareTo(array[ParentIndex(hole)]) < 0; hole = ParentIndex(hole))
            array[hole] = array[ParentIndex(hole)];
        array[hole] = box;
    }

    private void EnlargeArray(int newSize)
    {
        BoxCompareNode[] old = array;
        array = new BoxCompareNode[newSize];
        for (int i = 0; i < old.Length; i++)
            array[i] = old[i];
    }

    public int ParentIndex(int i)
    {
        if (i < 2)
            throw new System.ArgumentException(i + "");
        return i / 2;
    }

    public int FirstChildIndex(int i)
    {
        if (i < 1)
            throw new System.ArgumentException(i + "");
        return i * 2;
    }

    public int Size()
    {
        return currentSize;
    }

    public BoxCompareNode get(int i)
    {
        return array[i];
    }

    private void PercolateDown(int hole)
    {
        int child;
        BoxCompareNode tmp = array[hole];

        for (; FirstChildIndex(hole) <= currentSize; hole = child)
        {
            child = FirstChildIndex(hole);
            if (child != currentSize)
            { 
                int temp = child;
                for (int i = child; i < child + 2; i++)
                {
                    if (array[i] != null && array[i].CompareTo(array[temp]) < 0)
                        temp = i;
                }
                child = temp;
            }
            if (array[child].CompareTo(tmp) < 0)
                array[hole] = array[child];
            else
                break;
        }
        array[hole] = tmp;
    }

    public void MakeEmpty()
    {
        currentSize = 0;
    }

    public bool IsEmpty()
    {
        return currentSize == 0;
    }

    private void BuildHeap()
    {
        for (int i = currentSize / 2; i > 0; i--)
            PercolateDown(i);
    }

    public BoxCompareNode DeleteMin()
    {
        if (IsEmpty())
            throw new System.Exception("List is empty");

        BoxCompareNode minItem = FindMin();
        array[1] = array[currentSize--];
        PercolateDown(1);

        array[currentSize + 1] = null;

        return minItem;
    }

    public BoxCompareNode FindMin()
    {
        if (IsEmpty())
            throw new System.Exception("List is empty");
        return array[1];
    }
}
