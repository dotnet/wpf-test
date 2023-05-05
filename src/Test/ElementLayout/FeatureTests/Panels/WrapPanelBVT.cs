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

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This contains all WrapPanel code BVT's.
    /// 
    /// Possible Tests:
    /// 
    /// - ItemSizeChanged
    /// - PropertyTest
    /// - UniformWrap
    /// - MaxHeightWidth
    /// - MinHeightWidth
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(0, "Panels.WrapPanel", "WrapPanelItemSizeChange", Variables="Area=ElementLayout")]
    public class WrapPanelItemSizeChange : CodeTest
    {
        public WrapPanelItemSizeChange()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.WindowState = WindowState.Maximized;
            this.window.Content = this.TestContent();
        }

        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _wrap = new WrapPanel();
            _wrap.HorizontalAlignment = HorizontalAlignment.Left;
            _wrap.VerticalAlignment = VerticalAlignment.Top;
            _wrap.Background = Brushes.CornflowerBlue;
            _wrap.ItemWidth = 100;
            _wrap.ItemHeight = 100;

            AddChildren(153);

            return _wrap;
        }

        public override void TestActions()
        {
            for (int i = 0; i < 25; i++)
            {
                ResizeItems((_wrap.ItemWidth + 50), (_wrap.ItemHeight + 50));
                if (!VerifyWrap())
                {
                    _tempresult = false;
                    Helpers.Log("FAIL : Item Size " + _wrap.ItemWidth + ", " + _wrap.ItemHeight + ".");
                }
            }

            for (int i = 0; i < 25; i++)
            {
                ResizeItems((_wrap.ItemWidth - 50), (_wrap.ItemHeight - 50));
                if (!VerifyWrap())
                {
                    _tempresult = false;
                    Helpers.Log("FAIL : Item Size " + _wrap.ItemWidth + ", " + _wrap.ItemHeight + ".");
                }
            }

            CommonFunctionality.FlushDispatcher();
            _wrap.Orientation = Orientation.Vertical;
            CommonFunctionality.FlushDispatcher();

            for (int i = 0; i < 25; i++)
            {
                ResizeItems((_wrap.ItemWidth + 50), (_wrap.ItemHeight + 50));
                if (!VerifyWrap())
                {
                    _tempresult = false;
                    Helpers.Log("FAIL : Item Size " + _wrap.ItemWidth + ", " + _wrap.ItemHeight + ".");
                }
            }

            for (int i = 0; i < 25; i++)
            {
                ResizeItems((_wrap.ItemWidth - 50), (_wrap.ItemHeight - 50));
                if (!VerifyWrap())
                {
                    _tempresult = false;
                    Helpers.Log("FAIL : Item Size " + _wrap.ItemWidth + ", " + _wrap.ItemHeight + ".");
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
            this.window.WindowState = WindowState.Normal;
        }

        void ResizeItems(double newWidth, double newHeight)
        {
            _wrap.ItemWidth = newWidth;
            _wrap.ItemHeight = newHeight;
            CommonFunctionality.FlushDispatcher();
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
                b.Background = Brushes.DarkOrange;
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

    [Test(0, "Panels.WrapPanel", "WrapPanelPropertyTest", Variables="Area=ElementLayout")]
    public class WrapPanelPropertyTest : CodeTest
    {
        public WrapPanelPropertyTest()
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

            return _wrap;
        }

        public override void TestActions()
        {
            try
            {
                _wrap.SetValue(WrapPanel.OrientationProperty, Orientation.Horizontal);
            }
            catch (Exception e)
            {
                _tempresult = false;
                Helpers.Log("exception thrown with valid input.");
                Helpers.Log(e.Message);
            }

            try
            {
                _wrap.SetValue(WrapPanel.OrientationProperty, Orientation.Vertical);
            }
            catch (Exception e)
            {
                _tempresult = false;
                Helpers.Log("exception thrown with valid input.");
                Helpers.Log(e.Message);
            }

            try
            {
                _wrap.SetValue(WrapPanel.OrientationProperty, Dock.Right);
            }
            catch (Exception e)
            {
                _tempresult = true;
                Helpers.Log("exception thrown with in-valid input.");
                Helpers.Log(e.Message);
            }

            try
            {
                _wrap.SetValue(WrapPanel.ItemHeightProperty, 101d);
            }
            catch (Exception e)
            {
                _tempresult = false;
                Helpers.Log("exception thrown with valid input.");
                Helpers.Log(e.Message);
            }

            try
            {

                _wrap.SetValue(WrapPanel.ItemHeightProperty, -101d);
            }
            catch (Exception e)
            {
                _tempresult = true;
                Helpers.Log("exception thrown with in-valid input.");
                Helpers.Log(e.Message);
            }

            try
            {
                _wrap.SetValue(WrapPanel.ItemWidthProperty, 101d);
            }
            catch (Exception e)
            {
                _tempresult = false;
                Helpers.Log("exception thrown with valid input.");
                Helpers.Log(e.Message);
            }

            try
            {
                _wrap.SetValue(WrapPanel.ItemWidthProperty, -101d);
            }
            catch (Exception e)
            {
                _tempresult = true;
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

    [Test(0, "Panels.WrapPanel", "WrapPanelUniformWrap", Variables="Area=ElementLayout")]
    public class WrapPanelUniformWrap : CodeTest
    {
        public WrapPanelUniformWrap()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 1600;
            this.window.Width = 1600;
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
                Helpers.Log("Orientation.Horizontal test failed.");
            }
            else
            {
                Helpers.Log("Orientation.Horizontal Wrap Pass.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Vertical;
            _wrap.Width = double.NaN;
            _wrap.Height = 300;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("Orientation.Vertical test failed.");
            }
            else
            {
                Helpers.Log("Orientation.Vertical Wrap Pass.");
            }

            CommonFunctionality.FlushDispatcher();
            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(81);
            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Horizontal;
            _wrap.Width = 900;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("Orientation.Horizontal test failed.");
            }
            else
            {
                Helpers.Log("Orientation.Horizontal Wrap Pass.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Vertical;
            _wrap.Width = double.NaN;
            _wrap.Height = 900;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("Orientation.Vertical test failed.");
            }
            else
            {
                Helpers.Log("Orientation.Vertical Wrap Pass.");
            }

            CommonFunctionality.FlushDispatcher();
            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(225);
            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Horizontal;
            _wrap.Width = 1500;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("Orientation.Horizontal test failed.");
            }
            else
            {
                Helpers.Log("Orientation.Horizontal Wrap Pass.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Vertical;
            _wrap.Width = double.NaN;
            _wrap.Height = 1500;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log("Orientation.Vertical test failed.");
            }
            else
            {
                Helpers.Log("Orientation.Vertical Wrap Pass.");
            }

            CommonFunctionality.FlushDispatcher();
            _wrap.Children.Clear();
            CommonFunctionality.FlushDispatcher();
            AddChildren(1000);
            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Horizontal;
            _wrap.Width = 1500;
            _wrap.Height = double.NaN;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log(" Orientation.Horizontal test failed. ");
            }
            else
            {
                Helpers.Log("Orientation.Horizontal Wrap Pass.");
            }

            CommonFunctionality.FlushDispatcher();

            _wrap.Orientation = Orientation.Vertical;
            _wrap.Width = double.NaN;
            _wrap.Height = 1500;
            CommonFunctionality.FlushDispatcher();
            if (!VerifyWrap())
            {
                _tempresult = false;
                Helpers.Log(" Orientation.Vertical test failed. ");
            }
            else
            {
                Helpers.Log("Orientation.Vertical Wrap Pass.");
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
            Helpers.Log("Testing WrapPanel with Orientation " + _wrap.Orientation);
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

    [Test(0, "Panels.WrapPanel", "WrapPanelMaxHeightWidth", Variables="Area=ElementLayout")]
    public class WrapPanelMaxHeightWidth : CodeTest
    {
        public WrapPanelMaxHeightWidth()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        WrapPanel _wrap;
        Grid _root;
        Border _wrapcontent;

        double _maxValue = 350;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _wrap = new WrapPanel();
            _wrap.Height = 100;
            _wrap.Width = 100;

            _wrapcontent = new Border();
            _wrapcontent.Background = Brushes.YellowGreen;
            _wrapcontent.Height = 100;
            _wrapcontent.Width = 100;

            _wrap.Children.Add(_wrapcontent);

            _root.Children.Add(_wrap);

            return _root;
        }

        public override void TestActions()
        {
            _wrap.Height = 1000;
            _wrap.Width = 1000;
            _wrap.MaxHeight = _maxValue;
            _wrap.MaxWidth = _maxValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size WrapPanelSize = _wrap.RenderSize;

            if (WrapPanelSize.Height != _maxValue || WrapPanelSize.Width != _maxValue)
            {
                Helpers.Log("WrapPanel Max Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("WrapPanel Max Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.WrapPanel", "WrapPanelMinHeightWidth", Variables="Area=ElementLayout")]
    public class WrapPanelMinHeightWidth : CodeTest
    {
        public WrapPanelMinHeightWidth()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        WrapPanel _wrap;
        Grid _root;
        Border _wrapcontent;

        double _minValue = 200;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _wrap = new WrapPanel();
            _wrap.Height = 100;
            _wrap.Width = 100;

            _wrapcontent = new Border();
            _wrapcontent.Background = Brushes.YellowGreen;
            _wrapcontent.Height = 100;
            _wrapcontent.Width = 100;

            _wrap.Children.Add(_wrapcontent);

            _root.Children.Add(_wrap);

            return _root;
        }

        public override void TestActions()
        {
            _wrap.Height = 10;
            _wrap.Width = 10;
            _wrap.MinHeight = _minValue;
            _wrap.MinWidth = _minValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size WrapPanelSize = _wrap.RenderSize;

            if (WrapPanelSize.Height != _minValue || WrapPanelSize.Width != _minValue)
            {
                Helpers.Log("WrapPanel Min Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("WrapPanel Min Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }
}
