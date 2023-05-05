// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Memory leak around ADO DataSet
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Binding", SecurityLevel=TestCaseSecurityLevel.FullTrust, Versions="4.7.2+")]
    public class DataSetMemoryLeak : XamlTest
    {
        #region Private Data

        private ListBox _listBox;
        private WeakReference _wrDataSet;

        #endregion

        #region Constructors

        public DataSetMemoryLeak()
            : base(@"DataSetMemoryLeak.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _listBox = (ListBox)RootElement.FindName("listBox");
            DataSet ds = NewDataSet();
            _listBox.DataContext = ds;
            _wrDataSet = new WeakReference(ds);

            return TestResult.Pass;
        }

        private TestResult Verify()
        {
            // replace the DataSet with a new one
            _listBox.DataContext = NewDataSet();

            // let WPF react
            WaitForPriority(DispatcherPriority.SystemIdle);

            // run aggressive GC
            Util.GetMemory();

            // for some reason, this clears up all the references to the old
            // DataSet _except_ for the one in wrDataSet.   Doing it again
            // seems to work.
            Util.GetMemory();

            // the old DataSet should have been GC'd
            if (_wrDataSet.Target == null)
                return TestResult.Pass;

            LogComment("DataSet has leaked");
            return TestResult.Fail;
        }

        private DataSet NewDataSet()
        {
            DataSet customerOrders = new DataSet("CustomerOrders");

            DataTable ordersTable = customerOrders.Tables.Add("Orders");

            DataColumn pkOrderID =
                ordersTable.Columns.Add("OrderID", typeof(Int32));
            ordersTable.Columns.Add("OrderQuantity", typeof(Int32));
            ordersTable.Columns.Add("CompanyName", typeof(string));

            ordersTable.PrimaryKey = new DataColumn[] { pkOrderID };

            DataView view = new DataView(ordersTable);
            DataRowView rowView = view.AddNew();
            rowView.BeginEdit();
            rowView["OrderID"] = 1344334;
            rowView["OrderQuantity"] = 120;
            rowView["CompanyName"] = "ASAC";
            rowView.EndEdit();

            return customerOrders;
        }

        #endregion
    }
}

