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
using System.Windows.Documents;
using System.Windows.Markup;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.Decorator", "FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Decorator, test=HeightWidthDefault")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.HeightWidthActual", TestParameters = "target=Decorator, test=HeightWidthActual")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Decorator, test=ChildHeightWidth")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.MinHeightWidth", TestParameters = "target=Decorator, test=MinHeightWidth")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Decorator, test=MaxHeightWidth")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.Visibility", TestParameters = "target=Decorator, test=Visibility")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Decorator, test=HorizontalAlignment")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.VerticalAlignment", TestParameters = "target=Decorator, test=VerticalAlignment")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.FlowDirection", TestParameters = "target=Decorator, test=FlowDirection")]
    [Test(1, "Panels.Decorator", "FrameworkElementProps.Margin", TestParameters = "target=Decorator, test=Margin")]
    public class DecoratorFETest : FrameworkElementPropertiesTest
    {
        public DecoratorFETest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }
    }

    [Test(1, "Panels.Decorator", "DecoratorFrameworkElementProps", Variables="Area=ElementLayout")]
    public class DecoratorFrameworkElementProps : CodeTest
    {
        public DecoratorFrameworkElementProps()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Decorator _decorator;
        Border _child;
        Grid _root;
        double _staticWidth = 300;
        double _staticHeight = 250;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.DarkGray;

            _decorator = new Decorator();
            _decorator.Margin = new Thickness(10);

            _child = new Border();
            _child.Background = Brushes.LawnGreen;
            _child.Margin = new Thickness(10);

            _decorator.Child = _child;

            _root.Children.Add(_decorator);

            return _root;
        }

        public override void TestActions()
        {
            Helpers.Log("---------------------------------------------");
            Helpers.Log("Height / Widht Test : ");
            Helpers.Log("Decorator Height and Width have not been set.");
            Helpers.Log("Properties should be NaN.");
            Helpers.Log("---------------------------------------------");

            if (Double.IsNaN(_decorator.Width) && Double.IsNaN(_decorator.Height))
            {
                Helpers.Log("Decorator Height and Width ARE NaN (value if properties are unset).");

                if (_decorator.ActualWidth != (_child.ActualWidth + _child.Margin.Left + _child.Margin.Right))
                {
                    Helpers.Log("child width IS NOT equal to decorator width - child margins");
                    //Helpers.Log("heigt/width";
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("child width IS equal to decorator width - child margins");
                }

                if (_decorator.ActualHeight != (_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom))
                {
                    Helpers.Log("child height IS NOT equal to decorator height - child margins");
                    //Helpers.Log("heigt/width";
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("child height IS equal to decorator height - child margins");
                }
            }
            else
            {
                Helpers.Log("Decorator Height and Width ARE NOT NaN (value if properties are unset).");
                //Helpers.Log("heigt/width";
                this.Result = false;
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("Height/Width Test : ");
            Helpers.Log("Setting Height and Width on Decorator.");
            Helpers.Log("Properties should not be NaN.");
            Helpers.Log("---------------------------------------------");
            _decorator.Height = _staticHeight;
            _decorator.Width = _staticWidth;
            CommonFunctionality.FlushDispatcher();

            if (!Double.IsNaN(_decorator.Width) && !Double.IsNaN(_decorator.Height))
            {
                Helpers.Log("Decorator Height and Width ARE NOT NaN (value if properties are unset).");

                if (_decorator.ActualWidth != (_child.ActualWidth + _child.Margin.Left + _child.Margin.Right))
                {
                    Helpers.Log("child width IS NOT equal to decorator width - child margins");
                    //Helpers.Log("heigt/width";
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("child width IS equal to decorator width - child margins");
                }

                if (_decorator.ActualHeight != (_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom))
                {
                    Helpers.Log("child height IS NOT equal to decorator height - child margins");
                    //Helpers.Log("heigt/width";
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("child height IS equal to decorator height - child margins");
                }
            }
            else
            {
                Helpers.Log("Decorator Height and Width ARE NaN (value if properties are unset).");
                //Helpers.Log("heigt/width";
                this.Result = false;
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("Min Test : ");
            Helpers.Log("Setting Min Height and Width.");
            Helpers.Log("---------------------------------------------");

            _decorator.Height = double.NaN;
            _decorator.Width = double.NaN;
            _decorator.MinHeight = _staticHeight * 2;
            _decorator.MinWidth = _staticWidth * 2;
            _root.Height = 100;
            _root.Width = 100;
            CommonFunctionality.FlushDispatcher();

            if (
                (_decorator.ActualHeight != _staticHeight * 2) ||
                (_decorator.ActualWidth != _staticWidth * 2) ||
                !(_decorator.ActualHeight > _root.ActualHeight) ||
                !(_decorator.ActualWidth > _root.ActualWidth))
            {
                Helpers.Log("Decorator Min Height / Width failed.");
                //Helpers.Log("min test";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Min Height / Width passed.");
            }


            Helpers.Log("---------------------------------------------");
            Helpers.Log("Max Test : ");
            Helpers.Log("Setting Max Height and Width.");
            Helpers.Log("---------------------------------------------");

            _decorator.MinHeight = 0;
            _decorator.MinWidth = 0;
            _root.Height = double.NaN;
            _root.Width = double.NaN;
            _decorator.MaxHeight = _staticHeight / 2;
            _decorator.MaxWidth = _staticWidth / 2;
            _child.Height = 1000;
            _child.Width = 1000;
            CommonFunctionality.FlushDispatcher();

            if (
                (_decorator.ActualHeight != _staticHeight / 2) ||
                (_decorator.ActualWidth != _staticWidth / 2) ||
                !(_child.ActualHeight > _decorator.ActualHeight) ||
                !(_child.ActualWidth > _decorator.ActualWidth))
            {
                Helpers.Log("Decorator Max Height / Width failed.");
                //Helpers.Log("max test";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Max Height / Width passed.");
            }


            _decorator.MaxHeight = double.PositiveInfinity;
            _decorator.MaxWidth = double.PositiveInfinity;
            _decorator.Height = _staticHeight;
            _decorator.Width = _staticWidth;
            _child.Height = double.NaN;
            _child.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();

            Helpers.Log("---------------------------------------------");
            Helpers.Log("Visiblity Test : ");
            Helpers.Log("Setting Visibility (Visible, Hidden, Collapsed)");
            Helpers.Log("---------------------------------------------");


            _decorator.Visibility = Visibility.Visible;
            CommonFunctionality.FlushDispatcher();

            if (_decorator.ActualHeight != _staticHeight || _decorator.ActualWidth != _staticWidth)
            {
                Helpers.Log("Decorator Visilbity.Visilbe failed.");
                //Helpers.Log("visilbity.visible";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Visilbity.Visilbe passed.");
            }

            _decorator.Visibility = Visibility.Hidden;
            CommonFunctionality.FlushDispatcher();

            Point HitTestPoint = new Point((_root.ActualWidth / 2), (_root.ActualHeight / 2));

            IInputElement decoratorHitTest;
            decoratorHitTest = _root.InputHitTest(HitTestPoint);

            if ((_decorator.ActualHeight != _staticHeight || _decorator.ActualWidth != _staticWidth) && (decoratorHitTest.GetType().ToString() != "System.Windows.Controls.Grid"))
            {
                Helpers.Log("Decorator Visilbity.Hidden failed.");
                //Helpers.Log("visilbity.hidden";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Visilbity.Hidden passed.");
            }

            _decorator.Visibility = Visibility.Collapsed;
            CommonFunctionality.FlushDispatcher();

            decoratorHitTest = _root.InputHitTest(HitTestPoint);

            if ((_decorator.ActualHeight != 0 || _decorator.ActualWidth != 0) && (decoratorHitTest.GetType().ToString() != "System.Windows.Controls.Grid"))
            {
                Helpers.Log("Decorator Visilbity.Collapsed failed.");
                //Helpers.Log("visilbity.collapsed";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Visilbity.Collapsed passed.");
            }


            Helpers.Log("---------------------------------------------");
            Helpers.Log("HorizontalAlignment Test : ");
            Helpers.Log("Setting to Stretch, Center, Left, Right)");
            Helpers.Log("---------------------------------------------");

            _decorator.Visibility = Visibility.Visible;
            _decorator.HorizontalAlignment = HorizontalAlignment.Left;
            CommonFunctionality.FlushDispatcher();

            Point decoratorLoc;
            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.X != (0 + _decorator.Margin.Left))
            {
                Helpers.Log("Decorator HorizontalAlignment.Left failed.");
                //Helpers.Log("HorizontalAlignment.Left";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator HorizontalAlignment.Left passed.");
            }

            _decorator.HorizontalAlignment = HorizontalAlignment.Right;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.X != (_root.ActualWidth - (_decorator.ActualWidth + _decorator.Margin.Right)))
            {
                Helpers.Log("Decorator HorizontalAlignment.Right failed.");
                //Helpers.Log("HorizontalAlignment.right";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator HorizontalAlignment.Right passed.");
            }

            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.X != ((_root.ActualWidth / 2) - (_decorator.ActualWidth / 2)))
            {
                Helpers.Log("Decorator HorizontalAlignment.Center failed.");
                //Helpers.Log("HorizontalAlignment.center";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator HorizontalAlignment.Center passed.");
            }

            _decorator.Height = double.NaN;
            _decorator.Width = double.NaN;
            _decorator.HorizontalAlignment = HorizontalAlignment.Stretch;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if ((decoratorLoc.X != (0 + _decorator.Margin.Left)) && (_decorator.ActualWidth != (_root.ActualWidth - _decorator.Margin.Left - _decorator.Margin.Right)))
            {
                Helpers.Log("Decorator HorizontalAlignment.Stretch failed.");
                //Helpers.Log("HorizontalAlignment.stretch";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator HorizontalAlignment.Stretch passed.");
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("VerticalAlignment Test : ");
            Helpers.Log("Setting to Stretch, Center, Top, Bottom)");
            Helpers.Log("---------------------------------------------");

            _decorator.Height = _staticHeight;
            _decorator.Width = _staticWidth;
            _decorator.VerticalAlignment = VerticalAlignment.Top;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.Y != (0 + _decorator.Margin.Top))
            {
                Helpers.Log("Decorator VerticalAlignment.Top failed.");
                //Helpers.Log("VerticalAlignment.Top";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator VerticalAlignment.Top passed.");
            }

            _decorator.VerticalAlignment = VerticalAlignment.Bottom;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.Y != (_root.ActualHeight - (_decorator.ActualHeight + _decorator.Margin.Bottom)))
            {
                Helpers.Log("Decorator VerticalAlignment.Bottom failed.");
                //Helpers.Log("VerticalAlignment.bottom";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator VerticalAlignment.Bottom passed.");
            }

            _decorator.VerticalAlignment = VerticalAlignment.Center;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.Y != ((_root.ActualHeight / 2) - (_decorator.ActualHeight / 2)))
            {
                Helpers.Log("Decorator VerticalAlignment.Center failed.");
                //Helpers.Log("VerticalAlignment.center";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator VerticalAlignment.Center passed.");
            }

            _decorator.Height = double.NaN;
            _decorator.Width = double.NaN;
            _decorator.VerticalAlignment = VerticalAlignment.Stretch;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if ((decoratorLoc.Y != (0 + _decorator.Margin.Top)) && (_decorator.ActualHeight != (_root.ActualHeight - _decorator.Margin.Top - _decorator.Margin.Bottom)))
            {
                Helpers.Log("Decorator VerticalAlignment.Stretch failed.");
                //Helpers.Log("VerticalAlignment.stretch";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator VerticalAlignment.Stretch passed.");
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("FlowDirection Test : ");
            Helpers.Log("Setting to Left - Right, Right - Left");
            Helpers.Log("---------------------------------------------");


            _decorator.FlowDirection = FlowDirection.RightToLeft;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (!(decoratorLoc.X >= _decorator.ActualWidth))
            {
                Helpers.Log("Decorator FlowDirection.RightToLeft failed.");
                //Helpers.Log("right - left";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator FlowDirection.RightToLeft passed.");
            }

            _decorator.FlowDirection = FlowDirection.LeftToRight;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (!(decoratorLoc.X == (0 + _decorator.Margin.Left)))
            {
                Helpers.Log("Decorator FlowDirection.LeftToRight failed.");
                //Helpers.Log("left - right";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator FlowDirection.LeftToRight passed.");
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("Margin Test : ");
            Helpers.Log("Testing Margin on Decorator.");
            Helpers.Log("---------------------------------------------");

            _decorator.Margin = new Thickness(30, 75, 9, 102);
            CommonFunctionality.FlushDispatcher();

            Point LeftMarginHitTestPoint = new Point(RandomDoubleWithinMargin(0, _decorator.Margin.Left), (_root.ActualWidth / 2));

            IInputElement LeftMarginHitTest;
            LeftMarginHitTest = _root.InputHitTest(LeftMarginHitTestPoint);

            if (LeftMarginHitTest.GetType().ToString() != _decorator.Parent.GetType().ToString())
            {
                Helpers.Log("Decorator Margin.Left failed.");
                //Helpers.Log("Margin.Left";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Margin.Left passed.");
            }

            Point RightMarginHitTestPoint = new Point((_decorator.ActualWidth + _decorator.Margin.Left + RandomDoubleWithinMargin(0, _decorator.Margin.Right)), (_root.ActualWidth / 2));

            IInputElement RightMarginHitTest;
            RightMarginHitTest = _root.InputHitTest(RightMarginHitTestPoint);

            if (RightMarginHitTest.GetType().ToString() != _decorator.Parent.GetType().ToString())
            {
                Helpers.Log("Decorator Margin.Right failed.");
                //Helpers.Log("Margin.Right";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Margin.Right passed.");
            }

            Point TopMarginHitTestPoint = new Point((_root.ActualHeight / 2), RandomDoubleWithinMargin(0, _decorator.Margin.Top));

            IInputElement TopMarginHitTest;
            TopMarginHitTest = _root.InputHitTest(TopMarginHitTestPoint);

            if (TopMarginHitTest.GetType().ToString() != _decorator.Parent.GetType().ToString())
            {
                Helpers.Log("Decorator Margin.Top failed.");
                //Helpers.Log("Margin.Top";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Margin.Top passed.");
            }

            Point BottomMarginHitTestPoint = new Point((_root.ActualHeight / 2), (_decorator.ActualHeight + _decorator.Margin.Top + RandomDoubleWithinMargin(0, _decorator.Margin.Bottom)));

            IInputElement BottomMarginHitTest;
            BottomMarginHitTest = _root.InputHitTest(BottomMarginHitTestPoint);

            if (BottomMarginHitTest.GetType().ToString() != _decorator.Parent.GetType().ToString())
            {
                Helpers.Log("Decorator Margin.Bottom failed.");
                //Helpers.Log("Margin.Bottom";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Margin.Bottom passed.");
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

        int RandomDoubleWithinMargin(double Min, double Max)
        {
            Random random = new Random();
            int WithInMargin = random.Next((int)(Min + 1), (int)Max);
            return WithInMargin;
        }


    }

    [Test(1, "Panels.Decorator", "IAddChildTest.NonUIElement", TestParameters = "target=NonUIElement")]
    [Test(1, "Panels.Decorator", "IAddChildTest.Text", TestParameters = "target=Text")]
    [Test(1, "Panels.Decorator", "IAddChildTest.TextWhiteSpace", TestParameters = "target=TextWhiteSpace")]
    [Test(1, "Panels.Decorator", "IAddChildTest.MultipleChild", TestParameters = "target=MultipleChild")]
    //[Test(1, "Panels.Decorator", "DecoratorIAddChildTest", Variables="Area=ElementLayout")]
    public class DecoratorIAddChildTest : CodeTest
    {
        public DecoratorIAddChildTest() { }

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
            Grid root = new Grid();
            root.Background = Brushes.LightYellow;

            _decorator = new Decorator();

            root.Children.Add(_decorator);

            return root;
        }

        public override void TestActions()
        {
            string target = DriverState.DriverParameters["target"].ToLowerInvariant();

            switch (target)
            { 
                case "nonuielement":
                    //case "NonUIElement":
                    Table tbl = new Table();
                    try
                    {
                        ((IAddChild)_decorator).AddChild(tbl);
                        this.Result = false;
                        Helpers.Log("Exception was not thrown when Non-UiElement was added as Decorator Child.\n");
                    }
                    catch (ArgumentException argEx)
                    {
                        Helpers.Log("Non-UIElement as Child test passed.\nException Caught:\n" + argEx.Message);
                        this.Result = true;
                    }
                    break;
                case "text":
                    //case "Text":
                    try
                    {
                        ((IAddChild)_decorator).AddText("          test        ");
                        this.Result = false;
                        Helpers.Log("Exception was not thrown when Text was added as Decorator Child.\n");
                    }
                    catch (Exception ex)
                    {
                        Helpers.Log("Text as Child test passed.\nException Caught:\n" + ex.Message);
                        this.Result = true;
                    }
                    break;
                case "textwhitespace":
                    //case "TextWhiteSpace":
                    try
                    {
                        ((IAddChild)_decorator).AddText("                      ");
                        Helpers.Log("Only WhiteSpace Text as Child test passed.\nNo Exception Thrown.");
                        this.Result = true;
                    }
                    catch (Exception ex)
                    {
                        GlobalLog.LogEvidence(ex);
                        this.Result = false;
                    }
                    break;
                case "multiplechild":
                    //case "MultipleChild":
                    _decorator.Child = null;
                    CommonFunctionality.FlushDispatcher();
                    Border b = new Border();
                    Border b2 = new Border();
                    _decorator.Child = b;
                    try
                    {
                        ((IAddChild)_decorator).AddChild(b2);
                        this.Result = false;
                        Helpers.Log("Exception was not thrown when Second Child was added to Decorator.\n");
                    }
                    catch (ArgumentException argEx)
                    {
                        Helpers.Log("Multiple Child test passed.\nException Caught:\n" + argEx.Message);
                        this.Result = true;
                    }
                    break;
            }
        }

        private Decorator _decorator = null;
    }

    [Test(2, "Panels.Decorator", "CustomDecoratorAddRemoveChild", Variables="Area=ElementLayout")]
    public class CustomDecoratorAddRemoveChild : CodeTest
    {

        
        public CustomDecoratorAddRemoveChild()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        TestPanel _decorator;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Firebrick;

            _decorator = new TestPanel();
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            _root.Children.Add(_decorator);

            return _root;
        }

        public override void TestActions()
        {
            _decorator.Child = addChild();
            CommonFunctionality.FlushDispatcher();
            if (_decorator.Child == null)
            {
                Helpers.Log("Could not Add Child to Decorator.");
                this.Result = false;
            }

            _decorator.Child = null;
            CommonFunctionality.FlushDispatcher();
            if (_decorator.Child != null)
            {
                Helpers.Log("Could not Remove Child to Decorator.");
                this.Result = false;
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

        FrameworkElement addChild()
        {
            Rectangle child = new Rectangle();
            child.Margin = new Thickness(60, 40, 25, 25);
            child.Height = 400;
            child.Width = 350;
            child.Fill = Brushes.Orange;

            return child;
        }

        public class TestPanel : Decorator
        {
            public TestPanel()
                : base()
            {
            }

            protected override Size MeasureOverride(Size constraint)
            {
                UIElement child = Child;
                if (child != null)
                {
                    child.Measure(constraint);
                    return (child.DesiredSize);
                }
                return (new Size());
            }

            /// <summary>
            /// Decorator computes the position of its single child inside child's Margin and calls Arrange
            /// on the child.
            /// </summary>
            /// <param name="arrangeSize">Size the Decorator will assume.</param>
            protected override Size ArrangeOverride(Size arrangeSize)
            {
                UIElement child = Child;
                if (child != null)
                {
                    child.Arrange(new Rect(arrangeSize));
                }
                return (arrangeSize);
            }
        }

    }

    [Test(2, "Panels.Decorator", "CustomDecoratorChildSize", Variables="Area=ElementLayout")]
    public class CustomDecoratorChildSize : CodeTest
    {

        
        public CustomDecoratorChildSize()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        TestPanel _decorator;
        Rectangle _child;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.Firebrick;

            _decorator = new TestPanel();
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            _child = new Rectangle();
            _child.Margin = new Thickness(60, 40, 25, 25);
            _child.Height = 400;
            _child.Width = 350;
            _child.Fill = Brushes.Orange;

            _decorator.Child = _child;

            _root.Children.Add(_decorator);

            return _root;
        }

        public override void TestActions()
        {
            if (_decorator.ActualHeight != (_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom) || _decorator.ActualWidth != (_child.ActualWidth + _child.Margin.Left + _child.Margin.Right))
            {
                Helpers.Log("Decorator Size is not equal to Child Size + Margins");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Size is equal to Child Size + Margins");
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

        public class TestPanel : Decorator
        {
            public TestPanel()
                : base()
            {
            }

            protected override Size MeasureOverride(Size constraint)
            {
                UIElement child = Child;
                if (child != null)
                {
                    child.Measure(constraint);
                    return (child.DesiredSize);
                }
                return (new Size());
            }

            /// <summary>
            /// Decorator computes the position of its single child inside child's Margin and calls Arrange
            /// on the child.
            /// </summary>
            /// <param name="arrangeSize">Size the Decorator will assume.</param>
            protected override Size ArrangeOverride(Size arrangeSize)
            {
                UIElement child = Child;
                if (child != null)
                {
                    child.Arrange(new Rect(arrangeSize));
                }
                return (arrangeSize);
            }
        }
    }

    [Test(2, "Panels.Decorator", "CustomDecoratorFEProps", Variables="Area=ElementLayout")]
    public class CustomDecoratorFEProps : CodeTest
    {

        
        public CustomDecoratorFEProps()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        TestPanel _decorator;
        Border _child;
        Grid _root;

        double _staticWidth = 300;
        double _staticHeight = 250;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.DarkGray;

            _decorator = new TestPanel();
            _decorator.Margin = new Thickness(10);

            _child = new Border();
            _child.Background = Brushes.LawnGreen;
            _child.Margin = new Thickness(10);

            _decorator.Child = _child;

            _root.Children.Add(_decorator);

            return _root;
        }

        public override void TestActions()
        {
            Helpers.Log("---------------------------------------------");
            Helpers.Log("Height / Widht Test : ");
            Helpers.Log("Decorator Height and Width have not been set.");
            Helpers.Log("Properties should be NaN.");
            Helpers.Log("---------------------------------------------");

            if (Double.IsNaN(_decorator.Width) && Double.IsNaN(_decorator.Height))
            {
                Helpers.Log("Decorator Height and Width ARE NaN (value if properties are unset).");

                if (_decorator.ActualWidth != (_child.ActualWidth + _child.Margin.Left + _child.Margin.Right))
                {
                    Helpers.Log("child width IS NOT equal to decorator width - child margins");
                    //failingTest += "heigt/width";
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("child width IS equal to decorator width - child margins");
                }

                if (_decorator.ActualHeight != (_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom))
                {
                    Helpers.Log("child height IS NOT equal to decorator height - child margins");
                    //failingTest += "heigt/width";
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("child height IS equal to decorator height - child margins");
                }
            }
            else
            {
                Helpers.Log("Decorator Height and Width ARE NOT NaN (value if properties are unset).");
                //failingTest += "heigt/width";
                this.Result = false;
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("Height/Width Test : ");
            Helpers.Log("Setting Height and Width on Decorator.");
            Helpers.Log("Properties should not be NaN.");
            Helpers.Log("---------------------------------------------");
            _decorator.Height = _staticHeight;
            _decorator.Width = _staticWidth;
            CommonFunctionality.FlushDispatcher();

            if (!Double.IsNaN(_decorator.Width) && !Double.IsNaN(_decorator.Height))
            {
                Helpers.Log("Decorator Height and Width ARE NOT NaN (value if properties are unset).");

                if (_decorator.ActualWidth != (_child.ActualWidth + _child.Margin.Left + _child.Margin.Right))
                {
                    Helpers.Log("child width IS NOT equal to decorator width - child margins");
                    //failingTest += "heigt/width";
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("child width IS equal to decorator width - child margins");
                }

                if (_decorator.ActualHeight != (_child.ActualHeight + _child.Margin.Top + _child.Margin.Bottom))
                {
                    Helpers.Log("child height IS NOT equal to decorator height - child margins");
                    //failingTest += "heigt/width";
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("child height IS equal to decorator height - child margins");
                }
            }
            else
            {
                Helpers.Log("Decorator Height and Width ARE NaN (value if properties are unset).");
                //failingTest += "heigt/width";
                this.Result = false;
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("Min Test : ");
            Helpers.Log("Setting Min Height and Width.");
            Helpers.Log("---------------------------------------------");

            _decorator.Height = double.NaN;
            _decorator.Width = double.NaN;
            _decorator.MinHeight = _staticHeight * 2;
            _decorator.MinWidth = _staticWidth * 2;
            _root.Height = 100;
            _root.Width = 100;
            CommonFunctionality.FlushDispatcher();

            if (
                (_decorator.ActualHeight != _staticHeight * 2) ||
                (_decorator.ActualWidth != _staticWidth * 2) ||
                !(_decorator.ActualHeight > _root.ActualHeight) ||
                !(_decorator.ActualWidth > _root.ActualWidth))
            {
                Helpers.Log("Decorator Min Height / Width failed.");
                //failingTest += "min test";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Min Height / Width passed.");
            }


            Helpers.Log("---------------------------------------------");
            Helpers.Log("Max Test : ");
            Helpers.Log("Setting Max Height and Width.");
            Helpers.Log("---------------------------------------------");

            _decorator.MinHeight = 0;
            _decorator.MinWidth = 0;
            _root.Height = double.NaN;
            _root.Width = double.NaN;
            _decorator.MaxHeight = _staticHeight / 2;
            _decorator.MaxWidth = _staticWidth / 2;
            _child.Height = 1000;
            _child.Width = 1000;
            CommonFunctionality.FlushDispatcher();

            if (
                (_decorator.ActualHeight != _staticHeight / 2) ||
                (_decorator.ActualWidth != _staticWidth / 2) ||
                !(_child.ActualHeight > _decorator.ActualHeight) ||
                !(_child.ActualWidth > _decorator.ActualWidth))
            {
                Helpers.Log("Decorator Max Height / Width failed.");
                //failingTest += "max test";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Max Height / Width passed.");
            }


            _decorator.MaxHeight = double.PositiveInfinity;
            _decorator.MaxWidth = double.PositiveInfinity;
            _decorator.Height = _staticHeight;
            _decorator.Width = _staticWidth;
            _child.Height = double.NaN;
            _child.Width = double.NaN;
            CommonFunctionality.FlushDispatcher();

            Helpers.Log("---------------------------------------------");
            Helpers.Log("Visiblity Test : ");
            Helpers.Log("Setting Visibility (Visible, Hidden, Collapsed)");
            Helpers.Log("---------------------------------------------");


            _decorator.Visibility = Visibility.Visible;
            CommonFunctionality.FlushDispatcher();

            if (_decorator.ActualHeight != _staticHeight || _decorator.ActualWidth != _staticWidth)
            {
                Helpers.Log("Decorator Visilbity.Visilbe failed.");
                //failingTest += "visilbity.visible";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Visilbity.Visilbe passed.");
            }

            _decorator.Visibility = Visibility.Hidden;
            CommonFunctionality.FlushDispatcher();

            Point HitTestPoint = new Point((_root.ActualWidth / 2), (_root.ActualHeight / 2));

            IInputElement decoratorHitTest;
            decoratorHitTest = _root.InputHitTest(HitTestPoint);

            if ((_decorator.ActualHeight != _staticHeight || _decorator.ActualWidth != _staticWidth) && (decoratorHitTest.GetType().ToString() != "System.Windows.Controls.Grid"))
            {
                Helpers.Log("Decorator Visilbity.Hidden failed.");
                //failingTest += "visilbity.hidden";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Visilbity.Hidden passed.");
            }

            _decorator.Visibility = Visibility.Collapsed;
            CommonFunctionality.FlushDispatcher();

            decoratorHitTest = _root.InputHitTest(HitTestPoint);

            if ((_decorator.ActualHeight != 0 || _decorator.ActualWidth != 0) && (decoratorHitTest.GetType().ToString() != "System.Windows.Controls.Grid"))
            {
                Helpers.Log("Decorator Visilbity.Collapsed failed.");
                //failingTest += "visilbity.collapsed";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Visilbity.Collapsed passed.");
            }


            Helpers.Log("---------------------------------------------");
            Helpers.Log("HorizontalAlignment Test : ");
            Helpers.Log("Setting to Stretch, Center, Left, Right)");
            Helpers.Log("---------------------------------------------");

            _decorator.Visibility = Visibility.Visible;
            _decorator.HorizontalAlignment = HorizontalAlignment.Left;
            CommonFunctionality.FlushDispatcher();

            Point decoratorLoc;
            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.X != (0 + _decorator.Margin.Left))
            {
                Helpers.Log("Decorator HorizontalAlignment.Left failed.");
                //failingTest += "HorizontalAlignment.Left";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator HorizontalAlignment.Left passed.");
            }

            _decorator.HorizontalAlignment = HorizontalAlignment.Right;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.X != (_root.ActualWidth - (_decorator.ActualWidth + _decorator.Margin.Right)))
            {
                Helpers.Log("Decorator HorizontalAlignment.Right failed.");
                //failingTest += "HorizontalAlignment.right";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator HorizontalAlignment.Right passed.");
            }

            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.X != ((_root.ActualWidth / 2) - (_decorator.ActualWidth / 2)))
            {
                Helpers.Log("Decorator HorizontalAlignment.Center failed.");
                //failingTest += "HorizontalAlignment.center";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator HorizontalAlignment.Center passed.");
            }

            _decorator.Height = double.NaN;
            _decorator.Width = double.NaN;
            _decorator.HorizontalAlignment = HorizontalAlignment.Stretch;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if ((decoratorLoc.X != (0 + _decorator.Margin.Left)) && (_decorator.ActualWidth != (_root.ActualWidth - _decorator.Margin.Left - _decorator.Margin.Right)))
            {
                Helpers.Log("Decorator HorizontalAlignment.Stretch failed.");
                //failingTest += "HorizontalAlignment.stretch";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator HorizontalAlignment.Stretch passed.");
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("VerticalAlignment Test : ");
            Helpers.Log("Setting to Stretch, Center, Top, Bottom)");
            Helpers.Log("---------------------------------------------");

            _decorator.Height = _staticHeight;
            _decorator.Width = _staticWidth;
            _decorator.VerticalAlignment = VerticalAlignment.Top;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.Y != (0 + _decorator.Margin.Top))
            {
                Helpers.Log("Decorator VerticalAlignment.Top failed.");
                //failingTest += "VerticalAlignment.Top";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator VerticalAlignment.Top passed.");
            }

            _decorator.VerticalAlignment = VerticalAlignment.Bottom;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.Y != (_root.ActualHeight - (_decorator.ActualHeight + _decorator.Margin.Bottom)))
            {
                Helpers.Log("Decorator VerticalAlignment.Bottom failed.");
                //failingTest += "VerticalAlignment.bottom";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator VerticalAlignment.Bottom passed.");
            }

            _decorator.VerticalAlignment = VerticalAlignment.Center;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (decoratorLoc.Y != ((_root.ActualHeight / 2) - (_decorator.ActualHeight / 2)))
            {
                Helpers.Log("Decorator VerticalAlignment.Center failed.");
                //failingTest += "VerticalAlignment.center";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator VerticalAlignment.Center passed.");
            }

            _decorator.Height = double.NaN;
            _decorator.Width = double.NaN;
            _decorator.VerticalAlignment = VerticalAlignment.Stretch;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if ((decoratorLoc.Y != (0 + _decorator.Margin.Top)) && (_decorator.ActualHeight != (_root.ActualHeight - _decorator.Margin.Top - _decorator.Margin.Bottom)))
            {
                Helpers.Log("Decorator VerticalAlignment.Stretch failed.");
                //failingTest += "VerticalAlignment.stretch";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator VerticalAlignment.Stretch passed.");
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("FlowDirection Test : ");
            Helpers.Log("Setting to Left - Right, Right - Left");
            Helpers.Log("---------------------------------------------");


            _decorator.FlowDirection = FlowDirection.RightToLeft;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (!(decoratorLoc.X >= _decorator.ActualWidth))
            {
                Helpers.Log("Decorator FlowDirection.RightToLeft failed.");
                //failingTest += "right - left";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator FlowDirection.RightToLeft passed.");
            }

            _decorator.FlowDirection = FlowDirection.LeftToRight;
            CommonFunctionality.FlushDispatcher();

            decoratorLoc = LayoutUtility.GetElementPosition(_decorator, _root);
            if (!(decoratorLoc.X == (0 + _decorator.Margin.Left)))
            {
                Helpers.Log("Decorator FlowDirection.LeftToRight failed.");
                //failingTest += "left - right";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator FlowDirection.LeftToRight passed.");
            }

            Helpers.Log("---------------------------------------------");
            Helpers.Log("Margin Test : ");
            Helpers.Log("Testing Margin on Decorator.");
            Helpers.Log("---------------------------------------------");

            _decorator.Margin = new Thickness(30, 75, 9, 102);
            CommonFunctionality.FlushDispatcher();

            Point LeftMarginHitTestPoint = new Point(RandomDoubleWithinMargin(0, _decorator.Margin.Left), (_root.ActualWidth / 2));

            IInputElement LeftMarginHitTest;
            LeftMarginHitTest = _root.InputHitTest(LeftMarginHitTestPoint);

            if (LeftMarginHitTest.GetType().ToString() != _decorator.Parent.GetType().ToString())
            {
                Helpers.Log("Decorator Margin.Left failed.");
                //failingTest += "Margin.Left";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Margin.Left passed.");
            }

            Point RightMarginHitTestPoint = new Point((_decorator.ActualWidth + _decorator.Margin.Left + RandomDoubleWithinMargin(0, _decorator.Margin.Right)), (_root.ActualWidth / 2));

            IInputElement RightMarginHitTest;
            RightMarginHitTest = _root.InputHitTest(RightMarginHitTestPoint);

            if (RightMarginHitTest.GetType().ToString() != _decorator.Parent.GetType().ToString())
            {
                Helpers.Log("Decorator Margin.Right failed.");
                //failingTest += "Margin.Right";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Margin.Right passed.");
            }

            Point TopMarginHitTestPoint = new Point((_root.ActualHeight / 2), RandomDoubleWithinMargin(0, _decorator.Margin.Top));

            IInputElement TopMarginHitTest;
            TopMarginHitTest = _root.InputHitTest(TopMarginHitTestPoint);

            if (TopMarginHitTest.GetType().ToString() != _decorator.Parent.GetType().ToString())
            {
                Helpers.Log("Decorator Margin.Top failed.");
                //failingTest += "Margin.Top";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Margin.Top passed.");
            }

            Point BottomMarginHitTestPoint = new Point((_root.ActualHeight / 2), (_decorator.ActualHeight + _decorator.Margin.Top + RandomDoubleWithinMargin(0, _decorator.Margin.Bottom)));

            IInputElement BottomMarginHitTest;
            BottomMarginHitTest = _root.InputHitTest(BottomMarginHitTestPoint);

            if (BottomMarginHitTest.GetType().ToString() != _decorator.Parent.GetType().ToString())
            {
                Helpers.Log("Decorator Margin.Bottom failed.");
                //failingTest += "Margin.Bottom";
                this.Result = false;
            }
            else
            {
                Helpers.Log("Decorator Margin.Bottom passed.");
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

        int RandomDoubleWithinMargin(double Min, double Max)
        {
            Random random = new Random();
            int WithInMargin = random.Next((int)(Min + 1), (int)Max);
            return WithInMargin;
        }

        public class TestPanel : Decorator
        {
            public TestPanel()
                : base()
            {
            }

            protected override Size MeasureOverride(Size constraint)
            {
                UIElement child = Child;
                if (child != null)
                {
                    child.Measure(constraint);
                    return (child.DesiredSize);
                }
                return (new Size());
            }

            /// <summary>
            /// Decorator computes the position of its single child inside child's Margin and calls Arrange
            /// on the child.
            /// </summary>
            /// <param name="arrangeSize">Size the Decorator will assume.</param>
            protected override Size ArrangeOverride(Size arrangeSize)
            {
                UIElement child = Child;
                if (child != null)
                {
                    child.Arrange(new Rect(arrangeSize));
                }
                return (arrangeSize);
            }
        }
    }

    #region Content Property Change

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeRectangle", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeRectangle : CodeTest
    {

        
        public DecoratorContentPropChangeRectangle()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        Rectangle _rect;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.SlateBlue));
            _decorator.Child = _rect;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _rect.Width = _rect.ActualWidth * 2;
            _rect.Height = _rect.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeButton", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeButton : CodeTest
    {

        
        public DecoratorContentPropChangeButton()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Decorator _decorator;

        Button _btn;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _btn = CommonFunctionality.CreateButton(200, 200, Brushes.Red);
            _decorator.Child = _btn;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);


            _btn.Width = _btn.ActualWidth * 2;
            _btn.Height = _btn.ActualHeight * 2;
            _btn.Content = "Button Size Changed~!";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeTextBox", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeTextBox : CodeTest
    {

        
        public DecoratorContentPropChangeTextBox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Decorator _decorator;

        TextBox _tbox;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _tbox = CommonFunctionality.CreateTextBox(150, 100, "Width=150px");
            _decorator.Child = _tbox;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _tbox.Width = _tbox.ActualWidth * 2;
            _tbox.Height = _tbox.ActualHeight * 2;
            _tbox.Text = "Size changed * 2";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeEllipse", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeEllipse : CodeTest
    {

        
        public DecoratorContentPropChangeEllipse()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }


        Grid _root;
        Decorator _decorator;

        Ellipse _elps;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _elps = new Ellipse();
            _elps.Width = 150;
            _elps.Height = 150;
            _elps.Fill = new SolidColorBrush(Colors.SlateBlue);
            _decorator.Child = _elps;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _elps.Width = _elps.ActualWidth * 2;
            _elps.Height = _elps.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeImage")]
    public class DecoratorContentPropChangeImage : CodeTest
    {        
        public DecoratorContentPropChangeImage()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        Image _img;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _img = CommonFunctionality.CreateImage("light.bmp");
            _img.Height = 50;
            _img.Width = 50;
            _decorator.Child = _img;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _img.Width = _img.ActualWidth * 2;
            _img.Height = _img.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeText", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeText : CodeTest
    {

        
        public DecoratorContentPropChangeText()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        TextBlock _txt;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _txt = CommonFunctionality.CreateText("Testing Grid...");
            _decorator.Child = _txt;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _txt.Text = "Changing Text to very large text... Changing Text to very large text...";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeBorder", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeBorder : CodeTest
    {

        
        public DecoratorContentPropChangeBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        Border _b;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _b = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.SlateBlue), 25, 200);
            _decorator.Child = _b;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _b.Width = _b.ActualWidth * 2;
            _b.Height = _b.ActualHeight * 2;
            _b.BorderThickness = new Thickness(20);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeLabel", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeLabel : CodeTest
    {

        
        public DecoratorContentPropChangeLabel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        Label _lbl;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lbl = new Label();
            _lbl.Content = "Testing decoratorPanel with Label~!";
            _decorator.Child = _lbl;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _lbl.Content = "content changed. content changed.content changed. content changed. content changed. content changed. content changed.";
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeListBox", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeListBox : CodeTest
    {

        
        public DecoratorContentPropChangeListBox()
        {

            }public override void WindowSetup() { this.window.Height= 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        ListBox _lb;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _lb = CommonFunctionality.CreateListBox(10);
            _decorator.Child = _lb;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ListBoxItem li = new ListBoxItem();
            li.Content = "List Item added with Long Text...~";
            _lb.Items.Add(li);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeMenu", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeMenu : CodeTest
    {

        
        public DecoratorContentPropChangeMenu()
        {

            }public override void WindowSetup() { this.window.Height= 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        Menu _menu;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _menu = CommonFunctionality.CreateMenu(5);
            _decorator.Child = _menu;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            MenuItem mi = new MenuItem();
            mi.Header = "Menu Item Added~!!!";
            _menu.Items.Add(mi);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeCanvas", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeCanvas : CodeTest
    {

        
        public DecoratorContentPropChangeCanvas()
        {

            }public override void WindowSetup() { this.window.Height= 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        Canvas _canvas;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _canvas = new Canvas();
            _canvas.Height = 100;
            _canvas.Width = 100;
            _canvas.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle cRect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(cRect, 10);
            Canvas.SetTop(cRect, 10);
            _canvas.Children.Add(cRect);
            _decorator.Child = _canvas;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

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
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeDockPanel", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeDockPanel : CodeTest
    {

        
        public DecoratorContentPropChangeDockPanel()
        {

            }public override void WindowSetup() { this.window.Height= 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        DockPanel _dockpanel;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

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
            _decorator.Child = _dockpanel;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            _dockpanel.Width = _dockpanel.ActualWidth * 2;
            _dockpanel.Height = _dockpanel.ActualHeight * 2;
            DockPanel.SetDock(_dockpanel.Children[0], Dock.Right);
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeStackPanel", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeStackPanel : CodeTest
    {

        
        public DecoratorContentPropChangeStackPanel()
        {

            }public override void WindowSetup() { this.window.Height= 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        StackPanel _s;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

            //content that will have a prop change.
            _s = new StackPanel();
            _s.Width = 200;
            _decorator.Child = _s;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

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
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
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

    [Test(3, "Panels.Decorator", "DecoratorContentPropChangeGrid", Variables="Area=ElementLayout")]
    public class DecoratorContentPropChangeGrid : CodeTest
    {

        
        public DecoratorContentPropChangeGrid()
        {

            }public override void WindowSetup() { this.window.Height= 800;
            this.window.Width = 800;

            this.window.Content = this.TestContent();
        }

        Grid _root;
        Decorator _decorator;

        Grid _g;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();

            _decorator = new Decorator();
            //decorator.Background = Brushes.RoyalBlue;
            _decorator.HorizontalAlignment = HorizontalAlignment.Center;
            _decorator.VerticalAlignment = VerticalAlignment.Center;

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

            _decorator.Child = _g;

            _root.Children.Add(_decorator);
            return _root;
        }

        public override void TestActions()
        {
            CommonFunctionality.FlushDispatcher();
            _preTestSize = _decorator.RenderSize;

            _relayoutOccurred = false;
            _decorator.LayoutUpdated += new EventHandler(OnLayoutOccured);

            ColumnDefinition ccd = new ColumnDefinition();
            _g.ColumnDefinitions.Add(ccd);
            _g.Width = _g.ActualWidth * 2;
            _g.Height = _g.ActualHeight * 2;
        }

        public override void TestVerify()
        {
            Helpers.Log("Pre-Test Size : " + _preTestSize);
            Helpers.Log("After Update Size : " + _decorator.RenderSize);

            if (_relayoutOccurred)
            {
                Helpers.Log("Layout updated, check size change.");
                if (_decorator.RenderSize == _preTestSize)
                {
                    Helpers.Log("Layout updated, but decoratorPanel size was not updated!!!");
                    this.Result = false;
                }
                else
                {
                    Helpers.Log("Layout updated and decoratorPanel Size Changed!!!");
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

    [Test(1, "Panels.Decorator", "DecoratorGetVisualChildExceptionTest", Variables="Area=ElementLayout")]
    public class DecoratorGetVisualChildExceptionTest : CodeTest
    {

        
        public DecoratorGetVisualChildExceptionTest()
        {

            }public override void WindowSetup() { this.window.Height= 800;
            this.window.Width = 800;
            this.window.Top = 50;
            this.window.Left = 50;

            this.window.Content = this.TestContent();
        }

        Decorator _decorator;

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();

            _decorator = new Decorator();

            Border b = new Border();
            b.Height = 100;
            b.Width = 100;
            b.Background = Brushes.CadetBlue;

            _decorator.Child = b;

            root.Children.Add(_decorator);

            return root;
        }

        public override void TestActions()
        {
            try
            {
                VisualTreeHelper.GetChild(_decorator, 5);
                this.Result = false;
                Helpers.Log("Exception was not thrown when GetChild called on Child Out Of Range item.\n");
            }
            catch (ArgumentOutOfRangeException aex)
            {
                Helpers.Log("Child Out Of Range test passed.\nException Caught:\n" + aex.Message);
            }
        }

        bool _tempresult = true;
        public override void TestVerify()
        {
            this.Result = _tempresult;
        }
    }
}
