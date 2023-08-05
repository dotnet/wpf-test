// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Microsoft.Test.Hosting;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.UIAutomaion;


namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Testing ScrollPattern for Controls Below
    /// ListBox
    /// TreeView
    /// </summary>
    [Serializable]
    public class UiaScrollTest : UiaSimpleTestcase
    {
        bool _initialHorizontallyScrollable = false;

        public bool InitialHorizontallyScrollable
        {
            get { return _initialHorizontallyScrollable; }
            set { _initialHorizontallyScrollable = value; }
        }

        bool _initialVerticallyScrollable = false;

        public bool InitialVerticallyScrollable
        {
            get { return _initialVerticallyScrollable; }
            set { _initialVerticallyScrollable = value; }
        }

        public override void Init(object target)
        {
            ListBox listbox = target as ListBox;
            if (listbox != null)
            {
                //make sure that the ScrollViewer is in the intial state
                if (_initialHorizontallyScrollable == false)
                {
                    listbox.Width = double.NaN;
                }
                else if (_initialHorizontallyScrollable == true)
                {
                    listbox.Width = 80;
                }
                if (_initialVerticallyScrollable == false)
                {
                    listbox.Height = double.NaN;
                }
                else if (_initialVerticallyScrollable == true)
                {
                    listbox.Height = 80;
                }
            }
            TreeView treeview = target as TreeView;
            if (treeview != null)
            {
                //make sure that the ScrollViewer is in the intial state
                if (_initialHorizontallyScrollable == false)
                {
                    treeview.Width = double.NaN;
                }
                else if (_initialHorizontallyScrollable == true)
                {
                    treeview.Width = 80;
                }
                if (_initialVerticallyScrollable == false)
                {
                    treeview.Height = double.NaN;
                }
                else if (_initialVerticallyScrollable == true)
                {
                    treeview.Height = 80;
                }
            }
        }

        public override void DoTest(AutomationElement target)
        {
            ScrollPattern sp = null;
            if (target != null)
            {
                sp = target.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
            }
            else
            {
                TestLog.Current.LogEvidence("target is null.");
            }

            Thread.Sleep(500);
            if (sp.Current.HorizontallyScrollable)
            {
                sp.ScrollHorizontal(ScrollAmount.LargeDecrement);
                sp.ScrollHorizontal(ScrollAmount.LargeIncrement);
                sp.ScrollHorizontal(ScrollAmount.NoAmount);
                sp.ScrollHorizontal(ScrollAmount.SmallDecrement);
                sp.ScrollHorizontal(ScrollAmount.SmallIncrement);
                sp.SetScrollPercent(0, ScrollPattern.NoScroll);
                sp.SetScrollPercent(50, ScrollPattern.NoScroll);
                sp.SetScrollPercent(100, ScrollPattern.NoScroll);
                sp.Scroll(ScrollAmount.LargeDecrement, ScrollAmount.NoAmount);
                sp.Scroll(ScrollAmount.LargeIncrement, ScrollAmount.NoAmount);
                sp.Scroll(ScrollAmount.NoAmount, ScrollAmount.NoAmount);
                sp.Scroll(ScrollAmount.SmallDecrement, ScrollAmount.NoAmount);
                sp.Scroll(ScrollAmount.SmallIncrement, ScrollAmount.NoAmount);
                sp.ScrollVertical(ScrollAmount.NoAmount);
            }

            if (sp.Current.VerticallyScrollable)
            {
                sp.ScrollVertical(ScrollAmount.LargeDecrement);
                sp.ScrollVertical(ScrollAmount.LargeIncrement);
                sp.ScrollVertical(ScrollAmount.NoAmount);
                sp.ScrollVertical(ScrollAmount.SmallDecrement);
                sp.ScrollVertical(ScrollAmount.SmallIncrement);
                sp.SetScrollPercent(ScrollPattern.NoScroll, 0);
                sp.SetScrollPercent(ScrollPattern.NoScroll, 50);
                sp.SetScrollPercent(ScrollPattern.NoScroll, 100);
                sp.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeDecrement);
                sp.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
                sp.Scroll(ScrollAmount.NoAmount, ScrollAmount.NoAmount);
                sp.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallDecrement);
                sp.Scroll(ScrollAmount.NoAmount, ScrollAmount.SmallIncrement);
                sp.ScrollHorizontal(ScrollAmount.NoAmount);
            }

            Thread.Sleep(500);
            SharedState["targetHorizontallyScrollable"] = sp.Current.HorizontallyScrollable;
            SharedState["targetHorizontalScrollPercent"] = sp.Current.HorizontalScrollPercent;
            SharedState["targetHorizontalViewSize"] = sp.Current.HorizontalViewSize;
            SharedState["targetVerticallyScrollable"] = sp.Current.VerticallyScrollable;
            SharedState["targetVerticalScrollPercent"] = sp.Current.VerticalScrollPercent;
            SharedState["targetVerticalViewSize"] = sp.Current.VerticalViewSize;
        }

        public override void Validate(object target)
        {
            double horizontalViewSize = 0;
            double verticalViewSize = 0;
            double horizontalScrollPercent = 0;
            double verticalScrollPercent = 0;

            ScrollViewer scrollViewer = null;
            ListBox listbox = target as ListBox;
            if (listbox != null)
            {
                ListBoxItem listboxitem = null;
                if (_initialVerticallyScrollable)
                {
                    listboxitem = listbox.Items[listbox.Items.Count - 1] as ListBoxItem;
                }
                else
                {
                    listboxitem = listbox.Items[0] as ListBoxItem;
                }

                for (DependencyObject current = listboxitem; current != listbox && current != null; current = VisualTreeHelper.GetParent(current))
                {
                    scrollViewer = current as ScrollViewer;
                    if (scrollViewer != null)
                    {
                        break;
                    }
                }
            }

            TreeView treeview = target as TreeView;
            if (treeview != null)
            {
                TreeViewItem treeviewitem = null;
                if (_initialVerticallyScrollable)
                {
                    treeviewitem = treeview.Items[treeview.Items.Count - 1] as TreeViewItem;
                }
                else
                {
                    treeviewitem = treeview.Items[0] as TreeViewItem;
                }

                for (DependencyObject current = treeviewitem; current != treeview && current != null; current = VisualTreeHelper.GetParent(current))
                {
                    scrollViewer = current as ScrollViewer;
                    if (scrollViewer != null)
                    {
                        break;
                    }
                }
            }

            if (scrollViewer != null)
            {
                if (Math.Abs(scrollViewer.ExtentWidth) < 10.0 * Double.Epsilon)
                {
                    horizontalViewSize = 100.0;
                }
                else
                {
                    horizontalViewSize = Math.Min(100.0, (double)(scrollViewer.ViewportWidth * 100.0 / scrollViewer.ExtentWidth));
                }

                if (Math.Abs(scrollViewer.ExtentHeight) < 10.0 * Double.Epsilon)
                {
                    verticalViewSize = 100.0;
                }
                else
                {
                    verticalViewSize = Math.Min(100.0, (double)(scrollViewer.ViewportHeight * 100.0 / scrollViewer.ExtentHeight));
                }

                if (!(bool)SharedState["targetHorizontallyScrollable"])
                {
                    horizontalScrollPercent = ScrollPatternIdentifiers.NoScroll;
                }
                else
                {
                    horizontalScrollPercent = (double)(scrollViewer.HorizontalOffset * 100.0 / (scrollViewer.ExtentWidth - scrollViewer.ViewportWidth));
                }

                if (!(bool)SharedState["targetVerticallyScrollable"])
                {
                    verticalScrollPercent = ScrollPatternIdentifiers.NoScroll;
                }
                else
                {
                    verticalScrollPercent = (double)(scrollViewer.VerticalOffset * 100.0 / (scrollViewer.ExtentHeight - scrollViewer.ViewportHeight));
                }
            }
            else
            {
                TestLog.Current.LogEvidence("The scrollviewer is null.");

            }


            TestLog.Current.Result = TestResult.Pass;

            if ((double)SharedState["targetHorizontalScrollPercent"] != horizontalScrollPercent)
            {
                TestLog.Current.LogEvidence("The ScrollViewer HorizontalScrollPercent doesn't work.");
                TestLog.Current.LogEvidence("Automation SharedState['targetHorizontalScrollPercent'] = " + ((double)SharedState["targetHorizontalScrollPercent"]).ToString());
                TestLog.Current.LogEvidence("Avalon ScrollViewer horizontalScrollPercent = " + horizontalScrollPercent.ToString());
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((double)SharedState["targetVerticalScrollPercent"] != verticalScrollPercent)
            {
                TestLog.Current.LogEvidence("The ScrollViewer VerticalScrollPercent doesn't work.");
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((double)SharedState["targetHorizontalViewSize"] != horizontalViewSize)
            {
                TestLog.Current.LogEvidence("The ScrollViewer HorizontalViewSize doesn't work.");
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((double)SharedState["targetVerticalViewSize"] != verticalViewSize)
            {
                TestLog.Current.LogEvidence("The ScrollViewer VerticalViewSize doesn't work.");
                TestLog.Current.Result = TestResult.Fail;
            }

            if ((bool)SharedState["targetHorizontallyScrollable"] != (scrollViewer.ScrollableWidth > 0))
            {
                TestLog.Current.LogEvidence("The ScrollViewer HorizontallyScrollable doesn't work.");
                TestLog.Current.Result = TestResult.Fail;
            }
            if ((bool)SharedState["targetVerticallyScrollable"] != (scrollViewer.ScrollableHeight > 0))
            {
                TestLog.Current.LogEvidence("The ScrollViewer VerticallyScrollable doesn't work.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }
}
