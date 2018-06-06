using System.Collections;
using System.Collections.Generic;

namespace Ulibs.Tests.TinyJsonSer
{
    /// <summary>
    /// Simple implementation of <see cref="IEnumerable{T}"/> that doesn't implement any other
    /// collection interface.
    /// </summary>
    public class CustomEnumerable<T> : IEnumerable<T>
    {
        private readonly T _item;

        public CustomEnumerable(T item)
        {
            _item = item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> GetEnumerator()
        {
            yield return _item;
        }
    }
}