using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Collections;
using System.Windows;
using Avalon.Test.ComponentModel;
using System.Collections.Generic;
using Avalon.Test.ComponentModel.Utilities;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// BVTs for DataGrid Clipboard Copy operations
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridClipboardCopyBVTs", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite,MicroSuite")]
    public class DataGridClipboardCopyBVTs : DataGridTest
    {
        #region Constructor

        public DataGridClipboardCopyBVTs()
            : base(@"DataGridClipboardCopy.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestFullRowCopy);
            RunSteps += new TestStep(TestMultipleCellsCopy);
            RunSteps += new TestStep(TestMultipleRowsCopy);
            RunSteps += new TestStep(TestMultipleCellsInMultipleRowsCopy);
            RunSteps += new TestStep(TestMultipleCellsInAColumn);
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

            Status("Setup specific for DataGridClipboardCopyBVTs");

            this.SetupDataSource();

            LogComment("Setup for DataGridClipboardCopyBVTs was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify row is copied correctly to each format.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestFullRowCopy()
        {
            Status("TestFullRowCopy");

            foreach (DataGridClipboardCopyMode copyMode in new[] { DataGridClipboardCopyMode.ExcludeHeader, DataGridClipboardCopyMode.IncludeHeader })
            {
                LogComment(string.Format("Begin testing with DataGridClipboardCopyMode: {0}", copyMode));

                CleanupHelper();

                // init settings
                MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                MyDataGrid.ClipboardCopyMode = copyMode;
                QueueHelper.WaitTillQueueItemsProcessed();

                // select row 0
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[0]);
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                //DataGridActionHelper.SelectRow(MyDataGrid, 0, false, false);
                //QueueHelper.WaitTillQueueItemsProcessed();
                //if (MyDataGrid.SelectedItem != MyDataGrid.Items[0])
                //{
                //    QueueHelper.WaitTillQueueItemsProcessed();
                //    if (MyDataGrid.SelectedItem != MyDataGrid.Items[0])
                //    {
                //        throw new TestValidationException(string.Format("Unable to select the row.  Expected selection: {0}, Actual selection: {1}", MyDataGrid.Items[0], MyDataGrid.SelectedItem));
                //    }
                //}

                // copy 
                DataGridActionHelper.CopyToClipboard(MyDataGrid);
                QueueHelper.WaitTillQueueItemsProcessed();

                // verify copy
                DataGridClipboardHelper.ClipboardCopyInfo copyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
                {
                    clipboardCopyMode = copyMode,
                    minRowIndex = 0,
                    maxRowIndex = 0,
                    minColumnDisplayIndex = 0,
                    maxColumnDisplayIndex = MyDataGrid.Columns.Count - 1,
                    GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
                };
                DataGridClipboardHelper.VerifyClipboardCopy(MyDataGrid, copyInfo);

                LogComment(string.Format("End testing with DataGridClipboardCopyMode: {0}", copyMode));
            }

            LogComment("TestFullRowCopy was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify cells are copied correctly to each format.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestMultipleCellsCopy()
        {
            Status("TestMultipleCellsCopy");

            foreach (DataGridClipboardCopyMode copyMode in new[] { DataGridClipboardCopyMode.ExcludeHeader, DataGridClipboardCopyMode.IncludeHeader })
            {
                LogComment(string.Format("Begin testing with DataGridClipboardCopyMode: {0}", copyMode));

                CleanupHelper();

                // init settings
                MyDataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
                MyDataGrid.ClipboardCopyMode = copyMode;
                QueueHelper.WaitTillQueueItemsProcessed();

                Assert.AssertTrue("Number of columns has to be greater than the number of cells that will be clicked", MyDataGrid.Columns.Count > 4);

                // select cell(0,0) through cell(0,x)                
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[0]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[1]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[2]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[3]));
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                //DataGridActionHelper.ClickOnCell(MyDataGrid, 0, 0, true, false);
                //QueueHelper.WaitTillQueueItemsProcessed();
                //DataGridActionHelper.ClickOnCell(MyDataGrid, 0, 1, true, false);
                //QueueHelper.WaitTillQueueItemsProcessed();
                //DataGridActionHelper.ClickOnCell(MyDataGrid, 0, 2, true, false);
                //QueueHelper.WaitTillQueueItemsProcessed();
                //DataGridActionHelper.ClickOnCell(MyDataGrid, 0, 3, true, false);
                //QueueHelper.WaitTillQueueItemsProcessed();

                // copy 
                DataGridActionHelper.CopyToClipboard(MyDataGrid);
                QueueHelper.WaitTillQueueItemsProcessed();

                // verify copy
                DataGridClipboardHelper.ClipboardCopyInfo copyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
                {
                    clipboardCopyMode = copyMode,
                    minRowIndex = 0,
                    maxRowIndex = 0,
                    minColumnDisplayIndex = 0,
                    maxColumnDisplayIndex = 3,
                    GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
                };
                DataGridClipboardHelper.VerifyClipboardCopy(MyDataGrid, copyInfo);

                LogComment(string.Format("End testing with DataGridClipboardCopyMode: {0}", copyMode));
            }

            LogComment("TestMultipleCellsCopy was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify row is copied correctly to each format.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestMultipleRowsCopy()
        {
            Status("TestFullRowCopy");

            foreach (DataGridClipboardCopyMode copyMode in new[] { DataGridClipboardCopyMode.ExcludeHeader, DataGridClipboardCopyMode.IncludeHeader })
            {
                LogComment(string.Format("Begin testing with DataGridClipboardCopyMode: {0}", copyMode));

                CleanupHelper();

                // init settings
                MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                MyDataGrid.ClipboardCopyMode = copyMode;
                QueueHelper.WaitTillQueueItemsProcessed();

                // select multiple rows
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[0]);
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[1]);
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[2]);
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[3]);
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                // copy 
                DataGridActionHelper.CopyToClipboard(MyDataGrid);
                QueueHelper.WaitTillQueueItemsProcessed();

                // verify copy
                DataGridClipboardHelper.ClipboardCopyInfo copyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
                {
                    clipboardCopyMode = copyMode,
                    minRowIndex = 0,
                    maxRowIndex = 3,
                    minColumnDisplayIndex = 0,
                    maxColumnDisplayIndex = MyDataGrid.Columns.Count - 1,
                    GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
                };
                DataGridClipboardHelper.VerifyClipboardCopy(MyDataGrid, copyInfo);

                LogComment(string.Format("End testing with DataGridClipboardCopyMode: {0}", copyMode));
            }

            LogComment("TestFullRowCopy was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify cells from multiple rows are copied correctly to each format.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestMultipleCellsInMultipleRowsCopy()
        {
            Status("TestMultipleRowsAndCellsCopy");

            foreach (DataGridClipboardCopyMode copyMode in new[] { DataGridClipboardCopyMode.ExcludeHeader, DataGridClipboardCopyMode.IncludeHeader })
            {
                LogComment(string.Format("Begin testing with DataGridClipboardCopyMode: {0}", copyMode));

                CleanupHelper();

                // init settings
                MyDataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
                MyDataGrid.ClipboardCopyMode = copyMode;
                QueueHelper.WaitTillQueueItemsProcessed();

                Assert.AssertTrue("Number of columns has to be greater than the number of cells that will be clicked", MyDataGrid.Columns.Count > 4);

                // select cell(0,0) through cell(0,x)                
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[0]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[1]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[2]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[3]));
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                // select cell(1,0) through cell(1,x)                
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[1], MyDataGrid.Columns[0]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[1], MyDataGrid.Columns[1]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[1], MyDataGrid.Columns[2]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[1], MyDataGrid.Columns[3]));
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                // select cell(2,0) through cell(2,x)                
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[2], MyDataGrid.Columns[0]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[2], MyDataGrid.Columns[1]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[2], MyDataGrid.Columns[2]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[2], MyDataGrid.Columns[3]));
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                // copy 
                DataGridActionHelper.CopyToClipboard(MyDataGrid);
                QueueHelper.WaitTillQueueItemsProcessed();

                // verify copy
                DataGridClipboardHelper.ClipboardCopyInfo copyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
                {
                    clipboardCopyMode = copyMode,
                    minRowIndex = 0,
                    maxRowIndex = 2,
                    minColumnDisplayIndex = 0,
                    maxColumnDisplayIndex = 3,
                    GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
                };
                DataGridClipboardHelper.VerifyClipboardCopy(MyDataGrid, copyInfo);

                LogComment(string.Format("End testing with DataGridClipboardCopyMode: {0}", copyMode));
            }

            LogComment("TestMultipleRowsAndCellsCopy was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify multiple cells in a column are copied correctly to each format.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestMultipleCellsInAColumn()
        {
            Status("TestMultipleRowsAndCellsCopy");

            foreach (DataGridClipboardCopyMode copyMode in new[] { DataGridClipboardCopyMode.ExcludeHeader, DataGridClipboardCopyMode.IncludeHeader })
            {
                LogComment(string.Format("Begin testing with DataGridClipboardCopyMode: {0}", copyMode));

                CleanupHelper();

                // init settings
                MyDataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
                MyDataGrid.ClipboardCopyMode = copyMode;
                QueueHelper.WaitTillQueueItemsProcessed();

                Assert.AssertTrue("Number of columns has to be greater than the number of cells that will be clicked", MyDataGrid.Columns.Count > 4);

                // select cell(0,0) through cell(x,0)                
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[0]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[1], MyDataGrid.Columns[0]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[2], MyDataGrid.Columns[0]));
                MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[3], MyDataGrid.Columns[0]));
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                // copy 
                DataGridActionHelper.CopyToClipboard(MyDataGrid);
                QueueHelper.WaitTillQueueItemsProcessed();

                // verify copy
                DataGridClipboardHelper.ClipboardCopyInfo copyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
                {
                    clipboardCopyMode = copyMode,
                    minRowIndex = 0,
                    maxRowIndex = 3,
                    minColumnDisplayIndex = 0,
                    maxColumnDisplayIndex = 0,
                    GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
                };
                DataGridClipboardHelper.VerifyClipboardCopy(MyDataGrid, copyInfo);

                LogComment(string.Format("End testing with DataGridClipboardCopyMode: {0}", copyMode));
            }

            LogComment("TestMultipleRowsAndCellsCopy was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        #region Helpers

        private void CleanupHelper()
        {
            MyDataGrid.UnselectAll();
            MyDataGrid.UnselectAllCells();
            Clipboard.Clear();
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        public string GetDataFromTemplateColumn(DataGridCell currentCell, bool isEditing)
        {
            string cellData = null;

            //NOTE: this is hardcoded to the xaml file and expects
            //      a Button when !Editing and a TextBox when editing
            ContentPresenter cp = currentCell.Content as ContentPresenter;
            if (isEditing)
            {
                TextBox cellBlock = DataGridHelper.FindVisualChild<TextBox>(cp);
                cellData = cellBlock.Text;
            }
            else
            {
                Button cellBlock = DataGridHelper.FindVisualChild<Button>(cp);
                cellData = (string)cellBlock.Content;
            }

            return cellData;
        }

        #endregion Helpers
    }
}
