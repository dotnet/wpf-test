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
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// BVTs for DataGrid Clipboard Copy operations
    /// </description>

    /// </summary>
    // Disabling for .NET Core 3, Fix and re-enable.
    //[Test(1, "DataGrid", "DataGridClipboardCopyP1", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridClipboardCopyP1 : DataGridTest
    {
        #region Constructor

        public DataGridClipboardCopyP1()
            : base(@"DataGridClipboardCopy.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestCellClipboardEvent);
            RunSteps += new TestStep(TestRowClipboardEvent);
            RunSteps += new TestStep(TestNonContiguousSelectedRows);
            RunSteps += new TestStep(TestCellClipboardEventWithNoClipboardContentBinding);
            RunSteps += new TestStep(TestDataGridClipboardCellContent);
            RunSteps += new TestStep(TestPastingCellClipboardEvent);
            RunSteps += new TestStep(TestCopyToLockedClipboard);

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
        /// Verify CopyCellClipboardContent is called with correct values
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCellClipboardEvent()
        {
            Status("TestCellClipboardEvent");

            CleanupHelper();

            List<EventHandler<DataGridCellClipboardEventArgs>> copyingCellClipboardEvents = new List<EventHandler<DataGridCellClipboardEventArgs>>();
            try
            {
                // init settings
                MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                MyDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                QueueHelper.WaitTillQueueItemsProcessed();

                // select row 0
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[0]);
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                // create the expected data
                DataGridClipboardHelper.ClipboardCopyInfo clipboardCopyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
                {
                    clipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader,
                    minRowIndex = 0,
                    maxRowIndex = 0,
                    minColumnDisplayIndex = 0,
                    maxColumnDisplayIndex = MyDataGrid.Columns.Count - 1,
                    GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
                };
                List<List<string>> expectedRowValues = DataGridClipboardHelper.CreateExpectedData(MyDataGrid, ref clipboardCopyInfo);

                // setup the event
                int expectedRow = 0;
                int numEventsFired = 0;
                foreach (DataGridColumn column in MyDataGrid.Columns)
                {
                    EventHandler<DataGridCellClipboardEventArgs> copyingCellClipboardEvent = (s, e) =>
                        {
                            if (e.Column.DisplayIndex != numEventsFired)
                            {
                                throw new TestValidationException(string.Format(
                                    "Column in CopyingCellClipboardContent does not match.  Expected DisplayIndex: {0}, Actual DisplayIndex: {1}",
                                    numEventsFired,
                                    e.Column.DisplayIndex));
                            }
                            if (e.Item != MyDataGrid.Items[expectedRow])
                            {
                                throw new TestValidationException(string.Format(
                                    "Item in CopyingCellClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                    e.Item,
                                    MyDataGrid.Items[expectedRow]));
                            }
                            if (e.Content.ToString() != expectedRowValues[expectedRow][numEventsFired])
                            {
                                throw new TestValidationException(string.Format(
                                    "Content in CopyingCellClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                    expectedRowValues[expectedRow][numEventsFired],
                                    e.Content));
                            }

                            // test setting the content
                            e.Content = "new content";

                            numEventsFired++;
                        };
                    copyingCellClipboardEvents.Add(copyingCellClipboardEvent);

                    column.CopyingCellClipboardContent += copyingCellClipboardEvent;
                }

                // do the copy
                DataGridActionHelper.CopyToClipboard(MyDataGrid);
                QueueHelper.WaitTillQueueItemsProcessed();

                // verify the correct number of events
                if (numEventsFired != MyDataGrid.Columns.Count)
                {
                    throw new TestValidationException(string.Format(
                        "CopyingCellClipboardContent event did not fire the correct number of times.  Expected: {0}, Actual: {1}",
                        MyDataGrid.Columns.Count,
                        numEventsFired));
                }

                // now check that the content has updated
                IDataObject ido = Clipboard.GetDataObject();
                object actualData = (DataObject)ido.GetData(DataFormats.Text);
                LogComment("actualCopy data: " + actualData);
                if (!actualData.ToString().Contains("new content"))
                {
                    throw new TestValidationException(string.Format("Actual clipboard content does not contain the modified data.  Actual data: {0}", actualData));
                }

            }
            finally
            {
                int i = 0;
                foreach (DataGridColumn column in MyDataGrid.Columns)
                {
                    column.CopyingCellClipboardContent -= copyingCellClipboardEvents[i];
                    i++;
                }
            }

            LogComment("TestCellClipboardEvent was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify CopyRowClipboardContent is called with correct values
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestRowClipboardEvent()
        {
            Status("TestRowClipboardEvent");

            CleanupHelper();

            EventHandler<DataGridRowClipboardEventArgs> copyingRowClipboardEvent = null;
            try
            {
                // init settings
                MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                MyDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                QueueHelper.WaitTillQueueItemsProcessed();

                // select multiple rows
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[0]);
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[1]);
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[2]);
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[3]);
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                // create the expected data
                DataGridClipboardHelper.ClipboardCopyInfo clipboardCopyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
                {
                    clipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader,
                    minRowIndex = 0,
                    maxRowIndex = 3,
                    minColumnDisplayIndex = 0,
                    maxColumnDisplayIndex = MyDataGrid.Columns.Count - 1,
                    GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
                };
                List<List<string>> expectedRowValues = DataGridClipboardHelper.CreateExpectedData(MyDataGrid, ref clipboardCopyInfo);

                // setup the event
                int numEventsFired = 0;
                copyingRowClipboardEvent = (s, e) =>
                    {
                        if (e.StartColumnDisplayIndex != 0)
                        {
                            throw new TestValidationException(string.Format(
                                "StarColumnDisplayIndex of CopyingRowClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                0,
                                e.StartColumnDisplayIndex));
                        }
                        if (e.EndColumnDisplayIndex != MyDataGrid.Columns.Count - 1)
                        {
                            throw new TestValidationException(string.Format(
                               "EndColumnDisplayIndex of CopyingRowClipboardContent does not match.  Expected: {0}, Actual: {1}",
                               MyDataGrid.Columns.Count - 1,
                               e.EndColumnDisplayIndex));
                        }
                        if (e.Item != MyDataGrid.Items[numEventsFired])
                        {
                            throw new TestValidationException(string.Format(
                                "Item in CopyingRowClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                e.Item,
                                MyDataGrid.Items[numEventsFired]));
                        }

                        if (e.ClipboardRowContent.Count != expectedRowValues[numEventsFired].Count)
                        {
                            throw new TestValidationException(string.Format(
                                "ClipboardRowContent.Count in CopyingRowClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                expectedRowValues[numEventsFired].Count,
                                e.ClipboardRowContent.Count));
                        }

                        int i = 0;
                        foreach (string expectedValue in expectedRowValues[numEventsFired])
                        {
                            if (e.ClipboardRowContent[i].Content.ToString() != expectedValue)
                            {
                                throw new TestValidationException(string.Format(
                                    "ClipboardRowContent.Content in CopyingRowClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                    expectedValue,
                                    e.ClipboardRowContent[i].Content));
                            }

                            if (expectedValue.Contains(e.ClipboardRowContent[i].Item.ToString()))
                            {
                                throw new TestValidationException(string.Format(
                                    "ClipboardRowContent.Item in CopyingRowClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                    expectedValue,
                                    e.ClipboardRowContent[i].Item));
                            }

                            if (e.ClipboardRowContent[i].Column != MyDataGrid.Columns[i])
                            {
                                throw new TestValidationException(string.Format(
                                    "ClipboardRowContent.Column in CopyingRowClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                    MyDataGrid.Columns[i],
                                    e.ClipboardRowContent[i].Column));
                            }
                            i++;
                        }

                        numEventsFired++;
                    };
                MyDataGrid.CopyingRowClipboardContent += copyingRowClipboardEvent;


                // do the copy
                DataGridActionHelper.CopyToClipboard(MyDataGrid);
                QueueHelper.WaitTillQueueItemsProcessed();

                // verify the correct number of events
                if (numEventsFired != expectedRowValues.Count)
                {
                    throw new TestValidationException(string.Format(
                        "CopyingRowClipboardContent event did not fire the correct number of times.  Expected: {0}, Actual: {1}",
                        expectedRowValues.Count,
                        numEventsFired));
                }
            }
            finally
            {
                if (copyingRowClipboardEvent != null)
                    MyDataGrid.CopyingRowClipboardContent -= copyingRowClipboardEvent;
            }

            LogComment("TestRowClipboardEvent was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify support for noncontiguous selected rows.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestNonContiguousSelectedRows()
        {
            Status("TestNonContiguousSelectedRows");

            CleanupHelper();

            // init settings
            MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            MyDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            QueueHelper.WaitTillQueueItemsProcessed();

            // select multiple rows
            MyDataGrid.SelectedItems.Add(MyDataGrid.Items[0]);
            MyDataGrid.SelectedItems.Add(MyDataGrid.Items[2]);
            MyDataGrid.SelectedItems.Add(MyDataGrid.Items[3]);
            MyDataGrid.SelectedItems.Add(MyDataGrid.Items[7]);
            MyDataGrid.UpdateLayout();
            QueueHelper.WaitTillQueueItemsProcessed();

            // copy
            DataGridActionHelper.CopyToClipboard(MyDataGrid);
            QueueHelper.WaitTillQueueItemsProcessed();

            // verify copy
            DataGridClipboardHelper.ClipboardCopyInfo copyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
            {
                clipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader,
                rowIndices = new[] { 0, 2, 3, 7 },
                minColumnDisplayIndex = 0,
                maxColumnDisplayIndex = MyDataGrid.Columns.Count - 1,
                GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
            };
            DataGridClipboardHelper.VerifyClipboardCopy(MyDataGrid, copyInfo);

            LogComment("TestNonContiguousSelectedRows was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify CopyCellClipboardContent is called even when no ClipboardContentBinding is defined.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCellClipboardEventWithNoClipboardContentBinding()
        {
            Status("TestCellClipboardEventWithNoClipboardContentBinding");

            CleanupHelper();

            // init settings
            MyDataGrid.SelectionUnit = DataGridSelectionUnit.Cell;
            MyDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
            QueueHelper.WaitTillQueueItemsProcessed();

            DataGridTemplateColumn templateColumn = new DataGridTemplateColumn();

            Status("setup the event");
            int expectedRow = 0;
            int expectedDisplayIndex = MyDataGrid.Columns.Count;
            bool eventFired = false;
            templateColumn.CopyingCellClipboardContent += (s, e) =>
                {
                    eventFired = true;
                    if (e.Column.DisplayIndex != expectedDisplayIndex)
                    {
                        throw new TestValidationException(string.Format(
                            "Column in CopyingCellClipboardContent does not match.  Expected DisplayIndex: {0}, Actual DisplayIndex: {1}",
                            expectedDisplayIndex,
                            e.Column.DisplayIndex));
                    }
                    if (e.Item != MyDataGrid.Items[expectedRow])
                    {
                        throw new TestValidationException(string.Format(
                            "Item in CopyingCellClipboardContent does not match.  Expected: {0}, Actual: {1}",
                            e.Item,
                            MyDataGrid.Items[expectedRow]));
                    }
                };
            templateColumn.ClipboardContentBinding = null;
            MyDataGrid.Columns.Add(templateColumn);
            QueueHelper.WaitTillQueueItemsProcessed();

            Status("select the last cell");
            MyDataGrid.SelectedCells.Add(new DataGridCellInfo(MyDataGrid.Items[0], MyDataGrid.Columns[MyDataGrid.Columns.Count - 1]));
            MyDataGrid.UpdateLayout();
            QueueHelper.WaitTillQueueItemsProcessed();

            Status("do the copy");
            DataGridActionHelper.CopyToClipboard(MyDataGrid);
            QueueHelper.WaitTillQueueItemsProcessed();

            Status("verify the event fired");
            if (!eventFired)
            {
                throw new TestValidationException(string.Format("CopyingCellClipboardContent event did not fire for templateColumn with null ClipboardContentBinding."));
            }

            // remove the template column
            MyDataGrid.Columns.Remove(templateColumn);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("TestCellClipboardEventWithNoClipboardContentBinding was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Basic functional tests for the DataGridClipboardCellContent struct
        ///
        /// Verify:
        /// Equals
        /// op_Equals
        /// op_Inequality
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDataGridClipboardCellContent()
        {
            Status("TestDataGridClipboardCellContent");

            LogComment("Testing equality method");

            var item = "test item";
            var column = new DataGridTextColumn();
            var content = "test content";
            var clipboardCellContent1 = new DataGridClipboardCellContent(item, column, content);
            var clipboardCellContent2 = new DataGridClipboardCellContent(item, column, content);

            if (!clipboardCellContent1.Equals(clipboardCellContent2))
            {
                throw new TestValidationException(string.Format(
                    "Mismatch when should have matched. cellContent1: {0}, cellContent2: {1}", clipboardCellContent1, clipboardCellContent2));
            }

            if (!clipboardCellContent2.Equals(clipboardCellContent1))
            {
                throw new TestValidationException(string.Format(
                    "Mismatch when should have matched. cellContent1: {0}, cellContent2: {1}", clipboardCellContent1, clipboardCellContent2));
            }

            bool expected = true;
            bool actual = clipboardCellContent1 == clipboardCellContent2;
            if (expected != actual)
            {
                throw new TestValidationException(string.Format(
                    "Mismatch when should have matched. cellContent1: {0}, cellContent2: {1}", clipboardCellContent1, clipboardCellContent2));
            }

            expected = true;
            actual = clipboardCellContent2 == clipboardCellContent1;
            if (expected != actual)
            {
                throw new TestValidationException(string.Format(
                    "Mismatch when should have matched. cellContent1: {0}, cellContent2: {1}", clipboardCellContent1, clipboardCellContent2));
            }

            LogComment("Testing inequality method");

            var clipboardCellContent3 = new DataGridClipboardCellContent("different item", column, content);

            if (clipboardCellContent1.Equals(clipboardCellContent3))
            {
                throw new TestValidationException(string.Format(
                    "Match when should have Mismatch. cellContent1: {0}, cellContent2: {1}", clipboardCellContent1, clipboardCellContent2));
            }

            if (clipboardCellContent3.Equals(clipboardCellContent1))
            {
                throw new TestValidationException(string.Format(
                    "Match when should have Mismatch. cellContent1: {0}, cellContent2: {1}", clipboardCellContent1, clipboardCellContent2));
            }

            expected = true;
            actual = clipboardCellContent1 != clipboardCellContent3;
            if (expected != actual)
            {
                throw new TestValidationException(string.Format(
                    "Match when should have Mismatch. cellContent1: {0}, cellContent2: {1}", clipboardCellContent1, clipboardCellContent2));
            }

            expected = true;
            actual = clipboardCellContent3 != clipboardCellContent1;
            if (expected != actual)
            {
                throw new TestValidationException(string.Format(
                    "Match when should have Mismatch. cellContent1: {0}, cellContent2: {1}", clipboardCellContent1, clipboardCellContent2));
            }

            LogComment("Testing inequality with null value");

            if (clipboardCellContent1.Equals(null))
            {
                throw new TestValidationException(string.Format("Matched a null value which is incorrect"));
            }

            if (clipboardCellContent1.Equals("type that is different"))
            {
                throw new TestValidationException(string.Format("Matched a type not of the same type."));
            }

            LogComment("TestDataGridClipboardCellContent was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify PastingCellClipboardContent is called with correct values
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestPastingCellClipboardEvent()
        {
            Status("TestPastingCellClipboardEvent");

            CleanupHelper();

            var pastingCellClipboardEvents = new List<EventHandler<DataGridCellClipboardEventArgs>>();

            try
            {
                // init settings
                MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
                MyDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
                QueueHelper.WaitTillQueueItemsProcessed();

                // select row 0
                MyDataGrid.SelectedItems.Add(MyDataGrid.Items[0]);
                MyDataGrid.UpdateLayout();
                QueueHelper.WaitTillQueueItemsProcessed();

                // create the expected data
                List<List<string>> expectedRowValues = new List<List<string>>
                {
                    new List<string> { "Cell00", "Cell01", "Cell02", "Cell03", "Cell04", "Cell05", "Cell06" },
                    new List<string> { "Cell10", "Cell11", "Cell12", "Cell13", "Cell14", "Cell15", "Cell16" },
                };

                // setup the event
                int expectedRow = 0;
                int numEventsFired = 0;
                foreach (DataGridColumn column in MyDataGrid.Columns)
                {
                    EventHandler<DataGridCellClipboardEventArgs> pastingCellClipboardEvent = (s, e) =>
                    {
                        if (e.Column.DisplayIndex != numEventsFired)
                        {
                            throw new TestValidationException(string.Format(
                                "Column in PastingCellClipboardContent does not match.  Expected DisplayIndex: {0}, Actual DisplayIndex: {1}",
                                numEventsFired,
                                e.Column.DisplayIndex));
                        }
                        if (e.Item != MyDataGrid.Items[expectedRow])
                        {
                            throw new TestValidationException(string.Format(
                                "Item in PastingCellClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                e.Item,
                                MyDataGrid.Items[expectedRow]));
                        }
                        if (e.Content.ToString() != expectedRowValues[expectedRow][numEventsFired])
                        {
                            throw new TestValidationException(string.Format(
                                "Content in PastingCellClipboardContent does not match.  Expected: {0}, Actual: {1}",
                                expectedRowValues[expectedRow][numEventsFired],
                                e.Content));
                        }

                        // test setting the content
                        e.Content = "new content";

                        numEventsFired++;
                    };
                    pastingCellClipboardEvents.Add(pastingCellClipboardEvent);

                    column.PastingCellClipboardContent += pastingCellClipboardEvent;
                }

                // do the paste
                int i = 0;
                foreach (DataGridColumn column in MyDataGrid.Columns)
                {
                    column.OnPastingCellClipboardContent(MyDataGrid.Items[0], expectedRowValues[0][i]);
                    i++;
                }

                QueueHelper.WaitTillQueueItemsProcessed();

                // verify the correct number of events
                if (numEventsFired != MyDataGrid.Columns.Count)
                {
                    throw new TestValidationException(string.Format(
                        "CopyingCellClipboardContent event did not fire the correct number of times.  Expected: {0}, Actual: {1}",
                        MyDataGrid.Columns.Count,
                        numEventsFired));
                }
            }
            finally
            {
                int i = 0;
                foreach (DataGridColumn column in MyDataGrid.Columns)
                {
                    column.PastingCellClipboardContent -= pastingCellClipboardEvents[i];
                    i++;
                }
            }

            LogComment("TestPastingCellClipboardEvent was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify behavior of Copy when clipboard is locked
        /// </summary>
        private TestResult TestCopyToLockedClipboard()
        {
            // lock the clipboard
            NativeMethods.OpenClipboard(IntPtr.Zero);

            // test the behavior of Copy
            bool success = TryCopy(false);

            // unlock the clipboard
            NativeMethods.CloseClipboard();

            return success ? TestResult.Pass : TestResult.Fail;
        }

        private bool TryCopy(bool shouldThrow)
        {
            // do a Copy, recording whether an exception was thrown
            MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
            MyDataGrid.SelectedItems.Add(MyDataGrid.Items[0]);
            MyDataGrid.UpdateLayout();
            QueueHelper.WaitTillQueueItemsProcessed();

            bool exceptionThrown = false;
            try
            {
                DataGridActionHelper.CopyToClipboard(MyDataGrid);
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            catch (ExternalException)
            {
                exceptionThrown = true;
            }

            // report the result
            LogComment(String.Format("Copy to locked clipboard - should throw: {0}  did throw: {1}", shouldThrow, exceptionThrown));
            return (exceptionThrown == shouldThrow);
        }

        //








        //    // init settings
        //    MyDataGrid.SelectionUnit = DataGridSelectionUnit.FullRow;
        //    MyDataGrid.ClipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader;
        //    QueueHelper.WaitTillQueueItemsProcessed();

        //    // set cell(0,0) to the expected bad format
        //    CurRow = 0;
        //    CurCol = 0;
        //    string input = "Bad\\t\"In,pu\\t\"t";
        //    DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);
        //    DataGridActionHelper.EditCellCustomInput(MyDataGrid, CurRow, CurCol, input);
        //    DataGridCommandHelper.CommitEdit(MyDataGrid, CurRow, CurCol);

        //    // set cell(0,1) to the expected bad format
        //    CurRow = 0;
        //    CurCol = 1;
        //    string input2 = "Bad<I&>\\t \"np<u\"t";
        //    DataGridCommandHelper.BeginEdit(MyDataGrid, CurRow, CurCol);
        //    DataGridActionHelper.EditCellCustomInput(MyDataGrid, CurRow, CurCol, input2);
        //    DataGridCommandHelper.CommitEdit(MyDataGrid, CurRow, CurCol);

        //    CleanupHelper();

        //    // select row 0
        //    MyDataGrid.SelectedItems.Add(MyDataGrid.Items[0]);
        //    MyDataGrid.UpdateLayout();
        //    QueueHelper.WaitTillQueueItemsProcessed();

        //    // copy
        //    DataGridActionHelper.CopyToClipboard(MyDataGrid);
        //    QueueHelper.WaitTillQueueItemsProcessed();

        //    // verify copy
        //    DataGridClipboardHelper.ClipboardCopyInfo copyInfo = new DataGridClipboardHelper.ClipboardCopyInfo
        //    {
        //        clipboardCopyMode = DataGridClipboardCopyMode.ExcludeHeader,
        //        minRowIndex = 0,
        //        maxRowIndex = 0,
        //        minColumnDisplayIndex = 0,
        //        maxColumnDisplayIndex = MyDataGrid.Columns.Count - 1,
        //        GetDataFromTemplateColumn = this.GetDataFromTemplateColumn
        //    };
        //    DataGridClipboardHelper.VerifyClipboardCopy(MyDataGrid, copyInfo);

        //    LogComment("TestBadFormattedValues was successful");
        //    return TestResult.Pass;
        //}

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
