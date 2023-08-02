using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Scrolling a TreeView with uniform height can yield infinite recursion (StackOverflowException)
    /// </description>
    /// </summary>
    [Test(0, "VirtualizedScrolling", "UniformAnchoredScroll", Versions="4.5+")]
    public class UniformAnchoredScroll : XamlTest
    {
        #region Private Members

        private TreeView _treeView;
        private IScrollInfo _scrollInfo;

        #endregion

        #region Test Steps

        public UniformAnchoredScroll()
            : base(@"UniformAnchoredScroll.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        private TestResult Setup()
        {
            Status("Setup");

            // Find the treeview and the players in its scrolling behavior
            _treeView = (TreeView)RootElement.FindName("treeView");
            Assert.AssertTrue("Failed to find treeView", _treeView != null);
            _scrollInfo = DataGridHelper.FindVisualChild<VirtualizingStackPanel>(_treeView) as IScrollInfo;
            Assert.AssertTrue("Failed to find the ScrollInfo of the treeview. ", _scrollInfo != null);

            // Add data to the treeview
            _treeView.ItemsSource = CreateData(4984);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        List<string> CreateData(int n)
        {
            List<string> list = new List<string>(n);
            for (int i=0; i<n; ++i)
            {
                list.Add(String.Format("Item {0}", i));
            }
            return list;
        }

        private TestResult CleanUp()
        {
            _treeView = null;
            _scrollInfo = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Repro the bug, as described below
        /// </summary>
        private TestResult Repro()
        {
            const double ContainerSpan = 20.0;      // agrees with XAML
            Status("Repro");

            // the bug repros when the following all hold
            // 1. Virtualizing is enabled
            // 2. Pixel-scrolling is selected
            // 3. Container sizes are uniform
            // 4. Containers can have virtualizing children (i.e. TreeViewItem or GroupItem)
            // 5. Anchored scroll whose target is just before a container boundary (within ~ 0.000001 px)

            // The XAML takes care of 1-4.  All we do here is (5), and hope we don't loop.

            LogComment("Scroll to starting point");
            _scrollInfo.SetVerticalOffset(1234 * ContainerSpan - 0.00000053);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("Page up");
            _scrollInfo.PageUp();   // targets just before boundary of item 1224
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // no exception - bug is fixed
            return TestResult.Pass;
        }

        #endregion
    }
}
