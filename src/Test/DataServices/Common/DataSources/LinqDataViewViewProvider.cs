// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
    public class LinqDataViewViewProvider : DataViewViewProvider
    {
        #region Constructors

        public LinqDataViewViewProvider(IEnumerable data)
            : base(data)
        {
        }

        #endregion

        #region Override Members

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            DataView dataView = (DataView)base.CreateDataSource(data);

            // Change to ToLinqDataView
            return dataView;
        }

        protected override void SortCore(SortDescription sortDescription)
        {
            // Change to ApplySort
            base.SortCore(sortDescription);
        }

        protected override void FilterCore(string propertyName, FilterOperator filterOperator, object valueToFilterBy)
        {
            // Change to Predicate
            base.FilterCore(propertyName, filterOperator, valueToFilterBy);
        }

        #endregion
    }
}
