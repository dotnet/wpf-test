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

namespace ElementLayout.FeatureTests.Scenario
{
    //////////////////////////////////////////////////////////////////
    /// This code is for Sparkle scenario Test cases.
    /// 
    //////////////////////////////////////////////////////////////////

    [Test(2, "Scenario.Sparkle", "AutoStarSpan", Variables="Area=ElementLayout")]
    public class AutoStarSpan : CodeTest
    {
        public AutoStarSpan()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            try
            {
                this.window.Content = this.TestContent();
            }
            catch (Exception e)
            {
                Helpers.Log(e.Message);
                _exceptionCaught = true;
            }
        }

        bool _exceptionCaught = false;
        Grid _grid;
        public override FrameworkElement TestContent()
        {
            _grid = new Grid();

            ColumnDefinition cd1 = new ColumnDefinition();
            cd1.Width = new GridLength(0, GridUnitType.Auto);
            _grid.ColumnDefinitions.Add(cd1);

            RowDefinition rd1 = new RowDefinition();
            rd1.Height = new GridLength(0, GridUnitType.Auto);

            RowDefinition rd2 = new RowDefinition();

            rd2.Height = new GridLength(1, GridUnitType.Star);

            _grid.RowDefinitions.Add(rd1);
            _grid.RowDefinitions.Add(rd2);

            Button button = new Button();

            button.Content = "BUTTON!";
            button.SetValue(Grid.RowProperty, 0);
            button.SetValue(Grid.ColumnProperty, 0);
            button.SetValue(Grid.RowSpanProperty, 2);
            button.SetValue(Grid.ColumnSpanProperty, 1);

            _grid.Children.Add(button);
            _grid.ShowGridLines = true;
            return _grid;
        }

