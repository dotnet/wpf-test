using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test  Verify the SelectAll button is visible even when the NewItemPlaceholder row is retemplated.  
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest31", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest31 : DataGridTest
    {
        private Button debugButton;        
        private ControlTemplate newRowControlTemplate;

        #region Constructor

        public DataGridRegressionTest31()
            : base(@"DataGridRegressionTest31.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestSelectAllButton);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            base.Setup();

            Status("Setup specific for DataGridRegressionTest31");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            newRowControlTemplate = (ControlTemplate)MyDataGrid.FindResource("NewRow_ControlTemplate");
            Assert.AssertTrue("Unable to find newRowControlTemplate from the resources", newRowControlTemplate != null);

            MyDataGrid.ItemsSource = new Collection<Person> { new Person { FirstName = "Steve", LastName = "Balmer" } };            
            MyDataGrid.LoadingRow += DataGrid_Standard_LoadingRow;

            //uncomment for manual verification
            //Debug();

            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup for DataGridRegressionTest31 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            MyDataGrid.LoadingRow -= DataGrid_Standard_LoadingRow;
            debugButton = null;
            newRowControlTemplate = null;
            return base.CleanUp();
        }
        /// <summary>
        /// 1. Check the visibility of the SelectAll button
        /// 
        /// Verify: SelectAll button is visible after retemplating the NewItemPlaceholder row
        /// </summary>
        private TestResult TestSelectAllButton()
        {
            Status("TestSelectAllButton");

            LogComment("Check the visibility of the SelectAll button");

            var selectAllButton = VisualTreeUtils.GetVisualChild<Button>(MyDataGrid);
            Assert.AssertTrue("Unable to find selectAllButton from the resources", selectAllButton != null);
            
            LogComment("selectAllButton.ActualWidth: " + selectAllButton.ActualWidth);

            if (selectAllButton.ActualWidth <= 0)
            {
                throw new TestValidationException("SelectAllButton should be visible but is not.");
            }

            LogComment("TestSelectAllButton was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        private void DataGrid_Standard_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.Item == CollectionView.NewItemPlaceholder)
            {
                e.Row.Template = newRowControlTemplate;
                e.Row.UpdateLayout();
            }
        }

        private void Debug()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            debugButton.MouseLeftButtonDown += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }
    }
}
