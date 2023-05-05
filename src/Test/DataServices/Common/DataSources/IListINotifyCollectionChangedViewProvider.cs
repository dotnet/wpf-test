// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    public class IListINotifyCollectionChangedViewProvider : DataSourceViewProvider
    {
        #region Constructors

        public IListINotifyCollectionChangedViewProvider(IEnumerable data)
            : base(data, true, true)
        {
        }

        #endregion

        #region Override Members

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            return new IListINotifyCollectionChangedDataSource(data);
        }

        protected override void AddCore(object item)
        {
            ((IListINotifyCollectionChangedDataSource)Source).Add(item);
        }

        protected override void RemoveCore(object item)
        {
            ((IListINotifyCollectionChangedDataSource)Source).Remove(item);
        }

        #endregion

        private class IListINotifyCollectionChangedDataSource : IList, INotifyCollectionChanged
        {
            #region Private Data

            private ObservableCollection<object> _backingCollection;

            #endregion

            #region Constructors

            public IListINotifyCollectionChangedDataSource(IEnumerable data)
            {
                _backingCollection = new ObservableCollection<object>();
                foreach (object dataItem in data)
                {
                    _backingCollection.Add(dataItem);
                }
            }

            #endregion

            #region Public and Protected Members

            public IEnumerator GetEnumerator()
            {
                return _backingCollection.GetEnumerator();
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    _backingCollection.CollectionChanged += value;
                }
                remove
                {
                    _backingCollection.CollectionChanged -= value;
                }
            }

            #endregion

            #region IList Members

            public int Add(object value)
            {
                _backingCollection.Add(value);
                return IndexOf(value);
            }

            public void Clear()
            {
                _backingCollection.Clear();
            }

            public bool Contains(object value)
            {
                return _backingCollection.Contains(value);
            }

            public int IndexOf(object value)
            {
                return _backingCollection.IndexOf(value);
            }

            public void Insert(int index, object value)
            {
                _backingCollection.Insert(index, value);
            }

            public bool IsFixedSize
            {
                get { return ((IList)_backingCollection).IsFixedSize; }
            }

            public bool IsReadOnly
            {
                get { return ((IList)_backingCollection).IsReadOnly; }
            }

            public void Remove(object value)
            {
                _backingCollection.Remove(value);
            }

            public void RemoveAt(int index)
            {
                _backingCollection.RemoveAt(index);
            }

            public object this[int index]
            {
                get
                {
                    return _backingCollection[index];
                }
                set
                {
                    _backingCollection[index] = value;
                }
            }

            #endregion

            #region ICollection Members

            public void CopyTo(System.Array array, int index)
            {
                _backingCollection.CopyTo((object[])array, index);
            }

            public int Count
            {
                get { return _backingCollection.Count; }
            }

            public bool IsSynchronized
            {
                get { return ((ICollection)_backingCollection).IsSynchronized; }
            }

            public object SyncRoot
            {
                get { return ((ICollection)_backingCollection).SyncRoot; }
            }

            #endregion
        }
    }
}
