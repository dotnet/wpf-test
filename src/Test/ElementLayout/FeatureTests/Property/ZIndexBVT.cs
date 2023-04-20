// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Layout;
using Microsoft.Test.Logging;
using ElementLayout.TestLibrary;
using Microsoft.Test.Threading;
using System.Windows.Data;
using Microsoft.Test.Input;

namespace ElementLayout.FeatureTests.Property
{

    /*
     * Change the Z-order and insert a child, 
     * change Z-order in collecion with 1 child, 
     * change z-order and remove child, 
     * edge cases, etc., etc., etc.
     */

    /// <summary>
    /// 
    /// </summary>
    [Test(0, "Property.ZIndex", "SingleChildDecorator", Variables = "Area=ElementLayout")]
    public class ZIndexSingleChildDecorator : CodeTest
    {
        public ZIndexSingleChildDecorator() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        private Border _parent;
        private Border _child;
        private int _value = 5;

        public override FrameworkElement TestContent()
        {
            _parent = new Border();

            _child = new Border();
            _child.Background = Brushes.Crimson;
            _child.Height = 100;
            _child.Width = 100;
            _parent.Child =  _child;

            Panel.SetZIndex(_child, _value);

            return _parent;
        }

        public override void TestActions()
        {
            _tempresult = ZIndexCheck();

            _value = -1;
            Panel.SetZIndex(_child, _value);
            DispatcherHelper.DoEvents();

            _tempresult = ZIndexCheck();

            _value = 99;
            Panel.SetZIndex(_child, _value);
            DispatcherHelper.DoEvents();

            _tempresult = ZIndexCheck();

            _value = int.MaxValue;
            Panel.SetZIndex(_child, _value);
            DispatcherHelper.DoEvents();

            _tempresult = ZIndexCheck();

            _value = 1;
            Panel.SetZIndex(_child, _value);
            DispatcherHelper.DoEvents();
        }

        private bool _tempresult = true;
        private bool ZIndexCheck()
        {
            Helpers.Log(Panel.GetZIndex(_child) == _value ? string.Format("z index value ({0}) is correct.", _value) : string.Format("z index value ({0}) is wrong.", _value));
            return (Panel.GetZIndex(_child) == _value);
        }

        public override void TestVerify()
        {
            Helpers.Log(_tempresult ? "Panel ZIndex test passed." : "Panel ZIndex test failed.");
            this.Result = _tempresult;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Test(0, "Property.ZIndex", "SingleChildPanel", Variables = "Area=ElementLayout")]
    public class ZIndexSingleChildPanel : CodeTest
    {
        public ZIndexSingleChildPanel() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        private DockPanel _parent;
        private Border _child;
        private int _value = 5;

        public override FrameworkElement TestContent()
        {
            _parent = new DockPanel();

            _child = new Border();
            _child.Background = Brushes.Crimson;
            _child.Height = 100;
            _child.Width = 100;
            _parent.Children.Add(_child);

            Panel.SetZIndex(_child, _value);

            return _parent;
        }

        public override void TestActions()
        {
            _tempresult = ZIndexCheck();

            _value = -1;
            Panel.SetZIndex(_child, _value);
            DispatcherHelper.DoEvents();

            _tempresult = ZIndexCheck();

            _value = 99;
            Panel.SetZIndex(_child, _value);
            DispatcherHelper.DoEvents();

            _tempresult = ZIndexCheck();

            _value = int.MaxValue;
            Panel.SetZIndex(_child, _value);
            DispatcherHelper.DoEvents();

            _tempresult = ZIndexCheck();

            _value = 1;
            Panel.SetZIndex(_child, _value);
            DispatcherHelper.DoEvents();
        }

        private bool _tempresult = true;
        private bool ZIndexCheck()
        {
            Helpers.Log(Panel.GetZIndex(_child) == _value ? string.Format("z index value ({0}) is correct.", _value) : string.Format("z index value ({0}) is wrong.", _value));
            return (Panel.GetZIndex(_child) == _value);
        }

        public override void TestVerify()
        {
            Helpers.Log(_tempresult ? "Panel ZIndex test passed." : "Panel ZIndex test failed.");
            this.Result = _tempresult;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Test(0, "Property.ZIndex", "Insert", Variables = "Area=ElementLayout")]
    public class ZIndexInsert : CodeTest
    {
        public ZIndexInsert() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        private Canvas _canvas;
        private Border _one;
        private Border _two;
        private Border _three;

