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
using Avalon.Test.ComponentModel;
using System;

namespace Microsoft.Test.Controls
{
    // [ DISABLED_WHILE_PORTING ]
    [Test(0, "DataGrid", "RegressionTest34", SecurityLevel = TestCaseSecurityLevel.FullTrust, Disabled=true)]
    public class DataGridRegressionTest34 : DataGridColumnUserResizingStarBase
    {
        #region Constructor
        public DataGridRegressionTest34()
            : base(@"DataGridBasicRowColumnHeaderSizing.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestDragLeftSeparatorLeft);
            RunSteps += new TestStep(TestDragLeftSeparatorRight);        
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
        /// Verify left separator drags left when left side col not visible
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDragLeftSeparatorLeft()
        {
            Status("TestDragLeftSeparatorLeft");

            LogComment("allow column resizing");
            MyDataGrid.CanUserResizeColumns = true;

            int count = MyDataGrid.Columns.Count;            

            for (int i = 0; i < count - 2; i++)
            {
                double width = MyDataGrid.Columns[i].ActualWidth;
                if((width - 10)>20)
                {
                    List<double> headerWidths = DataGridHelper.GetColumnHeaderWidths(MyDataGrid);
                    LogComment("Collapse first column");
                    MyDataGrid.Columns[i + 1].Visibility = System.Windows.Visibility.Collapsed;

                    LogComment("Resize column on left and verify");

                    // get the gripper and resize column header                                    
                    DataGridActionHelper.ResizeColumnHeaderLeft(MyDataGrid, i + 2, -10);

                    // verify columns are resized correctly
                    MyDataGrid.Columns[i + 1].Visibility = System.Windows.Visibility.Visible;
                    QueueHelper.WaitTillQueueItemsProcessed();
                    DataGridVerificationHelper.VerifyColumnResizing(MyDataGrid, headerWidths, i, -10);
                }
            }
            LogComment("TestDragLeftSeparatorLeft was successful");
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify left separator drags right when left side col not visible        
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        private TestResult TestDragLeftSeparatorRight()
        {
            Status("TestDragLeftSeparatorRight");

            LogComment("allow column resizing");
            MyDataGrid.CanUserResizeColumns = true;

            int count = MyDataGrid.Columns.Count;

            for (int i = 0; i < count - 2; i++)
            {
                double width = MyDataGrid.Columns[i].ActualWidth;                
                List<double> headerWidths = DataGridHelper.GetColumnHeaderWidths(MyDataGrid);
                LogComment("Collapse first column");
                MyDataGrid.Columns[i + 1].Visibility = System.Windows.Visibility.Collapsed;

                LogComment("Resize column on left and verify");

                // get the gripper and resize column header                                    
                DataGridActionHelper.ResizeColumnHeaderLeft(MyDataGrid, i + 2, 10);

                // verify columns are resized correctly
                MyDataGrid.Columns[i + 1].Visibility = System.Windows.Visibility.Visible;
                QueueHelper.WaitTillQueueItemsProcessed();
                DataGridVerificationHelper.VerifyColumnResizing(MyDataGrid, headerWidths, i, 10);                
            }
            LogComment("TestDragLeftSeparatorRight was successful");
            return TestResult.Pass;
        }
#endregion
    }
}
