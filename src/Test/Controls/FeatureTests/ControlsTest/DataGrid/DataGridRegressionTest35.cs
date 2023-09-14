using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Input;
using Avalon.Test.ComponentModel;
using System.Windows.Input;
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
    [Test(0, "DataGrid", "DataGridRegressionTest35", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest35 : DataGridTest
    {
        private Button debugButton;

        #region Constructor

        public DataGridRegressionTest35()
            : base(@"DataGridRegressionTest35.xaml")
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

            Status("Setup specific for DataGridRegressionTest35");

            SetupDataSource();

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            MyDataGrid.EnableRowVirtualization = false;
            MyDataGrid.SelectionUnit = DataGridSelectionUnit.CellOrRowHeader;

            //


            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup for DataGridRegressionTest35 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            debugButton = null;
            return base.CleanUp();
        }
        /// <summary>
        /// 1. Click on a RowHeader to select the entire row
        /// 2. Press SHIFT + Down Key
        /// 
        /// Verify: row below is also selected
        /// 
        /// 3. Click on a RowHeader to select the entire row
        /// 4. Press SHIFT + PageDown
        /// 
        /// Verify: rows below is also selected
        /// 
        /// 5. Click on a RowHeader to select the entire row
        /// 6. Press Ctrl + SHIFT + END
        /// </summary>
        private TestResult TestDataGrid()
        {
            Status("TestDataGrid");

            Assert.AssertTrue("DataGrid needs to be in CellOrRowHeader SelectionUnit", MyDataGrid.SelectionUnit == DataGridSelectionUnit.CellOrRowHeader);

            LogComment("1. Click on a RowHeader to select the entire row");
            DataGridActionHelper.ClickOnRowHeader(MyDataGrid, 0);

            LogComment("2. Press SHIFT + Down Key");
            UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
            UserInput.KeyDown(System.Windows.Input.Key.Down.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.KeyUp(System.Windows.Input.Key.Down.ToString());
            UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Verify: row below is also selected");
            var row0 = DataGridHelper.GetRow(MyDataGrid, 0);
            var row1 = DataGridHelper.GetRow(MyDataGrid, 1);
            if (!row0.IsSelected || !row1.IsSelected)
            {
                throw new TestValidationException("Rows 0 or 1 have not been correctly selected");
            }

            LogComment("3. Click on a RowHeader to select the entire row");
            int startIndex = 2;
            DataGridActionHelper.ClickOnRowHeader(MyDataGrid, startIndex);

            LogComment("4. Press SHIFT + PageDown");
            UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
            UserInput.KeyDown(System.Windows.Input.Key.PageDown.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.KeyUp(System.Windows.Input.Key.PageDown.ToString());
            UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Verify: rows below is also selected");
            for (int i = startIndex; i < startIndex + 5; i++)
            {
                var row = DataGridHelper.GetRow(MyDataGrid, i);
                if (!row.IsSelected)
                {
                    throw new TestValidationException(string.Format("Row: {0}, has not been correctly selected", i));
                }
            }

            LogComment("5. Click on a RowHeader to select the entire row");
            startIndex = 4;
            DataGridActionHelper.ClickOnRowHeader(MyDataGrid, startIndex);

            LogComment("6. Press SHIFT + End");
            UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
            UserInput.KeyDown(System.Windows.Input.Key.LeftCtrl.ToString());
            UserInput.KeyDown(System.Windows.Input.Key.End.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.KeyUp(System.Windows.Input.Key.End.ToString());
            UserInput.KeyUp(System.Windows.Input.Key.LeftCtrl.ToString());
            UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Verify: rows below is also selected");
            for (int i = startIndex; i < MyDataGrid.Items.Count; i++)
            {
                var row = DataGridHelper.GetRow(MyDataGrid, i);
                if (!row.IsSelected)
                {
                    throw new TestValidationException(string.Format("Row: {0}, has not been correctly selected", i));
                }
            }

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
