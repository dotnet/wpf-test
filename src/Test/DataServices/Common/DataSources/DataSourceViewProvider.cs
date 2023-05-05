// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
    public enum FilterOperator
    {
        NotEqual,
        Equal,
        LessThan,
        GreaterThan
    }

    /// <summary>
    /// DataSourceViewProvider provides an abstraction of the type of data
    /// source and collection view. A consumer can create a particular
    /// DataSourceViewProvider with an IEnumerable collection of objects, and
    /// the DataSourceViewProvider will create a representation of that data
    /// in the appropriate type of data source. It also abstracts the type of
    /// collection view. Using a DataSourceViewProvider, a consumer can add or
    /// remove items, filtering, or sort using a canonical form that does not
    /// require knowledge of the underlying type of data source.
    /// 
    /// Sample usage:
    /// DataSourceViewProvider provider = DataSourceViewProvider.CreateDataSourceViewProvider(typeof(XmlViewProvider), new Places());
    /// Place placeToAdd = new Place("Brea", "CA");
    /// provider.Add(placeToAdd);
    /// provider.Filter("State", FilterOperator.Equal, "WA");
    /// 
    /// The XmlViewProvider will create an XmlElement representating that data.
    /// The DataViewViewProvider will create a DataRow representating that
    /// data. An ObservableCollectionViewProvider will simply add that object
    /// to it's collection. For the filter, if the data source is a DataView
    /// it will set the Filter string, if it is a LinqDataView it will set
    /// the Predicate lambda expression, etc. All of this is transparent to the
    /// consumer.
    /// </summary>
    public abstract class DataSourceViewProvider
    {
        #region Private Data

        private IEnumerable _data;
        private IEnumerable _source;
        private ICollectionView _view;
        private bool _canAddRemove;
        private bool _canFilter;

        #endregion

        #region Constructors

        protected DataSourceViewProvider(IEnumerable data, bool canAddRemove, bool canFilter)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            this._data = data;
            this._canAddRemove = canAddRemove;
            this._canFilter = canFilter;
        }

        #endregion

        #region Public and Protected Members

        public IEnumerable Source
        {
            get
            {
                if (_source == null)
                {
                    _source = CreateDataSource(_data);
                }
                return _source;
            }
        }

        public virtual ICollectionView View
        {
            get
            {
                if (_view == null)
                {
                    _view = CollectionViewSource.GetDefaultView(Source);
                }
                return _view;
            }
        }

        public bool CanAddRemove
        {
            get { return _canAddRemove; }
        }

        public bool CanSort
        {
            get { return View.CanSort; }
        }

        public bool CanFilter
        {
            get { return _canFilter; }
        }

        public void Add(object item)
        {
            AddCore(item);
        }

        protected virtual void AddCore(object item)
        {
            if (CanAddRemove)
            {
                throw new NotImplementedException("Either AddCore has not been implemented for this DataSourceViewProvider, or this DataSourceViewProvider is calling base.AddCore.");
            }
            else
            {
                throw new NotSupportedException("Add is not supported for this DataSourceViewProvider.");
            }
        }

        public void Remove(object item)
        {
            RemoveCore(item);
        }

        protected virtual void RemoveCore(object item)
        {
            if (CanAddRemove)
            {
                throw new NotImplementedException("Either RemoveCore has not been implemented for this DataSourceViewProvider, or this DataSourceViewProvider is calling base.RemoveCore.");
            }
            else
            {
                throw new InvalidOperationException("Remove is not supported for this DataSourceViewProvider.");
            }
        }

        public void Sort(SortDescription sortDescription)
        {
            SortCore(sortDescription);
        }

        protected virtual void SortCore(SortDescription sortDescription)
        {
            View.SortDescriptions.Clear();
            View.SortDescriptions.Add(sortDescription);
        }

        public void ClearSort()
        {
            ClearSortCore();
        }

        protected virtual void ClearSortCore()
        {
            View.SortDescriptions.Clear();
        }

        public void Filter(string propertyName, FilterOperator filterOperator, object valueToFilterBy)
        {
            FilterCore(propertyName, filterOperator, valueToFilterBy);
        }

        protected virtual void FilterCore(string propertyName, FilterOperator filterOperator, object valueToFilterBy)
        {
            View.Filter = new Predicate<object>(delegate(object objectToFilter)
            {
                if (objectToFilter == null)
                {
                    return false;
                }

                // When the property name string is null or empty, we compare
                // the object itself. (Consistent with Path="" in Data Binding)
                if (String.IsNullOrEmpty(propertyName))
                {
                    return DoesValuePassFilter(objectToFilter, valueToFilterBy, filterOperator);
                }
                else
                {
                    PropertyDescriptorCollection propertyDescriptorCollection = ConvertToCanonical(objectToFilter);
                    PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[propertyName];

                    if (propertyDescriptor != null)
                    {
                        object propertyValue = propertyDescriptor.GetValue(objectToFilter);
                        return DoesValuePassFilter(propertyValue, valueToFilterBy, filterOperator);
                    }
                    else
                    {
                        throw new Exception("Could not find the property " + propertyName + " on type " + objectToFilter.GetType().ToString() + ".");
                    }
                }
            });
        }

        public void ClearFilter()
        {
            ClearFilterCore();
        }

        protected virtual void ClearFilterCore()
        {
            View.Filter = null;
        }

        public bool CompareByValue(object primaryForm, object dataSourceForm)
        {
            if (primaryForm == null)
            {
                throw new ArgumentNullException("primaryForm");
            }
            if (dataSourceForm == null)
            {
                throw new ArgumentNullException("dataSourceForm");
            }

            bool valuesAreEqual = true;

            PropertyDescriptor[] propertyDescriptorCollection = GetByValueComparableProperties(primaryForm);
            PropertyDescriptorCollection dataSourcePropertyDescriptorCollection = ConvertToCanonical(dataSourceForm);

            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
            {
                object propertyValue = propertyDescriptor.GetValue(primaryForm);

                PropertyDescriptor dataSourcePropertyDescriptor = dataSourcePropertyDescriptorCollection[propertyDescriptor.Name];

                if (dataSourcePropertyDescriptor == null)
                {
                    throw new ArgumentException("The property " + propertyDescriptor.Name + " cannot be found on " + dataSourceForm.GetType().ToString(), "dataSourceForm");
                }

                object dataSourceValue = dataSourcePropertyDescriptor.GetValue(dataSourceForm);

                if (propertyValue == null)
                {
                    if (dataSourceValue != null)
                    {
                        valuesAreEqual = false;
                        break;
                    }
                }
                else if (!propertyValue.Equals(dataSourceValue))
                {
                    valuesAreEqual = false;
                    break;
                }
            }

            return valuesAreEqual;
        }

        public virtual PropertyDescriptorCollection ConvertToCanonical(object item)
        {
            return TypeDescriptor.GetProperties(item);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "data", Justification="This is an abstract member so we do not have private field name collisions.")]
        protected abstract IEnumerable CreateDataSource(IEnumerable data);

        #endregion

        #region Static Members

        public static DataSourceViewProvider CreateDataSourceViewProvider(Type dataSourceViewProviderType, IEnumerable data)
        {
            if (dataSourceViewProviderType == null)
            {
                throw new ArgumentNullException("dataSourceViewProviderType");
            }
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (!dataSourceViewProviderType.IsSubclassOf(typeof(DataSourceViewProvider)))
            {
                throw new ArgumentException("DataSourceViewProviderType must be a subclass of DataSourceViewProvider.");
            }

            // Subclasses of DataSourceViewProvider should implement a constructor that takes an IEnumerable.
            // Unfortunately, there is no way at compilation time to enforce this. If a subclass fails to provide
            // said constructor, this call will result in a MissingMethodException.
            return (DataSourceViewProvider)Activator.CreateInstance(dataSourceViewProviderType, new object[] { data });
        }

        protected static bool DoesValuePassFilter(object valueToEvaluate, object valueToFilterBy, FilterOperator filterOperator)
        {
            if (valueToEvaluate == null)
            {
                throw new ArgumentNullException("valueToEvaluate");
            }
            if (valueToFilterBy == null)
            {
                throw new ArgumentNullException("valueToFilterBy");
            }

            if (filterOperator == FilterOperator.Equal)
            {
                return valueToEvaluate.Equals(valueToFilterBy);
            }
            if (filterOperator == FilterOperator.NotEqual)
            {
                return !valueToEvaluate.Equals(valueToFilterBy);
            }

            IComparable itemComparable = valueToEvaluate as IComparable;
            IComparable valueComparable = valueToFilterBy as IComparable;

            if (itemComparable == null)
            {
                throw new System.ArgumentException(filterOperator.ToString() + " requires types to implement IComparable.", "valueToEvaluate");
            }
            if (valueComparable == null)
            {
                throw new System.ArgumentException(filterOperator.ToString() + " requires types to implement IComparable.", "valueToFilterBy");
            }

            if (filterOperator == FilterOperator.LessThan)
            {
                return itemComparable.CompareTo(valueComparable) < 0;
            }
            if (filterOperator == FilterOperator.GreaterThan)
            {
                return itemComparable.CompareTo(valueComparable) > 0;
            }

            throw new ArgumentException(filterOperator.ToString() + " is not supported.", "filterOperator");
        }

        protected static PropertyDescriptor[] GetByValueComparableProperties(object item)
        {
            PropertyDescriptorCollection propertyDescriptorCollection = TypeDescriptor.GetProperties(item);
            ArrayList propertyDescriptors = new ArrayList();
            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptorCollection)
            {
                if (propertyDescriptor.Attributes.Contains(new ComparableByValueAttribute()))
                {
                    propertyDescriptors.Add(propertyDescriptor);
                }
            }
            return (PropertyDescriptor[])propertyDescriptors.ToArray(typeof(PropertyDescriptor));
        }

        #endregion
    }
}
