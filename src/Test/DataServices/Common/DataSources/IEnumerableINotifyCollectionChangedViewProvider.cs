// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    public class IEnumerableINotifyCollectionChangedViewProvider : DataSourceViewProvider
    {
        #region Constructors

        public IEnumerableINotifyCollectionChangedViewProvider(IEnumerable data)
            : base(data, true, true)
        {
        }

        #endregion

        #region Override Members

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            return new IEnumerableINotifyCollectionChangedDataSource(data);
        }

        protected override void AddCore(object item)
        {
            ((IEnumerableINotifyCollectionChangedDataSource)Source).Add(item);
        }

        protected override void RemoveCore(object item)
        {
            ((IEnumerableINotifyCollectionChangedDataSource)Source).Remove(item);
        }

        #endregion

        private class IEnumerableINotifyCollectionChangedDataSource : IEnumerable, INotifyCollectionChanged
        {
            #region Private Data

            private ObservableCollection<object> _backingCollection;

            #endregion

            #region Constructors

            public IEnumerableINotifyCollectionChangedDataSource(IEnumerable data)
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

            public void Add(object o)
            {
                _backingCollection.Add(o);
            }

            public void Remove(object o)
            {
                if (_backingCollection.Contains(o))
                {
                    _backingCollection.Remove(o);
                }
            }

            #endregion
        }
    }
}
