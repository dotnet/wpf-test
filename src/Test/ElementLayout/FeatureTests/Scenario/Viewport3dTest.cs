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
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Scenario
{
    /// test will focus on NaN, and absolute size of Viewport3D
    /// - NaN Test : ensure that Viewport3D sizes to parent panel.
    /// - Absolute Test : ensure that parent panel can auto size around Viewport3D
    /// - Min/Max Height/Width Tests. 
    /// 
    /// 
    /// Files binplaced to 
    ///     \NTTEST\WINDOWSTEST\Client\WcpTests\Layout\ElementLayout\CodeTests
    /// 
    /// To run
    ///     Viewport3D.exe testname
    /// 
    /// Example 
    ///     Viewport3D.exe Viewport3D_Grid
    /// 
    /// 

    [Test(2, "Interop.Graphics", "Viewport3D_Grid", Variables="Area=ElementLayout")]
    public class Viewport3D_Grid : CodeTest
    {
        public Viewport3D_Grid() 
        { }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Grid _grid;
        Viewport3D _vp3d;

        public override FrameworkElement TestContent()
        {
            _grid = new Grid();
            _grid.RowDefinitions.Add(new RowDefinition());
            _grid.RowDefinitions.Add(new RowDefinition());
            _grid.RowDefinitions.Add(new RowDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition());
            _grid.ColumnDefinitions.Add(new ColumnDefinition());

            _vp3d = new Viewport3D();
            Grid.SetColumn(_vp3d, 1);
            Grid.SetRow(_vp3d, 1);

            _grid.Children.Add(_vp3d);

            return _grid;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            //NaN Test.
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, (_grid.ActualWidth / _grid.ColumnDefinitions.Count)))
            {
                Helpers.Log("[NaN] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).X, _grid.ColumnDefinitions[0].ActualWidth))
            {
                Helpers.Log("[NaN] X position of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, (_grid.ActualHeight / _grid.RowDefinitions.Count)))
            {
                Helpers.Log("[NaN] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Y, _grid.RowDefinitions[0].ActualHeight))
            {
                Helpers.Log("[NaN] X position of Viewport3D is incorrect.");
                _tempresult = false;
            }


            //real size test
            _vp3d.Height = 250;
            _vp3d.Width = 250;
            _grid.RowDefinitions[Grid.GetRow(_vp3d)].Height = new GridLength(1, GridUnitType.Auto);
            _grid.ColumnDefinitions[Grid.GetColumn(_vp3d)].Width = new GridLength(1, GridUnitType.Auto);

            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_vp3d.ActualWidth, _grid.ColumnDefinitions[Grid.GetColumn(_vp3d)].ActualWidth))
            {
                Helpers.Log("[Real Size] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).X, _grid.ColumnDefinitions[Grid.GetColumn(_vp3d) - 1].ActualWidth))
            {
                Helpers.Log("[Real Size] X position of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(_vp3d.ActualHeight, _grid.RowDefinitions[Grid.GetRow(_vp3d)].ActualHeight))
            {
                Helpers.Log("[Real Size] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Y, _grid.RowDefinitions[Grid.GetRow(_vp3d) - 1].ActualHeight))
            {
                Helpers.Log("[Real Size] Y position of Viewport3D is incorrect.");
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
    }

    [Test(2, "Interop.Graphics", "Viewport3D_DockPanel", Variables="Area=ElementLayout")]
    public class Viewport3D_DockPanel : CodeTest
    {
        public Viewport3D_DockPanel()
        { }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        DockPanel _dock;
        Viewport3D _vp3d;

        public override FrameworkElement TestContent()
        {
            _dock = new DockPanel();

            _vp3d = new Viewport3D();

            _dock.Children.Add(_vp3d);

            return _dock;
        }

        public override void TestActions()
        {
            DockPanel.SetDock(_vp3d, Dock.Right);

            CommonFunctionality.FlushDispatcher();

            //NaN Test.
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, _dock.ActualWidth))
            {
                Helpers.Log("[NaN] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, _dock.ActualHeight))
            {
                Helpers.Log("[NaN] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            _dock.LastChildFill = false;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, 0))
            {
                Helpers.Log("[NaN, No Fill] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, _dock.ActualHeight))
            {
                Helpers.Log("[NaN, No Fill] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            //real size test
            _vp3d.Width = 100;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_vp3d.ActualWidth, (_dock.ActualWidth - LayoutInformation.GetLayoutSlot(_vp3d).X)))
            {
                Helpers.Log("[Dock.Right] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(_vp3d.ActualHeight, _dock.ActualHeight))
            {
                Helpers.Log("[Dock.Right] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            _vp3d.Height = 100;
            _vp3d.Width = double.NaN;
            DockPanel.SetDock(_vp3d, Dock.Bottom);
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_vp3d.ActualWidth, _dock.ActualWidth))
            {
                Helpers.Log("[Dock.Bottom] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(_vp3d.ActualHeight, (_dock.ActualHeight - LayoutInformation.GetLayoutSlot(_vp3d).Y)))
            {
                Helpers.Log("[Dock.Bottom] Height of Viewport3D is incorrect.");
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
    }

    [Test(2, "Interop.Graphics", "Viewport3D_StackPanel", Variables="Area=ElementLayout")]
    public class Viewport3D_StackPanel : CodeTest
    {
        public Viewport3D_StackPanel()
        { }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        StackPanel _stack;
        Viewport3D _vp3d;

        public override FrameworkElement TestContent()
        {
            _stack = new StackPanel();

            _vp3d = new Viewport3D();

            _stack.Children.Add(_vp3d);

            return _stack;
        }

        public override void TestActions()
        {
            _stack.Orientation = Orientation.Horizontal;
            CommonFunctionality.FlushDispatcher();

            //NaN Test.
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, 0))
            {
                Helpers.Log("[NaN, Horizontal] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, _stack.ActualHeight))
            {
                Helpers.Log("[NaN, Horizontal] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            _stack.Orientation = Orientation.Vertical;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, _stack.ActualWidth))
            {
                Helpers.Log("[NaN, Vertical] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, 0))
            {
                Helpers.Log("[NaN, Vertical] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            //real size test
            _stack.Orientation = Orientation.Horizontal;
            _vp3d.Height = double.NaN;
            _vp3d.Width = 225;
            Rectangle rect = CommonFunctionality.CreateRectangle(5, 5, Brushes.Wheat);
            _stack.Children.Add(rect);
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, LayoutInformation.GetLayoutSlot(rect).X))
            {
                Helpers.Log("[Real Size, Horizontal] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, LayoutInformation.GetLayoutSlot(rect).X))
            {
                Helpers.Log("[Real Size, Horizontal] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }

            _stack.Orientation = Orientation.Vertical;
            _vp3d.Height = 222;
            _vp3d.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, LayoutInformation.GetLayoutSlot(rect).Y))
            {
                Helpers.Log("[Real Size, Vertical] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, LayoutInformation.GetLayoutSlot(rect).Y))
            {
                Helpers.Log("[Real Size, Vertical] Height of Viewport3D is incorrect.");
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
    }

    [Test(2, "Interop.Graphics", "Viewport3D_WrapPanel", Variables="Area=ElementLayout")]
    public class Viewport3D_WrapPanel : CodeTest
    {
        public Viewport3D_WrapPanel()
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
        Viewport3D _vp3d;

        Rectangle _childBefore;
        Rectangle _childAfter;

        public override FrameworkElement TestContent()
        {
            _wrap = new WrapPanel();

            _wrap.Children.Add(_childBefore = CommonFunctionality.CreateRectangle(149, 77, Brushes.CadetBlue));

            _vp3d = new Viewport3D();

            _wrap.Children.Add(_vp3d);

            _wrap.Children.Add(_childAfter = CommonFunctionality.CreateRectangle(33, 99, Brushes.OldLace));

            return _wrap;
        }

        public override void TestActions()
        {
            _wrap.Orientation = Orientation.Horizontal;
            CommonFunctionality.FlushDispatcher();

            //NaN Test.
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, 0))
            {
                Helpers.Log("[NaN, Horizontal] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, Math.Max(_childBefore.ActualHeight, _childAfter.ActualHeight)))
            {
                Helpers.Log("[NaN, Horizontal] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            _wrap.Orientation = Orientation.Vertical;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, Math.Max(_childBefore.ActualWidth, _childAfter.ActualWidth)))
            {
                Helpers.Log("[NaN, Vertical] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, 0))
            {
                Helpers.Log("[NaN, Vertical] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            //real size test

            _wrap.Orientation = Orientation.Horizontal;
            _vp3d.Height = 139;
            _vp3d.Width = 272;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_vp3d.ActualWidth, (LayoutInformation.GetLayoutSlot(_childAfter).X - _childBefore.ActualWidth)))
            {
                Helpers.Log("[Real Size, Horizontal] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(_vp3d.ActualHeight, LayoutInformation.GetLayoutSlot(_vp3d).Height))
            {
                Helpers.Log("[Real Size, Horizontal] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            _wrap.Orientation = Orientation.Vertical;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_vp3d.ActualHeight, (LayoutInformation.GetLayoutSlot(_childAfter).Y - _childBefore.ActualHeight)))
            {
                Helpers.Log("[Real Size, Vertical] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(_vp3d.ActualWidth, LayoutInformation.GetLayoutSlot(_vp3d).Width))
            {
                Helpers.Log("[Real Size, Vertical] Width of Viewport3D is incorrect.");
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
    }

    [Test(2, "Interop.Graphics", "Viewport3D_Border", Variables="Area=ElementLayout")]
    public class Viewport3D_Border : CodeTest
    {
        public Viewport3D_Border()
        { }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Border _border;
        Viewport3D _vp3d;

        public override FrameworkElement TestContent()
        {
            _border = new Border();
            _border.BorderThickness = new Thickness(10);
            _border.BorderBrush = Brushes.Orange;
            _border.CornerRadius = new CornerRadius(100);
            _border.Margin = new Thickness(10);
            _border.Padding = new Thickness(10);

            _vp3d = new Viewport3D();

            _border.Child = _vp3d;

            return _border;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            //NaN Test.
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, (_border.ActualWidth - (_border.BorderThickness.Left * 2) - (_border.Padding.Left * 2))))
            {
                Helpers.Log("[NaN] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, (_border.ActualHeight - (_border.BorderThickness.Top * 2) - (_border.Padding.Top * 2))))
            {
                Helpers.Log("[NaN] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            _vp3d.Height = 107;
            _vp3d.Width = 104;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(_border.ActualWidth, (_vp3d.ActualWidth + (_border.BorderThickness.Left * 2) + (_border.Padding.Left * 2))))
            {
                Helpers.Log("[Real Size] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(_border.ActualHeight, (_vp3d.ActualHeight + (_border.BorderThickness.Top * 2) + (_border.Padding.Top * 2))))
            {
                Helpers.Log("[Real Size] Height of Viewport3D is incorrect.");
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
    }

    [Test(2, "Interop.Graphics", "Viewport3D_Canvas", Variables="Area=ElementLayout")]
    public class Viewport3D_Canvas : CodeTest
    {
        public Viewport3D_Canvas()
        { }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Canvas _canvas;
        Viewport3D _vp3d;

        public override FrameworkElement TestContent()
        {
            _canvas = new Canvas();

            _vp3d = new Viewport3D();

            _canvas.Children.Add(_vp3d);

            return _canvas;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            //NaN Test.
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, 0))
            {
                Helpers.Log("[NaN] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, 0))
            {
                Helpers.Log("[NaN] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            //real size test
            _vp3d.Height = 175;
            _vp3d.Width = 175;
            Canvas.SetBottom(_vp3d, 25);
            Canvas.SetRight(_vp3d, 99);
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Right, (_canvas.ActualWidth - Canvas.GetRight(_vp3d))))
            {
                Helpers.Log("[Real Size, Right] Viewport3D layout in Canvas is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Bottom, (_canvas.ActualHeight - Canvas.GetBottom(_vp3d))))
            {
                Helpers.Log("[Real Size, Bottom] Viewport3D layout in Canvas is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(_vp3d.ActualWidth, (_canvas.ActualWidth - LayoutInformation.GetLayoutSlot(_vp3d).Left - Canvas.GetRight(_vp3d))))
            {
                Helpers.Log("[Real Size, Width] Viewport3D layout in Canvas is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(_vp3d.ActualHeight, (_canvas.ActualHeight - LayoutInformation.GetLayoutSlot(_vp3d).Top - Canvas.GetBottom(_vp3d))))
            {
                Helpers.Log("[Real Size, Height] Viewport3D layout in Canvas is incorrect.");
                _tempresult = false;
            }

            Canvas.SetLeft(_vp3d, 122);
            Canvas.SetTop(_vp3d, 10);
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Left, Canvas.GetLeft(_vp3d)))
            {
                Helpers.Log("[Real Size, Left] Viewport3D layout in Canvas is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Top, Canvas.GetTop(_vp3d)))
            {
                Helpers.Log("[Real Size, Top] Viewport3D layout in Canvas is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(_vp3d.ActualWidth, (LayoutInformation.GetLayoutSlot(_vp3d).Right - Canvas.GetLeft(_vp3d))))
            {
                Helpers.Log("[Real Size, Width] Viewport3D layout in Canvas is incorrect.");
                _tempresult = false;
            }

            if (!DoubleUtil.AreClose(_vp3d.ActualHeight, (LayoutInformation.GetLayoutSlot(_vp3d).Bottom - Canvas.GetTop(_vp3d))))
            {
                Helpers.Log("[Real Size, Height] Viewport3D layout in Canvas is incorrect.");
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
    }

    [Test(2, "Interop.Graphics", "Viewport3D_Viewbox", Variables="Area=ElementLayout")]
    public class Viewport3D_Viewbox : CodeTest
    {
        public Viewport3D_Viewbox()
        { }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Viewbox _vbx;
        Viewport3D _vp3d;

        public override FrameworkElement TestContent()
        {
            _vbx = new Viewbox();

            _vp3d = new Viewport3D();

            _vbx.Child = _vp3d;

            return _vbx;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();

            //NaN Test.
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, 0))
            {
                Helpers.Log("[NaN] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, 0))
            {
                Helpers.Log("[NaN] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            //real size test

            _vp3d.Height = 100;
            _vp3d.Width = 100;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.LessThan(LayoutInformation.GetLayoutSlot(_vp3d).Width, _vbx.ActualWidth))
            {
                Helpers.Log("[Real Size] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.LessThan(LayoutInformation.GetLayoutSlot(_vp3d).Height, _vbx.ActualHeight))
            {
                Helpers.Log("[Real Size] Height of Viewport3D is incorrect.");
                _tempresult = false;
            }

            _vbx.Stretch = Stretch.None;
            CommonFunctionality.FlushDispatcher();

            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Width, _vbx.ActualWidth))
            {
                Helpers.Log("[Real Size] Width of Viewport3D is incorrect.");
                _tempresult = false;
            }
            if (!DoubleUtil.AreClose(LayoutInformation.GetLayoutSlot(_vp3d).Height, _vbx.ActualHeight))
            {
                Helpers.Log("[Real Size] Height of Viewport3D is incorrect.");
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
    }

    [Test(2, "Interop.Graphics", "Viewport3D_MinMax", Variables="Area=ElementLayout")]
    public class Viewport3D_MinMax : CodeTest
    {
        public Viewport3D_MinMax()
        { }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Viewport3D _vp3d;

        public override FrameworkElement TestContent()
        {
            _border = new Border();

            _vp3d = new Viewport3D();

            _border.Child = _vp3d;

            return _border;
        }

        double _minValue = 222;
        double _maxValue = 109;

        public override void TestActions()
        {
            //verify defaults.
            if (_vp3d.MinHeight != 0 || _vp3d.MinWidth != 0)
            {
                Helpers.Log("Min Defaults were incorrect.");
                _tempresult = false;
            }

            if (_vp3d.MaxHeight != double.PositiveInfinity || _vp3d.MaxWidth != double.PositiveInfinity)
            {
                Helpers.Log("Max Defaults were incorrect");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            //min height/width test ()
            _vp3d.MinHeight = _minValue;
            _vp3d.MinWidth = _minValue;

            _border.Height = _minValue / 2;
            _border.Width = _minValue / 2;

            CommonFunctionality.FlushDispatcher();

            if (!Double.IsNaN(_vp3d.Height) || !Double.IsNaN(_vp3d.Width))
            {
                Helpers.Log("[Min] Viewport3D Height and Width should be NaN.");
                _tempresult = false;
            }
            if (!DoubleUtil.GreaterThan(_vp3d.ActualWidth, LayoutInformation.GetLayoutSlot(_vp3d).Width))
            {
                Helpers.Log("[Min] Viewport3D ActualWidth should be larger than Grid.");
                _tempresult = false;
            }
            if (!DoubleUtil.GreaterThan(_vp3d.ActualHeight, LayoutInformation.GetLayoutSlot(_vp3d).Height))
            {
                Helpers.Log("[Min] Viewport3D ActualHeight should be larger than Grid.");
                _tempresult = false;
            }

            //max height/width test ()
            _vp3d.MinWidth = 0;
            _vp3d.MinHeight = 0;
            _vp3d.MaxHeight = _maxValue;
            _vp3d.MaxWidth = _maxValue;

            _border.Height = _maxValue * 2;
            _border.Width = _maxValue * 2;

            CommonFunctionality.FlushDispatcher();

            if (!Double.IsNaN(_vp3d.Height) || !Double.IsNaN(_vp3d.Width))
            {
                Helpers.Log("[Max] Viewport3D Height and Width should be NaN.");
                _tempresult = false;
            }
            if (!DoubleUtil.LessThan(_vp3d.ActualWidth, LayoutInformation.GetLayoutSlot(_vp3d).Width))
            {
                Helpers.Log("[Max] Viewport3D ActualWidth should be less than Grid.");
                _tempresult = false;
            }
            if (!DoubleUtil.LessThan(_vp3d.ActualHeight, LayoutInformation.GetLayoutSlot(_vp3d).Height))
            {
                Helpers.Log("[Max] Viewport3D ActualHeight should be less than Grid.");
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
    }
}
