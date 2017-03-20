using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    /// <summary>
    /// Utilizes a Min-Max Heap set up with alternating even min and odd max levels
    /// Inserts at log n, Dequeues min and max at log n as well
    /// </summary>
public class PriorityQueue<T>
{
    private List<KeyValuePair<int, T>> items;

    public PriorityQueue()
    {
        items = new List<KeyValuePair<int, T>>();
    }

    /// <summary>
    /// Adds an item to the queue.
    /// </summary>
    /// <param name="value">The item itself</param>
    /// <param name="priority">the priority used for determining dequeue ordering</param>
    public void Enqueue(T value, int priority)
    {
        int currentIndex = items.Count;
        var item = new KeyValuePair<int, T>(priority, value);
        items.Add(item);
        HeapifyUp(currentIndex);
    }

    /// <summary>
    /// Gets the item with the smallest priority
    /// </summary>
    /// <param name="value">holds the value of the item with smallest priority. Blank if no available item</param>
    /// <returns>Does the out value parameter has a valid value</returns>
    public bool DequeueMin(out T value)
    {
        if (items.Count == 0)
        {
            value = default(T);
            return false;
        }
        else if (items.Count == 1)
        {
            var minItem = items.First();
            items.RemoveAt(items.Count - 1);
            value = minItem.Value;
            return true;
        }
        else
        {
            var minItem = items.First();
            HeapifyDown(0);
            value = minItem.Value;

            return true;
        }
    }

    /// <summary>
    /// Gets the item with the largest priority
    /// </summary>
    /// <param name="value">holds the value of the item with largest priority. Blank if no available item</param>
    /// <returns>Does the out value parameter has a valid value</returns>
    public bool DequeueMax(out T value)
    {
        if (items.Count == 0)
        {
            value = default(T);
            return false;
        }
        else if (items.Count == 1)
        {
            var maxItem = items.First();
            items.RemoveAt(items.Count - 1);
            value = maxItem.Value;
            return true;
        }
        else
        {
            int max = GetMinMaxChild(0, false);
            var maxItem = items[max];

            if (maxItem.Equals(items.Last()))
            {
                items.RemoveAt(items.Count - 1);
                value = maxItem.Value;
            }
            else
            {
                HeapifyDown(max, false);
            }

            value = maxItem.Value;
            return true;
        }
    }

    public void Print()
    {
        int totalLevels = (int)Math.Log(items.Count, 2);

        for (int i = 0; i < items.Count; i++)
        {
            int parentLevel = (int)Math.Log(i + 1, 2);
            var kv = items[i];

            Console.Write(kv.Key);
            if ((int)Math.Log(i + 2, 2) != parentLevel)
            {
                Console.Write("\n");
            }
            else
                Console.Write(" ");
        }
    }

    private void HeapifyDown(int startingIndex, bool min = true)
    {
        var lastItem = items.Last();
        items.RemoveAt(items.Count - 1);

        var chain = new List<KeyValuePair<int, int>>();
        GetMinMaxChain(startingIndex, chain, min);
        // Find point in chain to insert last item, swap places above insertion point
        int intersection = FindIntersection(chain, lastItem.Key, min);

        for (int i = 0; i < intersection; i++)
        {
            Swap(items, chain[i].Key, chain[i + 1].Key);
        }
        // Set the last item to that of the insertion point which is where the min item lies
        items[chain[intersection].Key] = lastItem;
    }

    private void HeapifyUp(int startingIndex)
    {
        int priority = items.ElementAt(startingIndex).Key;
        int currentIndex = startingIndex;
        int parent = GetParent(currentIndex);

        // Is Min
        if (priority < items.ElementAt(parent).Key)
        {
            // Get grandparent if parent on max level
            if (!IsMinIndex(parent))
                parent = GetParent(parent);

            while (currentIndex > 0 && priority < items.ElementAt(parent).Key)
            {
                Swap(items, currentIndex, parent);
                currentIndex = parent;
                parent = GetParent(GetParent(parent));
            }
        }
        // Max
        else
        {
            // Get grandparent if parent on min level
            if (IsMinIndex(parent))
                parent = GetParent(parent);

            while (parent > 0 && priority > items.ElementAt(parent).Key)
            {
                Swap(items, currentIndex, parent);
                currentIndex = parent;
                parent = GetParent(GetParent(parent));
            }
        }
    }

    /// <summary>
    /// Calculates the level of the index in order to facilitate knowing if its a min or max level
    /// </summary>
    /// <param name="index">the index of the node</param>
    /// <returns>The level of the node belonging to the index</returns>
    private int CalculateLevel(int index)
    {
        return (int)Math.Log(index + 1, 2);
    }

    /// <summary>
    /// Is it a min (even) level
    /// </summary>
    private bool IsMinLevel(int level)
    {
        return level % 2 == 0;
    }

