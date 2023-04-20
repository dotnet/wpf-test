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
using System.IO;

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.Border", "FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Border, test=HeightWidthDefault")]
    [Test(1, "Panels.Border", "FrameworkElementProps.HeightWidthActual", TestParameters = "target=Border, test=HeightWidthActual")]
    [Test(1, "Panels.Border", "FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Border, test=ChildHeightWidth")]
    [Test(1, "Panels.Border", "FrameworkElementProps.MinHeightWidth", TestParameters = "target=Border, test=MinHeightWidth")]
    [Test(1, "Panels.Border", "FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Border, test=MaxHeightWidth")]
    [Test(1, "Panels.Border", "FrameworkElementProps.Visibility", TestParameters = "target=Border, test=Visibility")]
    [Test(1, "Panels.Border", "FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Border, test=HorizontalAlignment")]
    [Test(1, "Panels.Border", "FrameworkElementProps.VerticalAlignment", TestParameters = "target=Border, test=VerticalAlignment")]
    [Test(1, "Panels.Border", "FrameworkElementProps.FlowDirection", TestParameters = "target=Border, test=FlowDirection")]
    [Test(1, "Panels.Border", "FrameworkElementProps.Margin", TestParameters = "target=Border, test=Margin")]
    public class BorderFETest : FrameworkElementPropertiesTest
    {
        public BorderFETest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }
    }

    [Test(1, "Panels.Border", "PaddingTest.Panel", TestParameters = "target=Panel")]
    [Test(1, "Panels.Border", "PaddingTest.Canvas", TestParameters = "target=Canvas")]
    [Test(1, "Panels.Border", "PaddingTest.StackPanel", TestParameters = "target=StackPanel")]
    [Test(1, "Panels.Border", "PaddingTest.Grid", TestParameters = "target=Grid")]
    [Test(1, "Panels.Border", "PaddingTest.DockPanel", TestParameters = "target=DockPanel")]
    [Test(1, "Panels.Border", "PaddingTest.Decorator", TestParameters = "target=Decorator")]
    [Test(1, "Panels.Border", "PaddingTest.Border", TestParameters = "target=Border")]
    [Test(1, "Panels.Border", "PaddingTest.Viewbox", TestParameters = "target=Viewbox")]
    [Test(1, "Panels.Border", "PaddingTest.Transform", TestParameters = "target=Transform")]
    [Test(1, "Panels.Border", "PaddingTest.ScrollViewer", TestParameters = "target=ScrollViewer")]
    public class BorderPaddingTest : CodeTest
    {
        public BorderPaddingTest() { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.RoyalBlue;

            _testBorder = new Border();
            _testBorder.BorderBrush = Brushes.DarkRed;

            _root.Children.Add(_testBorder);

            return _root;
        }

        public override void TestActions()
        {
            string target = DriverState.DriverParameters["target"];
            BorderTest(target);
        }

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

        private Grid _root = null;
        private Border _testBorder = null;
        private bool _tempresult = true;
        private int _testCounter = 0;

        private void BorderTest(string ChildType)
        {
            Helpers.Log("Border Child : " + ChildType + ".");
            double TestSize;
            if (ChildType == "Transform")
            {
                TestSize = 500;
            }
            else
            {
                TestSize = double.NaN;
            }
            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(0));
            SetBorderProps(new Thickness(0), new Size(TestSize, TestSize));
            ValidateBorderTest(ChildType, _testCounter);

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(0));
            SetBorderProps(new Thickness(0), new Size(5, 5));
            ValidateBorderTest(ChildType, _testCounter);

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(1));
            SetBorderProps(new Thickness(1), new Size(4, 4));
            ValidateBorderTest(ChildType, _testCounter);

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(10));
            SetBorderProps(new Thickness(10), new Size(TestSize, TestSize));
            ValidateBorderTest(ChildType, _testCounter);

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(44, 3, 75, 0));
            SetBorderProps(new Thickness(0, 22, 9, 101), new Size(TestSize, TestSize));
            ValidateBorderTest(ChildType, _testCounter);

            //cause my test sizes to be negative..

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(153));
            SetBorderProps(new Thickness(153), new Size(TestSize, TestSize));
            ValidateBorderTest(ChildType, _testCounter);

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(256));
            SetBorderProps(new Thickness(256), new Size(TestSize, TestSize));
            ValidateBorderTest(ChildType, _testCounter);

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(1256));
            SetBorderProps(new Thickness(1256), new Size(TestSize, TestSize));
            ValidateBorderTest(ChildType, _testCounter);

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(3003));
            SetBorderProps(new Thickness(153), new Size(6000, 6000));
            ValidateBorderTest(ChildType, _testCounter);

            AddBorderChild(ChildType, new Size(double.NaN, double.NaN), new Thickness(3));
            SetBorderProps(new Thickness(3), new Size(16153, 16153));
            ValidateBorderTest(ChildType, _testCounter);

            _testCounter = 0;

        }

        private void ValidateBorderTest(string ChildType, int TestCount)
        {
            bool isSkipTest = false;
            if (ChildType == "StackPanel" || ChildType == "Border")
            {
                if (TestCount == 1 || TestCount == 5)
                {
                    isSkipTest = true;
                }
            }
            if (ChildType == "Transform")
            {
                if (TestCount == 4)
                {
                    isSkipTest = true;
                }
            }

            CommonFunctionality.FlushDispatcher();
            FrameworkElement TestChild = _testBorder.Child as FrameworkElement;
            if (!isSkipTest)
            {
                if (ChildType != "Transform")
                {
                    //  Helpers.Log(TestCount);
                    //padding and margin totals
                    double PMLR = _testBorder.Padding.Left + _testBorder.Padding.Right + TestChild.Margin.Left + TestChild.Margin.Right;
                    double PMTB = _testBorder.Padding.Top + _testBorder.Padding.Bottom + TestChild.Margin.Top + TestChild.Margin.Bottom;

                    if (PMLR < _testBorder.ActualWidth || PMTB < _testBorder.ActualHeight)
                    {
                        Size ChildSizeTest = new Size((_testBorder.ActualWidth - _testBorder.Padding.Left - _testBorder.Padding.Right - TestChild.Margin.Left - TestChild.Margin.Right)
                          , (_testBorder.ActualHeight - _testBorder.Padding.Top - _testBorder.Padding.Bottom - TestChild.Margin.Top - TestChild.Margin.Bottom));

                        Size BorderSizeTest = new Size((TestChild.ActualWidth + TestChild.Margin.Left + TestChild.Margin.Right + _testBorder.Padding.Left + _testBorder.Padding.Right)
                            , (TestChild.ActualHeight + TestChild.Margin.Top + TestChild.Margin.Bottom + _testBorder.Padding.Top + _testBorder.Padding.Bottom));


                        if (!DoubleUtil.AreClose(new Size(_testBorder.ActualWidth, _testBorder.ActualHeight), BorderSizeTest))
                        {
                            Helpers.Log("FAIL : BorderSizeTest");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Pass : BorderSizeTest");
                        }

                        if (!DoubleUtil.AreClose(new Size(TestChild.ActualWidth, TestChild.ActualHeight), ChildSizeTest))
                        {
                            Helpers.Log("FAIL : ChildSizeTest");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Pass : ChildSizeTest");
                        }
                    }
                    else
                    {
                        if (ChildType == "StackPanel")
                        {
                            if (TestChild.RenderSize.Width > 0 || TestChild.RenderSize.Width > 0)
                            {
                                Helpers.Log("FAIL : Large Value Test");
                                Helpers.Log("Border Padding and Child Margin should consume all avaliable space ");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Pass : Large Value Test");
                            }
                        }
                        else if (ChildType == "Border")
                        {
                            if ((TestChild.ActualWidth - ((Border)TestChild).Padding.Right - ((Border)TestChild).Padding.Left) > 0 || (TestChild.ActualHeight - ((Border)TestChild).Padding.Top - ((Border)TestChild).Padding.Bottom) > 0)
                            {
                                Helpers.Log("FAIL : Large Value Test");
                                Helpers.Log("Border Padding and Child Margin should consume all avaliable space ");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Pass : Large Value Test");
                            }
                        }
                        else
                        {
                            if (TestChild.ActualWidth > 0 || TestChild.ActualHeight > 0)
                            {
                                Helpers.Log("FAIL : Large Value Test");
                                Helpers.Log("Border Padding and Child Margin should consume all avaliable space ");
                                _tempresult = false;
                            }
                            else
                            {
                                Helpers.Log("Pass : Large Value Test");
                            }
                        }
                    }
                }
                else
                {
                    Transform childTransform = VisualTreeHelper.GetTransform(TestChild);
                    double getSquareValue = ((TestChild.ActualHeight * TestChild.ActualHeight) + (TestChild.ActualWidth * TestChild.ActualWidth));

                    double rotatedValue = Math.Sqrt(getSquareValue);

                    double ChildWidth = 0;
                    double ChildHeight = 0;

                    ChildWidth = rotatedValue;
                    ChildHeight = rotatedValue;

                    double PMLR = _testBorder.Padding.Left + _testBorder.Padding.Right + TestChild.Margin.Left + TestChild.Margin.Right;
                    double PMTB = _testBorder.Padding.Top + _testBorder.Padding.Bottom + TestChild.Margin.Top + TestChild.Margin.Bottom;

                    if (PMLR < _testBorder.ActualWidth || PMTB < _testBorder.ActualHeight)
                    {
                        Size ChildSizeTest = new Size((_testBorder.ActualWidth - _testBorder.Padding.Left - _testBorder.Padding.Right - TestChild.Margin.Left - TestChild.Margin.Right)
                          , (_testBorder.ActualHeight - _testBorder.Padding.Top - _testBorder.Padding.Bottom - TestChild.Margin.Top - TestChild.Margin.Bottom));

                        Size BorderSizeTest = new Size((ChildWidth + TestChild.Margin.Left + TestChild.Margin.Right + _testBorder.Padding.Left + _testBorder.Padding.Right)
                            , (ChildHeight + TestChild.Margin.Top + TestChild.Margin.Bottom + _testBorder.Padding.Top + _testBorder.Padding.Bottom));


                        if (!DoubleUtil.AreClose(new Size(_testBorder.ActualWidth, _testBorder.ActualHeight), BorderSizeTest))
                        {
                            Helpers.Log("FAIL : BorderSizeTest");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Pass : BorderSizeTest");
                        }

                        if (!DoubleUtil.AreClose(new Size(ChildWidth, ChildHeight), ChildSizeTest))
                        {
                            Helpers.Log("FAIL : ChildSizeTest");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Pass : ChildSizeTest");
                        }
                    }
                    else
                    {
                        if (TestChild.ActualWidth > 0 || TestChild.ActualHeight > 0)
                        {
                            Helpers.Log("FAIL : Large Value Test");
                            Helpers.Log("Border Padding and Child Margin should consume all avaliable space ");
                            _tempresult = false;
                        }
                        else
                        {
                            Helpers.Log("Pass : Large Value Test");
                        }
                    }
                }
            }
            CommonFunctionality.FlushDispatcher();
            _testBorder.Child = null;
            _testCounter++;
        }

        private void SetBorderProps(Thickness padding, Size size)
        {
            _testBorder.Height = size.Height;
            _testBorder.Width = size.Width;
            _testBorder.Padding = padding;
        }

        private void AddBorderChild(string ChildType, Size ChildSize, Thickness margin)
        {
            FrameworkElement child = null;
            switch (ChildType)
            {
                case "Panel":
                    {
                        //Helpers.Log("Adding Child : Panel");
                        TestPanel panel = new TestPanel();
                        panel.Background = Brushes.YellowGreen;
                        child = panel;
                        break;
                    }

                case "Canvas":
                    {
                        //Helpers.Log("Adding Child : Canvas");
                        Canvas canvas = new Canvas();
                        canvas.Background = Brushes.YellowGreen;
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
                        //Helpers.Log("Adding Child : StackPanel");
                        StackPanel stack = new StackPanel();
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
                        Rectangle r = CommonFunctionality.CreateRectangle(100, 100, Brushes.Orange);

                        bor.Child = r;

                        child = bor;
                        break;
                    }

                case "Viewbox":
                    {
                        Viewbox vb = new Viewbox();
                        vb.Stretch = Stretch.Fill;
                        TextBlock txt = new TextBlock();
                        txt.Text = "foo";

                        vb.Child = txt;
                        child = vb;
                        break;
                    }

                case "Transform":
                    {
                        RotateTransform rt = new RotateTransform(45);

                        Decorator td = new Decorator();
                        td.LayoutTransform = rt;
                        Rectangle r = new Rectangle();
                        r.Fill = Brushes.AntiqueWhite;

                        td.Child = r;
                        child = td;
                        break;
                    }

                case "ScrollViewer":
                    {
                        ScrollViewer sv = new ScrollViewer();
                        sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                        sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

                        TextBlock txt = new TextBlock();
                        txt.Text = "some text";
                        txt.FontSize = 100;

                        sv.Content = txt;

                        child = sv;
                        break;
                    }
                case "WrapPanel":
                    WrapPanel wrap = new WrapPanel();
                    for (int i = 0; i < 6; i++)
                    {
                        Border wc = new Border();
                        wc.Background = Brushes.LightGray;
                        TextBlock txt = new TextBlock();
                        txt.FontSize = 100;
                        txt.Foreground = Brushes.White;
                        txt.HorizontalAlignment = HorizontalAlignment.Center;
                        txt.VerticalAlignment = VerticalAlignment.Center;
                        txt.Text = i.ToString();
                        wc.Child = txt;
                        wrap.Children.Add(wc);
                    }

                    child = wrap;
                    break;
            }

            child.Margin = margin;
            child.Height = ChildSize.Height;
            child.Width = ChildSize.Width;

            _testBorder.Child = child;
        }
    }

    [Test(1, "Panels.Border", "BorderPropertyRelayout", Variables="Area=ElementLayout")]
    public class BorderPropertyRelayout : CodeTest
    {
        public BorderPropertyRelayout()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Border _border;
        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.RoyalBlue;

            _border = new Border();

            _rect = new Rectangle();
            _rect.Fill = Brushes.Pink;
            _border.Child = _rect;

            _root.Children.Add(_border);

            return _root;
        }

        public override void TestActions()
        {
            for (int i = 0; i < 50; i++)
            {
                Test(i);
                CommonFunctionality.FlushDispatcher();
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


        double _borderThickness;
        double _padding;

        void Test(int TestRun)
        {
            Helpers.Log("*** Test Run " + TestRun + " ***");

            _borderThickness = 0;
            _padding = 0;

            Helpers.Log("*** No Props Set ***");
            if (_rect.ActualHeight != _border.ActualHeight && _border.ActualHeight != _root.ActualHeight)
            {
                Helpers.Log("FAIL : Heights are wrong before props are set.");
                _tempresult = false;
            }
            else if (_rect.ActualWidth != _border.ActualWidth && _border.ActualWidth != _root.ActualWidth)
            {
                Helpers.Log("FAIL : Widths are wrong before props are set.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("PASS : Sizes are correct before props are set.");
            }

            Helpers.Log("*** BorderThickness Test ***");
            _borderThickness = RandomDouble(3, 100);
            _border.BorderThickness = new Thickness(_borderThickness);
            _border.BorderBrush = Brushes.CadetBlue;

            CommonFunctionality.FlushDispatcher();

            UIElement btTestOne = Finder(new Point(RandomDouble(0, _borderThickness), RandomDouble(0, _borderThickness)), _root);
            UIElement btTestTwo = Finder(new Point((_root.ActualWidth - RandomDouble(0, _borderThickness)), (_root.ActualHeight - RandomDouble(0, _borderThickness))), _root);
            if (btTestOne.GetType().Name != "Border")
            {
                Helpers.Log("FAIL : BorderThickness Test One Failed." + _border.BorderThickness.ToString());
                _tempresult = false;
            }
            else if (btTestTwo.GetType().Name != "Border")
            {
                Helpers.Log("FAIL : BorderThickness Test Two Failed." + _border.BorderThickness.ToString());
                _tempresult = false;
            }
            else
            {
                Helpers.Log("PASS : BorderThickness Test Passed.");
            }


            Helpers.Log("*** Padding Test ***");
            _padding = RandomDouble(50, 150);
            _border.Padding = new Thickness(_padding);

            CommonFunctionality.FlushDispatcher();
            UIElement pTestOne = Finder(new Point(RandomDouble(_borderThickness, (_borderThickness + _padding)), RandomDouble(_borderThickness, (_borderThickness + _padding))), _root);
            UIElement pTestTwo = Finder(new Point((_root.ActualWidth - RandomDouble(_borderThickness, (_borderThickness + _padding))), (_root.ActualHeight - RandomDouble(_borderThickness, (_borderThickness + _padding)))), _root);

            if (pTestOne.GetType().Name != "Grid")
            {
                Helpers.Log("FAIL : Padding Test One Failed." + _border.Padding.ToString());
                _tempresult = false;
            }
            else if (pTestTwo.GetType().Name != "Grid")
            {
                Helpers.Log("FAIL : Padding Test Two Failed." + _border.Padding.ToString());
                _tempresult = false;
            }
            else
            {
                Helpers.Log("PASS : Padding Test Passed.");
            }

            Helpers.Log("*** CornerRadius Test ***");
            _border.CornerRadius = new CornerRadius(500);

            CommonFunctionality.FlushDispatcher();


            UIElement crTestOne = Finder(new Point(RandomDouble(0, _borderThickness), RandomDouble(0, _borderThickness)), _root);
            UIElement crTestTwo = Finder(new Point((_root.ActualWidth - RandomDouble(0, _borderThickness)), (_root.ActualHeight - RandomDouble(0, _borderThickness))), _root);

            UIElement crTestThree = Finder(new Point((_root.ActualWidth - RandomDouble(0, _borderThickness)), RandomDouble(0, _borderThickness)), _root);
            UIElement crTestFour = Finder(new Point(RandomDouble(0, _borderThickness), (_root.ActualHeight - RandomDouble(0, _borderThickness))), _root);

            UIElement crTestFive = Finder(new Point((_root.ActualWidth / 2), RandomDouble(0, _borderThickness)), _root);
            UIElement crTestSix = Finder(new Point((_root.ActualWidth / 2), (_root.ActualHeight - RandomDouble(0, _borderThickness))), _root);

            UIElement crTestSeven = Finder(new Point(RandomDouble(0, _borderThickness), (_root.ActualHeight / 2)), _root);
            UIElement crTestEight = Finder(new Point((_root.ActualWidth - RandomDouble(0, _borderThickness)), (_root.ActualHeight / 2)), _root);

            if (crTestOne.GetType().Name != "Grid" || crTestTwo.GetType().Name != "Grid" || crTestThree.GetType().Name != "Grid" || crTestFour.GetType().Name != "Grid")
            {
                Helpers.Log("FAIL : CornerRadius Test (Corners were not Grid)");
                _tempresult = false;
            }
            else if (crTestFive.GetType().Name != "Border" || crTestSix.GetType().Name != "Border" || crTestSeven.GetType().Name != "Border" || crTestEight.GetType().Name != "Border")
            {
                Helpers.Log("FAIL : CornerRadius Test (Sides were not Border)");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("PASS : CornerRadius Test");
            }

            //Helpers.Log(Finder(new Point(35, 35), root));  

            CommonFunctionality.FlushDispatcher();

            _border.BorderThickness = new Thickness(0);
            _border.Padding = new Thickness(0);
            _border.CornerRadius = new CornerRadius(0);

            CommonFunctionality.FlushDispatcher();

            Helpers.Log("*** No Props Set (Back To Zero)***");
            if (_rect.ActualHeight != _border.ActualHeight && _border.ActualHeight != _root.ActualHeight)
            {
                Helpers.Log("FAIL : Heights are wrong before props are set.");
                _tempresult = false;
            }
            else if (_rect.ActualWidth != _border.ActualWidth && _border.ActualWidth != _root.ActualWidth)
            {
                Helpers.Log("FAIL : Widths are wrong before props are set.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("PASS : Sizes are correct after all props are set back to zero.");
            }
        }

        UIElement Finder(Point point, UIElement root)
        {
            //Helpers.Log(point);
            UIElement hotElement = LayoutUtility.GetInputElement(root, point);
            return hotElement;
        }

        double RandomDouble(double Min, double Max)
        {
            Random random = new Random();
            double WithInRange = (double)random.Next((int)(Min + 1), (int)Max);
            return WithInRange;
        }

    }

    [Test(1, "Panels.Border", "BorderResize", Variables="Area=ElementLayout")]
    public class BorderResize : CodeTest
    {
        public BorderResize()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 480;
            this.window.Width = 640;
            this.window.Top = 0;
            this.window.Left = 0;

            this.window.Content = this.TestContent();
        }

        Border _theBorder = null;
        FrameworkElement _theParent = null;
        FrameworkElement _theContent = null;
        int _count = 0;
        bool _w;
        bool _h;

        public override FrameworkElement TestContent()
        {
            Border border = CommonFunctionality.CreateBorder(Brushes.LightPink, new Thickness(0), Brushes.Pink, 250, 250);
            Border bod = CommonFunctionality.CreateBorder(Brushes.LightBlue, new Thickness(0), Brushes.Blue);

            DockPanel child = new DockPanel();
            child.Background = Brushes.LightYellow;

            bod.Child = child;

            border.Child = bod;

            _theBorder = bod;
            _theContent = child;
            _theParent = border;

            return border;
        }

        public override void TestActions()
        {
            //Border with no constraints
            Helpers.Log("Border With no Constraints.");
            CommonFunctionality.FlushDispatcher();
            testVerification("ValidateEaualToContent");
            CommonFunctionality.FlushDispatcher();

            //Border Width and Height in small pixel value.
            Helpers.Log("Border With small pixel width,small pixel height.");
            _theContent.Width = 100;
            _theContent.Height = 100;
            _theBorder.Width = 20;
            _theBorder.Height = 20;
            CommonFunctionality.FlushDispatcher();
            testVerification("ValidateSmallerThanContent");
            CommonFunctionality.FlushDispatcher();

            //Border Width and Height in large pixel value.
            Helpers.Log("Border With large pixel width,large pixel height.");
            _theBorder.Width = 350;
            _theBorder.Height = 350;
            CommonFunctionality.FlushDispatcher();
            testVerification("ValidateLargerThanParent");
            CommonFunctionality.FlushDispatcher();

            //Small Pixel Max Width, Small Pixel Max Height.
            Helpers.Log("Border with small pixel max width, small pixel max height");
            _theBorder.MaxWidth = 18;
            _theBorder.MaxHeight = 18;
            CommonFunctionality.FlushDispatcher();
            testVerification("ValidateSmallerThanContent");
            CommonFunctionality.FlushDispatcher();

            //Large Pixel Max Width, Large Pixel Max Height.
            Helpers.Log("Border with large pixel max width, large pixel max height");
            _theBorder.MaxWidth = 400;
            _theBorder.MaxHeight = 400;
            CommonFunctionality.FlushDispatcher();
            testVerification("ValidateLargerThanContent");
            CommonFunctionality.FlushDispatcher();

            //Small Pixel Min Width, Small Pixel Min Height.
            Helpers.Log("Border with small pixel min width, small pixel min height");
            _theBorder.MinWidth = 18;
            _theBorder.MinHeight = 18;
            CommonFunctionality.FlushDispatcher();
            testVerification("ValidateLargerThanContent");
            CommonFunctionality.FlushDispatcher();

            //Large Pixel Min Width, Large Pixel Min Height.
            Helpers.Log("Border with large pixel min width, large pixel min height");
            _theBorder.MinWidth = 400;
            _theBorder.MinHeight = 400;
            CommonFunctionality.FlushDispatcher();
            testVerification("ValidateLargerThanParent");
            CommonFunctionality.FlushDispatcher();
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

        void testVerification(string type)
        {
            switch (type)
            {
                case "ValidateEaualToContent":
                    _w = Equal(_theBorder.RenderSize.Width, _theContent.RenderSize.Width);
                    _h = Equal(_theBorder.RenderSize.Height, _theContent.RenderSize.Height);
                    //tempresult = w && h;

                    if (!_w || !_h)
                    {
                        Helpers.Log("Border Size" + _theBorder.RenderSize + "Content Size" + _theContent.RenderSize);
                        Helpers.Log("The border is not the same size as its content");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log(_theBorder.RenderSize + " , " + _theContent.RenderSize);
                    }

                    break;
                case "ValidateSmallerThanContent":
                    _w = LessThanOrEqual(_theBorder.RenderSize.Width, _theContent.RenderSize.Width);
                    _h = LessThanOrEqual(_theBorder.RenderSize.Height, _theContent.RenderSize.Height);
                    //tempresult = w && h;

                    if (!_w || !_h)
                    {
                        Helpers.Log("Border Size" + _theBorder.RenderSize + "Content Size" + _theContent.RenderSize);
                        Helpers.Log("The border is not smaller than its content");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log(_theBorder.RenderSize + " , " + _theContent.RenderSize);
                    }

                    break;
                case "ValidateLargerThanParent":
                    _w = GreaterThanOrEqual(_theBorder.RenderSize.Width, _theParent.RenderSize.Width);
                    _h = GreaterThanOrEqual(_theBorder.RenderSize.Height, _theParent.RenderSize.Height);
                    //tempresult = w && h;

                    if (!_w || !_h)
                    {
                        Helpers.Log("Border Size" + _theBorder.RenderSize + "Parent Size" + _theParent.RenderSize);
                        Helpers.Log("The border is not greater than its Parent");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log(_theBorder.RenderSize + " , " + _theParent.RenderSize);
                    }

                    break;
                case "ValidateLargerThanContent":
                    _w = GreaterThanOrEqual(_theBorder.RenderSize.Width, _theContent.RenderSize.Width);
                    _h = GreaterThanOrEqual(_theBorder.RenderSize.Height, _theContent.RenderSize.Height);
                    //tempresult = w && h;

                    if (!_w || !_h)
                    {
                        Helpers.Log("Border Size " + _theBorder.RenderSize + "Content Size" + _theContent.RenderSize);
                        Helpers.Log("The border is not greater than its content");
                        _tempresult = false;
                    }
                    else
                    {
                        Helpers.Log(_theBorder.RenderSize + " , " + _theContent.RenderSize);
                    }

                    break;
            }
            this._count++;
        }


        const double FloatingPointError = 1e-10; //CLR doubles have about 15 to 16 digits of precision

        bool Equal(double a, double b)
        {
            return Math.Abs(a - b) < FloatingPointError;
        }

        bool LessThan(double a, double b)
        {
            return !Equal(a, b) && (a < b);
        }

        bool GreaterThan(double a, double b)
        {
            return !Equal(a, b) && (a > b);
        }

        bool LessThanOrEqual(double a, double b)
        {
            return !GreaterThan(a, b);
        }

        bool GreaterThanOrEqual(double a, double b)
        {
            return !LessThan(a, b);
        }
    }

    [Test(2, "Panels.Border", "BorderBigEllipseChild", Variables="Area=ElementLayout")]
    public class BorderBigEllipseChild : CodeTest
    {
        public BorderBigEllipseChild()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            string xamlfile = "BorderEllipseBig.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override void TestActions()
        {
            Canvas testRoot = this.window.Content as Canvas;

            Point borderPoint = new Point(20, 130);
            Point ellipsePoint = new Point(125, 40);

            IInputElement borderHitTestElement;
            borderHitTestElement = testRoot.InputHitTest(borderPoint);

            if (borderHitTestElement == null)
            {
                Helpers.Log("Border HitTest returned null.");
                _tempresult = false;
            }
            else
            {
                if (borderHitTestElement.GetType().ToString() != "System.Windows.Controls.Border")
                {
                    Helpers.Log("Border HitTest did nottempresult = Border Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Border Element is there and located where it supposed to be.");
                }
            }

            IInputElement ellipseHitTestElement;
            ellipseHitTestElement = testRoot.InputHitTest(ellipsePoint);

            if (ellipseHitTestElement == null)
            {
                Helpers.Log("Ellipse HitTest returned null.");
                _tempresult = false;
            }
            else
            {
                if (ellipseHitTestElement.GetType().ToString() != "System.Windows.Shapes.Ellipse")
                {
                    Helpers.Log("Ellipse HitTest did nottempresult = Ellipse Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Ellipse Element is there and located where it supposed to be.");
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

    [Test(2, "Panels.Border", "BorderPolygonChild", Variables="Area=ElementLayout")]
    public class BorderPolygonChild : CodeTest
    {
        public BorderPolygonChild()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 500;
            this.window.Width = 500;
            this.window.Top = 50;
            this.window.Left = 50;

            string xamlfile = "PolygonBod.xaml";
            System.IO.FileStream f = new System.IO.FileStream(xamlfile, FileMode.Open, FileAccess.Read);

            this.window.Content = (FrameworkElement)System.Windows.Markup.XamlReader.Load(f);

            f.Close();
        }

        public override void TestActions()
        {
            Canvas testRoot = this.window.Content as Canvas;

            Point borderPointTopLeft = new Point(100, 100);
            Point borderPointBotRight = new Point(320, 340);

            IInputElement borderHitTestElementTopLeft;
            borderHitTestElementTopLeft = testRoot.InputHitTest(borderPointTopLeft);

            if (borderHitTestElementTopLeft == null)
            {
                Helpers.Log("Border HitTest Top Left returned null.");
                _tempresult = false;
            }
            else
            {
                if (borderHitTestElementTopLeft.GetType().ToString() != "System.Windows.Controls.Border")
                {
                    Helpers.Log("Border HitTest Top Left did nottempresult = Border Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Border Element Top Left is there and located where it supposed to be.");
                }
            }

            IInputElement borderHitTestElementBotRight;
            borderHitTestElementBotRight = testRoot.InputHitTest(borderPointBotRight);

            if (borderHitTestElementBotRight == null)
            {
                Helpers.Log("Border HitTest Bottom Right returned null.");
                _tempresult = false;
            }
            else
            {
                if (borderHitTestElementBotRight.GetType().ToString() != "System.Windows.Controls.Border")
                {
                    Helpers.Log("Border HitTest Bottom Right did nottempresult = Border Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Border Element Bottom Right is there and located where it supposed to be.");
                }
            }

            Point polygonPointOne = new Point(226.5, 100);

            IInputElement polygoneHitTestPointOne;
            polygoneHitTestPointOne = testRoot.InputHitTest(polygonPointOne);

            if (polygoneHitTestPointOne == null)
            {
                Helpers.Log("Polygon HitTest Point One returned null.");
                _tempresult = false;
            }
            else
            {
                if (polygoneHitTestPointOne.GetType().ToString() != "System.Windows.Shapes.Polygon")
                {
                    Helpers.Log("Polygon HitTest Point One did nottempresult = Ellipse Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Polygon Point One is there and located where it supposed to be.");
                }
            }

            Point polygonPointTwo = new Point(336.485, 163.5);

            IInputElement polygoneHitTestPointTwo;
            polygoneHitTestPointTwo = testRoot.InputHitTest(polygonPointTwo);

            if (polygoneHitTestPointTwo == null)
            {
                Helpers.Log("Polygon HitTest Point Two returned null.");
                _tempresult = false;
            }
            else
            {
                if (polygoneHitTestPointTwo.GetType().ToString() != "System.Windows.Shapes.Polygon")
                {
                    Helpers.Log("Polygon HitTest Point Two did nottempresult = Polygon Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Polygon Point Two is there and located where it supposed to be.");
                }
            }

            Point polygonPointThree = new Point(336.485, 290.5);

            IInputElement polygoneHitTestPointThree;
            polygoneHitTestPointThree = testRoot.InputHitTest(polygonPointThree);

            if (polygoneHitTestPointThree == null)
            {
                Helpers.Log("Polygon HitTest Point Three returned null.");
                _tempresult = false;
            }
            else
            {
                if (polygoneHitTestPointThree.GetType().ToString() != "System.Windows.Shapes.Polygon")
                {
                    Helpers.Log("Polygon HitTest Point Three did nottempresult = Polygon Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Polygon Point Three is there and located where it supposed to be.");
                }
            }

            Point polygonPointFour = new Point(116.5148, 290.5);

            IInputElement polygoneHitTestPointFour;
            polygoneHitTestPointFour = testRoot.InputHitTest(polygonPointFour);

            if (polygoneHitTestPointFour == null)
            {
                Helpers.Log("Polygon HitTest Point Four returned null.");
                _tempresult = false;
            }
            else
            {
                if (polygoneHitTestPointFour.GetType().ToString() != "System.Windows.Shapes.Polygon")
                {
                    Helpers.Log("Polygon HitTest Point Four did nottempresult = Polygon Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Polygon Point Four is there and located where it supposed to be.");
                }
            }

            Point polygonPointFive = new Point(213.8, 205.003);

            IInputElement polygoneHitTestPointFive;
            polygoneHitTestPointFive = testRoot.InputHitTest(polygonPointFive);

            if (polygoneHitTestPointFive == null)
            {
                Helpers.Log("Polygon HitTest Point Five returned null.");
                _tempresult = false;
            }
            else
            {
                if (polygoneHitTestPointFive.GetType().ToString() != "System.Windows.Shapes.Polygon")
                {
                    Helpers.Log("Polygon HitTest Point Five did nottempresult = Polygon Element.");
                    _tempresult = false;
                }
                else
                {
                    Helpers.Log("The Polygon Point Five is there and located where it supposed to be.");
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

    #region Content Property Change Test

    [Test(3, "Panels.Border", "BorderContentPropChangeRectangle", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeRectangle : CodeTest
    {
        public BorderContentPropChangeRectangle()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.SlateBlue));
            _border.Child = _rect;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _rect.Width = _rect.ActualWidth * 2;
            _rect.Height = _rect.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeButton", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeButton : CodeTest
    {        
        public BorderContentPropChangeButton()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Border _border;

        Button _btn;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _btn = CommonFunctionality.CreateButton(200, 200, Brushes.Red);
            _border.Child = _btn;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);


            _btn.Width = _btn.ActualWidth * 2;
            _btn.Height = _btn.ActualHeight * 2;
            _btn.Content = "Button Size Changed~!";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderBorderContentPropChangeTextBox", Variables="Area=ElementLayout")]
    public class BorderBorderContentPropChangeTextBox : CodeTest
    {
        public BorderBorderContentPropChangeTextBox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Border _border;

        TextBox _tbox;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _tbox = CommonFunctionality.CreateTextBox(150, 100, "Width=150px");
            _border.Child = _tbox;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _tbox.Width = _tbox.ActualWidth * 2;
            _tbox.Height = _tbox.ActualHeight * 2;
            _tbox.Text = "Size changed * 2";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeEllipse", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeEllipse : CodeTest
    {
        public BorderContentPropChangeEllipse()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Border _border;

        Ellipse _elps;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _elps = new Ellipse();
            _elps.Width = 150;
            _elps.Height = 150;
            _elps.Fill = new SolidColorBrush(Colors.SlateBlue);
            _border.Child = _elps;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _elps.Width = _elps.ActualWidth * 2;
            _elps.Height = _elps.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeImage", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeImage : CodeTest
    {
        public BorderContentPropChangeImage()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        Image _img;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _img = CommonFunctionality.CreateImage("light.bmp");
            _img.Height = 50;
            _img.Width = 50;
            _border.Child = _img;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _img.Width = _img.ActualWidth * 2;
            _img.Height = _img.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeText", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeText : CodeTest
    {
        public BorderContentPropChangeText()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        TextBlock _txt;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _txt = CommonFunctionality.CreateText("Testing Grid...");
            _border.Child = _txt;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _txt.Text = "Changing Text to very large text... Changing Text to very large text...";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeBorder", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeBorder : CodeTest
    {
        public BorderContentPropChangeBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        Border _b;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _b = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.SlateBlue), 25, 200);
            _border.Child = _b;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _b.Width = _b.ActualWidth * 2;
            _b.Height = _b.ActualHeight * 2;
            _b.BorderThickness = new Thickness(20);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeLabel", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeLabel : CodeTest
    {
        public BorderContentPropChangeLabel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        Label _lbl;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lbl = new Label();
            _lbl.Content = "Testing borderPanel with Label~!";
            _border.Child = _lbl;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _lbl.Content = "content changed. content changed.content changed. content changed. content changed. content changed. content changed.";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeListBox", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeListBox : CodeTest
    {
        public BorderContentPropChangeListBox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        ListBox _lb;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lb = CommonFunctionality.CreateListBox(10);
            _border.Child = _lb;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ListBoxItem li = new ListBoxItem();
            li.Content = "List Item added with Long Text...~";
            _lb.Items.Add(li);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeMenu", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeMenu : CodeTest
    {
        public BorderContentPropChangeMenu()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        Menu _menu;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _menu = CommonFunctionality.CreateMenu(5);
            _border.Child = _menu;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            MenuItem mi = new MenuItem();
            mi.Header = "Menu Item Added~!!!";
            _menu.Items.Add(mi);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeCanvas", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeCanvas : CodeTest
    {
        public BorderContentPropChangeCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        Canvas _canvas;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _canvas = new Canvas();
            _canvas.Height = 100;
            _canvas.Width = 100;
            _canvas.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle cRect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(cRect, 10);
            Canvas.SetTop(cRect, 10);
            _canvas.Children.Add(cRect);
            _border.Child = _canvas;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

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
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeDockPanel", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeDockPanel : CodeTest
    {
        public BorderContentPropChangeDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        DockPanel _dockpanel;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

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
            _border.Child = _dockpanel;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _dockpanel.Width = _dockpanel.ActualWidth * 2;
            _dockpanel.Height = _dockpanel.ActualHeight * 2;
            DockPanel.SetDock(_dockpanel.Children[0], Dock.Right);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    [Test(3, "Panels.Border", "BorderContentPropChangeStackPanel", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeStackPanel : CodeTest
    {
        public BorderContentPropChangeStackPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        StackPanel _s;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _s = new StackPanel();
            _s.Width = 200;
            _border.Child = _s;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

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
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
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

    [Test(3, "Panels.Border", "BorderContentPropChangeGrid", Variables="Area=ElementLayout")]
    public class BorderContentPropChangeGrid : CodeTest
    {
        public BorderContentPropChangeGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        Grid _g;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _border = new Border();
            _border.Background = Brushes.RoyalBlue;
            _border.HorizontalAlignment = HorizontalAlignment.Center;
            _border.VerticalAlignment = VerticalAlignment.Center;

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

            _border.Child = _g;

            _root.Children.Add(_border);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _border.RenderSize;

            _relayoutOccurred = false;
            _border.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ColumnDefinition ccd = new ColumnDefinition();
            _g.ColumnDefinitions.Add(ccd);
            _g.Width = _g.ActualWidth * 2;
            _g.Height = _g.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _border.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_border.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but borderPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and borderPanel Size Changed!!!");
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

    #endregion

    #region CornerRadius Tests

    [Test(1, "Panels.Border", "BorderCornerRadiusTest", Variables="Area=ElementLayout")]
    public class BorderCornerRadiusTest : CodeTest
    {
        public BorderCornerRadiusTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        bool _tempResult = true;

        Grid _root;
        Border _childone;
        CornerRadius _crone;
        CornerRadius _crtwo;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Gray;
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            _root.ColumnDefinitions.Add(c1);
            _root.ColumnDefinitions.Add(c2);

            _childone = new Border();
            Grid.SetColumn(_childone, 0);
            _childone.Height = 401;
            _childone.Width = 401;
            _childone.Background = Brushes.Goldenrod;

            _root.Children.Add(_childone);

            return _root;
        }

        public override void TestActions()
        {
            _crone = new CornerRadius(10);

            CommonFunctionality.FlushDispatcher();

            if (_crone.Equals(_childone))
            {
                Helpers.Log("CornerRadius should not be equal to a Border element.");
                _tempResult = false;
            }

            CommonFunctionality.FlushDispatcher();

            #region TopLeft
            try
            {
                _childone.CornerRadius = new CornerRadius(double.NaN, 0, 0, 0);
                _tempResult = false;
            }
            catch (Exception exTopLeftNAN)
            {
                Helpers.Log("Exception caught for NAN CornerRadius.TopLeft.");
                Helpers.Log(exTopLeftNAN.Message);
            }

            try
            {
                _childone.CornerRadius = new CornerRadius(double.PositiveInfinity, 0, 0, 0);
                _tempResult = false;
            }
            catch (Exception exTopLeftPositiveIfinity)
            {
                Helpers.Log("Exception caught for PositiveInfinity CornerRadius.TopLeft.");
                Helpers.Log(exTopLeftPositiveIfinity.Message);
            }

            try
            {
                _childone.CornerRadius = new CornerRadius(double.NegativeInfinity, 0, 0, 0);
                _tempResult = false;
            }
            catch (Exception exTopLeftNegativeInfinity)
            {
                Helpers.Log("Exception caught for NegativeInfinity CornerRadius.TopLeft.");
                Helpers.Log(exTopLeftNegativeInfinity.Message);
            }
            #endregion

            #region TopRight
            try
            {
                _childone.CornerRadius = new CornerRadius(0, double.NaN, 0, 0);
                _tempResult = false;
            }
            catch (Exception exTopRightNAN)
            {
                Helpers.Log("Exception caught for NAN CornerRadius.TopRight.");
                Helpers.Log(exTopRightNAN.Message);
            }

            try
            {
                _childone.CornerRadius = new CornerRadius(0, double.PositiveInfinity, 0, 0);
                _tempResult = false;
            }
            catch (Exception exTopRightPositiveIfinity)
            {
                Helpers.Log("Exception caught for PositiveInfinity CornerRadius.TopRight.");
                Helpers.Log(exTopRightPositiveIfinity.Message);
            }

            try
            {
                _childone.CornerRadius = new CornerRadius(0, double.NegativeInfinity, 0, 0);
                _tempResult = false;
            }
            catch (Exception exTopRightNegativeInfinity)
            {
                Helpers.Log("Exception caught for NegativeInfinity CornerRadius.TopRight.");
                Helpers.Log(exTopRightNegativeInfinity.Message);
            }
            #endregion

            #region BottomLeft
            try
            {
                _childone.CornerRadius = new CornerRadius(0, 0, 0, double.NaN);
                _tempResult = false;
            }
            catch (Exception exBottomLeftNAN)
            {
                Helpers.Log("Exception caught for NAN CornerRadius.BottomLeft.");
                Helpers.Log(exBottomLeftNAN.Message);
            }

            try
            {
                _childone.CornerRadius = new CornerRadius(0, 0, 0, double.PositiveInfinity);
                _tempResult = false;
            }
            catch (Exception exBottomLeftPositiveIfinity)
            {
                Helpers.Log("Exception caught for PositiveInfinity CornerRadius.BottomLeft.");
                Helpers.Log(exBottomLeftPositiveIfinity.Message);
            }

            try
            {
                _childone.CornerRadius = new CornerRadius(0, 0, 0, double.NegativeInfinity);
                _tempResult = false;
            }
            catch (Exception exBottomLeftNegativeInfinity)
            {
                Helpers.Log("Exception caught for NegativeInfinity CornerRadius.BottomLeft.");
                Helpers.Log(exBottomLeftNegativeInfinity.Message);
            }
            #endregion

            #region BottomRight
            try
            {
                _childone.CornerRadius = new CornerRadius(0, 0, double.NaN, 0);
                _tempResult = false;
            }
            catch (Exception exBottomRightNAN)
            {
                Helpers.Log("Exception caught for NAN CornerRadius.BottomRight.BottomRight.");
                Helpers.Log(exBottomRightNAN.Message);
            }

            try
            {
                _childone.CornerRadius = new CornerRadius(0, 0, double.PositiveInfinity, 0);
                _tempResult = false;
            }
            catch (Exception exBottomRightPositiveIfinity)
            {
                Helpers.Log("Exception caught for PositiveInfinity CornerRadius.BottomRight.");
                Helpers.Log(exBottomRightPositiveIfinity.Message);
            }

            try
            {
                _childone.CornerRadius = new CornerRadius(0, 0, double.NegativeInfinity, 0);
                _tempResult = false;
            }
            catch (Exception exBottomRightNegativeInfinity)
            {
                Helpers.Log("Exception caught for NegativeInfinity CornerRadius.BottomRight.");
                Helpers.Log(exBottomRightNegativeInfinity.Message);
            }
            #endregion

            #region Equals

            _crone = new CornerRadius(10, 10, 10, 50);
            _crtwo = new CornerRadius(10, 10, 10, 10);

            if (_crone.Equals(_crtwo) || _crtwo.Equals(_crone))
            {
                Helpers.Log("CornerRadius's should not be equal [Equals Compare 1].");
                _tempResult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 10, 10, 10);
            _crtwo = new CornerRadius(10, 10, 50, 10);

            if (_crone.Equals(_crtwo) || _crtwo.Equals(_crone))
            {
                Helpers.Log("CornerRadius's should not be equal [Equals Compare 2].");
                _tempResult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 50, 10, 10);
            _crtwo = new CornerRadius(10, 10, 10, 10);

            if (_crone.Equals(_crtwo) || _crtwo.Equals(_crone))
            {
                Helpers.Log("CornerRadius's should not be equal [Equals Compare 3].");
                _tempResult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 10, 10, 10);
            _crtwo = new CornerRadius(50, 10, 10, 10);

            if (_crone.Equals(_crtwo) || _crtwo.Equals(_crone))
            {
                Helpers.Log("CornerRadius's should not be equal [Equals Compare 4].");
                _tempResult = false;
            }

            _crone = new CornerRadius(10, 10, 10, 50);
            _crtwo = new CornerRadius(10, 10, 10, 10);

            if (_crtwo == _crone)
            {
                Helpers.Log("CornerRadius's should not be equal [Compare 1].");
                _tempResult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 10, 10, 10);
            _crtwo = new CornerRadius(10, 10, 50, 10);

            if (_crtwo == _crone)
            {
                Helpers.Log("CornerRadius's should not be equal [Compare 2].");
                _tempResult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 50, 10, 10);
            _crtwo = new CornerRadius(10, 10, 10, 10);

            if (_crtwo == _crone)
            {
                Helpers.Log("CornerRadius's should not be equal [Compare 3].");
                _tempResult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 10, 10, 10);
            _crtwo = new CornerRadius(50, 10, 10, 10);

            if (_crtwo == _crone)
            {
                Helpers.Log("CornerRadius's should not be equal [Compare 4].");
                _tempResult = false;
            }


            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 10, 10, 10);
            _crtwo = new CornerRadius(10, 10, 10, 10);

            if (_crtwo == _crone)
            {
            }
            else
            {
                Helpers.Log("CornerRadius's should be equal [Compare 5].");
                _tempResult = false;
            }


            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 10, 10, 10);
            _crtwo = new CornerRadius(10, 10, 10, 10);

            if (_crtwo != _crone)
            {
                Helpers.Log("CornerRadius's are equal [Compare 6].");
                _tempResult = false;
            }

            CommonFunctionality.FlushDispatcher();

            _crone = new CornerRadius(10, 10, 10, 10);
            _crtwo = new CornerRadius(150, 150, 150, 150);

            if (_crtwo != _crone)
            {
            }
            else
            {
                Helpers.Log("CornerRadius's are not equal [Compare 7].");
                _tempResult = false;
            }

            #endregion

        }

        public override void TestVerify()
        {
            if (!_tempResult)
            {
                Helpers.Log("CornerRadius Test FAILED.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("CornerRadius Test PASSED.");
                this.Result = true;
            }
        }
    }

    #endregion

    [Test(2, "Panels.Border", "AutosizedChild", 
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class AutosizedChild : VisualScanTest
    {
        public AutosizedChild()
            : base("AutosizedChild.xaml")
        { }
    }

    [Test(2, "Panels.Border", "RectangleBod",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class RectangleBod : VisualScanTest
    {
        public RectangleBod()
            : base("RectangleBod.xaml")
        { }
    }
    
    [Test(2, "Panels.Border", "BorderThickness",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class BorderThickness : VisualScanTest
    {
        public BorderThickness()
            : base("BorderThickness.xaml")
        { }
    }
    
    [Test(2, "Panels.Border", "BorderBackgroundHex",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class BorderBackgroundHex : VisualScanTest
    {
        public BorderBackgroundHex()
            : base("BorderBackgroundHex.xaml")
        { }
    }

    [Test(2, "Panels.Border", "BorderBorderBrush",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class BorderBorderBrush : VisualScanTest
    {
        public BorderBorderBrush()
            : base("BorderBorderBrush.xaml")
        { }
    }

    [Test(2, "Panels.Border", "BorderBorderThickness",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class BorderBorderThickness : VisualScanTest
    {
        public BorderBorderThickness()
            : base("BorderBorderThickness.xaml")
        { }
    }

    [Test(2, "Panels.Border", "BorderWidthHeight",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class BorderWidthHeight : VisualScanTest
    {
        public BorderWidthHeight()
            : base("BorderWidthHeight.xaml")
        { }
    }
    
    [Test(2, "Panels.Border",    "BorderRevertFlowDirection",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN", Disabled = true)]
    public class BorderRevertFlowDirection : VisualScanTest
    {
        public BorderRevertFlowDirection()
            : base("BorderRevertFlowDirection.xaml")
        { }
    }

    [Test(2, "Panels.Border", "PolygonBod",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class PolygonBod : VisualScanTest
    {
        public PolygonBod()
            : base("PolygonBod.xaml")
        { }
    }

    [Test(2, "Panels.Border", "BorderEllipseBig",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class BorderEllipseBig : VisualScanTest
    {
        public BorderEllipseBig()
            : base("BorderEllipseBig.xaml")
        { }
    }
}