        public override FrameworkElement TestContent()
        {
            _canvas = new Canvas();

            _one = new Border();
            _one.Background = Brushes.Crimson;
            _one.Height = 100;
            _one.Width = 100;

            _two = new Border();
            _two.Background = Brushes.Navy;
            _two.Height = 400;
            _two.Width = 400;

            _canvas.Children.Add(_one);
            _canvas.Children.Add(_two);

            return _canvas;
        }

        public override void TestActions()
        {
            Panel.SetZIndex(_two, 1);
            Panel.SetZIndex(_one, 10);

            _three = new Border();
            _three.Background = Brushes.Green;
            _three.Height = 250;
            _three.Width = 250;

            Panel.SetZIndex(_three, 5);
            _canvas.Children.Add(_three);
        }

        public override void TestVerify()
        {
            Point point = new Point(50, 50);
            UIElement element = LayoutUtility.GetInputElement(_canvas, point);

            if (Object.Equals(element, _one))
            {
                Helpers.Log("one is on top.");
            }
            else
            {
                Helpers.Log("Test failed");
                this.Result = false;
                return;
            }

            point = new Point(point.X + 200, point.Y);
            element = LayoutUtility.GetInputElement(_canvas, point);

            if (Object.Equals(element, _three))
            {
                Helpers.Log("three is in middle.");
            }
            else
            {
                Helpers.Log("Test failed");
                this.Result = false;
                return;
            }

            point = new Point(point.X + 100, point.Y);
            element = LayoutUtility.GetInputElement(_canvas, point);

            if (Object.Equals(element, _two))
            {
                Helpers.Log("two is on bottom.");
            }
            else
            {
                Helpers.Log("Test failed.");
                this.Result = false;
                return;
            }
            this.Result = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Test(0, "Property.ZIndex", "RemovePanel", Variables = "Area=ElementLayout")]
    public class ZIndexRemovePanel : CodeTest
    {
        public ZIndexRemovePanel() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Border _one;
        private Border _two;
        private Border _three;

        public override FrameworkElement TestContent()
        {
            _grid = new Grid();

            _one = new Border();
            _one.Background = Brushes.Crimson;
            _one.Height = 100;
            _one.Width = 100;

            _two = new Border();
            _two.Background = Brushes.Navy;
            _two.Height = 400;
            _two.Width = 400;

            _three = new Border();
            _three.Background = Brushes.Green;
            _three.Height = 250;
            _three.Width = 250;

            Panel.SetZIndex(_one, 10);
            Panel.SetZIndex(_two, 1);
            Panel.SetZIndex(_three, 5);
            
            _grid.Children.Add(_one);
            _grid.Children.Add(_two);
            _grid.Children.Add(_three);

            return _grid;
        }

        public override void TestActions()
        {
            Panel.SetZIndex(_one, 99);
            Panel.SetZIndex(_two, 99);
            Panel.SetZIndex(_three, 99);
        }

        public override void TestVerify()
        {
            try 
            {
                _grid.Children.Remove(_two);
                this.Result = true;
            }
            catch (Exception ex) 
            {
                Helpers.Log(ex.Message);
                Helpers.Log("Error removing child after setting z index.");
                this.Result = false;
            }
            
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Test(0, "Property.ZIndex", "RemoveDecorator", Variables = "Area=ElementLayout")]
    public class ZIndexRemoveDecorator : CodeTest
    {
        public ZIndexRemoveDecorator() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        private Border _parent;
        private Border _child;

        public override FrameworkElement TestContent()
        {
            _parent = new Border();

            _child = new Border();
            _child.Background = Brushes.Crimson;
            _child.Height = 100;
            _child.Width = 100;

            Panel.SetZIndex(_child, -1);

            _parent.Child = _child;

            return _parent;
        }

        public override void TestActions()
        {
            Panel.SetZIndex(_child, int.MaxValue);
        }

