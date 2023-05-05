// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

///////////////////////////////////////////////////////////////
///
///   this case is for Physical Scrolling in Scrollviewer.
///   Verifying Physical Scrolling of Panel, Canvas, 
///   StackPanel, Grid, DockPanel, Decorator, Border, 
///   ViewBox, Transform, and ScrollViewer.
///
///////////////////////////////////////////////////////////////
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Shapes;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Scrolling
{
    [Test(1, "Scrolling", "PhysicalScrollingPanel", Variables="Area=ElementLayout")]
    public class PhysicalScrollingPanel : CodeTest
    {
        public PhysicalScrollingPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "Panel";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount,_scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();

            
            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingCanvas", Variables="Area=ElementLayout")]
    public class PhysicalScrollingCanvas : CodeTest
    {
        public PhysicalScrollingCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "Canvas";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        bool _result =true ;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            Helpers.Log(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingStackPanel", Variables="Area=ElementLayout")]
    public class PhysicalScrollingStackPanel : CodeTest
    {
        public PhysicalScrollingStackPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "StackPanel";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingGrid", Variables="Area=ElementLayout")]
    public class PhysicalScrollingGrid : CodeTest
    {
        public PhysicalScrollingGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "Grid";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingDockPanel", Variables="Area=ElementLayout")]
    public class PhysicalScrollingDockPanel : CodeTest
    {
        public PhysicalScrollingDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "DockPanel";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingDecorator", Variables="Area=ElementLayout")]
    public class PhysicalScrollingDecorator : CodeTest
    {
        public PhysicalScrollingDecorator()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "Decorator";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingBorder", Variables="Area=ElementLayout")]
    public class PhysicalScrollingBorder : CodeTest
    {
        public PhysicalScrollingBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "Border";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingViewbox", Variables="Area=ElementLayout")]
    public class PhysicalScrollingViewbox : CodeTest
    {
        public PhysicalScrollingViewbox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "Viewbox";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingTransform", Variables="Area=ElementLayout")]
    public class PhysicalScrollingTransform : CodeTest
    {
        public PhysicalScrollingTransform()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "Transform";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }

    [Test(1, "Scrolling", "PhysicalScrollingWrapPanel", Variables="Area=ElementLayout")]
    public class PhysicalScrollingWrapPanel : CodeTest
    {
        public PhysicalScrollingWrapPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 900;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        string _testElement = "WrapPanel";
        ScrollViewer _scroll;
        int _scrollAmount;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            root.Background = Brushes.LightGoldenrodYellow;

            Border scrollBorder = new Border();
            scrollBorder.HorizontalAlignment = HorizontalAlignment.Center;
            scrollBorder.VerticalAlignment = VerticalAlignment.Center;
            scrollBorder.BorderBrush = Brushes.Navy;
            scrollBorder.BorderThickness = new Thickness(2);

            _scroll = new ScrollViewer();
            _scroll.Height = 300;
            _scroll.Width = 300;
            _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Scroll Right Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Right");

            Helpers.Log("*** Scroll Down Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Down");

            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** Scroll Left Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Left");

            Helpers.Log("*** Scroll Up Test ***");
            _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
            ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
            CommonFunctionality.FlushDispatcher();
            Verification(_scrollAmount, "Up");



            this.window.Content = null;
            _scroll = null;
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ChangeToLeftyScroll(this.window);
            CommonFunctionality.FlushDispatcher();
            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;
            CommonFunctionality.FlushDispatcher();


            if (_scroll != null)
            {
                _scroll.Content = ScrollTestCommon.AddScrollViewerContent(_testElement);
                CommonFunctionality.FlushDispatcher();

                _scroll.ScrollToHorizontalOffset(0);
                _scroll.ScrollToVerticalOffset(0);
                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Right Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollRight(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Right");

                Helpers.Log("*** Scroll Down Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollDown(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Down");

                _scroll.ScrollToHorizontalOffset(600);
                _scroll.ScrollToVerticalOffset(600);

                CommonFunctionality.FlushDispatcher();

                Helpers.Log("*** Scroll Left Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollLeft(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Left");

                Helpers.Log("*** Scroll Up Test ***");
                _scrollAmount = ScrollTestCommon.RandomScrollAmount(_scroll);
                ScrollTestCommon.ScrollUp(_scrollAmount, _scroll);
                CommonFunctionality.FlushDispatcher();
                Verification(_scrollAmount, "Up");
            }
            else
            {
                _result = false;
                _testResults += "Could not change to lefty scroll.";
            }
        }

        private bool _result = true;
        private string _testResults = "\n";

        void Verification(int _AmountScrolled, string _ScrollDirection)
        {
            CommonFunctionality.FlushDispatcher();

            string teststatus;

            switch (_ScrollDirection)
            {
                case "Right":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Right, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Down":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_AmountScrolled * 16))
                    {
                        teststatus = "After Scrolling Down, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }

                    break;

                case "Left":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("HorizontalOffset : " + _scroll.HorizontalOffset);

                    if (_scroll.HorizontalOffset != (_scroll.ScrollableWidth - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Left, the ScrollViewer HorizontalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;

                case "Up":
                    Helpers.Log("Amount Scrolled : " + _AmountScrolled);
                    Helpers.Log("VerticalOffset   : " + _scroll.VerticalOffset);

                    if (_scroll.VerticalOffset != (_scroll.ScrollableHeight - (_AmountScrolled * 16)))
                    {
                        teststatus = "After Scrolling Up, the ScrollViewer VerticalOffset is incorrect.\n";
                        Helpers.Log(teststatus);
                        _testResults += teststatus;
                        _result = false;
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            TestLog.Current.LogStatus(_testResults);
            this.Result = _result;
        }
    }
}
