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
    /// This contains code for Framework Element Property Cases.
    //////////////////////////////////////////////////////////////////

    [Test(2, "Property.LayoutProperties", "PropertyTestBorder", Variables = "Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestBorder : CodeTest
    {
        public PropertyTestBorder()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    [Test(2, "Property.LayoutProperties", "PropertyTestPanel", Variables = "Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestPanel : CodeTest
    {
        public PropertyTestPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    [Test(2, "Property.LayoutProperties", "PropertyTestCanvas", Variables = "Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestCanvas : CodeTest
    {
        public PropertyTestCanvas()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }
    
        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    [Test(2, "Property.LayoutProperties", "PropertyTestStackPanel", Variables = "Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestStackPanel : CodeTest
    {
        public PropertyTestStackPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }
       
        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    [Test(2, "Property.LayoutProperties", "PropertyTestGrid", Variables = "Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestGrid : CodeTest
    {
        public PropertyTestGrid()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }
        
        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    [Test(2, "Property.LayoutProperties", "PropertyTestDockPanel", Variables = "Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestDockPanel : CodeTest
    {
        public PropertyTestDockPanel()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    [Test(2, "Property.LayoutProperties", "PropertyTestDecorator", Variables = "Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestDecorator : CodeTest
    {
        public PropertyTestDecorator()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    [Test(2, "Property.LayoutProperties", "PropertyTestViewbox", Variables = "Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestViewbox : CodeTest
    {
        public PropertyTestViewbox()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }

    [Test(2, "Property.LayoutProperties", "PropertyTestScrollViewer", Variables="Area=ElementLayout", Keywords = "Localization_Suite")]
    public class PropertyTestScrollViewer : CodeTest
    {
        public PropertyTestScrollViewer()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 700;
            this.window.Width = 700;
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
            _root.Background = Brushes.Yellow;

            _child = PropertyTestHelpers.AddTestContent(s_testElement);

            _root.Children.Add(_child);

            return _root;
        }

        double[] _midValues = { 0, 128, 256, 512, 1024, 2048, 4096, 4592, 9148, 18368 };
        string _failingTest = "";

        bool _tempResult = true;

        public override void TestActions()
        {
            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Negative", double.NegativeInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Negative", -1);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "Infinity", double.PositiveInfinity);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "LargeValue", double.MaxValue);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "SmallValue", double.Epsilon);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
            CommonFunctionality.FlushDispatcher();

            foreach (double value in _midValues)
            {
                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MarginProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MarginProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.HeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.HeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.WidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.WidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MinWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MinWidthProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxHeightProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxHeightProperty);
                CommonFunctionality.FlushDispatcher();

                PropertyTestHelpers.doublePropertyTest(_child, _tempResult, _failingTest, FrameworkElement.MaxWidthProperty, "MidValue", value);
                CommonFunctionality.FlushDispatcher();
                PropertyTestHelpers.restoreValue(_child, FrameworkElement.MaxWidthProperty);
                CommonFunctionality.FlushDispatcher();
            }

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "Valid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.FlowDirectionProperty, "InValid", Dock.Top);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.FlowDirectionProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "Valid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VisibilityProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VisibilityProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "Valid", HorizontalAlignment.Right);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.HorizontalAlignmentProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.HorizontalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "Valid", VerticalAlignment.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.VerticalAlignmentProperty, "InValid", HorizontalAlignment.Center);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.VerticalAlignmentProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "InValid", Dock.Bottom);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipToBoundsProperty, "Valid", true);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipToBoundsProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "InValid", false);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.ClipProperty, "Valid", PropertyTestHelpers.TestClipGeometry());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.ClipProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "InValid", FlowDirection.RightToLeft);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.LayoutTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.LayoutTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "InValid", Visibility.Hidden);
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

            PropertyTestHelpers.propertyTest(_child, _failingTest, _tempResult, FrameworkElement.RenderTransformProperty, "Valid", PropertyTestHelpers.TestTransform());
            CommonFunctionality.FlushDispatcher();
            PropertyTestHelpers.restoreValue(_child, FrameworkElement.RenderTransformProperty);
            CommonFunctionality.FlushDispatcher();

        }

        public override void TestVerify()
        {
            this.Result = _tempResult;
        }
    }
}
