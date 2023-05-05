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
using System.IO;
using System.Windows.Markup;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using ElementLayout.TestLibrary;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout.VisualScan;
using Microsoft.Test.Layout.TestTypes;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Filters;

namespace ElementLayout.FeatureTests.Panels
{
    [Test(1, "Panels.Grid", "FrameworkElementProps.HeightWidthDefault", TestParameters = "target=Grid, test=HeightWidthDefault")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.HeightWidthActual", TestParameters = "target=Grid, test=HeightWidthActual")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.ChildHeightWidth", TestParameters = "target=Grid, test=ChildHeightWidth")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.MinHeightWidth", TestParameters = "target=Grid, test=MinHeightWidth")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.MaxHeightWidth", TestParameters = "target=Grid, test=MaxHeightWidth")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.Visibility", TestParameters = "target=Grid, test=Visibility")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.HorizontalAlignment", TestParameters = "target=Grid, test=HorizontalAlignment")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.VerticalAlignment", TestParameters = "target=Grid, test=VerticalAlignment")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.FlowDirection", TestParameters = "target=Grid, test=FlowDirection")]
    [Test(1, "Panels.Grid", "FrameworkElementProps.Margin", TestParameters = "target=Grid, test=Margin")]
    public class GridFETest : FrameworkElementPropertiesTest
    {
        public GridFETest()
            : base(DriverState.DriverParameters["target"], DriverState.DriverParameters["test"])
        { }
    }

    #region Grid Column/Row Span cases

    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest1", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest1 : CodeTest
    {
        public GirdColumnRowSpanTest1()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 2;
        public int cell = 0;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    //px & Auto
    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest2", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest2 : CodeTest
    {
        public GirdColumnRowSpanTest2()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 2;
        public int cell = 1;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    //Auto & *
    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest3", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest3 : CodeTest
    {
        public GirdColumnRowSpanTest3()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 2;
        public int cell = 2;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    //* & *
    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest4", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest4 : CodeTest
    {
        public GirdColumnRowSpanTest4()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 2;
        public int cell = 3;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    //* & Auto
    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest5", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest5 : CodeTest
    {
        public GirdColumnRowSpanTest5()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 2;
        public int cell = 4;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    //*, Px, Auto
    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest6", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest6 : CodeTest
    {
        public GirdColumnRowSpanTest6()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 3;
        public int cell = 0;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    //px, Auto, *
    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest7", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest7 : CodeTest
    {
        public GirdColumnRowSpanTest7()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 3;
        public int cell = 1;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    //Auto, *, *
    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest8", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest8 : CodeTest
    {
        public GirdColumnRowSpanTest8()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 3;
        public int cell = 2;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    //*, px, Auto, *
    [Test(1, "Panels.Grid", "GirdColumnRowSpanTest9", Variables="Area=ElementLayout")]
    public class GirdColumnRowSpanTest9 : CodeTest
    {
        public GirdColumnRowSpanTest9()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public int numSpan = 4;
        public int cell = 0;
        public override FrameworkElement TestContent()
        {
            eRoot = (Grid)GridCommon.GridColumnRowSpanTestCommon.CommonContent(numSpan, cell);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnRowSpanTestCommon.GridColumnRowSpanTestVerifier(eRoot, numSpan, cell, TestLog.Current);
        }
    }

    #endregion

    [Test(1, "Panels.Grid", "ColumnRowTest.GetColumnSpan_Valid", TestParameters = "test=GetColumnSpan_Valid")]
    [Test(1, "Panels.Grid", "ColumnRowTest.GetColumn_Valid", TestParameters = "test=GetColumn_Valid")]
    [Test(1, "Panels.Grid", "ColumnRowTest.GetColumnSpan_Null_Child", TestParameters = "test=GetColumnSpan_Null_Child")]
    [Test(1, "Panels.Grid", "ColumnRowTest.GetColumn_Null_Child", TestParameters = "test=GetColumn_Null_Child")]
    [Test(1, "Panels.Grid", "ColumnRowTest.SetColumnSpan_Valid", TestParameters = "test=SetColumnSpan_Valid")]
    [Test(1, "Panels.Grid", "ColumnRowTest.SetColumn_Valid", TestParameters = "test=SetColumn_Valid")]
    [Test(1, "Panels.Grid", "ColumnRowTest.SetColumnSpan_Null_Child", TestParameters = "test=SetColumnSpan_Null_Child")]
    [Test(1, "Panels.Grid", "ColumnRowTest.SetColumn_Null_Child", TestParameters = "test=SetColumn_Null_Child")]
    [Test(1, "Panels.Grid", "ColumnRowTest.GetRowSpan_Valid", TestParameters = "test=GetRowSpan_Valid")]
    [Test(1, "Panels.Grid", "ColumnRowTest.GetRow_Valid", TestParameters = "test=GetRow_Valid")]
    [Test(1, "Panels.Grid", "ColumnRowTest.GetRowSpan_Null_Child", TestParameters = "test=GetRowSpan_Null_Child")]
    [Test(1, "Panels.Grid", "ColumnRowTest.GetRow_Null_Child", TestParameters = "test=GetRow_Null_Child")]
    [Test(1, "Panels.Grid", "ColumnRowTest.SetRowSpan_Valid", TestParameters = "test=SetRowSpan_Valid")]
    [Test(1, "Panels.Grid", "ColumnRowTest.SetRow_Valid", TestParameters = "test=SetRow_Valid")]
    [Test(1, "Panels.Grid", "ColumnRowTest.SetRowSpan_Null_Child", TestParameters = "test=SetRowSpan_Null_Child")]
    [Test(1, "Panels.Grid", "ColumnRowTest.SetRow_Null_Child", TestParameters = "test=SetRow_Null_Child")]
    public class GridGetSetColumnRowTest : CodeTest
    {
        public GridGetSetColumnRowTest() { }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            _root = new WrapPanel();
            _target = new Grid();
            _target.Background = Brushes.DarkBlue;
            _target.Height = 400;
            _target.Width = 400;
            _root.Children.Add(_target);
            return _root;
        }

        public override void TestActions()
        {
            string test = DriverState.DriverParameters["test"];

            AddRows(10, _target);
            AddColumns(10, _target);

            Rectangle child = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, Brushes.Orange);
            Rectangle child_bad = null;

            _target.Children.Add(child);

            switch (test)
            {
                #region Row / RowSpan Tests

                case "SetRow_Null_Child":
                    //set row null child
                    try
                    {
                        Grid.SetRow(child_bad, 6);
                        Helpers.Log("FAIL : Should not be able to set row on null element.");
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("PASS : Exception caught setting row on null element.");
                    }
                    break;

                case "SetRowSpan_Null_Child":
                    //set rowspan null child
                    try
                    {
                        Grid.SetRowSpan(child_bad, 3);
                        Helpers.Log("FAIL : Should not be able to set row span on null element.");
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("PASS : Exception caught setting row span on null element.");
                    }
                    break;

                case "SetRow_Valid":
                    //set row valid child
                    try
                    {
                        Grid.SetRow(child, 6);
                        Helpers.Log("PASS : Able to set row on element.");
                    }
                    catch (Exception)
                    {
                        Helpers.Log("FAIL : Exception caught setting row on valid element.");
                        _tempresult = false;
                    }
                    break;

                case "SetRowSpan_Valid":
                    //set row span valid child
                    try
                    {
                        Grid.SetRowSpan(child, 3);
                        Helpers.Log("PASS : Able to set row span on element.");
                    }
                    catch (Exception)
                    {
                        Helpers.Log("FAIL : Exception caught setting row span on valid element.");
                        _tempresult = false;
                    }
                    break;

                case "GetRow_Null_Child":
                    //get row null child.
                    try
                    {
                        Grid.GetRow(child_bad);
                        Helpers.Log("FAIL : Should not be able to get row on null element.");
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("PASS : Exception caught getting row on null element.");
                    }
                    break;

                case "GetRowSpan_Null_Child":
                    //get row span null child.
                    try
                    {
                        Grid.GetRowSpan(child_bad);
                        Helpers.Log("FAIL : Should not be able to get row span on null element.");
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("PASS : Exception caught getting row span on null element.");
                    }
                    break;

                case "GetRow_Valid":
                    //get row valid child
                    try
                    {
                        Grid.GetRow(child);
                        Helpers.Log("PASS : Able to get row on element.");
                    }
                    catch (Exception)
                    {
                        Helpers.Log("FAIL : Exception caught getting row on valid element.");
                        _tempresult = false;
                    }
                    break;

                case "GetRowSpan_Valid":
                    //get row span valid child
                    try
                    {
                        Grid.GetRowSpan(child);
                        Helpers.Log("PASS : Able to get row span on element.");
                    }
                    catch (Exception)
                    {
                        Helpers.Log("FAIL : Exception caught getting row span on valid element.");
                        _tempresult = false;
                    }
                    break;

                #endregion

                #region Column / ColumnSpan Tests

                case "SetColumn_Null_Child":
                    //set column null child
                    try
                    {
                        Grid.SetColumn(child_bad, 2);
                        Helpers.Log("FAIL : Should not be able to set column on null element.");
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("PASS : Exception caught setting column on null element.");
                    }
                    break;

                case "SetColumnSpan_Null_Child":
                    //set column span null child
                    try
                    {
                        Grid.SetColumnSpan(child_bad, 5);
                        Helpers.Log("FAIL : Should not be able to set column span on null element.");
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("PASS : Exception caught setting column span on null element.");
                    }
                    break;

                case "SetColumn_Valid":
                    //set column on valid child
                    try
                    {
                        Grid.SetColumn(child, 6);
                        Helpers.Log("PASS : Able to set column on element.");
                    }
                    catch (Exception)
                    {
                        Helpers.Log("FAIL : Exception caught setting column on valid element.");
                        _tempresult = false;
                    }
                    break;

                case "SetColumnSpan_Valid":
                    //set column span on valid child
                    try
                    {
                        Grid.SetColumnSpan(child, 7);
                        Helpers.Log("PASS : Able to set column span on element.");
                    }
                    catch (Exception)
                    {
                        Helpers.Log("FAIL : Exception caught setting column span on valid element.");
                        _tempresult = false;
                    }
                    break;

                case "GetColumn_Null_Child":
                    //get column on null child
                    try
                    {
                        Grid.GetColumn(child_bad);
                        Helpers.Log("FAIL : Should not be able to get column on null element.");
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("PASS : Exception caught getting column on null element.");
                    }
                    break;

                case "GetColumnSpan_Null_Child":
                    //get column span on null child
                    try
                    {
                        Grid.GetColumnSpan(child_bad);
                        Helpers.Log("FAIL : Should not be able to get column span on null element.");
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("PASS : Exception caught getting column span on null element.");
                    }
                    break;

                case "GetColumn_Valid":
                    //get column on valid child
                    try
                    {
                        Grid.GetColumn(child);
                        Helpers.Log("PASS : Able to get column on element.");
                    }
                    catch (Exception)
                    {
                        Helpers.Log("FAIL : Exception caught getting column on valid element.");
                        _tempresult = false;
                    }
                    break;

                case "GetColumnSpan_Valid":
                    //get column span on valid child
                    try
                    {
                        Grid.GetColumnSpan(child);
                        Helpers.Log("PASS : Able to get column span on element.");
                    }
                    catch (Exception)
                    {
                        Helpers.Log("FAIL : Exception caught getting column span on valid element.");
                        _tempresult = false;
                    }
                    break;

                #endregion

                default:
                    Helpers.Log("FAIL : No vailid test argument.");
                    _tempresult = false;
                    break;
            }
        }

        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
                Helpers.Log("Colum and Row Definition IEnumeration Test Failed.");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Colum and Row Definition IEnumeration Test Passed.");
            }
        }

        private WrapPanel _root = null;
        private Grid _target = null;

        private void AddRows(int count, Grid _target)
        {
            for (int i = 0; i < count; i++)
            {
                RowDefinition row = new RowDefinition();
                _target.RowDefinitions.Add(row);
            }
        }

        private void AddColumns(int count, Grid _target)
        {
            for (int i = 0; i < count; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                _target.ColumnDefinitions.Add(col);

            }
        }

        private bool _tempresult = true;
    }

    [Test(1, "Panels.Grid", "GridOverlappingTest", Variables="Area=ElementLayout")]
    public class GridOverlappingTest : CodeTest
    {
        public GridOverlappingTest()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] gridLegnthC = new GridLength[] { new GridLength(70), new GridLength(70), new GridLength(70), new GridLength(70), new GridLength(70), new GridLength(70), new GridLength(70) };
            GridLength[] gridLegnthR = new GridLength[] { new GridLength(40), new GridLength(40), new GridLength(40), new GridLength(40), new GridLength(40), new GridLength(40), new GridLength(40) };
            eRoot = GridCommon.CreateGrid(7, 7, gridLegnthC, gridLegnthR);
            eRoot.HorizontalAlignment = HorizontalAlignment.Left;
            eRoot.VerticalAlignment = VerticalAlignment.Top;

            Rectangle rect = CommonFunctionality.CreateRectangle(100, 40, new SolidColorBrush(Colors.Beige));
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.ClipToBounds = false;
            GridCommon.SettingSpan(rect, 7, 0);
            eRoot.Children.Add(rect);

            Rectangle rect1 = CommonFunctionality.CreateRectangle(100, 40, new SolidColorBrush(Colors.Blue));
            rect1.HorizontalAlignment = HorizontalAlignment.Left;
            rect1.ClipToBounds = false;
            GridCommon.PlacingChild(eRoot, rect1, 1, 0, true);

            Rectangle rect2 = CommonFunctionality.CreateRectangle(100, 20, new SolidColorBrush(Colors.Red));
            rect2.HorizontalAlignment = HorizontalAlignment.Left;
            rect2.ClipToBounds = false;
            GridCommon.PlacingChild(eRoot, rect2, 0, 0, true);

            Rectangle rect3 = CommonFunctionality.CreateRectangle(100, 30, new SolidColorBrush(Colors.Green));
            rect3.HorizontalAlignment = HorizontalAlignment.Left;
            rect3.ClipToBounds = false;
            GridCommon.PlacingChild(eRoot, rect3, 2, 0, true);

            return eRoot;
        }

        public override void TestVerify()
        {
            this.Result = true;
            Point[] act = GridCommon.GettingActualPosition(eRoot);
            Point[] exp = GridCommon.SettingExpectedPosition(eRoot);

            //Log status for Actual and Expected Value
            Helpers.Log("===Acutal Value===");
            for (int f = 0; f < act.Length; f++)
            {
                Helpers.Log("x: " + act[f].X + "\ty: " + act[f].Y);
            }
            Helpers.Log("===Expected Value===");
            for (int f = 0; f < exp.Length; f++)
            {
                Helpers.Log("x: " + exp[f].X + "\ty: " + exp[f].Y);
            }
            // Compare the values
            if (!CompareValues(exp, act))
            {
                Helpers.Log("===Failed!!!===");
                this.Result = false;
            }
        }

        public bool CompareValues(Point[] exp, Point[] act)
        {
            for (int c = 0; c < exp.Length; c++)
            {
                if (exp[c] != act[c])
                {
                    return false;
                }
            }
            return true;
        }
    }

    [Test(1, "Panels.Grid", "ColumnRowTest.Basic", Variables = "Area=ElementLayout")]
    public class GridColumnRowTest : CodeTest
    {
        public GridColumnRowTest() { }

        public override void WindowSetup()
        {
            this.window.Width = 708;
            this.window.Height = 728;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            GridLength[] gridLength = new GridLength[] { new GridLength(1, GridUnitType.Star), new GridLength(100, GridUnitType.Pixel), new GridLength(1, GridUnitType.Auto), new GridLength(3, GridUnitType.Star), new GridLength(2, GridUnitType.Star), new GridLength(1, GridUnitType.Auto) };
            _eRoot = GridCommon.CreateGrid(6, 6, gridLength, gridLength);
            _eRoot.ShowGridLines = true;

            Rectangle[] rect = new Rectangle[6];
            //Create child and placing diffrent column and row.
            for (int n = 0; n < 6; n++)
            {
                rect[n] = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.Orange));
                GridCommon.PlacingChild(_eRoot, rect[n], n, n, true);
            }
            return _eRoot;
        }

        public override void TestVerify()
        {
            this.Result = true;
            for (int i = 0; i < 6; i++)
            {
                if ((Grid.GetColumn(_eRoot.Children[i]) != i) || (Grid.GetRow(_eRoot.Children[i]) != i))
                {
                    Helpers.Log("\nRectangle: " + i + " is placed in (" + Grid.GetColumn(_eRoot.Children[i]) + "," + Grid.GetRow(_eRoot.Children[i]) + "), not in (" + i + "," + i + ")");
                    this.Result = false;
                }
            }
        }

        private Grid _eRoot = null;
    }

    [Test(1, "Panels.Grid", "ColumnRowTest.RowMin", TestParameters = "test=RowMin")]
    [Test(1, "Panels.Grid", "ColumnRowTest.RowMax", TestParameters = "test=RowMax")]
    [Test(1, "Panels.Grid", "ColumnRowTest.ColumnMin", TestParameters = "test=ColumnMin")]
    [Test(1, "Panels.Grid", "ColumnRowTest.ColumnMax", TestParameters = "test=ColumnMax")]
    public class ColumnRowMinMax : CodeTest
    {
        public ColumnRowMinMax() { }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            _target = new Grid();
            _target.Background = Brushes.DarkBlue;
            return _target;
        }

        public override void TestActions()
        {
            string test = DriverState.DriverParameters["test"];

            _target.RowDefinitions.Clear();
            _target.ColumnDefinitions.Clear();
            CommonFunctionality.FlushDispatcher();

            switch (test)
            {
                case "RowMin":
                    AddRows(4, "min");
                    CommonFunctionality.FlushDispatcher();
                    Validate("row", "min");
                    break;
                case "RowMax":
                    AddRows(4, "max");
                    CommonFunctionality.FlushDispatcher();
                    Validate("row", "max");
                    break;
                case "ColumnMin":
                    AddColumns(4, "min");
                    CommonFunctionality.FlushDispatcher();
                    Validate("col", "min");
                    break;
                case "ColumnMax":
                    AddColumns(4, "max");
                    CommonFunctionality.FlushDispatcher();
                    Validate("col", "max");
                    break;
                default:
                    Helpers.Log("FAIL : No vailid test argument.");
                    _tempresult = false;
                    break;
            }
        }

        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
                Helpers.Log("Colum and Row Definition Min/Max Test Failed.");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Colum and Row Definition Min/Max Test Passed.");
            }
        }

        private void AddRows(int count, string testtype)
        {
            for (int i = 0; i < count; i++)
            {
                _row = new RowDefinition();
                switch (testtype)
                {
                    case "min":
                        _row.MinHeight = 100;
                        break;
                    case "max":
                        _row.MaxHeight = 100;
                        break;
                }
                _target.RowDefinitions.Add(_row);
            }
        }

        private void AddColumns(int count, string testtype)
        {
            for (int i = 0; i < count; i++)
            {
                _col = new ColumnDefinition();
                switch (testtype)
                {
                    case "min":
                        _col.MinWidth = 100;
                        break;
                    case "max":
                        _col.MaxWidth = 100;
                        break;
                }
                _target.ColumnDefinitions.Add(_col);
            }
        }

        private void Validate(string rowcol, string testtype)
        {
            switch (rowcol)
            {
                case "row":
                    foreach (RowDefinition r in _target.RowDefinitions)
                    {
                        switch (testtype)
                        {
                            case "min":
                                double minheight = r.MinHeight;
                                if (minheight == 0)
                                {
                                    Helpers.Log("FAIL : RowDefinition.MinHeight was not set correctly.");
                                    _tempresult = false;
                                }
                                else
                                {
                                    if (minheight != (double)r.GetValue(RowDefinition.MinHeightProperty))
                                    {
                                        Helpers.Log("RowDefinition.MinHeight != RowDefinition.GetValue(MinHeight).");
                                        _tempresult = false;
                                    }
                                }
                                break;
                            case "max":
                                double maxheight = r.MaxHeight;
                                if (maxheight == 0)
                                {
                                    Helpers.Log("FAIL : RowDefinition.MaxHeight was not set correctly.");
                                    _tempresult = false;
                                }
                                else
                                {
                                    if (maxheight != (double)r.GetValue(RowDefinition.MaxHeightProperty))
                                    {
                                        Helpers.Log("RowDefinition.MaxHeight != RowDefinition.GetValue(MaxHeight).");
                                        _tempresult = false;
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case "col":
                    foreach (ColumnDefinition c in _target.ColumnDefinitions)
                    {
                        switch (testtype)
                        {
                            case "min":
                                double minwidth = c.MinWidth;
                                if (minwidth == 0)
                                {
                                    Helpers.Log("FAIL : ColumnDefinition.MinWidth was not set correctly.");
                                    _tempresult = false;
                                }
                                else
                                {
                                    if (minwidth != (double)c.GetValue(ColumnDefinition.MinWidthProperty))
                                    {
                                        Helpers.Log("ColumnDefinition.MinWidth != ColumnDefinition.GetValue(MinWidth).");
                                        _tempresult = false;
                                    }
                                }
                                break;
                            case "max":
                                double maxwidth = c.MaxWidth;
                                if (maxwidth == 0)
                                {
                                    Helpers.Log("FAIL : ColumnDefinition.MaxWidth was not set correctly.");
                                    _tempresult = false;
                                }
                                else
                                {
                                    if (maxwidth != (double)c.GetValue(ColumnDefinition.MaxWidthProperty))
                                    {
                                        Helpers.Log("ColumnDefinition.MaxWidth != ColumnDefinition.GetValue(MaxWidth).");
                                        _tempresult = false;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        private bool _tempresult = true;
        private Grid _target = null;
        private RowDefinition _row = null;
        private ColumnDefinition _col = null;
    }

    [Test(1, "Panels.Grid", "ColumnRowDefIList", Variables="Area=ElementLayout")]
    public class ColumnRowDefIList : CodeTest
    {
        public ColumnRowDefIList()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        WrapPanel _root;
        Grid _target;
        Grid _target2;

        public override FrameworkElement TestContent()
        {
            _root = new WrapPanel();

            _target = new Grid();
            _target.Background = Brushes.DarkBlue;
            _target.Height = 400;
            _target.Width = 400;
            _root.Children.Add(_target);

            _target2 = new Grid();
            _target2.Height = 300;
            _target2.Width = 300;
            _target2.Background = Brushes.Crimson;
            _root.Children.Add(_target2);

            return _root;
        }

        public override void TestActions()
        {
            //////////////////////////////////////
            // ROW DEFINITION TESTS
            //////////////////////////////////////

            Helpers.Log("*** Row Definition Collection IList Tests ************************");

            #region Row IList.Contains Test

            RowDefinition contains_row = null;

            //if grid contains null rowdef
            if (((IList)_target.RowDefinitions).Contains(contains_row))
            {
                _tempresult = false;
                Helpers.Log("Row collection should not contain row.");
            }

            contains_row = new RowDefinition();
            _target.RowDefinitions.Add(contains_row);

            //wrong parent contains check
            if (((IList)_target2.RowDefinitions).Contains(contains_row))
            {
                _tempresult = false;
                Helpers.Log("Row collection should not contain row.");
            }

            //grid actually contains rowdef
            if (!((IList)_target.RowDefinitions).Contains(contains_row))
            {
                _tempresult = false;
                Helpers.Log("Row collection should contain row.");
            }

            #endregion

            #region Row IList.IndexOf Test

            _target.RowDefinitions.Clear();
            AddRows(22, _target);
            CommonFunctionality.FlushDispatcher();

            RowDefinition indexof_row = null;

            if (_target.RowDefinitions.IndexOf(indexof_row) != -1)
            {
                _tempresult = false;
                Helpers.Log("Row collection IndexOf should be -1 for null item.");
            }

            if (_target2.RowDefinitions.IndexOf(indexof_row) != -1)
            {
                _tempresult = false;
                Helpers.Log("Row collection IndexOf should be -1 for checking wrong parent.");
            }

            indexof_row = new RowDefinition();
            _target.RowDefinitions.Add(indexof_row);

            if (((IList)_target.RowDefinitions).IndexOf(indexof_row) != (_target.RowDefinitions.Count - 1))
            {
                _tempresult = false;
                Helpers.Log("Row collection IndexOf is incorrect.");
            }

            if (_target.RowDefinitions.IndexOf(indexof_row) != (_target.RowDefinitions.Count - 1))
            {
                _tempresult = false;
                Helpers.Log("Row collection IndexOf is incorrect.");
            }

            #endregion

            #region Row IList.Insert Tests

            _target.RowDefinitions.Clear();
            AddRows(10, _target);
            CommonFunctionality.FlushDispatcher();

            RowDefinition insert_row = null;

            try
            {
                ((IList)_target.RowDefinitions).Insert(5, insert_row);
                _tempresult = false;
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("Null exception caught inserting null item in RowDefinition.");
            }

            insert_row = new RowDefinition();
            _target.RowDefinitions.Add(insert_row);

            try
            {
                ((IList)_target.RowDefinitions).Insert(-1, insert_row);
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Out of range exception caught inserting item in RowDefinition at -1.");
            }

            try
            {
                ((IList)_target.RowDefinitions).Insert(50, insert_row);
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Out of range exception caught inserting item in RowDefinition at value > size.");
            }

            #endregion

            #region Row IList.Remove Tests

            _target.RowDefinitions.Clear();
            AddRows(15, _target);
            CommonFunctionality.FlushDispatcher();

            RowDefinition remove_row = null;

            try
            {
                ((IList)_target.RowDefinitions).Remove(remove_row);
                _tempresult = false;
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("Arument Null Exception caught removing null item from RowDefinition.");
            }

            remove_row = new RowDefinition();
            _target.RowDefinitions.Add(remove_row);
            AddRows(15, _target);

            int pre_remove = _target.RowDefinitions.Count;

            ((IList)_target.RowDefinitions).Remove(remove_row);

            if (_target.RowDefinitions.Count != (pre_remove - 1))
            {
                _tempresult = false;
                Helpers.Log("RowDefinition count is incorrect after remove.");
            }

            #endregion

            #region RowDefinition Collection get/set item

            _target.RowDefinitions.Clear();
            AddRows(50, _target);
            RowDefinition getset_row = null;

            try
            {
                _target.RowDefinitions[0] = getset_row;
                _tempresult = false;
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("Exception was caught when setting RowDefinition[0] to null item.");
            }

            getset_row = new RowDefinition();

            try
            {
                _target.RowDefinitions[-1] = getset_row;
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Exception was caught when setting RowDefinition[-1].");
            }

            try
            {
                _target.RowDefinitions[100] = getset_row;
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Exception was caught when setting RowDefinition[value > size].");
            }

            try
            {
                _target.RowDefinitions[0] = getset_row;
            }
            catch (Exception)
            {
                Helpers.Log("Exception should not be thrown setting valid row definiton postion.");
                _tempresult = false;
            }

            _target.RowDefinitions.Clear();
            AddRows(50, _target);
            RowDefinition getset_ilist_row = null;

            try
            {
                ((IList)_target.RowDefinitions)[0] = getset_ilist_row;
                _tempresult = false;
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("Exception was caught when setting RowDefinition[0] to null item.");
            }

            getset_ilist_row = new RowDefinition();

            try
            {
                ((IList)_target.RowDefinitions)[-1] = getset_ilist_row;
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Exception was caught when setting RowDefinition[-1].");
            }

            try
            {
                ((IList)_target.RowDefinitions)[100] = getset_ilist_row;
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Exception was caught when setting RowDefinition[value > size].");
            }

            try
            {
                ((IList)_target.RowDefinitions)[0] = getset_ilist_row;
            }
            catch (Exception)
            {
                Helpers.Log("Exception should not be thrown setting valid row definiton postion.");
                _tempresult = false;
            }
            #endregion


            /////////////////////////////////////
            // COLUMN DEFINITION TEST
            /////////////////////////////////////

            Helpers.Log("*** Column Definition Collection IList Tests *********************");

            # region Column IList.Contains Tests
            ColumnDefinition contains_col = new ColumnDefinition();

            if (((IList)_target.ColumnDefinitions).Contains(contains_col))
            {
                _tempresult = false;
                Helpers.Log("Column collection should not contain column.");
            }

            _target.ColumnDefinitions.Add(contains_col);

            if (((IList)_target2.ColumnDefinitions).Contains(contains_col))
            {
                _tempresult = false;
                Helpers.Log("Column collection should not contain column.");
            }

            if (!((IList)_target.ColumnDefinitions).Contains(contains_col))
            {
                _tempresult = false;
                Helpers.Log("Column collection should contain column.");
            }
            #endregion

            #region Column IList.IndexOf Test

            _target.ColumnDefinitions.Clear();
            AddColumns(22, _target);
            CommonFunctionality.FlushDispatcher();

            ColumnDefinition indexof_column = null;

            if (_target.ColumnDefinitions.IndexOf(indexof_column) != -1)
            {
                _tempresult = false;
                Helpers.Log("Column collection IndexOf should be -1 for null item.");
            }

            if (_target2.ColumnDefinitions.IndexOf(indexof_column) != -1)
            {
                _tempresult = false;
                Helpers.Log("Column collection IndexOf should be -1 for checking wrong parent.");
            }

            indexof_column = new ColumnDefinition();
            _target.ColumnDefinitions.Add(indexof_column);

            if (((IList)_target.ColumnDefinitions).IndexOf(indexof_column) != (_target.ColumnDefinitions.Count - 1))
            {
                _tempresult = false;
                Helpers.Log("Column collection IndexOf is incorrect.");
            }

            if (_target.ColumnDefinitions.IndexOf(indexof_column) != (_target.ColumnDefinitions.Count - 1))
            {
                _tempresult = false;
                Helpers.Log("Column collection IndexOf is incorrect.");
            }

            #endregion

            #region Column IList.Insert Tests

            _target.ColumnDefinitions.Clear();
            AddColumns(10, _target);
            CommonFunctionality.FlushDispatcher();

            ColumnDefinition insert_column = null;

            try
            {
                ((IList)_target.ColumnDefinitions).Insert(5, insert_column);
                _tempresult = false;
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("Null exception caught inserting null item in ColumnDefinition.");
            }

            insert_column = new ColumnDefinition();
            _target.ColumnDefinitions.Add(insert_column);

            try
            {
                ((IList)_target.ColumnDefinitions).Insert(-1, insert_column);
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Out of range exception caught inserting item in ColumnDefinition at -1.");
            }

            try
            {
                ((IList)_target.ColumnDefinitions).Insert(50, insert_column);
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Out of range exception caught inserting item in ColumnDefinition at value > size.");
            }

            #endregion

            #region Column IList.Remove Tests

            _target.ColumnDefinitions.Clear();
            AddColumns(15, _target);
            CommonFunctionality.FlushDispatcher();

            ColumnDefinition remove_column = null;

            try
            {
                ((IList)_target.ColumnDefinitions).Remove(remove_column);
                _tempresult = false;
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("Arument Null Exception caught removing null item from ColumnDefinition.");
            }

            remove_column = new ColumnDefinition();
            _target.ColumnDefinitions.Add(remove_column);
            AddColumns(15, _target);

            pre_remove = _target.ColumnDefinitions.Count;

            ((IList)_target.ColumnDefinitions).Remove(remove_column);

            if (_target.ColumnDefinitions.Count != (pre_remove - 1))
            {
                _tempresult = false;
                Helpers.Log("ColumnDefinition count is incorrect after remove.");
            }

            #endregion

            #region ColumnDefinition Collection get/set item

            _target.ColumnDefinitions.Clear();
            AddColumns(50, _target);
            ColumnDefinition getset_column = null;

            try
            {
                _target.ColumnDefinitions[0] = getset_column;
                _tempresult = false;
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("Exception was caught when setting ColumnDefinition[0] to null item.");
            }

            getset_column = new ColumnDefinition();

            try
            {
                _target.ColumnDefinitions[-1] = getset_column;
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Exception was caught when setting ColumnDefinition[-1].");
            }

            try
            {
                _target.ColumnDefinitions[100] = getset_column;
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Exception was caught when setting ColumnDefinition[value > size].");
            }

            try
            {
                _target.ColumnDefinitions[0] = getset_column;
            }
            catch (Exception)
            {
                Helpers.Log("Exception should not be thcolumnn setting valid column definiton postion.");
                _tempresult = false;
            }

            _target.ColumnDefinitions.Clear();
            AddColumns(50, _target);
            ColumnDefinition getset_ilist_column = null;

            try
            {
                ((IList)_target.ColumnDefinitions)[0] = getset_ilist_column;
                _tempresult = false;
            }
            catch (ArgumentNullException)
            {
                Helpers.Log("Exception was caught when setting ColumnDefinition[0] to null item.");
            }

            getset_ilist_column = new ColumnDefinition();

            try
            {
                ((IList)_target.ColumnDefinitions)[-1] = getset_ilist_column;
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Exception was caught when setting ColumnDefinition[-1].");
            }

            try
            {
                ((IList)_target.ColumnDefinitions)[100] = getset_ilist_column;
                _tempresult = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                Helpers.Log("Exception was caught when setting ColumnDefinition[value > size].");
            }

            try
            {
                ((IList)_target.ColumnDefinitions)[0] = getset_ilist_column;
            }
            catch (Exception)
            {
                Helpers.Log("Exception should not be thcolumnn setting valid column definiton postion.");
                _tempresult = false;
            }
            #endregion

        }

        void AddRows(int count, Grid _target)
        {
            for (int i = 0; i < count; i++)
            {
                RowDefinition row = new RowDefinition();
                _target.RowDefinitions.Add(row);
            }
        }

        void AddColumns(int count, Grid _target)
        {
            for (int i = 0; i < count; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                _target.ColumnDefinitions.Add(col);

            }
        }

        bool _tempresult = true;

        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
                Helpers.Log("Colum and Row Definition IList Test Failed.");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Colum and Row Definition IList Test Passed.");
            }
        }
    }

    [Test(1, "Panels.Grid", "ColumnRowDefICollectionIEnumerator", Variables="Area=ElementLayout")]
    public class ColumnRowDefICollectionIEnumerator : CodeTest
    {
        public ColumnRowDefICollectionIEnumerator()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        WrapPanel _root;
        Grid _target;

        int _row_count = 153;
        int _col_count = 612;

        public override FrameworkElement TestContent()
        {
            _root = new WrapPanel();

            _target = new Grid();
            _target.Background = Brushes.DarkBlue;
            _target.Height = 400;
            _target.Width = 400;
            _root.Children.Add(_target);

            return _root;
        }

        public override void TestActions()
        {
            Helpers.Log("*** Row Definition Collection IList Tests ************************");

            AddRows(_row_count, _target);

            CommonFunctionality.FlushDispatcher();

            int counter = 0;
            IEnumerator ienum_rows = ((IEnumerable)_target.RowDefinitions).GetEnumerator();

            try
            {
                RowDefinition current_Row = ienum_rows.Current as RowDefinition;
                _tempresult = false;
            }
            catch (InvalidOperationException)
            {
                Helpers.Log("Exception caught trying to access IEnumerator.Current before enumeration begins.");
            }

            while (ienum_rows.MoveNext())
            {
                if (ienum_rows.Current != null)
                {
                    counter++;
                }
            }
            if (counter != _row_count)
            {
                _tempresult = false;
                Helpers.Log("Enumerator return different child count..");
            }
            else
            {
                Helpers.Log("Expected Row count is correct.");
            }

            try
            {
                RowDefinition last_Row = ienum_rows.Current as RowDefinition;
                _tempresult = false;
            }
            catch (InvalidOperationException)
            {
                Helpers.Log("Exception caught trying to access IEnumerator.Current after enumeration ended.");
            }

            Helpers.Log("*** Column Definition Collection IList Tests *********************");

            AddColumns(_col_count, _target);
            CommonFunctionality.FlushDispatcher();

            counter = 0;
            IEnumerator ienum_cols = ((IEnumerable)_target.ColumnDefinitions).GetEnumerator();

            try
            {
                ColumnDefinition current_col = ienum_rows.Current as ColumnDefinition;
                _tempresult = false;
            }
            catch (InvalidOperationException)
            {
                Helpers.Log("Exception caught trying to access IEnumerator.Current before enumeration begins.");
            }

            while (ienum_cols.MoveNext())
            {
                if (ienum_cols.Current != null)
                {
                    counter++;
                }
            }
            if (counter != _col_count)
            {
                _tempresult = false;
                Helpers.Log("Enumerator return different child count..");
            }
            else
            {
                Helpers.Log("Expected Column count is correct.");
            }

            try
            {
                ColumnDefinition last_col = ienum_cols.Current as ColumnDefinition;
                _tempresult = false;
            }
            catch (InvalidOperationException)
            {
                Helpers.Log("Exception caught trying to access IEnumerator.Current after enumeration ended.");
            }

        }

        void AddRows(int count, Grid _target)
        {
            for (int i = 0; i < count; i++)
            {
                RowDefinition row = new RowDefinition();
                _target.RowDefinitions.Add(row);
            }
        }

        void AddColumns(int count, Grid _target)
        {
            for (int i = 0; i < count; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                _target.ColumnDefinitions.Add(col);

            }
        }

        bool _tempresult = true;

        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
                Helpers.Log("Colum and Row Definition IEnumeration Test Failed.");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Colum and Row Definition IEnumeration Test Passed.");
            }
        }
    }

    [Test(1, "Panels.Grid", "ColumnRowDefICollectionCopypTo", Variables="Area=ElementLayout")]
    public class ColumnRowDefICollectionCopypTo : CodeTest
    {
        public ColumnRowDefICollectionCopypTo()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        WrapPanel _root;
        Grid _target;
        RowDefinition _row;
        ColumnDefinition _col;

        RowDefinitionCollection _rdc;
        RowDefinition[] _copyto_rows;

        ColumnDefinitionCollection _cdc;
        ColumnDefinition[] _copyto_columns;

        public override FrameworkElement TestContent()
        {
            _root = new WrapPanel();

            _target = new Grid();
            _target.Background = Brushes.DarkBlue;
            _target.Height = 400;
            _target.Width = 400;
            _root.Children.Add(_target);

            return _root;
        }

        public override void TestActions()
        {
            //copyto test

            Helpers.Log("*** RowDefinition ICollection Test");

            AddRows(15, _target);
            CommonFunctionality.FlushDispatcher();

            _rdc = _target.RowDefinitions;
            _copyto_rows = new RowDefinition[15];
            ((ICollection)_rdc).CopyTo(_copyto_rows, 0);

            if (_rdc.Count != _target.RowDefinitions.Count)
            {
                _tempresult = false;
                Helpers.Log("ICollect.CopyTo did not copy correct to same size array.");
            }

            CommonFunctionality.FlushDispatcher();

            _rdc = _target.RowDefinitions;
            _copyto_rows = new RowDefinition[25];
            ((ICollection)_rdc).CopyTo(_copyto_rows, 7);

            int null_count = 0;
            int actual_count = 0;
            foreach (RowDefinition r in _copyto_rows)
            {
                if (r != null)
                {
                    actual_count++;
                }
                else
                {
                    null_count++;
                }
            }

            if ((_copyto_rows.Length - _rdc.Count) != null_count &&
                (null_count + actual_count) != _copyto_rows.Length &&
                actual_count != _rdc.Count)
            {
                _tempresult = false;
                Helpers.Log("ICollection.CopyTo did not copy correct to larger array.");
            }

            CommonFunctionality.FlushDispatcher();

            bool ArgumentNullException_Caught = true;

            _rdc = _target.RowDefinitions;
            _copyto_rows = null;

            try
            {
                ((ICollection)_rdc).CopyTo(_copyto_rows, 0);
                ArgumentNullException_Caught = false;
            }
            catch (ArgumentNullException)
            {

                ArgumentNullException_Caught = true;
            }

            if (ArgumentNullException_Caught)
            {
                Helpers.Log("ArgumentNullException caught.");
            }
            else
            {
                Helpers.Log("ArgumentNullException was not caught.");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            bool ArgumentException_Rank_Caught = true;

            _rdc = _target.RowDefinitions;
            RowDefinition[,] copyto_rows_rank = new RowDefinition[15, 15];

            try
            {
                ((ICollection)_rdc).CopyTo(copyto_rows_rank, 0);
                ArgumentException_Rank_Caught = false;
            }
            catch (ArgumentException)
            {
                ArgumentException_Rank_Caught = true;
            }

            if (ArgumentException_Rank_Caught)
            {
                Helpers.Log("ArgumentException caught.");
            }
            else
            {
                Helpers.Log("ArgumentException was not caught.");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            bool ArgumentOutOfRangeException_Caught = true;

            _rdc = _target.RowDefinitions;
            _copyto_rows = new RowDefinition[20];

            try
            {
                ((ICollection)_rdc).CopyTo(_copyto_rows, -1);
                ArgumentOutOfRangeException_Caught = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                ArgumentOutOfRangeException_Caught = true;
            }

            if (ArgumentOutOfRangeException_Caught)
            {
                Helpers.Log("ArgumentOutOfRangeException caught.");
            }
            else
            {
                Helpers.Log("ArgumentOutOfRangeException was not caught.");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            bool ArgumentException_Length_Caught = true;

            _rdc = _target.RowDefinitions;
            _copyto_rows = new RowDefinition[3];

            try
            {
                ((ICollection)_rdc).CopyTo(copyto_rows_rank, 0);
                ArgumentException_Length_Caught = false;
            }
            catch (ArgumentException)
            {
                ArgumentException_Length_Caught = true;
            }

            if (ArgumentException_Length_Caught)
            {
                Helpers.Log("ArgumentException caught.");
            }
            else
            {
                Helpers.Log("ArgumentException was not caught.");
                _tempresult = false;
            }

            Helpers.Log("*** ColumnDefinition ICollection Test");

            _target.RowDefinitions.Clear();
            AddColumns(15, _target);
            CommonFunctionality.FlushDispatcher();

            _cdc = _target.ColumnDefinitions;
            _copyto_columns = new ColumnDefinition[15];
            ((ICollection)_cdc).CopyTo(_copyto_columns, 0);

            if (_cdc.Count != _target.ColumnDefinitions.Count)
            {
                _tempresult = false;
                Helpers.Log("ICollect.CopyTo did not copy correct to same size array.");
            }

            CommonFunctionality.FlushDispatcher();

            _cdc = _target.ColumnDefinitions;
            _copyto_columns = new ColumnDefinition[25];
            ((ICollection)_cdc).CopyTo(_copyto_columns, 7);

            null_count = 0;
            actual_count = 0;

            foreach (ColumnDefinition r in _copyto_columns)
            {
                if (r != null)
                {
                    actual_count++;
                }
                else
                {
                    null_count++;
                }
            }

            if ((_copyto_columns.Length - _cdc.Count) != null_count &&
                (null_count + actual_count) != _copyto_columns.Length &&
                actual_count != _cdc.Count)
            {
                _tempresult = false;
                Helpers.Log("ICollection.CopyTo did not copy correct to larger array.");
            }

            CommonFunctionality.FlushDispatcher();

            ArgumentNullException_Caught = true;

            _cdc = _target.ColumnDefinitions;
            _copyto_columns = null;

            try
            {
                ((ICollection)_cdc).CopyTo(_copyto_columns, 0);
                ArgumentNullException_Caught = false;
            }
            catch (ArgumentNullException)
            {

                ArgumentNullException_Caught = true;
            }

            if (ArgumentNullException_Caught)
            {
                Helpers.Log("ArgumentNullException caught.");
            }
            else
            {
                Helpers.Log("ArgumentNullException was not caught.");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            ArgumentException_Rank_Caught = true;

            _cdc = _target.ColumnDefinitions;
            ColumnDefinition[,] copyto_columns_rank = new ColumnDefinition[15, 15];

            try
            {
                ((ICollection)_cdc).CopyTo(copyto_columns_rank, 0);
                ArgumentException_Rank_Caught = false;
            }
            catch (ArgumentException)
            {
                ArgumentException_Rank_Caught = true;
            }

            if (ArgumentException_Rank_Caught)
            {
                Helpers.Log("ArgumentException caught.");
            }
            else
            {
                Helpers.Log("ArgumentException was not caught.");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            ArgumentOutOfRangeException_Caught = true;

            _cdc = _target.ColumnDefinitions;
            _copyto_columns = new ColumnDefinition[20];

            try
            {
                ((ICollection)_cdc).CopyTo(_copyto_columns, -1);
                ArgumentOutOfRangeException_Caught = false;
            }
            catch (ArgumentOutOfRangeException)
            {
                ArgumentOutOfRangeException_Caught = true;
            }

            if (ArgumentOutOfRangeException_Caught)
            {
                Helpers.Log("ArgumentOutOfRangeException caught.");
            }
            else
            {
                Helpers.Log("ArgumentOutOfRangeException was not caught.");
                _tempresult = false;
            }

            CommonFunctionality.FlushDispatcher();

            ArgumentException_Length_Caught = true;

            _cdc = _target.ColumnDefinitions;
            _copyto_columns = new ColumnDefinition[3];

            try
            {
                ((ICollection)_cdc).CopyTo(copyto_columns_rank, 0);
                ArgumentException_Length_Caught = false;
            }
            catch (ArgumentException)
            {
                ArgumentException_Length_Caught = true;
            }

            if (ArgumentException_Length_Caught)
            {
                Helpers.Log("ArgumentException caught.");
            }
            else
            {
                Helpers.Log("ArgumentException was not caught.");
                _tempresult = false;
            }

        }

        void AddRows(int count, Grid _target)
        {
            for (int i = 0; i < count; i++)
            {
                _row = new RowDefinition();
                _target.RowDefinitions.Add(_row);
            }
        }

        void AddColumns(int count, Grid _target)
        {
            for (int i = 0; i < count; i++)
            {
                _col = new ColumnDefinition();
                _target.ColumnDefinitions.Add(_col);

            }
        }

        bool _tempresult = true;

        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
                Helpers.Log("Colum and Row Definition ICollection Test Failed.");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Colum and Row Definition ICollection Test Passed.");
            }
        }
    }

    [Test(1, "Panels.Grid", "Exceptions.SetIsSharedSizeScope_Null", TestParameters = "target=SetIsSharedSizeScope_Null")]
    [Test(1, "Panels.Grid", "Exceptions.GetIsSharedSizeScope_Null", TestParameters = "target=GetIsSharedSizeScope_Null")]
    [Test(1, "Panels.Grid", "Exceptions.SetIsSharedSizeScope_Valid", TestParameters = "target=SetIsSharedSizeScope_Valid")]
    [Test(1, "Panels.Grid", "Exceptions.GetIsSharedSizeScope_Valid", TestParameters = "target=GetIsSharedSizeScope_Valid")]
    [Test(1, "Panels.Grid", "Exceptions.GetVisualChild", TestParameters = "target=GetVisualChild")]
    public class GridExceptions : CodeTest
    {
        public GridExceptions() { }

        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            _root = new Grid();
            return _root;
        }

        public override void TestActions()
        {
            string target = DriverState.DriverParameters["target"];

            switch (target)
            {
                case "SetIsSharedSizeScope_Null":

                    Helpers.Log("TEST: Grid.SetIsSharedSizeScope on null grid.");
                    try
                    {
                        Grid.SetIsSharedSizeScope(_grid, true);
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("Correct exception caught.");
                    }
                    break;

                case "GetIsSharedSizeScope_Null":
                    Helpers.Log("TEST: Grid.GetIsSharedSizeScope on null grid.");
                    try
                    {
                        Grid.GetIsSharedSizeScope(_grid);
                        _tempresult = false;
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("Correct exception caught.");
                    }
                    break;

                case "SetIsSharedSizeScope_Valid":
                    AddContent();
                    Helpers.Log("TEST: Grid.SetIsSharedSizeScope on grid.");
                    try
                    {
                        Grid.SetIsSharedSizeScope(_grid, true);
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("Exception caught.");
                        _tempresult = false;
                    }
                    break;

                case "GetIsSharedSizeScope_Valid":
                    AddContent();
                    Helpers.Log("TEST: Grid.GetIsSharedSizeScope on grid.");
                    try
                    {
                        Grid.GetIsSharedSizeScope(_grid);
                    }
                    catch (ArgumentNullException)
                    {
                        Helpers.Log("Exception caught.");
                        _tempresult = false;
                    }
                    break;

                case "GetVisualChild":
                    AddContent();
                    Helpers.Log("TEST: Grid.GetVisualChild with 0 visual child.");
                    try
                    {
                        VisualTreeHelper.GetChild(_grid, 0);
                        _tempresult = false;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Helpers.Log("Correct exception caught.");
                    }
                    break;
            }
        }

        public override void TestVerify()
        {
            if (!_tempresult)
            {
                this.Result = false;
                Helpers.Log("Grid Exception Test Failed.");
            }
            else
            {
                this.Result = true;
                Helpers.Log("Grid Exception Test Passed.");
            }
        }

        private void AddContent()
        {
            _grid = new Grid();
            _root.Children.Add(_grid);
            CommonFunctionality.FlushDispatcher();
        }

        private bool _tempresult = true;
        private Grid _root = null;
        private Grid _grid = null;
    }

    [Test(1, "Panels.Grid", "GridPropertyAttributePairs", Variables="Area=ElementLayout")]
    public class GridPropertyAttributePairs : CodeTest
    {
        public GridPropertyAttributePairs()
        { }

        int _propertyMask = 0xffff;

        public override void TestActions()
        {
            //the list of properties to test
            DependencyProperty[] properties = new DependencyProperty[]
			{
				//Grid.TopProperty, Grid.LeftProperty, Grid.BottomProperty, Grid.RightProperty, //these take spacing
				Grid.ColumnProperty, Grid.RowProperty, Grid.ColumnSpanProperty, Grid.RowSpanProperty, //these take ints
				RowDefinition.HeightProperty, //these take GridLengths
				ColumnDefinition.WidthProperty //these take GridLengths
			};

            //a list of integer values to be used on int properties
            int[] ints = new int[]
			{
				1,	1,	1,	2,	5,	100
			};

            //a list of gridlengths to be used on GridLength properties
            GridLength[] gridLengths = new GridLength[]
			{
				default(GridLength) ,new GridLength(), new GridLength(0), new GridLength(1),
				new GridLength(1), new GridLength(50), new GridLength(100), new GridLength(1, GridUnitType.Star),
				new GridLength(2, GridUnitType.Star), new GridLength(3, GridUnitType.Star),
				new GridLength(0, GridUnitType.Auto), new GridLength(0, GridUnitType.Star)
				//new GridLength(1, GridUnitType.Percent), new GridLength(20, GridUnitType.Percent),
				//new GridLength(50, GridUnitType.Percent)
			};

            //a list of lists, where each element contains a list of values that can be applied
            //to the corresponding element in this.properties
            IList[] valueLists = new IList[]
			{
				ints, ints, ints, ints, gridLengths, gridLengths
			};

            //a list of objects that can take row definition properties
            DependencyObject[] rowDefs = new RowDefinition[]
			{
				new RowDefinition()
			};

            //a list of objects that can take column definition properties
            DependencyObject[] colDefs = new ColumnDefinition[]
			{
				new ColumnDefinition()
			};

            //a list of objects that can take dependencyobject properties
            DependencyObject[] elementObjects = new DependencyObject[]
			{
				new Rectangle()//, new Button(), new Ellipse(), new TextBox(), new Canvas()
			};

            //a list of lists, where each element is a list of objects which can take the corresponding
            //property from this.properties
            IList[] objectLists = new IList[]
			{
				elementObjects, elementObjects, elementObjects, elementObjects, elementObjects,
				elementObjects, elementObjects, elementObjects, rowDefs, colDefs
			};


            //for all property pairs
            for (int i = 0; i < 10; i++)
            {
                //if the property mask says not to test this property, skip it
                if (((1 << i) & this._propertyMask) == 0)
                {
                    continue;
                }

                for (int j = i + 1; j < 10; j++)
                {
                    //if the property mask says not to test this property, skip it
                    if (((1 << j) & this._propertyMask) == 0)
                    {
                        continue;
                    }

                    //for all pairs of values that can be assigned for this property
                    if (i < valueLists.Length)
                    {
                        foreach (object oi in valueLists[i])
                        {
                            if (j < valueLists.Length)
                            {
                                foreach (object oj in valueLists[j])
                                {
                                    //for all pairs of objects that can take this property
                                    foreach (DependencyObject doi in objectLists[i])
                                    {
                                        foreach (DependencyObject doj in objectLists[j])
                                        {
                                            //run the test
                                            if (TestPair(doi, doj, properties[i], properties[j], oi, oj))
                                            {
                                                _passes++;
                                            }
                                            else
                                            {
                                                _failures++;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        int _passes = 0;
        int _failures = 0;

        private bool TestPair(DependencyObject objectA, DependencyObject objectB, DependencyProperty propertyA, DependencyProperty propertyB, object valueA, object valueB)
        {
            objectA.SetValue(propertyA, valueA);
            objectB.SetValue(propertyB, valueB);
            object a = objectA.GetValue(propertyA);
            object b = objectB.GetValue(propertyB);
            string testString = "[Grid property attribute pairs] " + objectA + " " + objectB + " " + propertyA.OwnerType + "." + propertyA.Name + " " + propertyB.OwnerType + "." + propertyB.Name + " " + valueA + " " + valueB;
            bool result = a.Equals(valueA) && b.Equals(valueB);
            if (!result)
            {
                Helpers.Log("[FAIL] " + testString);
            }
            return result;
        }

        public override void TestVerify()
        {
            Helpers.Log(string.Format("Grid PropertyAttributePairs: " + (100 * _passes / (float)(_passes + _failures)) + "% success."));
            Helpers.Log(string.Format("" + _passes + "/" + (_passes + _failures) + " tescases passed."));

            if (_failures > 0)
            {
                this.Result = false;
            }
            else
            {
                this.Result = true;
            }
        }
    }

    #region Grid RTL

    [Test(1, "Panels.Grid", "GridRTL01", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL01 : CodeTest
    {
        Grid _grid;

        public GridRTL01()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Top = 0;
            this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();

            _grid = GridCommon.GridRTLSupportTest.ColumnRowTest(_grid);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL02", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL02 : CodeTest
    {
        Grid _grid;

        public GridRTL02()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.LeftSpacingTest(_grid);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL03", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL03 : CodeTest
    {
        Grid _grid;

        public GridRTL03()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.RightSpacingTest(_grid);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL04", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL04 : CodeTest
    {
        Grid _grid;

        public GridRTL04()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.SpanningTest(_grid);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL06", Variables = "Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL06 : CodeTest
    {
        Grid _grid;

        public GridRTL06()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithTextPanel(_grid, true, false);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL07", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL07 : CodeTest
    {
        Grid _grid;

        public GridRTL07()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithFlowPanel(_grid, true, false);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL08", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL08 : CodeTest
    {
        Grid _grid;

        public GridRTL08()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithCanvas(_grid, true, false);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL09", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL09 : CodeTest
    {
        Grid _grid;

        public GridRTL09()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithDockPanel(_grid, true, false);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL10", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL10 : CodeTest
    {
        Grid _grid;

        public GridRTL10()
        {
        }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithTextPanel(_grid, false, true);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL11", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL11 : CodeTest
    {
        Grid _grid;

        public GridRTL11()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithFlowPanel(_grid, false, true);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL12", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL12 : CodeTest
    {
        Grid _grid;

        public GridRTL12()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithCanvas(_grid, false, true);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL13", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL13 : CodeTest
    {
        Grid _grid;

        public GridRTL13()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithDockPanel(_grid, false, true);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL14", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL14 : CodeTest
    {
        Grid _grid;

        public GridRTL14()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithButton(_grid, true, false);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    [Test(1, "Panels.Grid", "GridRTL15", Variables="Area=ElementLayout/VscanMasterPath=FeatureTests\\ElementLayout\\MASTERS\\VSCAN")]
    public class GridRTL15 : CodeTest
    {
        Grid _grid;

        public GridRTL15()
        { }

        public override void WindowSetup()
        {
            this.window.Top = 0; this.window.Left = 0;
            this.window.Height = 500;
            this.window.Width = 500;

            this.window.Resources.MergedDictionaries.Add(Helpers.LoadStyle("GenericControls.xaml"));
            this.window.Content = this.TestContent();
            Helpers.AdjustWindowContentSize(this.window);
        }

        public override FrameworkElement TestContent()
        {
            Grid root = new Grid();
            _grid = GridCommon.GridRTLSupportTest.TestWithTextBox(_grid, true, false);
            _grid.ShowGridLines = true;
            root.Children.Add(_grid);
            return root;
        }

        public override void TestVerify()
        {
            VScanCommon tool = new VScanCommon(this);
            this.Result = tool.CompareImage();
        }
    }

    #endregion

    #region Row Definition Exceptions

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest01", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest01 : CodeTest
    {

        public GridRowDefinitionExceptionThrownTest01()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            RowDefinition rdNull = null;
            try { _grid.RowDefinitions.Add(rdNull); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest02", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest02 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest02()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            RowDefinition[] rdArrayNull = new RowDefinition[_grid.RowDefinitions.Count];
            rdArrayNull = null;

            try { _grid.RowDefinitions.CopyTo(rdArrayNull, 0); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest03", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest03 : CodeTest
    {

        public GridRowDefinitionExceptionThrownTest03()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            RowDefinition[] rdArray = new RowDefinition[_grid.RowDefinitions.Count];
            try { _grid.RowDefinitions.CopyTo(rdArray, -1); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest04", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest04 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest04()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            RowDefinition[] rdArray = new RowDefinition[_grid.RowDefinitions.Count];
            try { _grid.RowDefinitions.CopyTo(rdArray, 6); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest05", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest05 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest05()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            RowDefinition rd = new RowDefinition();
            try { _grid.RowDefinitions.Insert(7, rd); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest06", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest06 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest06()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            RowDefinition rdNull = null;
            try { _grid.RowDefinitions.Insert(7, rdNull); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest07", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest07 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest07()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            RowDefinition rdNull = null;
            try { _grid.RowDefinitions.Remove(rdNull); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest09", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest09 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest09()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            try { _grid.RowDefinitions.RemoveAt(7); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest10", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest10 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest10()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            try { _grid.RowDefinitions.RemoveRange(7, 2); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest11", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest11 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest11()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            try { _grid.RowDefinitions.RemoveRange(2, 5); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest12", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest12 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest12()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            try { _grid.RowDefinitions.RemoveRange(-1, 2); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRowDefinitionExceptionThrownTest13", Variables="Area=ElementLayout")]
    public class GridRowDefinitionExceptionThrownTest13 : CodeTest
    {
        public GridRowDefinitionExceptionThrownTest13()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }

        public override void TestActions()
        {
            try { _grid.RowDefinitions.RemoveRange(3, -1); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridRowDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    #endregion

    #region Column Definition Exceptions

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest01", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest01 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest01()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            ColumnDefinition cdNull = null;
            try { _grid.ColumnDefinitions.Add(cdNull); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest02", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest02 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest02()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            ColumnDefinition[] cdArrayNull = new ColumnDefinition[_grid.ColumnDefinitions.Count];
            cdArrayNull = null;

            try { _grid.ColumnDefinitions.CopyTo(cdArrayNull, 0); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest03", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest03 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest03()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            ColumnDefinition[] cdArray = new ColumnDefinition[_grid.ColumnDefinitions.Count];
            try { _grid.ColumnDefinitions.CopyTo(cdArray, -1); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest04", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest04 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest04()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            ColumnDefinition[] cdArray = new ColumnDefinition[_grid.ColumnDefinitions.Count];
            try { _grid.ColumnDefinitions.CopyTo(cdArray, 6); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest05", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest05 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest05()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            ColumnDefinition cd = new ColumnDefinition();
            try { _grid.ColumnDefinitions.Insert(7, cd); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest06", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest06 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest06()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            ColumnDefinition cdNull = null;
            try { _grid.ColumnDefinitions.Insert(7, cdNull); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest07", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest07 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest07()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            ColumnDefinition cdNull = null;
            try { _grid.ColumnDefinitions.Remove(cdNull); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest09", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest09 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest09()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            try { _grid.ColumnDefinitions.RemoveAt(7); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest10", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest10 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest10()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            try { _grid.ColumnDefinitions.RemoveRange(7, 2); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest11", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest11 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest11()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            try { _grid.ColumnDefinitions.RemoveRange(2, 5); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest12", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest12 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest12()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            try { _grid.ColumnDefinitions.RemoveRange(-1, 2); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridColumnDefinitionExceptionThrownTest13", Variables="Area=ElementLayout")]
    public class GridColumnDefinitionExceptionThrownTest13 : CodeTest
    {

        public GridColumnDefinitionExceptionThrownTest13()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        private Grid _grid;
        private Type _exceptionType = null;
        public override FrameworkElement TestContent()
        {
            _grid = GridCommon.CreateGrid(5, 5);
            return _grid;
        }
        public override void TestActions()
        {
            try { _grid.ColumnDefinitions.RemoveRange(3, -1); }
            catch (Exception e) { _exceptionType = e.GetType(); }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridColumnDefinitionExceptionThrownTestVerifier(_exceptionType, TestLog.Current);
        }
    }

    #endregion

    #region Resize Height/Width

    [Test(1, "Panels.Grid", "GridResizingWidthHeightTest1", Variables="Area=ElementLayout")]
    public class GridResizingWidthHeightTest1 : CodeTest
    {
        public GridResizingWidthHeightTest1()
        {
            // Resetting static members for when tests run in a common app domain.
            GridCommon.GridResizingWidthHeightTestCommon.failCount = 0;
            GridCommon.GridResizingWidthHeightTestCommon.resizeCount = 0;
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] gridLength = new GridLength[] { new GridLength(100), new GridLength(100), new GridLength(100), new GridLength(100) };
            eRoot = GridCommon.CreateGrid(4, 4, gridLength, gridLength);
            GridCommon.AddingCommonRectangle(eRoot, 90);
            eRoot.ShowGridLines = true;
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 8; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingWidthHeightTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridResizingWidthHeightTestCommon.failCount == 0) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridResizingWidthHeightTest2", Variables="Area=ElementLayout")]
    public class GridResizingWidthHeightTest2 : CodeTest
    {
        public GridResizingWidthHeightTest2()
        {
            // Resetting static members for when tests run in a common app domain.
            GridCommon.GridResizingWidthHeightTestCommon.failCount = 0;
            GridCommon.GridResizingWidthHeightTestCommon.resizeCount = 0;
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] gridLength = new GridLength[] { new GridLength(5, GridUnitType.Star), new GridLength(5, GridUnitType.Star), new GridLength(5, GridUnitType.Star), new GridLength(5, GridUnitType.Star) };
            eRoot = GridCommon.CreateGrid(4, 4, gridLength, gridLength);
            GridCommon.AddingCommonRectangle(eRoot, 90);
            eRoot.ShowGridLines = true;
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 8; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingWidthHeightTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridResizingWidthHeightTestCommon.failCount == 0) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridResizingWidthHeightTest3", Variables="Area=ElementLayout")]
    public class GridResizingWidthHeightTest3 : CodeTest
    {
        public GridResizingWidthHeightTest3()
        {
            // Resetting static members for when tests run in a common app domain.
            GridCommon.GridResizingWidthHeightTestCommon.failCount = 0;
            GridCommon.GridResizingWidthHeightTestCommon.resizeCount = 0;
        }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] gridLength = new GridLength[] { new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Auto) };
            eRoot = GridCommon.CreateGrid(4, 4, gridLength, gridLength);
            GridCommon.AddingCommonRectangle(eRoot, 90);
            eRoot.ShowGridLines = true;
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 8; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingWidthHeightTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridResizingWidthHeightTestCommon.failCount == 0) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridResizingWidthHeightTest4", Variables="Area=ElementLayout")]
    public class GridResizingWidthHeightTest4 : CodeTest
    {
        public GridResizingWidthHeightTest4()
        {
            // Resetting static members for when tests run in a common app domain.
            GridCommon.GridResizingWidthHeightTestCommon.failCount = 0;
            GridCommon.GridResizingWidthHeightTestCommon.resizeCount = 0;
        }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] gridLength = new GridLength[] { new GridLength(1, GridUnitType.Star), new GridLength(1, GridUnitType.Star), new GridLength(1, GridUnitType.Star), new GridLength(1, GridUnitType.Star) };
            eRoot = GridCommon.CreateGrid(4, 4, gridLength, gridLength);
            GridCommon.AddingCommonRectangle(eRoot, 90);
            eRoot.ShowGridLines = true;
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 8; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingWidthHeightTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridResizingWidthHeightTestCommon.failCount == 0) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridResizingWidthHeightTest5", Variables="Area=ElementLayout")]
    public class GridResizingWidthHeightTest5 : CodeTest
    {
        public GridResizingWidthHeightTest5()
        {
            // Resetting static members for when tests run in a common app domain.
            GridCommon.GridResizingWidthHeightTestCommon.failCount = 0;
            GridCommon.GridResizingWidthHeightTestCommon.resizeCount = 0;
        }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            GridLength[] gridLength = new GridLength[] { new GridLength(100), new GridLength(5, GridUnitType.Star), new GridLength(1, GridUnitType.Auto), new GridLength(1, GridUnitType.Star) };
            eRoot = GridCommon.CreateGrid(4, 4, gridLength, gridLength);
            GridCommon.AddingCommonRectangle(eRoot, 90);
            eRoot.ShowGridLines = true;
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 8; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingWidthHeightTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridResizingWidthHeightTestCommon.failCount == 0) ? true : false;
        }
    }

    #endregion

    #region Resize objects in grid

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest01", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest01 : CodeTest
    {
        public GridResizingVariousObjectTest01()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Button btn = CommonFunctionality.CreateButton();
                GridCommon.PlacingChild(eRoot, btn, i, i, true);
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest02", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest02 : CodeTest
    {
        public GridResizingVariousObjectTest02()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                TextBox tb = CommonFunctionality.CreateTextBox(0, 0, " ");
                GridCommon.PlacingChild(eRoot, tb, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest03", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest03 : CodeTest
    {
        public GridResizingVariousObjectTest03()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Ellipse ellipse = CommonFunctionality.CreateEllipse(25, 25, new SolidColorBrush(Colors.Orange));
                GridCommon.PlacingChild(eRoot, ellipse, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest04", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest04 : CodeTest
    {
        public GridResizingVariousObjectTest04()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            eRoot.ShowGridLines = false;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Image image = CommonFunctionality.CreateImage("cloud.bmp");
                image.Stretch = Stretch.Fill;
                GridCommon.PlacingChild(eRoot, image, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest05", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest05 : CodeTest
    {
        public GridResizingVariousObjectTest05()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                TextBlock txt = CommonFunctionality.CreateText("Text in Grid");
                GridCommon.PlacingChild(eRoot, txt, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest06", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest06 : CodeTest
    {
        public GridResizingVariousObjectTest06()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Border b = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.Orange), new Thickness(10), Brushes.Green);
                GridCommon.PlacingChild(eRoot, b, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest07", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest07 : CodeTest
    {
        public GridResizingVariousObjectTest07()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Label lbl = new Label();
                lbl.Content = "Testing Grid with Lebel~!";
                lbl.HorizontalAlignment = HorizontalAlignment.Stretch;
                lbl.VerticalAlignment = VerticalAlignment.Stretch;
                GridCommon.PlacingChild(eRoot, lbl, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest08", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest08 : CodeTest
    {
        public GridResizingVariousObjectTest08()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                ListBox lb = CommonFunctionality.CreateListBox(5);
                GridCommon.PlacingChild(eRoot, lb, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest09", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest09 : CodeTest
    {
        public GridResizingVariousObjectTest09()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Menu menu = CommonFunctionality.CreateMenu(5);
                GridCommon.PlacingChild(eRoot, menu, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest10", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest10 : CodeTest
    {
        public GridResizingVariousObjectTest10()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Canvas canvas = new Canvas();
                canvas.Background = new SolidColorBrush(Colors.SlateBlue);
                Rectangle cRect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Red));
                Canvas.SetLeft(cRect, 10);
                Canvas.SetTop(cRect, 10);
                canvas.Children.Add(cRect);
                GridCommon.PlacingChild(eRoot, canvas, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest11", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest11 : CodeTest
    {
        public GridResizingVariousObjectTest11()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                DockPanel dockpanel = new DockPanel();
                dockpanel.Background = new SolidColorBrush(Colors.SlateBlue);
                dockpanel.LastChildFill = true;
                Rectangle rect0 = CommonFunctionality.CreateRectangle(20, 100, new SolidColorBrush(Colors.Red));
                DockPanel.SetDock(rect0, Dock.Top);
                dockpanel.Children.Add(rect0);
                Rectangle rect1 = CommonFunctionality.CreateRectangle(20, 100, new SolidColorBrush(Colors.Blue));
                DockPanel.SetDock(rect1, Dock.Left);
                dockpanel.Children.Add(rect1);
                Rectangle rect2 = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, new SolidColorBrush(Colors.Yellow));
                //DockPanel.SetDock(rect2, Dock.Fill);
                dockpanel.Children.Add(rect2);
                GridCommon.PlacingChild(eRoot, dockpanel, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }

    [Test(3, "Panels.Grid", "GridResizingVariousObjectTest12", Variables="Area=ElementLayout")]
    public class GridResizingVariousObjectTest12 : CodeTest
    {
        public GridResizingVariousObjectTest12()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingVariousObjectTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Grid g = GridCommon.CreateGrid(2, 2);
                g.ShowGridLines = true;
                g.Background = new SolidColorBrush(Colors.SlateBlue);
                Rectangle gRect0 = CommonFunctionality.CreateRectangle(10, 10, new SolidColorBrush(Colors.Red));
                GridCommon.PlacingChild(g, gRect0, 0, 0);
                g.Children.Add(gRect0);

                Rectangle gRect1 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Blue));
                GridCommon.PlacingChild(g, gRect1, 1, 0);
                g.Children.Add(gRect1);

                Rectangle gRect2 = CommonFunctionality.CreateRectangle(30, 30, new SolidColorBrush(Colors.Yellow));
                GridCommon.PlacingChild(g, gRect2, 0, 1);
                g.Children.Add(gRect2);

                GridCommon.PlacingChild(eRoot, g, i, i, true);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 6; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingVariousObjectTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingVariousObjectTestCommon.result;
        }
    }
    #endregion

    #region resize and spanning

    [Test(2, "Panels.Grid", "GridResizingSpanningTest1", Variables="Area=ElementLayout")]
    public class GridResizingSpanningTest1 : CodeTest
    {
        public GridResizingSpanningTest1()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingSpanningTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Grid.SetColumnSpan(eRoot.Children[i], eRoot.ColumnDefinitions.Count - i);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 8; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingSpanningTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingSpanningTestCommon.result;
        }
    }

    [Test(2, "Panels.Grid", "GridResizingSpanningTest2", Variables="Area=ElementLayout")]
    public class GridResizingSpanningTest2 : CodeTest
    {
        public GridResizingSpanningTest2()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingSpanningTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Grid.SetRowSpan(eRoot.Children[i], eRoot.ColumnDefinitions.Count - i);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 8; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingSpanningTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingSpanningTestCommon.result;
        }
    }

    [Test(2, "Panels.Grid", "GridResizingSpanningTest3", Variables="Area=ElementLayout")]
    public class GridResizingSpanningTest3 : CodeTest
    {
        public GridResizingSpanningTest3()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridResizingSpanningTestCommon.CommonContent() as Grid;
            for (int i = 0; i < eRoot.ColumnDefinitions.Count; i++)
            {
                Grid.SetColumnSpan(eRoot.Children[i], eRoot.ColumnDefinitions.Count - i);
                Grid.SetRowSpan(eRoot.Children[i], eRoot.ColumnDefinitions.Count - i);
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 8; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingSpanningTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingSpanningTestCommon.result;
        }
    }
    #endregion

    #region resize with offsets

    [Test(2, "Panels.Grid", "GridResizingOffsetTest01", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest01 : CodeTest
    {
        public GridResizingOffsetTest01()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 10, 0, 0, 0);
                ((Rectangle)eRoot.Children[i]).HorizontalAlignment = HorizontalAlignment.Left;
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest02", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest02 : CodeTest
    {
        public GridResizingOffsetTest02()
        { }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 0, 10, 0, 0);
                ((Rectangle)eRoot.Children[i]).VerticalAlignment = VerticalAlignment.Top;
            }
            return eRoot;
        }

        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }

        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest03", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest03 : CodeTest
    {


        public GridResizingOffsetTest03()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 0, 0, 10, 0);
                ((Rectangle)eRoot.Children[i]).HorizontalAlignment = HorizontalAlignment.Right;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest04", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest04 : CodeTest
    {


        public GridResizingOffsetTest04()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 0, 0, 0, 10);
                ((Rectangle)eRoot.Children[i]).VerticalAlignment = VerticalAlignment.Bottom;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest05", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest05 : CodeTest
    {


        public GridResizingOffsetTest05()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest06", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest06 : CodeTest
    {


        public GridResizingOffsetTest06()
        {}
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                ((Rectangle)eRoot.Children[i]).VerticalAlignment = VerticalAlignment.Bottom;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest07", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest07 : CodeTest
    {


        public GridResizingOffsetTest07()
        {}
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                ((Rectangle)eRoot.Children[i]).HorizontalAlignment = HorizontalAlignment.Left;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest08", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest08 : CodeTest
    {


        public GridResizingOffsetTest08()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                ((Rectangle)eRoot.Children[i]).VerticalAlignment = VerticalAlignment.Top;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest09", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest09 : CodeTest
    {


        public GridResizingOffsetTest09()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 10, 10, 10, 10);
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest10", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest10 : CodeTest
    {


        public GridResizingOffsetTest10()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                ((Rectangle)eRoot.Children[i]).HorizontalAlignment = HorizontalAlignment.Center;
                ((Rectangle)eRoot.Children[i]).VerticalAlignment = VerticalAlignment.Center;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest11", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest11 : CodeTest
    {


        public GridResizingOffsetTest11()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 10, 0, 10, 0);
                ((Rectangle)eRoot.Children[i]).VerticalAlignment = VerticalAlignment.Center;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest12", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest12 : CodeTest
    {


        public GridResizingOffsetTest12()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 0, 10, 0, 10);
                ((Rectangle)eRoot.Children[i]).HorizontalAlignment = HorizontalAlignment.Center;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest13", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest13 : CodeTest
    {


        public GridResizingOffsetTest13()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 10, 10, 0, 0);
                ((Rectangle)eRoot.Children[i]).HorizontalAlignment = HorizontalAlignment.Left;
                ((Rectangle)eRoot.Children[i]).VerticalAlignment = VerticalAlignment.Top;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    [Test(2, "Panels.Grid", "GridResizingOffsetTest14", Variables="Area=ElementLayout")]
    public class GridResizingOffsetTest14 : CodeTest
    {


        public GridResizingOffsetTest14()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            Grid eRoot = GridCommon.GridResizingOffsetTestCommon.CommonContent() as Grid;
            for (int i = 0; i < 4; i++)
            {
                GridCommon.SettingOffset(eRoot.Children[i], 0, 0, 10, 10);
                ((Rectangle)eRoot.Children[i]).HorizontalAlignment = HorizontalAlignment.Right;
                ((Rectangle)eRoot.Children[i]).VerticalAlignment = VerticalAlignment.Bottom;
            }
            return eRoot;
        }
        public override void TestActions()
        {
            for (int t = 0; t < 7; t++)
            {
                CommonFunctionality.FlushDispatcher();
                GridCommon.GridResizingOffsetTestCommon.ResizeWindow(this.window, TestLog.Current);
            }
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridResizingOffsetTestCommon.result;
        }

    }

    #endregion

    #region Grid Relayout on Property change

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest1", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest1 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest1()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.ColumnDefinitions[0].Width = new GridLength(20, GridUnitType.Star);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest2", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest2 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest2()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest3", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest3 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest3()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.ColumnDefinitions[2].Width = new GridLength(300);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest4", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest4 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest4()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.ColumnDefinitions[3].Width = new GridLength(1, GridUnitType.Auto);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest5", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest5 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest5()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest6", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest6 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest6()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Auto);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest7", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest7 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest7()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.RowDefinitions[2].Height = new GridLength(20, GridUnitType.Star);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest8", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest8 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest8()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.RowDefinitions[3].Height = new GridLength(100);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest9", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest9 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest9()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
            eRoot.ColumnDefinitions[1].Width = new GridLength(200);
            eRoot.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            eRoot.ColumnDefinitions[3].Width = new GridLength(2, GridUnitType.Star);
            eRoot.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Auto);
            eRoot.RowDefinitions[1].Height = new GridLength(200);
            eRoot.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
            eRoot.RowDefinitions[3].Height = new GridLength(2, GridUnitType.Star);

            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest10", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest10 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest10()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            eRoot.ColumnDefinitions[0].Width = new GridLength(1, GridUnitType.Auto);
            eRoot.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Auto);
            eRoot.ColumnDefinitions[2].Width = new GridLength(1, GridUnitType.Star);
            eRoot.ColumnDefinitions[3].Width = new GridLength(1, GridUnitType.Star);
            eRoot.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
            eRoot.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            eRoot.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Auto);
            eRoot.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Auto);

            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest11", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest11 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest11()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            GridCommon.SettingAlignment(eRoot.Children[0], HorizontalAlignment.Left);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest12", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest12 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest12()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            GridCommon.SettingAlignment(eRoot.Children[1], VerticalAlignment.Top);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest13", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest13 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest13()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            GridCommon.SettingAlignment(eRoot.Children[2], HorizontalAlignment.Right);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest14", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest14 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest14()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            GridCommon.SettingAlignment(eRoot.Children[3], VerticalAlignment.Top);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest15", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest15 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest15()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            GridCommon.SettingOffset(eRoot.Children[0], 30, 30, 0, 0);
            GridCommon.SettingAlignment(eRoot.Children[0], HorizontalAlignment.Left, VerticalAlignment.Top);

            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest16", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest16 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest16()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            GridCommon.SettingOffset(eRoot.Children[1], 0, 30, 0, 30);
            GridCommon.SettingAlignment(eRoot.Children[1], VerticalAlignment.Top);

            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest17", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest17 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest17()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            GridCommon.SettingOffset(eRoot.Children[2], 0, 30, 30, 0);
            GridCommon.SettingAlignment(eRoot.Children[2], HorizontalAlignment.Right, VerticalAlignment.Top);

            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest18", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest18 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest18()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            GridCommon.SettingOffset(eRoot.Children[3], 0, 0, 30, 30);
            GridCommon.SettingAlignment(eRoot.Children[3], HorizontalAlignment.Right, VerticalAlignment.Bottom);

            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest19", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest19 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest19()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetColumn(eRoot.Children[0], 1);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest20", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest20 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest20()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetColumn(eRoot.Children[2], 3);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest21", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest21 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest21()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetRow(eRoot.Children[1], 3);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest22", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest22 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest22()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetRow(eRoot.Children[3], 0);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest23", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest23 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest23()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetColumn(eRoot.Children[0], 1);
            Grid.SetRow(eRoot.Children[0], 2);

            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest24", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest24 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest24()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetColumn(eRoot.Children[0], 3);
            Grid.SetRow(eRoot.Children[0], 0);

            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest25", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest25 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest25()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetColumnSpan(eRoot.Children[0], 2);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest26", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest26 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest26()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetColumnSpan(eRoot.Children[3], 5);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest27", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest27 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest27()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetRowSpan(eRoot.Children[1], 3);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest28", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest28 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest28()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetRowSpan(eRoot.Children[2], 4);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest29", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest29 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest29()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetColumnSpan(eRoot.Children[0], 4);
            Grid.SetRowSpan(eRoot.Children[0], 4);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    [Test(1, "Panels.Grid", "GridRelayoutOnPropertyChangeTest30", Variables="Area=ElementLayout")]
    public class GridRelayoutOnPropertyChangeTest30 : CodeTest
    {


        public GridRelayoutOnPropertyChangeTest30()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnPropertyChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            Grid.SetColumnSpan(eRoot.Children[1], 3);
            Grid.SetRowSpan(eRoot.Children[1], 3);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnPropertyChangeTestCommon.OnLayoutUpdatedOccured);
        }

        public override void TestVerify()
        {
            this.Result = (GridCommon.GridRelayoutOnPropertyChangeTestCommon.relayoutOccurred) ? true : false;
        }
    }

    #endregion

    #region Relayout on Content change by adding different element.

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest1", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest1 : CodeTest
    {
        public GridRelayoutOnContentChangeTest1()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Rectangle rect = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.SlateBlue));
            GridCommon.PlacingChild(eRoot, rect, 2, 2, true);

            return eRoot;
        }
        public override void TestActions()
        {
            ((Rectangle)eRoot.Children[0]).Width = 150;
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest2", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest2 : CodeTest
    {


        public GridRelayoutOnContentChangeTest2()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Button btn = CommonFunctionality.CreateButton();
            GridCommon.PlacingChild(eRoot, btn, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ((Button)eRoot.Children[0]).Content = "Button TextBlock Changed~!";
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest3", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest3 : CodeTest
    {


        public GridRelayoutOnContentChangeTest3()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            TextBox tbox = CommonFunctionality.CreateTextBox(150, 100, "Width=150px");
            GridCommon.PlacingChild(eRoot, tbox, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ((TextBox)eRoot.Children[0]).Width = 150;
            ((TextBox)eRoot.Children[0]).Text = "Width changed to 50%";
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest4", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest4 : CodeTest
    {


        public GridRelayoutOnContentChangeTest4()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Ellipse elps = new Ellipse();
            elps.Width = 150;
            elps.Height = 150;
            elps.Fill = new SolidColorBrush(Colors.SlateBlue);
            GridCommon.PlacingChild(eRoot, elps, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ((Ellipse)eRoot.Children[0]).Width = 250;
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest5", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest5 : CodeTest
    {
        public GridRelayoutOnContentChangeTest5()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();
        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Image img = CommonFunctionality.CreateImage("light.bmp");
            GridCommon.PlacingChild(eRoot, img, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ((Image)eRoot.Children[0]).Width = 100;
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest6", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest6 : CodeTest
    {


        public GridRelayoutOnContentChangeTest6()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            TextBlock txt = CommonFunctionality.CreateText("Testing Grid...");
            GridCommon.PlacingChild(eRoot, txt, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ((TextBlock)eRoot.Children[0]).Text = "Changing Text...";
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest7", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest7 : CodeTest
    {


        public GridRelayoutOnContentChangeTest7()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Border b = CommonFunctionality.CreateBorder(new SolidColorBrush(Colors.SlateBlue), double.NaN, double.NaN);
            GridCommon.PlacingChild(eRoot, b, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ((Border)eRoot.Children[0]).BorderThickness = new Thickness(20);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest8", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest8 : CodeTest
    {


        public GridRelayoutOnContentChangeTest8()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Label lbl = new Label();
            lbl.Content = "Testing Grid with Lebel~!";
            GridCommon.PlacingChild(eRoot, lbl, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ((Label)eRoot.Children[0]).Content = "content changed.";
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest9", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest9 : CodeTest
    {


        public GridRelayoutOnContentChangeTest9()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            ListBox lb = CommonFunctionality.CreateListBox(10);
            GridCommon.PlacingChild(eRoot, lb, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ListBoxItem li = new ListBoxItem();
            li.Content = "List Item added with Long Text...~";
            ((ListBox)eRoot.Children[0]).Items.Add(li);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest10", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest10 : CodeTest
    {


        public GridRelayoutOnContentChangeTest10()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Menu menu = CommonFunctionality.CreateMenu(5);
            GridCommon.PlacingChild(eRoot, menu, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            MenuItem mi = new MenuItem();
            mi.Header = "Menu Item Added~!!!";
            ((Menu)eRoot.Children[0]).Items.Add(mi);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest11", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest11 : CodeTest
    {


        public GridRelayoutOnContentChangeTest11()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Canvas canvas = new Canvas();
            canvas.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle cRect = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(cRect, 10);
            Canvas.SetTop(cRect, 10);
            canvas.Children.Add(cRect);
            GridCommon.PlacingChild(eRoot, canvas, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            Rectangle crect = CommonFunctionality.CreateRectangle(40, 40, new SolidColorBrush(Colors.Red));
            Canvas.SetLeft(crect, 30);
            Canvas.SetTop(crect, 30);
            ((Canvas)eRoot.Children[0]).Children.Add(crect);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest12", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest12 : CodeTest
    {


        public GridRelayoutOnContentChangeTest12()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            DockPanel dockpanel = new DockPanel();
            dockpanel.Background = new SolidColorBrush(Colors.SlateBlue);
            dockpanel.LastChildFill = true;
            Rectangle rect0 = CommonFunctionality.CreateRectangle(20, 100, new SolidColorBrush(Colors.Red));
            DockPanel.SetDock(rect0, Dock.Top);
            dockpanel.Children.Add(rect0);
            Rectangle rect1 = CommonFunctionality.CreateRectangle(20, 100, new SolidColorBrush(Colors.Blue));
            DockPanel.SetDock(rect1, Dock.Left);
            dockpanel.Children.Add(rect1);
            Rectangle rect2 = CommonFunctionality.CreateRectangle(double.NaN, double.NaN, new SolidColorBrush(Colors.Yellow));
            //DockPanel.SetDock(rect2, Dock.Fill);
            dockpanel.Children.Add(rect2);
            GridCommon.PlacingChild(eRoot, dockpanel, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            DockPanel.SetDock(((DockPanel)eRoot.Children[0]).Children[0], Dock.Right);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest13", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest13 : CodeTest
    {


        public GridRelayoutOnContentChangeTest13()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            Grid subGrid = GridCommon.CreateGrid(2, 2);
            subGrid.Background = new SolidColorBrush(Colors.SlateBlue);
            Rectangle gRect0 = CommonFunctionality.CreateRectangle(10, 10, new SolidColorBrush(Colors.Red));
            GridCommon.PlacingChild(subGrid, gRect0, 0, 0, true);
            Rectangle gRect1 = CommonFunctionality.CreateRectangle(20, 20, new SolidColorBrush(Colors.Blue));
            GridCommon.PlacingChild(subGrid, gRect1, 1, 0, true);
            Rectangle gRect2 = CommonFunctionality.CreateRectangle(30, 30, new SolidColorBrush(Colors.Yellow));
            GridCommon.PlacingChild(subGrid, gRect2, 0, 1, true);

            GridCommon.PlacingChild(eRoot, subGrid, 2, 2, true);
            return eRoot;
        }
        public override void TestActions()
        {
            ColumnDefinition ccd = new ColumnDefinition();
            ((Grid)eRoot.Children[0]).ColumnDefinitions.Add(ccd);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest14", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest14 : CodeTest
    {


        public GridRelayoutOnContentChangeTest14()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            ColumnDefinition cd = new ColumnDefinition();
            cd.Width = new GridLength(200);
            eRoot.ColumnDefinitions.Add(cd);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    [Test(2, "Panels.Grid", "GridRelayoutOnContentChangeTest15", Variables="Area=ElementLayout")]
    public class GridRelayoutOnContentChangeTest15 : CodeTest
    {


        public GridRelayoutOnContentChangeTest15()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 408;
            this.window.Height = 428;
            this.window.Content = this.TestContent();


        }
        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridRelayoutOnContentChangeTestCommon.CommonContent() as Grid;
            return eRoot;
        }
        public override void TestActions()
        {
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(200);
            eRoot.RowDefinitions.Add(rd);
            eRoot.LayoutUpdated += new EventHandler(GridCommon.GridRelayoutOnContentChangeTestCommon.OnLayoutUpdatedOccured);
        }
        public override void TestVerify()
        {
            this.Result = GridCommon.GridRelayoutOnContentChangeTestCommon.GridRelayoutOnContentChangeTestVerifier(eRoot, TestLog.Current);
        }
    }

    #endregion

    #region Grid Line Visual Test

    [Test(3, "Panels.Grid", "GridLineVisualTest1", Variables="Area=ElementLayout")]
    public class GridLineVisualTest1 : CodeTest
    {

        public GridLineVisualTest1()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 400;
            this.window.WindowStyle = WindowStyle.None;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = new Grid();
            eRoot.Background = Brushes.DarkOrange;
            Border child = new Border();
            child.Height = 100;
            child.Width = 100;
            child.Background = Brushes.CornflowerBlue;
            eRoot.Children.Add(child);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = true;
            Visual parent = eRoot.Children[0] as Visual;
            Rect contentRect;
            Border child = eRoot.Children[0] as Border;

            Helpers.Log("Test One : Default ShowGridLines Value.");
            contentRect = VisualTreeHelper.GetDescendantBounds(parent);

            if (contentRect.Height != child.ActualHeight && contentRect.Width != child.ActualWidth && !(contentRect.TopLeft.X > 0) && !(contentRect.TopLeft.Y > 0))
            {
                Helpers.Log("Content Rect of Grid is NOT equal to child size with default ShowGridLines Value.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Content Rect of Grid is equal to child size with default ShowGridLines Value.");
            }

            eRoot.ShowGridLines = true;

            Helpers.Log("Test Two : ShowGridLines = True.");

            if (eRoot.ShowGridLines == false)
            {
                Helpers.Log("Grid did not make transition from ShowGridLines = False to ShowGridLines = True");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Grid transition from ShowGridLines = False to ShowGridLines = True was successful.");
            }

            eRoot.ShowGridLines = false;
            CommonFunctionality.FlushDispatcher();

            contentRect = VisualTreeHelper.GetDescendantBounds(parent);
            Helpers.Log("Test Three : ShowGridLines = False.");
            if (contentRect.Height != child.ActualHeight && contentRect.Width != child.ActualWidth && !(contentRect.TopLeft.X > 0) && !(contentRect.TopLeft.Y > 0))
            {
                Helpers.Log("Content Rect of Grid is NOT equal to child size with ShowGridLines == False.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Content Rect of Grid is equal to child size with ShowGridLines == False.");
            }
        }
    }

    [Test(3, "Panels.Grid", "GridLineVisualTest2", Variables="Area=ElementLayout")]
    public class GridLineVisualTest2 : CodeTest
    {

        public GridLineVisualTest2()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 400;
            this.window.WindowStyle = WindowStyle.None;
            this.window.Content = this.TestContent();

        }

        public Grid eRoot;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.CreateGrid(2, 2);
            eRoot.Background = Brushes.DarkOrange;
            Border child = new Border();
            child.Height = 100;
            child.Width = 100;
            child.Background = Brushes.CornflowerBlue;
            eRoot.Children.Add(child);
            return eRoot;
        }
        public override void TestVerify()
        {
            this.Result = true;
            Visual parent = eRoot.Children[0] as Visual;
            Rect contentRect;
            Border child = eRoot.Children[0] as Border;

            Helpers.Log("Test One : Default ShowGridLines Value.");
            contentRect = VisualTreeHelper.GetDescendantBounds(parent);

            if (contentRect.Height != child.ActualHeight && contentRect.Width != child.ActualWidth && !(contentRect.TopLeft.X > 0) && !(contentRect.TopLeft.Y > 0))
            {
                Helpers.Log("Content Rect of Grid is NOT equal to child size with default ShowGridLines Value.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Content Rect of Grid is equal to child size with default ShowGridLines Value.");
            }

            eRoot.ShowGridLines = true;
            // CommonFunctionality.FlushDispatcher();

            Helpers.Log("Test Two : ShowGridLines = True.");

            if (eRoot.ShowGridLines == false)
            {
                Helpers.Log("Grid did not make transition from ShowGridLines = False to ShowGridLines = True");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Grid transition from ShowGridLines = False to ShowGridLines = True was successful.");
            }

            eRoot.ShowGridLines = false;
            CommonFunctionality.FlushDispatcher();

            contentRect = VisualTreeHelper.GetDescendantBounds(parent);
            Helpers.Log("Test Three : ShowGridLines = False.");
            if (contentRect.Height != child.ActualHeight && contentRect.Width != child.ActualWidth && !(contentRect.TopLeft.X > 0) && !(contentRect.TopLeft.Y > 0))
            {
                Helpers.Log("Content Rect of Grid is NOT equal to child size with ShowGridLines == False.");
                this.Result = false;
            }
            else
            {
                Helpers.Log("Content Rect of Grid is equal to child size with ShowGridLines == False.");
            }
        }
    }

    #endregion

    #region Definition Collection

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest01", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest01 : CodeTest
    {

        public GridDefinitionCollectionTest01()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            ColumnDefinition coldef = new ColumnDefinition();
            coldef.Name = "cdAdded";
            _grid.ColumnDefinitions.Add(coldef);
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForColumn(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForColumn(_grid);
            expected.Add("cdAdded");
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest02", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest02 : CodeTest
    {

        public GridDefinitionCollectionTest02()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            RowDefinition rowdef = new RowDefinition();
            rowdef.Name = "rdAdded";
            _grid.RowDefinitions.Add(rowdef);
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForRow(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForRow(_grid);
            expected.Add("rdAdded");
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest03", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest03 : CodeTest
    {

        public GridDefinitionCollectionTest03()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            ColumnDefinition coldef = new ColumnDefinition();
            coldef.Name = "cdInserted";
            _grid.ColumnDefinitions.Insert(1, coldef);
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForColumn(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForColumn(_grid);
            expected.Insert(1, "cdInserted");
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest04", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest04 : CodeTest
    {

        public GridDefinitionCollectionTest04()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            RowDefinition rowdef = new RowDefinition();
            rowdef.Name = "rdInserted";
            _grid.RowDefinitions.Insert(1, rowdef);
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForRow(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForRow(_grid);
            expected.Insert(1, "rdInserted");
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest05", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest05 : CodeTest
    {

        public GridDefinitionCollectionTest05()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            ColumnDefinition cd = _grid.ColumnDefinitions[1];
            _grid.ColumnDefinitions.Remove(cd);
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForColumn(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForColumn(_grid);
            expected.Remove("cd1");
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest06", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest06 : CodeTest
    {

        public GridDefinitionCollectionTest06()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            RowDefinition rd = _grid.RowDefinitions[1];
            _grid.RowDefinitions.Remove(rd);
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForRow(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForRow(_grid);
            expected.Remove("rd1");
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest07", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest07 : CodeTest
    {

        public GridDefinitionCollectionTest07()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            try
            {
                _grid.ColumnDefinitions.RemoveAt(3);
            }
            catch (ArgumentOutOfRangeException a)
            {
                GlobalLog.LogEvidence(a);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForColumn(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForColumn(_grid);
            expected.RemoveAt(3);
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest08", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest08 : CodeTest
    {

        public GridDefinitionCollectionTest08()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            try
            {
                _grid.RowDefinitions.RemoveAt(3);
            }
            catch (ArgumentOutOfRangeException a)
            {
                GlobalLog.LogEvidence(a);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForRow(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForRow(_grid);
            expected.RemoveAt(3);
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest09", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest09 : CodeTest
    {

        public GridDefinitionCollectionTest09()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            try
            {
                _grid.ColumnDefinitions.RemoveRange(1, 3);
            }
            catch (ArgumentOutOfRangeException a1)
            {
                GlobalLog.LogEvidence(a1);
            }
            catch (ArgumentException a2)
            {
                GlobalLog.LogEvidence(a2);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForColumn(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForColumn(_grid);
            for (int rem = 1; rem < 4; rem++)
            {
                expected.RemoveAt(1);
            }
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest10", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest10 : CodeTest
    {

        public GridDefinitionCollectionTest10()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            try
            {
                _grid.RowDefinitions.RemoveRange(1, 3);
            }
            catch (ArgumentOutOfRangeException a)
            {
                GlobalLog.LogEvidence(a);
            }
            catch (ArgumentException a2)
            {
                GlobalLog.LogEvidence(a2);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForRow(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForRow(_grid);
            for (int rem = 1; rem < 4; rem++)
            {
                expected.RemoveAt(1);
            }
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest11", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest11 : CodeTest
    {

        public GridDefinitionCollectionTest11()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            _grid.ColumnDefinitions.Clear();
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForColumn(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForColumn(_grid);
            expected.Clear();
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest12", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest12 : CodeTest
    {

        public GridDefinitionCollectionTest12()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            _grid.RowDefinitions.Clear();
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForRow(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForRow(_grid);
            expected.Clear();
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest13", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest13 : CodeTest
    {

        public GridDefinitionCollectionTest13()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public ColumnDefinition coldef;
        public override void TestActions()
        {
            coldef = new ColumnDefinition();
            _grid.ColumnDefinitions.Add(coldef);
        }
        public override void TestVerify()
        {
            Helpers.Log("ColumnDefinitions.Contains returns " + _grid.ColumnDefinitions.Contains(coldef).ToString());
            this.Result = _grid.ColumnDefinitions.Contains(coldef) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest14", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest14 : CodeTest
    {

        public GridDefinitionCollectionTest14()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public ColumnDefinition coldef;
        public override void TestActions()
        {
            ColumnDefinition coldef = new ColumnDefinition();
        }
        public override void TestVerify()
        {
            Helpers.Log("ColumnDefinitions.Contains returns " + _grid.ColumnDefinitions.Contains(coldef).ToString());
            this.Result = (_grid.ColumnDefinitions.Contains(coldef)) ? false : true;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest15", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest15 : CodeTest
    {

        public GridDefinitionCollectionTest15()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public RowDefinition rowdef;
        public override void TestActions()
        {
            rowdef = new RowDefinition();
            _grid.RowDefinitions.Add(rowdef);
        }
        public override void TestVerify()
        {
            Helpers.Log("RowDefinitions.Contains returns " + _grid.RowDefinitions.Contains(rowdef).ToString());
            this.Result = _grid.RowDefinitions.Contains(rowdef) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest16", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest16 : CodeTest
    {

        public GridDefinitionCollectionTest16()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public RowDefinition rowdef;
        public override void TestActions()
        {
            rowdef = new RowDefinition();
        }
        public override void TestVerify()
        {
            Helpers.Log("RowDefinitions.Contains returns " + _grid.RowDefinitions.Contains(rowdef).ToString());
            this.Result = (_grid.RowDefinitions.Contains(rowdef)) ? false : true;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest17", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest17 : CodeTest
    {

        public GridDefinitionCollectionTest17()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public ColumnDefinition coldef;
        public override void TestActions()
        {
            coldef = new ColumnDefinition();
            _grid.ColumnDefinitions.Insert(2, coldef);
        }
        public override void TestVerify()
        {
            Helpers.Log("ColumnDefinitions.IndexOf returns " + _grid.ColumnDefinitions.IndexOf(coldef).ToString());
            this.Result = (_grid.ColumnDefinitions.IndexOf(coldef) == 2) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest18", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest18 : CodeTest
    {

        public GridDefinitionCollectionTest18()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public ColumnDefinition coldef;
        public override void TestActions()
        {
            ColumnDefinition coldef = new ColumnDefinition();
        }
        public override void TestVerify()
        {
            Helpers.Log("ColumnDefinitions.IndexOf returns " + _grid.ColumnDefinitions.IndexOf(coldef).ToString());
            this.Result = (_grid.ColumnDefinitions.IndexOf(coldef) == -1) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest19", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest19 : CodeTest
    {

        public GridDefinitionCollectionTest19()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public RowDefinition rowdef;
        public override void TestActions()
        {
            rowdef = new RowDefinition();
            _grid.RowDefinitions.Insert(2, rowdef);
        }
        public override void TestVerify()
        {
            Helpers.Log("RowDefinitions.IndexOf returns " + _grid.RowDefinitions.IndexOf(rowdef).ToString());
            this.Result = (_grid.RowDefinitions.IndexOf(rowdef) == 2) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest20", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest20 : CodeTest
    {

        public GridDefinitionCollectionTest20()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public RowDefinition rowdef;
        public override void TestActions()
        {
            RowDefinition rowdef = new RowDefinition();
        }
        public override void TestVerify()
        {
            Helpers.Log("RowDefinitions.IndexOf returns " + _grid.RowDefinitions.IndexOf(rowdef).ToString());
            this.Result = (_grid.RowDefinitions.IndexOf(rowdef) == -1) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest21", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest21 : CodeTest
    {

        public GridDefinitionCollectionTest21()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            ColumnDefinition[] colArray = new ColumnDefinition[_grid.ColumnDefinitions.Count];
            _grid.ColumnDefinitions.CopyTo(colArray, 0);
            int cCnt = _grid.ColumnDefinitions.Count;
            _grid.ColumnDefinitions.Clear();
            for (int i = 0; i < cCnt; i++)
            {
                _grid.ColumnDefinitions.Add(colArray[i]);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForColumn(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForColumn(_grid);
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridDefinitionCollectionTest22", Variables="Area=ElementLayout")]
    public class GridDefinitionCollectionTest22 : CodeTest
    {

        public GridDefinitionCollectionTest22()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridDefinitionCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            RowDefinition[] rowArray = new RowDefinition[_grid.RowDefinitions.Count];
            _grid.RowDefinitions.CopyTo(rowArray, 0);
            int rCnt = _grid.RowDefinitions.Count;
            _grid.RowDefinitions.Clear();
            for (int i = 0; i < rCnt; i++)
            {
                _grid.RowDefinitions.Add(rowArray[i]);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridDefinitionCollectionTestCommon.GetActualIDsForRow(_grid);
            ArrayList expected = GridCommon.GridDefinitionCollectionTestCommon.GetExpectedIDsForRow(_grid);
            Helpers.Log("Expected:" + GridCommon.GridDefinitionCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridDefinitionCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridDefinitionCollectionTestCommon.GetList(expected), GridCommon.GridDefinitionCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    #endregion

    #region Grid Children Collection

    [Test(2, "Panels.Grid", "GridChildrenCollectionTest1", Variables="Area=ElementLayout")]
    public class GridChildrenCollectionTest1 : CodeTest
    {

        public GridChildrenCollectionTest1()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridChildrenCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            Rectangle rectAdd = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.Green));
            rectAdd.Name = "RectAdd";
            _grid.Children.Add(rectAdd);
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridChildrenCollectionTestCommon.GetActualIDsForChildren(_grid);
            ArrayList expected = GridCommon.GridChildrenCollectionTestCommon.GetExpectedIDsForChildren(_grid);
            expected.Add("RectAdd");
            Helpers.Log("Expected:" + GridCommon.GridChildrenCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridChildrenCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridChildrenCollectionTestCommon.GetList(expected), GridCommon.GridChildrenCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridChildrenCollectionTest2", Variables="Area=ElementLayout")]
    public class GridChildrenCollectionTest2 : CodeTest
    {

        public GridChildrenCollectionTest2()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridChildrenCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            Rectangle rectInsert = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.Green));
            rectInsert.Name = "RectInsert";
            _grid.Children.Insert(2, rectInsert);
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridChildrenCollectionTestCommon.GetActualIDsForChildren(_grid);
            ArrayList expected = GridCommon.GridChildrenCollectionTestCommon.GetExpectedIDsForChildren(_grid);
            expected.Insert(2, "RectInsert");
            Helpers.Log("Expected:" + GridCommon.GridChildrenCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridChildrenCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridChildrenCollectionTestCommon.GetList(expected), GridCommon.GridChildrenCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridChildrenCollectionTest3", Variables="Area=ElementLayout")]
    public class GridChildrenCollectionTest3 : CodeTest
    {

        public GridChildrenCollectionTest3()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridChildrenCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            try
            {
                _grid.Children.RemoveAt(3);
            }
            catch (ArgumentOutOfRangeException a)
            {
                GlobalLog.LogEvidence(a);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridChildrenCollectionTestCommon.GetActualIDsForChildren(_grid);
            ArrayList expected = GridCommon.GridChildrenCollectionTestCommon.GetExpectedIDsForChildren(_grid);
            expected.RemoveAt(3);
            Helpers.Log("Expected:" + GridCommon.GridChildrenCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridChildrenCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridChildrenCollectionTestCommon.GetList(expected), GridCommon.GridChildrenCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridChildrenCollectionTest4", Variables="Area=ElementLayout")]
    public class GridChildrenCollectionTest4 : CodeTest
    {

        public GridChildrenCollectionTest4()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridChildrenCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            try
            {
                _grid.Children.RemoveRange(2, 2);
            }
            catch (ArgumentOutOfRangeException a)
            {
                GlobalLog.LogEvidence(a);
            }
            catch (ArgumentException a1)
            {
                GlobalLog.LogEvidence(a1);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridChildrenCollectionTestCommon.GetActualIDsForChildren(_grid);
            ArrayList expected = GridCommon.GridChildrenCollectionTestCommon.GetExpectedIDsForChildren(_grid);
            for (int rem = 2; rem < 4; rem++)
            {
                expected.RemoveAt(2);
            }
            Helpers.Log("Expected:" + GridCommon.GridChildrenCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridChildrenCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridChildrenCollectionTestCommon.GetList(expected), GridCommon.GridChildrenCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridChildrenCollectionTest5", Variables="Area=ElementLayout")]
    public class GridChildrenCollectionTest5 : CodeTest
    {

        public GridChildrenCollectionTest5()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridChildrenCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            _grid.Children.Clear();
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridChildrenCollectionTestCommon.GetActualIDsForChildren(_grid);
            ArrayList expected = GridCommon.GridChildrenCollectionTestCommon.GetExpectedIDsForChildren(_grid);
            expected.Clear();
            Helpers.Log("Expected:" + GridCommon.GridChildrenCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridChildrenCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridChildrenCollectionTestCommon.GetList(expected), GridCommon.GridChildrenCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    [Test(2, "Panels.Grid", "GridChildrenCollectionTest6", Variables="Area=ElementLayout")]
    public class GridChildrenCollectionTest6 : CodeTest
    {

        public GridChildrenCollectionTest6()
        {
        }
        public override void WindowSetup()
        {
            this.window.Width = 400;
            this.window.Height = 300;
            this.window.Content = this.TestContent();

        }

        public DockPanel eRoot;
        private Grid _grid;
        public override FrameworkElement TestContent()
        {
            eRoot = GridCommon.GridChildrenCollectionTestCommon.CommonContent() as DockPanel;
            _grid = LayoutUtility.GetChildFromVisualTree(eRoot, typeof(Grid)) as Grid;
            return eRoot;
        }

        public override void TestActions()
        {
            int cnt = _grid.Children.Count + 1;
            Rectangle[] rectArray = new Rectangle[cnt];
            rectArray[0] = CommonFunctionality.CreateRectangle(50, 50, new SolidColorBrush(Colors.Green));
            rectArray[0].Name = "Rect00";
            Grid.SetColumn(rectArray[0], 1);
            _grid.Children.CopyTo(rectArray, 1);
            _grid.Children.Clear();
            for (int r = 0; r < cnt; r++)
            {
                _grid.Children.Add(rectArray[r]);
            }
        }
        public override void TestVerify()
        {
            ArrayList actual = GridCommon.GridChildrenCollectionTestCommon.GetActualIDsForChildren(_grid);
            ArrayList expected = GridCommon.GridChildrenCollectionTestCommon.GetExpectedIDsForChildren(_grid);
            expected.Insert(0, "Rect00");
            Helpers.Log("Expected:" + GridCommon.GridChildrenCollectionTestCommon.GetList(expected));
            Helpers.Log("Actual:  " + GridCommon.GridChildrenCollectionTestCommon.GetList(actual));
            this.Result = (Equals(GridCommon.GridChildrenCollectionTestCommon.GetList(expected), GridCommon.GridChildrenCollectionTestCommon.GetList(actual))) ? true : false;
        }
    }

    #endregion

    #region Star Distribution Tests

    [Test(0, "Panels.Grid", "StarDistribution0a", Versions="4.7+", TestParameters="Scenario=MinResolvesLate")]
    [Test(0, "Panels.Grid", "StarDistribution0b", Versions="4.7+", TestParameters="Scenario=MinResolvesLate, Scale=1.25")]
    [Test(0, "Panels.Grid", "StarDistribution0c", Versions="4.7+", TestParameters="Scenario=MinResolvesLate, Scale=1.5324")]
    [Test(0, "Panels.Grid", "StarDistribution1a", Versions="4.7+", TestParameters="Scenario=MinProportion")]
    [Test(0, "Panels.Grid", "StarDistribution2a", Versions="4.7+", TestParameters="Scenario=MinMaxA")]
    [Test(0, "Panels.Grid", "StarDistribution2b", Versions="4.7+", TestParameters="Scenario=MinMaxB")]
    //[Test(0, "Panels.Grid", "StarDistribution3a", Versions="4.7+", TestParameters="Scenario=InfiniteHuge")]
    [Test(0, "Panels.Grid", "StarDistribution4a", Versions="4.7+", TestParameters="Scenario=HugeLarge")]
    [Test(0, "Panels.Grid", "StarDistribution5a", Versions="4.7+", TestParameters="Scenario=Overflow")]
    [Test(0, "Panels.Grid", "StarDistribution6a", Versions="4.7+", TestParameters="Scenario=TinyProportion")]
    [Test(0, "Panels.Grid", "StarDistribution6b", Versions="4.7+", TestParameters="Scenario=TinyProportion, Scale=3.0")]
    [Test(0, "Panels.Grid", "StarDistribution7a", Versions="4.7+", TestParameters="Scenario=Cancellation")]
    [Test(0, "Panels.Grid", "StarDistribution7b", Versions="4.7+", TestParameters="Scenario=Cancellation, Scale=1.5")]
    [Test(0, "Panels.Grid", "StarDistribution8a", Versions="4.7.1+", TestParameters="Scenario=MultipleMinMax")]
    [Test(0, "Panels.Grid", "StarDistribution9a", Versions="4.8+", TestParameters="Scenario=MaxExceedsAvailable")]
    [Test(0, "Panels.Grid", "StarDistribution10a", Versions="4.8+", TestParameters="Scenario=UnconstrainedSize")]
    [Test(0, "Panels.Grid", "StarDistribution11a", Versions="4.8+", TestParameters="Scenario=MinFromSpan")]
    [Test(0, "Panels.Grid", "StarDistribution11b", Versions="4.8+", TestParameters="Scenario=MinFromSpan, Scale=1.25")]
    public class StarDistributionTest : CodeTest
    {
        Scenario _scenario;
        double _scale;
        Grid _grid;
        double[] _initialWidths;

        public StarDistributionTest()
        {
            this.Result = true;     // change to false if we find a problem

            // choose the scenario
            string name = DriverState.DriverParameters["Scenario"];
            foreach (Scenario s in s_scenarios)
            {
                if (s.Name == name)
                {
                    _scenario = s;
                    break;
                }
            }

            if (_scenario == null)
            {
                Helpers.Log(String.Format("Unknown scenario name '{0}'", name));
                this.Result = false;
            }

            // simulate different DPI
            _scale = 1.0;
            string scaleArg = DriverState.DriverParameters["Scale"];
            if (!String.IsNullOrEmpty(scaleArg))
            {
                if (!Double.TryParse(scaleArg, out _scale) || _scale <= 0.0)
                {
                    Helpers.Log(String.Format("Bad scale '{0}'", scaleArg));
                    this.Result = false;
                }
            }
        }

        public override void WindowSetup()
        {
            this.window.Width = 800;
            this.window.Height = 300;
            this.window.Content = this.TestContent();
        }

        public override FrameworkElement TestContent()
        {
            FrameworkElement content;

            _grid = new Grid() { ShowGridLines = true };

            if (_scenario.Width > 0)
            {
                // normal case
                content = _grid;
                _grid.Width = _scenario.Width * _scale;
            }
            else
            {
                // unconstrained case
                StackPanel panel = new StackPanel() { Orientation=Orientation.Horizontal };
                panel.Children.Add(_grid);
                content = panel;
            }

            for (int i=0; i<_scenario.ColumnDefinitions.Length; ++i)
            {
                ColumnDefinition def = new ColumnDefinition() {
                    Width = _scenario.ColumnDefinitions[i].Width,
                    MinWidth = _scenario.ColumnDefinitions[i].MinWidth * _scale,
                    MaxWidth = _scenario.ColumnDefinitions[i].MaxWidth * _scale
                    };
                if (def.Width.IsAbsolute)
                {
                    def.Width = new GridLength(def.Width.Value * _scale, def.Width.GridUnitType);
                }

                _grid.ColumnDefinitions.Add(def);

                FrameworkElement child;
                ContentType contentType = (_scenario.ColumnContents != null && i < _scenario.ColumnContents.Length)
                    ? _scenario.ColumnContents[i] : ContentType.Border;
                switch (contentType)
                {
                    case ContentType.Border:    child = NewBorder(i);       break;
                    case ContentType.Viewbox:   child = NewViewbox();       break;
                    case ContentType.TextBlock: child = NewTextBlock();     break;
                    case ContentType.Span3:     child = NewSpan3(_grid.Width); break;
                    default:                    child = null;               break;
                }

                if (child != null)
                {
                    Grid.SetColumn(child, i);
                    _grid.Children.Add(child);
                }
            }

            return content;
        }

        public override void TestActions()
        {
            double[] widths = GridCommon.GettingActualColumnWidth(_grid);
            VerifyColumnWidths(widths);

            if (_scenario.DeltaW != 0.0)
            {
                // change the grid width - the column widths should change continuously
                _initialWidths = widths;
                _grid.Width += _scenario.DeltaW * _scale;
            }

            _grid.UseLayoutRounding = true; // excercise the rounding code
        }

        public override void TestVerify()
        {
            double[] widths = GridCommon.GettingActualColumnWidth(_grid);
            VerifyColumnWidths(widths);

            if (_initialWidths != null)
            {
                double delta = _scenario.DeltaW * _scale;
                for (int i=0; i<widths.Length; ++i)
                {
                    if (Math.Abs((_initialWidths[i] - widths[i]) / delta) > 1.5)
                    {
                        Helpers.Log(String.Format("Changing Grid.Width by {0} changed column {1} by {2}",
                            delta, i, (_initialWidths[i] - widths[i])));
                        this.Result = false;
                    }
                }
            }

            switch (_scenario.Name)
            {
                case "MinFromSpan":
                    VerifyMinFromSpan();
                    break;
            }
        }

        void VerifyColumnWidths(double[] widths)
        {
            double totalWidth=0.0, totalMinWidth=0.0, totalMaxWidth=0.0;
            int prevStarIndex = -1;

            for (int i=0; i<widths.Length; ++i)
            {
                double width = widths[i];
                double minWidth = _grid.ColumnDefinitions[i].MinWidth;
                double maxWidth = _grid.ColumnDefinitions[i].MaxWidth;

                // columns receive at least their content's width
                for (int j=Math.Min(_grid.Children.Count - 1, i); j>=0; --j)
                {
                    UIElement child = _grid.Children[j];
                    if (Grid.GetColumn(child) == i)
                    {
                        // we only use Border, Viewbox, or TextBlock content.
                        // the first two have natural width = 0 (no margins, border width, etc.)
                        TextBlock tb = child as TextBlock;
                        if (tb != null)
                        {
                            minWidth = Math.Max(minWidth, tb.ActualWidth);
                        }
                        break;      // we create at most one child per column
                    }
                }

                // pixel columns receive at least the declared width
                if (_grid.ColumnDefinitions[i].Width.IsAbsolute)
                {
                    minWidth = Math.Max(minWidth, _grid.ColumnDefinitions[i].Width.Value);
                }

                // max is always at least min
                maxWidth = Math.Max(minWidth, maxWidth);

                // accumulate totals
                totalWidth += width;
                totalMinWidth += minWidth;
                totalMaxWidth += maxWidth;

                // *-columns that aren't allocated min or max should be proportional
                if (_grid.ColumnDefinitions[i].Width.IsStar &&
                    !GreaterOrClose(minWidth, width) && !GreaterOrClose(width, maxWidth))
                {
                    double weight = _grid.ColumnDefinitions[i].Width.Value;
                    if (weight == 0.0)
                    {
                        if (!AreClose(width, 0.0))
                        {
                            Helpers.Log(String.Format("*-column {0} with zero weight has nonzero width {1}",
                                i, width));
                            this.Result = false;
                        }
                    }
                    else
                    {
                        if (prevStarIndex >= 0)
                        {
                            double prevWidth = _grid.ColumnDefinitions[prevStarIndex].ActualWidth;
                            double prevWeight = _grid.ColumnDefinitions[prevStarIndex].Width.Value;
                            if (!AreProportional(prevWidth, prevWeight, width, weight))
                            {
                                Helpers.Log(String.Format("*-columns {0} and {1} are not proportional",
                                    prevStarIndex, i));
                                Helpers.Log(String.Format("  {0}/{1} : {2}/{3}",
                                    prevWidth, prevWeight, width, weight));
                                this.Result = false;
                            }
                        }
                        prevStarIndex = i;
                    }
                }
            }

            // the total width should agree with grid's width, unless the min/max
            // constraints make that impossible
            double gridWidth = (_grid.Width > 0.0) ? _grid.Width : _grid.ActualWidth;
            if (!(_grid.Width > 0.0) ||
                (!GreaterOrClose(totalMinWidth, gridWidth) &&
                 !GreaterOrClose(gridWidth, totalMaxWidth)))
            {
                if (!AreClose(totalWidth, gridWidth))
                {
                    Helpers.Log(String.Format("Column widths don't sum to grid width.  Widths: {0}  Grid width: {1}",
                        totalWidth, gridWidth));
                    this.Result = false;
                }
            }

        }

        // A regression bug reported a hang during Arrange, due to rounding issues
        // when min-sizes are imputed from content that spans columns.
        // The hang occurs when layout rounding is enabled, for certain sizes:
        // e.g. 55, 115, 393, 453, 1715 when running at 120 dpi (=125%).
        // It doesn't happen at all at 96 dpi.  The MinFromSpan scenario includes one
        // of the failing cases already.  To catch regressions, we'll try all sizes
        // from 50 to 500.   (To repeat, this test passes at 96 dpi, but fails
        // at 120 dpi.)
        void VerifyMinFromSpan()
        {
            // locate the child that spans columns
            FrameworkElement child = null;
            foreach (UIElement uie in _grid.Children)
            {
                if (Grid.GetColumnSpan(uie) > 1)
                {
                    child = uie as FrameworkElement;
                    break;
                }
            }

            if (child != null)
            {
                _grid.UseLayoutRounding = true;

                for (double width = 50.0; width < 500; width = width + 1.0)
                {
                    _grid.Width = width;
                    child.Width = width;

                    // wait for layout.  This shouldn't hang.
                    CommonFunctionality.FlushDispatcher();
                }
            }
            else
            {
                Helpers.Log("Cannot locate child that spans columns");
                this.Result = false;
            }
        }

        const double Slack = 1.0;   // to account for layout rounding and FP drift

        static bool AreClose(double x, double y)
        {
            return Math.Abs(x - y) <= Slack;
        }

        static bool GreaterOrClose(double x, double y)
        {
            return (x > y) || AreClose(x, y);
        }

        // verify that width1/weight1 == width2/weight2
        // but allow for slack, and avoid floating-point issues
        static bool AreProportional(double width1, double weight1, double width2, double weight2)
        {
            // special case for infinite weights
            if (Double.IsPositiveInfinity(weight1))
            {
                if (Double.IsPositiveInfinity(weight2))
                {
                    weight1 = weight2 = 1.0;
                }
                else
                {
                    return !AreClose(width2, 0.0);
                }
            }
            else if (Double.IsPositiveInfinity(weight2))
            {
                return !AreClose(width1, 0.0);
            }

            // caller guarantees that neither weight is zero,
            // and special case guarantees that neither is infinite,
            // so it's now safe to divide by weight.
            return ! ( (width1+Slack)/weight1 < (width2-Slack)/weight2 ||
                       (width2+Slack)/weight2 < (width1-Slack)/weight1 );
        }

        static FrameworkElement NewBorder(int i)
        {
            return new Border() { Background = s_backgrounds[i % s_backgrounds.Length] };
        }

        static FrameworkElement NewViewbox()
        {
            Viewbox viewbox = new Viewbox() { Stretch = Stretch.Uniform };
            viewbox.Child = NewTextBlock();
            return viewbox;
        }

        static FrameworkElement NewTextBlock()
        {
            return new TextBlock() { Text = "Test" };
        }

        static FrameworkElement NewSpan3(double width)
        {
            Rectangle rect = new Rectangle() { Width = width, Height = 20 };
            Grid.SetColumnSpan(rect, 3);
            return rect;
        }

        static Brush[] s_backgrounds = new Brush[]
        {
            Brushes.LightGreen,
            Brushes.Salmon,
            Brushes.LightBlue,
        };

        static Scenario[] s_scenarios = new Scenario[]
        {
            // Col0 resolved as min, col2 should get less
            new Scenario("MinResolvesLate")
            {
                Width = 505,  DeltaW = -10,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(1000, GridUnitType.Star),
                                            MinWidth = 250 },
                    new ColumnDefinition { Width = new GridLength(3) },
                    new ColumnDefinition { Width = new GridLength(1001, GridUnitType.Star) },
                },
                ColumnContents = new ContentType[]
                {
                    ContentType.Border,
                    ContentType.None,
                    ContentType.Viewbox,
                },
            },

            // col2 resolved as min, other columns should still be proportional
            new Scenario("MinProportion")
            {
                Width = 350,  DeltaW = 10,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star),
                                            MinWidth = 150 },
                    new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) },
                },
            },

            // some algorithms have a discontinuity near 375
            new Scenario("MinMaxA")
            {
                Width = 376,  DeltaW = -2,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star),
                                            MinWidth = 50 },
                    new ColumnDefinition { Width = new GridLength(80, GridUnitType.Star),
                                            MaxWidth = 100 },
                    new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star) },
                },
            },

            // some algorithms have a discontinuity near 188
            new Scenario("MinMaxB")
            {
                Width = 189,  DeltaW = -2,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(20, GridUnitType.Star),
                                            MinWidth = 50 },
                    new ColumnDefinition { Width = new GridLength(80, GridUnitType.Star),
                                            MaxWidth = 100 },
                    new ColumnDefinition { Width = new GridLength(50, GridUnitType.Star) },
                },
            },

            /* Uncomment when Grid supports infinite *-weight
            // infinite columns should get all the space, even compared to huge
            new Scenario("InfiniteHuge")
            {
                Width = 600,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(Double.PositiveInfinity, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1e+297, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(Double.PositiveInfinity, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1e+299, GridUnitType.Star) },
                },
            },
            */

            // proportions should be honored, even between large weights
            new Scenario("HugeLarge")
            {
                Width = 600,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(1e+297, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1e+298, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1e+299, GridUnitType.Star) },
                },
            },

            // sum of weights exceeds MaxValue
            new Scenario("Overflow")
            {
                Width = 600,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(Double.MaxValue * 0.2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(Double.MaxValue * 0.4, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(Double.MaxValue * 0.6, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(Double.MaxValue * 0.8, GridUnitType.Star) },
                },
            },

            // from DRTLayout/GridFpOverlow.  Col1 resolved as max, 0 and 2 should get more.
            // Some algorithms have discontinuity near 205
            new Scenario("TinyProportion")
            {
                Width = 204,  DeltaW = 1,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(4e-5, GridUnitType.Star),
                                            MinWidth = 52 },
                    new ColumnDefinition { Width = new GridLength(Double.MaxValue, GridUnitType.Star),
                                            MinWidth = 56, MaxWidth = 153 },
                    new ColumnDefinition { Width = new GridLength(8e-5, GridUnitType.Star),
                                            MinWidth = 48 },
                },
            },

            // col2 has most of the weight, could lead to catastrophic cancellation
            new Scenario("Cancellation")
            {
                Width = 600,  DeltaW = 1,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(4e-5, GridUnitType.Star),
                                            MinWidth = 52 },
                    new ColumnDefinition { Width = new GridLength(Double.MaxValue, GridUnitType.Star),
                                            MinWidth = 56, MaxWidth = 153 },
                    new ColumnDefinition { Width = new GridLength(8e-5, GridUnitType.Star),
                                            MinWidth = 48 },
                },
            },

            // multiple *-columns with both Min and Max 
            new Scenario("MultipleMinMax")
            {
                Width = 310,  DeltaW = -70,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(100) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star),
                                            MinWidth = 50, MaxWidth = 100 },
                    new ColumnDefinition { Width = new GridLength(100) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star),
                                            MinWidth = 50, MaxWidth = 100 },
                },
            },

            // *-column with max exceeds available space 
            new Scenario("MaxExceedsAvailable")
            {
                Width = 325,  DeltaW = -10,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(100) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star),
                                            MaxWidth = 20 },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star),
                                            MinWidth = 200 },
                },
            },

            // grid size is unconstrained, some *-columns are empty 
            new Scenario("UnconstrainedSize")
            {
                Width = 0,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                ColumnContents = new ContentType[]
                {
                    ContentType.TextBlock,
                    ContentType.None,
                    ContentType.TextBlock,
                },
            },

            // min sizes come from content that spans columns 
            new Scenario("MinFromSpan")
            {
                Width = 115,
                ColumnDefinitions = new ColumnDefinition[]
                {
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(6, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                },
                ColumnContents = new ContentType[]
                {
                    ContentType.Span3,
                },
            },
        };

        enum ContentType { None, Border, Viewbox, TextBlock, Span3 };

        class Scenario
        {
            public string Name { get; private set; }
            public double Width { get; set; }
            public double DeltaW { get; set; }
            public ColumnDefinition[] ColumnDefinitions { get; set; }
            public ContentType[] ColumnContents { get; set; }

            public Scenario(string name) { Name = name; }
        }
    }


    // [DISABLED_WHILE_PORTING]
    [Test(0, "Panels.Grid", "GridStarColumnResize", Variables="Area=ElementLayout", Timeout = 20, Disabled = true)]
    public class GridStarColumnResize : CodeTest
    {
        public GridStarColumnResize()
        { }

        public override void WindowSetup()
        {
            this.window.Width = 1102;
            this.window.Height = 714;
            FileStream f = new FileStream("GridStarColumnResize.xaml", FileMode.Open, FileAccess.Read);
            this.window.Content = (FrameworkElement)XamlReader.Load(f);
            f.Close();
        }

        // GitHub 5231 reported a hang during column(with star width) resize, due to calculation 
        // of width of *-columns produces a result that's less than the available width by a tiny amount, 
        // due to floating-point drift. This causes a loop to continue when it shouldn't, leading to
        // infinite loop.
        // This Test verification include a failing test case to catch the regresion.
        public override void TestVerify()
        {
            Grid root = (Grid)this.window.Content;
            ColumnDefinition coldef = root.ColumnDefinitions[15];
            int[] TestMatrix = new int[] { 3, 2, 3, 2, 4, 5, 6 };
            for(int i =0; i <TestMatrix.Length; i++)
            {
                coldef.Width = new GridLength(TestMatrix[i], GridUnitType.Star);
                CommonFunctionality.FlushDispatcher();
            }
            this.Result = true;
        }
    }
    #endregion Star Distribution Tests

}