    /// <summary>
    /// Is it a min (even) index belonging to a node
    /// </summary>
    private bool IsMinIndex(int index)
    {
        return CalculateLevel(index) % 2 == 0;
    }

    private int GetParent(int currentIndex)
    {
        return currentIndex == 0 ? 0 : ((currentIndex + 1) / 2) - 1;
    }

    private int GetChild(int currentIndex)
    {
        return (currentIndex * 2 + 1);
    }

    /// <summary>
    /// Find a appropriate insertion point for a new value in order to keep the chain sorted
    /// </summary>
    /// <param name="chain">reference to the chain</param>
    /// <param name="target">target priority</param>
    /// <param name="min">If its a min or max check</param>
    /// <returns>the insertion point</returns>
    private int FindIntersection(List<KeyValuePair<int, int>> chain, int target, bool min = true)
    {
        int l = 0;
        int r = chain.Count - 1;

        while (l <= r)
        {
            int m = l + (r - l) / 2;

            if (min)
            {
                if (chain[m].Value >= target && m == 0)
                    return m;

                if (chain[m].Value <= target && (m + 1 < chain.Count && chain[m + 1].Value >= target))
                    return m;

                if (chain[m].Value < target)
                    l = m + 1;
                else
                    r = m - 1;
            }
            else
            {
                if (chain[m].Value <= target && m == 0)
                    return m;

                if (chain[m].Value >= target && (m + 1 < chain.Count && chain[m + 1].Value <= target))
                    return m;

                if (chain[m].Value > target)
                    l = m + 1;
                else
                    r = m - 1;
            }
        }

        // if we reach here, then element was not present
        return chain.Count - 1;
    }

    /// <summary>
    /// Creates a chain of nodes from smallest (largest) to largest (smalleest)
    /// </summary>
    /// <param name="index">node to start in</param>
    /// <param name="chain">chain to store nodes</param>
    /// <param name="min">true for small to large, false for large to small</param>
    private void GetMinMaxChain(int index, List<KeyValuePair<int, int>> chain, bool min = true)
    {
        var node = items.ElementAt(index);
        chain.Add(new KeyValuePair<int, int>(index, items.ElementAt(index).Key));

        int left = GetChild(index);
        int right = left + 1;
        int minMax;

        // No Childs. Get opposite min-max level parent nodes in order to create a sorted chain
        // from smallest (largest) to largest (smallest).  
        if (left > items.Count - 1)
        {
            int curentLevel = CalculateLevel(index);
            int sourceLevel = CalculateLevel(chain[0].Key);
            int parent;

            // Get grandparent if starting node's min-max is different
            parent = IsMinLevel(curentLevel) != IsMinLevel(sourceLevel)
                ? GetParent(GetParent(index))
                : GetParent(index);

            // Keep getting opposite min-max grandparents until reach first node
            while (parent != 0)
            {
                chain.Add(new KeyValuePair<int, int>(parent, items.ElementAt(parent).Key));
                parent = GetParent(GetParent(parent));
            }
        }

        // Has Grandchildren
        if (GetChild(GetChild(index)) < items.Count)
        {
            int minMaxLeftG = GetMinMaxChild(left, min);
            int minMaxRightG = GetMinMaxChild(right, min);

            minMax = GetMinMaxBetween(minMaxLeftG, minMaxRightG, min);
        }
        // Compare just children
        else
        {
            minMax = GetMinMaxBetween(left, right, min);
        }

        if (minMax < items.Count)
        {
            GetMinMaxChain(minMax, chain, min);
        }
    }

    private int GetMinMaxChild(int index, bool min = true)
    {
        int firstChild = GetChild(index);
        int secondChild = firstChild + 1;

        if (firstChild > items.Count - 1)
        {
            return index;
        }
        else
        {
            return GetMinMaxBetween(firstChild, secondChild, min);
        }
    }

    /// <summary>
    /// Gets the min-max element between two indices
    /// </summary>
    /// <param name="firstIndex">first index</param>
    /// <param name="secondIndex">second index</param>
    /// <param name="min">if its a min or max check</param>
    /// <returns>the min or max index between them</returns>
    private int GetMinMaxBetween(int firstIndex, int secondIndex, bool min = true)
    {
        var first = items.ElementAtOrDefault(firstIndex);
        int minMax = firstIndex;

        if (secondIndex < items.Count)
        {
            var second = items.ElementAtOrDefault(secondIndex);
            if (min)
                minMax = second.Key < first.Key ? secondIndex : firstIndex;
            else
                minMax = second.Key > first.Key ? secondIndex : firstIndex;
        }

        return minMax;
    }

    // Should be extension method or static helper
    private void Swap(IList<KeyValuePair<int, T>> list, int indexA, int indexB)
    {
        KeyValuePair<int, T> tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }
}

