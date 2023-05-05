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

namespace ElementLayout.FeatureTests.Property
{
    //////////////////////////////////////////////////////////////////
    /// This Zero Transform Tests on panels.
    /// 
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(3, "Property.Transform", "ZeroTransformTest1", Variables="Area=ElementLayout")]
    public class ZeroTransformTest1 : CodeTest
    {
        public ZeroTransformTest1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "Panel";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }

    [Test(3, "Property.Transform", "ZeroTransformTest2", Variables="Area=ElementLayout")]
    public class ZeroTransformTest2 : CodeTest
    {
        public ZeroTransformTest2()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "Canvas";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }

    [Test(3, "Property.Transform", "ZeroTransformTest3", Variables="Area=ElementLayout")]
    public class ZeroTransformTest3 : CodeTest
    {
        public ZeroTransformTest3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "StackPanel";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }

    [Test(3, "Property.Transform", "ZeroTransformTest4", Variables="Area=ElementLayout")]
    public class ZeroTransformTest4 : CodeTest
    {
        public ZeroTransformTest4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "Grid";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }

    [Test(3, "Property.Transform", "ZeroTransformTest5", Variables="Area=ElementLayout")]
    public class ZeroTransformTest5 : CodeTest
    {
        public ZeroTransformTest5()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "DockPanel";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }

    [Test(3, "Property.Transform", "ZeroTransformTest6", Variables="Area=ElementLayout")]
    public class ZeroTransformTest6 : CodeTest
    {
        public ZeroTransformTest6()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "Decorator";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }

    [Test(3, "Property.Transform", "ZeroTransformTest7", Variables="Area=ElementLayout")]
    public class ZeroTransformTest7 : CodeTest
    {
        public ZeroTransformTest7()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "Border";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }

    [Test(3, "Property.Transform", "ZeroTransformTest8", Variables="Area=ElementLayout")]
    public class ZeroTransformTest8 : CodeTest
    {
        public ZeroTransformTest8()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "Viewbox";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }

    [Test(3, "Property.Transform", "ZeroTransformTest9", Variables="Area=ElementLayout")]
    public class ZeroTransformTest9 : CodeTest
    {
        public ZeroTransformTest9()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        static string s_testElement = "ScrollViewer";

        Grid _root;
        FrameworkElement _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGoldenrodYellow;

            _child = AddTestContent(s_testElement);
            _root.Children.Add(_child);

            return _root;
        }

        bool _tempresult = true;
        string _failingTest = "";
        Rect _noTransformRect;
        Point _preTransformPoint;

        public override void TestActions()
        {
            _preTransformPoint = LayoutUtility.GetElementPosition(_child, _root);
            _noTransformRect = new Rect(_preTransformPoint, _child.RenderSize);
            CommonFunctionality.FlushDispatcher();

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Rotate"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Skew"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);

            _child.SetValue(FrameworkElement.LayoutTransformProperty, TestTransform("Scale"));
            CommonFunctionality.FlushDispatcher();
            verify(_child);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        void verify(FrameworkElement TestElement)
        {
            Rect teRect = VisualTreeHelper.GetDescendantBounds(_root);

            Rect newRect = new Rect(LayoutUtility.GetElementPosition(_child, _root), _child.RenderSize);

            if (newRect != _noTransformRect)
            {
                Helpers.Log("Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")");
                _failingTest += "Zero Transform Test Failed.(" + _child.LayoutTransform.GetType().Name + ")";
                _tempresult = false;
            }
            else
            {
                Helpers.Log("Zero Transform Test Passed.(" + _child.LayoutTransform.GetType().Name + ")");
            }
        }

        Transform TestTransform(string _transform)
        {
            Transform transform = null;
            switch (_transform)
            {
                case "Rotate":
                    RotateTransform rt = new RotateTransform();
                    rt.Angle = 0;
                    transform = rt;
                    break;
                case "Skew":
                    SkewTransform st = new SkewTransform();
                    st.AngleX = 0;
                    st.AngleY = 0;
                    transform = st;
                    break;
                case "Scale":
                    ScaleTransform sct = new ScaleTransform();
                    sct.ScaleX = 1;
                    sct.ScaleY = 1;
                    transform = sct;
                    break;
            }

            return transform;
        }

        FrameworkElement AddTestContent(string _TestElement)
        {
            switch (_TestElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    panel.Height = 200;
                    panel.Width = 250;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    canvas.Height = 222;
                    canvas.Width = 111;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    stack.Height = 150;
                    stack.Width = 300;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    grid.Height = 350;
                    grid.Width = 350;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    dock.Height = 333;
                    dock.Width = 333;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    decorator.Height = 250;
                    decorator.Width = 250;
                    Border dChild = new Border();
                    dChild.Height = 500;
                    dChild.Width = 500;
                    dChild.Background = Brushes.SpringGreen;
                    decorator.Child = dChild;
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    border.Height = 201;
                    border.Width = 259;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    viewbox.Height = 500;
                    viewbox.Width = 449;
                    Border vChild = new Border();
                    vChild.Height = 500;
                    vChild.Width = 449;
                    vChild.Background = Brushes.SpringGreen;
                    viewbox.Child = vChild;
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    scrollviewer.Height = 340;
                    scrollviewer.Width = 320;
                    Border svChild = new Border();
                    svChild.Height = 500;
                    svChild.Width = 449;
                    svChild.Background = Brushes.SpringGreen;
                    scrollviewer.Content = svChild;
                    _child = scrollviewer;
                    break;
            }
            return _child;
        }
    }
}
