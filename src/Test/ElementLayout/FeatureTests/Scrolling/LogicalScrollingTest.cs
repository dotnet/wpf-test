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

namespace ElementLayout.FeatureTests.Scrolling
{
    //////////////////////////////////////////////////////////////////
    /// This Logical Scrolling Priority Test cases.
    /// 
    //////////////////////////////////////////////////////////////////
    
    [Test(1, "Scrolling", "LogicalScrolling1", Variables="Area=ElementLayout")]
    public class LogicalScrolling1 : CodeTest
    {
        public LogicalScrolling1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            ((Border)_stackpanel.Children[3]).BringIntoView();
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.LineUp();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling2", Variables="Area=ElementLayout")]
    public class LogicalScrolling2 : CodeTest
    {
        public LogicalScrolling2()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _scrollviewer.LineDown();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling3", Variables="Area=ElementLayout")]
    public class LogicalScrolling3 : CodeTest
    {
        public LogicalScrolling3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _stackpanel.Orientation = Orientation.Horizontal;
            _stackpanel.UpdateLayout();
            ((Border)_stackpanel.Children[3]).BringIntoView();
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.LineLeft();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling4", Variables="Area=ElementLayout")]
    public class LogicalScrolling4 : CodeTest
    {
        public LogicalScrolling4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _stackpanel.Orientation = Orientation.Horizontal;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.LineRight();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling5", Variables="Area=ElementLayout")]
    public class LogicalScrolling5 : CodeTest
    {
        public LogicalScrolling5()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            ((Border)_stackpanel.Children[4]).BringIntoView();
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.PageUp();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling6", Variables="Area=ElementLayout")]
    public class LogicalScrolling6 : CodeTest
    {
        public LogicalScrolling6()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _childIndex = 2;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.PageDown();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling7", Variables="Area=ElementLayout")]
    public class LogicalScrolling7 : CodeTest
    {
        public LogicalScrolling7()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _stackpanel.Orientation = Orientation.Horizontal;
            _stackpanel.UpdateLayout();
            ((Border)_stackpanel.Children[4]).BringIntoView();
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.PageLeft();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling8", Variables="Area=ElementLayout")]
    public class LogicalScrolling8 : CodeTest
    {
        public LogicalScrolling8()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _stackpanel.Orientation = Orientation.Horizontal;
            _childIndex = 2;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.PageRight();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling9", Variables="Area=ElementLayout")]
    public class LogicalScrolling9 : CodeTest
    {
        public LogicalScrolling9()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _childIndex = 4;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.ScrollToBottom();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling10", Variables="Area=ElementLayout")]
    public class LogicalScrolling10 : CodeTest
    {
        public LogicalScrolling10()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _grid.Width = 150;
            _childIndex = 4;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.ScrollToEnd();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling11", Variables="Area=ElementLayout")]
    public class LogicalScrolling11 : CodeTest
    {
        public LogicalScrolling11()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _grid.Width = 150;
            _stackpanel.UpdateLayout();
            _scrollviewer.ScrollToHorizontalOffset(_scrollviewer.ScrollableWidth);
            _scrollviewer.ScrollToVerticalOffset(_scrollviewer.ScrollableHeight);
            _childIndex = 0;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.ScrollToHome();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling12", Variables="Area=ElementLayout")]
    public class LogicalScrolling12 : CodeTest
    {
        public LogicalScrolling12()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _stackpanel.Orientation = Orientation.Horizontal;
            _stackpanel.UpdateLayout();
            ((Border)_stackpanel.Children[4]).BringIntoView();
            _childIndex = 0;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.ScrollToLeftEnd();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling13", Variables="Area=ElementLayout")]
    public class LogicalScrolling13 : CodeTest
    {
        public LogicalScrolling13()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            _stackpanel.Orientation = Orientation.Horizontal;
            _childIndex = 4;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.ScrollToRightEnd();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }

    [Test(1, "Scrolling", "LogicalScrolling14", Variables="Area=ElementLayout")]
    public class LogicalScrolling14 : CodeTest
    {
        public LogicalScrolling14()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ScrollViewer _scrollviewer;
        StackPanel _stackpanel;

        string _resultMsg = null;
        int _childIndex = 1;

        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(1, 1);
            _grid.Width = 450;
            _grid.Height = 450;
            _scrollviewer = new ScrollViewer();
            _stackpanel = new StackPanel();
            _stackpanel.HorizontalAlignment = HorizontalAlignment.Left;
            _stackpanel.VerticalAlignment = VerticalAlignment.Top;

            _scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _scrollviewer.CanContentScroll = true;
            byte color = 0;
            for (int c = 0; c < 6; c++)
            {
                Border border = CommonFunctionality.CreateBorder(new SolidColorBrush(Color.FromRgb(color, 100, color)), 200, 200);
                _stackpanel.Children.Add(border);
                border.Name = "Children" + c.ToString();
                color += 50;

                TextBlock text = new TextBlock();
                text.Text = "Children of StackPanel " + c.ToString();
                border.Child = text;
            }
            _scrollviewer.Content = _stackpanel;
            _grid.Children.Add(_scrollviewer);
            return _grid;
        }

        public override void TestActions()
        {
            ((Border)_stackpanel.Children[3]).BringIntoView();
            _childIndex = 0;
            CommonFunctionality.FlushDispatcher();
            _scrollviewer.ScrollToTop();
        }

        public override void TestVerify()
        {
            Point expectedPosition = new Point(0, 0);
            Point actualPosition;
            if (_tempresult)
            {
                actualPosition = LayoutUtility.GetElementPosition(_stackpanel.Children[_childIndex], _scrollviewer);
                _tempresult = Equals(expectedPosition, actualPosition);
                _resultMsg = "Index of Children: " + _childIndex.ToString()
                    + "\n\tExpected Position (x,y) = (" + expectedPosition.X + "," + expectedPosition.Y
                    + ") \n\tAcutal Position (x,y) = (" + actualPosition.X + "," + actualPosition.Y + ")";
                if (_tempresult)
                {
                    if (_stackpanel.Orientation == Orientation.Vertical)
                        _tempresult = Equals(_scrollviewer.VerticalOffset, (double)_childIndex);
                    else if (_stackpanel.Orientation == Orientation.Horizontal)
                        _tempresult = Equals(_scrollviewer.HorizontalOffset, (double)_childIndex);
                    if (!_tempresult)
                        _resultMsg += "\nOffset does not matched with Index of Chidren";
                }
            }
            Helpers.Log(_resultMsg);
            this.Result = _tempresult;
        }
        bool _tempresult = true;
    }
}
