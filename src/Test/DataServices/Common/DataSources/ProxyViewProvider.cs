// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Specialized;
using System;
using System.Windows.Controls;

namespace Microsoft.Test.DataServices
{
    public class ProxyViewProvider<T> : DataSourceViewProvider where T : DataSourceViewProvider
    {
        private T _dsvp;

        #region Constructors

        public ProxyViewProvider(IEnumerable data)
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
                    CollectionViewSource cvs = new CollectionViewSource();
                    ((ISupportInitialize)cvs).BeginInit();
                    cvs.CollectionViewType = typeof(CustomCollectionView);
                    cvs.Source = Source;
                    ((ISupportInitialize)cvs).EndInit();
                    ItemsControl ic = new ItemsControl();
                    Binding b = new Binding();
                    b.Source = cvs;
                    ic.SetBinding(ItemsControl.ItemsSourceProperty, b);
                    _view = (ICollectionView)ic.ItemsSource;
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

        private ICollectionView _view;

        public class CustomCollectionView : ICollectionView, IItemProperties, IEditableCollectionView
        {
            ICollectionView _backingCollectionView;
            IEditableCollectionView _backingIEditableCollectionView;

            public CustomCollectionView(IEnumerable collection)
            {
                _backingCollectionView = CollectionViewSource.GetDefaultView(collection);
                _backingIEditableCollectionView = (IEditableCollectionView)_backingCollectionView;
            }

            #region ICollectionView Members

            public bool CanFilter
            {
                get { return _backingCollectionView.CanFilter; }
            }

            public bool CanGroup
            {
                get { return _backingCollectionView.CanGroup; }
            }

            public bool CanSort
            {
                get { return _backingCollectionView.CanSort; }
            }

            public bool Contains(object item)
            {
                return _backingCollectionView.Contains(item);
            }

            public System.Globalization.CultureInfo Culture
            {
                get
                {
                    return _backingCollectionView.Culture;
                }
                set
                {
                    _backingCollectionView.Culture = value;
                }
            }

            public event EventHandler CurrentChanged
            {
                add
                {
                    _backingCollectionView.CurrentChanged += value;
                }
                remove
                {
                    _backingCollectionView.CurrentChanged -= value;
                }
            }

            public event CurrentChangingEventHandler CurrentChanging
            {
                add
                {
                    _backingCollectionView.CurrentChanging += value;
                }
                remove
                {
                    _backingCollectionView.CurrentChanging -= value;
                }
            }

            public object CurrentItem
            {
                get { return _backingCollectionView.CurrentItem; }
            }

            public int CurrentPosition
            {
                get { return _backingCollectionView.CurrentPosition; }
            }

            public System.IDisposable DeferRefresh()
            {
                return _backingCollectionView.DeferRefresh();
            }

            public System.Predicate<object> Filter
            {
                get
                {
                    return _backingCollectionView.Filter;
                }
                set
                {
                    _backingCollectionView.Filter = value;
                }
            }

            public ObservableCollection<GroupDescription> GroupDescriptions
            {
                get { return _backingCollectionView.GroupDescriptions; }
            }

            public ReadOnlyObservableCollection<object> Groups
            {
                get { return _backingCollectionView.Groups; }
            }

            public bool IsCurrentAfterLast
            {
                get { return _backingCollectionView.IsCurrentAfterLast; }
            }

            public bool IsCurrentBeforeFirst
            {
                get { return _backingCollectionView.IsCurrentBeforeFirst; }
            }

            public bool IsEmpty
            {
                get { return _backingCollectionView.IsEmpty; }
            }

            public bool MoveCurrentTo(object item)
            {
                return _backingCollectionView.MoveCurrentTo(item);
            }

            public bool MoveCurrentToFirst()
            {
                return _backingCollectionView.MoveCurrentToFirst();
            }

            public bool MoveCurrentToLast()
            {
                return _backingCollectionView.MoveCurrentToLast();
            }

            public bool MoveCurrentToNext()
            {
                return _backingCollectionView.MoveCurrentToNext();
            }

            public bool MoveCurrentToPosition(int position)
            {
                return _backingCollectionView.MoveCurrentToPosition(position);
            }

            public bool MoveCurrentToPrevious()
            {
                return _backingCollectionView.MoveCurrentToPrevious();
            }

            public void Refresh()
            {
                _backingCollectionView.Refresh();
            }

            public SortDescriptionCollection SortDescriptions
            {
                get { return _backingCollectionView.SortDescriptions; }
            }

            public IEnumerable SourceCollection
            {
                get { return _backingCollectionView.SourceCollection; }
            }

            #endregion

            #region IEnumerable Members

            public IEnumerator GetEnumerator()
            {
                return _backingCollectionView.GetEnumerator();
            }

            #endregion

            #region INotifyCollectionChanged Members

            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    _backingCollectionView.CollectionChanged += value;
                }
                remove
                {
                    _backingCollectionView.CollectionChanged -= value;
                }
            }

            #endregion

            #region IItemProperties Members

            public ReadOnlyCollection<ItemPropertyInfo> ItemProperties
            {
                get
                {
                    if (_backingCollectionView as IItemProperties != null)
                    {
                        return ((IItemProperties)_backingCollectionView).ItemProperties;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            #endregion

            #region IEditableCollectionView Members

            public object AddNew()
            {
                return _backingIEditableCollectionView.AddNew();
            }

            public bool CanAddNew
            {
                get { return _backingIEditableCollectionView.CanAddNew; }
            }

            public bool CanCancelEdit
            {
                get { return _backingIEditableCollectionView.CanCancelEdit; }
            }

            public bool CanRemove
            {
                get { return _backingIEditableCollectionView.CanRemove; }
            }

            public void CancelEdit()
            {
                _backingIEditableCollectionView.CancelEdit();
            }

            public void CancelNew()
            {
                _backingIEditableCollectionView.CancelNew();
            }

            public void CommitEdit()
            {
                _backingIEditableCollectionView.CommitEdit();
            }

            public void CommitNew()
            {
                _backingIEditableCollectionView.CommitNew();
            }

            public object CurrentAddItem
            {
                get { return _backingIEditableCollectionView.CurrentAddItem; }
            }

            public object CurrentEditItem
            {
                get { return _backingIEditableCollectionView.CurrentEditItem; }
            }

            public void EditItem(object item)
            {
                _backingIEditableCollectionView.EditItem(item);
            }

            public bool IsAddingNew
            {
                get { return _backingIEditableCollectionView.IsAddingNew; }
            }

            public bool IsEditingItem
            {
                get { return _backingIEditableCollectionView.IsEditingItem; }
            }

            public NewItemPlaceholderPosition NewItemPlaceholderPosition
            {
                get
                {
                    return _backingIEditableCollectionView.NewItemPlaceholderPosition;
                }
                set
                {
                    _backingIEditableCollectionView.NewItemPlaceholderPosition = value;
                }
            }

            public void Remove(object item)
            {
                _backingIEditableCollectionView.Remove(item);
            }

            public void RemoveAt(int index)
            {
                _backingIEditableCollectionView.RemoveAt(index);
            }

            #endregion
        }

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
