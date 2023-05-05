// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.RenderingVerification;

#endregion

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This will load and run all code BVT's for Negative margins.
    /// 
    /// Possible Tests:
    /// 
    /// - ScrollViewerCase1
    /// - ScrollViewerCase2
    /// - ScrollViewerCase3
    /// - ScrollViewerCase4
    /// - ScrollViewerCase5
    /// - ScrollViewerCase6
    /// - ScrollViewerCase7
    /// 
    //////////////////////////////////////////////////////////////////

    /// <summary>
    /// Testing default value of Vertical/Horizontal ScrollBar's Visibility
    /// with small content.
    /// </summary>
    [Test(0, "Panels.ScrollViewer", "ScrollViewerCase1", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class ScrollViewerCase1 : CodeTest
    {
        public ScrollViewerCase1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("Aero.Theme.xaml"));
            Helpers.AdjustWindowContentSize(this.window);
        }

        Grid _eRoot;
        ScrollViewer _sv;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            WrapPanel wrap = new WrapPanel();
            for (int i = 0; i < 3; i++)
            {
                Rectangle r = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                wrap.Children.Add(r);
            }
            _sv.Content = wrap;
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestVerify()
        {
            Helpers.Log("Doing Vscan compare.");
            VScanCommon vscan = new VScanCommon(this);
            this.Result = vscan.CompareImage();
        }
    }

    /// <summary>
    /// Testing default value of Vertical/Horizontal ScrollBar's Visibility
    /// with large content.
    /// </summary>
    [Test(0, "Panels.ScrollViewer", "ScrollViewerCase2", Variables="Area=ElementLayout")]
    public class ScrollViewerCase2 : CodeTest
    {
        public ScrollViewerCase2()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;
        WrapPanel _wrap;
        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _wrap = new WrapPanel();
            for (int i = 0; i < 3; i++)
            {
                Rectangle r = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                _wrap.Children.Add(r);
            }
            _sv.Content = _wrap;
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestActions()
        {
            _wrap.Width = 500;
            ScrollTestCommon.AddMoreContent(_wrap);
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.Verification(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    /// <summary>
    /// ScrollBar Visibility changed when Content changed.
    /// - Adding Content
    /// </summary>
    [Test(0, "Panels.ScrollViewer", "ScrollViewerCase3", Variables="Area=ElementLayout")]
    public class ScrollViewerCase3 : CodeTest
    {
        public ScrollViewerCase3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;
        WrapPanel _wrap;
        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _wrap = new WrapPanel();
            for (int i = 0; i < 3; i++)
            {
                Rectangle r = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                _wrap.Children.Add(r);
            }
            _sv.Content = _wrap;
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.AddMoreContent((WrapPanel)_sv.Content);
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.Verification(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    /// <summary>
    /// ScrollBar Visibility changed when Content changed.
    /// - Reducing size of Content
    /// </summary>
    [Test(0, "Panels.ScrollViewer", "ScrollViewerCase4", Variables="Area=ElementLayout")]
    public class ScrollViewerCase4 : CodeTest
    {
        public ScrollViewerCase4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;
        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _wrap = new WrapPanel();
            for (int i = 0; i < 3; i++)
            {
                Rectangle r = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                _wrap.Children.Add(r);
            }
            _sv.Content = _wrap;
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestActions()
        {
            ScrollTestCommon.AddMoreContent(_wrap);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.RemoveContent(_wrap);
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.Verification(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    /// <summary>
    /// ScrollViewer Size to Content when
    /// ScrollViewer will wrap it's content as tightly as possible without using scrollbar.
    /// </summary>
    [Test(0, "Panels.ScrollViewer", "ScrollViewerCase5", Variables="Area=ElementLayout")]
    public class ScrollViewerCase5 : CodeTest
    {
        public ScrollViewerCase5()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;
        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _wrap = new WrapPanel();
            for (int i = 0; i < 3; i++)
            {
                Rectangle r = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                _wrap.Children.Add(r);
            }
            _sv.Content = _wrap;
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestActions()
        {
            TestWin.ResizeWindow(this.window, 300);
            CommonFunctionality.FlushDispatcher();
            ScrollTestCommon.AddRectangle(_sv, CommonFunctionality.CreateRectangle(500, 500, new SolidColorBrush(Colors.Lavender)));
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.Verification(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    /// <summary>
    /// ScrollViewer Size to Content when 
    /// 1) MaxSize is set on ScrollViewer and its content's DesiredSize is larger than MaxSize
    /// </summary>
    [Test(0, "Panels.ScrollViewer", "ScrollViewerCase6", Variables="Area=ElementLayout")]
    public class ScrollViewerCase6 : CodeTest
    {
        public ScrollViewerCase6()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;
        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _wrap = new WrapPanel();
            for (int i = 0; i < 3; i++)
            {
                Rectangle r = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                _wrap.Children.Add(r);
            }
            _sv.Content = _wrap;
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestActions()
        {
            _sv.MaxWidth = 300;
            _sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _wrap.Width = 500;
            ScrollTestCommon.AddMoreContent(_wrap);
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.Verification(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    /// <summary>
    /// ScrollViewer Size to Content when
    /// 2) ScrollViewer is constrainted by it's parent such that it cannot grow larger to accommodate the content.
    /// </summary>
    [Test(0, "Panels.ScrollViewer", "ScrollViewerCase7", Variables = "Area=ElementLayout", Keywords = "MicroSuite")]
    public class ScrollViewerCase7 : CodeTest
    {
        public ScrollViewerCase7()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;
        WrapPanel _wrap;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _wrap = new WrapPanel();
            for (int i = 0; i < 3; i++)
            {
                Rectangle r = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                _wrap.Children.Add(r);
            }
            _sv.Content = _wrap;
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestActions()
        {
            ScrollTestCommon.AddMoreContent((WrapPanel)_sv.Content);
            _eRoot.Children.Remove(_sv);
            Canvas c = CommonFunctionality.CreateCanvas(200, 200, new SolidColorBrush(Colors.HotPink));
            _eRoot.Children.Add(c);
            c.Children.Add(_sv);
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.Verification(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    // Test: Extent <= ViewportSize
    // Result: Thum is not shown.
    [Test(0, "Panels.ScrollViewer", "ScrollBarPrimitive1", Variables="Area=ElementLayout")]
    public class ScrollBarPrimitive1 : CodeTest
    {
        public ScrollBarPrimitive1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = _winHeight;
            this.window.Width = _winWidth;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;
        
        int _contentLength = 1;
        double _winHeight = 400;
        double _winWidth = 300;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _sv.HorizontalAlignment = HorizontalAlignment.Left;
            _sv.VerticalAlignment = VerticalAlignment.Top;

            _sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (_contentLength != 0)
            {
                WrapPanel wp = new WrapPanel();

                for (int i = 0; i <= _contentLength; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                    wp.Children.Add(rect);
                }
                _sv.Content = wp;
            }
            else
            {
                Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                _sv.Content = rect;
            }
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.VerifyWithThumb(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    // Test: Extent > ViewportSize
    //	      & ViewportSize/Extent*TrackLength > Min Thumb size
    // Result: Thumb size is larger than the min thumb size.
    [Test(0, "Panels.ScrollViewer", "ScrollBarPrimitive2", Variables="Area=ElementLayout")]
    public class ScrollBarPrimitive2 : CodeTest
    {
        public ScrollBarPrimitive2()
        { }

        public override void WindowSetup()
        {
            this.window.Height = _winHeight;
            this.window.Width = _winWidth;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;

        int _contentLength = 10;
        double _winHeight = 400;
        double _winWidth = 300;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _sv.HorizontalAlignment = HorizontalAlignment.Left;
            _sv.VerticalAlignment = VerticalAlignment.Top;

            _sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (_contentLength != 0)
            {
                WrapPanel wp = new WrapPanel();

                for (int i = 0; i <= _contentLength; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                    wp.Children.Add(rect);
                }
                _sv.Content = wp;
            }
            else
            {
                Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                _sv.Content = rect;
            }
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.VerifyWithThumb(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    // Test: Extent > ViewportSize
    //	      & ViewportSize/Extent*TrackLength <= Min Thumb size
    // Result: Thumb equals to the min thumb size.
    [Test(0, "Panels.ScrollViewer", "ScrollBarPrimitive3", Variables="Area=ElementLayout")]
    public class ScrollBarPrimitive3 : CodeTest
    {
        public ScrollBarPrimitive3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = _winHeight;
            this.window.Width = _winWidth;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;

        int _contentLength = 50;
        double _winHeight = 150;
        double _winWidth = 300;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _sv.HorizontalAlignment = HorizontalAlignment.Left;
            _sv.VerticalAlignment = VerticalAlignment.Top;

            _sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (_contentLength != 0)
            {
                WrapPanel wp = new WrapPanel();

                for (int i = 0; i <= _contentLength; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                    wp.Children.Add(rect);
                }
                _sv.Content = wp;
            }
            else
            {
                Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                _sv.Content = rect;
            }
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.VerifyWithThumb(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    // Test: TrackLength < Min Thumb size
    // Result: Track & Thumb removed.
    [Test(0, "Panels.ScrollViewer", "ScrollBarPrimitive4", Variables="Area=ElementLayout")]
    public class ScrollBarPrimitive4 : CodeTest
    {
        public ScrollBarPrimitive4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = _winHeight;
            this.window.Width = _winWidth;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;

        int _contentLength = 1;
        double _winHeight = 80;
        double _winWidth = 300;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _sv.HorizontalAlignment = HorizontalAlignment.Left;
            _sv.VerticalAlignment = VerticalAlignment.Top;

            _sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (_contentLength != 0)
            {
                WrapPanel wp = new WrapPanel();

                for (int i = 0; i <= _contentLength; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                    wp.Children.Add(rect);
                }
                _sv.Content = wp;
            }
            else
            {
                Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                _sv.Content = rect;
            }
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.VerifyWithThumb(_sv);
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    // Test: ScrollBarSize < (LineUpButtonSize + LineDownButtonSize)
    // Result: LineButton scaled to fit.
    [Test(0, "Panels.ScrollViewer", "ScrollBarPrimitive5", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN", Disabled = true)]
    public class ScrollBarPrimitive5 : CodeTest
    {
        public ScrollBarPrimitive5()
        { }

        public override void WindowSetup()
        {
            this.window.Height = _winHeight;
            this.window.Width = _winWidth;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            Helpers.AdjustWindowContentSize(this.window);
        }

        Grid _eRoot;
        ScrollViewer _sv;

        int _contentLength = 1;
        double _winHeight = 56;
        double _winWidth = 300;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _sv.HorizontalAlignment = HorizontalAlignment.Left;
            _sv.VerticalAlignment = VerticalAlignment.Top;

            _sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (_contentLength != 0)
            {
                WrapPanel wp = new WrapPanel();

                for (int i = 0; i <= _contentLength; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                    wp.Children.Add(rect);
                }
                _sv.Content = wp;
            }
            else
            {
                Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                _sv.Content = rect;
            }
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    // Test: Size to Content with small content Hieght (for VerticalScrollBar)
    // Result: Size of ScrollBar determined by Content size.
    [Test(0, "Panels.ScrollViewer", "ScrollBarPrimitive6", Variables="Area=ElementLayout")]
    public class ScrollBarPrimitive6 : CodeTest
    {
        public ScrollBarPrimitive6()
        { }

        public override void WindowSetup()
        {
            this.window.Height = _winHeight;
            this.window.Width = _winWidth;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;

        int _contentLength = 1;
        double _winHeight = 400;
        double _winWidth = 300;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _sv.HorizontalAlignment = HorizontalAlignment.Left;
            _sv.VerticalAlignment = VerticalAlignment.Top;

            _sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (_contentLength != 0)
            {
                WrapPanel wp = new WrapPanel();

                for (int i = 0; i <= _contentLength; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                    wp.Children.Add(rect);
                }
                _sv.Content = wp;
            }
            else
            {
                Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                _sv.Content = rect;
            }
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.VerifyWithScrollBarSize(_sv, typeof(ScrollBar));
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }

    // Test: Size to Content with small content Width (for HorizontalScrollBar)
    // Result: Size of ScrollBar determined by Content size.
    [Test(0, "Panels.ScrollViewer", "ScrollBarPrimitive7", Variables="Area=ElementLayout")]
    public class ScrollBarPrimitive7 : CodeTest
    {
        public ScrollBarPrimitive7()
        { }

        public override void WindowSetup()
        {
            this.window.Height = _winHeight;
            this.window.Width = _winWidth;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _eRoot;
        ScrollViewer _sv;

        int _contentLength = 0;
        double _winHeight = 400;
        double _winWidth = 300;

        public override FrameworkElement TestContent()
        {
            _eRoot = new Grid();
            _sv = new ScrollViewer();
            _sv.HorizontalAlignment = HorizontalAlignment.Left;
            _sv.VerticalAlignment = VerticalAlignment.Top;

            _sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            if (_contentLength != 0)
            {
                WrapPanel wp = new WrapPanel();

                for (int i = 0; i <= _contentLength; i++)
                {
                    Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                    wp.Children.Add(rect);
                }
                _sv.Content = wp;
            }
            else
            {
                Rectangle rect = CommonFunctionality.CreateRectangle(100, 100, new SolidColorBrush(Colors.CadetBlue));
                _sv.Content = rect;
            }
            _eRoot.Children.Add(_sv);
            return _eRoot;
        }

        public override void TestVerify()
        {
            ScrollTestCommon.LayoutTestResult tr = ScrollTestCommon.VerifyWithScrollBarSize(_sv, typeof(ScrollBar));
            Helpers.Log(tr.message);
            this.Result = tr.result;
        }
    }
}
