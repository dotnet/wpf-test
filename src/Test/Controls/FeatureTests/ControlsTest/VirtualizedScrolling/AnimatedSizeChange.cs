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
    /// Using VisualStateManager (hence storyboards/animation) to change the
    /// visibility of the ItemsHost leads to loops and/or scroll-to-wrong-place
    /// </description>
    /// </summary>
    [Test(0, "VirtualizedScrolling", "AnimatedSizeChange", Versions="4.7.2+", TestParameters="ContainerStyle=Custom, Mode=Recycling, Expand=9.8.5.4.0, Scroll=Up")]
    [Test(0, "VirtualizedScrolling", "AnimatedSizeChange", Versions="4.7.2+", TestParameters="ContainerStyle=Custom, Mode=Recycling, Expand=9.8.5.4.0, Scroll=Down")]
    [Test(0, "VirtualizedScrolling", "AnimatedSizeChange", Versions="4.7.2+", TestParameters="ContainerStyle=Custom, Mode=Standard, Expand=9.8.5.4.0, Scroll=Up")]
    [Test(0, "VirtualizedScrolling", "AnimatedSizeChange", Versions="4.7.2+", TestParameters="ContainerStyle=Custom, Mode=Standard, Expand=9.8.5.4.0, Scroll=Down")]
    [Test(0, "VirtualizedScrolling", "AnimatedSizeChange", Versions="4.7.2+", TestParameters="ContainerStyle=Builtin, Mode=Recycling, Expand=9.8.5.4.0, Scroll=Up")]
    [Test(0, "VirtualizedScrolling", "AnimatedSizeChange", Versions="4.7.2+", TestParameters="ContainerStyle=Builtin, Mode=Recycling, Expand=9.8.5.4.0, Scroll=Down")]
    [Test(0, "VirtualizedScrolling", "AnimatedSizeChange", Versions="4.7.2+", TestParameters="ContainerStyle=Builtin, Mode=Standard, Expand=9.8.5.4.0, Scroll=Up")]
    [Test(0, "VirtualizedScrolling", "AnimatedSizeChange", Versions="4.7.2+", TestParameters="ContainerStyle=Builtin, Mode=Standard, Expand=9.8.5.4.0, Scroll=Down")]
    public class AnimatedSizeChange : XamlTest
    {
        #region Private Members

        private ObservableCollection<NodeViewModel> data;
        private TreeView testTreeView;
        private IScrollInfo scrollInfo;

        #endregion

        #region Test Steps

        public AnimatedSizeChange()
            : base(@"AnimatedSizeChange.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        private TestResult Setup()
        {
            Status("Setup");

            LogComment("Parameters:");
            foreach (KeyValuePair<string,string> kvp in DriverState.DriverParameters)
            {
                LogComment(String.Format("   {0}: {1}", kvp.Key, kvp.Value));
            }

            // Find the treeview and the players in its scrolling behavior
            string tvName = String.Format("{0}TreeView", DriverState.DriverParameters["Mode"]);
            testTreeView = (TreeView)RootElement.FindName(tvName);
            Assert.AssertTrue("Failed to find treeview named " + tvName, testTreeView != null);
            scrollInfo = DataGridHelper.FindVisualChild<VirtualizingStackPanel>(testTreeView) as IScrollInfo;
            Assert.AssertTrue("Failed to find the ScrollInfo of the treeview. ", scrollInfo != null);
            scrollInfo.ScrollOwner.ScrollChanged += OnScrollChanged;

            // set the container style from test parameters
            switch (DriverState.DriverParameters["ContainerStyle"])
            {
                case "Custom":
                    testTreeView.ItemContainerStyle = (Style)testTreeView.FindResource("DefaultItemContainerStyle");
                    break;
            }

            // Add data to the treeview
            CreateData();
            testTreeView.ItemsSource = data;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        void CreateData()
        {
            data = new ObservableCollection<NodeViewModel>();
            data.Add(new NodeViewModel() { Name = "Root" });

            var item = data[0];

            for (int i = 0; i < 10; i++)
            {
                var child = new NodeViewModel() { Name = i.ToString() };
                item.Children.Add(child);

                for (int j = 0; j < (i %4== 0 ? 20 : 7); j++)
                    child.Children.Add(new NodeViewModel() { Name = j.ToString() });
            }
        }

        private TestResult CleanUp()
        {
            testTreeView = null;
            scrollInfo.ScrollOwner.ScrollChanged -= OnScrollChanged;
            scrollInfo = null;
            return TestResult.Pass;
        }

        /// <summary>
        /// Repro the bug, as described below
        /// </summary>
        private TestResult Repro()
        {
            LogComment("Expand the root");
            TreeViewItem root = (TreeViewItem)testTreeView.ItemContainerGenerator.ContainerFromIndex(0);
            root.IsExpanded = true;
            CheckForSoftLoop();

            LogComment("Expand the desired children");
            string[] a = DriverState.DriverParameters["Expand"].Split('.');
            foreach (string s in a)
            {
                int index;
                if (Int32.TryParse(s, out index))
                {
                    TreeViewItem child = (TreeViewItem)root.ItemContainerGenerator.ContainerFromIndex(index);
                    if (child != null)
                    {
                        child.IsExpanded = true;
                        CheckForSoftLoop();
                    }
                }
            }

            bool down = (DriverState.DriverParameters["Scroll"] == "Down");
            double extent = scrollInfo.ExtentHeight;

            LogComment("Move to desired edge");
            scrollInfo.SetVerticalOffset(down ? 0.0 : Double.PositiveInfinity);
            CheckForSoftLoop();
            double offset = scrollInfo.VerticalOffset, oldOffset = offset;
            double expectedDelta = Double.PositiveInfinity;

            LogComment("Scroll through the TreeView");
            do
            {
                oldOffset = offset;
                if (down)   scrollInfo.MouseWheelDown();
                else        scrollInfo.MouseWheelUp();
                CheckForSoftLoop();

                // after the scroll completes (including measure-cache, animated
                // size changes, and any other background work), the
                // extent shouldn't have changed and the offset should change by
                // the expected amount.  (Both quantities may change transiently
                // during the background work.)
                Assert.AssertTrue("Extent changed during scroll", LayoutDoubleUtil.AreClose(extent, scrollInfo.ExtentHeight));

                offset = scrollInfo.VerticalOffset;
                if (expectedDelta == Double.PositiveInfinity)
                {
                    expectedDelta = offset - oldOffset;
                }
                else
                {
                    if (!LayoutDoubleUtil.AreClose(expectedDelta, offset - oldOffset) &&
                        ((down && offset + scrollInfo.ViewportHeight < extent) ||
                         (!down && offset > 0.0)))
                    {
                        Assert.Fail(String.Format("Bad offset after scroll.  Expected {0:f2}  Actual: {1:f2}",
                            oldOffset + expectedDelta, offset));
                    }
                }
            }
            while (down ? (offset > oldOffset) : (offset > 0.0));

            // no exception - bug is fixed
            return TestResult.Pass;
        }

        #endregion

        #region Soft loops

        int _scrollChangedEventCount = -1;

        void CheckForSoftLoop()
        {
            _scrollChangedEventCount = 0;
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            _scrollChangedEventCount = -1;
        }

        void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // if ScrollChanged is raised too many times, it's probably a soft loop
            if (_scrollChangedEventCount >= 0)
            {
                if (++_scrollChangedEventCount > 200)   // arbitrary number
                {
                    Assert.Fail("Soft loop detected");
                }
            }
        }

        #endregion
    }

    public class NodeViewModel
    {
        public string Name { get; set; }

        public ObservableCollection<NodeViewModel> Children { get; set; }

        public NodeViewModel()
        {
            Children = new ObservableCollection<NodeViewModel>();
        }
    }

}

