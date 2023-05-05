// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Collections;
using System.Windows.Controls.Primitives;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Layout.PropertyDump;
using System.IO;
using Microsoft.Test.Threading;

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.StackPanel", "FrameworkElementProps.HeightWidthDefault", TestParameters = "target=StackPanel, test=HeightWidthDefault")]
    [Test(1, "Panels.StackPanel", "FrameworkElementProps.HeightWidthActual", TestParameters = "target=StackPanel, test=HeightWidthActual")]
    [Test(1, "Panels.StackPanel", "FrameworkElementProps.ChildHeightWidth", TestParameters = "target=StackPanel, test=ChildHeightWidth")]
    [Test(1, "Panels.StackPanel", "FrameworkElementProps.Visibility", TestParameters = "target=StackPanel, test=Visibility")]
    [Test(1, "Panels.StackPanel", "FrameworkElementProps.HorizontalAlignment", TestParameters = "target=StackPanel, test=HorizontalAlignment")]
    [Test(1, "Panels.StackPanel", "FrameworkElementProps.VerticalAlignment", TestParameters = "target=StackPanel, test=VerticalAlignment")]
    [Test(1, "Panels.StackPanel", "FrameworkElementProps.FlowDirection", TestParameters = "target=StackPanel, test=FlowDirection")]
    [Test(1, "Panels.StackPanel", "FrameworkElementProps.Margin", TestParameters = "target=StackPanel, test=Margin")]
    public class StackPanelFETest : FrameworkElementPropertiesTest
    {
        public StackPanelFETest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }
    }

    [Test(1, "Panels.StackPanel", "StackPanelHorizontalStacking", Variables = "Area=ElementLayout")]
    public class StackPanelHorizontalStacking : CodeTest
    {
        public StackPanelHorizontalStacking() 
        {
            CreateLog = false;
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        private StackPanel _root = null;
        public override FrameworkElement TestContent()
        {
            _root = new StackPanel();
            _root.Background = Brushes.CornflowerBlue;
            _root.Orientation = Orientation.Horizontal;
            return _root;
        }

        private bool RunTest(int childCount, string target)
        {
            TestLog testLog = new TestLog(target);

            _root.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddChildren(childCount, target, _root, testLog);
            CommonFunctionality.FlushDispatcher();

            bool result = StackPanelCommon.VerifyHorizontalStacking(target, _root, testLog);

            if (result)
            {
                testLog.Result = TestResult.Pass;
            }
            else
            {
                testLog.Result = TestResult.Fail;
            }
            testLog.Close();

            return result;
        }

        public override void TestActions()
        {
            _tempresult = RunTest(6, "Panel");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(5, "Canvas");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(3, "StackPanel");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(6, "Grid");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(5, "DockPanel");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(3, "Decorator");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(6, "Border");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(5, "Viewbox");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(3, "Transform");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(3, "ScrollViewer");
            DispatcherHelper.DoEvents();
        }

        private bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Panels.StackPanel", "StackPanelVerticalStacking", Variables = "Area=ElementLayout")]
    public class StackPanelVerticalStacking : CodeTest
    {
        public StackPanelVerticalStacking() 
        {
            CreateLog = false;
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        private StackPanel _root = null;
        public override FrameworkElement TestContent()
        {
            _root = new StackPanel();
            _root.Background = Brushes.CornflowerBlue;
            _root.Orientation = Orientation.Vertical;
            return _root;
        }

        private bool RunTest(int childCount, string target)
        {
            TestLog testLog = new TestLog(target);

            _root.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddChildren(childCount, target, _root, testLog);
            CommonFunctionality.FlushDispatcher();

            bool result = StackPanelCommon.VerifyVerticalStacking(target, _root, testLog);

            if (result)
            {
                testLog.Result = TestResult.Pass;
            }
            else
            {
                testLog.Result = TestResult.Fail;
            }
            testLog.Close();

            return result;
        }

        public override void TestActions()
        {
            _tempresult = RunTest(6, "Panel");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(5, "Canvas");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(3, "StackPanel");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(5, "Grid");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(3, "DockPanel");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(4, "Decorator");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(3, "Border");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(4, "Viewbox");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(6, "Transform");
            DispatcherHelper.DoEvents();
            _tempresult = RunTest(4, "ScrollViewer");
            DispatcherHelper.DoEvents();
        }

        private bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }

    [Test(1, "Panels.StackPanel", "StackPanelPropertyAttributePairs", Variables = "Area=ElementLayout")]
    public class StackPanelPropertyAttributePairs : CodeTest
    {
        public StackPanelPropertyAttributePairs()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        StackPanel _root;
        public override FrameworkElement TestContent()
        {
            _root = new StackPanel();
            _root.Background = Brushes.CornflowerBlue;
            return _root;
        }

        public override void TestActions()
        {
            try
            {
                _root.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            }
            catch (Exception e)
            {
                _tempresult = false;
                Helpers.Log("exception thrown with valid input.");
                Helpers.Log(e.Message);
            }

            try
            {
                _root.SetValue(StackPanel.OrientationProperty, Orientation.Vertical);
            }
            catch (Exception e)
            {
                _tempresult = false;
                Helpers.Log("exception thrown with valid input.");
                Helpers.Log(e.Message);
            }

            try
            {
                _root.SetValue(StackPanel.OrientationProperty, VerticalAlignment.Center);
                Helpers.Log("exception was not thrown with in-valid input.");
                _tempresult = false;
            }
            catch (Exception e)
            {
                Helpers.Log("exception thrown with in-valid input.");
                Helpers.Log(e.Message);
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }

    [Test(1, "Panels.StackPanel", "StackPanelBringIntoView", Variables = "Area=ElementLayout")]
    public class StackPanelBringIntoView : CodeTest
    {
        public StackPanelBringIntoView()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            string xamlfile = "StackBringIntoView.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

        }

        public override FrameworkElement TestContent()
        {
            //need to load a xaml file;
            return null;
        }
        private bool _loaded = false;
        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //DoVerticalTest();
            //CommonFunctionality.FlushDispatcher();
            //DoHorizontalTest();

            foreach (UIElement child in _stack.Children)
            {
                if (child is Button)
                {
                    ((Button)child).Height = 25;
                    ((Button)child).Width = double.NaN;
                }
            }

            int ChildPostitionInStack = 0;

            Button btn = LogicalTreeHelper.FindLogicalNode(this.window, "btn4") as Button;

            int counter = 0;
            foreach (UIElement child in _stack.Children)
            {
                if (((Button)child).Name == "btn4")
                {
                    ChildPostitionInStack = counter;
                }
                counter++;
            }

            if (btn != null)
            {
                btn.BringIntoView();
            }

            CommonFunctionality.FlushDispatcher();

            if (ChildPostitionInStack != _stack.VerticalOffset)
            {
                Helpers.Log("The StackPanel's vertical offset is not equal to the focused buttons position.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("The StackPanel's vertical offset is equal to the focused buttons position.");
                Helpers.Log("StackPanelBringIntoView worked.");
                Helpers.Log("Do Horizontal Test.");
            }

            _stack.Orientation = Orientation.Horizontal;
            foreach (UIElement child in _stack.Children)
            {
                if (child is Button)
                {
                    ((Button)child).Height = double.NaN;
                    ((Button)child).Width = 50;
                }
            }
            CommonFunctionality.FlushDispatcher();

            ChildPostitionInStack = 0;

            btn = LogicalTreeHelper.FindLogicalNode(this.window, "btn5") as Button;

            counter = 0;
            foreach (UIElement child in _stack.Children)
            {
                if (((Button)child).Name == "btn5")
                {
                    ChildPostitionInStack = counter;
                }
                counter++;
            }

            if (btn != null)
            {
                btn.BringIntoView();
            }

            CommonFunctionality.FlushDispatcher();

            if (ChildPostitionInStack != _stack.HorizontalOffset)
            {
                Helpers.Log("The StackPanel's horizontal offset is not equal to the focused buttons position.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("The StackPanel's horizontal offset is equal to the focused buttons position.");
                Helpers.Log("StackPanelBringIntoView worked.");
            }
        }

        StackPanel _stack;
        private bool _testStarted = false;
        private void EnterTest()
        {
            _stack = LogicalTreeHelper.FindLogicalNode(this.window, "stack") as StackPanel;

            if (_stack != null)
            {
                if (!_testStarted)
                {
                    _loaded = true;
                    _testStarted = true;
                }
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }

    [Test(1, "Panels.StackPanel", "StackPanelContentReLayout", Variables = "Area=ElementLayout")]
    public class StackPanelContentReLayout : CodeTest
    {
        public StackPanelContentReLayout()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;
        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;

            _root.Children.Add(_stack);

            return _root;
        }

        public override void TestActions()
        {

            StackPanelCommon.AddSizedChildren(1000, "Small", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddSizedChildren(1000, "Large", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddSizedChildren(1000, "Increasing", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddSizedChildren(1000, "Decreasing", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddSizedChildren(1000, "Small", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            _stack.Orientation = Orientation.Horizontal;
            //IncreasingValue = 10;
            //DecreasingValue = 1000;
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddSizedChildren(1000, "Small", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddSizedChildren(1000, "Large", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddSizedChildren(1000, "Increasing", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddSizedChildren(1000, "Decreasing", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();


            StackPanelCommon.AddSizedChildren(1000, "Small", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            IncreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            DecreaseChildSize();
            CommonFunctionality.FlushDispatcher();
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();
            _stack.Children.Clear();
            CommonFunctionality.FlushDispatcher();
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }

        bool ValidateReLayout()
        {
            bool validationResult = true;
            switch (_stack.Orientation)
            {
                case Orientation.Vertical:
                    validationResult = ValidateStackSize();
                    if (validationResult)
                    {
                        validationResult = StackPanelCommon.VerifyVerticalStacking("", _stack, TestLog.Current);
                    }
                    break;

                case Orientation.Horizontal:
                    validationResult = ValidateStackSize();
                    if (validationResult)
                    {
                        validationResult = StackPanelCommon.VerifyHorizontalStacking("", _stack, TestLog.Current);
                    }
                    break;
            }
            return validationResult;
        }

        bool ValidateStackSize()
        {
            bool validationResult = true;

            switch (_stack.Orientation)
            {
                case Orientation.Vertical:

                    FrameworkElement vChildOne = _stack.Children[0] as FrameworkElement;
                    FrameworkElement vStackParent = _stack.Parent as FrameworkElement;
                    double allChildHeight = 0;

                    foreach (UIElement child in _stack.Children)
                    {
                        allChildHeight += ((FrameworkElement)child).ActualHeight;
                    }

                    if (DoubleUtil.GreaterThanOrClose(_stack.ActualWidth, vChildOne.DesiredSize.Width) && DoubleUtil.LessThanOrClose(_stack.ActualWidth, vStackParent.ActualWidth) && DoubleUtil.AreClose(_stack.ActualHeight, allChildHeight))
                    {
                        validationResult = true;
                    }
                    else
                    {
                        Helpers.Log("StackPanel Size was incorrect after Re-Layout.");
                        validationResult = false;
                    }

                    break;

                case Orientation.Horizontal:
                    //validationResult = StackPanelCommon.VerifyHorizontalStacking();
                    FrameworkElement hChildOne = _stack.Children[0] as FrameworkElement;
                    FrameworkElement hStackParent = _stack.Parent as FrameworkElement;
                    double allChildWidth = 0;

                    foreach (UIElement child in _stack.Children)
                    {
                        allChildWidth += ((FrameworkElement)child).ActualWidth;
                    }

                    if (DoubleUtil.GreaterThanOrClose(_stack.ActualHeight, hChildOne.DesiredSize.Height) && DoubleUtil.LessThanOrClose(_stack.ActualHeight, hStackParent.ActualHeight) && DoubleUtil.AreClose(_stack.ActualWidth, allChildWidth))
                    {
                        validationResult = true;
                    }
                    else
                    {
                        Helpers.Log("StackPanel Size was incorrect after Re-Layout.");
                        validationResult = false;
                    }
                    break;
            }

            return validationResult;
        }

        //bool StackPanelCommon.VerifyHorizontalStacking()
        //{
        //    Helpers.Log("Orientation : " + stack.Orientation);

        //    FrameworkElement currentChild = null;
        //    FrameworkElement previousChild = null;

        //    Point currentChildLoc;
        //    Point previousChildLoc;

        //    Transform currentChildTransform;
        //    Transform previousChildTransform;

        //    int ChildCount = 0;

        //    IEnumerator children = LogicalTreeHelper.GetChildren(stack).GetEnumerator();

        //    if (children != null)
        //    {
        //        while (children.MoveNext())
        //        {
        //            ChildCount++;
        //            currentChild = children.Current as FrameworkElement;
        //            if (currentChild == stack.Children[0])
        //            {
        //                currentChildLoc = LayoutUtility.GetElementPosition(currentChild, stack);
        //                if (currentChildLoc.X != (0 + currentChild.Margin.Left))
        //                {
        //                    Helpers.Log("child " + ChildCount + " location is not ok");
        //                    //TestResult = false;
        //                    return false;
        //                }
        //                else
        //                {
        //                    //Helpers.Log("child " + ChildCount + " location is ok");
        //                    //TestResult = true;
        //                    previousChild = currentChild;
        //                }
        //            }
        //            else
        //            {
        //                previousChildLoc = LayoutUtility.GetElementPosition(previousChild, stack);
        //                currentChildLoc = LayoutUtility.GetElementPosition(currentChild, stack);

        //                if (previousChild != null)
        //                {
        //                    if (currentChildLoc.X != (previousChildLoc.X + previousChild.ActualWidth + previousChild.Margin.Right + currentChild.Margin.Left))
        //                    {
        //                        Helpers.Log("child " + ChildCount + " location is not ok");
        //                        //TestResult = false;
        //                        return false;
        //                    }
        //                    else
        //                    {
        //                        //Helpers.Log("child " + ChildCount + " location is ok");
        //                        //TestResult = true;
        //                        previousChild = currentChild;
        //                    }
        //                }
        //                else
        //                {
        //                    Helpers.Log("previous child is null, test cannot continue.");
        //                    //TestResult = false;
        //                    return false;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Helpers.Log("Child enumerator is null! Test cannot continue.");
        //        //TestResult = false;
        //        return false;
        //    }
        //    return true;
        //}

        //bool StackPanelCommon.VerifyVerticalStacking()
        //{
        //    Helpers.Log("Orientation : " + stack.Orientation);

        //    FrameworkElement currentChild = null;
        //    FrameworkElement previousChild = null;

        //    Point currentChildLoc;
        //    Point previousChildLoc;

        //    int ChildCount = 0;

        //    IEnumerator children = LogicalTreeHelper.GetChildren(stack).GetEnumerator();

        //    if (children != null)
        //    {
        //        while (children.MoveNext())
        //        {
        //            ChildCount++;
        //            currentChild = children.Current as FrameworkElement;
        //            if (currentChild == stack.Children[0])
        //            {
        //                currentChildLoc = LayoutUtility.GetElementPosition(currentChild, stack);
        //                if (currentChildLoc.Y != (0 + currentChild.Margin.Top))
        //                {
        //                    Helpers.Log("child " + ChildCount + " location is not ok");
        //                    //TestResult = false;
        //                    return false;
        //                }
        //                else
        //                {
        //                    //Helpers.Log("child " + ChildCount + " location is ok");
        //                    //TestResult = true;
        //                    previousChild = currentChild;
        //                }
        //            }
        //            else
        //            {
        //                previousChildLoc = LayoutUtility.GetElementPosition(previousChild, stack);
        //                currentChildLoc = LayoutUtility.GetElementPosition(currentChild, stack);

        //                if (previousChild != null)
        //                {
        //                    if (currentChildLoc.Y != (previousChildLoc.Y + previousChild.ActualHeight + previousChild.Margin.Bottom + currentChild.Margin.Top))
        //                    {
        //                        Helpers.Log("child " + ChildCount + " location is not ok");
        //                        //TestResult = false;
        //                        return false;
        //                    }
        //                    else
        //                    {
        //                        //Helpers.Log("child " + ChildCount + " location is ok");
        //                        //TestResult = true;
        //                        previousChild = currentChild;
        //                    }
        //                }
        //                else
        //                {
        //                    Helpers.Log("previous child is null, test cannot continue.");
        //                    //TestResult = false;
        //                    return false;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Helpers.Log("Child enumerator is null! Test cannot continue.");
        //        //TestResult = false;
        //        return false;
        //    }
        //    return true;
        //}

        //void StackPanelCommon.AddChildren(int ChildCount, string SizeType)
        //{
        //    switch (SizeType)
        //    {
        //        case "Small":
        //            Helpers.Log("Testing : Small Child Size");
        //            for (int i = 0; i < ChildCount; i++)
        //            {
        //                Border child = new Border();
        //                child.Height = 10;
        //                child.Width = 10;
        //                child.Background = Brushes.Gray;
        //                stack.Children.Add(child);
        //            }
        //            break;
        //        case "Medium":
        //            Helpers.Log("Testing : Medium Child Size");
        //            for (int i = 0; i < ChildCount; i++)
        //            {
        //                Border child = new Border();
        //                child.Height = 100;
        //                child.Width = 100;
        //                child.Background = Brushes.Gray;
        //                stack.Children.Add(child);
        //            }
        //            break;
        //        case "Large":
        //            Helpers.Log("Testing : Large Child Size");
        //            for (int i = 0; i < ChildCount; i++)
        //            {
        //                Border child = new Border();
        //                child.Height = 1000;
        //                child.Width = 1000;
        //                child.Background = Brushes.Gray;
        //                stack.Children.Add(child);
        //            }
        //            break;
        //        case "Increasing":
        //            Helpers.Log("Testing : Increasing Child Size");
        //            for (int i = 0; i < ChildCount; i++)
        //            {
        //                Border child = new Border();
        //                child.Height = IncreasingValue;
        //                child.Width = IncreasingValue;
        //                child.Background = Brushes.Gray;
        //                stack.Children.Add(child);
        //                IncreasingValue = IncreasingValue + 1;
        //            }
        //            break;
        //        case "Decreasing":
        //            Helpers.Log("Testing : Decreasing Child Size");
        //            for (int i = 0; i < ChildCount; i++)
        //            {
        //                Border child = new Border();
        //                child.Height = DecreasingValue;
        //                child.Width = DecreasingValue;
        //                child.Background = Brushes.Gray;
        //                stack.Children.Add(child);
        //                DecreasingValue = DecreasingValue - 1;
        //            }
        //            break;
        //    }
        //}

        //double IncreasingValue = 10;
        //double DecreasingValue = 1000;

        void IncreaseChildSize()
        {
            Helpers.Log("Increasing child size * 5");
            foreach (UIElement child in _stack.Children)
            {
                ((FrameworkElement)child).Height = ((FrameworkElement)child).ActualHeight * 5;
                ((FrameworkElement)child).Width = ((FrameworkElement)child).ActualWidth * 5;
            }
        }

        void DecreaseChildSize()
        {
            Helpers.Log("Decreasing child size / 5");
            foreach (UIElement child in _stack.Children)
            {
                ((FrameworkElement)child).Height = ((FrameworkElement)child).ActualHeight / 5;
                ((FrameworkElement)child).Width = ((FrameworkElement)child).ActualWidth / 5;
            }
        }
    }

    [Test(1, "Panels.StackPanel", "StackPanelErrors", Variables = "Area=ElementLayout")]
    public class StackPanelErrors : CodeTest
    {
        public StackPanelErrors()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();

        }

        StackPanel _stack;
        public override FrameworkElement TestContent()
        {
            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            return _stack;
        }

        StackPanel _stackinScroll;
        FrameworkElement ContentwithScroll()
        {
            ScrollViewer sv = new ScrollViewer();
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            sv.CanContentScroll = true;
            _stackinScroll = new StackPanel();
            _stackinScroll.Background = Brushes.RoyalBlue;
            AddItems(500, _stackinScroll);

            sv.Content = _stackinScroll;

            return sv;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            object badBool = FlowDirection.RightToLeft;
            object badDouble = "false";
            object badScrollOwner = Orientation.Vertical;
            //            Border badScrollOwner = new Border();

            try
            {
                _stack.CanHorizontallyScroll = (bool)badBool;
                _tempresult = false;
                Helpers.Log(" Code should have thrown exception for Bad Bool (CanHorizontallyScroll). ");
            }
            catch (Exception exBadBool)
            {
                Helpers.Log("Exception Caught : " + exBadBool.Message);
            }

            try
            {
                _stack.CanVerticallyScroll = (bool)badBool;
                _tempresult = false;
                Helpers.Log(" Code should have thrown exception for Bad Bool (CanVerticallyScroll). ");
            }
            catch (Exception exBadBool)
            {
                Helpers.Log("Exception Caught : " + exBadBool.Message);
            }

            try
            {
                _stack.SetHorizontalOffset((double)badDouble);
                _tempresult = false;
                Helpers.Log(" Code should have thrown exception for Bad Double (HorizontalOffset). ");
            }
            catch (Exception exBadDouble)
            {
                Helpers.Log("Exception Caught : " + exBadDouble.Message);
            }

            try
            {
                _stack.SetVerticalOffset((double)badDouble);
                _tempresult = false;
                Helpers.Log(" Code should have thrown exception for Bad Double (VerticalOffset). ");
            }
            catch (Exception exBadDouble)
            {
                Helpers.Log("Exception Caught : " + exBadDouble.Message);
            }

            try
            {
                _stack.SetValue(StackPanel.OrientationProperty, Dock.Right);
                _tempresult = false;
                Helpers.Log(" Code should have thrown exception for invalid Orientation. ");
            }
            catch (Exception exBadOrientation)
            {
                Helpers.Log("Exception Caught : " + exBadOrientation.Message);
            }

            try
            {
                _stack.ScrollOwner = (ScrollViewer)badScrollOwner;
                _tempresult = false;
                Helpers.Log(" Code should have thrown exception for Bad . ");
            }
            catch (Exception exBadScrollOwner)
            {
                Helpers.Log("Exception Caught : " + exBadScrollOwner.Message);
            }

            CommonFunctionality.FlushDispatcher();

            this.window.Content = null;
            CommonFunctionality.FlushDispatcher();
            this.window.Content = this.ContentwithScroll();

            CommonFunctionality.FlushDispatcher();
            ((IScrollInfo)_stackinScroll).SetVerticalOffset(5000);
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_stackinScroll.VerticalOffset, (_stackinScroll.Children.Count - 1)))
            {
                Helpers.Log(" Vertical Offset Should Equal Child Count - 1. ");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelLine Down Scrolling at end.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).LineDown();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At End). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelPage Down Scrolling at end.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).PageDown();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At End). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelMouse Wheel Down Scrolling at end.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).MouseWheelDown();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At End). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            // scroll up test.

            ((IScrollInfo)_stackinScroll).SetVerticalOffset(-1);
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_stackinScroll.VerticalOffset, 0))
            {
                Helpers.Log(" Vertical Offset Should Equal 0. ");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelLine Up Scrolling at start.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).LineUp();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At Start). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelPage Up Scrolling at start.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).PageUp();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At Start). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelMouse Wheel Up Scrolling at start.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).MouseWheelUp();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At Start). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            _stackinScroll.Orientation = Orientation.Horizontal;
            CommonFunctionality.FlushDispatcher();

            ((IScrollInfo)_stackinScroll).SetHorizontalOffset(5000);
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_stackinScroll.HorizontalOffset, (_stackinScroll.Children.Count - 1)))
            {
                Helpers.Log(" Horizontal Offset Should Equal Child Count - 2. ");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelLine Right Scrolling at end.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).LineRight();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At End). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelPage Right Scrolling at end.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).PageRight();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At End). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelMouse Wheel Right Scrolling at end.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).MouseWheelRight();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At End). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            // scroll left test.

            ((IScrollInfo)_stackinScroll).SetHorizontalOffset(-1);
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_stackinScroll.HorizontalOffset, 0))
            {
                Helpers.Log(" Horizontal Offset Should Equal 0. ");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelLine Left Scrolling at start.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).LineLeft();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At Start). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelPage Left Scrolling at start.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).PageLeft();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At Start). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }

            CommonFunctionality.FlushDispatcher();
            Helpers.Log("StackPanelMouse Wheel Left Scrolling at start.");
            for (int i = 0; i < 500; i++)
            {
                try
                {
                    ((IScrollInfo)_stackinScroll).MouseWheelLeft();
                }
                catch (Exception ex)
                {
                    Helpers.Log(ex.Message);
                    Helpers.Log(" Exception was thrown on a valid scroll (Scroll At Start). ");
                    _tempresult = false;
                }
                CommonFunctionality.FlushDispatcher();
            }
            CommonFunctionality.FlushDispatcher();
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }

        void AddItems(int itemCount, StackPanel stack)
        {
            for (int i = 1; i <= itemCount; i++)
            {
                Border b = new Border();
                b.Background = Brushes.LightGray;
                TextBlock txt = new TextBlock();
                txt.Text = i.ToString();
                txt.FontSize = i;
                b.Child = txt;
                stack.Children.Add(b);

            }
        }
    }

    [Test(1, "Panels.StackPanel", "StackPanelPropReLayout", Variables = "Area=ElementLayout")]
    public class StackPanelPropReLayout : CodeTest
    {
        public StackPanelPropReLayout()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;
        //bool isTrasformed = false;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;

            StackPanelCommon.AddChildren(25, "Border", _stack, TestLog.Current);

            _root.Children.Add(_stack);

            return _root;
        }

        public override void TestActions()
        {
            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();

            _stack.Orientation = Orientation.Horizontal;
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();

            _stack.Orientation = Orientation.Vertical;
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();

            _stack.Orientation = Orientation.Horizontal;
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddChildren(100, "Border", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            _stack.Orientation = Orientation.Vertical;
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();

            _stack.Orientation = Orientation.Horizontal;
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();

            StackPanelCommon.AddChildren(1000, "Border", _stack, TestLog.Current);
            CommonFunctionality.FlushDispatcher();

            _stack.Orientation = Orientation.Vertical;
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();

            _stack.Orientation = Orientation.Horizontal;
            CommonFunctionality.FlushDispatcher();

            if (!ValidateReLayout())
            {
                _tempresult = false;
            }
            CommonFunctionality.FlushDispatcher();

        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }

        bool ValidateReLayout()
        {
            bool validationResult = true;
            switch (_stack.Orientation)
            {
                case Orientation.Vertical:
                    validationResult = ValidateStackSize();
                    if (validationResult)
                    {
                        validationResult = StackPanelCommon.VerifyVerticalStacking("", _stack, TestLog.Current);
                    }
                    break;

                case Orientation.Horizontal:
                    validationResult = ValidateStackSize();
                    if (validationResult)
                    {
                        validationResult = StackPanelCommon.VerifyHorizontalStacking("", _stack, TestLog.Current);
                    }
                    break;
            }
            return validationResult;
        }

        bool ValidateStackSize()
        {
            bool validationResult = true;

            switch (_stack.Orientation)
            {
                case Orientation.Vertical:

                    FrameworkElement vChildOne = _stack.Children[0] as FrameworkElement;
                    FrameworkElement vStackParent = _stack.Parent as FrameworkElement;
                    double allChildHeight = 0;
                    foreach (UIElement child in _stack.Children)
                    {
                        allChildHeight += ((FrameworkElement)child).ActualHeight;
                    }

                    if (_stack.ActualWidth >= vChildOne.ActualWidth && _stack.ActualWidth <= vStackParent.ActualWidth && _stack.ActualHeight == allChildHeight)
                    {
                        validationResult = true;
                    }
                    else
                    {
                        Helpers.Log("StackPanel Size was incorrect after Re-Layout.");
                        validationResult = false;
                    }


                    break;

                case Orientation.Horizontal:
                    //validationResult = StackPanelCommon.VerifyHorizontalStacking();
                    FrameworkElement hChildOne = _stack.Children[0] as FrameworkElement;
                    FrameworkElement hStackParent = _stack.Parent as FrameworkElement;
                    double allChildWidth = 0;
                    foreach (UIElement child in _stack.Children)
                    {
                        allChildWidth += ((FrameworkElement)child).ActualWidth;
                    }

                    if (_stack.ActualHeight >= hChildOne.ActualHeight && _stack.ActualHeight <= hStackParent.ActualHeight && _stack.ActualWidth == allChildWidth)
                    {
                        validationResult = true;
                    }
                    else
                    {
                        Helpers.Log("StackPanel Size was incorrect after Re-Layout.");
                        validationResult = false;
                    }
                    break;
            }

            return validationResult;
        }

        //    bool StackPanelCommon.VerifyHorizontalStacking()
        //    {
        //        Helpers.Log("Orientation : " + stack.Orientation);

        //        FrameworkElement currentChild = null;
        //        FrameworkElement previousChild = null;

        //        Point currentChildLoc;
        //        Point previousChildLoc;

        //        Transform currentChildTransform;
        //        Transform previousChildTransform;

        //        int ChildCount = 0;

        //        IEnumerator children = LogicalTreeHelper.GetChildren(stack).GetEnumerator();

        //        if (children != null)
        //        {
        //            while (children.MoveNext())
        //            {
        //                if (!isTrasformed)
        //                {
        //                    ChildCount++;
        //                    currentChild = children.Current as FrameworkElement;
        //                    if (currentChild == stack.Children[0])
        //                    {
        //                        currentChildLoc = LayoutUtility.GetElementPosition(currentChild, stack);
        //                        if (currentChildLoc.X != (0 + currentChild.Margin.Left))
        //                        {
        //                            Helpers.Log("child " + ChildCount + " location is not ok");
        //                            //TestResult = false;
        //                            return false;
        //                        }
        //                        else
        //                        {
        //                            //Helpers.Log("child " + ChildCount + " location is ok");
        //                            //TestResult = true;
        //                            previousChild = currentChild;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        previousChildLoc = LayoutUtility.GetElementPosition(previousChild, stack);
        //                        currentChildLoc = LayoutUtility.GetElementPosition(currentChild, stack);

        //                        if (previousChild != null)
        //                        {
        //                            if (currentChildLoc.X != (previousChildLoc.X + previousChild.ActualWidth + previousChild.Margin.Right + currentChild.Margin.Left))
        //                            {
        //                                Helpers.Log("child " + ChildCount + " location is not ok");
        //                                //TestResult = false;
        //                                return false;
        //                            }
        //                            else
        //                            {
        //                                //Helpers.Log("child " + ChildCount + " location is ok");
        //                                //TestResult = true;
        //                                previousChild = currentChild;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Helpers.Log("previous child is null, test cannot continue.");
        //                            //TestResult = false;
        //                            return false;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    ChildCount++;
        //                    currentChild = children.Current as FrameworkElement;
        //                    currentChildTransform = VisualTreeHelper.GetTransform(currentChild);
        //                    double getSquareValue = ((currentChild.ActualHeight * currentChild.ActualHeight) + (currentChild.ActualWidth * currentChild.ActualWidth));

        //                    double rotatedWidth = Math.Sqrt(getSquareValue);
        //                    if (currentChild == stack.Children[0])
        //                    {
        //                        if (currentChildTransform.Value.OffsetX != (rotatedWidth / 2))
        //                        {
        //                            Helpers.Log("child " + ChildCount + " location is not ok");
        //                            //TestResult = false;
        //                            return false;
        //                        }
        //                        else
        //                        {
        //                            //Helpers.Log("child " + ChildCount + " location is ok");
        //                            //TestResult = true;
        //                            previousChild = currentChild;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        currentChildTransform = VisualTreeHelper.GetTransform(currentChild);
        //                        previousChildTransform = VisualTreeHelper.GetTransform(previousChild);

        //                        getSquareValue = ((previousChild.ActualHeight * previousChild.ActualHeight) + (previousChild.ActualWidth * previousChild.ActualWidth));

        //                        rotatedWidth = Math.Sqrt(getSquareValue);

        //                        double offset = rotatedWidth / 2;

        //                        if (previousChild != null)
        //                        {
        //                            if (currentChildTransform.Value.OffsetX != (rotatedWidth * (ChildCount - 1) + offset))
        //                            {
        //                                Helpers.Log("child " + ChildCount + " location is not ok");
        //                                //TestResult = false;
        //                                return false;
        //                            }
        //                            else
        //                            {
        //                                //Helpers.Log("child " + ChildCount + " location is ok");
        //                                //TestResult = true;
        //                                previousChild = currentChild;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Helpers.Log("previous child is null, test cannot continue.");
        //                            //TestResult = false;
        //                            return false;
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //        else
        //        {
        //            Helpers.Log("Child enumerator is null! Test cannot continue.");
        //            //TestResult = false;
        //            return false;
        //        }
        //        return true;
        //    }

        //    bool StackPanelCommon.VerifyVerticalStacking()
        //    {
        //        Helpers.Log("Orientation : " + stack.Orientation);

        //        FrameworkElement currentChild = null;
        //        FrameworkElement previousChild = null;

        //        Point currentChildLoc;
        //        Point previousChildLoc;

        //        Transform currentChildTransform;
        //        Transform previousChildTransform;

        //        int ChildCount = 0;

        //        IEnumerator children = LogicalTreeHelper.GetChildren(stack).GetEnumerator();

        //        if (children != null)
        //        {
        //            while (children.MoveNext())
        //            {
        //                if (!isTrasformed)
        //                {
        //                    ChildCount++;
        //                    currentChild = children.Current as FrameworkElement;
        //                    if (currentChild == stack.Children[0])
        //                    {
        //                        currentChildLoc = LayoutUtility.GetElementPosition(currentChild, stack);
        //                        if (currentChildLoc.Y != (0 + currentChild.Margin.Top))
        //                        {
        //                            Helpers.Log("child " + ChildCount + " location is not ok");
        //                            //TestResult = false;
        //                            return false;
        //                        }
        //                        else
        //                        {
        //                            //Helpers.Log("child " + ChildCount + " location is ok");
        //                            //TestResult = true;
        //                            previousChild = currentChild;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        previousChildLoc = LayoutUtility.GetElementPosition(previousChild, stack);
        //                        currentChildLoc = LayoutUtility.GetElementPosition(currentChild, stack);

        //                        if (previousChild != null)
        //                        {
        //                            if (currentChildLoc.Y != (previousChildLoc.Y + previousChild.ActualHeight + previousChild.Margin.Bottom + currentChild.Margin.Top))
        //                            {
        //                                Helpers.Log("child " + ChildCount + " location is not ok");
        //                                //TestResult = false;
        //                                return false;
        //                            }
        //                            else
        //                            {
        //                                //Helpers.Log("child " + ChildCount + " location is ok");
        //                                //TestResult = true;
        //                                previousChild = currentChild;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Helpers.Log("previous child is null, test cannot continue.");
        //                            //TestResult = false;
        //                            return false;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    ChildCount++;
        //                    currentChild = children.Current as FrameworkElement;
        //                    currentChildTransform = VisualTreeHelper.GetTransform(currentChild);

        //                    if (currentChild == stack.Children[0])
        //                    {
        //                        if (currentChildTransform.Value.OffsetY != 0)
        //                        {
        //                            Helpers.Log("child " + ChildCount + " location is not ok");
        //                            //TestResult = false;
        //                            return false;
        //                        }
        //                        else
        //                        {
        //                            //Helpers.Log("child " + ChildCount + " location is ok");
        //                            //TestResult = true;
        //                            previousChild = currentChild;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        currentChildTransform = VisualTreeHelper.GetTransform(currentChild);
        //                        previousChildTransform = VisualTreeHelper.GetTransform(previousChild);

        //                        double getSquareValue = ((previousChild.ActualHeight * previousChild.ActualHeight) + (previousChild.ActualWidth * previousChild.ActualWidth));

        //                        double rotatedHeight = Math.Sqrt(getSquareValue);

        //                        if (previousChild != null)
        //                        {
        //                            if (currentChildTransform.Value.OffsetY != (rotatedHeight * (ChildCount - 1)))
        //                            {
        //                                Helpers.Log("child " + ChildCount + " location is not ok");
        //                                //TestResult = false;
        //                                return false;
        //                            }
        //                            else
        //                            {
        //                                //Helpers.Log("child " + ChildCount + " location is ok");
        //                                //TestResult = true;
        //                                previousChild = currentChild;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            Helpers.Log("previous child is null, test cannot continue.");
        //                            //TestResult = false;
        //                            return false;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Helpers.Log("Child enumerator is null! Test cannot continue.");
        //            //TestResult = false;
        //            return false;
        //        }
        //        return true;
        //    }

        //    void StackPanelCommon.AddChildren(int ChildCount)
        //    {
        //        for (int i = 0; i < ChildCount; i++)
        //        {
        //            Border child = new Border();
        //            child.Height = 100;
        //            child.Width = 100;
        //            child.Background = Brushes.Gray;
        //            stack.Children.Add(child);
        //        }
        //    }
    }

    [Test(1, "Panels.StackPanel", "StackPanelHorizontalScrolling", Variables = "Area=ElementLayout")]
    public class StackPanelHorizontalScrolling : CodeTest
    {
        public StackPanelHorizontalScrolling()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "StackPanelHScroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        private ListBox _lbx = null;
        private bool _testStarted = false;
        private bool _loaded = false;

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }


            Scroll(_lbxStack, "StackPanelLineRight", 5);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "StackPanelLineLeft", 77);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "StackPanelPageRight", 3);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "StackPanelPageLeft", 7);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "StackPanelMouseWheelRight", 93);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "StackPanelMouseWheelLeft", 74);

        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }

        private void Scroll(StackPanel stack, string ScrollCommand, int ScrollAmount)
        {
            switch (ScrollCommand)
            {
                case "StackPanelLineRight":
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.LineRight();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "StackPanelLineLeft":
                    stack.SetHorizontalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.LineLeft();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "StackPanelPageRight":
                    stack.SetHorizontalOffset(0);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.PageRight();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "StackPanelPageLeft":
                    stack.SetHorizontalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.PageLeft();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "StackPanelMouseWheelRight":
                    stack.SetHorizontalOffset(0);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.MouseWheelRight();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "StackPanelMouseWheelLeft":
                    stack.SetHorizontalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.MouseWheelLeft();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;
            }
        }

        void Verify(StackPanel stack, string ScrollCommand, int ScrollAmount)
        {
            switch (ScrollCommand)
            {
                case "StackPanelLineRight":
                    if (stack.HorizontalOffset != ScrollAmount)
                    {
                        Helpers.Log("StackPanelLine RIGHT FAIL..");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("StackPanelLine RIGHT PASS..");
                    }
                    break;

                case "StackPanelLineLeft":
                    if ((stack.HorizontalOffset + ScrollAmount) != stack.ExtentWidth - 3)
                    {
                        Helpers.Log("StackPanelLine LEFT FAIL..");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("StackPanelLine LEFT PASS..");
                    }
                    break;

                case "StackPanelPageRight":
                    if (stack.HorizontalOffset != (ScrollAmount * stack.ViewportWidth))
                    {
                        Helpers.Log("StackPanelPage RIGHT FAIL..");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("StackPanelPage RIGHT PASS..");
                    }
                    break;

                case "StackPanelPageLeft":
                    if ((stack.HorizontalOffset + (ScrollAmount * stack.ViewportWidth)) != stack.ExtentWidth - 3)
                    {
                        Helpers.Log("StackPanelPage LEFT FAIL..");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("StackPanelPage LEFT PASS..");
                    }
                    break;

                case "StackPanelMouseWheelRight":
                    if (stack.HorizontalOffset != (ScrollAmount * 3))
                    {
                        Helpers.Log("StackPanelMouse WHEEL RIGHT FAIL..");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("StackPanelMouse WHEEL RIGHT PASS..");
                    }
                    break;

                case "StackPanelMouseWheelLeft":
                    if ((stack.HorizontalOffset + (ScrollAmount * 3)) != stack.ExtentWidth - 3)
                    {
                        Helpers.Log("StackPanelMouse WHEEL LEFT FAIL..");
                        this.Result = false;
                    }
                    else
                    {
                        Helpers.Log("StackPanelMouse WHEEL LEFT PASS..");
                    }
                    break;
            }
        }
    }

    [Test(1, "Panels.StackPanel", "StackPanelVerticalScrolling", Variables = "Area=ElementLayout")]
    public class StackPanelVerticalScrolling : CodeTest
    {
        public StackPanelVerticalScrolling()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;


            string xamlfile = "StackPanelVScroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override FrameworkElement TestContent()
        {
            return null;
        }

        private ListBox _lbx = null;
        private bool _testStarted = false;
        private bool _loaded = false;

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;

                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, "LineDown", 301);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "LineUp", 209);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "PageDown", 3);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "PageUp", 7);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "MouseWheelDown", 93);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "MouseWheelUp", 99);

        }

        private void Scroll(StackPanel stack, string ScrollCommand, int ScrollAmount)
        {
            OperatingSystem os = Environment.OSVersion;
            Version ver = os.Version;
            switch (ScrollCommand)
            {
                case "LineDown":
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.LineDown();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "LineUp":
                    stack.SetVerticalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.LineUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    //For Win8 OS show Stackpanel stely different with other's OS So make stackpanel up rise once more
                    if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1))
                    {
                        stack.LineUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "PageDown":
                    stack.SetVerticalOffset(0);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.PageDown();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "PageUp":
                    stack.SetVerticalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.PageUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    //For Win8 OS show Stackpanel stely different with other's OS So change  stackpanel.VerticalOffset befor Verify
                    if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1))
                    {
                        stack.LineDown();
                        stack.LineUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "MouseWheelDown":
                    stack.SetVerticalOffset(0);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.MouseWheelDown();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "MouseWheelUp":
                    stack.SetVerticalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        stack.MouseWheelUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    //For Win8 OS show Stackpanel stely different with other's OS So make stackpanel up rise once more
                    if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1)) 
                    {
                        stack.LineUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;
            }
        }

        void Verify(StackPanel stack, string ScrollCommand, int ScrollAmount)
        {
            switch (ScrollCommand)
            {
                case "LineDown":
                    if (stack.VerticalOffset != ScrollAmount)
                    {
                        Helpers.Log("LINE DOWN FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("LINE DOWN PASS..");
                    }
                    break;

                case "LineUp":
                    if (stack.VerticalOffset != (_itemCount - ScrollAmount - 4))
                    {
                        Helpers.Log(stack.VerticalOffset);
                        Helpers.Log(_itemCount - ScrollAmount);
                        Helpers.Log("LINE UP FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("LINE UP PASS..");
                    }
                    break;

                case "PageDown":
                    if (stack.VerticalOffset != (ScrollAmount * stack.ViewportHeight))
                    {
                        Helpers.Log("PAGE DOWN FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("PAGE DOWN PASS..");
                    }
                    break;

                case "PageUp":
                    if ((stack.VerticalOffset + (ScrollAmount * stack.ViewportHeight)) != (stack.ExtentHeight - 4))
                    {
                        Helpers.Log("PAGE UP FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("PAGE UP PASS..");
                    }
                    break;

                case "MouseWheelDown":
                    if (stack.VerticalOffset != (ScrollAmount * 3))
                    {
                        Helpers.Log("MOUSE WHEEL DOWN FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("MOUSE WHEEL DOWN PASS..");
                    }
                    break;

                case "MouseWheelUp":
                    if ((stack.VerticalOffset + (ScrollAmount * 3)) != (stack.ExtentHeight - 4))
                    {
                        Helpers.Log("MOUSE WHEEL UP FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("MOUSE WHEEL UP PASS..");
                    }
                    break;
            }
        }

        bool _tempResult = true;
        public override void TestVerify()
        {
            this.Result = this._tempResult;
        }
    }

    #region VScan Scroll Cases

    [Test(2, "Panels.StackPanel", "StackPanelLineRightScrollVScan", Variables = "Area=ElementLayout")]
    public class StackPanelLineRightScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelLineRightScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelHScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 256);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.LineRight();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelLineLeftScrollVScan",
        Variables = "Area=ElementLayout")]
    public class StackPanelLineLeftScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelLineLeftScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelHScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 256);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetHorizontalOffset(_itemCount);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.LineLeft();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelPageRightScrollVScan",
        Variables = "Area=ElementLayout")]
    public class StackPanelPageRightScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelPageRightScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelHScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 10);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetHorizontalOffset(0);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.PageRight();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelPageLeftScrollVScan", Variables = "Area=ElementLayout")]
    public class StackPanelPageLeftScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelPageLeftScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelHScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 10);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetHorizontalOffset(_itemCount);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.PageLeft();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelMouseWheelRightScrollVScan", Variables = "Area=ElementLayout")]
    public class StackPanelMouseWheelRightScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelMouseWheelRightScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelHScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 100);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetHorizontalOffset(0);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.MouseWheelRight();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelMouseWheelLeftScrollVScan", Variables = "Area=ElementLayout")]
    public class StackPanelMouseWheelLeftScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelMouseWheelLeftScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelHScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 100);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetHorizontalOffset(_itemCount);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.MouseWheelLeft();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelLineDownScrollVScan", Variables = "Area=ElementLayout")]
    public class StackPanelLineDownScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelLineDownScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelVScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 256);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.LineDown();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelLineUpScrollVScan", Variables = "Area=ElementLayout")]
    public class StackPanelLineUpScrollVScan : CodeTest
    {
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private ListBox _lbx = null;
        private bool _testStarted = false;
        private bool _loaded = false;
        private int _itemCount = 0;

        public StackPanelLineUpScrollVScan()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelVScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 256);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetVerticalOffset(_itemCount);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.LineUp();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelPageDownScrollVScan",
        Variables = "Area=ElementLayout")]
    public class StackPanelPageDownScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelPageDownScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelVScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 10);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetVerticalOffset(0);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.PageDown();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelPageUpScrollVScan", Variables = "Area=ElementLayout")]
    public class StackPanelPageUpScrollVScan : CodeTest
    {
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private ListBox _lbx = null;
        private bool _testStarted = false;
        private bool _loaded = false;
        private int _itemCount = 0;

        public StackPanelPageUpScrollVScan()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelVScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 10);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetVerticalOffset(_itemCount);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.PageUp();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelMouseWheelDownScrollVScan", Variables = "Area=ElementLayout")]
    public class StackPanelMouseWheelDownScrollVScan : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelMouseWheelDownScrollVScan()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelVScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 100);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetVerticalOffset(0);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.MouseWheelDown();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    [Test(2, "Panels.StackPanel", "StackPanelMouseWheelUpScrollVScan",
        Variables = "Area=ElementLayout")]
    public class StackPanelMouseWheelUpScrollVScan : CodeTest
    {
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private ListBox _lbx = null;
        private int _itemCount = 0;
        private bool _testStarted = false;
        private bool _loaded = false;

        public StackPanelMouseWheelUpScrollVScan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = Helpers.LoadContent("StackPanelVScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;
                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            Scroll(_lbxStack, 100);
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, int ScrollAmount)
        {
            stack.SetVerticalOffset(_itemCount);
            CommonFunctionality.FlushDispatcher();
            for (int i = 0; i < ScrollAmount; i++)
            {
                stack.MouseWheelUp();
                CommonFunctionality.FlushDispatcher();
            }
        }
    }

    #endregion

    #region Content Prop Change

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeRectangle", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeRectangle : CodeTest
    {


        public StackPanelContentPropChangeRectangle()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.SlateBlue));
            _stack.Children.Add(_rect);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _rect.Width = _rect.ActualWidth * 2;
            _rect.Height = _rect.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeButton", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeButton : CodeTest
    {


        public StackPanelContentPropChangeButton()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        StackPanel _stack;

        Button _btn;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _btn = CommonFunctionality.CreateButton(200, 200, Brushes.Red);
            _stack.Children.Add(_btn);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);


            _btn.Width = _btn.ActualWidth * 2;
            _btn.Height = _btn.ActualHeight * 2;
            _btn.Content = "Button Size Changed~!";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeTextBox", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeTextBox : CodeTest
    {


        public StackPanelContentPropChangeTextBox()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        StackPanel _stack;

        TextBox _tbox;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _tbox = CommonFunctionality.CreateTextBox(150, 100, "Width=150px");
            _stack.Children.Add(_tbox);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _tbox.Width = _tbox.ActualWidth * 2;
            _tbox.Height = _tbox.ActualHeight * 2;
            _tbox.Text = "Size changed * 2";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeEllipse", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeEllipse : CodeTest
    {


        public StackPanelContentPropChangeEllipse()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        StackPanel _stack;

        Ellipse _elps;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _elps = new Ellipse();
            _elps.Width = 150;
            _elps.Height = 150;
            _elps.Fill = new SolidColorBrush(Colors.SlateBlue);
            _stack.Children.Add(_elps);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _elps.Width = _elps.ActualWidth * 2;
            _elps.Height = _elps.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeImage", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeImage : CodeTest
    {


        public StackPanelContentPropChangeImage()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        Image _img;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _img = CommonFunctionality.CreateImage("light.bmp");
            _img.Height = 50;
            _img.Width = 50;
            _stack.Children.Add(_img);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _img.Width = _img.ActualWidth * 2;
            _img.Height = _img.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeText", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeText : CodeTest
    {


        public StackPanelContentPropChangeText()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        TextBlock _txt;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _txt = CommonFunctionality.CreateText("Testing Grid...");
            _stack.Children.Add(_txt);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _txt.Text = "Changing Text to very large text... Changing Text to very large text...";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeBorder", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeBorder : CodeTest
    {


        public StackPanelContentPropChangeBorder()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        Border _b;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _b = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.SlateBlue), 25, 200);
            _stack.Children.Add(_b);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _b.Width = _b.ActualWidth * 2;
            _b.Height = _b.ActualHeight * 2;
            _b.BorderThickness = new Thickness(20);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeLabel", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeLabel : CodeTest
    {


        public StackPanelContentPropChangeLabel()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        Label _lbl;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lbl = new Label();
            _lbl.Content = "Testing StackPanel with Label~!";
            _stack.Children.Add(_lbl);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _lbl.Content = "content changed. content changed.content changed. content changed. content changed. content changed. content changed.";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeListBox", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeListBox : CodeTest
    {


        public StackPanelContentPropChangeListBox()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        ListBox _lb;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lb = CommonFunctionality.CreateListBox(10);
            _stack.Children.Add(_lb);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ListBoxItem li = new ListBoxItem();
            li.Content = "List Item added with Long Text...~";
            _lb.Items.Add(li);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeMenu", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeMenu : CodeTest
    {


        public StackPanelContentPropChangeMenu()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        Menu _menu;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _menu = CommonFunctionality.CreateMenu(5);
            _stack.Children.Add(_menu);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            MenuItem mi = new MenuItem();
            mi.Header = "Menu Item Added~!!!";
            _menu.Items.Add(mi);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeCanvas", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeCanvas : CodeTest
    {


        public StackPanelContentPropChangeCanvas()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        Canvas _canvas;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _canvas = new Canvas();
            _canvas.Height = 100;
            _canvas.Width = 100;
            _canvas.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle cRect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(cRect, 10);
            Canvas.SetTop(cRect, 10);
            _canvas.Children.Add(cRect);
            _stack.Children.Add(_canvas);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            Rectangle crect = CommonFunctionality.CreateRectangle(40, 40, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(crect, 30);
            Canvas.SetTop(crect, 30);
            _canvas.Children.Add(crect);
            _canvas.Width = _canvas.ActualWidth * 2;
            _canvas.Height = _canvas.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeDockPanel", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeDockPanel : CodeTest
    {


        public StackPanelContentPropChangeDockPanel()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        DockPanel _dockpanel;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _dockpanel = new DockPanel();
            _dockpanel.Background = new SolidColorBrush(Colors.SlateBlue);
            _dockpanel.LastChildFill = true;
            Rectangle rect0 = CommonFunctionality.CreateRectangle(20, 100, new SolidColorBrush(Colors.Red));
            DockPanel.SetDock(rect0, Dock.Top);
            _dockpanel.Children.Add(rect0);
            Rectangle rect1 = CommonFunctionality.CreateRectangle(20, 100, new SolidColorBrush(Colors.Blue));
            DockPanel.SetDock(rect1, Dock.Left);
            _dockpanel.Children.Add(rect1);
            Rectangle rect2 = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, new SolidColorBrush(Colors.Yellow));
            //DockPanel.SetDock(rect2, Dock.Fill);
            _dockpanel.Children.Add(rect2);
            _stack.Children.Add(_dockpanel);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _dockpanel.Width = _dockpanel.ActualWidth * 2;
            _dockpanel.Height = _dockpanel.ActualHeight * 2;
            DockPanel.SetDock(_dockpanel.Children[0], Dock.Right);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeStackPanel", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeStackPanel : CodeTest
    {


        public StackPanelContentPropChangeStackPanel()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        StackPanel _s;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _s = new StackPanel();
            _s.Width = 200;
            _stack.Children.Add(_s);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            Rectangle sChild1 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Orange));
            Rectangle sChild2 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Blue));
            Rectangle sChild3 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.DarkSeaGreen));
            _stack.Children.Add(sChild1);
            _stack.Children.Add(sChild2);
            _stack.Children.Add(sChild3);
            _stack.Width = 150;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    [Test(3, "Panels.StackPanel", "StackPanelContentPropChangeGrid", Variables = "Area=ElementLayout")]
    public class StackPanelContentPropChangeGrid : CodeTest
    {


        public StackPanelContentPropChangeGrid()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        StackPanel _stack;

        Grid _g;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _stack = new StackPanel();
            _stack.Background = Brushes.RoyalBlue;
            _stack.HorizontalAlignment = HorizontalAlignment.Center;
            _stack.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _g = GridCommon.CreateGrid(2, 2);
            _g.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle gRect0 = CommonFunctionality.CreateRectangle(10, 10, new SolidColorBrush(Colors.Red));
            GridCommon.PlacingChild(_g, gRect0, 0, 0);
            _g.Children.Add(gRect0);

            Rectangle gRect1 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Blue));
            GridCommon.PlacingChild(_g, gRect1, 1, 0);
            _g.Children.Add(gRect1);

            Rectangle gRect2 = CommonFunctionality.CreateRectangle(30, 30, new SolidColorBrush(Colors.Yellow));
            GridCommon.PlacingChild(_g, gRect2, 0, 1);
            _g.Children.Add(gRect2);

            _stack.Children.Add(_g);

            _root.Children.Add(_stack);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _stack.RenderSize;

            _relayoutOccurred = false;
            _stack.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ColumnDefinition ccd = new ColumnDefinition();
            _g.ColumnDefinitions.Add(ccd);
            _g.Width = _g.ActualWidth * 2;
            _g.Height = _g.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _stack.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_stack.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but StackPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and StackPanel Size Changed!!!");
                    this.Result = true;
                }
            }
            else
            {
                Helpers.Log("Layout did not updated~!!!");
                this.Result = false;
            }
        }


        Size _preTestSize = new Size();
        bool _relayoutOccurred;
        void OnLayoutOccured(object sender, EventArgs e)
        {
            _relayoutOccurred = true;
        }
    }

    #endregion

    [Test(2, "Panels.StackPanel", "StackPanelLeftyVerticalScrolling", Variables = "Area=ElementLayout")]
    public class StackPanelLeftyVerticalScrolling : CodeTest
    {
        private ListBox _lbx = null;
        private ScrollViewer _lbxScroll = null;
        private StackPanel _lbxStack = null;
        private bool _tempResult = true;
        private bool _testStarted = false;
        private bool _loaded = false;
        private int _itemCount = 0;

        public StackPanelLeftyVerticalScrolling()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = Helpers.LoadContent("StackPanelLeftyVScroll.xaml");
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override void TestActions()
        {
            while (!_loaded)
            {
                CommonFunctionality.FlushDispatcher();
                EnterTest();
            }

            //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
            _itemCount = _lbx.Items.Count;
            int count = VisualTreeHelper.GetChildrenCount(_lbx);

            for (int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(_lbx, i);
                if (v.GetType().Name == "Border")
                {
                    Border lbxborder = v as Border;
                    if (lbxborder.Child.GetType().Name == "ScrollViewer")
                    {
                        _lbxScroll = lbxborder.Child as ScrollViewer;

                    }
                }
            }

            if (_lbxScroll != null)
            {
                if (_lbxScroll.Content.GetType().Name == "StackPanel")
                {
                    _lbxStack = _lbxScroll.Content as StackPanel;
                }
            }

            ((IScrollInfo)_lbxStack).CanVerticallyScroll = true;
            ((IScrollInfo)_lbxStack).CanHorizontallyScroll = true;
            ((IScrollInfo)_lbxStack).ScrollOwner = _lbxScroll;

            Scroll(_lbxStack, "LineDown", 301);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "LineUp", 209);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "PageDown", 3);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "PageUp", 7);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "MouseWheelDown", 93);
            CommonFunctionality.FlushDispatcher();
            Scroll(_lbxStack, "MouseWheelUp", 99);

        }

        public override void TestVerify()
        {
            this.Result = this._tempResult;
        }

        private void EnterTest()
        {
            _lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

            if (_lbx != null)
            {
                if (_lbx.HasItems)
                {
                    if (!_testStarted)
                    {
                        _loaded = true;
                        _testStarted = true;
                    }
                }
            }
        }

        private void Scroll(StackPanel stack, string ScrollCommand, int ScrollAmount)
        {
            OperatingSystem os = Environment.OSVersion;
            Version verleft = os.Version;
            switch (ScrollCommand)
            {
                case "LineDown":
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        ((IScrollInfo)stack).LineDown();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "LineUp":
                    stack.SetVerticalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        ((IScrollInfo)stack).LineUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    //For Win8 OS show Stackpanel stely different with other's OS So make stackpanel up rise once more
                    if (verleft.Major > 6 || ((6 == verleft.Major) && verleft.Minor > 1))
                    {
                        ((IScrollInfo)stack).LineUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "PageDown":
                    stack.SetVerticalOffset(0);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        ((IScrollInfo)stack).PageDown();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "PageUp":
                    stack.SetVerticalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        ((IScrollInfo)stack).PageUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    //For Win8 OS show Stackpanel stely different with other's OS So change  stackpanel.VerticalOffset befor Verify
                    if (verleft.Major > 6 || ((6 == verleft.Major) && verleft.Minor > 1))
                    {
                        ((IScrollInfo)stack).LineDown();
                        ((IScrollInfo)stack).LineUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "MouseWheelDown":
                    stack.SetVerticalOffset(0);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        ((IScrollInfo)stack).MouseWheelDown();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;

                case "MouseWheelUp":
                    stack.SetVerticalOffset(_itemCount);
                    CommonFunctionality.FlushDispatcher();
                    for (int i = 0; i < ScrollAmount; i++)
                    {
                        ((IScrollInfo)stack).MouseWheelUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    //For Win8 OS show Stackpanel stely different with other's OS So make stackpanel up rise once more
                    if (verleft.Major > 6 || ((6 == verleft.Major) && verleft.Minor > 1))
                    {
                        ((IScrollInfo)stack).LineUp();
                        CommonFunctionality.FlushDispatcher();
                    }
                    Verify(stack, ScrollCommand, ScrollAmount);
                    break;
            }
        }

        private void Verify(StackPanel stack, string ScrollCommand, int ScrollAmount)
        {
            switch (ScrollCommand)
            {
                case "LineDown":
                    if (stack.VerticalOffset != ScrollAmount)
                    {
                        Helpers.Log("LINE DOWN FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("LINE DOWN PASS..");
                    }
                    break;

                case "LineUp":
                    if (stack.VerticalOffset != (_itemCount - ScrollAmount - 4))
                    {
                        Helpers.Log("LINE UP FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("LINE UP PASS..");
                    }
                    break;

                case "PageDown":
                    if (stack.VerticalOffset != (ScrollAmount * stack.ViewportHeight))
                    {
                        Helpers.Log("PAGE DOWN FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("PAGE DOWN PASS..");
                    }
                    break;

                case "PageUp":
                    if ((stack.VerticalOffset + (ScrollAmount * stack.ViewportHeight)) != (stack.ExtentHeight - 4))
                    {
                        Helpers.Log("PAGE UP FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("PAGE UP PASS..");
                    }
                    break;

                case "MouseWheelDown":
                    if (stack.VerticalOffset != (ScrollAmount * 3))
                    {
                        Helpers.Log("MOUSE WHEEL DOWN FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("MOUSE WHEEL DOWN PASS..");
                    }
                    break;

                case "MouseWheelUp":
                    if ((stack.VerticalOffset + (ScrollAmount * 3)) != (stack.ExtentHeight - 4))
                    {
                        Helpers.Log("MOUSE WHEEL UP FAIL..");
                        this._tempResult = false;
                    }
                    else
                    {
                        Helpers.Log("MOUSE WHEEL UP PASS..");
                    }
                    break;
            }
        }
    }

    [Test(3, "Panels.StackPanel", "VSPVertical", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class VSPVertical : CodeTest
    {
        public VSPVertical() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Children.Add(new VSP(250, 250, 10, Orientation.Vertical));
            return root;
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Panels.StackPanel", "VSPHorizontal", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class VSPHorizontal : CodeTest
    {
        public VSPHorizontal() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Children.Add(new VSP(250, 250, 10, Orientation.Horizontal));
            return root;
        }

        public override void TestVerify()
        {
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Panels.StackPanel", "VSPDefaultOrientationXAML",
        Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class VSPDefaultOrientationXAML : VisualScanTest
    {
        public VSPDefaultOrientationXAML()
            : base("VSPOrientationDefault.xaml")
        { }
    }

    [Test(3, "Panels.StackPanel", "VSPHorizontalOrientationXAML",
        Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class VSPHorizontalOrientationXAML : VisualScanTest
    {
        public VSPHorizontalOrientationXAML()
            : base("VSPOrientationHorizontal.xaml")
        { }
    }

    [Test(3, "Panels.StackPanel", "VSPVerticalOrientationXAML",
        Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class VSPVerticalOrientationXAML : VisualScanTest
    {
        public VSPVerticalOrientationXAML()
            : base("VSPOrientationVertical.xaml")
        { }
    }
}
