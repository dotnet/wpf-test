using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Adding selected items using DataGrid.SelectedItems causes slow performance on large collections
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest43", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest43 : DataGridTest
    {
        #region Private Members

        private PagedCollection<TestData> data;
        private static int threshold = 1000;

        #endregion

        #region Constructor

        public DataGridRegressionTest43()
            : base(@"DataGridRegressionTest43.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Repro);
        }

        #endregion

        #region Test Steps

        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRegressionTest43");
            this.SetupDataSource();

            LogComment("Setup DataGridRegressionTest43 successfully");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            data = null;
            return base.CleanUp();
        }

        protected override void SetupDataSource()
        {
            List<TestData> tdata = new List<TestData>();
            for (int i = 1; i < 100; i++)
            {
                tdata.Add(new TestData { Id = i, Text = string.Format("Element {0}", i) });
            }
            this.WaitForPriority(DispatcherPriority.SystemIdle);

            data = new PagedCollection<TestData>(tdata, 10000, 1000);
            data.PagedDataRequest += new EventHandler<PagedDataEventArgs<TestData>>(PagedItemsRequest);

            MyDataGrid.ItemsSource = data;
            this.WaitForPriority(DispatcherPriority.SystemIdle);
            Assert.AssertTrue("Failed to setup datagrid data source", MyDataGrid.ItemsSource != null);
            this.Window.Focus();
        }

        private TestResult Repro()
        {
            Status("Repro");
            LogComment("Select first item on datagrid");
            TestData firstDataItem = data.First();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            LogComment(string.Format("Start adding object with Id: {0}", firstDataItem.Id));
            MyDataGrid.SelectedItems.Add(firstDataItem);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment(string.Format("End adding object with Id: {0}", firstDataItem.Id));
            stopWatch.Stop();
            long timeSpent = stopWatch.ElapsedMilliseconds;

            LogComment(string.Format("Spent time can't more than {0} millseconds, actual time is {1} millseconds", threshold, timeSpent));

            if (timeSpent < threshold)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }

        }

        private void PagedItemsRequest(object sender, PagedDataEventArgs<TestData> e)
        {
            List<TestData> data = new List<TestData>();

            LogComment(string.Format("Paged request called for item {0}, pagesize {1}", e.StartPos, e.PageSize));

            for (int i = e.StartPos + 1; i <= e.StartPos + e.PageSize; i++)
            {
                data.Add(new TestData { Id = i, Text = string.Format("Element {0}", i) });
            }

            Thread.Sleep(500); //thread sleep to simulate a retrieval from server

            e.Result = data;
        }

        #endregion Test Steps

    }

    #region Helper  Class

    public class TestData : INotifyPropertyChanged
    {
        private int id;

        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        private string text;

        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged("Text");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}
