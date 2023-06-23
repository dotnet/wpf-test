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
    /// When the items source contains duplicate value-types, scrolling hangs
    /// </description>
    /// </summary>
    [Test(0, "VirtualizedScrolling", "DuplicateValueTypes", Versions="4.8+")]
    public class DuplicateValueTypes : XamlTest
    {
        #region Private Members

        private ViewModel _vm;
        private IScrollInfo scrollInfo;

        #endregion

        #region Test Steps

        public DuplicateValueTypes()
            : base(@"DuplicateValueTypes.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(Repro);
        }

        private TestResult Setup()
        {
            Status("Setup");

            // Find the ListBox and the players in its scrolling behavior
            ListBox listBox = (ListBox)RootElement.FindName("listBox");
            Assert.AssertTrue("Failed to find ListBox named listBox", listBox != null);
            scrollInfo = DataGridHelper.FindVisualChild<VirtualizingStackPanel>(listBox) as IScrollInfo;
            Assert.AssertTrue("Failed to find the ScrollInfo of the ListBox. ", scrollInfo != null);
            scrollInfo.ScrollOwner.ScrollChanged += OnScrollChanged;

            // Add data to the ListBox
            _vm = new ViewModel();
            RootElement.SetValue(FrameworkElement.DataContextProperty, _vm);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            scrollInfo.ScrollOwner.ScrollChanged -= OnScrollChanged;
            scrollInfo = null;
            return TestResult.Pass;
        }

        private TestResult Repro()
        {
            bool down = true;

            LogComment("Move to top");
            scrollInfo.SetVerticalOffset(down ? 0.0 : Double.PositiveInfinity);
            CheckForSoftLoop();
            double offset = scrollInfo.VerticalOffset, oldOffset = offset;
            double expectedDelta = Double.PositiveInfinity;

            LogComment("Scroll through the ListBox");
            do
            {
                oldOffset = offset;
                if (down)   scrollInfo.MouseWheelDown();
                else        scrollInfo.MouseWheelUp();
                CheckForSoftLoop();

                // after the scroll completes the offset should change by
                // the expected amount.
                offset = scrollInfo.VerticalOffset;
                if (expectedDelta == Double.PositiveInfinity)
                {
                    expectedDelta = offset - oldOffset;
                }
                else
                {
                    if (!LayoutDoubleUtil.AreClose(expectedDelta, offset - oldOffset) &&
                        ((down && offset + scrollInfo.ViewportHeight < scrollInfo.ExtentHeight) ||
                         (!down && offset > 0.0)))
                    {
                        Assert.Fail(String.Format("Bad offset after scroll.  Expected {0:f2}  Actual: {1:f2}",
                            oldOffset + expectedDelta, offset));
                    }
                }
            }
            while (down ? (offset > oldOffset) : (offset > 0.0));

           // no exception or hang
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

    public class ViewModel
    {
        public double[] Heights { get; private set; }

        public ViewModel()
        {
            Heights = new double[] { 21, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, };
        }
    }
}


