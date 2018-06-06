using System.Collections;
using System.Collections.Generic;

namespace Ulibs.Tests.TinyJsonSer
{
    /// <summary>
    /// Simple implementation of <see cref="IReadOnlyCollection{T}"/> that doesn't implement any other
    /// collection interface.
    /// </summary>
    public class CustomReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private readonly T _item;

        public CustomReadOnlyCollection(T item)
        {
            _item = item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
            yield return _item;
        }

        public int Count => 1;
    }
}