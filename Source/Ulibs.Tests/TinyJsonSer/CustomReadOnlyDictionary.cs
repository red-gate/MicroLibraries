using System.Collections;
using System.Collections.Generic;

namespace Ulibs.Tests.TinyJsonSer
{
    /// <summary>
    /// Simple implementation of <see cref="IReadOnlyDictionary{TKey,TValue}"/>
    /// that doesn't implement any other collection interface.
    /// </summary>
    public class CustomReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        private readonly TKey _key;
        private readonly TValue _value;

        public CustomReadOnlyDictionary(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            yield return new KeyValuePair<TKey, TValue>(_key, _value);
        }

        public int Count => 1;

        public bool ContainsKey(TKey key) => EqualityComparer<TKey>.Default.Equals(key, _key);

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = _value;
            return ContainsKey(_key);
        }

        public TValue this[TKey key] => ContainsKey(key) ? _value : throw new KeyNotFoundException();

        public IEnumerable<TKey> Keys
        {
            get { yield return _key; }
        }

        public IEnumerable<TValue> Values
        {
            get { yield return _value; }
        }
    }
}