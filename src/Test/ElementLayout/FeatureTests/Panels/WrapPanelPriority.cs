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
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using System.IO;

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.HeightWidthDefault", TestParameters = "target=WrapPanel, test=HeightWidthDefault")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.HeightWidthActual", TestParameters = "target=WrapPanel, test=HeightWidthActual")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.ChildHeightWidth", TestParameters = "target=WrapPanel, test=ChildHeightWidth")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.MinHeightWidth", TestParameters = "target=WrapPanel, test=MinHeightWidth")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.MaxHeightWidth", TestParameters = "target=WrapPanel, test=MaxHeightWidth")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.Visibility", TestParameters = "target=WrapPanel, test=Visibility")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.HorizontalAlignment", TestParameters = "target=WrapPanel, test=HorizontalAlignment")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.VerticalAlignment", TestParameters = "target=WrapPanel, test=VerticalAlignment")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.FlowDirection", TestParameters = "target=WrapPanel, test=FlowDirection")]
    [Test(1, "Panels.WrapPanel", "FrameworkElementProps.Margin", TestParameters = "target=WrapPanel, test=Margin")]
    public class WrapPanelFETest : FrameworkElementPropertiesTest
    {
        public WrapPanelFETest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }
    }

    [Test(1, "Panels.WrapPanel", "WrapPanelAlignments", Variables="Area=ElementLayout")]
    public class WrapPanelAlignments : CodeTest
    {
        public WrapPanelAlignments()
        { }

        public override void WindowSetup()
        {

            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Gray;

            _wrap = new WrapPanel();
            _wrap.Width = 300;
            _wrap.Height = 300;
            _wrap.Background = Brushes.White;
            _wrap.ItemHeight = 150;
            _wrap.ItemWidth = 150;


            Border wrapchild1 = new Border();
            wrapchild1.Margin = new Thickness(3);
            wrapchild1.Background = Brushes.OrangeRed;
            _wrap.Children.Add(wrapchild1);

            Border wrapchild2 = new Border();
            wrapchild2.Margin = new Thickness(3);
            wrapchild2.Background = Brushes.DarkGreen;
            _wrap.Children.Add(wrapchild2);

            Border wrapchild3 = new Border();
            wrapchild3.Margin = new Thickness(3);
            wrapchild3.Background = Brushes.RoyalBlue;
            _wrap.Children.Add(wrapchild3);

            Border wrapchild4 = new Border();
            wrapchild4.Margin = new Thickness(3);
            wrapchild4.Background = Brushes.DarkGoldenrod;
            _wrap.Children.Add(wrapchild4);

            _root.Children.Add(_wrap);

            return _root;
        }

        public override void TestActions()
        {
            _wrap.HorizontalAlignment = HorizontalAlignment.Left;
            _wrap.VerticalAlignment = VerticalAlignment.Top;

            CommonFunctionality.FlushDispatcher();

            ValidateAlign(1);

            CommonFunctionality.FlushDispatcher();

            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            CommonFunctionality.FlushDispatcher();

            ValidateAlign(2);

            CommonFunctionality.FlushDispatcher();

            _wrap.HorizontalAlignment = HorizontalAlignment.Right;
            _wrap.VerticalAlignment = VerticalAlignment.Bottom;

            CommonFunctionality.FlushDispatcher();

            ValidateAlign(3);
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

        void ValidateAlign(int casenum)
        {
            Point wraplocation = LayoutUtility.GetElementPosition(_wrap, _root);

            switch (casenum)
            {
                case 1:
                    if (wraplocation.X != 0 || wraplocation.Y != 0)
                    {
                        Helpers.Log("FAIL : Alignment Test Failed with " + _wrap.HorizontalAlignment + " and " + _wrap.VerticalAlignment + ".");
                        _tempresult = false;
                    }
                    else
                    {

                        Helpers.Log("PASS : Alignment Test Passed with " + _wrap.HorizontalAlignment + " and " + _wrap.VerticalAlignment + ".");
                    }
                    break;

                case 2:
                    if (!DoubleUtil.AreClose(wraplocation.X, ((_root.ActualWidth / 2) - (_wrap.ActualWidth / 2))) || !DoubleUtil.AreClose(wraplocation.Y, ((_root.ActualHeight / 2) - (_wrap.ActualHeight / 2))))
                    {
                        Helpers.Log("FAIL : Alignment Test Failed with " + _wrap.HorizontalAlignment + " and " + _wrap.VerticalAlignment + ".");
                        _tempresult = false;
                    }
                    else
                    {

                        Helpers.Log("PASS : Alignment Test Passed with " + _wrap.HorizontalAlignment + " and " + _wrap.VerticalAlignment + ".");
                    }
                    break;

                case 3:
                    if (!DoubleUtil.AreClose(wraplocation.X, (_root.ActualWidth - _wrap.ActualWidth)) || !DoubleUtil.AreClose(wraplocation.Y, (_root.ActualHeight - _wrap.ActualHeight)))
                    {
                        Helpers.Log("FAIL : Alignment Test Failed with " + _wrap.HorizontalAlignment + " and " + _wrap.VerticalAlignment + ".");
                        _tempresult = false;
                    }
                    else
                    {

                        Helpers.Log("PASS : Alignment Test Passed with " + _wrap.HorizontalAlignment + " and " + _wrap.VerticalAlignment + ".");
                    }
                    break;

            }
        }
    }

    [Test(1, "Panels.WrapPanel", "WrapPanelChildCountRelayout", Variables="Area=ElementLayout")]
    public class WrapPanelChildCountRelayout : CodeTest
    {
        public WrapPanelChildCountRelayout()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _wrap = new WrapPanel();
            _wrap.Background = Brushes.CornflowerBlue;
            _wrap.ItemWidth = 100;
            _wrap.ItemHeight = 100;

            return _wrap;
        }

        public override void TestActions()
        {
            Helpers.Log("Add / Remove Children Test ( Orientation." + _wrap.Orientation + ").");
            AddChildren(1);
            _wrap.Width = 600;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();

            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : One Child.");
            }

            CommonFunctionality.FlushDispatcher();

            for (int i = 0; i < 25; i++)
            {
                AddChildren(100);
                CommonFunctionality.FlushDispatcher();

                if (!VerifyWrap())
                {
                    _tempresult = false;
                    Helpers.Log("FAIL : Add 100 Children.");
                }
            }

            CommonFunctionality.FlushDispatcher();

            for (int i = 0; i < 25; i++)
            {
                RemoveChildren(100);
                CommonFunctionality.FlushDispatcher();

                if (!VerifyWrap())
                {
                    _tempresult = false;
                    Helpers.Log("FAIL : Remove 100 Children.");
                }
            }

            CommonFunctionality.FlushDispatcher();
            _wrap.Children.Clear();
            _wrap.Orientation = Orientation.Vertical;
            _wrap.Width = double.NaN;
            _wrap.Height = 600;
            CommonFunctionality.FlushDispatcher();

            Helpers.Log("Add / Remove Children Test ( Orientation." + _wrap.Orientation + ").");
            AddChildren(1);

            CommonFunctionality.FlushDispatcher();

            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : One Child.");
            }

            CommonFunctionality.FlushDispatcher();

            for (int i = 0; i < 25; i++)
            {
                AddChildren(100);
                CommonFunctionality.FlushDispatcher();

                if (!VerifyWrap())
                {
                    _tempresult = false;
                    Helpers.Log("FAIL : Add 100 Children.");
                }
            }

            CommonFunctionality.FlushDispatcher();

            for (int i = 0; i < 25; i++)
            {
                RemoveChildren(100);
                CommonFunctionality.FlushDispatcher();

                if (!VerifyWrap())
                {
                    _tempresult = false;
                    Helpers.Log("FAIL : Remove 100 Children.");
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

        double _totalChildWidth;
        double _totalChildHeight;
        bool VerifyWrap()
        {
            UIElement PreviousChild = null;
            Point PreviousChildLoc = new Point();
            Point ChildLoc = new Point();

            _totalChildWidth = 0;
            _totalChildHeight = 0;

            bool IsNewLine = false;

            foreach (UIElement child in _wrap.Children)
            {
                int ChildIndex = _wrap.Children.IndexOf(child);
                ChildLoc = LayoutUtility.GetElementPosition(child, _wrap);

                if (ChildIndex == 0)
                {
                    if (ChildLoc.X != 0 || ChildLoc.Y != 0)
                    {
                        Helpers.Log("First Child in " + _wrap.Orientation + " Orientation is not at 0,0.");
                        return false;
                    }
                    else
                    {
                        _totalChildWidth += ((FrameworkElement)child).ActualWidth;
                        _totalChildHeight += ((FrameworkElement)child).ActualHeight;
                        if (DoubleUtil.AreClose(_totalChildHeight, _wrap.ActualHeight) || DoubleUtil.AreClose(_totalChildWidth, _wrap.ActualWidth))
                        {
                            IsNewLine = true;
                        }

                        PreviousChildLoc = ChildLoc;
                        PreviousChild = child;
                    }
                }
                else
                {
                    if (_wrap.Orientation == Orientation.Horizontal)
                    {
                        if (!VerifyHorizontal(ChildLoc, PreviousChild, PreviousChildLoc, IsNewLine))
                        {
                            Helpers.Log("Child " + ChildIndex + " location was not correct.");
                            return false;
                        }
                        else
                        {
                            _totalChildWidth += ((FrameworkElement)child).ActualWidth;
                            if (DoubleUtil.AreClose(_totalChildWidth, _wrap.ActualWidth))
                            {
                                IsNewLine = true;
                            }
                            PreviousChildLoc = ChildLoc;
                            PreviousChild = child;
                        }
                    }
                    else
                    {
                        if (!VerifyVertical(ChildLoc, PreviousChild, PreviousChildLoc, IsNewLine))
                        {
                            Helpers.Log("Child " + ChildIndex + " location was not correct.");
                            return false;
                        }
                        else
                        {
                            _totalChildHeight += ((FrameworkElement)child).ActualHeight;
                            if (DoubleUtil.AreClose(_totalChildHeight, _wrap.ActualHeight))
                            {
                                IsNewLine = true;
                            }

                            PreviousChildLoc = ChildLoc;
                            PreviousChild = child;
                        }
                    }
                }
            }
            return true;
        }

        bool VerifyHorizontal(Point childLoc, UIElement previousChild, Point previousChildLoc, bool isNewLine)
        {
            if (!isNewLine)
            {
                if (childLoc.X != ((FrameworkElement)previousChild).ActualWidth + previousChildLoc.X)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        bool VerifyVertical(Point childLoc, UIElement previousChild, Point previousChildLoc, bool isNewLine)
        {
            if (!isNewLine)
            {
                if (childLoc.Y != ((FrameworkElement)previousChild).ActualHeight + previousChildLoc.Y)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        void AddChildren(int childCount)
        {
            for (int i = 0; i < childCount; i++)
            {
                Border b = new Border();
                b.Background = Brushes.LightGray;
                TextBlock txt = new TextBlock();
                txt.Text = i.ToString();
                txt.Foreground = Brushes.White;
                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Center;
                txt.FontSize = 50;
                b.Child = txt;

                _wrap.Children.Add(b);
            }
        }

        void RemoveChildren(int childCount)
        {
            for (int i = 0; i < childCount; i++)
            {
                _wrap.Children.RemoveAt(_wrap.Children.Count - 1);
            }
        }

    }

    [Test(1, "Panels.WrapPanel", "WrapPanelFlowDirectionTest", Variables="Area=ElementLayout")]
    public class WrapPanelFlowDirectionTest : CodeTest
    {
        public WrapPanelFlowDirectionTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _wrap = new WrapPanel();
            _wrap.FlowDirection = FlowDirection.RightToLeft;
            _wrap.Background = Brushes.CornflowerBlue;
            _wrap.ItemWidth = 100;
            _wrap.ItemHeight = 100;

            AddChildren(9);

            return _wrap;
        }

        public override void TestActions()
        {
            _wrap.Orientation = Orientation.Horizontal;
            _wrap.Width = 300;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FlowDirection.RightToLeft with Orientation.Horizontal test failed.");
            }
            else
            {
                Helpers.Log("FlowDirection.RightToLeft with Orientation.Horizontal Wrap Pass.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Vertical;
            _wrap.Width = double.NaN;
            _wrap.Height = 300;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FlowDirection.RightToLeft with Orientation.Vertical test failed.");
            }
            else
            {
                Helpers.Log("FlowDirection.RightToLeft with Orientation.Vertical Wrap Pass.");
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

        double _totalChildWidth;
        double _totalChildHeight;
        double _wrapOffset;
        bool _isNewLine;

        bool VerifyWrap()
        {
            UIElement PreviousChild = null;
            Point PreviousChildLoc = new Point();
            Point ChildLoc = new Point();

            _totalChildWidth = 0;
            _totalChildHeight = 0;
            _wrapOffset = 0;
            _isNewLine = false;

            foreach (UIElement child in _wrap.Children)
            {
                int ChildIndex = _wrap.Children.IndexOf(child);
                ChildLoc = LayoutUtility.GetElementPosition(child, _wrap);

                if (ChildIndex == 0)
                {
                    if (ChildLoc.X != 0 || ChildLoc.Y != 0)
                    {
                        Helpers.Log("First Child in " + _wrap.Orientation + " Orientation is not at 0,0.");
                        return false;
                    }
                    else
                    {
                        //Helpers.Log("Child " + ChildIndex + " location was correct.");
                        _totalChildWidth += ((FrameworkElement)child).ActualWidth;
                        _totalChildHeight += ((FrameworkElement)child).ActualHeight;

                        PreviousChildLoc = ChildLoc;
                        PreviousChild = child;
                    }
                }
                else
                {
                    if (_wrap.Orientation == Orientation.Horizontal)
                    {
                        if (!VerifyHorizontal(ChildLoc, PreviousChild, PreviousChildLoc, _isNewLine))
                        {
                            Helpers.Log("Child " + ChildIndex + " location was not correct.");
                            return false;
                        }
                        else
                        {
                            //Helpers.Log("Child " + ChildIndex + " location was correct.");
                            _totalChildWidth += ((FrameworkElement)child).ActualWidth;
                            if (DoubleUtil.AreClose(_totalChildWidth, _wrap.ActualWidth))
                            {
                                _isNewLine = true;
                            }
                            PreviousChildLoc = ChildLoc;
                            PreviousChild = child;
                        }
                    }
                    else
                    {
                        if (!VerifyVertical(ChildLoc, PreviousChild, PreviousChildLoc, _isNewLine))
                        {
                            Helpers.Log("Child " + ChildIndex + " location was not correct.");
                            return false;
                        }
                        else
                        {
                            //Helpers.Log("Child " + ChildIndex + " location was correct.");
                            _totalChildHeight += ((FrameworkElement)child).ActualHeight;
                            if (DoubleUtil.AreClose(_totalChildHeight, _wrap.ActualHeight))
                            {
                                _isNewLine = true;
                            }

                            PreviousChildLoc = ChildLoc;
                            PreviousChild = child;
                        }
                    }
                }
            }
            return true;
        }

        bool VerifyHorizontal(Point childLoc, UIElement previousChild, Point previousChildLoc, bool isNewLine)
        {
            if (isNewLine)
            {
                _isNewLine = false;
                _totalChildWidth = 0;
                //this wrap offset value will need to change if non-uniform wrapping.
                _wrapOffset += _wrap.ItemHeight;
            }


            if (!DoubleUtil.AreClose(childLoc.X, _totalChildWidth) || !DoubleUtil.AreClose(childLoc.Y, _wrapOffset))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        bool VerifyVertical(Point childLoc, UIElement previousChild, Point previousChildLoc, bool isNewLine)
        {
            if (isNewLine)
            {
                _isNewLine = false;
                _totalChildHeight = 0;
                //this wrap offset value will need to change if non-uniform wrapping.
                _wrapOffset += _wrap.ItemWidth;
            }

            if (!DoubleUtil.AreClose(childLoc.Y, _totalChildHeight) || !DoubleUtil.AreClose(childLoc.X, _wrapOffset))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        void AddChildren(int childCount)
        {
            for (int i = 0; i < childCount; i++)
            {
                Border b = new Border();
                b.Background = Brushes.LightGray;
                TextBlock txt = new TextBlock();
                txt.Text = i.ToString();
                txt.Foreground = Brushes.White;
                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Center;
                txt.FontSize = 50;
                b.Child = txt;

                _wrap.Children.Add(b);
            }
        }

    }

    [Test(1, "Panels.WrapPanel", "WrapPanelMarginTest", Variables="Area=ElementLayout")]
    public class WrapPanelMarginTest : CodeTest
    {
        public WrapPanelMarginTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;
        Border _wrapborder;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrapborder = new Border();
            _wrapborder.BorderThickness = new Thickness(2);
            _wrapborder.BorderBrush = Brushes.DarkOrange;
            _wrapborder.HorizontalAlignment = HorizontalAlignment.Center;
            _wrapborder.VerticalAlignment = VerticalAlignment.Center;

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.Navy;
            _wrap.ItemHeight = 200;
            _wrap.ItemWidth = 200;

            Border wrapchild1 = new Border();
            wrapchild1.Background = Brushes.Gray;
            _wrap.Children.Add(wrapchild1);

            Border wrapchild2 = new Border();
            wrapchild2.Background = Brushes.Gray;
            _wrap.Children.Add(wrapchild2);

            Border wrapchild3 = new Border();
            wrapchild3.Background = Brushes.Gray;
            _wrap.Children.Add(wrapchild3);

            Border wrapchild4 = new Border();
            wrapchild4.Background = Brushes.Gray;
            _wrap.Children.Add(wrapchild4);

            _wrapborder.Child = _wrap;

            _root.Children.Add(_wrapborder);
            return _root;
        }

        public override void TestActions()
        {
            _wrap.Margin = new Thickness(5);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(5);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(10);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(10);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(20);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(20);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(40);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(40);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(40);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(40);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(20);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(20);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(10);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(10);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(5);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(5);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();
            _wrap.Orientation = Orientation.Vertical;
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(5);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(5);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(10);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(10);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(20);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(20);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(40);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(40);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(40);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(40);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(20);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(20);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(10);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(10);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
            CommonFunctionality.FlushDispatcher();

            _wrap.Margin = new Thickness(5);
            foreach (UIElement child in _wrap.Children)
            {
                ((FrameworkElement)child).Margin = new Thickness(5);
            }

            CommonFunctionality.FlushDispatcher();
            ValidateMargin();
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

        void ValidateMargin()
        {
            Size wrapbordersize = new Size(_wrapborder.ActualWidth, _wrapborder.ActualHeight);
            Size wrapsize = new Size(_wrap.ActualWidth, _wrap.ActualHeight);

            if (!DoubleUtil.AreClose((wrapbordersize.Width - _wrapborder.BorderThickness.Left - _wrapborder.BorderThickness.Right), (wrapsize.Width + _wrap.Margin.Left + _wrap.Margin.Right)))
            {
                Helpers.Log("FAIL : WrapPanel size is incorrect (width).");
                _tempresult = false;
            }
            else if (!DoubleUtil.AreClose((wrapbordersize.Height - _wrapborder.BorderThickness.Top - _wrapborder.BorderThickness.Bottom), (wrapsize.Height + _wrap.Margin.Top + _wrap.Margin.Bottom)))
            {
                Helpers.Log("FAIL : WrapPanel size is incorrect (height).");
                _tempresult = false;
            }
            else
            {
               // Helpers.Log("WrapPanel Size is Correct with Margins, now validate children.");
            }

            foreach (UIElement child in _wrap.Children)
            {
                Size childsize = new Size(((FrameworkElement)child).ActualWidth, ((FrameworkElement)child).ActualHeight);
                if (!DoubleUtil.AreClose((childsize.Height + ((FrameworkElement)child).Margin.Top + ((FrameworkElement)child).Margin.Bottom), _wrap.ItemHeight))
                {
                    Helpers.Log("FAIL : Child Size is incorrect (height).");
                    _tempresult = false;
                }
                else if (!DoubleUtil.AreClose((childsize.Height + ((FrameworkElement)child).Margin.Top + ((FrameworkElement)child).Margin.Bottom), _wrap.ItemHeight))
                {
                    Helpers.Log("FAIL : Child Size is incorrect (width).");
                    _tempresult = false;
                }
                else
                {
                   // Helpers.Log("PASS : Child Size is correct with margins.");
                }
            }
        }
    }

    [Test(2, "Panels.WrapPanel", "WrapPanelSizeRelayout", Variables="Area=ElementLayout")]
    public class WrapPanelSizeRelayout : CodeTest
    {
        public WrapPanelSizeRelayout()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _wrap = new WrapPanel();
            _wrap.Background = Brushes.CornflowerBlue;
            _wrap.ItemWidth = 100;
            _wrap.ItemHeight = 100;

            AddChildren(36);

            return _wrap;
        }

        public override void TestActions()
        {
            _wrap.Width = 600;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();

            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Initial Layout, Orientation.Vertical.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 100;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 100.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 300;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 300.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 600;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 600.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 900;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 900.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 1200;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 1200.");
            }

            CommonFunctionality.FlushDispatcher();


            _wrap.Width = 1500;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 1500.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 1800;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 1800.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 2100;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 2100.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 2400;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 2400.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 2700;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 2700.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 3000;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 3000.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 3300;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 3300.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Width = 3600;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Width 3600.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Vertical;
            _wrap.Width = double.NaN;
            _wrap.Height = 600;
            CommonFunctionality.FlushDispatcher();

            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Initial Layout, Orientation.Vertical.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 100;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 100.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 300;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 300.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 600;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 600.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 900;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 900.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 1200;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 1200.");
            }

            CommonFunctionality.FlushDispatcher();


            _wrap.Height = 1500;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 1500.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 1800;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 1800.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 2100;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 2100.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 2400;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 2400.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 2700;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 2700.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 3000;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 3000.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 3300;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 3300.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Height = 3600;
            _wrap.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("FAIL : Height 3600.");
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


        double _totalChildWidth;
        double _totalChildHeight;
        bool VerifyWrap()
        {
            UIElement PreviousChild = null;
            Point PreviousChildLoc = new Point();
            Point ChildLoc = new Point();

            _totalChildWidth = 0;
            _totalChildHeight = 0;

            bool IsNewLine = false;

            foreach (UIElement child in _wrap.Children)
            {
                int ChildIndex = _wrap.Children.IndexOf(child);
                ChildLoc = LayoutUtility.GetElementPosition(child, _wrap);

                if (ChildIndex == 0)
                {
                    if (ChildLoc.X != 0 || ChildLoc.Y != 0)
                    {
                        Helpers.Log("First Child in " + _wrap.Orientation + " Orientation is not at 0,0.");
                        return false;
                    }
                    else
                    {
                        _totalChildWidth += ((FrameworkElement)child).ActualWidth;
                        _totalChildHeight += ((FrameworkElement)child).ActualHeight;
                        if (DoubleUtil.AreClose(_totalChildHeight, _wrap.ActualHeight) || DoubleUtil.AreClose(_totalChildWidth, _wrap.ActualWidth))
                        {
                            IsNewLine = true;
                        }

                        PreviousChildLoc = ChildLoc;
                        PreviousChild = child;
                    }
                }
                else
                {
                    if (_wrap.Orientation == Orientation.Horizontal)
                    {
                        if (!VerifyHorizontal(ChildLoc, PreviousChild, PreviousChildLoc, IsNewLine))
                        {
                            Helpers.Log("Child " + ChildIndex + " location was not correct.");
                            return false;
                        }
                        else
                        {
                            _totalChildWidth += ((FrameworkElement)child).ActualWidth;
                            if (DoubleUtil.AreClose(_totalChildWidth, _wrap.ActualWidth))
                            {
                                IsNewLine = true;
                            }
                            PreviousChildLoc = ChildLoc;
                            PreviousChild = child;
                        }
                    }
                    else
                    {
                        if (!VerifyVertical(ChildLoc, PreviousChild, PreviousChildLoc, IsNewLine))
                        {
                            Helpers.Log("Child " + ChildIndex + " location was not correct.");
                            return false;
                        }
                        else
                        {
                            _totalChildHeight += ((FrameworkElement)child).ActualHeight;
                            if (DoubleUtil.AreClose(_totalChildHeight, _wrap.ActualHeight))
                            {
                                IsNewLine = true;
                            }

                            PreviousChildLoc = ChildLoc;
                            PreviousChild = child;
                        }
                    }
                }
            }
            return true;
        }

        bool VerifyHorizontal(Point childLoc, UIElement previousChild, Point previousChildLoc, bool isNewLine)
        {
            if (!isNewLine)
            {
                if (childLoc.X != ((FrameworkElement)previousChild).ActualWidth + previousChildLoc.X)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        bool VerifyVertical(Point childLoc, UIElement previousChild, Point previousChildLoc, bool isNewLine)
        {
            if (!isNewLine)
            {
                if (childLoc.Y != ((FrameworkElement)previousChild).ActualHeight + previousChildLoc.Y)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        void AddChildren(int childCount)
        {
            for (int i = 0; i < childCount; i++)
            {
                Border b = new Border();
                b.Background = Brushes.LightGray;
                TextBlock txt = new TextBlock();
                txt.Text = i.ToString();
                txt.Foreground = Brushes.White;
                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Center;
                txt.FontSize = 50;
                b.Child = txt;

                _wrap.Children.Add(b);
            }
        }

    }

    #region Content Prop Change

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeRectangle", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeRectangle : CodeTest
    {
        public WrapPanelContentPropChangeRectangle()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.SlateBlue));
            _wrap.Children.Add(_rect);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _rect.Width = _rect.ActualWidth * 2;
            _rect.Height = _rect.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeButton", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeButton : CodeTest
    {
        public WrapPanelContentPropChangeButton()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        WrapPanel _wrap;

        Button _btn;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _btn = CommonFunctionality.CreateButton(200, 200, Brushes.Red);
            _wrap.Children.Add(_btn);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);


            _btn.Width = _btn.ActualWidth * 2;
            _btn.Height = _btn.ActualHeight * 2;
            _btn.Content = "Button Size Changed~!";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeTextBox", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeTextBox : CodeTest
    {
        public WrapPanelContentPropChangeTextBox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        WrapPanel _wrap;

        TextBox _tbox;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _tbox = CommonFunctionality.CreateTextBox(150, 100, "Width=150px");
            _wrap.Children.Add(_tbox);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _tbox.Width = _tbox.ActualWidth * 2;
            _tbox.Height = _tbox.ActualHeight * 2;
            _tbox.Text = "Size changed * 2";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeEllipse", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeEllipse : CodeTest
    {
        public WrapPanelContentPropChangeEllipse()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        WrapPanel _wrap;

        Ellipse _elps;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _elps = new Ellipse();
            _elps.Width = 150;
            _elps.Height = 150;
            _elps.Fill = new SolidColorBrush(Colors.SlateBlue);
            _wrap.Children.Add(_elps);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _elps.Width = _elps.ActualWidth * 2;
            _elps.Height = _elps.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeImage", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeImage : CodeTest
    {
        public WrapPanelContentPropChangeImage()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        Image _img;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _img = CommonFunctionality.CreateImage("light.bmp");
            _img.Height = 50;
            _img.Width = 50;
            _wrap.Children.Add(_img);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _img.Width = _img.ActualWidth * 2;
            _img.Height = _img.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeText", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeText : CodeTest
    {
        public WrapPanelContentPropChangeText()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        TextBlock _txt;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _txt = CommonFunctionality.CreateText("Testing Grid...");
            _wrap.Children.Add(_txt);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _txt.Text = "Changing Text to very large text... Changing Text to very large text...";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeBorder", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeBorder : CodeTest
    {
        public WrapPanelContentPropChangeBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        Border _b;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _b = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.SlateBlue), 25, 200);
            _wrap.Children.Add(_b);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _b.Width = _b.ActualWidth * 2;
            _b.Height = _b.ActualHeight * 2;
            _b.BorderThickness = new Thickness(20);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeLabel", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeLabel : CodeTest
    {
        public WrapPanelContentPropChangeLabel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        Label _lbl;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lbl = new Label();
            _lbl.Content = "Testing wrapPanel with Label~!";
            _wrap.Children.Add(_lbl);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _lbl.Content = "content changed. content changed.content changed. content changed. content changed. content changed. content changed.";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeListBox", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeListBox : CodeTest
    {
        public WrapPanelContentPropChangeListBox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        ListBox _lb;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lb = CommonFunctionality.CreateListBox(10);
            _wrap.Children.Add(_lb);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ListBoxItem li = new ListBoxItem();
            li.Content = "List Item added with Long Text...~";
            _lb.Items.Add(li);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeMenu", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeMenu : CodeTest
    {
        public WrapPanelContentPropChangeMenu()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        Menu _menu;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _menu = CommonFunctionality.CreateMenu(5);
            _wrap.Children.Add(_menu);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            MenuItem mi = new MenuItem();
            mi.Header = "Menu Item Added~!!!";
            _menu.Items.Add(mi);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeCanvas", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeCanvas : CodeTest
    {
        public WrapPanelContentPropChangeCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        Canvas _canvas;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _canvas = new Canvas();
            _canvas.Height = 100;
            _canvas.Width = 100;
            _canvas.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle cRect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(cRect, 10);
            Canvas.SetTop(cRect, 10);
            _canvas.Children.Add(cRect);
            _wrap.Children.Add(_canvas);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

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
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeDockPanel", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeDockPanel : CodeTest
    {
        public WrapPanelContentPropChangeDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        DockPanel _dockpanel;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

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
            _wrap.Children.Add(_dockpanel);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _dockpanel.Width = _dockpanel.ActualWidth * 2;
            _dockpanel.Height = _dockpanel.ActualHeight * 2;
            DockPanel.SetDock(_dockpanel.Children[0], Dock.Right);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but dockPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and dockPanel Size Changed!!!");
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeStackPanel", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeStackPanel : CodeTest
    {
        public WrapPanelContentPropChangeStackPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        StackPanel _s;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _s = new StackPanel();
            _s.Width = 200;
            _wrap.Children.Add(_s);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            Rectangle sChild1 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Orange));
            Rectangle sChild2 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Blue));
            Rectangle sChild3 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.DarkSeaGreen));
            _s.Children.Add(sChild1);
            _s.Children.Add(sChild2);
            _s.Children.Add(sChild3);
            _s.Width = 150;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
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

    [Test(3, "Panels.WrapPanel", "WrapPanelContentPropChangeGrid", Variables="Area=ElementLayout")]
    public class WrapPanelContentPropChangeGrid : CodeTest
    {
        public WrapPanelContentPropChangeGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        WrapPanel _wrap;

        Grid _g;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _wrap = new WrapPanel();
            _wrap.Background = Brushes.RoyalBlue;
            _wrap.HorizontalAlignment = HorizontalAlignment.Center;
            _wrap.VerticalAlignment = VerticalAlignment.Center;

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

            _wrap.Children.Add(_g);

            _root.Children.Add(_wrap);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _wrap.RenderSize;

            _relayoutOccurred = false;
            _wrap.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ColumnDefinition ccd = new ColumnDefinition();
            _g.ColumnDefinitions.Add(ccd);
            _g.Width = _g.ActualWidth * 2;
            _g.Height = _g.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _wrap.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_wrap.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but wrapPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and wrapPanel Size Changed!!!");
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

    [Test(2, "Panels.WrapPanel", "WrapPanelPanelWrapping", Variables="Area=ElementLayout")]
    public class WrapPanelPanelWrapping : CodeTest
    {
        public WrapPanelPanelWrapping()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _wrap = new WrapPanel();
            _wrap.Width = 900;
            _wrap.Height = 900;
            _wrap.ItemHeight = 300;
            _wrap.ItemWidth = 300;
            return _wrap;
        }

        public override void TestActions()
        {
            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(6, "Panel");
            CommonFunctionality.FlushDispatcher();

            if (!VerifyWrap())
            {
                _tempresult = false;
            }

            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(5, "Canvas");
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
            }


            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(3, "StackPanel");
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
            }


            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(5, "Grid");
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
            }



            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(3, "DockPanel");
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
            }


            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(4, "Decorator");
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
            }


            //wrap.Children.Clear();
            //CommonFunctionality.FlushDispatcher();
            //AddChildren(4, "Viewbox");
            //CommonFunctionality.FlushDispatcher();
            //if (!VerifyWrap())
            //{
            //    tempresult = false;
            //}


            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(6, "Transform");
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
            }


            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(4, "ScrollViewer");
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
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

        double _totalChildWidth;
        double _totalChildHeight;
        double _wrapOffset;
        bool _isNewLine;

        bool VerifyWrap()
        {
            Helpers.Log("Testing " + _wrap.Orientation + " wrapping with child type " + _testtype);
            Helpers.Log("Child Count : " + _wrap.Children.Count);

            UIElement PreviousChild = null;
            Point PreviousChildLoc = new Point();
            Point ChildLoc = new Point();

            _totalChildWidth = 0;
            _totalChildHeight = 0;
            _wrapOffset = 0;
            _isNewLine = false;

            foreach (UIElement child in _wrap.Children)
            {
                int ChildIndex = _wrap.Children.IndexOf(child);
                ChildLoc = LayoutUtility.GetElementPosition(child, _wrap);

                if (ChildIndex == 0)
                {
                    if (ChildLoc.X != 0 || ChildLoc.Y != 0)
                    {
                        Helpers.Log("First Child in " + _wrap.Orientation + " Orientation is not at 0,0.");
                        return false;
                    }
                    else
                    {
                        //Helpers.Log("Child " + ChildIndex + " location was correct.");
                        _totalChildWidth += ((FrameworkElement)child).ActualWidth;
                        _totalChildHeight += ((FrameworkElement)child).ActualHeight;

                        PreviousChildLoc = ChildLoc;
                        PreviousChild = child;
                    }
                }
                else
                {
                    if (_wrap.Orientation == Orientation.Horizontal)
                    {
                        if (!VerifyHorizontal(ChildLoc, PreviousChild, PreviousChildLoc, _isNewLine))
                        {
                            Helpers.Log("Child " + ChildIndex + " location was not correct.");
                            return false;
                        }
                        else
                        {
                            //Helpers.Log("Child " + ChildIndex + " location was correct.");
                            _totalChildWidth += ((FrameworkElement)child).ActualWidth;
                            if (DoubleUtil.AreClose(_totalChildWidth, _wrap.ActualWidth))
                            {
                                _isNewLine = true;
                            }
                            PreviousChildLoc = ChildLoc;
                            PreviousChild = child;
                        }
                    }
                    else
                    {
                        if (!VerifyVertical(ChildLoc, PreviousChild, PreviousChildLoc, _isNewLine))
                        {
                            Helpers.Log("Child " + ChildIndex + " location was not correct.");
                            return false;
                        }
                        else
                        {
                            //Helpers.Log("Child " + ChildIndex + " location was correct.");
                            _totalChildHeight += ((FrameworkElement)child).ActualHeight;
                            if (DoubleUtil.AreClose(_totalChildHeight, _wrap.ActualHeight))
                            {
                                _isNewLine = true;
                            }

                            PreviousChildLoc = ChildLoc;
                            PreviousChild = child;
                        }
                    }
                }
            }
            return true;
        }

        bool VerifyHorizontal(Point childLoc, UIElement previousChild, Point previousChildLoc, bool isNewLine)
        {
            if (isNewLine)
            {
                _isNewLine = false;
                _totalChildWidth = 0;
                //this wrap offset value will need to change if non-uniform wrapping.
                _wrapOffset += _wrap.ItemHeight;
            }


            if (!DoubleUtil.AreClose(childLoc.X, _totalChildWidth) || !DoubleUtil.AreClose(childLoc.Y, _wrapOffset))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        bool VerifyVertical(Point childLoc, UIElement previousChild, Point previousChildLoc, bool isNewLine)
        {
            if (isNewLine)
            {
                _isNewLine = false;
                _totalChildHeight = 0;
                //this wrap offset value will need to change if non-uniform wrapping.
                _wrapOffset += _wrap.ItemWidth;
            }

            if (!DoubleUtil.AreClose(childLoc.Y, _totalChildHeight) || !DoubleUtil.AreClose(childLoc.X, _wrapOffset))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        string _testtype = "";
        void AddChildren(int childCount, string panelType)
        {
            _testtype = panelType;
            int i;
            string panelTyle = panelType.ToLower();
            switch (panelTyle)
            {
                case "dockpanel":
                    for (i = 0; i < childCount; i++)
                    {
                        DockPanel dock = new DockPanel();
                        dock.Background = Brushes.Firebrick;

                        Rectangle r1 = CommonFunctionality.CreateRectangle(250, 100, Brushes.Gray);
                        Rectangle r2 = CommonFunctionality.CreateRectangle(100, 150, Brushes.HotPink);

                        dock.Children.Add(r1);
                        dock.Children.Add(r2);

                        DockPanel.SetDock(r1, Dock.Top);
                        DockPanel.SetDock(r2, Dock.Right);
                        _wrap.Children.Add(dock);
                    }
                    break;

                case "wrappanel":
                    for (i = 0; i < childCount; i++)
                    {
                        WrapPanel w = new WrapPanel();
                        w.Background = Brushes.GhostWhite;
                        Rectangle w1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Yellow);
                        Rectangle w2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Green);
                        Rectangle w3 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                        w.Children.Add(w1);
                        w.Children.Add(w2);
                        w.Children.Add(w3);
                        _wrap.Children.Add(w);
                    }
                    break;

                case "stackpanel":
                    for (i = 0; i < childCount; i++)
                    {
                        StackPanel stack = new StackPanel();
                        stack.Background = Brushes.GhostWhite;
                        Rectangle r1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Yellow);
                        Rectangle r2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Green);
                        Rectangle r3 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                        stack.Children.Add(r1);
                        stack.Children.Add(r2);
                        stack.Children.Add(r3);
                        _wrap.Children.Add(stack);
                    }
                    break;

                case "grid":
                    for (i = 0; i < childCount; i++)
                    {
                        ColumnDefinition col1, col2;
                        RowDefinition row1, row2;
                        Grid grid = new Grid();
                        grid.Background = Brushes.Crimson;
                        grid.ColumnDefinitions.Add(col1 = new ColumnDefinition());
                        grid.ColumnDefinitions.Add(col2 = new ColumnDefinition());
                        grid.RowDefinitions.Add(row1 = new RowDefinition());
                        grid.RowDefinitions.Add(row2 = new RowDefinition());

                        Rectangle r1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Gray);
                        Rectangle r2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.HotPink);

                        Grid.SetRow(r1, 1);
                        Grid.SetColumn(r1, 1);
                        Grid.SetRow(r2, 0);
                        Grid.SetColumn(r2, 0);

                        grid.Children.Add(r1);
                        grid.Children.Add(r2);

                        _wrap.Children.Add(grid);
                    }
                    break;

                case "canvas":
                    for (i = 0; i < childCount; i++)
                    {
                        Canvas canvas = new Canvas();
                        Rectangle r1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Yellow);
                        Rectangle r2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Green);
                        canvas.Children.Add(r1);
                        canvas.Children.Add(r2);
                        Canvas.SetLeft(r1, 10);
                        Canvas.SetTop(r1, 10);
                        Canvas.SetBottom(r2, 10);
                        Canvas.SetRight(r2, 10);
                        _wrap.Children.Add(canvas);
                    }
                    break;

                case "decorator":
                    for (i = 0; i < childCount; i++)
                    {
                        Decorator dec = new Decorator();

                        Rectangle r = new Rectangle();
                        r.Fill = Brushes.Crimson;
                        dec.Child = r;
                        _wrap.Children.Add(dec);
                    }
                    break;

                case "panel":
                    for (i = 0; i < childCount; i++)
                    {
                        TestPanel panel = new TestPanel();
                        panel.Background = Brushes.YellowGreen;
                        _wrap.Children.Add(panel);
                    }
                    break;

                case "viewbox":
                    for (i = 0; i < childCount; i++)
                    {
                        Viewbox vb = new Viewbox();
                        vb.Stretch = Stretch.Fill;

                        TextBlock txt = new TextBlock();
                        txt.Text = "foo";

                        vb.Child = txt;

                        _wrap.Children.Add(vb);
                    }
                    break;

                case "transform":
                    for (i = 0; i < childCount; i++)
                    {
                        RotateTransform rt = new RotateTransform(360);

                        //TransformDecorator td = new TransformDecorator();
                        Decorator td = new Decorator();
                        td.LayoutTransform = rt;
                        Rectangle r = new Rectangle();
                        r.Fill = Brushes.AntiqueWhite;

                        td.Child = r;
                        _wrap.Children.Add(td);
                    }
                    break;

                case "scrollviewer":
                    for (i = 0; i < childCount; i++)
                    {
                        ScrollViewer sv = new ScrollViewer();
                        sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                        sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                        TextBlock txt = new TextBlock();
                        txt.Text = "some text";
                        txt.FontSize = 100;

                        sv.Content = txt;

                        _wrap.Children.Add(sv);
                    }
                    break;
                default:
                    Helpers.Log("invalid case name...");
                    break;

            }
        }

    }

    //[Test(2, "Panels.WrapPanel", "WrapPanelHorizontalScrolling", Variables="Area=ElementLayout")]
    //public class WrapPanelHorizontalScrolling : CodeTest
    //{
    //    public WrapPanelHorizontalScrolling()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 500;
    //        this.window.Width = 500;
    //        this.window.Top = 50;
    //        this.window.Left = 50;


    //        string xamlfile = "WrapPanelHScroll.xaml";
    //        System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

    //        this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

    //        f.Close();
    //    }

    //    public override FrameworkElement TestContent()
    //    {
    //        return null;
    //    }

    //    ListBox lbx;
    //    bool testStarted = false;
    //    bool loaded = false;

    //    void EnterTest()
    //    {
    //        lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

    //        if (lbx != null)
    //        {
    //            if (lbx.HasItems)
    //            {
    //                if (!testStarted)
    //                {
    //                    loaded = true;
    //                    testStarted = true;
    //                }
    //            }
    //        }
    //        //stack = LogicalTreeHelper.FindLogicalNode(this.window, "stack") as StackPanel;

    //        //if (stack != null)
    //        //{
    //        //    if (!testStarted)
    //        //    {
    //        //        loaded = true;
    //        //        testStarted = true;
    //        //    }
    //        //}
    //    }

    //    ScrollViewer lbxScroll;
    //    StackPanel lbxStack;
    //    int ItemCount;

    //    public override void TestActions()
    //    {
    //        while (!loaded)
    //        {
    //            CommonFunctionality.FlushDispatcher();
    //            EnterTest();
    //        }

    //        //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
    //        ItemCount = lbx.Items.Count;
    //        int count = VisualTreeHelper.GetChildrenCount(lbx);

    //        for (int i = 0; i < count; i++)
    //        {
    //            DependencyObject v = VisualTreeHelper.GetChild(lbx, i);
    //            if (v.GetType().Name == "Border")
    //            {
    //                Border lbxborder = v as Border;
    //                if (lbxborder.Child.GetType().Name == "ScrollViewer")
    //                {
    //                    lbxScroll = lbxborder.Child as ScrollViewer;
    //                }
    //            }
    //        }

    //        if (lbxScroll != null)
    //        {
    //            if (lbxScroll.Content.GetType().Name == "StackPanel")
    //            {
    //                lbxStack = lbxScroll.Content as StackPanel;
    //            }
    //        }

    //        Scroll(lbxStack, "LineRight", 5);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "LineLeft", 209);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "PageRight", 3);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "PageLeft", 7);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "MouseWheelRight", 93);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "MouseWheelLeft", 222);

    //    }

    //    bool tempresult = true;
    //    public override void TestVerify()
    //    {
    //        if (!tempresult)
    //        {
    //            this.Result = false;
    //        }
    //        else
    //        {
    //            this.Result = true;
    //        }
    //    }

    //    void Scroll(StackPanel stack, string ScrollCommand, int ScrollAmount)
    //    {
    //        switch (ScrollCommand)
    //        {
    //            case "LineRight":
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.LineRight();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "LineLeft":
    //                stack.SetHorizontalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.LineLeft();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "PageRight":
    //                stack.SetHorizontalOffset(0);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.PageRight();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "PageLeft":
    //                stack.SetHorizontalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.PageLeft();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "MouseWheelRight":
    //                stack.SetHorizontalOffset(0);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.MouseWheelRight();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "MouseWheelLeft":
    //                stack.SetHorizontalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.MouseWheelLeft();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;
    //        }
    //    }

    //    void Verify(StackPanel stack, string ScrollCommand, int ScrollAmount)
    //    {
    //        switch (ScrollCommand)
    //        {
    //            case "LineRight":
    //                if (stack.HorizontalOffset != ScrollAmount)
    //                {
    //                    Helpers.Log("LINE RIGHT FAIL.. Horizontal Offset does not equal Scroll Amount. \n");
    //                    Helpers.Log("LINE RIGHT FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("LINE RIGHT PASS..");
    //                }
    //                break;

    //            case "LineLeft":
    //                if ((stack.HorizontalOffset + ScrollAmount) != stack.ExtentWidth)
    //                {
    //                    Helpers.Log("LINE LEFT FAIL.. Horizontal Offset + Scroll Amount does not equal Extent Height. \n");
    //                    Helpers.Log("LINE LEFT FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("LINE LEFT PASS..");
    //                }
    //                break;

    //            case "PageRight":
    //                if (stack.HorizontalOffset != (ScrollAmount * stack.ViewportWidth))
    //                {
    //                    Helpers.Log("PAGE RIGHT FAIL.. Horizontal Offset does not equal Scroll Amount * Viewport Height. \n");
    //                    Helpers.Log("PAGE RIGHT FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("PAGE RIGHT PASS..");
    //                }
    //                break;

    //            case "PageLeft":
    //                if ((stack.HorizontalOffset + (ScrollAmount * stack.ViewportWidth)) != stack.ExtentWidth)
    //                {
    //                    Helpers.Log("PAGE LEFT FAIL.. Horizontal Offset + Scroll Amount * Viewport Height does not equal Extent Height. \n");
    //                    Helpers.Log("PAGE LEFT FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("PAGE LEFT PASS..");
    //                }
    //                break;

    //            case "MouseWheelRight":
    //                if (stack.HorizontalOffset != (ScrollAmount * 3))
    //                {
    //                    Helpers.Log("MOUSE WHEEL RIGHT FAIL.. Horizontal Offset does not equal Scroll Amount * 3. \n");
    //                    Helpers.Log("MOUSE WHEEL RIGHT FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("MOUSE WHEEL RIGHT PASS..");
    //                }
    //                break;

    //            case "MouseWheelLeft":
    //                if ((stack.HorizontalOffset + (ScrollAmount * 3)) != stack.ExtentWidth)
    //                {
    //                    Helpers.Log("MOUSE WHEEL LEFT FAIL.. Horizontal Offset + Scroll Amount * 3 does not equal Extent Height. \n");
    //                    Helpers.Log("MOUSE WHEEL LEFT FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("MOUSE WHEEL LEFT PASS..");
    //                }
    //                break;
    //        }
    //    }
    //}

    //[Test(2, "Panels.WrapPanel", "WrapPanelVerticalScrolling", Variables="Area=ElementLayout")]
    //public class WrapPanelVerticalScrolling : CodeTest
    //{
    //    public WrapPanelVerticalScrolling()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 500;
    //        this.window.Width = 500;
    //        this.window.Top = 50;
    //        this.window.Left = 50;


    //        string xamlfile = "WrapPanelVScroll.xaml";
    //        System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

    //        this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

    //        f.Close();
    //    }


    //    public override FrameworkElement TestContent()
    //    {
    //        return null;
    //    }

    //    ListBox lbx;
    //    bool testStarted = false;
    //    bool loaded = false;

    //    void EnterTest()
    //    {
    //        lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

    //        if (lbx != null)
    //        {
    //            if (lbx.HasItems)
    //            {
    //                if (!testStarted)
    //                {
    //                    loaded = true;
    //                    testStarted = true;
    //                }
    //            }
    //        }
    //        //stack = LogicalTreeHelper.FindLogicalNode(this.window, "stack") as StackPanel;

    //        //if (stack != null)
    //        //{
    //        //    if (!testStarted)
    //        //    {
    //        //        loaded = true;
    //        //        testStarted = true;
    //        //    }
    //        //}
    //    }

    //    ScrollViewer lbxScroll;
    //    StackPanel lbxStack;
    //    int ItemCount;

    //    public override void TestActions()
    //    {
    //        while (!loaded)
    //        {
    //            CommonFunctionality.FlushDispatcher();
    //            EnterTest();
    //        }

    //        //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
    //        ItemCount = lbx.Items.Count;
    //        int count = VisualTreeHelper.GetChildrenCount(lbx);

    //        for (int i = 0; i < count; i++)
    //        {
    //            DependencyObject v = VisualTreeHelper.GetChild(lbx, i);
    //            if (v.GetType().Name == "Border")
    //            {
    //                Border lbxborder = v as Border;
    //                if (lbxborder.Child.GetType().Name == "ScrollViewer")
    //                {
    //                    lbxScroll = lbxborder.Child as ScrollViewer;
    //                }
    //            }
    //        }

    //        if (lbxScroll != null)
    //        {
    //            if (lbxScroll.Content.GetType().Name == "StackPanel")
    //            {
    //                lbxStack = lbxScroll.Content as StackPanel;
    //            }
    //        }

    //        Scroll(lbxStack, "LineDown", 333);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "LineUp", 209);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "PageDown", 3);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "PageUp", 7);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "MouseWheelDown", 93);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "MouseWheelUp", 222);

    //    }

    //    bool tempresult = true;
    //    public override void TestVerify()
    //    {
    //        if (!tempresult)
    //        {
    //            this.Result = false;
    //        }
    //        else
    //        {
    //            this.Result = true;
    //        }
    //    }

    //    void Scroll(StackPanel stack, string ScrollCommand, int ScrollAmount)
    //    {
    //        switch (ScrollCommand)
    //        {
    //            case "LineDown":
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.LineDown();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "LineUp":
    //                stack.SetVerticalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.LineUp();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "PageDown":
    //                stack.SetVerticalOffset(0);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.PageDown();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "PageUp":
    //                stack.SetVerticalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.PageUp();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "MouseWheelDown":
    //                stack.SetVerticalOffset(0);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.MouseWheelDown();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "MouseWheelUp":
    //                stack.SetVerticalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.MouseWheelUp();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;
    //        }
    //    }

    //    void Verify(StackPanel stack, string ScrollCommand, int ScrollAmount)
    //    {
    //        switch (ScrollCommand)
    //        {
    //            case "LineDown":
    //                if (stack.VerticalOffset != ScrollAmount)
    //                {
    //                    Helpers.Log("LINE DOWN FAIL.. Vertical Offset does not equal Scroll Amount. \n");
    //                    Helpers.Log("LINE DOWN FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("LINE DOWN PASS..");
    //                }
    //                break;

    //            case "LineUp":
    //                if ((stack.VerticalOffset + ScrollAmount) != stack.ExtentHeight)
    //                {
    //                    Helpers.Log("LINE UP FAIL.. Vertical Offset + Scroll Amount does not equal Extent Height. \n");
    //                    Helpers.Log("LINE UP FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("LINE UP PASS..");
    //                }
    //                break;

    //            case "PageDown":
    //                if (stack.VerticalOffset != (ScrollAmount * stack.ViewportHeight))
    //                {
    //                    Helpers.Log("PAGE DOWN FAIL.. Vertical Offset does not equal Scroll Amount * Viewport Height. \n");
    //                    Helpers.Log("PAGE DOWN FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("PAGE DOWN PASS..");
    //                }
    //                break;

    //            case "PageUp":
    //                if ((stack.VerticalOffset + (ScrollAmount * stack.ViewportHeight)) != stack.ExtentHeight)
    //                {
    //                    Helpers.Log("PAGE UP FAIL.. Vertical Offset + Scroll Amount * Viewport Height does not equal Extent Height. \n");
    //                    Helpers.Log("PAGE UP FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("PAGE UP PASS..");
    //                }
    //                break;

    //            case "MouseWheelDown":
    //                if (stack.VerticalOffset != (ScrollAmount * 3))
    //                {
    //                    Helpers.Log("MOUSE WHEEL DOWN FAIL.. Vertical Offset does not equal Scroll Amount * 3. \n");
    //                    Helpers.Log("MOUSE WHEEL DOWN FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("MOUSE WHEEL DOWN PASS..");
    //                }
    //                break;

    //            case "MouseWheelUp":
    //                if ((stack.VerticalOffset + (ScrollAmount * 3)) != stack.ExtentHeight)
    //                {
    //                    Helpers.Log("MOUSE WHEEL UP FAIL.. Vertical Offset + Scroll Amount * 3 does not equal Extent Height. \n");
    //                    Helpers.Log("MOUSE WHEEL UP FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("MOUSE WHEEL UP PASS..");
    //                }
    //                break;
    //        }
    //    }
    //}

    //[Test(2, "Panels.WrapPanel", "WrapPanelLeftyVerticalScrolling", Variables="Area=ElementLayout")]
    //public class WrapPanelLeftyVerticalScrolling : CodeTest
    //{
    //    public WrapPanelLeftyVerticalScrolling()
    //    { }

    //    public override void WindowSetup()
    //    {
    //        this.window.Height = 500;
    //        this.window.Width = 500;
    //        this.window.Top = 50;
    //        this.window.Left = 50;


    //        string xamlfile = "WrapPanelLeftyVScroll.xaml";
    //        System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

    //        this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

    //        f.Close();
    //    }


    //    public override FrameworkElement TestContent()
    //    {
    //        return null;
    //    }

    //    ListBox lbx;
    //    bool testStarted = false;
    //    bool loaded = false;

    //    void EnterTest()
    //    {
    //        lbx = LogicalTreeHelper.FindLogicalNode(this.window, "MyListBox") as ListBox;

    //        if (lbx != null)
    //        {
    //            if (lbx.HasItems)
    //            {
    //                if (!testStarted)
    //                {
    //                    loaded = true;
    //                    testStarted = true;
    //                }
    //            }
    //        }
    //        //stack = LogicalTreeHelper.FindLogicalNode(this.window, "stack") as StackPanel;

    //        //if (stack != null)
    //        //{
    //        //    if (!testStarted)
    //        //    {
    //        //        loaded = true;
    //        //        testStarted = true;
    //        //    }
    //        //}
    //    }

    //    ScrollViewer lbxScroll;
    //    StackPanel lbxStack;
    //    int ItemCount;

    //    public override void TestActions()
    //    {
    //        while (!loaded)
    //        {
    //            CommonFunctionality.FlushDispatcher();
    //            EnterTest();
    //        }

    //        //lbx = LogicalTreeHelper.FindLogicalNode(w, "MyListBox") as ListBox;
    //        ItemCount = lbx.Items.Count;
    //        int count = VisualTreeHelper.GetChildrenCount(lbx);

    //        for (int i = 0; i < count; i++)
    //        {
    //            DependencyObject v = VisualTreeHelper.GetChild(lbx, i);
    //            if (v.GetType().Name == "Border")
    //            {
    //                Border lbxborder = v as Border;
    //                if (lbxborder.Child.GetType().Name == "ScrollViewer")
    //                {
    //                    lbxScroll = lbxborder.Child as ScrollViewer;
    //                }
    //            }
    //        }

    //        if (lbxScroll != null)
    //        {
    //            if (lbxScroll.Content.GetType().Name == "StackPanel")
    //            {
    //                lbxStack = lbxScroll.Content as StackPanel;
    //            }
    //        }

    //        Scroll(lbxStack, "LineDown", 333);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "LineUp", 209);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "PageDown", 3);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "PageUp", 7);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "MouseWheelDown", 93);
    //        CommonFunctionality.FlushDispatcher();
    //        Scroll(lbxStack, "MouseWheelUp", 222);

    //    }

    //    bool tempresult = true;
    //    public override void TestVerify()
    //    {
    //        if (!tempresult)
    //        {
    //            this.Result = false;
    //        }
    //        else
    //        {
    //            this.Result = true;
    //        }
    //    }

    //    void Scroll(StackPanel stack, string ScrollCommand, int ScrollAmount)
    //    {
    //        switch (ScrollCommand)
    //        {
    //            case "LineDown":
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.LineDown();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "LineUp":
    //                stack.SetVerticalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.LineUp();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "PageDown":
    //                stack.SetVerticalOffset(0);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.PageDown();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "PageUp":
    //                stack.SetVerticalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.PageUp();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "MouseWheelDown":
    //                stack.SetVerticalOffset(0);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.MouseWheelDown();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;

    //            case "MouseWheelUp":
    //                stack.SetVerticalOffset(ItemCount);
    //                for (int i = 0; i < ScrollAmount; i++)
    //                {
    //                    stack.MouseWheelUp();
    //                    CommonFunctionality.FlushDispatcher();
    //                }
    //                Verify(stack, ScrollCommand, ScrollAmount);
    //                break;
    //        }
    //    }

    //    void Verify(StackPanel stack, string ScrollCommand, int ScrollAmount)
    //    {
    //        switch (ScrollCommand)
    //        {
    //            case "LineDown":
    //                if (stack.VerticalOffset != ScrollAmount)
    //                {
    //                    Helpers.Log("LINE DOWN FAIL.. Vertical Offset does not equal Scroll Amount. \n");
    //                    Helpers.Log("LINE DOWN FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("LINE DOWN PASS..");
    //                }
    //                break;

    //            case "LineUp":
    //                if ((stack.VerticalOffset + ScrollAmount) != stack.ExtentHeight)
    //                {
    //                    Helpers.Log("LINE UP FAIL.. Vertical Offset + Scroll Amount does not equal Extent Height. \n");
    //                    Helpers.Log("LINE UP FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("LINE UP PASS..");
    //                }
    //                break;

    //            case "PageDown":
    //                if (stack.VerticalOffset != (ScrollAmount * stack.ViewportHeight))
    //                {
    //                    Helpers.Log("PAGE DOWN FAIL.. Vertical Offset does not equal Scroll Amount * Viewport Height. \n");
    //                    Helpers.Log("PAGE DOWN FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("PAGE DOWN PASS..");
    //                }
    //                break;

    //            case "PageUp":
    //                if ((stack.VerticalOffset + (ScrollAmount * stack.ViewportHeight)) != stack.ExtentHeight)
    //                {
    //                    Helpers.Log("PAGE UP FAIL.. Vertical Offset + Scroll Amount * Viewport Height does not equal Extent Height. \n");
    //                    Helpers.Log("PAGE UP FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("PAGE UP PASS..");
    //                }
    //                break;

    //            case "MouseWheelDown":
    //                if (stack.VerticalOffset != (ScrollAmount * 3))
    //                {
    //                    Helpers.Log("MOUSE WHEEL DOWN FAIL.. Vertical Offset does not equal Scroll Amount * 3. \n");
    //                    Helpers.Log("MOUSE WHEEL DOWN FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("MOUSE WHEEL DOWN PASS..");
    //                }
    //                break;

    //            case "MouseWheelUp":
    //                if ((stack.VerticalOffset + (ScrollAmount * 3)) != stack.ExtentHeight)
    //                {
    //                    Helpers.Log("MOUSE WHEEL UP FAIL.. Vertical Offset + Scroll Amount * 3 does not equal Extent Height. \n");
    //                    Helpers.Log("MOUSE WHEEL UP FAIL..");
    //                    this.Result = false;
    //                }
    //                else
    //                {
    //                    Helpers.Log("MOUSE WHEEL UP PASS..");
    //                }
    //                break;
    //        }
    //    }
    //}


    [Test(2, "Panels.WrapPanel", "WrapPanelVisibility",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class WrapPanelVisibility : VisualScanTest
    {
        public WrapPanelVisibility()
            : base("WrapPanelVisibility.xaml")
        { }
    }
}
