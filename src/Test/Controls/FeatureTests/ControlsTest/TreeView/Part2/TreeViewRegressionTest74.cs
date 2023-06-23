using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// TreeViewRegressionTest74
    /// When using a treeview set VirtualizingStackPanel.IsVirtualizing true and top margin not 0, scroll the scrollbar will make the window hung.
    /// </description>
    /// </summary>
    [Test(0, "TreeView", "TreeViewRegressionTest74", Versions = "4.5.1+")]
    public class TreeViewRegressionTest74 : XamlTest
    {
        #region Private Members

        private TreeView testTreeview;
        private ScrollBar treeviewScrollBar;
        private Track scrollBarTrack;
        private RepeatButton increaseRepeatButton;

        #endregion

        #region Test Steps

        public TreeViewRegressionTest74()
            : base(@"TreeViewRegressionTest74.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        private TestResult Setup()
        {
            Status("Setup");

            // Find the treeview and find its scrollbar
            testTreeview = (TreeView)RootElement.FindName("TestTreeView");
            treeviewScrollBar = DataGridHelper.FindVisualChild<ScrollBar>(testTreeview);
            Assert.AssertTrue("Failed to find the scrollBar of the treeview. ", treeviewScrollBar != null);

            // Get the inner IncreaseRepeatButton of the Scrollbar
            scrollBarTrack = (Track)treeviewScrollBar.Template.FindName("PART_Track", treeviewScrollBar);
            Assert.AssertTrue("Failed to find the track of the scrollBar. ", scrollBarTrack != null);
            increaseRepeatButton = scrollBarTrack.IncreaseRepeatButton;
            Assert.AssertTrue("Failed to find the increaseRepeatButton of the track. ", increaseRepeatButton != null);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            testTreeview = null;
            treeviewScrollBar = null;
            scrollBarTrack = null;
            increaseRepeatButton = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Only scroll down the scrollbar , if the window hung , test will fail due to time out.
        /// </summary>
        private TestResult Repro()
        {
            LogComment("Try to click increaseRepeatebutton to scroll");
            UserInput.MouseLeftClickCenter(increaseRepeatButton);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("ScrollBar scrolled and treeView worked fine.");
            return TestResult.Pass;
        }

        #endregion
    }
}
