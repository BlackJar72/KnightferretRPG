using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


// FIXME: THis should just be in kfutils, not RPG specfic
namespace kfutils
{
    /// <summary>
    /// Implents a non-expandable Deque in an array that acts as a ring structure.
    /// 
    /// The capacity is set at creation.  The end and start points are moved as elements 
    /// are added or removed, overwriting unused data rather than adding or removing 
    /// new elements.  If full, it will remove the element from the opposite side of the 
    /// deque; the safe add options can be used to void this, add will instead fail if 
    /// the deque is full. 
    /// 
    /// Elements can be removed (but not added) to the middle of the deque, though this 
    /// is not how it is typcally meant to be used.  Adding to the middle is not allowed 
    /// as it would not be clear which end to ovewrite if full. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RingDeque<T> : IEnumerable<T>, ICollection<T>
    {
        private readonly T[] data;
        private int start;
        private int stop;
        private int count;

        public int capacity => data.Length;
        public int Count => count;

        public bool IsReadOnly => false;
        public bool IsFull => count >= data.Length;
        public bool HasSpace => count < data.Length;
        public bool IsEmpty => count < 1;


        public RingDeque(int capacity)
        {
            data = new T[capacity];
            start = 0;
            stop = 0;
        }


        /// <summary>
        /// Adds a new element to the front of the deque.  If full the 
        /// last elements will be pushed off the end (deleted from the
        /// deque).
        /// </summary>
        /// <param name="elem"></param>
        public void AddFront(T elem)
        {
            start = (start + data.Length - 1) % data.Length;
            data[start] = elem;
            if (start == stop) stop = (stop + data.Length - 1) % data.Length;
            count = Mathf.Min(++count, data.Length);
        }


        /// <summary>
        /// Adds a element to the end of the deque.  If the deque is full 
        /// the first element will be pushed off the front (deleted from 
        /// the deque).
        /// </summary>
        /// <param name="elem"></param>
        public void AddBack(T elem)
        {
            data[stop] = elem;
            stop = (stop + 1) % data.Length;
            count++;
            if (count > data.Length) start = (start + 1) % data.Length;
            count = Mathf.Min(count, data.Length);
        }


        /// <summary>
        /// Will add a new element to the front deque if and only if it is 
        /// not already full.  Otherwise it will fail.
        /// </summary>
        /// <param name="elem"></param>
        public void AddFrontSafe(T elem)
        {
            if (count < data.Length)
            {
                start = (start + data.Length - 1) % data.Length;
                data[start] = elem;
                count++;
            }
        }


        /// <summary>
        /// Will add a new element to the end of the deeuq if and only if it 
        /// is not already full.  Otherwise it will fail.
        /// </summary>
        /// <param name="elem"></param>
        public void AddBackSafe(T elem)
        {
            if (count < data.Length)
            {
                data[stop] = elem;
                stop = (stop + 1) % data.Length;
                count++;
            }
        }


        /// <summary>
        /// Replace the first element of the deque with the provided 
        /// element.  The deque will not otherwise change.  If the 
        /// deque is empty this will have no effect.
        /// </summary>
        /// <param name="elem"></param>
        public void ReplaceFront(T elem)
        {
            data[start] = elem;
        }


        /// <summary>
        /// Replaces the last element of the deque with the provided 
        /// element.  The deque will not otherwise change.  If the 
        /// deque is empty this will do nothing.
        /// </summary>
        /// <param name="elem"></param>
        public void ReplaceBack(T elem)
        {
            data[start] = elem;
        }


        /// <summary>
        /// Pops the first item fromt he deque, returning it as the result.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T PopFront()
        {
            if (count < 1) throw new IndexOutOfRangeException();
            T result = data[start];
            start = (start + 1) % data.Length;
            count--;
            return result;
        }


        /// <summary>
        /// Pops the last item from the deque, returning it as the result.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T PopBack()
        {
            if (count < 1) throw new IndexOutOfRangeException();
            T result = data[stop];
            stop = (stop + data.Length - 1) % data.Length;
            count--;
            return result;
        }


        /// <summary>
        /// Returns the first element of the deque without altering its contents.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T PeekFront()
        {
            if (count < 1) throw new IndexOutOfRangeException();
            return data[start];
        }


        /// <summary>
        /// Returnes the last element in the deque without altering its contents.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public T PeekBack()
        {
            if (count < 1) throw new IndexOutOfRangeException();
            return data[stop];
        }


        /// <summary>
        /// Returns the contents of the deque as an array.
        /// </summary>
        /// <returns></returns>
        public T[] ToArray()
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = data[(start + i) % data.Length];
            }
            return result;
        }


        /// <summary>
        /// Returns the contents of the deque as a List.
        /// </summary>
        /// <returns></returns>
        public List<T> ToList()
        {
            List<T> result = new(count);
            for (int i = 0; i < count; i++)
            {
                result[i] = data[(start + i) % data.Length];
            }
            return result;
        }


        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                yield return data[(start + i) % data.Length];
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        /// <summary>
        /// Adds an item to the ends, removing the first elements if full. 
        /// The wraps AddBack(T elem) as is only here for compatibility with 
        /// ICollection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            AddBack(item);
        }


        /// <summary>
        ///  Removes all elements from the deque.  Technically this resets 
        /// the beginning, end, and count to 0 and does not alter the backing 
        /// array.
        /// </summary>
        public void Clear()
        {
            start = stop = count = 0;
        }


        /// <summary>
        /// Returns true of the deque contains the item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if ((object)data[(start + i) % data.Length] == (object)item) return true;
            }
            return false;
        }


        /// <summary>
        /// Copies the data into the provided array starting at the given inded, stopping 
        /// when either the deque or array reaches its end.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; (i < count) && ((i + arrayIndex) < array.Length); i++)
            {
                array[i + arrayIndex] = data[(start + i) % data.Length];
            }
        }


        /// <summary>
        /// Removes the first instance of the item from the deque. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            if (count == 0) return false;
            int i;
            bool found = false;
            for (i = 0; !found && (i < count); i++)
            {
                found = (object)data[(start + i) % data.Length] == (object)item;
            }
            for (i++; i < count; i++)
            {
                data[(start + i - 1) % data.Length] = data[(start + i) % data.Length];
            }
            if (found) count--;
            return found;
        }


        /// <summary>
        /// Removes tha element at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveAt(int index)
        {
            if (count == 0) return false;
            for (index++; index < count; index++)
            {
                data[(start + index - 1) % data.Length] = data[(start + index) % data.Length];
            }
            count--;
            return true;
        }


        /// <summary>
        /// Returns a string representation of the deque.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new("[");
            for (int i = 0; i < count; i++)
            {
                builder.Append(data[(start + i) % data.Length]);
                if (i < (count - 1)) builder.Append(", ");
            }
            builder.Append("]");
            return builder.ToString();
        }


    }


}