        public override void TestVerify()
        {
            try
            {
                _parent.Child = null;
                this.Result = true;
            }
            catch (Exception ex)
            {
                Helpers.Log(ex.Message);
                Helpers.Log("Error removing child after setting z index.");
                this.Result = false;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Test(1, "Property.ZIndex", "RandomUpdate", Variables = "Area=ElementLayout")]
    public class ZIndexRandomUpdate : CodeTest
    {
        public ZIndexRandomUpdate() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        private StackPanel _parent;

        public override FrameworkElement TestContent()
        {
            ScrollViewer sv = new ScrollViewer();
            sv.CanContentScroll = true;
            _parent = new StackPanel();
            AddChildren(25);
            sv.Content = _parent;
            return sv;
        }

        private void AddChildren(int count) 
        {
            for (int i = 0; i <= count; i++) 
            {
                Button btn = new Button();
                btn.Click += new RoutedEventHandler(btn_Click);
                Binding bind = new Binding("ZIndex");
                bind.Source = btn;
                btn.SetBinding(Button.ContentProperty, bind);
                btn.Height = 200;
                btn.Width = 200;
                Panel.SetZIndex(btn, _zindex);   
                _parent.Children.Add(btn);
            }
        }
        private int _zindex = 0;
        private int _count = 0;

        public override void TestActions()
        {
            DoTest();
        }
        
        private void DoTest() 
        {
            while (_count < 100)
            {
                // get random child
                int child = RandomValue(0, _parent.Children.Count);
                UIElement target = _parent.Children[child];

                Helpers.Log(string.Format("target child is {0}.", child));

                // bring child into view
                ((FrameworkElement)target).BringIntoView();
                DispatcherHelper.DoEvents();

                // click child
                Input.MoveToAndClick(target);
                DispatcherHelper.DoEvents();
                
                _count++;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _zindex = RandomValue(-100, 100);
                Panel.SetZIndex((UIElement)sender, _zindex);
                Helpers.Log(string.Format("set z index to {0}.", _zindex));
            }
            catch (Exception ex)
            {
                Helpers.Log(ex.Message);
                Helpers.Log(string.Format("Error setting z index to {0}.",_zindex));
                _tempresult = false;
            }
        }

        private int RandomValue(int min, int max) 
        {
            Random r = new Random(DateTime.Now.Millisecond);
            return r.Next(min, max);
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

    /// <summary>
    /// 
    /// </summary>
    [Test(1, "Property.ZIndex", "RandomUpdateAndRemove", Variables = "Area=ElementLayout")]
    public class ZIndexRandomUpdateAndRemove : CodeTest
    {
        public ZIndexRandomUpdateAndRemove() { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        private StackPanel _parent;

        public override FrameworkElement TestContent()
        {
            ScrollViewer sv = new ScrollViewer();
            sv.CanContentScroll = true;
            _parent = new StackPanel();
            AddChildren(25);
            sv.Content = _parent;
            return sv;
        }

        private void AddChildren(int count)
        {
            for (int i = 0; i <= count; i++)
            {
                Button btn = new Button();
                btn.Click += new RoutedEventHandler(btn_Click);
                Binding bind = new Binding("ZIndex");
                bind.Source = btn;
                btn.SetBinding(Button.ContentProperty, bind);
                btn.Height = 200;
                btn.Width = 200;
                Panel.SetZIndex(btn, _zindex);
                _parent.Children.Add(btn);
            }
        }
        private int _zindex = 0;
        private int _count = 0;

        public override void TestActions()
        {
            DoTest();
        }

        private void DoTest()
        {
            while (_parent.Children.Count != 0)
            {
                // get random child
                int child = RandomValue(0, _parent.Children.Count);
                UIElement target = _parent.Children[child];

                Helpers.Log(string.Format("target child is {0}.", child));

                // bring child into view
                ((FrameworkElement)target).BringIntoView();
                DispatcherHelper.DoEvents();

                // click child
                Input.MoveToAndClick(target);
                DispatcherHelper.DoEvents();

                _count++;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //change zindex
                _zindex = RandomValue(-100, 100);
                Panel.SetZIndex((UIElement)sender, _zindex);
                Helpers.Log(string.Format("set z index to {0}.", _zindex));
                DispatcherHelper.DoEvents();

                //now remove child
                _parent.Children.Remove((UIElement)sender);
                DispatcherHelper.DoEvents();
            }
            catch (Exception ex)
            {
                Helpers.Log(ex.Message);
                Helpers.Log(string.Format("Error setting z index to {0} and removing child.", _zindex));
                _tempresult = false;
            }
        }

        private int RandomValue(int min, int max)
        {
            Random r = new Random(DateTime.Now.Millisecond);
            return r.Next(min, max);
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

}
