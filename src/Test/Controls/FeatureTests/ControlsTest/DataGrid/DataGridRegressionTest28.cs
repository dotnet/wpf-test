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


namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression test : Verify ArgumentException is not thrown when resizing a column initially set
    /// to SizeToCell and an empty ItemsSource.
    /// </description>

    /// </summary>
    [Test(0, "DataGrid", "DataGridRegressionTest28", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class DataGridRegressionTest28 : DataGridTest
    {
        private Button debugButton;

        #region Constructor

        public DataGridRegressionTest28()
            : base(@"DataGridRegressionTest28.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestResizeColumnHeaderOnEmptyList);
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

            Status("Setup specific for DataGridRegressionTest28");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            //Debug();

            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup for DataGridRegressionTest28 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            debugButton = null;

            return base.CleanUp();
        }
        /// <summary>
        /// 1. have ItemsSource set to an empty/null list.  have the first column set to SizeToCell
        /// 2. Resize the first column        
        /// 
        /// Verify: column is able to resize without any exceptions being thrown
        /// </summary>
        private TestResult TestResizeColumnHeaderOnEmptyList()
        {
            Status("TestResizeColumnHeaderOnEmptyList");

            double originalWidth = MyDataGrid.Columns[0].ActualWidth;

            // get the gripper and resize column header
            Thumb rightThumbGripper = DataGridHelper.GetColumnHeaderGripper(MyDataGrid, 0);
            FrameworkElement elem = rightThumbGripper as FrameworkElement;
            if (elem == null)
            {
                throw new NullReferenceException("RightThumbGripper is either null or could not be converted to a FrameworkElement.");
            }

            this.WaitFor(1000);

            UserInput.MouseMove(elem, 0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftDown(elem);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(elem, 100, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftUp(elem);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(0, 0);
            QueueHelper.WaitTillQueueItemsProcessed();

            this.WaitFor(1000);
            this.WaitForPriority(DispatcherPriority.ApplicationIdle);

            // verify width updated
            double actualWidth = MyDataGrid.Columns[0].ActualWidth;
            if (originalWidth == actualWidth)
            {
                throw new TestValidationException(string.Format(
                    "Column Width should have updated.  Expected width: {0}, actual width: {1}",
                    originalWidth,
                    actualWidth));
            }

            //
            // if made it this far, no exception has occurred, return pass
            //

            LogComment("TestResizeColumnHeaderOnEmptyList was successful");
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
