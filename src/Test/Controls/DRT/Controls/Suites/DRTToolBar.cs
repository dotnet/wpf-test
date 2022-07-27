// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DRT
{
    public class DrtToolBarSuite : DrtTestSuite
    {
        public DrtToolBarSuite() : base("ToolBar")
        {
            Contact = "Microsoft";
        }

        private ToolBarTray _toolBarTrayHorizontal;
        private ToolBarTray _toolBarTrayVertical;
        private ToolBar _toolBar1;
        private ToolBar _toolBar2;
        private ToolBar _toolBar3;
        private ToolBar _toolBar4;
        private ToolBar _toolBar5;
        private ToolBar _toolBar6;
        private ToolBar _toolBar7;
        private ToolBar _toolBar8;
        private ToolBar _toolBar9;
        private ToolBar _toolBar10;

        public override DrtTest[] PrepareTests()
        {
            // Build tree
            DockPanel root = new DockPanel();     // Returned root object
            root.Width = 500d;
            root.Height = 500d;
            root.Background = Brushes.Gold;

            _toolBarTrayHorizontal = new ToolBarTray();
            DockPanel.SetDock(_toolBarTrayHorizontal, Dock.Top);
            _toolBar1 = new ToolBar(); AddButtons(_toolBar1, 10); _toolBar1.Band = -5; _toolBar1.BandIndex = 4;
            _toolBar2 = new ToolBar(); AddButtons(_toolBar2, 10); _toolBar2.Band = -5; _toolBar2.BandIndex = 1;
            _toolBar3 = new ToolBar(); AddButtons(_toolBar3, 10); _toolBar3.Band = 0; _toolBar3.BandIndex = 1;
            _toolBar4 = new ToolBar(); AddButtons(_toolBar4, 10); _toolBar4.Band = 3; _toolBar4.BandIndex = 4;
            _toolBar5 = new ToolBar(); AddButtons(_toolBar5, 10); _toolBar5.Band = 0; _toolBar5.BandIndex = 2;
            _toolBar1.Header = "Style";
            _toolBar2.Header = "Font";
            _toolBarTrayHorizontal.ToolBars.Add(_toolBar1);
            _toolBarTrayHorizontal.ToolBars.Add(_toolBar2);
            _toolBarTrayHorizontal.ToolBars.Add(_toolBar3);
            _toolBarTrayHorizontal.ToolBars.Add(_toolBar4);
            _toolBarTrayHorizontal.ToolBars.Add(_toolBar5);
            root.Children.Add(_toolBarTrayHorizontal);


            _toolBarTrayVertical = new ToolBarTray();
            _toolBarTrayVertical.Orientation = System.Windows.Controls.Orientation.Vertical;
            DockPanel.SetDock(_toolBarTrayVertical, Dock.Left);
            _toolBar6 = new ToolBar();
            _toolBar7 = new ToolBar();
            _toolBar8 = new ToolBar();
            _toolBar9 = new ToolBar();
            _toolBar10 = new ToolBar();
            _toolBar6.Header = "Style";
            _toolBar7.Header = "Font";
            _toolBarTrayVertical.ToolBars.Add(_toolBar6);
            _toolBarTrayVertical.ToolBars.Add(_toolBar7);
            _toolBarTrayVertical.ToolBars.Add(_toolBar8);
            _toolBarTrayVertical.ToolBars.Add(_toolBar9);
            _toolBarTrayVertical.ToolBars.Add(_toolBar10);
            root.Children.Add(_toolBarTrayVertical);

            DRT.Show(root);

            return new DrtTest[]
                {
                    new DrtTest(VerifyNormalization),
                    new DrtTest(Step1),
                    new DrtTest(Step2),
                    new DrtTest(Step3),
                    new DrtTest(Step4),
                    new DrtTest(Step5),
                    new DrtTest(Step6),
                    new DrtTest(Step7),
                    new DrtTest(Step8),
                    new DrtTest(Step9),
                    new DrtTest(Step10),
                    new DrtTest(Step11),
                    new DrtTest(Step12),
                };
        }

        private void AddButtons(ToolBar tb, int n)
        {
            for (int i=0; i<n; i++)
            {
                Button b = new Button();
                b.Content = i;
                tb.Items.Add(b);
            }
        }

        private void VerifyBand(ToolBar tb, int expectedValue)
        {
            DRT.Assert(tb.Band == expectedValue, "ToolBar.Band should be " + expectedValue + ", was " + tb.Band);
        }

        private void VerifyBandIndex(ToolBar tb, int expectedValue)
        {
            DRT.Assert(tb.BandIndex == expectedValue, "ToolBar.BandIndex should be " + expectedValue + ", was " + tb.BandIndex);
        }

        private Thumb FindThumb(DependencyObject v)
        {
            if (v != null)
            {
                int count = VisualTreeHelper.GetChildrenCount(v);
                for(int i = 0; i < count; i++)
                {
                    if (VisualTreeHelper.GetChild(v, i) is Thumb)
                        return (Thumb)(VisualTreeHelper.GetChild(v, i));
                    else
                    {
                        Thumb s = FindThumb(VisualTreeHelper.GetChild(v, i));

                        if (s != null)
                            return s;
                    }
                }
            }
            return null;
        }


        private void VerifyNormalization()
        {
            // Verify normalized band
            VerifyBand(_toolBar1, 0);
            VerifyBand(_toolBar2, 0);
            VerifyBand(_toolBar3, 1);
            VerifyBand(_toolBar4, 2);
            VerifyBand(_toolBar5, 1);

            // Verify normalized BandIndex
            VerifyBandIndex(_toolBar1, 1);
            VerifyBandIndex(_toolBar2, 0);
            VerifyBandIndex(_toolBar3, 0);
            VerifyBandIndex(_toolBar4, 0);
            VerifyBandIndex(_toolBar5, 1);
        }

        private void Step1()
        {
            Thumb thumb1 = FindThumb(_toolBar1);
            DRT.Assert(thumb1 != null, "Could nor find Thumb in toolbar1");

            DRT.MoveMouse(thumb1, 0.5, 0.5); // Begining of toolbar1
            DRT.MouseButtonDown();
            DRT.MoveMouse(_toolBar2, 0.5, 0.5); // Middle of toolbar2
        }

        private void Step2()
        {
            DRT.Assert(!double.IsNaN(_toolBar2.Width), "ToolBar2 should have Width!=Auto");
            DRT.Assert(Mouse.Captured == FindThumb(_toolBar1), "Thumb1 should have mouse capture");
            VerifyNormalization();

            AssertOverflow(true, false, _toolBar2, 9);

            ToolBar.SetOverflowMode((DependencyObject)_toolBar2.Items[9], OverflowMode.Never);
        }

        private void Step3()
        {
            AssertOverflow(false, true, _toolBar2, 9);

            DRT.MoveMouse(_toolBar3, 0.5, 0.5); // Middle of toolbar2
            DRT.MouseButtonUp();
        }

        private void Step4()
        {
            DRT.Assert(Mouse.Captured == null, "Mouse.Captured should be null");
            VerifyBand(_toolBar1, 1);
            VerifyBandIndex(_toolBar1, 1);
            VerifyBandIndex(_toolBar5, 2);
            DRT.Assert(double.IsNaN(_toolBar1.Width), "ToolBar1 should have Width=Auto");
            DRT.Assert(!double.IsNaN(_toolBar3.Width), "ToolBar3 should have Width!=Auto");

            DRT.MouseButtonDown();
            DRT.MoveMouse(FindThumb(_toolBar5), 0.5, 0.5);
            DRT.MouseButtonUp();
        }

        private void Step5()
        {
            DRT.Assert(Mouse.Captured == null, "Mouse.Captured should be null");
            VerifyBand(_toolBar1, 1);
            VerifyBandIndex(_toolBar1, 2);
            VerifyBandIndex(_toolBar5, 1);

            DRT.MoveMouse(FindThumb(_toolBar1), 0.5, 0.5); // Grip toolbar1
            DRT.MouseButtonDown();
            DRT.MoveMouse(_toolBar5, 0.75, 0.5); // 3/4 of toolbar5
            DRT.MouseButtonUp();
        }

        private void Step6()
        {
            // 0-5 should be visible
            AssertOverflow(false, true, _toolBar5, 0, 5);

            // 6-9 should be in overflow
            AssertOverflow(true, false, _toolBar5, 6, 9);

            _toolBar5.IsOverflowOpen = true;
            DRT.Pause(100); // Allows you to see the popup
        }

        private void Step7()
        {
            // 0-5 should be visible
            AssertOverflow(false, true, _toolBar5, 0, 5);

            // 6-9 should be in overflow
            AssertOverflow(true, true, _toolBar5, 6, 9);

            _toolBar5.IsOverflowOpen = false;
        }

        private void Step8()
        {
            RadioButton radioButton = new RadioButton();
            radioButton.Content = "A";
            _toolBar5.Items.Insert(3, radioButton);

            radioButton = new RadioButton();
            radioButton.Content = "A8";
            _toolBar5.Items.Insert(8, radioButton);
        }

        private void Step9()
        {
            // Some elements were should be in the tree and others not
            AssertOverflow(false, true, _toolBar5, 0, 5);
            AssertOverflow(true, false, _toolBar5, 6, 6);
            AssertOverflow(true, true, _toolBar5, 7, 7);
            AssertOverflow(true, false, _toolBar5, 8, 8);
            AssertOverflow(true, true, _toolBar5, 9, 11);

            // Remove an existing element that was in the tree
            _toolBar5.Items.RemoveAt(9);
            // Remove one of the added elements
            _toolBar5.Items.RemoveAt(3);
        }

        private void Step10()
        {
            // Some elements were should be in the tree and others not
            AssertOverflow(false, true, _toolBar5, 0, 5);
            AssertOverflow(true, true, _toolBar5, 6, 6);
            AssertOverflow(true, false, _toolBar5, 7, 7);
            AssertOverflow(true, true, _toolBar5, 8, 9);

            _toolBar5.IsOverflowOpen = true;
            DRT.Pause(100); // Allows you to see the popup
        }

        private void Step11()
        {
            // All elements should be in the tree now
            AssertOverflow(false, true, _toolBar5, 0, 5);
            AssertOverflow(true, true, _toolBar5, 6, 9);

            _toolBar5.IsOverflowOpen = false;
        }

        private void Step12()
        {
            // All elements should still be in the tree
            AssertOverflow(false, true, _toolBar5, 0, 5);
            AssertOverflow(true, true, _toolBar5, 6, 9);
        }

        private void AssertOverflow(bool isOverflow, bool inTree, ToolBar toolbar, int startIndex, int endIndex)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                AssertOverflow(isOverflow, inTree, toolbar, i);
            }
        }

        private void AssertOverflow(bool isOverflow, bool inTree, ToolBar toolbar, int childIndex)
        {
            AssertIsOverflow(isOverflow, toolbar, childIndex);

            if (isOverflow)
            {
                if (inTree)
                {
                    AssertVisualParent<ToolBarOverflowPanel>(toolbar, childIndex);
                }
                else
                {
                    AssertNoVisualParent(toolbar, childIndex);
                }
            }
            else
            {
                AssertVisualParent<ToolBarPanel>(toolbar, childIndex);
            }
        }

        private void AssertIsOverflow(bool isOverflow, ToolBar toolbar, int childIndex)
        {
            DependencyObject child = toolbar.Items[childIndex] as DependencyObject;
            bool actualIsOverflow = ToolBar.GetIsOverflowItem(child);
            DRT.Assert(isOverflow == actualIsOverflow, String.Format("Child {0} IsOverflow={1}.", childIndex, actualIsOverflow));
        }

        private void AssertVisualParent<T>(ToolBar toolbar, int childIndex)
        {
            DependencyObject child = toolbar.Items[childIndex] as DependencyObject;
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            DRT.Assert(parent != null, String.Format("Visual parent of child {0} is null.", childIndex));
            DRT.Assert(parent is T, String.Format("Visual parent of child {0} is of type {1} instead of {2}.", childIndex, parent.GetType(), typeof(T)));
        }

        private void AssertNoVisualParent(ToolBar toolbar, int childIndex)
        {
            DependencyObject child = toolbar.Items[childIndex] as DependencyObject;
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            if (parent != null)
            {
                DRT.Assert(false, String.Format("Visual parent of child {0} is non-null and of type {1}.", childIndex, parent.GetType()));
            }
        }

    }
}


