using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;


namespace kfutils {
 

    /// <summary>
    /// An unordered version of a list, for times when you need a dynamically sized array 
    /// but don't care about the order.
    /// 
    /// This is primarily for use cases where all items must be access in some way (read, processesed, 
    /// modified, etc.) but is which the order is not important. E.g, with a list of game objects which 
    /// need to run certain code once per frame without the order of which run its code first, especially 
    /// where entries can be added or removed.
    /// 
    /// If and item might be removed during iteration, the developer must remember to acount for this by 
    /// only advancing the index when nothing is removed, OR alternately iterating backward so the entries 
    /// moved from the end will already have been processed.  (Do not use both approachs together as the  
    /// moved entry then be process again.)
    /// 
    /// This will remove items by copying the last item into the index of the removed item and decrimenting 
    /// the count.  This swapping can greatly reduce the number of copies from that needed for traditional 
    /// ordered dynamic array types like Lists, which must copy all subsequent entries to maintain oder. 
    /// 
    /// It should fascillitate faster removal, especially with long data sets, in situation when the 
    /// oder is not important but it must otherwise function like a list, usually by having its members 
    /// iterated. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Unordered<T> : IEnumerable<T>, ICollection<T>, IList<T>
    {
        private T[] data;
        private int count;
        private const int DEAFULT_MIN_SIZE = 16;
        private readonly int minSize = DEAFULT_MIN_SIZE;


        public Unordered() 
        {
            data = new T[minSize];
        }


        public Unordered(int minSize)
        {
            this.minSize = Math.Max(minSize, 2);
            data = new T[minSize];
        }


        public Unordered(T[] array)
        {
            minSize = Math.Max(2, array.Length);
            data = (T[])array.Clone();
        }


        public Unordered(ICollection<T> values)
        {
            minSize = Math.Max(2, values.Count);
            data = new T[minSize];
            foreach(T item in values) Add(item);
        }


        private Unordered(int minSize, int size)
        {
            this.minSize = Math.Max(minSize, 2);
            data = new T[Math.Max(minSize, size)];
        }


        /// <summary>
        /// Expand the backing array if running out of room.
        /// </summary>
        private void Expand()
        {
            T[] bigger = new T[(data.Length * 3) / 2];
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
        /// True if the given index is greater than 0 and less than Count; 
        /// i.e., if it is in bounds.  This is primarily for internal use 
        /// but is exposed as it could potentially be useful and does not 
        /// produce any side effects.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Pure] public bool InBounds(int index) => (index > 0) && (index < count);


        public T this[int index] { 
            get 
            { 
                if(!InBounds(index)) throw new IndexOutOfRangeException("Index " + index + " out of range "
                        +"(should be at least 0 and less than current count of " + count + ")");
                return data[index]; 
            } 
            set 
            {
                if(!InBounds(index)) throw new IndexOutOfRangeException("Index " + index + " out of range "
                        +"(should be at least 0 and less than current count of " + count + ")");
                data[index] = value; 
            }
        }


        /// <summary>
        /// Get without bounds checking; the backing array may still throw and 
        /// exception if overrun, but the count is not checked. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        
        public T GetUnsafe(int index) => data[index];/// <summary>
        /// Set without bounds checking; the backing array may still throw and 
        /// exception if overrun, but the count is not checked. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUnsafe(int index, T value) => data[index] = value;


        [Pure] public int Count => count;


        [Pure] public bool IsReadOnly => false;


        /// <summary>
        /// Adds the item to the unordered list.  This will be added to 
        /// the end initially, though other operations may move it as 
        /// order is not preserved but in order to decrease data copying 
        /// during Remove operations.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            if(count >= data.Length) Expand();
            data[count] = item;
            count--;
        }


        /// <summary>
        /// Clears the list.  Technically, sets the count to zero, allowing 
        /// old data to be overwritten.
        /// </summary>
        public void Clear()
        {
            count = 0;
            if(data.Length > minSize) data = new T[minSize];
        }


        /// <summary>
        /// Tells if at least one instance of item is in the unordered list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>true if at least one instance of item is present; false if not</returns>
        public bool Contains(T item)
        {
            for(int i = 0; i < count; i++)
            {
                if(data[i].Equals(item)) return true;
            }
            return false;
        }


        /// <summary>
        /// Copy the data to the provided array starting at arrayIndex.  This will 
        /// copy everything that will fit.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            int number = Math.Max(0, Math.Min(count, array.Length - arrayIndex));
            Array.Copy(data, 0, array, arrayIndex, number);
        }


