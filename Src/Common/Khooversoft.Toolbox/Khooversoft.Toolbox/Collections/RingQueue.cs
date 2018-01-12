using System;
using System.Collections;
using System.Collections.Generic;

namespace Khooversoft.Toolbox
{
    /// <summary>
    /// Ring queue, FIFO performance queue.
    /// Queue is a fixed size ring
    /// Items in the ring can be overwritten if reads are not as fast as writes.
    /// 
    /// Note: Additional performance can be achieved for reference class when its members are updated and not replaced with
    /// a new instance.
    /// 
    /// This is thread safe
    /// </summary>
    /// <typeparam name="T">Queue of T</typeparam>
    public class RingQueue<T> : IEnumerable<T>
    {
        private int _read = 0;
        private int _write = 0;
        private T[] _objects;
        private readonly object _lock = new object();

        /// <summary>
        /// Construct ring of a specific size
        /// </summary>
        /// <param name="size"></param>
        public RingQueue(int size)
        {
            Verify.Assert(size >= 0, "Size must be greater then 0");

            Size = size;
            Clear();
        }

        /// <summary>
        /// Is queue empty?
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                lock (_lock)
                {
                    return (_read == _write) && (Count == 0);
                }
            }
        }

        /// <summary>
        /// Is queue full
        /// </summary>
        public bool IsFull
        {
            get
            {
                lock (_lock)
                {
                    return (_read == _write) && (Count > 0);
                }
            }
        }

        /// <summary>
        /// Number of records lost because of overwrite
        /// </summary>
        public int LostCount { get; private set; }

        /// <summary>
        /// Current queue count
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Size of queue
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Clear queue
        /// </summary>
        /// <returns>this</returns>
        public RingQueue<T> Clear()
        {
            lock (_lock)
            {
                _read = 0;
                _write = 0;
                _objects = new T[Size + 1];
            }

            return this;
        }

        /// <summary>
        /// Enqueue new item
        /// </summary>
        /// <param name="item">item</param>
        public void Enqueue(T item)
        {
            lock (_lock)
            {
                if (IsFull)
                {
                    Count--;
                    _read = (_read + 1) % Size;
                    LostCount++;
                }

                _objects[_write] = item;

                Count++;
                _write = (_write + 1) % Size;
            }
        }

        /// <summary>
        /// Dequeue from the queue
        /// </summary>
        /// <returns>T</returns>
        /// <exception cref="IndexOutOfRangeException">if queue is empty</exception>
        public T Dequeue()
        {
            lock (_lock)
            {
                Verify.Assert<IndexOutOfRangeException>(!IsEmpty, "Queue is empty");

                T item = _objects[_read];
                Count--;
                _read = (_read + 1) % Size;

                return item;
            }
        }

        /// <summary>
        /// Try to dequeue value
        /// </summary>
        /// <param name="value">value returned</param>
        /// <returns>true if value exists, false if queue is empty</returns>
        public bool TryDequeue(out T value)
        {
            value = default(T);

            lock (_lock)
            {
                if (IsEmpty)
                {
                    return false;
                }

                value = Dequeue();
                return true;
            }
        }

        /// <summary>
        /// Try to peek at value
        /// </summary>
        /// <param name="value">value returned</param>
        /// <returns>true if value exists, false if queue is empty</returns>
        public bool TryPeek(out T value)
        {
            value = default(T);

            lock (_lock)
            {
                if (IsEmpty)
                {
                    return false;
                }

                value = _objects[_read];
                return true;
            }
        }

        /// <summary>
        /// Create new list of items
        /// </summary>
        /// <returns>new IList(T)</returns>
        public IList<T> ToList()
        {
            var list = new List<T>();

            lock (_lock)
            {
                for (int i = 0; i < Count; i++)
                {
                    int index = (_read + i) % Size;
                    list.Add(_objects[index]);
                }
            }

            return list;
        }

        /// <summary>
        /// Makes a copy of the ring before enumerator is returned
        /// </summary>
        /// <returns>enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
