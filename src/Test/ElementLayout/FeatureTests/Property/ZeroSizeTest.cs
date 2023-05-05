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
    /// This Zero Size Tests on panels.
    /// 
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(3, "Property.HeightWidth", "ZeroSizeTest1", Variables="Area=ElementLayout")]
    public class ZeroSizeTest1 : CodeTest
    {
        public ZeroSizeTest1()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "Panel";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }

    [Test(3, "Property.HeightWidth", "ZeroSizeTest2", Variables="Area=ElementLayout")]
    public class ZeroSizeTest2 : CodeTest
    {
        public ZeroSizeTest2()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "Canvas";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }

    [Test(3, "Property.HeightWidth", "ZeroSizeTest3", Variables="Area=ElementLayout")]
    public class ZeroSizeTest3 : CodeTest
    {
        public ZeroSizeTest3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "StackPanel";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }

    [Test(3, "Property.HeightWidth", "ZeroSizeTest4", Variables="Area=ElementLayout")]
    public class ZeroSizeTest4 : CodeTest
    {
        public ZeroSizeTest4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "Grid";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }

    [Test(3, "Property.HeightWidth", "ZeroSizeTest5", Variables="Area=ElementLayout")]
    public class ZeroSizeTest5 : CodeTest
    {
        public ZeroSizeTest5()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "DockPanel";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }

    [Test(3, "Property.HeightWidth", "ZeroSizeTest6", Variables="Area=ElementLayout")]
    public class ZeroSizeTest6 : CodeTest
    {
        public ZeroSizeTest6()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "Decorator";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }

    [Test(3, "Property.HeightWidth", "ZeroSizeTest7", Variables="Area=ElementLayout")]
    public class ZeroSizeTest7 : CodeTest
    {
        public ZeroSizeTest7()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "Border";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }

    [Test(3, "Property.HeightWidth", "ZeroSizeTest8", Variables="Area=ElementLayout")]
    public class ZeroSizeTest8 : CodeTest
    {
        public ZeroSizeTest8()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "Viewbox";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }

    [Test(3, "Property.HeightWidth", "ZeroSizeTest9", Variables="Area=ElementLayout")]
    public class ZeroSizeTest9 : CodeTest
    {
        public ZeroSizeTest9()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 400;
            this.window.Width = 400;
            this.window.Top = 10;
            this.window.Left = 10;

            this.window.Content = this.TestContent();
        }

        static string s_baseElement = "ScrollViewer";

        Grid _root;
        FrameworkElement _baseElement;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.AliceBlue;

            _baseElement = NewElement(s_baseElement);
            _baseElement.HorizontalAlignment = HorizontalAlignment.Center;
            _baseElement.VerticalAlignment = VerticalAlignment.Center;


            _root.Children.Add(_baseElement);
            return _root;
        }

        string[] _testElements = { "Panel", "Canvas", "StackPanel", "Grid", "DockPanel", "Decorator", "Border", "Viewbox", "ScrollViewer" };

        public override void TestActions()
        {
            AddElements(_baseElement);
        }

        public override void TestVerify()
        {
            if (_baseElement.ActualHeight != 0 && _baseElement.ActualWidth != 0)
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Zero Size Test for " + s_baseElement + " Passed.");
                this.Result = true;
            }
        }

        FrameworkElement NewElement(string _TestPanel)
        {

            FrameworkElement temp = null;

            switch (_TestPanel)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    //panel.Height = 0;
                    //panel.Width = 0;
                    temp = panel;
                    break;
                case "Grid":
                    Grid g = new Grid();
                    //g.Height = 0;
                    //g.Width = 0;
                    temp = g;
                    break;

                case "StackPanel":
                    StackPanel s = new StackPanel();
                    //s.Height = 0;
                    //s.Width = 0;
                    temp = s;
                    break;

                case "DockPanel":
                    DockPanel d = new DockPanel();
                    //d.Height = 0;
                    //d.Width = 0;
                    temp = d;
                    break;

                case "Canvas":
                    Canvas c = new Canvas();
                    //c.Height = 0;
                    //c.Width = 0;
                    temp = c;
                    break;

                case "Border":
                    Border b = new Border();
                    //b.Height = 0;
                    //b.Width = 0;
                    temp = b;
                    break;

                case "Decorator":
                    Decorator de = new Decorator();
                    //de.Height = 0;
                    //de.Width = 0;
                    temp = de;
                    break;

                case "Viewbox":
                    Viewbox v = new Viewbox();
                    //v.Height = 0;
                    //v.Width = 0;
                    temp = v;
                    break;

                case "ScrollViewer":
                    ScrollViewer sv = new ScrollViewer();
                    sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    //sv.Height = 0;
                    //sv.Width = 0;
                    temp = sv;
                    break;

            }
            return temp;
        }

        FrameworkElement _previousChild = null;
        FrameworkElement _currentChild = null;

        void AddElements(FrameworkElement parent)
        {
            foreach (string element in _testElements)
            {
                if (_currentChild == null)
                {
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, parent);
                }
                else
                {
                    _previousChild = _currentChild;
                    _currentChild = NewElement(element);
                    AddChild(_currentChild, _previousChild);
                }
                CommonFunctionality.FlushDispatcher();
            }
        }

        void AddChild(FrameworkElement _child, FrameworkElement _parent)
        {
            Helpers.Log("Adding " + _child.GetType().Name + " as Child of " + _parent.GetType().Name);
            if (_parent.GetType().BaseType.Name == "Decorator" || _parent.GetType().Name == "Decorator")
            {
                ((Decorator)_parent).Child = _child;
            }
            else if (_parent.GetType().BaseType.Name == "ContentControl")
            {
                ((ContentControl)_parent).Content = _child;
            }
            else
            {
                ((Panel)_parent).Children.Add(_child);
            }
        }
    }
}
