// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Test.DataServices
{
    public class FullBindingList<T> : BindingList<T>, IBindingListView, ITypedList
    {
        public int OriginalCount
        {
            get { return _baseList.Count; }
        }

        public T GetOriginalItem(int index)
        {
            return _baseList[index];
        }

        #region Collection<T> overrides

        protected override void ClearItems()
        {
            _baseList.Clear();
            base.ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            T item = Items[index];
            _baseList.Remove(item);

            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, T item)
        {
            _baseList.Add(item);
            if (PassesFilter(item))
            {
                base.InsertItem(FindIndex(index, item), item);
            }
        }

        protected override void SetItem(int index, T item)
        {
            int baseIndex = _baseList.IndexOf(Items[index]);
            _baseList[baseIndex] = item;
            base.SetItem(index, item);
        }

        #endregion Collection<T> overrides

        #region BindingList<T> overrides

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            ListSortDescription[] lsd = new ListSortDescription[1]{new ListSortDescription(prop, direction)};
            _sortDescriptions = new ListSortDescriptionCollection(lsd);
            RebuildItems();
        }

        protected override void RemoveSortCore()
        {
            _sortDescriptions = null;
            RebuildItems();
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        #endregion BindingList<T> overrides

        #region IBindingListView

        string IBindingListView.Filter
        {
            get { return _filterString; }
            set
            {
                _filterString = value;
                PrepareFilter();
                RebuildItems();
            }
        }

        ListSortDescriptionCollection IBindingListView.SortDescriptions
        {
            get { return _sortDescriptions; }
        }

        bool IBindingListView.SupportsAdvancedSorting
        {
            get { return true; }
        }

        bool IBindingListView.SupportsFiltering
        {
            get { return true; }
        }

        void IBindingListView.ApplySort(ListSortDescriptionCollection sorts)
        {
            _sortDescriptions = sorts;
            RebuildItems();
        }

        void IBindingListView.RemoveFilter()
        {
            _filterString = null;
            PrepareFilter();
            RebuildItems();
        }

        #endregion IBindingListView

        #region ITypedList

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (listAccessors != null)
                throw new NotSupportedException("I have no idea what this means");

            return TypeDescriptor.GetProperties(typeof(T));
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return "BookList";
        }

        #endregion ITypedList

        #region Helper methods

        private void RebuildItems()
        {
            bool oldRaiseListChangedEvents = this.RaiseListChangedEvents;
            this.RaiseListChangedEvents = false;

            // filter the base list
            int n = 0;
            base.ClearItems();
            foreach (T item in _baseList)
            {
                if (PassesFilter(item))
                {
                    base.InsertItem(n++, item);
                }
            }

            // sort the result
            if (_sortDescriptions != null)
            {
                // insertion sort
                for (int i=1; i<Items.Count; ++i)
                {
                    T item = Items[i];

                    int j;
                    for (j=i-1; j>=0; --j)
                    {
                        if (Compare(Items[j], item) <= 0)
                            break;

                        Items[j+1] = Items[j];
                    }

                    Items[j+1] = item;
                }
            }

            this.RaiseListChangedEvents = oldRaiseListChangedEvents;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, null));
        }

        private int FindIndex(int index, T item)
        {
            if (_sortDescriptions == null)
                return index;

            int low = 0, high = Items.Count;
            while (low < high)
            {
                int mid = (low + high) / 2;
                if (Compare(Items[mid], item) < 0)
                    low = mid + 1;
                else
                    high = mid;
            }

            return low;
        }

        private int Compare(T item1, T item2)
        {
            for (int i=0; i<_sortDescriptions.Count; ++i)
            {
                object value1 = _sortDescriptions[i].PropertyDescriptor.GetValue(item1);
                object value2 = _sortDescriptions[i].PropertyDescriptor.GetValue(item2);
                int result = _comparer.Compare(value1, value2);

                if (result != 0)
                {
                    if (_sortDescriptions[i].SortDirection == ListSortDirection.Ascending)
                        return result;
                    else
                        return result;
                }
            }

            return 0;
        }

        private bool PassesFilter(T item)
        {
            if (_filterPD == null)
                return true;

            object value = _filterPD.GetValue(item);
            int comp = _comparer.Compare(value, _filterValue);

            switch (_filterOp)
            {
                case "<":   return (comp < 0);
                case "<=":  return (comp <= 0);
                case ">":   return (comp > 0);
                case ">=":  return (comp >= 0);
                case "==":  return (comp == 0);
                case "<>":  return (comp != 0);
            }

            return true;
        }

        private void PrepareFilter()
        {
            if (String.IsNullOrEmpty(_filterString))
            {
                _filterPD = null;
                _filterOp = null;
                _filterValue = null;
                return;
            }

            int index;
            int opIndex = Int32.MaxValue;

            // find the operator
            index = _filterString.IndexOf('<');
            if (0 <= index && index < opIndex)  opIndex = index;
            index = _filterString.IndexOf('>');
            if (0 <= index && index < opIndex) opIndex = index;
            index = _filterString.IndexOf('=');
            if (0 <= index && index < opIndex)  opIndex = index;
            
            int opLength = (_filterString[opIndex + 1] == '=') ? 2 : 1;
            opLength = (_filterString[opIndex + 1] == '>') ? 2 : 1;

            _filterOp = _filterString.Substring(opIndex, opLength);

            // find the property descriptor
            string propertyName = _filterString.Substring(0, opIndex).Trim();
            _filterPD = TypeDescriptor.GetProperties(typeof(T))[propertyName];

            // find the value
            int intValue;
            double doubleValue;
            string valueString = _filterString.Substring(opIndex + opLength).Trim();
            if (valueString.StartsWith("'") && valueString.EndsWith("'"))
            {
                _filterValue = valueString.Substring(1, valueString.Length-2);
            }
            else if (Int32.TryParse(valueString, out intValue))
            {
                _filterValue = intValue;
            }
            else if (Double.TryParse(valueString, out doubleValue))
            {
                _filterValue = doubleValue;
            }
            else
            {
                _filterValue = null;
            }
        }

        #endregion Helper methods

        List<T> _baseList = new List<T>();
        ListSortDescriptionCollection _sortDescriptions;
        Comparer _comparer = new Comparer(CultureInfo.InvariantCulture);

        string _filterString;
        PropertyDescriptor _filterPD;
        object _filterValue;
        string _filterOp;
    }
}
