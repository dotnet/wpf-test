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
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.HeightWidthDefault", TestParameters = "target=DockPanel, test=HeightWidthDefault")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.HeightWidthActual", TestParameters = "target=DockPanel, test=HeightWidthActual")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.ChildHeightWidth", TestParameters = "target=DockPanel, test=ChildHeightWidth")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.MinHeightWidth", TestParameters = "target=DockPanel, test=MinHeightWidth")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.MaxHeightWidth", TestParameters = "target=DockPanel, test=MaxHeightWidth")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.Visibility", TestParameters = "target=DockPanel, test=Visibility")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.HorizontalAlignment", TestParameters = "target=DockPanel, test=HorizontalAlignment")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.VerticalAlignment", TestParameters = "target=DockPanel, test=VerticalAlignment")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.FlowDirection", TestParameters = "target=DockPanel, test=FlowDirection")]
    [Test(1, "Panels.DockPanel", "FrameworkElementProps.Margin", TestParameters = "target=DockPanel, test=Margin")]
    public class DockPanelFETest : FrameworkElementPropertiesTest
    {
        public DockPanelFETest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }
    }

    [Test(1, "Panels.DockPanel", "DockPanelChildNaturalSizePlusMargins", Variables = "Area=ElementLayout")]
    public class DockPanelChildNaturalSizePlusMargins : CodeTest
    {
        public DockPanelChildNaturalSizePlusMargins()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Content = this.TestContent();
        }

        Image _img;
        DockPanel _dock;

        public override FrameworkElement TestContent()
        {
            Canvas root = new Canvas();
            Border border = new Border();

            border.Background = new SolidColorBrush(Colors.Purple);

            _dock = new DockPanel();

            BitmapImage bitImg = new BitmapImage(new Uri("computer.bmp", UriKind.RelativeOrAbsolute));

            _img = new Image();
            _img.Source = bitImg;
            _img.Margin = new Thickness(25, 25, 25, 25);

            root.Children.Add(border);
            border.Child = _dock;
            _dock.Children.Add(_img);

            return root;
        }

        public override void TestActions()
        {
            Helpers.Log("No Actions Needed...");
        }

        public override void TestVerify()
        {
            if (_dock.RenderSize.Height != (_img.RenderSize.Height + 50) || _dock.RenderSize.Width != (_img.RenderSize.Width + 50))
            {
                Helpers.Log("image size = " + _img.RenderSize);
                Helpers.Log("dockpanel size = " + _dock.RenderSize);
                Helpers.Log("dockpanel size is not equal to the image size + margins...");
                this.Result = false;
            }
            else
            {
                Helpers.Log("image size = " + _img.RenderSize);
                Helpers.Log("dockpanel size = " + _dock.RenderSize);
                this.Result = true;
            }

        }
    }

    [Test(1, "Panels.DockPanel", "DockingTest.Panel", TestParameters = "target=Panel")]
    [Test(1, "Panels.DockPanel", "DockingTest.Canvas", TestParameters = "target=Canvas")]
    [Test(1, "Panels.DockPanel", "DockingTest.StackPanel", TestParameters = "target=StackPanel")]
    [Test(1, "Panels.DockPanel", "DockingTest.Grid", TestParameters = "target=Grid")]
    [Test(1, "Panels.DockPanel", "DockingTest.DockPanel", TestParameters = "target=DockPanel")]
    [Test(1, "Panels.DockPanel", "DockingTest.Decorator", TestParameters = "target=Decorator")]
    [Test(1, "Panels.DockPanel", "DockingTest.Border", TestParameters = "target=Border")]
    [Test(1, "Panels.DockPanel", "DockingTest.Viewbox", TestParameters = "target=Viewbox")]
    [Test(1, "Panels.DockPanel", "DockingTest.Transform", TestParameters = "target=Transform")]
    [Test(1, "Panels.DockPanel", "DockingTest.ScrollViewer", TestParameters = "target=ScrollViewer")]
    public class DockPanelDocking : CodeTest
    {
        public DockPanelDocking() { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            _root = new DockPanel();
            _root.Background = Brushes.RoyalBlue;
            _root.Height = 600;
            _root.Width = 600;
            return _root;
        }

        public override void TestActions()
        {
            string target = DriverState.DriverParameters["target"];
            DockPanelDockingTest(target);
        }

        public override void TestVerify()
        {
            this.Result = _tempresult;
        }

        private DockPanel _root = null;
        private bool _tempresult = true;
        private bool _isTransformed = false;
        private bool _isSpecialTransformSize = false;
        private double _childWidth = 0;
        private double _childHeight = 0;
        private Transform _childTransform = null;
        private double _getSquareValue = 0;
        private double _rotatedValue = 0;

        private void DockPanelDockingTest(string ChildType)
        {
            int ChildCount = 0;
            if (ChildType == "Transform")
            {
                _isTransformed = true;
                ChildCount = 4;
            }
            else
            {
                _isTransformed = false;
                ChildCount = 5;
            }

            //one child LastChildFill = true
            _root.LastChildFill = true;
            AddDockChild(ChildType, new Size(double.NaN, double.NaN), Dock.Left, new Thickness(0));
            VerifyDockPanelDocking("SingleChildLCFtrue");

            //one child LastChildFill = false
            _root.LastChildFill = false;
            AddDockChild(ChildType, new Size(233, 233), Dock.Left, new Thickness(0));
            VerifyDockPanelDocking("SingleChildLCFfalse");

            _isSpecialTransformSize = true;

            //single DockPanelDocking LastChildFill = false 
            _root.LastChildFill = false;
            for (int i = 0; i <= ChildCount; i++)
            {
                AddDockChild(ChildType, new Size(100, double.NaN), Dock.Left, new Thickness(0));
            }
            VerifyDockPanelDocking("SingleDock");

            _root.LastChildFill = false;
            for (int i = 0; i <= ChildCount; i++)
            {
                AddDockChild(ChildType, new Size(100, double.NaN), Dock.Right, new Thickness(0));
            }
            VerifyDockPanelDocking("SingleDock");

            _root.LastChildFill = false;
            for (int i = 0; i <= ChildCount; i++)
            {
                AddDockChild(ChildType, new Size(double.NaN, 100), Dock.Top, new Thickness(0));
            }
            VerifyDockPanelDocking("SingleDock");

            _root.LastChildFill = false;
            for (int i = 0; i <= ChildCount; i++)
            {
                AddDockChild(ChildType, new Size(double.NaN, 100), Dock.Bottom, new Thickness(0));
            }
            VerifyDockPanelDocking("SingleDock");

            //single DockPanelDocking with margins LastChildFill = false 
            _root.LastChildFill = false;
            for (int i = 0; i <= (ChildCount - 1); i++)
            {
                AddDockChild(ChildType, new Size(100, double.NaN), Dock.Left, new Thickness(10));
            }
            VerifyDockPanelDocking("SingleDock");

            _root.LastChildFill = false;
            for (int i = 0; i <= (ChildCount - 1); i++)
            {
                AddDockChild(ChildType, new Size(100, double.NaN), Dock.Right, new Thickness(10));
            }
            VerifyDockPanelDocking("SingleDock");

            _root.LastChildFill = false;
            for (int i = 0; i <= (ChildCount - 1); i++)
            {
                AddDockChild(ChildType, new Size(double.NaN, 100), Dock.Top, new Thickness(10));
            }
            VerifyDockPanelDocking("SingleDock");

            _root.LastChildFill = false;
            for (int i = 0; i <= (ChildCount - 1); i++)
            {
                AddDockChild(ChildType, new Size(double.NaN, 100), Dock.Bottom, new Thickness(10));
            }
            VerifyDockPanelDocking("SingleDock");

            //mixed DockPanelDocking LastChildFill = false 

            if (ChildType != "Transform")
            {
                //mixed DockPanelDocking LastChildFill = true 
                _root.LastChildFill = true;
                for (int i = 0; i <= (ChildCount - 1); i++)
                {
                    switch (i)
                    {
                        case 0:
                            AddDockChild(ChildType, new Size(100, double.NaN), Dock.Left, new Thickness(0));
                            break;
                        case 1:
                            AddDockChild(ChildType, new Size(double.NaN, 100), Dock.Top, new Thickness(0));
                            break;
                        case 2:
                            AddDockChild(ChildType, new Size(100, double.NaN), Dock.Right, new Thickness(0));
                            break;
                        case 3:
                            AddDockChild(ChildType, new Size(double.NaN, 100), Dock.Bottom, new Thickness(0));
                            break;
                        case 4:
                            AddDockChild(ChildType, new Size(double.NaN, double.NaN), Dock.Left, new Thickness(0));
                            break;
                    }
                }
                VerifyDockPanelDocking("MultipleDock");

                //mixed DockPanelDocking LastChildFill = true 
                _root.LastChildFill = true;
                for (int i = 0; i <= (ChildCount - 1); i++)
                {
                    switch (i)
                    {
                        case 0:
                            AddDockChild(ChildType, new Size(100, double.NaN), Dock.Left, new Thickness(10));
                            break;
                        case 1:
                            AddDockChild(ChildType, new Size(double.NaN, 100), Dock.Top, new Thickness(10));
                            break;
                        case 2:
                            AddDockChild(ChildType, new Size(100, double.NaN), Dock.Right, new Thickness(10));
                            break;
                        case 3:
                            AddDockChild(ChildType, new Size(double.NaN, 100), Dock.Bottom, new Thickness(10));
                            break;
                        case 4:
                            AddDockChild(ChildType, new Size(double.NaN, double.NaN), Dock.Left, new Thickness(10));
                            break;
                    }
                }
                VerifyDockPanelDocking("MultipleDock");
            }
        }

        private void AddDockChild(string ChildType, Size ChildSize, Dock ChildDock, Thickness margin)
        {
            FrameworkElement child = null;
            switch (ChildType)
            {
                case "Panel":
                    {
                        //Helpers.Log("Adding Child : Panel");
                        TestPanel panel = new TestPanel();
                        panel.Margin = margin;
                        panel.Background = Brushes.YellowGreen;
                        child = panel;
                        break;
                    }

                case "Canvas":
                    {
                        //Helpers.Log("Adding Child : Canvas");
                        Canvas canvas = new Canvas();
                        canvas.Background = Brushes.YellowGreen;
                        canvas.Margin = margin;
                        Rectangle r1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Yellow);
                        Rectangle r2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Green);
                        canvas.Children.Add(r1);
                        canvas.Children.Add(r2);
                        Canvas.SetLeft(r1, 10);
                        Canvas.SetTop(r1, 10);
                        Canvas.SetBottom(r2, 10);
                        Canvas.SetRight(r2, 10);
                        child = canvas;
                        break;
                    }

                case "StackPanel":
                    {
                        //Helpers.Log("Adding Child : dockPanel");
                        StackPanel stack = new StackPanel();
                        stack.Margin = margin;
                        stack.Background = Brushes.GhostWhite;
                        Rectangle r1 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Yellow);
                        Rectangle r2 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Green);
                        Rectangle r3 = CommonFunctionality.CreateRectangle(50, 50, Brushes.Blue);
                        stack.Children.Add(r1);
                        stack.Children.Add(r2);
                        stack.Children.Add(r3);
                        child = stack;
                        break;
                    }

                case "Grid":
                    {
                        //Helpers.Log("Adding Child : Grid");
                        ColumnDefinition col1, col2;
                        RowDefinition row1, row2;
                        Grid grid = new Grid();
                        grid.Margin = margin;
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
                        child = grid;
                        break;
                    }

                case "DockPanel":
                    {
                        //Helpers.Log("Adding Child : DockPanel");
                        DockPanel dock = new DockPanel();
                        dock.Background = Brushes.Firebrick;
                        dock.Margin = margin;
                        Rectangle r1 = CommonFunctionality.CreateRectangle(250, 100, Brushes.Gray);
                        Rectangle r2 = CommonFunctionality.CreateRectangle(100, 150, Brushes.HotPink);

                        dock.Children.Add(r1);
                        dock.Children.Add(r2);

                        DockPanel.SetDock(r1, Dock.Top);
                        DockPanel.SetDock(r2, Dock.Right);
                        child = dock;
                        break;
                    }

                case "Decorator":
                    {
                        //Helpers.Log("Adding Child : Decorator");
                        Decorator dec = new Decorator();
                        dec.Margin = margin;

                        Rectangle r = new Rectangle();
                        r.Fill = Brushes.Crimson;
                        r.Margin = new Thickness(10);
                        dec.Child = r;
                        child = dec;
                        break;
                    }

                case "Border":
                    {
                        //Helpers.Log("Adding Child : Border");
                        Border bor = new Border();
                        bor.CornerRadius = new CornerRadius(50);
                        bor.Background = Brushes.Gray;
                        bor.Padding = new Thickness(50);
                        bor.Margin = margin;
                        Rectangle r = CommonFunctionality.CreateRectangle(100, 100, Brushes.Orange);

                        bor.Child = r;

                        child = bor;
                        break;
                    }

                case "Viewbox":
                    {
                        //Helpers.Log("Adding Child : ViewBox");
                        Viewbox vb = new Viewbox();
                        vb.Stretch = Stretch.Fill;
                        vb.Margin = margin;
                        TextBlock txt = new TextBlock();
                        txt.Text = "foo";

                        vb.Child = txt;
                        child = vb;
                        break;
                    }

                case "Transform":
                    {
                        //Helpers.Log("Adding Child : Transform");
                        RotateTransform rt = new RotateTransform(45);

                        Decorator td = new Decorator();
                        td.LayoutTransform = rt;
                        td.Margin = margin;
                        Rectangle r = new Rectangle();
                        r.Fill = Brushes.AntiqueWhite;

                        td.Child = r;
                        child = td;
                        break;
                    }

                case "ScrollViewer":
                    {
                        //Helpers.Log("Adding Child : ScrollViewer");
                        ScrollViewer sv = new ScrollViewer();
                        sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                        sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                        sv.Margin = margin;
                        TextBlock txt = new TextBlock();
                        txt.Text = "some text";
                        txt.FontSize = 100;

                        sv.Content = txt;

                        child = sv;
                        break;
                    }
            }
            if (!_isTransformed)
            {
                child.Height = ChildSize.Height;
                child.Width = ChildSize.Width;
            }
            else
            {
                if (!_isSpecialTransformSize)
                {
                    child.Height = ChildSize.Height;
                    child.Width = ChildSize.Width;
                }
                else
                {
                    child.Height = 100;
                    child.Width = 100;
                }
            }
            DockPanel.SetDock(child, ChildDock);
            _root.Children.Add(child);
        }

        private void VerifyDockPanelDocking(string DockPanelDockingScenario)
        {
            CommonFunctionality.FlushDispatcher();
            FrameworkElement child = null;
            Point childPosition;

            if (!_isTransformed)
            {
                switch (DockPanelDockingScenario)
                {
                    case "SingleChildLCFtrue":
                        child = _root.Children[0] as FrameworkElement;

                        if (!DoubleUtil.AreClose((child.ActualWidth + child.Margin.Left + child.Margin.Right), _root.ActualWidth) || !DoubleUtil.AreClose((child.ActualHeight + child.Margin.Top + child.Margin.Bottom), _root.ActualHeight))
                        {
                            Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                        }
                        break;

                    case "SingleChildLCFfalse":
                        child = _root.Children[0] as FrameworkElement;
                        childPosition = LayoutUtility.GetElementPosition(child, _root);

                        if (!DoubleUtil.AreClose(childPosition.Y, ((_root.ActualHeight / 2) - (child.ActualHeight / 2))) || !DoubleUtil.AreClose(childPosition.X, (0 + child.Margin.Left)))
                        {
                            Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                        }
                        break;

                    case "SingleDock":
                        bool result = true;
                        foreach (UIElement c in _root.Children)
                        {
                            if (result)
                            {
                                child = c as FrameworkElement;
                                result = ValidateSingleDock(child, false);
                            }
                        }
                        if (!result)
                        {
                            if (child.Margin.Left != 0)
                            {
                                Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + " " + DockPanel.GetDock(child).ToString() + ", Margin On Child " + child.Margin.ToString() + ".");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + " " + DockPanel.GetDock(child).ToString() + ".");
                                _tempresult = false;
                            }
                        }
                        else
                        {
                            if (child.Margin.Left != 0)
                            {
                                Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + " " + DockPanel.GetDock(child).ToString() + ", Margin On Child " + child.Margin.ToString() + ".");
                            }
                            else
                            {
                                Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + " " + DockPanel.GetDock(child).ToString() + ".");
                            }
                        }
                        _offsetLeft = 0;
                        _offsetRight = 0;
                        _offsetTop = 0;
                        _offsetBottom = 0;
                        break;

                    case "MultipleDock":
                        bool mResult = true;
                        foreach (UIElement c in _root.Children)
                        {
                            if (mResult)
                            {
                                child = c as FrameworkElement;
                                mResult = ValidateMultipleDock(child, false);
                            }
                        }
                        if (!mResult)
                        {
                            if (child.Margin.Left != 0)
                            {
                                Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + " , Margin On Child " + child.Margin.ToString() + ".");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                                _tempresult = false;
                            }
                        }
                        else
                        {
                            if (child.Margin.Left != 0)
                            {
                                Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + " , Margin On Child " + child.Margin.ToString() + ".");
                            }
                            else
                            {
                                Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                            }
                        }
                        _offsetLeft = 0;
                        _offsetRight = 0;
                        _offsetTop = 0;
                        _offsetBottom = 0;
                        break;
                }
            }
            else
            {
                //Transform childTransform = null;// VisualTreeHelper.GetTransform(child);
                //double getSquareValue = 0;// ((child.ActualHeight * child.ActualHeight) + (child.ActualWidth * child.ActualWidth));

                //double rotatedValue = 0;// Math.Sqrt(getSquareValue);

                switch (DockPanelDockingScenario)
                {
                    case "SingleChildLCFtrue":
                        child = _root.Children[0] as FrameworkElement;
                        childPosition = LayoutUtility.GetElementPosition(child, _root);

                        _childTransform = VisualTreeHelper.GetTransform(child);
                        _getSquareValue = ((child.ActualHeight * child.ActualHeight) + (child.ActualWidth * child.ActualWidth));

                        _rotatedValue = Math.Sqrt(_getSquareValue);

                        _childWidth = _rotatedValue;
                        _childHeight = _rotatedValue;

                        if (!DoubleUtil.AreClose(childPosition.Y, ((_root.ActualHeight / 2) - (_childHeight / 2))) || !DoubleUtil.AreClose(childPosition.X, (_root.ActualHeight / 2)))
                        //if (!DoubleUtil.AreClose((ChildWidth + child.Margin.Left + child.Margin.Right), root.ActualWidth) || !DoubleUtil.AreClose((ChildHeight + child.Margin.Top + child.Margin.Bottom), root.ActualHeight))
                        {
                            Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                        }
                        break;

                    case "SingleChildLCFfalse":
                        child = _root.Children[0] as FrameworkElement;
                        childPosition = LayoutUtility.GetElementPosition(child, _root);

                        _childTransform = VisualTreeHelper.GetTransform(child);
                        _getSquareValue = ((child.ActualHeight * child.ActualHeight) + (child.ActualWidth * child.ActualWidth));

                        _rotatedValue = Math.Sqrt(_getSquareValue);

                        _childWidth = _rotatedValue;
                        _childHeight = _rotatedValue;
                        //ChildTransformedY = childPosition.Y - (ChildHeight - child.ActualHeight);
                        //ChildTransformedX = childPosition.X + (ChildWidth - child.ActualWidth);

                        if (!DoubleUtil.AreClose(childPosition.Y, ((_root.ActualHeight / 2) - (_childHeight / 2))) || !DoubleUtil.AreClose(childPosition.X, ((_childWidth / 2) + child.Margin.Left)))
                        {
                            Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                        }
                        break;

                    case "SingleDock":
                        bool result = true;
                        foreach (UIElement c in _root.Children)
                        {
                            if (result)
                            {
                                child = c as FrameworkElement;

                                _childTransform = VisualTreeHelper.GetTransform(child);
                                _getSquareValue = ((child.ActualHeight * child.ActualHeight) + (child.ActualWidth * child.ActualWidth));

                                _rotatedValue = Math.Sqrt(_getSquareValue);

                                _childWidth = _rotatedValue;
                                _childHeight = _rotatedValue;

                                result = ValidateSingleDock(child, true);
                            }
                        }
                        if (!result)
                        {
                            if (child.Margin.Left != 0)
                            {
                                Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + " " + DockPanel.GetDock(child).ToString() + ", Margin On Child " + child.Margin.ToString() + ".");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + " " + DockPanel.GetDock(child).ToString() + ".");
                                _tempresult = false;
                            }
                        }
                        else
                        {
                            if (child.Margin.Left != 0)
                            {
                                Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + " " + DockPanel.GetDock(child).ToString() + ", Margin On Child " + child.Margin.ToString() + ".");
                            }
                            else
                            {
                                Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + " " + DockPanel.GetDock(child).ToString() + ".");
                            }
                        }
                        _offsetLeft = 0;
                        _offsetRight = 0;
                        _offsetTop = 0;
                        _offsetBottom = 0;
                        break;

                    case "MultipleDock":
                        bool mResult = true;
                        foreach (UIElement c in _root.Children)
                        {
                            if (mResult)
                            {
                                child = c as FrameworkElement;
                                mResult = ValidateMultipleDock(child, false);
                            }
                        }
                        if (!mResult)
                        {
                            if (child.Margin.Left != 0)
                            {
                                Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + " , Margin On Child " + child.Margin.ToString() + ".");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("FAIL. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                                _tempresult = false;
                            }
                        }
                        else
                        {
                            if (child.Margin.Left != 0)
                            {
                                Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + " , Margin On Child " + child.Margin.ToString() + ".");
                            }
                            else
                            {
                                Helpers.Log("PASS. DockPanelDockingScenario : " + DockPanelDockingScenario + ".");
                            }
                        }
                        _offsetLeft = 0;
                        _offsetRight = 0;
                        _offsetTop = 0;
                        _offsetBottom = 0;
                        break;
                }

            }
            CommonFunctionality.FlushDispatcher();
            _root.Children.Clear();
        }

        private double _offsetLeft = 0;
        private double _offsetRight = 0;
        private double _offsetTop = 0;
        private double _offsetBottom = 0;

        private bool ValidateSingleDock(FrameworkElement child, bool UseRotateSize)
        {
            int index = _root.Children.IndexOf(child);
            Point ChildPosition = LayoutUtility.GetElementPosition(child, _root);

            if (DockPanel.GetDock(child) == Dock.Left)
            {
                if (!UseRotateSize)
                {
                    if (!DoubleUtil.AreClose((ChildPosition.X - child.Margin.Left), _offsetLeft))
                    //if ((ChildPosition.X - child.Margin.Left) != OffsetLeft)
                    {
                        Helpers.Log("Child Position.X was not equal to the Left Offset");
                        return false;
                    }
                    if (Double.IsNaN(child.Height))
                    {
                        if (!DoubleUtil.AreClose((child.ActualHeight + child.Margin.Top + child.Margin.Bottom), (_root.ActualHeight - _offsetTop - _offsetBottom)))
                        //if ((child.ActualHeight + child.Margin.Top + child.Margin.Bottom) != (root.ActualHeight - OffsetTop - OffsetBottom))
                        {
                            Helpers.Log("Child Height was not equal to the height remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.Y, (((_root.ActualHeight - _offsetTop - _offsetBottom) / 2) - (child.ActualHeight / 2))))
                        //if (ChildPosition.Y != (((root.ActualHeight - OffsetTop - OffsetBottom) / 2) - (child.ActualHeight / 2)))
                        {
                            Helpers.Log("Child was not vertically centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    _offsetLeft += child.ActualWidth + child.Margin.Left + child.Margin.Right;
                    return true;
                }
                else
                {
                    //here
                    if (!DoubleUtil.AreClose(Math.Round(ChildPosition.X - ((_childWidth / 2) + child.Margin.Left)), Math.Round(_offsetLeft)))
                    //if ((ChildPosition.X - child.Margin.Left) != OffsetLeft)
                    {
                        Helpers.Log("Child Position.X was not equal to the Left Offset");
                        return false;
                    }
                    if (Double.IsNaN(child.Height))
                    {
                        if (!DoubleUtil.AreClose((_childHeight + child.Margin.Top + child.Margin.Bottom), (_root.ActualHeight - _offsetTop - _offsetBottom)))
                        //if ((child.ActualHeight + child.Margin.Top + child.Margin.Bottom) != (root.ActualHeight - OffsetTop - OffsetBottom))
                        {
                            Helpers.Log("Child Height was not equal to the height remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.Y, (((_root.ActualHeight - _offsetTop - _offsetBottom) / 2) - (_childHeight / 2))))
                        //if (ChildPosition.Y != (((root.ActualHeight - OffsetTop - OffsetBottom) / 2) - (child.ActualHeight / 2)))
                        {
                            Helpers.Log("Child was not vertically centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    _offsetLeft += _childWidth + child.Margin.Left + child.Margin.Right;
                    return true;
                }
            }
            else if (DockPanel.GetDock(child) == Dock.Right)
            {
                if (!UseRotateSize)
                {
                    if (!DoubleUtil.AreClose((_root.ActualWidth - (child.ActualWidth + child.Margin.Right + _offsetRight)), ChildPosition.X))
                    //if (root.ActualWidth - (child.ActualWidth + child.Margin.Right + OffsetRight) != ChildPosition.X)
                    {
                        Helpers.Log("Child Position.X was not equal to the Right Offset + Child Width");
                        return false;
                    }
                    if (Double.IsNaN(child.Height))
                    {
                        if (!DoubleUtil.AreClose((child.ActualHeight + child.Margin.Top + child.Margin.Bottom), (_root.ActualHeight - _offsetTop - _offsetBottom)))
                        //if ((child.ActualHeight + child.Margin.Top + child.Margin.Bottom) != (root.ActualHeight - OffsetTop - OffsetBottom))
                        {
                            Helpers.Log("Child Height was not equal to the height remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.Y, (((_root.ActualHeight - _offsetTop - _offsetBottom) / 2) - (child.ActualHeight / 2))))
                        //if (ChildPosition.Y != (((root.ActualHeight - OffsetTop - OffsetBottom) / 2) - (child.ActualHeight / 2)))
                        {
                            Helpers.Log("Child was not vertically centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    _offsetRight += child.ActualWidth + child.Margin.Left + child.Margin.Right;
                    return true;
                }
                else
                {
                    if (index != (_root.Children.Count - 1))
                    {
                        if (!DoubleUtil.AreClose((_root.ActualWidth - ((_childWidth / 2) + child.Margin.Right + _offsetRight)), ChildPosition.X))
                        //if (root.ActualWidth - (child.ActualWidth + child.Margin.Right + OffsetRight) != ChildPosition.X)
                        {
                            Helpers.Log("Child Position.X was not equal to the Right Offset + Child Width");
                            return false;
                        }
                        if (Double.IsNaN(child.Height))
                        {
                            if (!DoubleUtil.AreClose((_childHeight + child.Margin.Top + child.Margin.Bottom), (_root.ActualHeight - _offsetTop - _offsetBottom)))
                            //if ((child.ActualHeight + child.Margin.Top + child.Margin.Bottom) != (root.ActualHeight - OffsetTop - OffsetBottom))
                            {
                                Helpers.Log("Child Height was not equal to the height remaining in the DockPanel.");
                                return false;
                            }
                        }
                        else
                        {
                            if (!DoubleUtil.AreClose(ChildPosition.Y, (((_root.ActualHeight - _offsetTop - _offsetBottom) / 2) - (_childHeight / 2))))
                            //if (ChildPosition.Y != (((root.ActualHeight - OffsetTop - OffsetBottom) / 2) - (child.ActualHeight / 2)))
                            {
                                Helpers.Log("Child was not vertically centered in layout slot in the DockPanel.");
                                return false;
                            }
                        }
                    }
                    _offsetRight += _childWidth + child.Margin.Left + child.Margin.Right;
                    return true;
                }
            }
            else if (DockPanel.GetDock(child) == Dock.Top)
            {
                if (!UseRotateSize)
                {
                    if (!DoubleUtil.AreClose((ChildPosition.Y - child.Margin.Top), _offsetTop))
                    //if ((ChildPosition.Y - child.Margin.Top) != OffsetTop)
                    {
                        Helpers.Log("Child Position.Y was not equal to the Top Offset");
                        return false;
                    }
                    if (Double.IsNaN(child.Width))
                    {
                        if (!DoubleUtil.AreClose((child.ActualWidth + child.Margin.Left + child.Margin.Right), (_root.ActualWidth - _offsetLeft - _offsetRight)))
                        //if ((child.ActualWidth + child.Margin.Left + child.Margin.Right) != (root.ActualWidth - OffsetLeft - OffsetRight))
                        {
                            Helpers.Log("Child Width was not equal to the Width remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.X, (((_root.ActualWidth - _offsetLeft - _offsetRight) / 2) - (child.ActualWidth / 2))))
                        //if (ChildPosition.X != (((root.ActualWidth - OffsetLeft - OffsetRight) / 2) - (child.ActualWidth / 2)))
                        {
                            Helpers.Log("Child was not horizontally centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    if (child.GetType().Name == "StackPanel")
                    {
                        _offsetTop += child.DesiredSize.Height;
                    }
                    else
                    {
                        _offsetTop += child.ActualHeight + child.Margin.Top + child.Margin.Bottom;
                    }
                    return true;
                }
                else
                {
                    if (!DoubleUtil.AreClose((ChildPosition.Y - child.Margin.Top), _offsetTop))
                    //if ((ChildPosition.Y - child.Margin.Top) != OffsetTop)
                    {
                        Helpers.Log("Child Position.Y was not equal to the Top Offset");
                        return false;
                    }
                    if (Double.IsNaN(child.Width))
                    {
                        if (!DoubleUtil.AreClose((_childWidth + child.Margin.Left + child.Margin.Right), (_root.ActualWidth - _offsetLeft - _offsetRight)))
                        //if ((child.ActualWidth + child.Margin.Left + child.Margin.Right) != (root.ActualWidth - OffsetLeft - OffsetRight))
                        {
                            Helpers.Log("Child Width was not equal to the Width remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.X, (((_root.ActualWidth - _offsetLeft - _offsetRight) / 2))))
                        //if (ChildPosition.X != (((root.ActualWidth - OffsetLeft - OffsetRight) / 2) - (child.ActualWidth / 2)))
                        {
                            Helpers.Log("Child was not horizontally centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    if (child.GetType().Name == "StackPanel")
                    {
                        _offsetTop += child.DesiredSize.Height;
                    }
                    else
                    {
                        _offsetTop += _childHeight + child.Margin.Top + child.Margin.Bottom;
                    }
                    return true;
                }
            }
            else if (DockPanel.GetDock(child) == Dock.Bottom)
            {
                double childHeightValue = 0;
                if (child.GetType().Name == "StackPanel")
                {
                    childHeightValue = child.DesiredSize.Height - child.Margin.Top;
                }
                else
                {
                    if (!UseRotateSize)
                    {
                        childHeightValue = child.ActualHeight + child.Margin.Bottom;
                    }
                    else
                    {
                        childHeightValue = _childHeight + child.Margin.Bottom;
                    }
                }
                if (!UseRotateSize)
                {
                    if (!DoubleUtil.AreClose((_root.ActualHeight - (childHeightValue + _offsetBottom)), ChildPosition.Y))
                    //if (root.ActualHeight - (childHeightValue + OffsetBottom) != ChildPosition.Y)
                    {
                        Helpers.Log("Child Position.Y was not equal to the Bottom Offset + Child Height");
                        return false;
                    }
                    if (Double.IsNaN(child.Width))
                    {
                        if (!DoubleUtil.AreClose((child.ActualWidth + child.Margin.Left + child.Margin.Right), (_root.ActualWidth - _offsetLeft - _offsetRight)))
                        //if ((ChildWidth + child.Margin.Left + child.Margin.Right) != (root.ActualWidth - OffsetLeft - OffsetRight))
                        {
                            Helpers.Log("Child Width was not equal to the width remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.X, (((_root.ActualWidth - _offsetLeft - _offsetRight) / 2) - (child.ActualWidth / 2))))
                        //if (ChildPosition.X != (((root.ActualWidth - OffsetLeft - OffsetRight) / 2) - (ChildWidth / 2)))
                        {
                            Helpers.Log("Child was not horizontally centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    if (child.GetType().Name == "StackPanel")
                    {
                        _offsetBottom += childHeightValue + child.Margin.Top;
                    }
                    else
                    {
                        _offsetBottom += child.ActualHeight + child.Margin.Top + child.Margin.Bottom;
                    }
                    return true;
                }
                else
                {
                    if (index != (_root.Children.Count - 1))
                    {
                        if (!DoubleUtil.AreClose((_root.ActualHeight - (childHeightValue + _offsetBottom)), ChildPosition.Y))
                        //if (root.ActualHeight - (childHeightValue + OffsetBottom) != ChildPosition.Y)
                        {
                            Helpers.Log("Child Position.Y was not equal to the Bottom Offset + Child Height");
                            return false;
                        }
                        if (Double.IsNaN(child.Width))
                        {
                            if (!DoubleUtil.AreClose((_childWidth + child.Margin.Left + child.Margin.Right), (_root.ActualWidth - _offsetLeft - _offsetRight)))
                            //if ((ChildWidth + child.Margin.Left + child.Margin.Right) != (root.ActualWidth - OffsetLeft - OffsetRight))
                            {
                                Helpers.Log("Child Width was not equal to the width remaining in the DockPanel.");
                                return false;
                            }
                        }
                        else
                        {
                            if (!DoubleUtil.AreClose(ChildPosition.X, (((_root.ActualWidth - _offsetLeft - _offsetRight) / 2))))
                            //if (ChildPosition.X != (((root.ActualWidth - OffsetLeft - OffsetRight) / 2) - (ChildWidth / 2)))
                            {
                                Helpers.Log("Child was not horizontally centered in layout slot in the DockPanel.");
                                return false;
                            }
                        }
                    }
                    if (child.GetType().Name == "StackPanel")
                    {
                        _offsetBottom += childHeightValue + child.Margin.Top;
                    }
                    else
                    {
                        _offsetBottom += _childHeight + child.Margin.Top + child.Margin.Bottom;
                    }
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool ValidateMultipleDock(FrameworkElement child, bool UseRotateSize)
        {
            int index = _root.Children.IndexOf(child);

            if (index != _root.Children.Count - 1)
            {
                Point ChildPosition = LayoutUtility.GetElementPosition(child, _root);

                if (DockPanel.GetDock(child) == Dock.Left)
                {
                    if (!DoubleUtil.AreClose((ChildPosition.X - child.Margin.Left), _offsetLeft))
                    //if ((ChildPosition.X - child.Margin.Left) != OffsetLeft)
                    {
                        Helpers.Log("Child Position.X was not equal to the Left Offset");
                        return false;
                    }
                    if (Double.IsNaN(child.Height))
                    {
                        if (!DoubleUtil.AreClose((child.ActualHeight + child.Margin.Top + child.Margin.Bottom), (_root.ActualHeight - _offsetTop - _offsetBottom)))
                        //if ((child.ActualHeight + child.Margin.Top + child.Margin.Bottom) != (root.ActualHeight - OffsetTop - OffsetBottom))
                        {
                            Helpers.Log("Child Height was not equal to the height remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.Y, (((_root.ActualHeight - _offsetTop - _offsetBottom) / 2) - (child.ActualHeight / 2))))
                        //if (ChildPosition.Y != (((root.ActualHeight - OffsetTop - OffsetBottom) / 2) - (child.ActualHeight / 2)))
                        {
                            Helpers.Log("Child was not vertically centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    _offsetLeft += child.ActualWidth + child.Margin.Left + child.Margin.Right;
                    return true;
                }
                else if (DockPanel.GetDock(child) == Dock.Right)
                {
                    if (!DoubleUtil.AreClose((_root.ActualWidth - (child.ActualWidth + child.Margin.Right + _offsetRight)), ChildPosition.X))
                    //                    if (root.ActualWidth - (child.ActualWidth + child.Margin.Right + OffsetRight) != ChildPosition.X)
                    {
                        Helpers.Log("Child Position.X was not equal to the Right Offset + Child Width");
                        return false;
                    }
                    if (Double.IsNaN(child.Height))
                    {
                        if (!DoubleUtil.AreClose((child.ActualHeight + child.Margin.Top + child.Margin.Bottom), (_root.ActualHeight - _offsetTop - _offsetBottom)))
                        //                        if ((child.ActualHeight + child.Margin.Top + child.Margin.Bottom) != (root.ActualHeight - OffsetTop - OffsetBottom))
                        {
                            Helpers.Log("Child Height was not equal to the height remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.Y, (((_root.ActualHeight - _offsetTop - _offsetBottom) / 2) - (child.ActualHeight / 2))))
                        //                        if (ChildPosition.Y != (((root.ActualHeight - OffsetTop - OffsetBottom) / 2) - (child.ActualHeight / 2)))
                        {
                            Helpers.Log("Child was not vertically centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    _offsetRight += child.ActualWidth + child.Margin.Left + child.Margin.Right;
                    return true;
                }
                else if (DockPanel.GetDock(child) == Dock.Top)
                {
                    if (!DoubleUtil.AreClose((ChildPosition.Y - child.Margin.Top), _offsetTop))
                    //                    if ((ChildPosition.Y - child.Margin.Top) != OffsetTop)
                    {
                        Helpers.Log("Child Position.Y was not equal to the Top Offset");
                        return false;
                    }
                    if (Double.IsNaN(child.Width))
                    {
                        if (!DoubleUtil.AreClose((child.ActualWidth + child.Margin.Left + child.Margin.Right), (_root.ActualWidth - _offsetLeft - _offsetRight)))
                        //                        if ((child.ActualWidth + child.Margin.Left + child.Margin.Right) != (root.ActualWidth - OffsetLeft - OffsetRight))
                        {
                            Helpers.Log("Child Width was not equal to the Width remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.X, (((_root.ActualWidth - _offsetLeft - _offsetRight) / 2) - (child.ActualWidth / 2))))
                        //if (ChildPosition.X != (((root.ActualWidth - OffsetLeft - OffsetRight) / 2) - (child.ActualWidth / 2)))
                        {
                            Helpers.Log("Child was not horizontally centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    if (child.GetType().Name == "StackPanel")
                    {
                        _offsetTop += child.DesiredSize.Height;
                    }
                    else
                    {
                        _offsetTop += child.ActualHeight + child.Margin.Top + child.Margin.Bottom;
                    }
                    return true;
                }
                else if (DockPanel.GetDock(child) == Dock.Bottom)
                {
                    double childHeightValue = 0;
                    if (child.GetType().Name == "StackPanel")
                    {
                        childHeightValue = child.DesiredSize.Height - child.Margin.Top;
                    }
                    else
                    {
                        childHeightValue = child.ActualHeight + child.Margin.Bottom;
                    }

                    if (!DoubleUtil.AreClose((_root.ActualHeight - (childHeightValue + _offsetBottom)), ChildPosition.Y))
                    //                    if (root.ActualHeight - (childHeightValue + OffsetBottom) != ChildPosition.Y)
                    {
                        Helpers.Log("Child Position.Y was not equal to the Bottom Offset + Child Height");
                        return false;
                    }
                    if (Double.IsNaN(child.Width))
                    {
                        if (!DoubleUtil.AreClose((child.ActualWidth + child.Margin.Left + child.Margin.Right), (_root.ActualWidth - _offsetLeft - _offsetRight)))
                        //                        if ((ChildWidth + child.Margin.Left + child.Margin.Right) != (root.ActualWidth - OffsetLeft - OffsetRight))
                        {
                            Helpers.Log("Child Width was not equal to the width remaining in the DockPanel.");
                            return false;
                        }
                    }
                    else
                    {
                        if (!DoubleUtil.AreClose(ChildPosition.X, (((_root.ActualWidth - _offsetLeft - _offsetRight) / 2) - (child.ActualWidth / 2))))
                        //                        if (ChildPosition.X != (((root.ActualWidth - OffsetLeft - OffsetRight) / 2) - (ChildWidth / 2)))
                        {
                            Helpers.Log("Child was not horizontally centered in layout slot in the DockPanel.");
                            return false;
                        }
                    }
                    if (child.GetType().Name == "StackPanel")
                    {
                        _offsetBottom += childHeightValue + child.Margin.Top;
                    }
                    else
                    {
                        _offsetBottom += child.ActualHeight + child.Margin.Top + child.Margin.Bottom;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!DoubleUtil.AreClose((child.ActualHeight + child.Margin.Bottom + child.Margin.Top), (_root.ActualHeight - _offsetBottom - _offsetTop)) || !DoubleUtil.AreClose((child.ActualWidth + child.Margin.Left + child.Margin.Right), (_root.ActualWidth - _offsetLeft - _offsetRight)))
                //                if (child.ActualHeight + child.Margin.Bottom + child.Margin.Top != root.ActualHeight  - OffsetBottom - OffsetTop || child.ActualWidth + child.Margin.Left + child.Margin.Right != root.ActualWidth - OffsetLeft - OffsetRight)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    [Test(1, "Panels.DockPanel", "DockPanelDockRelayout", Variables = "Area=ElementLayout")]
    public class DockPanelDockRelayout : CodeTest
    {


        public DockPanelDockRelayout()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;

            this.window.Content = this.TestContent();
        }

        DockPanel _root;
        DockPanel _child;
        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new DockPanel();
            _root.Background = Brushes.RosyBrown;
            _root.Height = 600;
            _root.Width = 600;
            _root.LastChildFill = true;

            _child = new DockPanel();
            _child.Background = Brushes.Gray;
            _child.LastChildFill = false;

            _rect = new Rectangle();
            _rect.Fill = Brushes.DarkOrange;
            _rect.Height = 100;
            _rect.Width = 100;

            _child.Children.Add(_rect);

            DockPanel.SetDock(_child, Dock.Left);

            _root.Children.Add(_child);

            return _root;
        }

        int _dockTest = 0;

        public override void TestActions()
        {
            switch (_dockTest)
            {
                case 0:
                    Helpers.Log("Changing Dock property from Fill to Dock.Left.");
                    _root.LastChildFill = false;
                    DockPanel.SetDock(_child, Dock.Left);
                    CommonFunctionality.FlushDispatcher();
                    _dockTest++;
                    verifyLayout();
                    break;

                case 1:
                    Helpers.Log("Changing Dock property from Dock.Left to Dock.Right.");
                    DockPanel.SetDock(_child, Dock.Right);
                    CommonFunctionality.FlushDispatcher();
                    _dockTest++;
                    verifyLayout();
                    break;

                case 2:
                    Helpers.Log("Changing Dock property from Dock.Right to Dock.Bottom.");
                    DockPanel.SetDock(_child, Dock.Bottom);
                    CommonFunctionality.FlushDispatcher();
                    _dockTest++;
                    verifyLayout();
                    break;

                case 3:
                    Helpers.Log("Changing Dock property from Dock.Bottom to Dock.Top.");
                    DockPanel.SetDock(_child, Dock.Top);
                    CommonFunctionality.FlushDispatcher();
                    _dockTest++;
                    verifyLayout();
                    break;

                case 4:
                    Helpers.Log("Changing Dock property from Dock.Top to Dock.Fill.");
                    _root.LastChildFill = true;
                    CommonFunctionality.FlushDispatcher();
                    _dockTest++;
                    verifyLayout();
                    break;
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

        void verifyLayout()
        {
            double childH = _child.RenderSize.Height;
            double childW = _child.RenderSize.Width;
            double parentH = _root.RenderSize.Height;
            double parentW = _root.RenderSize.Width;
            double contentH = _rect.RenderSize.Height;
            double contentW = _rect.RenderSize.Width;
            PointHitTestResult result = (PointHitTestResult)VisualTreeHelper.HitTest(_child, new Point(0, 0));
            Matrix pt;
            System.Windows.Media.GeneralTransform gt = _child.TransformToAncestor(_root);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if (t == null)
            {
                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }
            pt = t.Value;

            if (_root.LastChildFill == false)
            {
                if ((DockPanel.GetDock(_child).ToString()) == "Top")
                {
                    Helpers.Log("Verify Dock.Top.");
                    if (childH != contentH || childW != parentW || pt.OffsetX != 0 || pt.OffsetY != 0)
                    {
                        Helpers.Log("Test failed verifying Dock.Top prop.");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log("Layout for Dock.Top is correct.");
                        this.TestActions();
                    }
                }

                if ((DockPanel.GetDock(_child).ToString()) == "Bottom")
                {
                    Helpers.Log("Verify Dock.Bottom.");
                    if (childH != contentH || childW != parentW || pt.OffsetX != 0 || pt.OffsetY != (parentH - childH))
                    {
                        Helpers.Log("Test failed verifying Dock.Bottom prop.");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log("Layout for Dock.Bottom is correct.");
                        this.TestActions();
                    }
                }
                if ((DockPanel.GetDock(_child).ToString()) == "Left")
                {
                    Helpers.Log("Verify Dock.Left.");
                    if (childH != parentH || childW != contentW || pt.OffsetX != 0 || pt.OffsetY != 0)
                    {
                        Helpers.Log("Test failed verifying Dock.Left prop.");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log("Layout for Dock.Left is correct.");
                        this.TestActions();
                    }
                }

                if ((DockPanel.GetDock(_child).ToString()) == "Right")
                {
                    Helpers.Log("Verify Dock.Right.");
                    if (childH != parentH || childW != contentW || pt.OffsetX != (parentW - childW) || pt.OffsetY != 0)
                    {
                        Helpers.Log(pt.OffsetX + " " + pt.OffsetY);
                        Helpers.Log("Test failed verifying Dock.Right prop.");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log("Layout for Dock.Right is correct.");
                        this.TestActions();
                    }
                }
            }
            else
            {
                Helpers.Log("Verify Dock.Fill.");
                if (childH != parentH || childW != parentW || pt.OffsetX != 0 || pt.OffsetY != 0)
                {
                    Helpers.Log("Test failed verifying Dock.Fill prop.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("Layout for Dock.Fill is correct.");
                }
            }
        }
    }

    [Test(1, "Panels.DockPanel", "DockPanelResize3", Variables = "Area=ElementLayout")]
    public class DockPanelResize3 : CodeTest
    {
        public DockPanelResize3()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        DockPanel _dock;
        DockPanel _right;
        DockPanel _bottom;
        Border _fill;
        int _count = 0;

        public override FrameworkElement TestContent()
        {
            _dock = new DockPanel();
            _dock.Background = Brushes.Pink;
            _dock.LastChildFill = true;

            _right = new DockPanel();
            _right.Background = Brushes.Crimson;
            _right.Width = 100;
            DockPanel.SetDock(_right, Dock.Right);

            _bottom = new DockPanel();
            _bottom.Background = Brushes.CornflowerBlue;
            _bottom.Height = 100;
            DockPanel.SetDock(_bottom, Dock.Bottom);

            _fill = new Border();
            _fill.Background = Brushes.Orange;

            _dock.Children.Add(_right);
            _dock.Children.Add(_bottom);
            _dock.Children.Add(_fill);

            return _dock;
        }

        public override void TestActions()
        {
            this.window.Height = this.window.RenderSize.Height / 2;
            this.window.Width = this.window.RenderSize.Width / 2;
            this._count++;

            CommonFunctionality.FlushDispatcher();

            if (!verifySize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            this.window.Height = this.window.RenderSize.Height * 3;
            this.window.Width = this.window.RenderSize.Width * 3;
            this._count++;

            CommonFunctionality.FlushDispatcher();

            if (!verifySize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            this.window.Height = 200;
            this.window.Width = 1000;
            this._count++;

            CommonFunctionality.FlushDispatcher();

            if (!verifySize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            this.window.Height = 1000;
            this.window.Width = 200;
            this._count++;

            CommonFunctionality.FlushDispatcher();

            if (!verifySize())
            {
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            this.window.Height = 500;
            this.window.Width = 500;
            this._count++;

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

        bool verifySize()
        {
            double parentH = _dock.RenderSize.Height;
            double parentW = _dock.RenderSize.Width;
            double rightH = _right.RenderSize.Height;
            double rightW = _right.RenderSize.Width;
            double bottomH = _bottom.RenderSize.Height;
            double bottomW = _bottom.RenderSize.Width;
            double fillH = _fill.RenderSize.Height;
            double fillW = _fill.RenderSize.Width;

            Helpers.Log(string.Format("PARENT : {0},{1}", parentH, parentW));
            Helpers.Log(string.Format("RIGHT  : {0},{1}", rightH, rightW));
            Helpers.Log(string.Format("BOTTOM : {0},{1}", bottomH, bottomW));
            Helpers.Log(string.Format("FILL   : {0},{1}", fillH, fillW));

            //verify size of panel docked right
            if (!DoubleUtil.AreClose(rightH, parentH))
            {
                Helpers.Log(string.Format("test failed in case {0}.  right panel height is wrong", this._count));
                return false;
            }

            if (!DoubleUtil.AreClose(rightW, (parentW - fillW)))
            {
                Helpers.Log(string.Format("test failed in case {0}.  right panel width is wrong", this._count));
                return false;
            }

            //verify size of panel docked bottom
            if (!DoubleUtil.AreClose(bottomH, (parentH - fillH)))
            {
                Helpers.Log(string.Format("test failed in case {0}.  bottom panel height is wrong", this._count));
                return false;
            }

            if (!DoubleUtil.AreClose(bottomW, (parentW - rightW)))
            {
                Helpers.Log(string.Format("test failed in case {0}.  bottom panel width is wrong", this._count));
                return false;
            }

            //verify size of panel docked fill
            if (!DoubleUtil.AreClose(fillH, (parentH - bottomH)))
            {
                Helpers.Log(string.Format("test failed in case {0}.  fill panel height is wrong", this._count));
                return false;
            }

            if (!DoubleUtil.AreClose(fillW, (parentW - rightW)))
            {
                Helpers.Log(string.Format("test failed in case {0}.  fill panel width is wrong", this._count));
                return false;
            }

            Helpers.Log(string.Format("test failed in case {0}", this._count));
            return true;
        }
    }

    [Test(1, "Panels.DockPanel", "DockPanelResize4", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class DockPanelResize4 : CodeTest
    {
        public DockPanelResize4()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            DockPanel dock = new DockPanel();
            dock.LastChildFill = true;
            DockPanel one = new DockPanel();

            one.Margin = new Thickness(5);
            one.Background = Brushes.Crimson;
            one.Width = 100;
            DockPanel.SetDock(one, Dock.Left);

            DockPanel two = new DockPanel();

            two.Margin = new Thickness(5);
            two.Background = Brushes.CornflowerBlue;
            two.Height = 100;
            DockPanel.SetDock(two, Dock.Bottom);

            DockPanel three = new DockPanel();

            three.Margin = new Thickness(5);
            three.Background = Brushes.Orange;
            three.Height = 50;
            DockPanel.SetDock(three, Dock.Top);

            DockPanel four = new DockPanel();

            four.Margin = new Thickness(5);
            four.Background = Brushes.Salmon;
            four.Width = 100;
            DockPanel.SetDock(four, Dock.Right);

            DockPanel five = new DockPanel();

            five.Margin = new Thickness(5);
            five.Background = Brushes.Silver;
            //DockPanel.SetDock(five, Dock.Fill);
            dock.Children.Add(one);
            dock.Children.Add(two);
            dock.Children.Add(three);
            dock.Children.Add(four);
            dock.Children.Add(five);
            return dock;
        }

        public override void TestActions()
        {

            this.window.Height = this.window.RenderSize.Height / 2;
            this.window.Width = this.window.RenderSize.Width / 2;
            CommonFunctionality.FlushDispatcher();
            this.window.Height = this.window.RenderSize.Height * 3;
            this.window.Width = this.window.RenderSize.Width * 3;
            CommonFunctionality.FlushDispatcher();
            this.window.Height = 200;
            this.window.Width = 1000;
            CommonFunctionality.FlushDispatcher();
            this.window.Height = 1000;
            this.window.Width = 200;
            CommonFunctionality.FlushDispatcher();
            this.window.Height = 500;
            this.window.Width = 500;
            CommonFunctionality.FlushDispatcher();
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();
            VScanCommon tool = new VScanCommon(this);
            if (!tool.CompareImage())
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }

    [Test(1, "Panels.DockPanel", "DockPanelPropertyAttributePair", Variables = "Area=ElementLayout")]
    public class DockPanelPropertyAttributePair : CodeTest
    {
        public DockPanelPropertyAttributePair()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;

            this.window.Content = this.TestContent();
        }

        DockPanel _parent;
        DockPanel _child;
        Rectangle _staticcontent;
        Border _fillcontent;

        bool _hasChild = false;

        static double s_expectedPixel = 225;

        public override FrameworkElement TestContent()
        {
            _parent = new DockPanel();
            _parent.Background = Brushes.SteelBlue;

            _child = new DockPanel();
            _child.Background = Brushes.DarkOrange;

            _parent.Children.Add(_child);
            return _parent;
        }

        public override void TestActions()
        {
            Helpers.Log("Starting Tests for Height & Width with Percent, Pixel, and Auto values");

            CommonFunctionality.FlushDispatcher();
            _parent.LastChildFill = false;
            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            DockPanel.SetDock(_child, Dock.Left);
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            DockPanel.SetDock(_child, Dock.Right);
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            DockPanel.SetDock(_child, Dock.Top);
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            DockPanel.SetDock(_child, Dock.Bottom);
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            _parent.LastChildFill = true;
            CommonFunctionality.FlushDispatcher();
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();
            _parent.LastChildFill = false;

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            _hasChild = true;
            DockPanel.SetDock(_child, Dock.Left);
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            _hasChild = true;
            DockPanel.SetDock(_child, Dock.Right);
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            _hasChild = true;
            DockPanel.SetDock(_child, Dock.Top);
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            _hasChild = true;
            DockPanel.SetDock(_child, Dock.Bottom);
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();

            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            _hasChild = true;
            _parent.LastChildFill = true;
            CommonFunctionality.FlushDispatcher();
            VerifyAuto();

            CommonFunctionality.FlushDispatcher();
            _parent.LastChildFill = false;

            _child.Height = s_expectedPixel;
            _child.Width = s_expectedPixel;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            DockPanel.SetDock(_child, Dock.Left);
            VerifyPixel();

            CommonFunctionality.FlushDispatcher();

            _child.Height = s_expectedPixel;
            _child.Width = s_expectedPixel;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            DockPanel.SetDock(_child, Dock.Right);
            VerifyPixel();

            CommonFunctionality.FlushDispatcher();

            _child.Height = s_expectedPixel;
            _child.Width = s_expectedPixel;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            DockPanel.SetDock(_child, Dock.Top);
            VerifyPixel();

            CommonFunctionality.FlushDispatcher();

            _child.Height = s_expectedPixel;
            _child.Width = s_expectedPixel;
            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            DockPanel.SetDock(_child, Dock.Bottom);
            VerifyPixel();

            CommonFunctionality.FlushDispatcher();


            _child.Children.Clear();
            _child.Children.Add(StaticContent());
            _parent.LastChildFill = true;
            CommonFunctionality.FlushDispatcher();
            VerifyPixel();
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

        void VerifyPixel()
        {
            CommonFunctionality.FlushDispatcher();

            Point position = LayoutUtility.GetElementPosition(_child, _parent);
            Size ParentSize = _parent.RenderSize;
            Size ChildSize = _child.RenderSize;
            string dockprop = DockPanel.GetDock(_child).ToString();

            if (_parent.LastChildFill == false)
            {
                switch (dockprop)
                {
                    case "Left":
                        if (position.X != 0 || position.Y != ((ParentSize.Height / 2) - (ChildSize.Height / 2)) || _child.RenderSize.Height != s_expectedPixel || _child.RenderSize.Width != s_expectedPixel)
                        {
                            Helpers.Log("Layout for Pixel Size with Dock.Left failed in verify");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Layout for Pixel Size with Dock.Left was correct");
                        }

                        break;

                    case "Right":
                        if (position.X != (ParentSize.Width - ChildSize.Width) || position.Y != ((ParentSize.Height / 2) - (ChildSize.Height / 2)) || _child.RenderSize.Height != s_expectedPixel || _child.RenderSize.Width != s_expectedPixel)
                        {
                            Helpers.Log("Layout for Pixel Size with Dock.Right failed in verify");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Layout for Pixel Size with Dock.Right was correct");
                        }

                        break;

                    case "Top":
                        if (position.X != ((ParentSize.Width / 2) - (ChildSize.Width / 2)) || position.Y != 0 || _child.RenderSize.Height != s_expectedPixel || _child.RenderSize.Width != s_expectedPixel)
                        {
                            Helpers.Log("Layout for Pixel Size with Dock.Top failed in verify");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Layout for Pixel Size with Dock.Top and no child was correct");
                        }

                        break;

                    case "Bottom":
                        if (position.X != ((ParentSize.Width / 2) - (ChildSize.Width / 2)) || position.Y != (ParentSize.Height - ChildSize.Height) || _child.RenderSize.Height != s_expectedPixel || _child.RenderSize.Width != s_expectedPixel)
                        {
                            Helpers.Log("Layout for Pixel Size with Dock.Bottom failed in verify");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Layout for Pixel Size with Dock.Bottom was correct");
                        }

                        break;
                }
            }
            else
            {
                if (ChildSize.Height != s_expectedPixel || ChildSize.Width != s_expectedPixel || position.Y != ((ParentSize.Height / 2) - (ChildSize.Height / 2)) || position.X != ((ParentSize.Width / 2) - (ChildSize.Width / 2)))
                {
                    Helpers.Log("Layout for Pixel Size with Dock.Fill failed in verify");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("Layout for Pixel Size with Dock.Fill was correct");
                }

            }
        }

        void VerifyAuto()
        {
            CommonFunctionality.FlushDispatcher();

            Size ParentSize = _parent.RenderSize;
            Size ChildSize = _child.RenderSize;
            string dockprop = DockPanel.GetDock(_child).ToString();

            if (_hasChild == false)
            {
                if (_parent.LastChildFill == false)
                {
                    switch (dockprop)
                    {
                        case "Left":
                            if (ChildSize.Height != ParentSize.Height || ChildSize.Width != 0)
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Left and no child failed in verify");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Left and no child was correct");
                            }

                            break;

                        case "Right":
                            if (ChildSize.Height != ParentSize.Height || ChildSize.Width != 0)
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Right and no child failed in verify");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Right and no child was correct");
                            }

                            break;

                        case "Top":
                            if (ChildSize.Width != ParentSize.Width || ChildSize.Height != 0)
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Top and no child failed in verify");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Top and no child was correct");
                            }

                            break;

                        case "Bottom":
                            if (ChildSize.Width != ParentSize.Width || ChildSize.Height != 0)
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Bottom and no child failed in verify");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Bottom and no child was correct");
                            }

                            break;

                    }
                }
                else
                {
                    if (ChildSize.Height != ParentSize.Height || ChildSize.Width != ParentSize.Width)
                    {
                        Helpers.Log("Layout for Auto Size with Dock.Fill failed in verify");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log("Layout for Auto Size with Dock.Fill was correct");
                    }
                }
            }

            else
            {
                Size ContentSize = _staticcontent.RenderSize;

                if (_parent.LastChildFill == false)
                {
                    switch (dockprop)
                    {
                        case "Left":
                            if (ChildSize.Height != ParentSize.Height || ChildSize.Width != ContentSize.Width)
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Left with a child failed in verify");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Left with a child was correct");
                            }

                            break;

                        case "Right":
                            if (ChildSize.Height != ParentSize.Height || ChildSize.Width != ContentSize.Width)
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Right with a child failed in verify");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Right with a child was correct");
                            }

                            break;

                        case "Top":
                            if (ChildSize.Width != ParentSize.Width || ChildSize.Height != ContentSize.Height)
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Top with a child failed in verify");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Top with a child was correct");
                            }

                            break;

                        case "Bottom":
                            if (ChildSize.Width != ParentSize.Width || ChildSize.Height != ContentSize.Height)
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Bottom with a child failed in verify");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Layout for Auto Size with Dock.Bottom with a child was correct");
                            }

                            break;

                    }
                }
                else
                {
                    if (ChildSize.Height != ParentSize.Height || ChildSize.Width != ParentSize.Width)
                    {
                        Helpers.Log("Layout for Auto Size with Dock.Fill failed in verify");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log("Layout for Auto Size with Dock.Fill was correct");
                    }
                }
            }
        }

        FrameworkElement StaticContent()
        {
            _parent.LastChildFill = false;
            _staticcontent = CommonFunctionality.CreateRectangle(100, 100, Brushes.DarkGray);
            return _staticcontent;
        }

        FrameworkElement FillContent()
        {
            _parent.LastChildFill = true;
            _fillcontent = CommonFunctionality.CreateBorder(Brushes.Snow, double.NaN, double.NaN);
            return _fillcontent;
        }
    }

    [Test(1, "Panels.DockPanel", "DockPanelReLayoutWithMargins", Variables = "Area=ElementLayout")]
    public class DockPanelReLayoutWithMargins : CodeTest
    {


        public DockPanelReLayoutWithMargins()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 600;

            this.window.Content = this.TestContent();
        }

        DockPanel _root;
        DockPanel _child;
        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new DockPanel();
            _root.Background = Brushes.Crimson;
            _root.Width = 500;
            _root.Height = 500;
            _root.LastChildFill = true;

            _child = new DockPanel();
            _child.Background = Brushes.DarkGray;
            _child.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Margin = new Thickness(25);
            _child.LastChildFill = false;

            _rect = CommonFunctionality.CreateRectangle(100, 100, Brushes.DarkOrange);

            _child.Children.Add(_rect);
            _root.Children.Add(_child);
            return _root;
        }

        public override void TestActions()
        {
            string dockprop = DockPanel.GetDock(_child).ToString();

            if (_root.LastChildFill == true)
            {
                if ((_child.ActualWidth + _child.Margin.Left + _child.Margin.Right) != _root.ActualWidth || (_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom) != _root.ActualHeight)
                {
                    Helpers.Log("Dock.Fill + Margins Layout is incorrect.");
                    Helpers.Log("child   : Height = " + _child.ActualHeight + ", Width = " + _child.ActualWidth);
                    _tempresult = false;

                }
                else
                {
                    Helpers.Log("Dock.Fill + Margins Layout is correct.");
                    Helpers.Log("Switching to Dock.Left");
                    _root.LastChildFill = false;
                    DockPanel.SetDock(_child, Dock.Left);
                    CommonFunctionality.FlushDispatcher();
                    this.TestActions();
                    //                        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(Verify), null);

                }

            }
            else
            {
                switch (dockprop)
                {
                    case "Left":
                        if ((_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom) != _root.ActualHeight || _child.ActualWidth != _rect.ActualWidth)
                        {
                            Helpers.Log("Dock.Left + Margins Layout is incorrect.");
                            Helpers.Log("child   : Height = " + _child.ActualHeight + ", Width = " + _child.ActualWidth);
                            _tempresult = false;
                            //this.Shutdown();
                        }
                        else
                        {
                            Helpers.Log("Dock.Left + Margins Layout is correct.");
                            Helpers.Log("Switching to Dock.Right");
                            DockPanel.SetDock(_child, Dock.Right);
                            CommonFunctionality.FlushDispatcher();
                            this.TestActions();
                            //                                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(Verify), null);
                        }

                        break;

                    case "Right":
                        if ((_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom) != _root.ActualHeight || _child.ActualWidth != _rect.ActualWidth)
                        {
                            Helpers.Log("Dock.Right + Margins Layout is incorrect.");
                            Helpers.Log("child   : Height = " + _child.ActualHeight + ", Width = " + _child.ActualWidth);
                            _tempresult = false;
                            //this.Shutdown();
                        }
                        else
                        {
                            Helpers.Log("Dock.Right + Margins Layout is correct.");
                            Helpers.Log("Switching to Dock.Top");
                            DockPanel.SetDock(_child, Dock.Top);
                            CommonFunctionality.FlushDispatcher();
                            this.TestActions();
                            //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(Verify), null);
                        }

                        break;

                    case "Top":
                        if ((_child.ActualWidth + _child.Margin.Left + _child.Margin.Right) != _root.ActualWidth || _child.ActualHeight != _rect.ActualHeight)
                        {
                            Helpers.Log("Dock.Top + Margins Layout is incorrect.");
                            Helpers.Log("child   : Height = " + _child.ActualHeight + ", Width = " + _child.ActualWidth);
                            _tempresult = false;
                            //this.Shutdown();
                        }
                        else
                        {
                            Helpers.Log("Dock.Top + Margins Layout is correct.");
                            Helpers.Log("Switching to Dock.Bottom");
                            DockPanel.SetDock(_child, Dock.Bottom);
                            CommonFunctionality.FlushDispatcher();
                            this.TestActions();
                            //Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(Verify), null);
                        }

                        break;

                    case "Bottom":
                        if ((_child.ActualWidth + _child.Margin.Left + _child.Margin.Right) != _root.ActualWidth || _child.ActualHeight != _rect.ActualHeight)
                        {
                            Helpers.Log("Dock.Bottom + Margins Layout is incorrect.");
                            Helpers.Log("child   : Height = " + _child.ActualHeight + ", Width = " + _child.ActualWidth);
                            _tempresult = false;
                            //this.Shutdown();
                        }
                        else
                        {
                            Helpers.Log("Dock.Bottom + Margins Layout is correct.");
                            Helpers.Log("Layouts for all Dock properties + Margins were correct.");
                        }
                        break;
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
    }

    [Test(1, "Panels.DockPanel", "DockPanelGetDockSetDockException", Variables = "Area=ElementLayout")]
    public class DockPanelGetDockSetDockException : CodeTest
    {


        public DockPanelGetDockSetDockException()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 650;
            this.window.Width = 650;

            this.window.Content = this.TestContent();
        }

        DockPanel _root;
        Button _btn = null;
        public override FrameworkElement TestContent()
        {
            _root = new DockPanel();
            _root.Background = Brushes.CornflowerBlue;
            return _root;
        }

        public override void TestActions()
        {
            try
            {
                DockPanel.SetDock(_btn, Dock.Left);
            }
            catch (Exception e)
            {
                Helpers.Log(e.Message);
                if (e.GetType().ToString() != "System.ArgumentNullException")
                {
                    Helpers.Log("No exception or wrong exception caught with SetDock.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("SetDock exception test passed.");
                }
            }

            CommonFunctionality.FlushDispatcher();

            try
            {
                DockPanel.GetDock(_btn);
            }
            catch (Exception e)
            {
                Helpers.Log(e.Message);
                if (e.GetType().ToString() != "System.ArgumentNullException")
                {
                    Helpers.Log("No exception or wrong exception caught with GetDock.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("GetDock exception test passed.");
                }
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            if (!_tempresult)
            { this.Result = false; }
            else
            { this.Result = true; }
        }
    }

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeRectangle", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeRectangle : CodeTest
    {


        public DockPanelContentPropChangeRectangle()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.SlateBlue));
            _dock.Children.Add(_rect);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _rect.Width = _rect.ActualWidth * 2;
            _rect.Height = _rect.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeButton", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeButton : CodeTest
    {


        public DockPanelContentPropChangeButton()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        DockPanel _dock;

        Button _btn;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _btn = CommonFunctionality.CreateButton(200, 200, Brushes.Red);
            _dock.Children.Add(_btn);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);


            _btn.Width = _btn.ActualWidth * 2;
            _btn.Height = _btn.ActualHeight * 2;
            _btn.Content = "Button Size Changed~!";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeTextBox", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeTextBox : CodeTest
    {


        public DockPanelContentPropChangeTextBox()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        DockPanel _dock;

        TextBox _tbox;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _tbox = CommonFunctionality.CreateTextBox(150, 100, "Width=150px");
            _dock.Children.Add(_tbox);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _tbox.Width = _tbox.ActualWidth * 2;
            _tbox.Height = _tbox.ActualHeight * 2;
            _tbox.Text = "Size changed * 2";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeEllipse", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeEllipse : CodeTest
    {


        public DockPanelContentPropChangeEllipse()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        DockPanel _dock;

        Ellipse _elps;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _elps = new Ellipse();
            _elps.Width = 150;
            _elps.Height = 150;
            _elps.Fill = new SolidColorBrush(Colors.SlateBlue);
            _dock.Children.Add(_elps);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _elps.Width = _elps.ActualWidth * 2;
            _elps.Height = _elps.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeImage", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeImage : CodeTest
    {


        public DockPanelContentPropChangeImage()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        Image _img;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _img = CommonFunctionality.CreateImage("light.bmp");
            _img.Height = 50;
            _img.Width = 50;
            _dock.Children.Add(_img);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _img.Width = _img.ActualWidth * 2;
            _img.Height = _img.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeText", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeText : CodeTest
    {


        public DockPanelContentPropChangeText()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        TextBlock _txt;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _txt = CommonFunctionality.CreateText("Testing Grid...");
            _dock.Children.Add(_txt);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _txt.Text = "Changing Text to very large text... Changing Text to very large text...";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeBorder", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeBorder : CodeTest
    {


        public DockPanelContentPropChangeBorder()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        Border _b;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _b = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.SlateBlue), 25, 200);
            _dock.Children.Add(_b);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _b.Width = _b.ActualWidth * 2;
            _b.Height = _b.ActualHeight * 2;
            _b.BorderThickness = new Thickness(20);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeLabel", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeLabel : CodeTest
    {


        public DockPanelContentPropChangeLabel()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        Label _lbl;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lbl = new Label();
            _lbl.Content = "Testing dockPanel with Label~!";
            _dock.Children.Add(_lbl);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _lbl.Content = "content changed. content changed.content changed. content changed. content changed. content changed. content changed.";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeListBox", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeListBox : CodeTest
    {


        public DockPanelContentPropChangeListBox()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        ListBox _lb;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lb = CommonFunctionality.CreateListBox(10);
            _dock.Children.Add(_lb);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ListBoxItem li = new ListBoxItem();
            li.Content = "List Item added with Long Text...~";
            _lb.Items.Add(li);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeMenu", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeMenu : CodeTest
    {


        public DockPanelContentPropChangeMenu()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        Menu _menu;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _menu = CommonFunctionality.CreateMenu(5);
            _dock.Children.Add(_menu);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            MenuItem mi = new MenuItem();
            mi.Header = "Menu Item Added~!!!";
            _menu.Items.Add(mi);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeCanvas", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeCanvas : CodeTest
    {


        public DockPanelContentPropChangeCanvas()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        Canvas _canvas;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _canvas = new Canvas();
            _canvas.Height = 100;
            _canvas.Width = 100;
            _canvas.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle cRect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(cRect, 10);
            Canvas.SetTop(cRect, 10);
            _canvas.Children.Add(cRect);
            _dock.Children.Add(_canvas);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

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
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeDockPanel", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeDockPanel : CodeTest
    {


        public DockPanelContentPropChangeDockPanel()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        DockPanel _dockpanel;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

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
            _dock.Children.Add(_dockpanel);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _dockpanel.Width = _dockpanel.ActualWidth * 2;
            _dockpanel.Height = _dockpanel.ActualHeight * 2;
            DockPanel.SetDock(_dockpanel.Children[0], Dock.Right);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeStackPanel", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeStackPanel : CodeTest
    {


        public DockPanelContentPropChangeStackPanel()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        StackPanel _s;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _s = new StackPanel();
            _s.Width = 200;
            _dock.Children.Add(_s);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

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
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(3, "Panels.DockPanel", "DockPanelContentPropChangeGrid", Variables = "Area=ElementLayout")]
    public class DockPanelContentPropChangeGrid : CodeTest
    {


        public DockPanelContentPropChangeGrid()
        {

        }
        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        DockPanel _dock;

        Grid _g;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _dock = new DockPanel();
            _dock.Background = Brushes.RoyalBlue;
            _dock.HorizontalAlignment = HorizontalAlignment.Center;
            _dock.VerticalAlignment = VerticalAlignment.Center;

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

            _dock.Children.Add(_g);

            _root.Children.Add(_dock);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _dock.RenderSize;

            _relayoutOccurred = false;
            _dock.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ColumnDefinition ccd = new ColumnDefinition();
            _g.ColumnDefinitions.Add(ccd);
            _g.Width = _g.ActualWidth * 2;
            _g.Height = _g.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _dock.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_dock.RenderSize == _preTestSize)
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

    [Test(2, "Panels.DockPanel", "DockPanelClip",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelClip : VisualScanTest
    {
        public DockPanelClip()
            : base("DockPanelClip.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelClipToBounds",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelClipToBounds : VisualScanTest
    {
        public DockPanelClipToBounds()
            : base("DockPanelClipToBounds.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelFillLastChild",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelFillLastChild : VisualScanTest
    {
        public DockPanelFillLastChild()
            : base("DockPanelFillLastChild.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelLastChildHStack",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelLastChildHStack : VisualScanTest
    {
        public DockPanelLastChildHStack()
            : base("DockPanelLastChildHStack.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelLastChildVStack",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelLastChildVStack : VisualScanTest
    {
        public DockPanelLastChildVStack()
            : base("DockPanelLastChildVStack.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelMixedDocking2",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelMixedDocking2 : VisualScanTest
    {
        public DockPanelMixedDocking2()
            : base("DockPanelMixedDocking2.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelMixedDocking3",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelMixedDocking3 : VisualScanTest
    {
        public DockPanelMixedDocking3()
            : base("DockPanelMixedDocking3.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelBoxModel",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelBoxModel : VisualScanTest
    {
        public DockPanelBoxModel()
            : base("DockPanelBoxModel.xaml")
        { }
    }
 
    [Test(2, "Panels.DockPanel", "DockPanelBoxModelRTL",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelBoxModelRTL : VisualScanTest
    {
        public DockPanelBoxModelRTL()
            : base("DockPanelBoxModelRTL.xaml")
        { }
    }
    
    // [DISABLED_WHILE_PORTING] 
    [Test(2, "Panels.DockPanel", "DockPanelChildNaturalSizePlusMarginsRTL",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN", Disabled=true)]
    public class DockPanelChildNaturalSizePlusMarginsRTL : VisualScanTest
    {
        public DockPanelChildNaturalSizePlusMarginsRTL()
            : base("DockPanelChildNaturalSizePlusMarginsRTL.xaml")
        { }
    }   
    [Test(2, "Panels.DockPanel", "DockPanelClipRTL",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelClipRTL : VisualScanTest
    {
        public DockPanelClipRTL()
            : base("DockPanelClipRTL.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelLastChildHStackRTL",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelLastChildHStackRTL : VisualScanTest
    {
        public DockPanelLastChildHStackRTL()
            : base("DockPanelLastChildHStackRTL.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelLastChildVStackRTL",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelLastChildVStackRTL : VisualScanTest
    {
        public DockPanelLastChildVStackRTL()
            : base("DockPanelLastChildVStackRTL.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelMixedDocking1RTL",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelMixedDocking1RTL : VisualScanTest
    {
        public DockPanelMixedDocking1RTL()
            : base("DockPanelMixedDocking1RTL.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelMixedDocking2RTL",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelMixedDocking2RTL : VisualScanTest
    {
        public DockPanelMixedDocking2RTL()
            : base("DockPanelMixedDocking2RTL.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelStyle",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelStyle : VisualScanTest
    {
        public DockPanelStyle()
            : base("DockPanelStyle.xaml")
        { }
    }
    [Test(2, "Panels.DockPanel", "DockPanelChildStyle",
    Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class DockPanelChildStyle : VisualScanTest
    {
        public DockPanelChildStyle()
            : base("DockPanelChildStyle.xaml")
        { }
    }
}
