// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Test.DataServices
{
    public class BindingListViewProvider<T> : DataSourceViewProvider
    {
        #region Constructors

        public BindingListViewProvider(IEnumerable data)
            : base(data, true, false)
        {
        }

        #endregion

        #region Override Members

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            BindingList<T> bindingList = new BindingList<T>();
            foreach (object item in data)
            {
                bindingList.Add((T)item);
            }

            return bindingList;
        }

        protected override void AddCore(object item)
        {
            ((BindingList<T>)Source).Add((T)item);
        }

        protected override void RemoveCore(object item)
        {
            ((BindingList<T>)Source).Remove((T)item);
        }

        #endregion
    }
}
