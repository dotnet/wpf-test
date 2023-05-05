// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System;
using Microsoft.Test.Threading;

namespace Microsoft.Test.DataServices
{
    public class ItemsCollectionViewProvider<T> : DataSourceViewProvider where T : DataSourceViewProvider
    {
        #region Private Data

        private T _dsvp;
        private ICollectionView _view;

        #endregion

        #region Constructors

        public ItemsCollectionViewProvider(IEnumerable data)
            : base(data, ((T)Activator.CreateInstance(typeof(T), new Object[] { data })).CanAddRemove,
                         ((T)Activator.CreateInstance(typeof(T), new Object[] { data })).CanFilter)
        {
        }

        #endregion

        #region Override Members

        public override ICollectionView View
        {
            get
            {
                if (_view == null)
                {
                    ItemsControl itemsControl = new ItemsControl();
                    Binding b = new Binding();
                    if (Source == null)
                        return null;
                    b.Source = Source;
                    itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, b);
                    _view = CollectionViewSource.GetDefaultView(itemsControl.Items);
                }
                return _view;
            }
        }

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            _dsvp = (T)Activator.CreateInstance(typeof(T), new Object[] { data });
            return _dsvp.Source;
        }

        protected override void AddCore(object item)
        {
            _dsvp.Add(item);
        }

        protected override void RemoveCore(object item)
        {
            _dsvp.Remove(item);
        }

        #endregion
    }
}
