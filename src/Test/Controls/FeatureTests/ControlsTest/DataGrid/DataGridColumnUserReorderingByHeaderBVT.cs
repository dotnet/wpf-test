using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// BVT tests for end user reorders columns by drag-n-drop
    ///     - clicking on a column header and then dragging and dropping it where the user desires it to be places
    ///
    /// Various factors to consider in testing:
    ///     a. preconditions and their variations
    ///     b. column header in intersts
    ///     c. drag delta
    ///     d. cancellation of the actions
    ///     e. events involved
    ///     f. drag and drop indicator
    ///     g. styles of both
    ///     h. verification of the results
    ///
    /// Not in scope: non-end user column reordering, which are covered in other tests.

    //[Test(0, "DataGrid", "DataGridColumnUserReorderingByHeaderBVT", SecurityLevel = TestCaseSecurityLevel.FullTrust, Timeout = 300)]
    public class DataGridColumnUserReorderingByHeaderBVT : XamlTest
    {
        #region Private Fields

        DataGrid dataGrid;
        Page page;
        private Control columnHeaderDropLocationIndicator = null;
        private Control columnHeaderDragIndicator = null;
        private DataGridColumnHeadersPresenter columnHeadersPresenter;
        private Style dgDragIndicatorStyle = null;
        private Style dgDropLocationIndicatorStyle = null;
        private Style dgOrigDragIndicatorStyle = null;
        private Style dgOrigDropLocationIndicatorStyle = null;
        private bool origDataGridCanUserReorderColumns;
        private int eventFiredCount;
        private int origFrozenColumnCount;
        private UserColumnDragTestData testdata;

        #endregion

        #region Public DS

        public struct UserColumnDragTestData
        {
            public bool CanUserReorderColumns;
            public int FrozenColumnCount;
            public bool CanUserReorder;
            public int OrigDisplayIndex;
            public int DragDelta;
            public bool IsCancel;
            public bool IsEventCancel;
            public int NewDisplayIndex;
            public int EventFiredCount;
            public Visibility DragIndicatorVisibility;
            public Visibility DropIndcatorVisibility;
            public Style DragStyle;
            public Style DropStyle;
        }

        #endregion

        #region Constructor

        public DataGridColumnUserReorderingByHeaderBVT()
            : base(@"DataGridColumnUserReorderingByHeaderBVT.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);

            RunSteps += new TestStep(TestValidMoveToRightNoFrozen);
            RunSteps += new TestStep(TestValidMoveToLeftNoFrozen);
            RunSteps += new TestStep(TestInValidMoveToRightNoFrozen);
            RunSteps += new TestStep(TestInValidMoveToLeftNoFrozen);
            RunSteps += new TestStep(TestReorderingInFrozenColumns);

            RunSteps += new TestStep(TestReorderingBetweenFrozenGroups);
            RunSteps += new TestStep(TestColumnReorderCancel);
            RunSteps += new TestStep(TestColumnReorderCancelThroughEvent);
            RunSteps += new TestStep(TestCanUserReorderFalse);
            RunSteps += new TestStep(TestCanUserReorderColumnsFalse);
            RunSteps += new TestStep(TestIndicatorStyles);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial setups
        /// </summary>
        /// <returns></returns>
        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            dataGrid = (DataGrid)RootElement.FindName("DataGrid_Standard");
            Assert.AssertTrue("Can not find the DataGrid in the xaml file!", dataGrid != null);
            columnHeadersPresenter = DataGridHelper.GetColumnHeadersPresenter(dataGrid);
            Assert.AssertTrue("Can not find the DataGridColumnHeaderPresenter!", columnHeadersPresenter != null);

            page = (Page)this.Window.Content;

            dgDragIndicatorStyle = (Style)(page.FindResource("dragStyle"));
            Assert.AssertTrue("Can not find the dragStyle!", dgDragIndicatorStyle != null);
            dgDropLocationIndicatorStyle = (Style)(page.FindResource("dropStyle"));
            Assert.AssertTrue("Can not find the dropStyle!", dgDropLocationIndicatorStyle != null);

            dgOrigDragIndicatorStyle = (Style)(dataGrid.DragIndicatorStyle);
            Assert.AssertTrue("The orig dragStyle should be null!", dgOrigDragIndicatorStyle == null);
            dgOrigDropLocationIndicatorStyle = (Style)(dataGrid.DropLocationIndicatorStyle);
            Assert.AssertTrue("The orig dropStyle should be null!", dgOrigDropLocationIndicatorStyle == null);

            dataGrid.CanUserReorderColumns = true;
            origDataGridCanUserReorderColumns = dataGrid.CanUserReorderColumns;
            eventFiredCount = 0;
            origFrozenColumnCount = 0;

            dataGrid.ColumnHeaderDragStarted += new EventHandler<DragStartedEventArgs>(dataGrid_ColumnHeaderDragStarted);
            dataGrid.ColumnReordering += new EventHandler<DataGridColumnReorderingEventArgs>(dataGrid_ColumnReordering);
            dataGrid.ColumnHeaderDragDelta += new EventHandler<DragDeltaEventArgs>(dataGrid_ColumnHeaderDragDelta);
            dataGrid.ColumnHeaderDragCompleted += new EventHandler<DragCompletedEventArgs>(dataGrid_ColumnHeaderDragCompleted);
            dataGrid.ColumnReordered += new EventHandler<DataGridColumnEventArgs>(dataGrid_ColumnReordered);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        TestResult CleanUp()
        {
            dataGrid.ColumnHeaderDragStarted -= dataGrid_ColumnHeaderDragStarted;
            dataGrid.ColumnReordering -= dataGrid_ColumnReordering;
            dataGrid.ColumnHeaderDragDelta -= dataGrid_ColumnHeaderDragDelta;
            dataGrid.ColumnHeaderDragCompleted -= dataGrid_ColumnHeaderDragCompleted;
            dataGrid.ColumnReordered -= dataGrid_ColumnReordered;
            dataGrid = null;
            columnHeadersPresenter = null;
            columnHeaderDropLocationIndicator = null;
            columnHeaderDragIndicator = null;
            page = null;
            dgDragIndicatorStyle = null;
            dgDropLocationIndicatorStyle = null;
            dgOrigDragIndicatorStyle = null;
            dgOrigDropLocationIndicatorStyle = null;
            return TestResult.Pass;
        }


        /// <summary>
        /// move right with positive delta
        /// </summary>
        /// <returns></returns>
        TestResult TestValidMoveToRightNoFrozen()
        {
            Status("TestValidMoveToRightNoFrozen");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 0,
                CanUserReorder = true,
                OrigDisplayIndex = 0,
                DragDelta = 150,
                IsCancel = false,
                NewDisplayIndex = 1,
                EventFiredCount = 4,
                DragIndicatorVisibility = Visibility.Visible,
                DropIndcatorVisibility = Visibility.Visible,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestValidMoveToRightNoFrozen was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// move left with negative delta
        /// </summary>
        /// <returns></returns>
        TestResult TestValidMoveToLeftNoFrozen()
        {
            Status("TestValidMoveToLeftNoFrozen");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 0,
                CanUserReorder = true,
                OrigDisplayIndex = 4,
                DragDelta = -200,
                IsCancel = false,
                NewDisplayIndex = 2,
                EventFiredCount = 4,
                DragIndicatorVisibility = Visibility.Visible,
                DropIndcatorVisibility = Visibility.Visible,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestValidMoveToLeftNoFrozen was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// move right to outside the bound
        /// </summary>
        /// <returns></returns>
        TestResult TestInValidMoveToRightNoFrozen()
        {
            Status("TestInValidMoveToRightNoFrozen");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 0,
                CanUserReorder = true,
                OrigDisplayIndex = 3,
                DragDelta = (int)dataGrid.ActualWidth,
                IsCancel = false,
                NewDisplayIndex = 7,
                EventFiredCount = 4,
                DragIndicatorVisibility = Visibility.Visible,
                DropIndcatorVisibility = Visibility.Visible,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestInValidMoveToRightNoFrozen was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// move left to outside the bound
        /// </summary>
        /// <returns></returns>
        TestResult TestInValidMoveToLeftNoFrozen()
        {
            Status("TestInValidMoveToLeftNoFrozen");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 0,
                CanUserReorder = true,
                OrigDisplayIndex = 6,
                DragDelta = -1 * ((int)dataGrid.ActualWidth),
                IsCancel = false,
                NewDisplayIndex = 6,
                EventFiredCount = 0,
                DragIndicatorVisibility = Visibility.Collapsed,
                DropIndcatorVisibility = Visibility.Collapsed,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestInValidMoveToLeftNoFrozen was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test reordering within same frozen group
        /// </summary>
        /// <returns></returns>
        TestResult TestReorderingInFrozenColumns()  // 
        {
            Status("TestReorderingInFrozenColumns");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 2,
                CanUserReorder = true,
                OrigDisplayIndex = 1,
                DragDelta = -150,
                IsCancel = false,
                NewDisplayIndex = 0,
                EventFiredCount = 4,
                DragIndicatorVisibility = Visibility.Visible,
                DropIndcatorVisibility = Visibility.Visible,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestReorderingInFrozenColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test reordering between frozen groups
        /// </summary>
        /// <returns></returns>
        TestResult TestReorderingBetweenFrozenGroups()
        {
            Status("TestReorderingBetweenFrozenGroups");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 2,
                CanUserReorder = true,
                OrigDisplayIndex = 1,
                DragDelta = 350,
                IsCancel = false,
                NewDisplayIndex = 1,
                EventFiredCount = 3,  // 
                DragIndicatorVisibility = Visibility.Visible,  // 
                DropIndcatorVisibility = Visibility.Visible,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestReorderingBetweenFrozenGroups was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test reordering cancelled
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnReorderCancel()
        {
            Status("TestColumnReorderCancel");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 2,
                CanUserReorder = true,
                OrigDisplayIndex = 5,
                DragDelta = 150,
                IsCancel = true,
                NewDisplayIndex = 5,
                EventFiredCount = 0,
                DragIndicatorVisibility = Visibility.Collapsed,
                DropIndcatorVisibility = Visibility.Collapsed,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestColumnReorderCancel was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test reordering cancelled by DataGridColumnReorderingEventArgs
        /// </summary>
        /// <returns></returns>
        TestResult TestColumnReorderCancelThroughEvent()
        {
            Status("TestColumnReorderCancelThroughEvent");
                        
            dataGrid.ColumnReordering += dataGrid_ColumnReorderingWithCancel;
            QueueHelper.WaitTillQueueItemsProcessed();

            try
            {
                testdata = new UserColumnDragTestData
                {
                    CanUserReorderColumns = true,
                    FrozenColumnCount = 2,
                    CanUserReorder = true,
                    OrigDisplayIndex = 5,
                    DragDelta = 150,
                    IsCancel = false,
                    IsEventCancel = true,
                    NewDisplayIndex = 5,
                    EventFiredCount = 0,
                    DragIndicatorVisibility = Visibility.Collapsed,
                    DropIndcatorVisibility = Visibility.Collapsed,
                    DragStyle = new Style(),
                    DropStyle = new Style()
                };
                this.DoDragAndVerify(testdata);
            }
            finally
            {
                dataGrid.ColumnReordering -= dataGrid_ColumnReorderingWithCancel;                
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            LogComment("TestColumnReorderCancelThroughEvent was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test column.CanUserReorder=false
        /// </summary>
        /// <returns></returns>
        TestResult TestCanUserReorderFalse()
        {
            Status("TestCanUserReorderFalse");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 2,
                CanUserReorder = false,
                OrigDisplayIndex = 5,
                DragDelta = 150,
                IsCancel = false,
                NewDisplayIndex = 5,
                EventFiredCount = 0,
                DragIndicatorVisibility = Visibility.Collapsed,
                DropIndcatorVisibility = Visibility.Collapsed,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestCanUserReorderFalse was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test the DG.CanUserReorderColumns=false
        /// </summary>
        /// <returns></returns>
        TestResult TestCanUserReorderColumnsFalse()
        {
            Status("TestCanUserReorderColumnsFalse");

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = false,
                FrozenColumnCount = 2,
                CanUserReorder = true,
                OrigDisplayIndex = 6,
                DragDelta = 150,
                IsCancel = false,
                NewDisplayIndex = 6,
                EventFiredCount = 0,
                DragIndicatorVisibility = Visibility.Collapsed,
                DropIndcatorVisibility = Visibility.Collapsed,
                DragStyle = new Style(),
                DropStyle = new Style()
            };
            this.DoDragAndVerify(testdata);

            LogComment("TestCanUserReorderColumnsFalse was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Test the custom styles .
		/// </summary>

        TestResult TestIndicatorStyles()
        {
            Status("TestIndicatorStyles");

            // apply the new style
            dataGrid.DragIndicatorStyle = this.dgDragIndicatorStyle;
            dataGrid.DropLocationIndicatorStyle = this.dgDropLocationIndicatorStyle;

            testdata = new UserColumnDragTestData
            {
                CanUserReorderColumns = true,
                FrozenColumnCount = 0,
                CanUserReorder = true,
                OrigDisplayIndex = 1,
                DragDelta = (int)dataGrid.ActualWidth,
                IsCancel = false,
                NewDisplayIndex = 5,
                EventFiredCount = 4,
                DragIndicatorVisibility = Visibility.Visible,
                DropIndcatorVisibility = Visibility.Visible,
                DragStyle = this.dgDragIndicatorStyle,
                DropStyle = this.dgDropLocationIndicatorStyle
            };
            this.DoDragAndVerify(testdata);

            // reset
            dataGrid.DragIndicatorStyle = this.dgOrigDragIndicatorStyle;
            dataGrid.DropLocationIndicatorStyle = this.dgOrigDropLocationIndicatorStyle;

            LogComment("TestIndicatorStyles was successful");
            return TestResult.Pass;
        }

        #endregion

        #region Drag and Drop Event Handlers

        void dataGrid_ColumnReordering(object sender, DataGridColumnReorderingEventArgs e)
        {
            eventFiredCount++;

            columnHeaderDragIndicator = e.DragIndicator;
            columnHeaderDropLocationIndicator = e.DropLocationIndicator;
        }

        void dataGrid_ColumnReorderingWithCancel(object sender, DataGridColumnReorderingEventArgs e)
        {
            e.Cancel = true;
        }

        void dataGrid_ColumnHeaderDragStarted(object sender, DragStartedEventArgs e)
        {
            eventFiredCount++;
        }

        void dataGrid_ColumnHeaderDragDelta(object sender, DragDeltaEventArgs e)
        {
            eventFiredCount++;
        }

        void dataGrid_ColumnHeaderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            eventFiredCount++;
        }

        void dataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
            eventFiredCount++;
        }

        #endregion

        #region Actions and Verifications

        private void DoDragAndVerify(UserColumnDragTestData dragTestData)
        {
            // preconditions
            dataGrid.CanUserReorderColumns = dragTestData.CanUserReorderColumns;
            dataGrid.FrozenColumnCount = dragTestData.FrozenColumnCount;
            QueueHelper.WaitTillQueueItemsProcessed();

            // get the test column and make sure it can be reordered
            DataGridColumn column = dataGrid.ColumnFromDisplayIndex(dragTestData.OrigDisplayIndex);
            column.CanUserReorder = dragTestData.CanUserReorder;
            QueueHelper.WaitTillQueueItemsProcessed();

            // get the column header
            DataGridColumnHeader header = (DataGridColumnHeader)DataGridHelper.GetColumnHeaderFromDisplayIndex(dataGrid, dragTestData.OrigDisplayIndex);
            LogComment("The header text is " + header.Content.ToString());

            // drag
            this.DragColumnHeader(
                header,
                dragTestData.DragDelta,
                2,
                dragTestData.IsCancel,
                dragTestData.IsEventCancel,
                dragTestData.DragIndicatorVisibility,
                dragTestData.DropIndcatorVisibility);

            // ...and verify
            if (dragTestData.DragIndicatorVisibility == Visibility.Visible && dragTestData.DragStyle.Setters.Count > 0)
            {
                Assert.AssertEqual(string.Format("The column drag indicator style is not correct"),
                    dragTestData.DragStyle,
                    column.DragIndicatorStyle);
            }
            Assert.AssertEqual(string.Format("The new displayIndex should be {0} for the column {1}",
                dragTestData.NewDisplayIndex,
                dragTestData.OrigDisplayIndex),
                dragTestData.NewDisplayIndex,
                column.DisplayIndex);
            Assert.AssertEqual(string.Format("The event fired count should be {0}",
                dragTestData.EventFiredCount),
                dragTestData.EventFiredCount,
                eventFiredCount);

            // reset
            dataGrid.CanUserReorderColumns = origDataGridCanUserReorderColumns;
            dataGrid.FrozenColumnCount = origFrozenColumnCount;
            eventFiredCount = 0;
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        /// <summary>
        /// Drag column header, can be changed to FE drag easily
        /// </summary>
        /// <param name="header"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="cancel"></param>
        /// <param name="drag"></param>
        /// <param name="drop"></param>
        private void DragColumnHeader(
            DataGridColumnHeader header,
            int x,
            int y,
            bool cancel,
            bool eventCancel,
            Visibility drag,
            Visibility drop)
        {
            LogComment("MouseLeftButtonDown on header: " + header.Content.ToString());
            UserInput.MouseLeftDownCenter(header);
            QueueHelper.WaitTillQueueItemsProcessed();

            int xDelta = x + (int)(header.RenderSize.Width / 2);
            LogComment("MouseMove on header, delta: " + xDelta);
            UserInput.MouseMove(header, xDelta, y);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!eventCancel)
            {
                Assert.AssertTrue("The drag indicator should not be null", columnHeaderDragIndicator != null);
                Assert.AssertTrue("The drop indicator should not be null", columnHeaderDropLocationIndicator != null);
                Assert.AssertEqual("The drag indicator should be " + drag.ToString(), drag, columnHeaderDropLocationIndicator.Visibility);
                Assert.AssertEqual("The drop indicator should be " + drop.ToString(), drop, columnHeaderDropLocationIndicator.Visibility);
            }

            if (cancel)
            {
                UserInput.KeyPress("Escape");
                QueueHelper.WaitTillQueueItemsProcessed();
            }

            LogComment("MouseLeftButtonUp on header: " + header.Content.ToString());
            UserInput.MouseLeftUp(header, xDelta, y);            
            QueueHelper.WaitTillQueueItemsProcessed();           
        }

        #endregion

    }
}
