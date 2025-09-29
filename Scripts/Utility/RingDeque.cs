using System;
using System.Collections;
using System.Collections.Generic;


// FIXME: THis should just be in kfutils, not RPG specfic
namespace kfutils.rpg
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
        public bool IsEmpty => count < 1;


        public RingDeque(int capacity)
        {
            data = new T[capacity];
            start = 0;
            stop = 0;
        }


        public void AddFront(T elem)
        {
            start = (start + data.Length - 1) % data.Length;
            data[start] = elem;
            if (start == stop) stop = (stop + data.Length - 1) % data.Length;
            else count++;
        }


        public void AddBack(T elem)
        {
            stop = (stop + 1) % data.Length;
            data[stop] = elem;
            if (start == stop) stop = (start + 1) % data.Length;
            else count++;
        }


        public void AddFrontSafe(T elem)
        {
            if (count < data.Length)
            {
                start = (start + data.Length - 1) % data.Length;
                data[start] = elem;
                count++;
            }
        }


        public void AddBackSafe(T elem)
        {
            if (count < data.Length)
            {
                stop = (stop + 1) % data.Length;
                data[stop] = elem;
                count++;
            }
        }


        public T PopFront()
        {
            if (count < 1) throw new IndexOutOfRangeException();
            T result = data[start];
            start = (start + 1) % data.Length;
            count--;
            return result;
        }


        public T PopBack()
        {
            if (count < 1) throw new IndexOutOfRangeException();
            T result = data[stop];
            stop = (stop + data.Length - 1) % data.Length;
            count--;
            return result;
        }


        public T PeekFront()
        {
            if (count < 1) throw new IndexOutOfRangeException();
            return data[start];
        }


        public T PeekBack()
        {
            if (count < 1) throw new IndexOutOfRangeException();
            return data[stop];
        }


        public T[] ToArray()
        {
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = data[(start + i) % data.Length];
            }
            return result;
        }


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


        public void Add(T item)
        {
            AddBack(item);
        }


        public void Clear()
        {
            start = stop = count = 0;
        }


        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if ((object)data[(start + i) % data.Length] == (object)item) return true;
            }
            return false;
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; (i < count) && ((i + arrayIndex) < array.Length); i++)
            {
                array[i + arrayIndex] = data[(start + i) % data.Length];
            }
        }


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


    }


}
