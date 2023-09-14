using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.Controls.Data;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    // Disabling for .NET Core 3, Fix and re-enable. Reproduces in .NET Framework 4.8.
    //[Test(0, "GridSplitter", "GridSplitterResizeAndIncrementTests")]
    public class GridSplitterResizeAndIncrementTests : XamlTest
    {
        #region Private Members

        private StackPanel parentPanel;

        private Dictionary<String, Object>[] testVariations;

        private String variationParameter;

        #endregion

        #region Public Members

        [Variation("ResizeBehavior")]
        [Variation("ResizeDirection")]
        [Variation("DragIncrement")]
        [Variation("KeyboardIncrement")]
        public GridSplitterResizeAndIncrementTests(String arg)
            : base(@"GridSplitterResizeAndIncrementTests.xaml")
        {
            variationParameter = arg;
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
        }

        public TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            parentPanel = RootElement.FindName("parentPanel") as StackPanel;
            if (parentPanel == null)
            {
                throw new TestValidationException("xaml StackPanel 'parentPanel' not found.");
            }

            if (variationParameter == "ResizeBehavior")
            {
                testVariations = ResizeDefaultTestData.InitializeResizeBehaviorTestVariations();
            }
            else if (variationParameter == "ResizeDirection")
            {
                testVariations = ResizeDefaultTestData.InitializeResizeDirectionTestVariations();
            }
            else if (variationParameter == "KeyboardIncrement")
            {
                testVariations = IncrementTestData.InitializeKeyboardIncrementVariations();
            }
            else if (variationParameter == "DragIncrement")
            {
                testVariations = IncrementTestData.InitializeDragIncrementVariations();
            }
            else
            {
                TestLog.Current.LogEvidence("Variation parameter: \""
                                            + variationParameter + "\" Unknown!");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            parentPanel = null;
            testVariations = null;
            return TestResult.Pass;
        }

        public TestResult RunTests()
        {
            TestResult result = TestResult.Pass;

            StringBuilder sb = new StringBuilder("VariationContext: ");
            for (Int32 idx = 0; idx < testVariations.Length; ++idx)
            {
                GridSplitterVariationContext context
                        = new GridSplitterVariationContext(testVariations[idx]);

                if (context["KNOWN_BUG"] != null)
                {
                    TestLog.Current.LogStatus(
                        "Skipping KNOWN_BUG TestCase# = " + idx.ToString() + "; " + (context["KNOWN_BUG"]).ToString());
                    continue;
                }

                GridSplitterVerifyDelegate verify = context.Verify;
                GridSplitterActionDelegate action = context.Action;
                Object[] actionArgs = context.ActionArgs;
                Object[] verifyArgs = context.VerifyArgs;
                String gridResourceKey = context["PanelGridResourceKey"] as String;
                
                #region Check variation context parameters

                if (null == verify)
                {
                    sb.AppendLine(idx.ToString() + " is missing a 'Verify' delegate specification.");
                    throw new TestValidationException(sb.ToString());
                }
                if (null == action)
                {
                    sb.AppendLine(idx.ToString() + " is missing a 'Action' delegate specification.");
                    throw new TestValidationException(sb.ToString());
                }
                if (null == actionArgs || actionArgs.Length == 0)
                {
                    sb.AppendLine(idx.ToString() + " is missing a 'ActionArgs' specification.");
                    throw new TestValidationException(sb.ToString());
                }
                if (null == verifyArgs || verifyArgs.Length == 0)
                {
                    sb.AppendLine(idx.ToString() + " is missing a 'VerifyArgs' specification.");
                    throw new TestValidationException(sb.ToString());
                }
                if (null == gridResourceKey)
                {
                    sb.AppendLine(idx.ToString() + " is missing a 'PanelGridResourceKey' resource name specification.");
                    throw new TestValidationException(sb.ToString());
                }
                #endregion

                Grid grid = null;
                GridSplitter splitter = null;
                try
                {
                    Grid sampleGrid = (Grid)parentPanel.Resources[gridResourceKey];
                    if (sampleGrid == null)
                    {
                        throw new TestValidationException(
                            String.Format("Grid resource '{0}' was not found or failed to load.", gridResourceKey));
                    }
                    grid = new Grid();
                    grid.Width = parentPanel.ActualWidth;
                    grid.Height = parentPanel.ActualHeight;

                    grid.Background = sampleGrid.Background;
                    grid.ShowGridLines = sampleGrid.ShowGridLines;

                    GridSplitterGridExtend.CloneRowColumnDefinitions(grid, sampleGrid);

                    splitter = new GridSplitter();
                    GridSplitterExtend.SetPropertiesFromVariationContext(splitter, context);

                    QueueHelper.WaitTillQueueItemsProcessed();

                    parentPanel.Children.Add(grid);
                    QueueHelper.WaitTillQueueItemsProcessed();

                    grid.Children.Add(splitter);

                    if (variationParameter == "ResizeBehavior"
                     || variationParameter == "ResizeDirection")
                    {
                        ResizeDefaultTestData.AssertResizeDefaultsTestsPreconditions(parentPanel, grid);
                    }

                    QueueHelper.WaitTillQueueItemsProcessed();

                    result = verify(idx, grid, splitter, action, actionArgs, verifyArgs);

                    if (result != TestResult.Pass) break;
                }
                finally
                {
                    if( null != grid )
                    {
                        if ( null != splitter) grid.Children.Remove(splitter);
                        parentPanel.Children.Remove(grid);
                    }
                    splitter = null;
                    grid = null;
                }
            }
            return result;
        }
        #endregion
    }
}