        public override void TestVerify()
        {
            if (_exceptionCaught)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
                Helpers.Log("Grid did not assert when an element spanned a star and auto row.");
            }
        }

    }

    [Test(2, "Scenario.Sparkle", "ComputedSizeTest", Variables="Area=ElementLayout")]
    public class ComputedSizeTest : CodeTest
    {
        public ComputedSizeTest()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        Button _button;
        RowDefinition _rd;
        ColumnDefinition _cd;

        public override FrameworkElement TestContent()
        {
            _grid = new Grid();

            _rd = new RowDefinition();
            _rd.Height = new GridLength(0, GridUnitType.Auto);
            _grid.RowDefinitions.Add(_rd);

            _cd = new ColumnDefinition();
            _cd.Width = new GridLength(0, GridUnitType.Auto);
            _grid.ColumnDefinitions.Add(_cd);

            _button = new Button();
            _button.SetValue(Grid.RowProperty, 0);
            _button.SetValue(Grid.ColumnProperty, 0);
            _button.SetValue(Grid.RowSpanProperty, 1);
            _button.SetValue(Grid.ColumnSpanProperty, 1);
            _button.Content = "Grid!";

            _grid.Children.Add(_button);

            return _grid;
        }

        public override void TestVerify()
        {
            GridLength gridWidth = (GridLength)_cd.GetValue(ColumnDefinition.WidthProperty);
            GridLength gridHeight = (GridLength)_rd.GetValue(RowDefinition.HeightProperty);

            if (gridWidth.Value == 0)
            {
                this.Result = false;
                Helpers.Log("Computed size of grid column is zero!");
            }
            else if (gridHeight.Value == 0)
            {
                this.Result = false;
                Helpers.Log("Computed size of grid row is zero!");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Grid's computed sizes are correct");
            }
        }

    }

    [Test(2, "Scenario.Sparkle", "GridPropertyChange", Variables="Area=ElementLayout")]
    public class GridPropertyChange : CodeTest
    {
        public GridPropertyChange()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        ColumnDefinition _cd1;
        Button _button;

        public override FrameworkElement TestContent()
        {
            _grid = new Grid();

            _cd1 = new ColumnDefinition();

            _cd1.Width = new GridLength(200, GridUnitType.Pixel);

            ColumnDefinition cd2 = new ColumnDefinition();

            cd2.Width = new GridLength(1, GridUnitType.Star);

            _grid.ColumnDefinitions.Add(_cd1);
            _grid.ColumnDefinitions.Add(cd2);

            RowDefinition rd1 = new RowDefinition();
            rd1.Height = new GridLength(1, GridUnitType.Star);

            _grid.RowDefinitions.Add(rd1);

            _button = new Button();
            _button.Content = "Button!";
            _button.SetValue(Grid.RowProperty, 0);
            _button.SetValue(Grid.ColumnProperty, 0);
            _button.SetValue(Grid.RowSpanProperty, 1);
            _button.SetValue(Grid.ColumnSpanProperty, 1);
            _grid.Children.Add(_button);

            return _grid;
        }

        public override void TestVerify()
        {
            Rect rect = GetElementBounds(_button, _grid);
            _cd1.Width = new GridLength(100, GridUnitType.Pixel);

            Rect rect2 = GetElementBounds(_button, _grid);

            if (rect.Width == rect2.Width)
            {
                this.Result = false;
                Helpers.Log("Grid failed to update when the width of a column definition changed - regression bug");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Grid updated correctly when the width of a column definition changed - regression bug");
            }
        }

        public static Rect GetElementBounds(FrameworkElement element, FrameworkElement context)
        {
            // 
            element.UpdateLayout();

            // Transform bounding box from element coordinates to context coordinates.
            Rect boundingBox = new Rect(0, 0, element.RenderSize.Width, element.RenderSize.Height);
            Matrix matrix = GetElementToContextTransform(element, context);

            boundingBox.Transform(matrix);
            return boundingBox;
        }

        public static Matrix GetElementToContextTransform(FrameworkElement element, FrameworkElement context)
        {
            System.Diagnostics.Debug.Assert(element.IsDescendantOf(context), "Element is not a descendant of context.");

            Matrix matrix;
            System.Windows.Media.GeneralTransform gt = element.TransformToAncestor(context);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if (t == null)
            {
                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }
            matrix = t.Value;
            Transform transform = null; // = (Transform)element.GetValue(Path2DElement.TransformEffectProperty);

            if (transform != null)
            {
                matrix = transform.Value * matrix;
            }

            return matrix;
        }
    }

    [Test(2, "Scenario.Sparkle", "ZeroStarSize", Variables="Area=ElementLayout")]
    public class ZeroStarSize : CodeTest
    {
        public ZeroStarSize()
        { }

        public override void WindowSetup()
        {
            this.window.Height = 600;
            this.window.Width = 800;
            this.window.Content = this.TestContent();
        }

        Grid _grid;
        Button _button;

        public override FrameworkElement TestContent()
        {
            try
            {
                _grid = new Grid();
                this.window.Content = _grid;

                ColumnDefinition cd1 = new ColumnDefinition();

                cd1.Width = new GridLength(1, GridUnitType.Star);
                _grid.ColumnDefinitions.Add(cd1);

                RowDefinition rd1 = new RowDefinition();

                rd1.Height = new GridLength(0, GridUnitType.Star);
                _grid.RowDefinitions.Add(rd1);
                _button = new Button();

                TextBlock text = new TextBlock();

                text.Text = "This is a button";
                text.FontSize = 40;
                _button.Content = text;
                _button.SetValue(Grid.RowProperty, 0);
                _button.SetValue(Grid.ColumnProperty, 0);
                _button.SetValue(Grid.RowSpanProperty, 1);
                _button.SetValue(Grid.ColumnSpanProperty, 1);

                _grid.Children.Add(_button);

                return _grid;
                
            }
            catch (Exception)
            {
                Helpers.Log("GridTest2 - Grid crashed with row def having length = 0*");
                _exceptionThrown = true;
                return null;
            }
        }

        bool _exceptionThrown = false;
        public override void TestVerify()
        {
            if (_exceptionThrown)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
                Helpers.Log("GridTest2 - Grid did not crash with row def having length = 0*");
            }
        }
    }
}
