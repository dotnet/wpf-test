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
using System.Windows.Documents;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Layout.PropertyDump;
using System.IO;

namespace ElementLayout.FeatureTests.Scrolling
{
    ///////////////////////////////////////////////////////////////
    ///
    ///   this case is for Physical Scrolling in Scrollviewer.
    ///   Verifying Physical Scrolling of Panel, Canvas, 
    ///   StackPanel, Grid, DockPanel, Decorator, Border, 
    ///   ViewBox, Transform, and ScrollViewer.
    ///
    ///////////////////////////////////////////////////////////////

    /// PANEL CONTENT ////////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan01", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan01 : CodeTest
    {
        public PhysicalScrollVscan01()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Panel";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan02", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan02 : CodeTest
    {
        public PhysicalScrollVscan02()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Panel";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan03", 
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan03 : CodeTest
    {
        public PhysicalScrollVscan03()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            
            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);
            
            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Panel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan04", 
        Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan04 : CodeTest
    {
        public PhysicalScrollVscan04()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        
        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Panel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// CANVAS CONTENT ////////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan05", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan05 : CodeTest
    {
        public PhysicalScrollVscan05()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;
        
        static string s_testElement = "Canvas";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan06", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan06 : CodeTest
    {
        public PhysicalScrollVscan06()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Canvas";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan07",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan07 : CodeTest
    {
        public PhysicalScrollVscan07()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            } 
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Canvas";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan08",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan08 : CodeTest
    {
        public PhysicalScrollVscan08()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Canvas";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// STACKPANEL CONTENT ////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan09", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan09 : CodeTest
    {
        public PhysicalScrollVscan09()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "StackPanel";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan10", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan10 : CodeTest
    {
        public PhysicalScrollVscan10()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "StackPanel";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {           
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan11",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan11 : CodeTest
    {
        public PhysicalScrollVscan11()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "StackPanel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan12",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan12 : CodeTest
    {
        public PhysicalScrollVscan12()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "StackPanel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// GRID CONTENT ////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan13", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan13 : CodeTest
    {
        public PhysicalScrollVscan13()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }


        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Grid";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan14", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan14 : CodeTest
    {
        public PhysicalScrollVscan14()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Grid";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }
     
        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan15",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan15 : CodeTest
    {
        public PhysicalScrollVscan15()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Grid";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan16",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan16 : CodeTest
    {
        public PhysicalScrollVscan16()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Grid";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// DOCKPANEL CONTENT ////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan17", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan17 : CodeTest
    {
        public PhysicalScrollVscan17()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "DockPanel";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }
       
        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan18", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan18 : CodeTest
    {
        public PhysicalScrollVscan18()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "DockPanel";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan19",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan19 : CodeTest
    {
        public PhysicalScrollVscan19()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "DockPanel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan20",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan20 : CodeTest
    {
        public PhysicalScrollVscan20()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "DockPanel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// DECORATOR CONTENT ////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan21", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan21 : CodeTest
    {
        public PhysicalScrollVscan21()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Decorator";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan22", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan22 : CodeTest
    {
        public PhysicalScrollVscan22()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Decorator";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan23",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan23 : CodeTest
    {
        public PhysicalScrollVscan23()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Decorator";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan24",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan24 : CodeTest
    {
        public PhysicalScrollVscan24()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "DockPanel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// BORDER CONTENT ////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan25", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan25 : CodeTest
    {
        public PhysicalScrollVscan25()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Border";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan26", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan26 : CodeTest
    {
        public PhysicalScrollVscan26()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Border";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan27",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan27 : CodeTest
    {
        public PhysicalScrollVscan27()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Border";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan28",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan28 : CodeTest
    {
        public PhysicalScrollVscan28()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Border";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// VIEWBOX CONTENT ////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan29", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan29 : CodeTest
    {
        public PhysicalScrollVscan29()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Viewbox";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan30", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan30 : CodeTest
    {
        public PhysicalScrollVscan30()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Viewbox";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan31",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan31 : CodeTest
    {
        public PhysicalScrollVscan31()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Viewbox";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan32",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan32 : CodeTest
    {
        public PhysicalScrollVscan32()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Viewbox";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// TRANSFORM CONTENT ////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan33", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan33 : CodeTest
    {
        public PhysicalScrollVscan33()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Transform";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan34", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan34 : CodeTest
    {
        public PhysicalScrollVscan34()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Transform";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan35",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan35 : CodeTest
    {
        public PhysicalScrollVscan35()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "Transform";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan36",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan36 : CodeTest
    {
        public PhysicalScrollVscan36()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "Transform";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    /// WRAPPANEL CONTENT ////////////////////////////////////////////////////////

    [Test(3, "Scrolling", "PhysicalScrollVscan37", Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan37 : CodeTest
    {
        public PhysicalScrollVscan37()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "WrapPanel";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan38",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan38 : CodeTest
    {
        public PhysicalScrollVscan38()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "WrapPanel";

        ScrollViewer _scroll;

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

            ScrollTestCommon.AddScrollViewerContent(s_testElement, _scroll);

            scrollBorder.Child = _scroll;
            root.Children.Add(scrollBorder);

            return root;
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan39",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan39 : CodeTest
    {
        public PhysicalScrollVscan39()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }
            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountLeft = 13;
        static int s_scrollAmountUp = 7;

        static string s_testElement = "WrapPanel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            _scroll.ScrollToHorizontalOffset(600);
            _scroll.ScrollToVerticalOffset(600);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.ScrollUp(s_scrollAmountUp, _scroll);
            ScrollTestCommon.ScrollLeft(s_scrollAmountLeft, _scroll);
        }

        public override void TestVerify()
        {           
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }

    [Test(3, "Scrolling", "PhysicalScrollVscan40",
       Variables = "Area=ElementLayout")]
    public class PhysicalScrollVscan40 : CodeTest
    {
        public PhysicalScrollVscan40()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Top = 0;
            this.window.Left = 0;

            string xamlfile = "leftyscroll.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();

            CommonFunctionality.FlushDispatcher();

            _scroll = LogicalTreeHelper.FindLogicalNode(this.window, "lefty_scroll") as ScrollViewer;

            if (_scroll != null)
            {
                _scroll.Height = 300;
                _scroll.Width = 300;
                _scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                _scroll.Content = this.TestContent();
            }

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericScrollViewer.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        static int s_scrollAmountRight = 9;
        static int s_scrollAmountDown = 3;

        static string s_testElement = "WrapPanel";

        ScrollViewer _scroll;

        public override FrameworkElement TestContent()
        {
            return ScrollTestCommon.AddScrollViewerContent(s_testElement);
        }

        public override void TestActions()
        {
            ScrollTestCommon.ScrollDown(s_scrollAmountDown, _scroll);
            ScrollTestCommon.ScrollRight(s_scrollAmountRight, _scroll);
        }

        public override void TestVerify()
        {
            
            
            PropertyDumpHelper helper = new PropertyDumpHelper(this.window.Content as Visual);
            this.Result = helper.CompareLogShow(new Arguments(this));
        }
    }
}
