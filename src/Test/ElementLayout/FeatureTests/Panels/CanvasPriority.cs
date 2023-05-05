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
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This contains all StackPanel code priority cases.
    /// 
    /// Possible Tests :
    /// - CanvasBasicFuncTest1
    /// - CanvasBasicFuncTest2
    /// - CanvasBasicFuncTest3
    /// - CanvasBasicFuncTest4
    /// - CanvasBasicFuncTest5
    /// - CanvasBasicFuncTest6
    /// - CanvasBasicFuncTest7
    /// - CanvasBasicFuncTest8
    /// - CanvasBasicFuncTest9
    /// - CanvasBasicFuncTest10
    /// - CanvasBasicFuncTest11
    /// - CanvasBasicFuncTest12
    /// - CanvasBasicFuncTest13
    /// - CanvasBasicFuncTest14
    /// - CanvasRelayoutOnPropChangeTest1
    /// - CanvasRelayoutOnPropChangeTest2
    /// - CanvasRelayoutOnPropChangeTest3
    /// - CanvasRelayoutOnPropChangeTest4
    /// - CanvasRelayoutOnPropChangeTest5
    /// - CanvasRelayoutOnPropChangeTest6
    /// - CanvasRelayoutOnPropChangeTest7
    /// - CanvasRelayoutOnPropChangeTest8
    /// - CanvasRelayoutOnPropChangeTest9
    /// - CanvasRelayoutOnPropChangeTest10
    /// - CanvasRelayoutOnPropChangeTest11
    /// - CanvasGetSetLRTBException
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest1", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest1 : CodeTest
    {
        public CanvasBasicFuncTest1()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasSizeVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest2", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest2 : CodeTest
    {
        public CanvasBasicFuncTest2()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            canvas.HorizontalAlignment = HorizontalAlignment.Center;
            canvas.VerticalAlignment = VerticalAlignment.Top;
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasSizeVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest3", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest3 : CodeTest
    {
        public CanvasBasicFuncTest3()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            canvas.Width = 25;
            canvas.Height = 25;
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasSizeVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest4", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest4 : CodeTest
    {
        public CanvasBasicFuncTest4()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest5", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest5 : CodeTest
    {
        public CanvasBasicFuncTest5()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, 50);
            Canvas.SetTop(rect, 50);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest6", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest6 : CodeTest
    {
        public CanvasBasicFuncTest6()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, -20);
            Canvas.SetTop(rect, -10);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest7", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest7 : CodeTest
    {
        public CanvasBasicFuncTest7()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetRight(rect, 50);
            Canvas.SetBottom(rect, 100);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest8", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest8 : CodeTest
    {
        public CanvasBasicFuncTest8()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, 50);
            Canvas.SetTop(rect, 50);
            Canvas.SetRight(rect, 100);
            Canvas.SetBottom(rect, 100);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest9", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest9 : CodeTest
    {
        public CanvasBasicFuncTest9()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, 100);
            Canvas.SetTop(rect, 50);
            rect.Margin = new Thickness(0, 30, 0, 0);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest10", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest10 : CodeTest
    {
        public CanvasBasicFuncTest10()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, 100);
            Canvas.SetTop(rect, 50);
            rect.Margin = new Thickness(10, 20, 30, 40);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest11", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest11 : CodeTest
    {
        public CanvasBasicFuncTest11()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetRight(rect, 100);
            Canvas.SetBottom(rect, 50);
            rect.Margin = new Thickness(10, 20, 30, 40);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest12", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest12 : CodeTest
    {
        public CanvasBasicFuncTest12()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, 70);
            Canvas.SetTop(rect, 30);
            Border child11 = new Border();
            child11.Background = new RadialGradientBrush(Colors.Purple, Colors.Orange);
            child11.Width = 50;
            child11.Height = 50;
            child11.CornerRadius = new CornerRadius(2, 30, 9, 0);
            Canvas.SetLeft(child11, 15);
            Canvas.SetTop(child11, 70);
            canvas.Children.Add(child11);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest13", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest13 : CodeTest
    {
        public CanvasBasicFuncTest13()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            eRoot.ClipToBounds = true;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, 90);
            Canvas.SetTop(rect, 30);
            Border child12 = new Border();
            child12.Background = new RadialGradientBrush(Colors.Purple, Colors.Orange);
            child12.Width = 50;
            child12.Height = 50;
            child12.CornerRadius = new CornerRadius(2, 30, 9, 0);
            Canvas.SetLeft(child12, 110);
            Canvas.SetTop(child12, 110);
            canvas.Children.Add(child12);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "CanvasBasicFuncTest14", Variables = "Area=ElementLayout")]
    public class CanvasBasicFuncTest14 : CodeTest
    {
        public CanvasBasicFuncTest14()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            Canvas canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas), 0) as Canvas;
            Rectangle rect = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Orange));
            canvas.Children.Add(rect);
            Canvas.SetLeft(rect, 90);
            Canvas.SetTop(rect, 30);
            Border child13 = new Border();
            child13.Background = new RadialGradientBrush(Colors.Purple, Colors.Orange);
            child13.Width = 50;
            child13.Height = 50;
            child13.CornerRadius = new CornerRadius(2, 30, 9, 0);
            Canvas.SetLeft(child13, 110);
            Canvas.SetTop(child13, 110);
            canvas.Children.Add(child13);
            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = CanvasCommon.CanvasChildPositionVerifier(this.window, TestLog.Current);
        }
    }

    [Test(1, "Panels.Canvas", "Exception.SetBottom", TestParameters = "target=Null_SetBottom")]
    [Test(1, "Panels.Canvas", "Exception.SetRight", TestParameters = "target=Null_SetRight")]
    [Test(1, "Panels.Canvas", "Exception.SetTop", TestParameters = "target=Null_SetTop")]
    [Test(1, "Panels.Canvas", "Exception.SetLeft", TestParameters = "target=Null_SetLeft")]
    [Test(1, "Panels.Canvas", "Exception.GetBottom", TestParameters = "target=Null_GetBottom")]
    [Test(1, "Panels.Canvas", "Exception.GetRight", TestParameters = "target=Null_GetRight")]
    [Test(1, "Panels.Canvas", "Exception.GetTop", TestParameters = "target=Null_GetTop")]
    [Test(1, "Panels.Canvas", "Exception.GetLeft", TestParameters = "target=Null_GetLeft")]
    public class CanvasGetSetLRTBException : CodeTest
    {
        public CanvasGetSetLRTBException() { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();
        }

        private Canvas _canvas = null;

        public override FrameworkElement TestContent()
        {
            _canvas = new Canvas();
            _canvas.Background = Brushes.Yellow;
            return _canvas;
        }

        private Rectangle _rect = null;
        public override void TestActions()
        {
            string target = DriverState.DriverParameters["target"];
            switch (target)
            {
                case "Null_SetBottom":
                    try
                    {
                        Canvas.SetBottom(_rect, 400);
                        Helpers.Log("Exception should have been caught for null Canvas.SetBottom.");
                        _tempResult = false;
                    }
                    catch (Exception ex_Null_SetBottom)
                    {
                        Helpers.Log("Exception caught for null Canvas.SetBottom. \n" + ex_Null_SetBottom.Message);
                    }
                    break;
                case "Null_SetRight":
                    try
                    {
                        Canvas.SetRight(_rect, 400);
                        Helpers.Log("Exception should have been caught for null Canvas.SetRight.");
                        _tempResult = false;
                    }
                    catch (Exception ex_Null_SetRight)
                    {
                        Helpers.Log("Exception caught for null Canvas.SetRight. \n" + ex_Null_SetRight.Message);
                    }
                    break;
                case "Null_SetTop":
                    try
                    {
                        Canvas.SetTop(_rect, 400);
                        Helpers.Log("Exception should have been caught for null Canvas.SetTop.");
                        _tempResult = false;
                    }
                    catch (Exception ex_Null_SetTop)
                    {
                        Helpers.Log("Exception caught for null Canvas.SetTop. \n" + ex_Null_SetTop.Message);
                    }
                    break;
                case "Null_SetLeft":
                    try
                    {
                        Canvas.SetLeft(_rect, 400);
                        Helpers.Log("Exception should have been caught for null Canvas.SetLeft.");
                        _tempResult = false;
                    }
                    catch (Exception ex_Null_SetLeft)
                    {
                        Helpers.Log("Exception caught for null Canvas.SetLeft. \n" + ex_Null_SetLeft.Message);
                    }
                    break;
                case "Null_GetBottom":
                    try
                    {
                        Canvas.GetBottom(_rect);
                        Helpers.Log("Exception should have been caught for null Canvas.GetBottom.");
                        _tempResult = false;
                    }
                    catch (Exception ex_Null_GetBottom)
                    {
                        Helpers.Log("Exception caught for null Canvas.GetBottom. \n" + ex_Null_GetBottom.Message);
                    }
                    break;
                case "Null_GetRight":
                    try
                    {
                        Canvas.GetRight(_rect);
                        Helpers.Log("Exception should have been caught for null Canvas.GetRight.");
                        _tempResult = false;
                    }
                    catch (Exception ex_Null_GetRight)
                    {
                        Helpers.Log("Exception caught for null Canvas.GetRight. \n" + ex_Null_GetRight.Message);
                    }
                    break;
                case "Null_GetTop":
                    try
                    {
                        Canvas.GetTop(_rect);
                        Helpers.Log("Exception should have been caught for null Canvas.GetTop.");
                        _tempResult = false;
                    }
                    catch (Exception ex_Null_GetTop)
                    {
                        Helpers.Log("Exception caught for null Canvas.GetTop. \n" + ex_Null_GetTop.Message);
                    }
                    break;
                case "Null_GetLeft":
                    try
                    {
                        Canvas.GetLeft(_rect);
                        Helpers.Log("Exception should have been caught for null Canvas.GetLeft.");
                        _tempResult = false;
                    }
                    catch (Exception ex_Null_GetLeft)
                    {
                        Helpers.Log("Exception caught for null Canvas.GetLeft. \n" + ex_Null_GetLeft.Message);
                    }
                    break;
            }
        }

        bool _tempResult = true;
        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    #region Relayout On Property Change

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest1", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest1 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest1()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;

        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }
        public override void TestActions()
        {
            Canvas.SetLeft(rect, -100);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(-100, 0);
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest2", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest2 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest2()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }
        public override void TestActions()
        {
            Canvas.SetTop(rect, 20);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(0, 20);
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest3", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest3 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest3()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = true;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }

        public override void TestActions()
        {
            Canvas.SetRight(rect, double.NaN);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = false;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(0, 0);
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest4", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest4 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest4()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }
        public override void TestActions()
        {
            Canvas.SetBottom(rect, 0);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(0, (canvas.ActualHeight - rect.Height));
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest5", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest5 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest5()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }
        public override void TestActions()
        {
            Canvas.SetLeft(rect, double.NaN);
            Canvas.SetTop(rect, 50);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(0, Canvas.GetTop(rect));
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest6", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest6 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest6()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }
        public override void TestActions()
        {
            Canvas.SetLeft(rect, 10);
            Canvas.SetRight(rect, 30);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(Canvas.GetLeft(rect), 0);
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest7", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest7 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest7()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }
        public override void TestActions()
        {
            Canvas.SetLeft(rect, 100);
            Canvas.SetBottom(rect, 50);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(100, (canvas.ActualHeight - rect.Height - 50));
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest8", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest8 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest8()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }
        public override void TestActions()
        {
            Canvas.SetTop(rect, double.NaN);
            Canvas.SetRight(rect, -100);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point((canvas.ActualWidth - rect.Width - Canvas.GetRight(rect)), 0);
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest9", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest9 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest9()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = true;

        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }

        public override void TestActions()
        {
            Canvas.SetTop(rect, double.NaN);
            Canvas.SetBottom(rect, double.NaN);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = false;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(0, 0);
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest10", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest10 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest10()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }
        public override void TestActions()
        {
            Canvas.SetRight(rect, double.NaN);
            Canvas.SetBottom(rect, -50);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(0, canvas.ActualHeight - rect.Height - Canvas.GetBottom(rect));
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    [Test(2, "Panels.Canvas", "CanvasRelayoutOnPropChangeTest11", Variables = "Area=ElementLayout")]
    public class CanvasRelayoutOnPropChangeTest11 : CodeTest
    {
        public CanvasRelayoutOnPropChangeTest11()
        {
        }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public Canvas canvas;
        public Rectangle rect;
        public bool relayoutOccurred = false;

        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)CanvasCommon.CanvasContent();
            canvas = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Canvas)) as Canvas;
            rect = LayoutUtility.GetChildFromVisualTree(canvas, typeof(Rectangle)) as Rectangle;
            return eRoot;
        }

        public override void TestActions()
        {
            Canvas.SetLeft(rect, 100);
            Canvas.SetTop(rect, double.NaN);
            Canvas.SetRight(rect, double.NaN);
            Canvas.SetBottom(rect, 50);
            canvas.LayoutUpdated += new EventHandler(canvas_LayoutUpdated);
        }

        void canvas_LayoutUpdated(object sender, EventArgs e)
        {
            relayoutOccurred = true;
            Helpers.Log("LayoutUpdated event fired~");
        }

        public override void TestVerify()
        {
            Point expected = new Point(Canvas.GetLeft(rect), canvas.ActualHeight - rect.Height - Canvas.GetBottom(rect));
            this.Result = CanvasCommon.CanvasRelayoutOnPropChangeTestVerifier(this.window, TestLog.Current, relayoutOccurred, expected);
        }
    }

    #endregion

    [Test(2, "Panels.Canvas", "CanvasBoxModel",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class CanvasBoxModel : VisualScanTest
    {
        public CanvasBoxModel()
            : base("CanvasBoxModel.xaml")
        { }
    }

    [Test(2, "Panels.Canvas", "CanvasPositionPropsXAML",
        Disabled = true,
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class CanvasPositionPropsXAML : VisualScanTest
    {
        public CanvasPositionPropsXAML()
            : base("CanvasPositionProps.xaml")
        { }
    }

    [Test(2, "Panels.Canvas", "CanvasBoxModelRTL",
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class CanvasBoxModelRTL : VisualScanTest
    {
        public CanvasBoxModelRTL()
            : base("CanvasBoxModelRTL.xaml")
        { }
    }

    [Test(2, "Panels.Canvas", "CanvasPositionPropsRTLXAML",
        Disabled = true,
        Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class CanvasPositionPropsRTLXAML : VisualScanTest
    {
        public CanvasPositionPropsRTLXAML()
            : base("CanvasPositionPropsRTL.xaml")
        { }
    }
}
