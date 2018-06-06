using System;
using System.Collections;
using System.Collections.Generic;

namespace Ulibs.Tests.TinyJsonSer
{
    /// <summary>
    /// Simple implementation of <see cref="IReadOnlyList{T}"/> that doesn't implement any other
    /// collection interface.
    /// </summary>
    public class CustomReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly T _item;

        public CustomReadOnlyList(T item)
        {
            _item = item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
            yield return _item;
        }

        public int Count => 1;

        public T this[int index] => index == 0 ? _item : throw new IndexOutOfRangeException();
    }
}