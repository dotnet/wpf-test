// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;
using System;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// Provides a view over a DataView data source.
    /// </summary>
    public class DataViewViewProvider : DataSourceViewProvider
    {
        #region Constructors

        public DataViewViewProvider(IEnumerable data)
            : base(data, true, true)
        {
        }

        #endregion

        #region Override Members

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            IEnumerator dataEnumerator = data.GetEnumerator();
            dataEnumerator.MoveNext();

            DataTable dataTable = new DataTable();
            dataTable.TableName = data.GetType().Name;

            PropertyDescriptor[] propertyDescriptors = GetByValueComparableProperties(dataEnumerator.Current);

            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptors)
            {
                dataTable.Columns.Add(propertyDescriptor.Name);
            }

            do
            {
                ArrayList arrayList = new ArrayList();
                foreach (PropertyDescriptor propertyDescriptor in propertyDescriptors)
                {
                    arrayList.Add(propertyDescriptor.GetValue(dataEnumerator.Current));
                }

                dataTable.Rows.Add(arrayList.ToArray());
            } while (dataEnumerator.MoveNext());

            return dataTable.DefaultView;
        }

        protected override void AddCore(object item)
        {
            PropertyDescriptor[] propertyDescriptors = GetByValueComparableProperties(item);

            ArrayList arrayList = new ArrayList();
            foreach (PropertyDescriptor propertyDescriptor in propertyDescriptors)
            {
                arrayList.Add(propertyDescriptor.GetValue(item));
            }

            (((DataView)Source).Table).Rows.Add(arrayList.ToArray());
        }

        protected override void RemoveCore(object item)
        {
            if (item.GetType() == typeof(DataRowView))
            {
                ((DataRowView)item).Delete();
            }
            else
            {
                foreach (DataRowView dataRowView in ((DataView)Source))
                {
                    if (CompareByValue(item, dataRowView))
                    {
                        dataRowView.Delete();
                        return;
                    }
                }
            }
        }

        protected override void SortCore(SortDescription sortDescription)
        {
            string sortString = sortDescription.PropertyName + " ";

            switch (sortDescription.Direction)
            {
                case ListSortDirection.Ascending:
                    sortString += "ASC";
                    break;
                case ListSortDirection.Descending:
                    sortString += "DESC";
                    break;
                default:
                    throw new ArgumentException(sortDescription.Direction.ToString() + " is not a supported ListSortDescription.", "sortDescription");
            }

            ((DataView)Source).Sort = sortString;
        }

        protected override void ClearSortCore()
        {
            ((DataView)Source).Sort = "";
        }

        protected override void FilterCore(string propertyName, FilterOperator filterOperator, object valueToFilterBy)
        {
            string filterString = propertyName + " ";

            switch (filterOperator)
            {
                case FilterOperator.NotEqual:
                    filterString += " <> ";
                    break;
                case FilterOperator.Equal:
                    filterString += " = ";
                    break;
                case FilterOperator.LessThan:
                    filterString += " < ";
                    break;
                case FilterOperator.GreaterThan:
                    filterString += " > ";
                    break;
                default:
                    throw new ArgumentException(filterOperator.ToString() + " is not a supported FilterOperator.", "filterOperator");
            }

            filterString += "'" + valueToFilterBy.ToString() + "'";

            ((DataView)Source).RowFilter = filterString;
        }

        protected override void ClearFilterCore()
        {
            ((DataView)Source).RowFilter = "";
        }

        #endregion
    }
}
