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
    /// An anchored scroll that reaches an item whose size has changed
    /// since it was last realized can crash with ArgumentNullException.
    /// </description>
    /// </summary>
    [Test(0, "VirtualizedScrolling", "SizeChangeDuringAnchoredScroll", Versions = "4.5.2+")]
    public class SizeChangeDuringAnchoredScroll : XamlTest
    {
        #region Private Members

        private Model model;
        private ListBox testListBox;
        private ScrollViewer listboxScrollViewer;

        #endregion

        #region Test Steps

        public SizeChangeDuringAnchoredScroll()
            : base(@"SizeChangeDuringAnchoredScroll.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        private TestResult Setup()
        {
            Status("Setup");

            // Find the listbox and its ScrollViewer
            testListBox = (ListBox)RootElement.FindName("TestListBox");
            listboxScrollViewer = DataGridHelper.FindVisualChild<ScrollViewer>(testListBox);
            Assert.AssertTrue("Failed to find the ScrollViewer of the listbox. ", listboxScrollViewer != null);

            // Add data to the listbox
            model = Model.Create();
            testListBox.DataContext = model;

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            testListBox = null;
            listboxScrollViewer = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Repro the bug, as described below
        /// </summary>
        private TestResult Repro()
        {
            LogComment("PageDown through entire list");
            double offset = 0.0, oldOffset = 0.0;
            do
            {
                oldOffset = offset;
                listboxScrollViewer.PageDown();
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                offset = listboxScrollViewer.VerticalOffset;
            }
            while (offset > oldOffset);

            LogComment("Change the height of a virtualized item");
            model.Items[10].Height = 29;

            LogComment("Scroll to a position after the item");
            listboxScrollViewer.ScrollToVerticalOffset(10.0*30 + 600.0 + listboxScrollViewer.ViewportHeight - 5.0);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("PageUp (anchored scroll) to the item");
            listboxScrollViewer.PageUp();       // causes ArgumentNullException without bug fix
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // no exception - bug is fixed
            return TestResult.Pass;
        }

        #endregion
    }
}
