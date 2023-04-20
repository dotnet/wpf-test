// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Data;
using System.ComponentModel;
using System.Windows;


namespace Microsoft.Test.DataServices
{

    public class MyCollView : ICollectionView
    {
        IEnumerable _ds;
        int _currentindex;
        CultureInfo _culture;
        Predicate<object> _filter;
        String _datasetsort;
        String _datasetfilter;

        public MyCollView(IEnumerable ds) : base()
        {
            _currentindex = -1;
            _ds = ds;
            _datasetsort = "";
            _datasetfilter = "";
            _culture = new CultureInfo("en-US");

            INotifyCollectionChanged incc = ds as INotifyCollectionChanged;
            if (incc != null)
            {
                incc.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
            }


        }

        #region ICollectionView Members
        public bool CanFilter
        {
            get { return true ; }
        }

        public bool CanSort
        {
            get { return false; }
        }

        public bool Contains(object item)
        {
            return ((IBindingList)_ds).Contains(item);
        }

        public bool IsEmpty
        {
            get { return (((IBindingList)_ds).Count == 0); }
        }

        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }

        public object CurrentItem
        {
            get {
                if (this.IsCurrentAfterLast || this.IsCurrentBeforeFirst)
                    return null;
//                DataRowView drv = ((DataView)this._ds)[_currentindex];
                return ((Object)((DataView)this._ds)[_currentindex]);
            }
        }

        public int CurrentPosition
        {
            get { return _currentindex; }
        }

        public IDisposable DeferRefresh()
        {
            return null;
        }
        public Predicate<object> Filter
        {
            get { return _filter; }
            set { if (!CanFilter)
                    throw new NotSupportedException();
                _filter = value;
            }
        }

        /// <summary>
        /// Returns true if this view really supports grouping.
        /// When this returns false, the rest of the interface is ignored.
        /// </summary>
        public virtual bool CanGroup
        {
            get { return false; }
        }

        /// <summary>
        /// The description of grouping, indexed by level.
        /// </summary>
        public virtual ObservableCollection<GroupDescription> GroupDescriptions
        {
            get { return null; }
        }

        /// <summary>
        /// The top-level groups, constructed according to the descriptions
        /// given in GroupDescriptions.
        /// </summary>
        public virtual ReadOnlyObservableCollection<object> Groups
        {
            get { return null; }
        }

        #region Cursor Functions
        public bool IsCurrentAfterLast
        {
            get { return _currentindex > ((DataView)this._ds).Table.Rows.Count -1 ; }
        }

        public bool IsCurrentBeforeFirst
        {
            get { return _currentindex < 0; }
        }

        public bool MoveCurrentTo(object item)
        {
            if (item != null)
            {
                OnCurrentChanging(this, new CurrentChangingEventArgs());
                _currentindex = this.IndexOf(item);
                OnCurrentChanged(this, new EventArgs());
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool MoveCurrentToFirst()
        {
            OnCurrentChanging(this, new CurrentChangingEventArgs());
            _currentindex = 0;
            OnCurrentChanged(this, new EventArgs());
            return true;
        }

        public bool MoveCurrentToLast()
        {
            OnCurrentChanging(this, new CurrentChangingEventArgs());
            _currentindex = ((DataView)this._ds).Table.Rows.Count -1 ;
            OnCurrentChanged(this, new EventArgs());
            return true;
        }

        public bool MoveCurrentToNext()
        {
            if (_currentindex <= ((DataView)this._ds).Table.Rows.Count)
            {
                OnCurrentChanging(this, new CurrentChangingEventArgs());
                _currentindex++;
                OnCurrentChanged(this, new EventArgs());
                return true;
            }
            return false;

        }

        public bool MoveCurrentToPosition(int position)
        {
            OnCurrentChanging(this, new CurrentChangingEventArgs());
            _currentindex = position;
            OnCurrentChanged(this, new EventArgs());

            return _currentindex == position;
        }

        public bool MoveCurrentToPrevious()
        {
            if (_currentindex > -1)
            {
                OnCurrentChanging(this, new CurrentChangingEventArgs());
                _currentindex--;
                OnCurrentChanged(this, new EventArgs());
                return true;
            }

            return false;
        }

        public int IndexOf(object item)
        {
            return ((DataView)this._ds).Table.Rows.IndexOf(((DataRowView)item).Row);
        }

        #endregion


        public void Refresh()
        {
            OnCurrentChanging(this, new CurrentChangingEventArgs());
            _currentindex = -1;
            OnCurrentChanged(this, new EventArgs());

            if (this.CanFilter)
            {
                // This does nothing but hit code coverage.
                 foreach(object o in _ds)
                    this.Filter(o);
            }
            if (this.CanSort)
                ((DataView)_ds).Sort = _datasetsort;
            OnCollectionChanged(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }


        public int Count
        {
            get { return ((DataView)this._ds).Table.Rows.Count; }
        }

        // Currently not used
        public string DataSetSort
        {
            get { return _datasetsort;}
            set { _datasetsort = value;}
        }

        //Currently not used
        public string DataSetFilter
        {
            get { return _datasetfilter; }
            set { _datasetfilter = value; }
        }

        public SortDescriptionCollection SortDescriptions
        {
            get { return SortDescriptionCollection.Empty; }
        }

        public IEnumerable SourceCollection
        {
            get { return this._ds; }
        }


        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this._ds.GetEnumerator();
        }

        #endregion

        #region Events

        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler CurrentChanged;
        public event CurrentChangingEventHandler CurrentChanging;


        protected void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged!=null)
                CollectionChanged(sender,args);
        }

        protected void OnCurrentChanging(object sender, CurrentChangingEventArgs args)
        {
            if (CurrentChanging != null)
                CurrentChanging(sender, args);
        }

        protected void OnCurrentChanged(object sender, EventArgs args)
        {
            if (CurrentChanged != null)
                CurrentChanged(sender, args);
        }


        #endregion


    }




}
