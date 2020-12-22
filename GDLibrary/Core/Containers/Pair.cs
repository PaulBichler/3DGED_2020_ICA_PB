using System;
using System.Collections.Generic;

namespace GDLibrary.Containers
{
    /// <summary>
    /// Generic key-value pair class for C# that provide set property for key and value, usable after initialization
    /// </summary>
    /// <typeparam name="K">Value or reference type</typeparam>
    /// <typeparam name="V">Value or reference type</typeparam>
    public class Pair<K, V>
    {
        #region Fields
        private K key;
        private V value;
        #endregion Fields

        #region Properties
        public K Key { get => key; set => key = value; }
        public V Value { get => value; set => this.value = value; }
        #endregion Properties

        #region Constructors & Core

        public Pair(K key, V value)
        {
            Key = key;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Pair<K, V> pair &&
                   EqualityComparer<K>.Default.Equals(key, pair.key) &&
                   EqualityComparer<V>.Default.Equals(value, pair.value) &&
                   EqualityComparer<K>.Default.Equals(Key, pair.Key) &&
                   EqualityComparer<V>.Default.Equals(Value, pair.Value);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(key, value, Key, Value);
        }

        #endregion Constructors & Core
    }
}