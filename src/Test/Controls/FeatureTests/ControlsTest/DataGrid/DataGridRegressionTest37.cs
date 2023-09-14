using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Automation;
using System.Windows.Controls;
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
    /// Regression Test   
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest37", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest37 : XamlTest
    {
        private string id;
        private DataGrid datagrid;
        private int headerIndex;
        private int moveDelta;
        private HeaderGripper headerGripper;
        private MouseMoveDirection mouseMoveDirection;

        #region Constructor
        
        // Test Star Column
        [Variation("1", 1, HeaderGripper.Left, 15, MouseMoveDirection.Left, "DataGridRegressionTest37.xaml")]
        [Variation("2", 1, HeaderGripper.Left, 15, MouseMoveDirection.Right, "DataGridRegressionTest37.xaml")]

        // Test right most Auto Column
        [Variation("3", 2, HeaderGripper.Left, 15, MouseMoveDirection.Left, "DataGridRegressionTest37.xaml")]
        [Variation("4", 2, HeaderGripper.Left, 15, MouseMoveDirection.Right, "DataGridRegressionTest37.xaml")]
        public DataGridRegressionTest37(string id, int headerIndex, HeaderGripper headerGripper, int moveDelta, MouseMoveDirection mouseMoveDirection, string fileName)
            : base(fileName)
        {
            this.id = id;
            this.headerIndex = headerIndex;
            this.headerGripper = headerGripper;
            this.moveDelta = moveDelta;
            this.mouseMoveDirection = mouseMoveDirection;
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(ResizeDataGridColumn);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            LogComment(String.Format("# TestID is {0} #", id));

            datagrid = (DataGrid)RootElement.FindName("datagrid");
            datagrid.ItemsSource = new List<MyDataGridRow>
            {
                new MyDataGridRow{Column1="aaa", Column2="bbb", Column3=111111},
                new MyDataGridRow{Column1="cccccccc", Column2="dddddddddddd", Column3=222222}
            };

            LogComment("Setup for DataGridRegressionTest37 was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// </summary>
        private TestResult ResizeDataGridColumn()
        {
            DataGridColumnSizingValidator validator = new DataGridColumnSizingValidator(datagrid, headerIndex, headerGripper, moveDelta, mouseMoveDirection);
            validator.Run();

            return TestResult.Pass;
        }

        #endregion Test Steps
    }

    public class MyDataGridRow
    {
        public string Column1 { set; get; }
        public string Column2 { set; get; }
        public long Column3 { set; get; }
    }
}
