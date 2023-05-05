// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.Layout.PropertyDump;

namespace ElementLayout.FeatureTests.Panels
{
	[Test(0, "Panels.Border", "BorderPropertyTest", Variables="Area=ElementLayout")]
    public class BorderPropertyTest : CodeTest
    {
        public BorderPropertyTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 650;
            this.window.Width = 650;
            this.window.Top = 50;
            this.window.Left = 50;
            this.window.Content = this.TestContent();
        }

        Grid _root;
        Border _border;

        double _paddingValue = 25d;
        double _cornerRadiusValue = 50d;
        double _borderThicknessValue = 25d;
        static float s_aValue = 1f;
        static float s_rValue = .4f;
        static float s_gValue = .6f;
        static float s_bValue = .8f;
        SolidColorBrush _borderBrushColor = new SolidColorBrush(Color.FromScRgb(s_aValue, s_rValue, s_gValue, s_bValue));


        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _root.VerticalAlignment = VerticalAlignment.Center;

            _border = new Border();
            _border.Height = 200;
            _border.Width = 200;
            _border.Background = Brushes.CornflowerBlue;

            _root.Children.Add(_border);

            return _root;
        }

        public override void TestActions()
        {
            //borderthickness test
            Helpers.Log("***BorderThickness Test***");
            _border.SetValue(Border.BorderThicknessProperty, new Thickness(_borderThicknessValue));
            CommonFunctionality.FlushDispatcher();
            Helpers.Log("BorderThickness Values : " + _border.GetValue(Border.BorderThicknessProperty));

            if (_border.BorderThickness.Left != _borderThicknessValue || _border.BorderThickness.Top != _borderThicknessValue || _border.BorderThickness.Right != _borderThicknessValue || _border.BorderThickness.Bottom != _borderThicknessValue)
            {
                Helpers.Log("border borderthickness values are not correct.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("border borderthickness values are correct.");
            }

            //borderbrush test
            Helpers.Log("***BorderBrush Test***");
            _border.SetValue(Border.BorderBrushProperty, _borderBrushColor);
            CommonFunctionality.FlushDispatcher();
            Helpers.Log("BorderBrush Values : " + _border.GetValue(Border.BorderBrushProperty));
            SolidColorBrush brushColor = _border.GetValue(Border.BorderBrushProperty) as SolidColorBrush;

            if (brushColor.Color.ScA != s_aValue || brushColor.Color.ScR != s_rValue || brushColor.Color.ScG != s_gValue || brushColor.Color.ScB != s_bValue)
            {
                Helpers.Log("border borderbrush values are not correct.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("border borderbrush values are correct.");
            }

            //padding test
            Helpers.Log("***Padding Test***");
            _border.SetValue(Border.PaddingProperty, new Thickness(_paddingValue));
            CommonFunctionality.FlushDispatcher();
            Helpers.Log("Border Padding Values : " + _border.GetValue(Border.PaddingProperty));

            if (_border.Padding.Bottom != _paddingValue || _border.Padding.Top != _paddingValue || _border.Padding.Left != _paddingValue || _border.Padding.Right != _paddingValue)
            {
                Helpers.Log("border padding values are not correct.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("border padding values are correct.");
            }

            //corner radius test
            Helpers.Log("***Corner Radius Test***");
            _border.SetValue(Border.CornerRadiusProperty, new CornerRadius(_cornerRadiusValue));
            CommonFunctionality.FlushDispatcher();

            Helpers.Log("Border Corner Radius Values : " + _border.GetValue(Border.CornerRadiusProperty));
            if (_border.CornerRadius.BottomLeft != _cornerRadiusValue || _border.CornerRadius.BottomRight != _cornerRadiusValue || _border.CornerRadius.TopLeft != _cornerRadiusValue || _border.CornerRadius.TopRight != _cornerRadiusValue)
            {
                Helpers.Log("border corner radius values are not correct.");
                _tempresult = false;
            }
            else
            {
                Helpers.Log("border corner radius values are correct.");
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

    [Test(0, "Panels.Border", "BorderMaxHeightWidth", Variables="Area=ElementLayout")]
    public class BorderMaxHeightWidth : CodeTest
    {
        public BorderMaxHeightWidth()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Grid _root;
        Border _bordercontent;

        double _maxValue = 350;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _border = new Border();
            _border.Height = 100;
            _border.Width = 100;

            _bordercontent = new Border();
            _bordercontent.Background = Brushes.YellowGreen;
            _bordercontent.Height = 100;
            _bordercontent.Width = 100;

            _border.Child = _bordercontent;

            _root.Children.Add(_border);

            return _root;
        }

        public override void TestActions()
        {
            _border.Height = 1000;
            _border.Width = 1000;
            _border.MaxHeight = _maxValue;
            _border.MaxWidth = _maxValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size BorderSize = _border.RenderSize;

            if (BorderSize.Height != _maxValue || BorderSize.Width != _maxValue)
            {
                Helpers.Log("Border Max Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Border Max Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Border", "BorderMinHeightWidth", Variables="Area=ElementLayout")]
    public class BorderMinHeightWidth : CodeTest
    {
        public BorderMinHeightWidth()
        {
        }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Content = this.TestContent();
        }

        Border _border;
        Grid _root;
        Border _bordercontent;

        double _minValue = 200;

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            _root.Background = Brushes.LightGray;
            _root.Height = 650;
            _root.Width = 600;

            _border = new Border();
            _border.Height = 100;
            _border.Width = 100;

            _bordercontent = new Border();
            _bordercontent.Background = Brushes.YellowGreen;
            _bordercontent.Height = 100;
            _bordercontent.Width = 100;

            _border.Child = _bordercontent;

            _root.Children.Add(_border);

            return _root;
        }

        public override void TestActions()
        {
            _border.Height = 10;
            _border.Width = 10;
            _border.MinHeight = _minValue;
            _border.MinWidth = _minValue;
        }

        public override void TestVerify()
        {
            CommonFunctionality.FlushDispatcher();

            Size BorderSize = _border.RenderSize;

            if (BorderSize.Height != _minValue || BorderSize.Width != _minValue)
            {
                Helpers.Log("Border Min Height/Width Test Failed.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Border Min Height/Width Test Passed.");
                this.Result = true;
            }
        }
    }

    [Test(0, "Panels.Border", "BorderComplex", Variables = @"Area=ElementLayout/VscanMasterPath=FeatureTests\ElementLayout\MASTERS\VSCAN")]
    public class BorderComplex : VisualScanTest
    {
        public BorderComplex()
            : base("BorderComplex.xaml")
        { }
    }
}
