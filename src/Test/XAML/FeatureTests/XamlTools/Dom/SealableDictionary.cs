// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq; //this is only used for _baseDictionary.Contains().

namespace Microsoft.Xaml.Tools.XamlDom
{
    internal class SealableDictionary<K, V> : IDictionary<K, V>
    {
        public void Seal()
        {
            _isSealed = true;
        }

        public bool IsSealed { get { return _isSealed; } }

        #region IDictionary<K,V> Members

        void IDictionary<K, V>.Add(K key, V value)
        {
            CheckSealed();
            _baseDictionary.Add(key, value);
        }

        bool IDictionary<K, V>.ContainsKey(K key)
        {
            return _baseDictionary.ContainsKey(key);
        }

        ICollection<K> IDictionary<K, V>.Keys
        {
            get { return _baseDictionary.Keys; }
        }

        bool IDictionary<K, V>.Remove(K key)
        {
            CheckSealed();
            return _baseDictionary.Remove(key);
        }

        public bool TryGetValue(K key, out V value)
        {
            return _baseDictionary.TryGetValue(key, out value);
        }

        ICollection<V> IDictionary<K, V>.Values
        {
            get { return _baseDictionary.Values; }
        }

        public V this[K key]
        {
            get
            {
                return _baseDictionary[key];
            }
            set
            {
                CheckSealed();
                _baseDictionary[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<K,V>> Members

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
        {
            CheckSealed();
            ((ICollection<KeyValuePair<K, V>>)_baseDictionary).Add(item);
        }

        void ICollection<KeyValuePair<K, V>>.Clear()
        {
            CheckSealed();
            _baseDictionary.Clear();
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
        {
            return _baseDictionary.Contains(item);
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            CheckSealed();
            ((ICollection<KeyValuePair<K, V>>)_baseDictionary).CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<K, V>>.Count
        {
            get { return _baseDictionary.Count; }
        }

        bool ICollection<KeyValuePair<K, V>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<K, V>>)_baseDictionary).IsReadOnly; }
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        {
            CheckSealed();
            return ((ICollection<KeyValuePair<K, V>>)_baseDictionary).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<K,V>> Members

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            return _baseDictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _baseDictionary.GetEnumerator();
        }

        #endregion

        private void CheckSealed()
        {
            if (IsSealed)
            {
                throw new NotSupportedException("The Dictionary has been sealed.");
            }
        }

        private Dictionary<K, V> _baseDictionary = new Dictionary<K, V>();
        private bool _isSealed;

    }
}
