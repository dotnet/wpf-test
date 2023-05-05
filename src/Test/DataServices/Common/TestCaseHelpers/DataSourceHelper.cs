// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.DataServices
{
    using Microsoft.Test.Logging;
    using Microsoft.Test.Threading;
    using System;
    using System.Windows.Data;

    /// <summary>
    /// DataSource utilities.
    /// </summary>
    public static class DataSourceHelper
    {
        /// <summary>
        /// Wait for a DataSourceProvider to be populated with data. Primarily
        /// used to make sure an asynchronous data source (such as XmlDataProvider)
        /// has finished loading before testing. Times out after five seconds.
        /// </summary>
        /// <param name="dataSource">DataSourceProvider to wait for data upon.</param>
        /// <returns>Data from the DataSourceProvider.</returns>
        public static object WaitForData(DataSourceProvider dataSource)
        {
            return WaitForData(dataSource, new TimeSpan(0, 0, 5));
        }

        /// <summary>
        /// Wait for a DataSourceProvider to be populated with data. Primarily
        /// used to make sure an asynchronous data source (such as XmlDataProvider)
        /// has finished loading before testing.
        /// </summary>
        /// <param name="dataSource">DataSourceProvider to wait for data upon.</param>
        /// <param name="timeout">Timeout period after which a TimeoutException is thrown.</param>
        /// <returns>Data from the DataSourceProvider.</returns>
        public static object WaitForData(DataSourceProvider dataSource, TimeSpan timeout)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            if (timeout.TotalMilliseconds < 0)
                throw new ArgumentException("timeout cannot be a negative number");

            DispatcherSignalHelper signalHelper = new DispatcherSignalHelper();

            EventHandler eventHandler = delegate(object sender, EventArgs args)
            {
                signalHelper.Signal(TestResult.Pass);
            };

            dataSource.DataChanged += eventHandler;

            try
            {
                if (dataSource.Data != null)
                    return dataSource.Data;

                if (signalHelper.WaitForSignal((int)timeout.TotalMilliseconds) == TestResult.Unknown)
                    throw new TimeoutException("A timeout of " + timeout.TotalMilliseconds.ToString() + " milliseconds occurred while waiting for the DataChanged event for the data source");
            }
            finally
            {
                dataSource.DataChanged -= eventHandler;
            }

            return dataSource.Data;
        }
    }
}
