using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Test.Controls.DataSources;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Input;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Code coverage test for testing DataGridColumnReorderingEventArgs.Cancel.  
    /// </description>
    /// </summary>
    [Test(0, "DataGrid", "DataGridColumnReorderingEventArgsCC", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridColumnReorderingEventArgsCC : XamlTest
    {
        private DataGrid datagrid;
        private DataGridColumnHeader firstColumnHeader;

        #region Constructor

        public DataGridColumnReorderingEventArgsCC()
            : base("DataGridColumnReorderingEventArgsCC.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        public TestResult Setup()
        {
            datagrid = (DataGrid)RootElement.FindName("datagrid");
            datagrid.ColumnReordering += new EventHandler<DataGridColumnReorderingEventArgs>(datagrid_ColumnReordering);

            LogComment("Setup for DataGridColumnReorderingEventArgsCC was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Try to move the first columne to the second column
        /// </summary>
        private TestResult RunTest()
        {
            LogComment("RunTest");

            firstColumnHeader = DataGridHelper.GetColumnHeader(datagrid, 0);
            DataGridColumnHeader secondColumnHeader = DataGridHelper.GetColumnHeader(datagrid, 1);
            Point secondHeaderPoint = secondColumnHeader.PointToScreen(new Point());

            InputHelper.MouseMoveToElementCenter(firstColumnHeader);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Mouse.Down(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Mouse.MoveTo(new System.Drawing.Point(Convert.ToInt32(secondHeaderPoint.X + secondColumnHeader.ActualWidth * 2), Convert.ToInt32(secondHeaderPoint.Y + secondColumnHeader.ActualHeight / 2)));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            Microsoft.Test.Input.Mouse.Up(Microsoft.Test.Input.MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        /// <summary>
        /// Validate the first column did not move to the second column because we canceled it on ColumnReordering callback
        /// </summary>
        private TestResult Verify()
        {
            LogComment("Verify");

            DataGridColumnHeader targetColumnHeader = DataGridHelper.GetColumnHeader(datagrid, 0);
            if (!Object.ReferenceEquals(targetColumnHeader, firstColumnHeader))
            {
                throw new TestValidationException("We could still move column after we canceled it.");
            }

            return TestResult.Pass;
        }

        private void datagrid_ColumnReordering(object sender, DataGridColumnReorderingEventArgs e)
        {
            // Set DataGridColumnReorderingEventArgs Cancel to true for Code coverage hole
            e.Cancel = true;
        }

        #endregion Test Steps
    }
}
