using System;
using System.Windows.Controls;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Regression Test 
    /// This was caused by a precision error in the floating point computation done by GridSplitter.
    /// </summary>
    [Test(1, "GridSplitter", "GridSplitterRegressionTest57")]
    public class GridSplitterRegressionTest57 : XamlTest
    {
        #region Private Members
        GridSplitter gridSplitter;
        ColumnDefinition columnDefinition1;
        #endregion

        #region Public Members

        public GridSplitterRegressionTest57()
            : base(@"GridSplitterRegressionTest57.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(MouseClickButton);
        }

        public TestResult Setup()
        {
            Status("Setup");

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            gridSplitter = (GridSplitter)RootElement.FindName("gridSplitter");
            if (gridSplitter == null)
            {
                throw new TestValidationException("GridSplitter is null");
            }

            columnDefinition1 = (ColumnDefinition)RootElement.FindName("columnDefinition1");
            if (columnDefinition1 == null)
            {
                throw new TestValidationException("ColumnDefinition is null");
            }

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            gridSplitter = null;
            columnDefinition1 = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Mouse drag splitter to right and validate the first ColumnDefinition width is not decreasing.
        /// </summary>
        /// <returns></returns>
        public TestResult MouseClickButton()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            Panel panel = (Panel)gridSplitter.Parent;
            
            // Store the first ColumnDefinition initial width value.
            double initialWidth = columnDefinition1.ActualWidth;

            // Mouse drag splitter to right most of GridSplitter parent.
            UserInput.MouseLeftDownCenter(gridSplitter);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseMove(gridSplitter, Convert.ToInt32(panel.ActualWidth), 0);
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.MouseLeftUpCenter(gridSplitter);
            QueueHelper.WaitTillQueueItemsProcessed();

            // Validating the first ColumnDefinition width is not decreasing.
            if (initialWidth > columnDefinition1.ActualWidth)
            {
                throw new TestValidationException("Inital width " + initialWidth + " is bigger than current width " + gridSplitter.ActualWidth + " after draged splitter to right side.");
            }

            return TestResult.Pass;
        }

        #endregion
    } 
}
