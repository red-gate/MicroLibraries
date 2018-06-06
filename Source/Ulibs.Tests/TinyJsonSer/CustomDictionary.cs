using System;
using System.Collections;
using System.Collections.Generic;

namespace Ulibs.Tests.TinyJsonSer
{
    /// <summary>
    /// Simple implementation of <see cref="IDictionary{TKey,TValue}"/>
    /// that doesn't implement any other collection interface.
    /// </summary>
    public class CustomDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly TKey _key;
        private readonly TValue _value;

        public CustomDictionary(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            yield return new KeyValuePair<TKey, TValue>(_key, _value);
        }

        public void Add(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();

        public void Clear() => throw new NotSupportedException();

        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            EqualityComparer<TKey>.Default.Equals(item.Key, _key) &&
            EqualityComparer<TValue>.Default.Equals(item.Value, _value);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            array[arrayIndex] = new KeyValuePair<TKey, TValue>(_key, _value);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new NotSupportedException();

        public int Count => 1;
        public bool IsReadOnly => true;
        public bool ContainsKey(TKey key) => EqualityComparer<TKey>.Default.Equals(key, _key);

        public void Add(TKey key, TValue value) => throw new NotSupportedException();

        public bool Remove(TKey key) => throw new NotSupportedException();

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = _value;
            return ContainsKey(key);
        }

        public TValue this[TKey key]
        {
            get => ContainsKey(key) ? _value : throw new KeyNotFoundException();
            set => throw new NotSupportedException();
        }

        public ICollection<TKey> Keys => new[] {_key};
        public ICollection<TValue> Values => new[] {_value};
    }
}