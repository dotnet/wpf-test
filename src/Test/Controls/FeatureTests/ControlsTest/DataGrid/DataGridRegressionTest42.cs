using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest42", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest42 : DataGridTest
    {
        private Button debugButton;        
        
        #region Constructor

        public DataGridRegressionTest42()
            : base(@"DataGridRegressionTest42.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestDataGrid);
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

            Status("Setup specific for DataGridRegressionTest42");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            this.SetupDataSource();

            //


            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup for DataGridRegressionTest42 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            debugButton = null;            
            return base.CleanUp();
        }

        /// <summary>
        /// 1. remove the selected item from the collection
        /// 
        /// Verify: DataGrid.SelectedItem is changed from the item that was removed
        /// </summary>
        private TestResult TestDataGrid()
        {
            Status("TestDataGrid");

            // set the first item as selected
            MyDataGrid.SelectedItem = (DataSource as People)[0];

            var itemToDelete = MyDataGrid.SelectedItem as Person;
            Assert.AssertTrue("itemToDelete cannot be null", itemToDelete != null);

            LogComment("removed the selected item from the collection");
            var collection = MyDataGrid.ItemsSource as People;
            collection.Remove(itemToDelete);

            LogComment("verify the SelectedItem on DG has changed.");
            var item = MyDataGrid.SelectedItem;
            Assert.AssertTrue("Deleted item shouldn't be selected", itemToDelete != item);                    

            LogComment("TestDataGrid was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps        

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
