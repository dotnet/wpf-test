using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Test DataGrid selection exceptions.
    /// </summary>
    [Test(1, "DataGrid", "DataGridSelectionExceptionTest", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridSelectionExceptionTest : XamlTest
    {
        public DataGridSelectionExceptionTest()
            : base(@"DataGridSelectionMode.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestAccessDataGridItemsException);
            RunSteps += new TestStep(TestBindToDataGridItemsSourceException);
        }

        #region Private Members

        Panel panel;
        
        #endregion

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            panel = (Panel)RootElement.FindName("panel");
            if (panel == null)
            {
                throw new TestValidationException("Panel is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            panel = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Test access DataGrid Items exception
        /// </summary>
        /// <returns></returns>
        TestResult TestAccessDataGridItemsException()
        {
            ExceptionHelper.ExpectException(
                () =>
                {
                    DataGrid dataGrid = new DataGrid();
                    ObservableCollection<DataGridData> dataGridDataCollection = new ObservableCollection<DataGridData>();
                    dataGridDataCollection.Add(new DataGridData("one", 1, 1.0, true, null));
                    dataGrid.ItemsSource = dataGridDataCollection;
                    QueueHelper.WaitTillQueueItemsProcessed();
                    dataGrid.Items.Add(new DataGridRow());
                },
                    new InvalidOperationException());

            return TestResult.Pass;
        }

        /// <summary>
        /// Test bind to DataGrid ItemsSource exception
        /// </summary>
        /// <returns></returns>
        TestResult TestBindToDataGridItemsSourceException()
        {
            ExceptionHelper.ExpectException(
                () =>
                {
                    DataGrid dataGrid = new DataGrid();
                    dataGrid.Items.Add(new DataGridRow());
                    QueueHelper.WaitTillQueueItemsProcessed();
                    ObservableCollection<DataGridData> dataGridDataCollection = new ObservableCollection<DataGridData>();
                    dataGridDataCollection.Add(new DataGridData("one", 1, 1.0, true, null));
                    dataGrid.ItemsSource = dataGridDataCollection;
                },
                    new InvalidOperationException());

            return TestResult.Pass;
        }
    }
}
