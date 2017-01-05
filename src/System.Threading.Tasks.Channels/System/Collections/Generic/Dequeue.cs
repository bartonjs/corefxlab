﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Collections.Generic
{
    /// <summary>Provides a double-ended queue data structure.</summary>
    /// <typeparam name="T">Type of the data stored in the dequeue.</typeparam>
    [DebuggerDisplay("Count = {_size}")]
    internal sealed class Dequeue<T>
    {
        private T[] _array = Array.Empty<T>();
        private int _head; // First valid element in the queue
        private int _tail; // First open slot in the dequeue, unless the dequeue is full
        private int _size; // Number of elements.

        public int Count => _size;

        public void EnqueueTail(T item)
        {
            if (_size == _array.Length)
            {
                Grow();
            }

            _array[_tail] = item;
            if (++_tail == _array.Length)
            {
                _tail = 0;
            }
            _size++;
        }

        public void EnqueueHead(T item)
        {
            if (_size == _array.Length)
            {
                Grow();
            }

            _head = (_head == 0 ? _array.Length : _head) - 1;
            _array[_head] = item;
            _size++;
        }

        public T DequeueHead()
        {
            Debug.Assert(_size > 0); // caller's responsibility to make sure there are elements remaining

            T item = _array[_head];
            _array[_head] = default(T);

            if (++_head == _array.Length)
            {
                _head = 0;
            }
            _size--;

            return item;
        }

        public T DequeueTail()
        {
            Debug.Assert(_size > 0); // caller's responsibility to make sure there are elements remaining

            if (--_tail == -1)
            {
                _tail = _array.Length - 1;
            }

            T item = _array[_tail];
            _array[_tail] = default(T);

            _size--;
            return item;
        }

        public IEnumerator<T> GetEnumerator() // meant for debug purposes only
        {
            int pos = _head;
            int count = _size;
            while (count-- > 0)
            {
                yield return _array[pos];
                pos = (pos + 1) % _size;
            }
        }

        private void Grow()
        {
            const int MinimumGrow = 4;

            int capacity = (int)(_array.Length * 2L);
            if (capacity < _array.Length + MinimumGrow)
            {
                capacity = _array.Length + MinimumGrow;
            }

            T[] newArray = new T[capacity];

            if (_head < _tail)
            {
                Array.Copy(_array, _head, newArray, 0, _size);
            }
            else
            {
                Array.Copy(_array, _head, newArray, 0, _array.Length - _head);
                Array.Copy(_array, 0, newArray, _array.Length - _head, _tail);
            }

            _array = newArray;
            _head = 0;
            _tail = (_size == capacity) ? 0 : _size;
        }
    }
}
