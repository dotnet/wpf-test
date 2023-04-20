// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;

namespace Microsoft.Test.DataServices
{
    public class IListViewProvider : DataSourceViewProvider
    {
        #region Constructors

        public IListViewProvider(IEnumerable data)
            : base(data, true, true)
        {
        }

        #endregion

        #region Override Members

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            return new IListDataSource(data);
        }

        protected override void AddCore(object item)
        {
            ((IListDataSource)Source).Add(item);
        }

        protected override void RemoveCore(object item)
        {
            ((IListDataSource)Source).Remove(item);
        }

        #endregion

        private class IListDataSource : IList
        {
            #region Private Data

            private ObservableCollection<object> _backingCollection;

            #endregion

            #region Constructors

            public IListDataSource(IEnumerable data)
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
