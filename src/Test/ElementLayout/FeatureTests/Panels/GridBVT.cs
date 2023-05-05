// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.IO;
using System.Windows.Markup;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.TestTypes;

namespace ElementLayout.FeatureTests.Panels
{
    //////////////////////////////////////////////////////////////////
    /// This will load and run all Grid code BVT's.
    /// 
    /// Possible tests:
    /// - GridColumnWidth
    /// - GridMinMaxSize1
    /// - GridMinMaxSize2
    /// - GridMinMaxSize3
    /// - GridMinMaxSize4
    /// - GridOffsetRightBottom
    /// - GridSpanning
    /// - GridNumberOfColRow
    /// - GridRowHeight
    /// 
    //////////////////////////////////////////////////////////////////
    [Test(0, "Panels.Grid", "GridColumnWidth", Variables="Area=ElementLayout")]
    public class GridColumnWidth : CodeTest
    {
        public GridColumnWidth()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] width = { new GridLength(0, GridUnitType.Auto), new GridLength(150), 
                new GridLength(150), new GridLength(1.5, GridUnitType.Star), new GridLength(3, GridUnitType.Star) };

            GridLength[] height = { };

            eRoot = GridCommon.CreateGrid(5, 1, width, height);

            double[] rectSize = { 100, double.NaN, double.NaN, double.NaN, double.NaN };
            SolidColorBrush[] rectBackground = { new SolidColorBrush(Colors.Red), 
                new SolidColorBrush(Colors.Orange), new SolidColorBrush(Colors.Yellow), 
                new SolidColorBrush(Colors.Green), new SolidColorBrush(Colors.Blue)};
            GridCommon.AddingRectangle(eRoot, rectSize, rectBackground, true, false);
            return eRoot;
        }
        public override void TestVerify()
        {
            Helpers.Log("Total Width= " + eRoot.RenderSize.Width.ToString());
            this.Result = true;

            double[] expt = new double[5];
            expt[0] = 100;
            expt[1] = 150;
            expt[2] = 150;
            expt[3] = (eRoot.RenderSize.Width - (expt[0] + expt[1] + expt[2])) / 3;
            expt[4] = expt[3] * 2;

            for (int n = 0; n < 5; n++)
            {
                if (Math.Abs(eRoot.ColumnDefinitions[n].ActualWidth - expt[n]) < 0.01)
                {
                    Helpers.Log("Width for Column " + n + ": " + eRoot.ColumnDefinitions[n].ActualWidth.ToString() + ": Passed.");
                }
                else
                {
                    Helpers.Log("Width for Column " + n + ": Failed~!");
                    Helpers.Log("Actual Value = " + eRoot.ColumnDefinitions[n].ActualWidth.ToString());
                    Helpers.Log("Expected Value = " + expt[n].ToString());
                    this.Result = false;
                    break;
                }
            }
        }

    }

    /// <summary>
    /// Definition size cannot be smaller than Min Size or larger than Max size, 
    /// but if Content size is larger than that, Grid will size to content.
    /// </summary>

    /// <summary>
    /// Definition Size: No Set
    /// Min Size: Set to 300px
    /// Max Size: No Set
    /// Content Size: 150px
    /// Result: Size to Content and larger than min -> Def Size = Min Size
    /// </summary>
    [Test(0, "Panels.Grid", "GridMinMaxSize1", Variables="Area=ElementLayout")]
    public class GridMinMaxSize1 : CodeTest
    {
        public GridMinMaxSize1()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 628;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(3, 3);
            eRoot.ShowGridLines = true;
            double[] minWidth = new double[] { 300, 300, 300 };
            GridCommon.SettingColumnMinWidth(eRoot, minWidth);

            Rectangle rect = CommonFunctionality.CreateRectangle(150, 150, new SolidColorBrush(Colors.Blue));
            GridCommon.PlacingChild(eRoot, rect, 1, 1, true);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridMinMaxSizeVerification.GridMinMaxSizeVerifier(eRoot, TestLog.Current);
        }

    }
    
    /// <summary>
    /// Definition Size: Auto
    /// Min Size: Set to 150px
    /// Max Size: No Set
    /// Content Size: Auto
    /// Result: Size to Content and larger than Min -> Def Size = Min Size
    /// </summary>
    [Test(0, "Panels.Grid", "GridMinMaxSize2", Variables="Area=ElementLayout")]
    public class GridMinMaxSize2 : CodeTest
    {
        public GridMinMaxSize2()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 628;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(3, 3);
            eRoot.ShowGridLines = true;
            GridLength[] gl = new GridLength[] { new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Auto) };
            GridCommon.SettingColumnWidth(eRoot, gl);

            double[] minWidth = new double[] { 150, 150, 150 };

            GridCommon.SettingColumnMinWidth(eRoot, minWidth);
            Rectangle rect = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, new SolidColorBrush(Colors.Blue));
            GridCommon.PlacingChild(eRoot, rect, 1, 1, true);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridMinMaxSizeVerification.GridMinMaxSizeVerifier(eRoot, TestLog.Current);
        }

    }

    /// <summary>
    /// Definition Size: No Set
    /// Min Size: No Set
    /// Max Size: Set to 150px
    /// Content Size: No Set
    /// Result: Less than Max -> Def Size = Max Size (150px)
    /// </summary>
    [Test(0, "Panels.Grid", "GridMinMaxSize3", Variables="Area=ElementLayout")]
    public class GridMinMaxSize3 : CodeTest
    {
        public GridMinMaxSize3()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 628;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(3, 3);
            eRoot.ShowGridLines = true;
            double[] maxWidth = new double[] { 150, 150, 150 };

            GridCommon.SettingColumnMaxWidth(eRoot, maxWidth);
            Rectangle rect = CommonFunctionality.CreateRectangle(15, 15, new SolidColorBrush(Colors.Blue));

            GridCommon.PlacingChild(eRoot, rect, 1, 1, true);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridMinMaxSizeVerification.GridMinMaxSizeVerifier(eRoot, TestLog.Current);
        }

    }
    
    /// <summary>
    /// Definition Size: Set to 200px
    /// Min Size: No Set
    /// Max Size: Set to 150px
    /// Content Size: 50% (300px)
    /// Result: Less than Max -> Max Size ignored, and Definition sizes to Content
    /// </summary>
    [Test(0, "Panels.Grid", "GridMinMaxSize4", Variables="Area=ElementLayout")]
    public class GridMinMaxSize4 : CodeTest
    {
        public GridMinMaxSize4()
         { }

        public override void WindowSetup()
        {
            this.window.Width = 608;
            this.window.Height = 628;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(3, 3);
            eRoot.ShowGridLines = true;
            GridLength[] gl = new GridLength[] { new GridLength(200), new GridLength(200), new GridLength(200) };
            GridCommon.SettingColumnWidth(eRoot, gl);

            double[] maxWidth = new double[] { 150, 150, 150 };
            GridCommon.SettingColumnMaxWidth(eRoot, maxWidth);

            Rectangle rect = CommonFunctionality.CreateRectangle(300, 300, new SolidColorBrush(Colors.Blue));
            GridCommon.PlacingChild(eRoot, rect, 1, 1, true);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridMinMaxSizeVerification.GridMinMaxSizeVerifier(eRoot, TestLog.Current);
        }

    }

    [Test(0, "Panels.Grid", "GridNumberOfColRow", Variables="Area=ElementLayout")]
    public class GridNumberOfColRow : CodeTest
    {

        public GridNumberOfColRow()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(6, 4);
            return eRoot;
        }
        public override void TestVerify()
        {
            Helpers.Log("Column = " + eRoot.ColumnDefinitions.Count.ToString());
            Helpers.Log("Row = " + eRoot.RowDefinitions.Count.ToString());
            this.Result = ((eRoot.ColumnDefinitions.Count == 6) && (eRoot.RowDefinitions.Count == 4)) ? true : false;
            if (!this.Result) Helpers.Log("Number of Columns should be 6 and Number of Rows should be 4.");
        }

    }

    [Test(0, "Panels.Grid", "GridOffsetRightBottom", Variables="Area=ElementLayout")]
    public class GridOffsetRightBottom : CodeTest
    {
        public GridOffsetRightBottom()
        { }

        public override void WindowSetup()
        {
            FileStream f = new FileStream("GridOffsetRightBottom.xaml", FileMode.Open, FileAccess.Read);
            this.window.Content = (FrameworkElement)XamlReader.Load(f);
            f.Close();
        }

        public override void TestVerify()
        {
            double expectedValueX, expectedValueY;
            double actualValueX, actualValueY;
            Grid root = (Grid)this.window.Content;
            System.Windows.Media.GeneralTransform gt = root.Children[0].TransformToAncestor(root);
            System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
            if (t == null)
            {
                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case - introduced by Transforms Breaking Change");
            }
            Matrix pt = t.Value;

            actualValueX = pt.OffsetX;
            actualValueY = pt.OffsetY;
            expectedValueX = 60;
            expectedValueY = 45;

            this.Result = ((expectedValueX == actualValueX) && (expectedValueY == actualValueY)) ? true : false;
            if (!this.Result) Helpers.Log("Expected and Actual values are NOT matched~\nExpected X Value: 60\nExpected Y Value: 45\nActual X Value: " + actualValueX.ToString() + "\nActual Y Value: " + actualValueY.ToString());
        }
    }

    [Test(0, "Panels.Grid", "GridRowHeight", Variables="Area=ElementLayout")]
    public class GridRowHeight : CodeTest
    {

        public GridRowHeight()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 450;
            this.window.Height = 450;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] width = { };
            GridLength[] height = { new GridLength(0, GridUnitType.Auto), new GridLength(80), new GridLength(150), new GridLength(1.5, GridUnitType.Star), new GridLength(3, GridUnitType.Star) };
            eRoot = GridCommon.CreateGrid(1, 5, width, height);
            double[] rectSize = { 100, double.NaN, double.NaN, double.NaN, double.NaN };
            SolidColorBrush[] rectBackground = { new SolidColorBrush(Colors.Red), new SolidColorBrush(Colors.Orange), new SolidColorBrush(Colors.Yellow), new SolidColorBrush(Colors.Green), new SolidColorBrush(Colors.Blue) };
            GridCommon.AddingRectangle(eRoot, rectSize, rectBackground, false, true);
            return eRoot;
        }
        public override void TestVerify()
        {
            Helpers.Log("Total Height= " + eRoot.RenderSize.Height.ToString());
            this.Result = true;
            double[] expt = new double[5]; //for expected value.
            expt[0] = 100;
            expt[1] = 80;
            expt[2] = 150;
            expt[3] = (eRoot.RenderSize.Height - (expt[0] + expt[1] + expt[2])) / 3;
            expt[4] = expt[3] * 2;

            for (int n = 0; n < 5; n++)
            {
                if (Math.Abs(eRoot.RowDefinitions[n].ActualHeight - expt[n]) < 0.01)
                    Helpers.Log("Height for Row" + n + ": " + eRoot.RowDefinitions[n].ActualHeight.ToString() + ": Passed.");
                else
                {
                    Helpers.Log("Height for row" + n + ": Failed~!");
                    Helpers.Log("Actual Value = " + eRoot.RowDefinitions[n].ActualHeight.ToString());
                    Helpers.Log("Expected Value = " + expt[n].ToString());
                    this.Result = false;
                }
            }
        }
    }

    [Test(0, "Panels.Grid", "GridSpanning", Variables="Area=ElementLayout")]
    public class GridSpanning : CodeTest
    {

        public GridSpanning()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 600;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(2, 2);
            eRoot.ShowGridLines = true;
            eRoot.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Star);
            eRoot.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
            Rectangle rect1_1 = CommonFunctionality.CreateRectangle(double.NaN, 50, new SolidColorBrush(Colors.Orange));
            Rectangle rect1_2 = CommonFunctionality.CreateRectangle(100, 50, new SolidColorBrush(Colors.Green));
            Rectangle rect1_3 = CommonFunctionality.CreateRectangle(150, 50, new SolidColorBrush(Colors.Blue));
            GridCommon.PlacingChild(eRoot, rect1_1, 0, 0, true);
            GridCommon.PlacingChild(eRoot, rect1_2, 1, 0, true);
            GridCommon.PlacingChild(eRoot, rect1_3, 0, 1, true);
            Grid.SetColumnSpan(rect1_3, 2);
            rect1_3.HorizontalAlignment = HorizontalAlignment.Right;
            rect1_3.VerticalAlignment = VerticalAlignment.Bottom;

            return eRoot;
        }
        public override void TestVerify()
        {
            Helpers.Log("1st Column Width = " + eRoot.ColumnDefinitions[0].ActualWidth.ToString());
            Helpers.Log("2nd Column Width = " + eRoot.ColumnDefinitions[1].ActualWidth.ToString());
            Helpers.Log("2nd Column Width = " + eRoot.ColumnDefinitions[1].GetValue(FrameworkElement.WidthProperty).ToString());
            this.Result = (eRoot.ColumnDefinitions[1].ActualWidth == ((FrameworkElement)eRoot.Children[1]).ActualWidth) ? true : false;
            if (!this.Result)
                Helpers.Log("2nd Column Width should be " + ((FrameworkElement)eRoot.Children[1]).ActualWidth.ToString() + ", but it's " + eRoot.ColumnDefinitions[1].ActualWidth.ToString());
        }

    }
}
