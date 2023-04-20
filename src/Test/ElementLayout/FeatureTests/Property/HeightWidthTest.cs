// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Resources;
using System.Globalization;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Globalization;

namespace ElementLayout.FeatureTests.Property
{    
    [Test(2, "Property.HeightWidth", "HeightWidthTest Panel", TestParameters = "target=Panel")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest Canvas", TestParameters = "target=Canvas")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest StackPanel", TestParameters = "target=StackPanel")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest Grid", TestParameters = "target=Grid")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest DockPanel", TestParameters = "target=DockPanel")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest Decorator", TestParameters = "target=Decorator")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest Border", TestParameters = "target=Border")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest Viewbox", TestParameters = "target=Viewbox")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest ScrollViewer", TestParameters = "target=ScrollViewer")]
    [Test(2, "Property.HeightWidth", "HeightWidthTest WrapPanel", TestParameters = "target=WrapPanel")]    
    public class HeightWidthTest : CodeTest
    {       
        private FrameworkElement _child;
        private double _widthExpressionValue = 0;
        private double _heightExpressionValue = 0;
        private static double s_staticHeight = 175;
        private static double s_staticWidth = 250;
        private string _failingTests = "";
        bool _isLayoutRoundingTest = false;

        public HeightWidthTest()
        {
            this.Result = true;
        }

        public HeightWidthTest(bool isLayoutRoundingTest)
        {
            this.Result = true;
            this._isLayoutRoundingTest = true;
        }

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
            root.Background = Brushes.LightGoldenrodYellow;
            root.Children.Add(AddTestContent(DriverState.DriverParameters["target"]));            
            return root;
        }

        public override void TestActions()
        {           
            Helpers.Log("Basic Width Height Tests.");
            Helpers.Log("-----------------------------");

            FrameworkElement parent = VisualTreeHelper.GetParent(_child) as FrameworkElement;
           
            /// case 1. default values.
            /// values for Width / Height should be nan.            

            if (!(Double.IsNaN(_child.Height)) && !(Double.IsNaN(_child.Width)))
            {
                _failingTests += "Default Value Test. ";                
                this.Result = false;
            }
                        
            /// case 2. SetValue.
            /// Values for Width / Height should be expected value            

            _child.Height = s_staticHeight;
            _child.Width = s_staticWidth;
            CommonFunctionality.FlushDispatcher();

            if (_child.ActualHeight != s_staticHeight && _child.ActualWidth != s_staticWidth)
            {
                _failingTests += "Set Value Test. ";
                this.Result = false;
            }
                        
            /// case 3. Incorrect Value.
            /// if value is not a positive double values should throw ArgumentException.            

            try
            {
                _child.Width = -1;
            }
            catch (ArgumentException ex)
            {                
                if(!Exceptions.CompareMessage(ex.Message, "InvalidPropertyValue", WpfBinaries.WindowsBase))
                {
                    _failingTests += "Incorrect Value Test : Negative Width. ";
                    this.Result = false;
                }              
            }

            CommonFunctionality.FlushDispatcher();

            try
            {
                _child.Height = -1;
            }
            catch (ArgumentException ex)
            {                
                if (!Exceptions.CompareMessage(ex.Message, "InvalidPropertyValue", WpfBinaries.WindowsBase))
                {
                    _failingTests += "Incorrect Value Test : Negative Height. ";
                    this.Result = false;
                }               
            }
            
            /// case 4. Expression Values.
            /// Values for Width / Height will be set as an Expression, end values should be expected value.           

            _widthExpressionValue = (((s_staticWidth * 5) / 10) - 2);
            _child.Width = (((s_staticWidth * 5) / 10) - 2);

            CommonFunctionality.FlushDispatcher();

            if (_child.ActualWidth != _widthExpressionValue)
            {
                _failingTests += "Expression Value : Width. ";
                this.Result = false;
            }           

            _heightExpressionValue = (((s_staticHeight / 10) + 6) * 9);
            _child.Height = (((s_staticHeight / 10) + 6) * 9);

            CommonFunctionality.FlushDispatcher();

            // If the test is a LayoutRounding test, skip this validation b/c the expected rounding of the value will fail this validation.
            if (!_isLayoutRoundingTest)
            {
                if (_child.ActualHeight != _heightExpressionValue)
                {
                    _failingTests += "Expression Value : Height. ";
                    this.Result = false;
                }
            }
            
            /// case 5. Constrained Scenario.
            /// Values for Width / Height will be set but contrained by parent.
            /// child.ActualSize > parent.ActualSize
            /// child.RenderSize > parent.RenderSize
            /// child.DesiredSize == parent.DesiredSize           

            parent.Width = s_staticWidth / 3;
            parent.Height = s_staticHeight / 3;
            _child.Width = s_staticWidth;
            _child.Height = s_staticHeight;

            CommonFunctionality.FlushDispatcher();

            if (!(_child.ActualWidth > parent.ActualWidth) &&
                !(_child.ActualHeight > parent.ActualHeight) &&
                !(_child.RenderSize.Width > parent.RenderSize.Width) &&
                !(_child.RenderSize.Height > parent.RenderSize.Height) &&
                _child.DesiredSize.Width != parent.DesiredSize.Width &&
                _child.DesiredSize.Height != parent.DesiredSize.Height)
            {
                _failingTests += "Constrained : Width / Height. ";
                this.Result = false;
            }           
            
            /// case 6. Min / Max default values.
            /// Default values should be
            /// Min = 0
            /// Max = Infinity           

            parent.Width = double.NaN;
            parent.Height = double.NaN;
            _child.Width = double.NaN;
            _child.Height = double.NaN;

            CommonFunctionality.FlushDispatcher();

            if (_child.MinHeight != 0 && _child.MinWidth != 0)
            {
                _failingTests += "Defaults : MinWidth / MinHeight. ";
                this.Result = false;
            }           

            if (_child.MaxHeight != double.PositiveInfinity && _child.MaxWidth != double.PositiveInfinity)
            {
                _failingTests += "Defaults : MaxWidth / MaxHeight. ";
                this.Result = false;
            }            
            
            /// case 7. Min / Max incorrect values.
            /// Default values should be
            /// Min = 0
            /// Max = Infinity          

            try
            {
                _child.MaxWidth = -1;
            }
            catch (ArgumentException ex)
            {                
                if (!Exceptions.CompareMessage(ex.Message, "InvalidPropertyValue", WpfBinaries.WindowsBase))
                {
                    _failingTests += "Incorrect Value Test : Negative MaxWidth. ";
                    this.Result = false;
                }               
            }

            CommonFunctionality.FlushDispatcher();

            try
            {
                _child.MaxHeight = -1;
            }
            catch (ArgumentException ex)
            {              
                if (!Exceptions.CompareMessage(ex.Message, "InvalidPropertyValue", WpfBinaries.WindowsBase))
                {
                    _failingTests += "Incorrect Value Test : Negative MaxHeight. ";
                    this.Result = false;
                }
            }

            try
            {
                _child.MinWidth = -1;
            }
            catch (ArgumentException ex)
            {               
                if (!Exceptions.CompareMessage(ex.Message, "InvalidPropertyValue", WpfBinaries.WindowsBase))
                {
                    _failingTests += "Incorrect Value Test : Negative MinWidth. ";
                    this.Result = false;
                }              
            }

            CommonFunctionality.FlushDispatcher();

            try
            {
                _child.MinHeight = -1;
            }
            catch (ArgumentException ex)
            {               
                if (!Exceptions.CompareMessage(ex.Message, "InvalidPropertyValue", WpfBinaries.WindowsBase))
                {
                    _failingTests += "Incorrect Value Test : Negative MinHeight. ";
                    this.Result = false;
                }               
            }
        }
        
        private FrameworkElement AddTestContent(string testElement)
        {
            switch (testElement)
            {
                case "Panel":
                    TestPanel panel = new TestPanel();
                    panel.Background = Brushes.LightSteelBlue;
                    _child = panel;
                    break;

                case "Canvas":
                    Canvas canvas = new Canvas();
                    canvas.Background = Brushes.LightSteelBlue;
                    _child = canvas;
                    break;

                case "StackPanel":
                    StackPanel stack = new StackPanel();
                    stack.Background = Brushes.LightSalmon;
                    _child = stack;
                    break;

                case "Grid":
                    Grid grid = new Grid();
                    grid.Background = Brushes.Orange;
                    _child = grid;
                    break;

                case "DockPanel":
                    DockPanel dock = new DockPanel();
                    dock.Background = Brushes.Blue;
                    _child = dock;
                    break;

                case "Decorator":
                    Decorator decorator = new Decorator();
                    _child = decorator;
                    break;

                case "Border":
                    Border border = new Border();
                    border.Background = Brushes.Blue;
                    _child = border;
                    break;

                case "Viewbox":
                    Viewbox viewbox = new Viewbox();
                    _child = viewbox;
                    break;

                case "ScrollViewer":
                    ScrollViewer scrollviewer = new ScrollViewer();
                    _child = scrollviewer;
                    break;

                case "WrapPanel":
                    WrapPanel wrap = new WrapPanel();
                    wrap.Background = Brushes.CornflowerBlue;

                    Border wrapchild = new Border();
                    wrapchild.Height = 1000;
                    wrapchild.Width = 1000;
                    wrapchild.Background = Brushes.DarkOrange;
                    
                    wrap.Children.Add(wrapchild);
                    _child = wrap;
                    break;
            }
            return _child;
        }
      
        public override void TestVerify()
        {           
            if (!this.Result)
            {
                Helpers.Log("Failing tests were: " + _failingTests);
            }            
        }
    }    
}
