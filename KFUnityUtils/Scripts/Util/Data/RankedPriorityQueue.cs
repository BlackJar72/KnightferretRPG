using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;


namespace kfutils {


    /// <summary>
    /// A simple priority queue implemented as a binary heap.
    /// 
    /// This is a modified version of the PriorityQueue class that 
    /// allows the value used for ranking entries separate from the 
    /// data being stored.  This is very useful for algorithms like
    /// A* in which the same object could have different values 
    /// (in the case of A*, this would occure if the same graph node 
    /// could be reached by multiple routes with different costs.)
    /// 
    /// The sorting value is called "rank" here as it ranks the order, and 
    /// can be be any data type that implements IComparable, but is primarily 
    /// intended that this be a numeric type (integer or floating point). The 
    /// objects being stored do not need to implement IComparable or any 
    /// other particular interface. 
    /// 
    /// This implementation will alway return the lowest value (or first in 
    /// a natural sort order) first.  This the most common type and what 
    /// is usually needed.  If the reverse is needed the CompareTo method 
    /// must be invereted (either directly or through a wrapping class).
    /// 
    /// Examples of algorithms using priority queues include A* pathfinding, 
    /// heep sort, Prim's minimum spanning tree algorithm, Huffman encoding 
    /// (compression), and various others (only considering those already  
    /// invented). Both Doomlike Dungeons and Caverns Of Evil use priority  
    /// queues, through custum A* implementations, as part of their quality  
    /// control pass in proceduarlly generating dungeons / levels.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class RankedPriorityQueue<T, R> : IEnumerable<T> where R : IComparable
    {

        private struct Entry : IComparable
        {
            public T data;
            public R rank;
            public Entry(T data, R rank) { this.data = data; this.rank = rank; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)] 
            public int CompareTo(object obj) => rank.CompareTo(obj);
            public override string ToString() => "{" + data + "  :@  " + rank + "}";
        }


        private Entry[] data;
        private int count;
        
        private const int MIN_SIZE = 16;
        private readonly int minSize;

        [Pure] public int Count => count;
        [Pure] public bool IsEmpty => count < 1;
        [Pure] public bool NotEmpty => count > 0;

        [Pure] public bool IsReadOnly => false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        [Pure] private int GetParent(int i) => (i - 1) / 2;
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        [Pure] private int GetLeft(int i) => (i * 2) + 1;  // Left child
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        [Pure] private int GetRight(int i) => (i * 2) + 2; // Right child
        


        public RankedPriorityQueue(int minSize = MIN_SIZE)
        {
            this.minSize = minSize;
            data = new Entry[minSize];
            count = 0;
        }


        /// <summary>
        /// Expand the backing array if running out of room.
        /// </summary>
        private void Expand()
        {
            Entry[] bigger = new Entry[data.Length * 2];
            Array.Copy(data, 0, bigger, 0, data.Length);
            data = bigger;
        }


        /// <summary>
        /// Shrink the backing array if the count becomes much smaller 
        /// than its length.
        /// </summary>
        private void Shrink()
        {            
            Entry[] smaller = new Entry[Math.Max(data.Length / 2, minSize)];
            Array.Copy(data, 0, smaller, 0, smaller.Length);
            data = smaller;        
        }


        /// <summary>
        /// Return the first item in the priority queue without removing it. 
        /// The queue does not change.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public T Peek() => data[0].data;


        /// <summary>
        /// Add an item to the priority queue. 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item, R rank)
        {
            if(count >= data.Length) Expand();
            data[count] = new Entry(item, rank);
            MoveUp(count);
            count++;
        }


        /// <summary>
        /// A synonym for add, for those who prefer more stack like language 
        /// mirroring the use of the terms Peek and Pop for other methods.
        /// </summary>
        /// <param name="item"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public void Push(T item, R rank) => Add(item, rank);


        /// <summary>
        /// Removes the first item in the priority queue and returns it. 
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if(count < 1) return default(T);
            T result = data[0].data;
            --count;
            data[0] = data[count];
            MoveDown(0);
            if((count < (data.Length / 4)) && (data.Length > minSize)) Shrink();
            return result;
        }


        /// <summary>
        /// Remove all entries from the priority queue.
        /// </summary>
        public void Clear()
        {
            count = 0;
            if(data.Length > minSize) data = new Entry[minSize];
        }


        /// <summary>
        /// Method internal to the priority queue, for maintaining the heap.
        /// </summary>
        /// <param name="i"></param>
        private void MoveUp(int i)
        {
            Entry tmp = data[i];
            for(int parent; (i > 0) && (tmp.CompareTo(data[parent = GetParent(i)].rank) < 0); i = parent)
            {
                data[i] = data[parent];
            }
            data[i] = tmp;
        } 


        /// <summary>
        /// Method internal to the priority queue, for maintaining the heap.
        /// </summary>
        /// <param name="i"></param>
        private void MoveDown(int i)
        {
            Entry tmp = data[i];
            int rightChild;
            for(int child; (child = GetLeft(i)) < count; i = child)
            {
                rightChild = child + 1;
                if((rightChild < count) && (data[rightChild].CompareTo(data[child].rank) < 0)) child = rightChild;
                if(data[child].rank.CompareTo(tmp.rank) > -1) break;
                data[i] = data[child];
            }
            data[i] = tmp;
        }


        /****************************************************************************/
        /*        METHODS BELOW WILL MOST LLIKELY ONLY BE USED FOR TESTING          */
        /****************************************************************************/


        /// <summary>
        /// Returns a string representing the data in the priority queue.
        /// </summary>
        /// <returns></returns>
       public override string ToString()
        {
            System.Text.StringBuilder builder = new("[");
            for (int i = 0; i < count; i++)
            {
                builder.Append(data[i].data);
                if (i < (count - 1)) builder.Append(", ");
            }
            builder.Append("]");
            return builder.ToString();
        }


        /// <summary>
        /// Returns a string representing the data in the priority queue along 
        /// with its rank. The results will be formatted as {data  :@  rank} . 
        /// </summary>
        /// <returns></returns>
       public string ToDetailedString()
        {
            System.Text.StringBuilder builder = new("[");
            for (int i = 0; i < count; i++)
            {
                builder.Append(data[i]);
                if (i < (count - 1)) builder.Append(", ");
            }
            builder.Append("]");
            return builder.ToString();
        }


        /// <summary>
        /// Returns a shallow copy of the data in the priority queue as a new array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] result = new T[count];
            for(int i = 0; i < result.Length; i++) result[i] = data[i].data;
            return result;
        }


        /// <summary>
        /// Returns a shallow copy of the data in the priority queue as a new standard List.
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            List<T> result = new(count);
            for(int i = 0; i < data.Length; i++) result.Add(data[i].data);
            return result;
        }


        /// <summary>
        /// Returns an enumaerator over the backing array from 0 to count - 1; 
        /// that is, all elements actually containting valid data for the 
        /// priotity queue.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++ )
            {
                yield return data[i].data;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        /// <summary>
        /// Returns true, if the priority queue has at least one entry 
        /// equal to item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if item is in the priority queue at least once, false 
        /// otherwise</returns>
        public bool Contains(T item)
        {
            for(int i = 0; i < count; i++)
            {
                if(data[i].data.Equals(item)) return true;
            }
            return false;
        }


        /// <summary>
        /// Copies data from the backing array into the provided array starting 
        /// at arrayIndex until reaching the end of either array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            int number = Math.Max(0, Math.Min(count, array.Length - arrayIndex));
            for(int i = 0; i < number; i++) array[i] = data[i].data;
        }

    }


}
