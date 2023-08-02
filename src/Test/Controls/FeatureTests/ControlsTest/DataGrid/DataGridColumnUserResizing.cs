using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Linq;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Tests end-user column width resizing.
    /// </description>

    /// </summary>
    // [ DISABLED_WHILE_PORTING ]
    [Test(0, "DataGrid", "DataGridColumnUserResizing", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite", Disabled=true)]
    public class DataGridColumnUserResizing : DataGridColumnUserResizingStarBase
    {
        #region Constructor

        public DataGridColumnUserResizing()
            : base(@"DataGridBasicRowColumnHeaderSizing.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestCanUserResizeColumns);
            RunSteps += new TestStep(TestDraggingRightColumnSeparatorRight);
            RunSteps += new TestStep(TestDraggingRightColumnSeparatorLeft);
            RunSteps += new TestStep(TestDraggingRightColumnSeparatorRightPastMaxWidth);
            RunSteps += new TestStep(TestDraggingRightColumnSeparatorLeftPastMinWidth);            
            RunSteps += new TestStep(TestDoubleClickOnColumnSeparator);

            //

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

            Status("Setup specific for DataGridColumnResizing");

            this.SetupDataSource();

            LogComment("Setup for DataGridColumnResizing was successful");
            return TestResult.Pass;
        }        

        /// <summary>
        ///  Verify when CanUserResizeColumns set to false, user cannot use separators 
        ///  to resize columns.  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestCanUserResizeColumns()
        {
            Status("TestCanUserResizeColumns");

            // disallow column resizing
            MyDataGrid.CanUserResizeColumns = false;

            // try to resize each column
            for (int i = 0; i < MyDataGrid.Columns.Count; i++)
            {
                double originalWidth = MyDataGrid.Columns[i].ActualWidth;

                // get the gripper and resize column header
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, i, 20);

                this.WaitForPriority(DispatcherPriority.ApplicationIdle);

                // verify width did not change
                double actualWidth = MyDataGrid.Columns[i].ActualWidth;
                if (originalWidth != actualWidth)
                {
                    LogComment(string.Format(
                        "Expected width: {0}, actual width: {1}",
                        originalWidth,
                        actualWidth));
                    return TestResult.Fail;
                }
            }
            
            LogComment("TestCanUserResizeColumns was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify dragging right column separator right widens the column and 
        /// header for each column width type.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDraggingRightColumnSeparatorRight()
        {
            Status("TestDraggingRightColumnSeparatorRight");

            LogComment("allow column resizing"); 
            MyDataGrid.CanUserResizeColumns = true;

            LogComment("Resize each column right and verify");
            for (int i = 0; i < MyDataGrid.Columns.Count; i++)
            {
                List<double> headerWidths = DataGridHelper.GetColumnHeaderWidths(MyDataGrid);

                // get the gripper and resize column header                
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, i, 20);

                // verify columns are resized correctly
                DataGridVerificationHelper.VerifyColumnResizing(MyDataGrid, headerWidths, i, 20);                
            }

            LogComment("TestDraggingRightColumnSeparatorRight was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify dragging right column separator left shrinks the column and header
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDraggingRightColumnSeparatorLeft()
        {
            Status("TestDraggingRightColumnSeparatorLeft");

            LogComment("allow column resizing");
            MyDataGrid.CanUserResizeColumns = true;

            LogComment("Resize each column left and verify");
            for (int i = 0; i < MyDataGrid.Columns.Count; i++)
            {
                List<double> headerWidths = DataGridHelper.GetColumnHeaderWidths(MyDataGrid);

                // get the gripper and resize column header                                
                DataGridActionHelper.ResizeColumnHeader(MyDataGrid, i, -10);

                // verify columns are resized correctly
                DataGridVerificationHelper.VerifyColumnResizing(MyDataGrid, headerWidths, i, -10);
            }

            LogComment("TestDraggingRightColumnSeparatorLeft was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify dragging right column separator right past MaxWidth widens the column 
        /// and header to at most MaxWidth.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDraggingRightColumnSeparatorRightPastMaxWidth()
        {
            Status("TestDraggingRightColumnSeparatorRightPastMaxWidth");

            LogComment("allow column resizing");
            MyDataGrid.CanUserResizeColumns = true;
            
            List<double> headerWidths = DataGridHelper.GetColumnHeaderWidths(MyDataGrid);

            // get the expected size
            DataGridTemplateColumn templateColumn = MyDataGrid.FindName("templateColumnWithMinMax") as DataGridTemplateColumn;
            int colIndex = DataGridHelper.FindColumnIndex(MyDataGrid, templateColumn);
            double expectedSize = templateColumn.MaxWidth - templateColumn.ActualWidth;

            // get the gripper and resize column header
            DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, 500);
            
            DataGridVerificationHelper.VerifyColumnResizing(MyDataGrid, headerWidths, colIndex, expectedSize + 4);

            //

            LogComment("TestDraggingRightColumnSeparatorRightPastMaxWidth was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify dragging right column separator left past MinWidth shrinks 
        /// the column and header no less than MinWidth.
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDraggingRightColumnSeparatorLeftPastMinWidth()
        {
            Status("TestDraggingRightColumnSeparatorLeftPastMinWidth");

            LogComment("allow column resizing");
            MyDataGrid.CanUserResizeColumns = true;

            List<double> headerWidths = DataGridHelper.GetColumnHeaderWidths(MyDataGrid);

            // get the expected size
            DataGridTemplateColumn templateColumn = MyDataGrid.FindName("templateColumnWithMinMax") as DataGridTemplateColumn;
            int colIndex = DataGridHelper.FindColumnIndex(MyDataGrid, templateColumn);
            double expectedSizeDiff = templateColumn.MinWidth - templateColumn.ActualWidth;

            // get the gripper and resize column header
            DataGridActionHelper.ResizeColumnHeader(MyDataGrid, colIndex, -400);

            DataGridVerificationHelper.VerifyColumnResizing(MyDataGrid, headerWidths, colIndex, expectedSizeDiff + 4);
            
            //

            LogComment("TestDraggingRightColumnSeparatorLeftPastMinWidth was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify double-click behavior with column separators.  On double-click,
        /// header should change to "Auto".
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDoubleClickOnColumnSeparator()
        {
            Status("TestDoubleClickOnColumnSeparator");

            LogComment("allow column resizing");
            MyDataGrid.CanUserResizeColumns = true;

            LogComment("double-click each column separator and verify");
            for (int i = 0; i < MyDataGrid.Columns.Count; i++)
            {
                // navigate into view
                DataGridActionHelper.NavigateTo(MyDataGrid, 0, i);

                // get the gripper and do a double click
                DataGridActionHelper.DoubleClickColumnHeaderGripper(MyDataGrid, i);

                DataGridVerificationHelper.VerifyColumnWidthType(MyDataGrid, i, DataGridLengthUnitType.Auto);
            }

            LogComment("TestDoubleClickOnColumnSeparator was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}
