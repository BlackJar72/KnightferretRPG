using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;


namespace kfutils {


    /// <summary>
    /// A simple priority queue implemented as a binary heap.
    /// 
    /// While recent version of .NET have a standard priority queue, 
    /// these version are not supported by Unity; with Unity there 
    /// is not pre-made standard priority, and priority queues have 
    /// many uses. 
    /// 
    /// In the past I used an free prioriy queue implementation from 
    /// a developer going by Blue Raja: 
    /// 
    /// https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    /// 
    /// However, I wanted my own that I fully understood and could better 
    /// predict the behavior of, as the one mentioned above and the one 
    /// I was used to from Java did not always seem to produce the same 
    /// result. Also, it seems like a good learning exercise to improve 
    /// my knowledge of algorithms.
    /// 
    /// For any not familiar with priority queues, they return the item 
    /// with the lowest (or sometimes highest) value first without fully 
    /// sorting the items; a binary heap being a data structure than 
    /// naturally functions as a priority queue.
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
    public class PriorityQueue<T> : IEnumerable<T> where T : IComparable 
    {
        private T[] data;
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
        


        public PriorityQueue(int minSize = MIN_SIZE)
        {
            this.minSize = minSize;
            data = new T[minSize];
            count = 0;
        }


        /// <summary>
        /// Expand the backing array if running out of room.
        /// </summary>
        private void Expand()
        {
            T[] bigger = new T[data.Length * 2];
            Array.Copy(data, 0, bigger, 0, data.Length);
            data = bigger;
        }


        /// <summary>
        /// Shrink the backing array if the count becomes much smaller 
        /// than its length.
        /// </summary>
        private void Shrink()
        {            
            T[] smaller = new T[Math.Max(data.Length / 2, minSize)];
            Array.Copy(data, 0, smaller, 0, smaller.Length);
            data = smaller;        
        }


        /// <summary>
        /// Return the first item in the priority queue without removing it. 
        /// The queue does not change.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public T Peek() => data[0];


        /// <summary>
        /// Add an item to the priority queue. 
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if(count >= data.Length) Expand();
            data[count] = item;
            MoveUp(count);
            count++;
        }


        /// <summary>
        /// A synonym for add, for those who prefer more stack like language 
        /// mirroring the use of the terms Peek and Pop for other methods.
        /// </summary>
        /// <param name="item"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        public void Push(T item) => Add(item);


        /// <summary>
        /// Removes the first item in the priority queue and returns it. 
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            if(count < 1) return default(T);
            T result = data[0];
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
            if(data.Length > minSize) data = new T[minSize];
        }


        /// <summary>
        /// Method internal to the priority queue, for maintaining the heap.
        /// </summary>
        /// <param name="i"></param>
        private void MoveUp(int i)
        {
            T tmp = data[i];
            for(int parent; (i > 0) && (tmp.CompareTo(data[parent = GetParent(i)]) < 0); i = parent)
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
            T tmp = data[i];
            int rightChild;
            for(int child; (child = GetLeft(i)) < count; i = child)
            {
                rightChild = child + 1;
                if((rightChild < count) && (data[rightChild].CompareTo(data[child]) < 0)) child = rightChild;
                if(data[child].CompareTo(tmp) > -1) break;
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
            Array.Copy(data, 0, result, 0, result.Length);
            return result;
        }


        /// <summary>
        /// Returns a shallow copy of the data in the priority queue as a new standard List.
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            List<T> result = new(count);
            for(int i = 0; i < count; i++) result.Add(data[i]);
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
                yield return data[i];
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
                if(data[i].Equals(item)) return true;
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
            Array.Copy(data, 0, array, arrayIndex, number);
        }

    }

}

