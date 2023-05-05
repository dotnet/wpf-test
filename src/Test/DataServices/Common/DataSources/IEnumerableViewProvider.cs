// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.ObjectModel;

namespace Microsoft.Test.DataServices
{
    public class IEnumerableViewProvider : DataSourceViewProvider
    {
        #region Constructors

        public IEnumerableViewProvider(IEnumerable data)
            : base(data, false, true)
        {
        }

        #endregion

        #region Override Members

        protected override IEnumerable CreateDataSource(IEnumerable data)
        {
            return new IEnumerableDataSource(data);
        }

        #endregion

        private class IEnumerableDataSource : IEnumerable
        {
            #region Private Data

            private ObservableCollection<object> _backingCollection;

            #endregion

            #region Constructors

            public IEnumerableDataSource(IEnumerable data)
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
        }
    }
}