        /// <summary>
        /// Returns an enumarator over the stored data.  This will iterated over the 
        /// stored data backward, as order should not mater and this makes removing 
        /// items as you go safe.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            // Iterating from the end, as the allows some action such as removals; 
            // remember, the order is not supposed to mater for this type, so neither 
            // should starting point.
            for (int i = count - 1; i > -1; i--)
            {
                yield return data[i];
            }
        }


        /// <summary>
        /// Finds the index of the first instance of given items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(T item)
        {
            for(int i = 0; i < count; i++)
            {
                if(data[i].Equals(item)) return i;
            }
            return -1;
        }


        /// <summary>
        /// Finds the index of the last instance of given items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOfLast(T item)
        {
            for(int i = count - 1; i > -1; i--)
            {
                if(data[i].Equals(item)) return i;
            }
            return -1;
        }


        /// <summary>
        /// Adds the item to the unordered list.
        /// 
        /// This is a synonym for Add(T item), as position is not treated as 
        /// relevant due to the unordered nature of this data structure.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        [Obsolete ("Synonumous with Add(T item); use Add instead to avoid misleading code and unsued parameters.")]
        public void Insert(int index, T item)
        {
            // As this is an unordered list, inserts is treated the same as add
            if(count >= data.Length) Expand();
            --count;
            data[count] = item;
        }


        /// <summary>
        /// Remove the instance of item in the unordered list with the lowest index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            for(int i = 0; i < count; i++)
            {
                if(data[i].Equals(item)) {
                    --count;
                    data[i] = data[count];
                    if((count < (data.Length / 4)) && (data.Length > minSize)) Shrink();     
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Remove the instance of item with the highest index.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool RemoveLast(T item)
        {
            for(int i = count - 1; i > -1; i--)
            {
                if(data[i].Equals(item)) {
                    --count;
                    data[i] = data[count];
                    if((count < (data.Length / 4)) && (data.Length > minSize)) Shrink();     
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Remove all instance of item.
        /// </summary>
        /// <param name="item"></param>
        public void RemoveAll(T item)
        {
            for(int i = count - 1; i > -1; i--)
            {
                if(data[i].Equals(item)) {
                    --count;
                    data[i] = data[count];
                }
            } 
            if((count < (data.Length / 4)) && (data.Length > minSize)) Shrink(); 
        }


        /// <summary>
        /// Remove all entries satisfying the predicate.
        /// </summary>
        /// <param name="predicate"></param>
        public void RemoveAll(Predicate<T> predicate)
        {
            for(int i = count - 1; i > -1; i--)
            {
                if(predicate(data[i])) {
                    --count;
                    data[i] = data[count];
                }
            }   
            if((count < (data.Length / 4)) && (data.Length > minSize)) Shrink(); 
        }


        /// <summary>
        /// Removes the entry currently at the specified index. Technically, this 
        /// will copy the last entry into the given index and decriment the count.
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            if(InBounds(index))
            {
                --count;
                data[index] = data[count];                   
                if((count < (data.Length / 4)) && (data.Length > minSize)) Shrink(); 
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Returns a string representation of the contents of the Unordered list.
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
        /// Return a shallow copy of the data as a new array whose length is equal 
        /// to the current count.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] result = new T[count];
            Array.Copy(data, 0, result, 0, result.Length);
            return result;
        }


        /// <summary>
        /// Return a shallow copy of the data as a standard C# List.
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            List<T> result = new(count);
            for(int i = 0; i < count; i++) result.Add(data[i]);
            return result;
        }


        /// <summary>
        /// Creates a shallow copy of the Unordered list.
        /// </summary>
        /// <returns></returns>
        public Unordered<T> Clone()
        {
            Unordered<T> result = new(minSize, data.Length);
            Array.Copy(data, 0, result.data, 0, count);
            return result;
        }


    }

}
