// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Automation;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using MS.Internal; // PointUtil

namespace DRT
{
    public class GridSplitterSuite : DrtTestSuite
    {
        public GridSplitterSuite() : base("GridSplitter")
        {
            Contact = "Microsoft";
        }

        Grid _rootGrid;

        // Resize columns
        Grid _columnSplitGrid;
        GridSplitter _column0Left;
        GridSplitter _column0Right;
        GridSplitter _column2Left;
        GridSplitter _column3Stretch;
        GridSplitter _column5Center;
        GridSplitter _column6Right;
        
        // Resize rows
        Grid _rowSplitGrid;
        GridSplitter _row0Top;
        GridSplitter _row0Bottom;
        GridSplitter _row2Top;
        GridSplitter _row3Stretch;
        GridSplitter _row5Center;
        GridSplitter _row6Bottom;

        // Other GridSplitters
        GridSplitter _canvasGridSplitter;
        GridSplitter _defaultGridSplitter;


        #region Window setup and common hooks

        public override DrtTest[] PrepareTests()
        {
            Keyboard.Focus(null);

            DRT.LoadXamlFile("DrtGridSplitter.xaml");
            DRT.Show(DRT.RootElement);

            _rootGrid = DRT.FindElementByID("RootGrid") as Grid;

            _columnSplitGrid = DRT.FindElementByID("ColumnSplitGrid") as Grid;
            _column0Left = DRT.FindElementByID("Column0Left") as GridSplitter;
            _column0Right = DRT.FindElementByID("Column0Right") as GridSplitter;
            _column2Left = DRT.FindElementByID("Column2Left") as GridSplitter;
            _column3Stretch = DRT.FindElementByID("Column3Stretch") as GridSplitter;
            _column5Center = DRT.FindElementByID("Column5Center") as GridSplitter;
            _column6Right = DRT.FindElementByID("Column6Right") as GridSplitter;

            // Resize rows
            _rowSplitGrid = DRT.FindElementByID("RowSplitGrid") as Grid;
            _row0Top = DRT.FindElementByID("Row0Top") as GridSplitter;
            _row0Bottom = DRT.FindElementByID("Row0Bottom") as GridSplitter;
            _row2Top = DRT.FindElementByID("Row2Top") as GridSplitter;
            _row3Stretch = DRT.FindElementByID("Row3Stretch") as GridSplitter;
            _row5Center = DRT.FindElementByID("Row5Center") as GridSplitter;
            _row6Bottom = DRT.FindElementByID("Row6Bottom") as GridSplitter;

            _canvasGridSplitter = DRT.FindElementByID("CanvasGridSplitter") as GridSplitter;
            _defaultGridSplitter = new GridSplitter();

            List<DrtTest> tests = new List<DrtTest>();
            if (!DRT.KeepAlive)
            {
                tests.Add(new DrtTest(BasicTest));
                tests.Add(new DrtTest(MouseTest));
                tests.Add(new DrtTest(KeyboardTest));
            } 
            return tests.ToArray();
        }

        #endregion

        #region Grid Helpers

        // The width and height of grid
        double _gridWidth;
        double _gridHeight;

        //The expected values for each of the rows and columns
        List<GridLength> _expectedColumnLengths;
        List<GridLength> _expectedRowLengths;

        public void SaveGridInfo(Grid grid)
        {
            _gridWidth = grid.ActualWidth;
            _gridHeight = grid.ActualHeight;

            _expectedColumnLengths = new List<GridLength>(grid.ColumnDefinitions.Count);
            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                _expectedColumnLengths.Add((GridLength)grid.ColumnDefinitions[i].GetValue(ColumnDefinition.WidthProperty));
            }

            _expectedRowLengths = new List<GridLength>(grid.RowDefinitions.Count);
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                _expectedRowLengths.Add((GridLength)grid.RowDefinitions[i].GetValue(RowDefinition.HeightProperty));
            }
        }

        public void VerifyGrid(Grid grid)
        {
            double tol = 1e-5;

            DRT.Assert(Math.Abs(grid.ActualWidth - _gridWidth) < tol, "Grid changed size");
            DRT.Assert(Math.Abs(grid.ActualHeight - _gridHeight) < tol, "Grid changed size");

            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                GridLength expected = _expectedColumnLengths[i];
                GridLength actual = (GridLength)grid.ColumnDefinitions[i].GetValue(ColumnDefinition.WidthProperty);
                
                DRT.Assert(actual.IsStar == expected.IsStar, "Column {0} was expected to be defined in {1} but isn't", i, expected.IsStar? "stars":"pixels");

                double diff = actual.Value - expected.Value;
                DRT.Assert(Math.Abs(diff) < tol, "Column {0}'s actual length ({1}) does not match the expected length ({2})", i, actual.Value, expected.Value);
            }

            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                GridLength expected = _expectedRowLengths[i];
                GridLength actual = (GridLength)grid.RowDefinitions[i].GetValue(RowDefinition.HeightProperty);

                DRT.Assert(actual.IsStar == expected.IsStar, "Row {0} was expected to be defined in {1} but isn't", i, expected.IsStar ? "stars" : "pixels");

                double diff = actual.Value - expected.Value;
                DRT.Assert(Math.Abs(diff) < tol, "Row {0}'s actual length ({1}) does not match the expected length ({2})", i, actual.Value, expected.Value);
            }
        }

        #endregion

        #region Tests

        #region BasicTest

        public void BasicTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---GridSplitter Basic Tests");

            // Test the default values for GridSplitter
            DRT.Assert(_defaultGridSplitter.HorizontalAlignment == System.Windows.HorizontalAlignment.Right, "HorizontalAlignment should be right by default");
            DRT.Assert(_defaultGridSplitter.VerticalAlignment == System.Windows.VerticalAlignment.Stretch, "VerticalAlignment should be stretch by default");
            DRT.Assert(_defaultGridSplitter.DragIncrement ==  1.0, "DragIncrement should be 1.0 by default");
            DRT.Assert(_defaultGridSplitter.KeyboardIncrement == 10.0, "DragIncrement should be 10.0 by default");
            DRT.Assert(_defaultGridSplitter.ResizeDirection == GridResizeDirection.Auto, "ResizeDirection should be BasedOnAlignment by default");
            DRT.Assert(_defaultGridSplitter.ResizeBehavior == GridResizeBehavior.BasedOnAlignment, "ResizeDirection should be BasedOnAlignment by default");
            DRT.Assert(_defaultGridSplitter.Focusable == true, "Focusable should be true by default");

            // Test changing ResizeDirection
            _defaultGridSplitter.ResizeDirection = GridResizeDirection.Columns;
            DRT.Assert(_defaultGridSplitter.ResizeDirection == GridResizeDirection.Columns, "ResizeDirection should be Columns");
            _defaultGridSplitter.ResizeDirection = GridResizeDirection.Rows;
            DRT.Assert(_defaultGridSplitter.ResizeDirection == GridResizeDirection.Rows, "ResizeDirection should be Rows");

            try
            {
                _defaultGridSplitter.KeyboardIncrement = -1.0; // invalid
                DRT.Assert(false, "KeyboardIncrement should not allow negative values");
            }
            catch(ArgumentException) { /* Good */ }

            _defaultGridSplitter.KeyboardIncrement = 13.0; 
            DRT.Assert(_defaultGridSplitter.KeyboardIncrement == 13.0, "KeyboardIncrement should be 13.0");

            try
            {
                _defaultGridSplitter.DragIncrement = -1.0; // invalid
                DRT.Assert(false, "MouseIncrement should not allow negative values");
            }
            catch (ArgumentException) { /* Good */ }

            _defaultGridSplitter.DragIncrement = 15.0;
            DRT.Assert(_defaultGridSplitter.DragIncrement == 15.0, "MouseIncrement should be 15.0");
        }
        
        #endregion

        #region Mouse Test


        private void MouseTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---GridSplitter Mouse Tests");

            DRT.ResumeAt(new DrtTest(MouseTestProc));
        }

        private enum MouseTestStep
        {
            Start,

            // Resize columns
            Column0Right_Prepare,
            Column0Right_PressMouse,
            Column0Right_Drag,
            Column0Right_ReleaseMouse,

            Column3Stretch_Prepare,
            Column3Stretch_PressMouse,
            Column3Stretch_Drag,
            Column3Stretch_ReleaseMouse,
            
            Column6Right_Prepare,
            Column6Right_PressMouse,
            Column6Right_Drag,
            Column6Right_ReleaseMouse,

            // Resize rows 
            Row0Top_Prepare,
            Row0Top_PressMouse,
            Row0Top_Drag,
            Row0Top_ReleaseMouse,

            Row2Top_Prepare,
            Row2Top_PressMouse,
            Row2Top_Drag,
            Row2Top_ReleaseMouse,

            Row5Center_Prepare,
            Row5Center_PressMouse,
            Row5Center_Drag,
            Row5Center_ReleaseMouse,

            // Drag a Splitter that is not in a grid
            CanvasGridSplitter_Prepare,
            CanvasGridSplitter_PressMouse,
            CanvasGridSplitter_Drag,
            CanvasGridSplitter_ReleaseMouse,
            
            End,
        }

        MouseTestStep _mouseTestStep = MouseTestStep.Start;

        private void MouseTestProc()
        {
            if (DRT.Verbose) Console.WriteLine("Mouse test = " + _mouseTestStep);

            switch (_mouseTestStep)
            {
                case MouseTestStep.Start:
                    break;

                case MouseTestStep.Column0Right_Prepare:
                    DRT.MoveMouse(_column0Right, 0.5, 0.5);
                    break;

                case MouseTestStep.Column0Right_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.Column0Right_Drag:
                    DRT.MoveMouse(_column0Right, -9.5, 0.95); //Move 50px left
                    break;

                case MouseTestStep.Column0Right_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;

                
                case MouseTestStep.Column3Stretch_Prepare:
                    DRT.MoveMouse(_column3Stretch, 0.5, 0.5);
                    break;
                
                case MouseTestStep.Column3Stretch_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.Column3Stretch_Drag:
                    DRT.MoveMouse(_column3Stretch, 2.5, 0.45); //Move 10px right
                    break;
                
                case MouseTestStep.Column3Stretch_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;


                case MouseTestStep.Column6Right_Prepare:
                    DRT.MoveMouse(_column6Right, 0.5, 0.5);
                    break;

                case MouseTestStep.Column6Right_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.Column6Right_Drag:
                    DRT.MoveMouse(_column6Right, 2.5, 0.75); // Move 10px right
                    break;

                case MouseTestStep.Column6Right_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;


                // Resize rows 
                case MouseTestStep.Row0Top_Prepare:
                    DRT.MoveMouse(_row0Top, 0.5, 0.5);
                    break;

                case MouseTestStep.Row0Top_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.Row0Top_Drag:
                    DRT.MoveMouse(_row0Top, 0.1, 4.5); //Move 20px down 
                    break;

                case MouseTestStep.Row0Top_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;


                case MouseTestStep.Row2Top_Prepare:
                    DRT.MoveMouse(_row2Top, 0.5, 0.5);
                    break;

                case MouseTestStep.Row2Top_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.Row2Top_Drag:
                    DRT.MoveMouse(_row2Top, 0.2, 4.5); // Move 20px down
                    break;

                case MouseTestStep.Row2Top_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;

                
                case MouseTestStep.Row5Center_Prepare:
                    DRT.MoveMouse(_row5Center, 0.5, 0.5);
                    break;

                case MouseTestStep.Row5Center_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.Row5Center_Drag:
                    DRT.MoveMouse(_row5Center, .9, -1.5); //Move 10px up
                    break;

                case MouseTestStep.Row5Center_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;


                // Drag a Splitter that is not in a grid
                case MouseTestStep.CanvasGridSplitter_Prepare:
                    DRT.MoveMouse(_canvasGridSplitter, 0.5, 0.5);
                    break;

                case MouseTestStep.CanvasGridSplitter_PressMouse:
                    DRT.MouseButtonDown();
                    break;

                case MouseTestStep.CanvasGridSplitter_Drag:
                    DRT.MoveMouse(_canvasGridSplitter, 2.5, -0.5); //Move 10px right, 10px up 
                    break;

                case MouseTestStep.CanvasGridSplitter_ReleaseMouse:
                    DRT.MouseButtonUp();
                    break;
                
                case MouseTestStep.End:
                    
                    break;
            }
            DRT.Pause(10);
            DRT.ResumeAt(new DrtTest(MouseTestVerifyProc));
        }

        private void MouseTestVerifyProc()
        {
            if (DRT.Verbose) Console.WriteLine("Mouse test verify = " + _mouseTestStep);

            switch (_mouseTestStep)
            {
                case MouseTestStep.Start:
                    break;

                case MouseTestStep.Column0Right_Prepare:

                    // Initial Column Layout:
                    // 0     | 1      | 2        | 3     | 4        | 5     | 6
                    // 40px  | 40px   | * (40px) | 5px   | * (40px) | 5px   | * (40px)
                    SaveGridInfo(_columnSplitGrid);
                    
                    // Resize Column 1 left by 50px 
                    // Column 1 reduces to splitter width = 5px
                    // * columns absorb (40-5) = 35 pixels.  (Per column that is 11.667)

                    // 0     | 1      | 2          | 3     | 4          | 5     | 6
                    //  5px  | 40px   | * (51.7px) | 5px   | * (51.7px) | 5px   | * (51.7px)
                    _expectedColumnLengths[0] = new GridLength(5, GridUnitType.Pixel);
                    break;

                case MouseTestStep.Column0Right_PressMouse:
                    DRT.Assert(_column0Right.IsDragging, "GridSplitter should be dragging");
                    break;

                case MouseTestStep.Column0Right_ReleaseMouse:
                    VerifyGrid(_columnSplitGrid);                    
                    break;

                case MouseTestStep.Column3Stretch_Prepare:
                    SaveGridInfo(_columnSplitGrid);
                    
                    // Move right 10 px

                    // 0     | 1      | 2              | 3     | 4                | 5     | 6
                    //  5px  | 40px   | 61.7* (61.7px) | 5px   | 41.7* (41.667px) | 5px   | 51.7* (51.7px)
                    _expectedColumnLengths[2] = new GridLength(61.6666667, GridUnitType.Star);
                    _expectedColumnLengths[4] = new GridLength(41.6666667, GridUnitType.Star);
                    _expectedColumnLengths[6] = new GridLength(51.6666667, GridUnitType.Star);
                    break;

                case MouseTestStep.Column3Stretch_PressMouse:
                    DRT.Assert(_column3Stretch.IsDragging, "GridSplitter should be dragging");
                    break;

                case MouseTestStep.Column3Stretch_ReleaseMouse:
                    VerifyGrid(_columnSplitGrid);
                    break;


                case MouseTestStep.Column6Right_Prepare:
                    //Shouldn't move
                    SaveGridInfo(_columnSplitGrid);
                    break;
                
                case MouseTestStep.Column6Right_PressMouse:
                   DRT.Assert(_column6Right.IsDragging, "GridSplitter should be dragging");
                    break;

                case MouseTestStep.Column6Right_ReleaseMouse:
                    VerifyGrid(_columnSplitGrid);
                    break;


                // Resize rows 
                case MouseTestStep.Row0Top_Prepare:
                    // Initial Row Layout:
                    // 0     | 1      | 2        | 3     | 4        | 5     | 6
                    // 40px  | 40px   | * (40px) | 5px   | * (40px) | 5px   | * (40px)
                    SaveGridInfo(_rowSplitGrid);

                    break;


                case MouseTestStep.Row0Top_ReleaseMouse:
                    VerifyGrid(_rowSplitGrid);
                    break;


                case MouseTestStep.Row2Top_Prepare:
                    SaveGridInfo(_rowSplitGrid);


                    // Move down by 20px 

                    // 0     | 1      | 2          | 3     | 4          | 5     | 6
                    // 40px  | 60px   | * (33.3px) | 5px   | * (33.3px) | 5px   | * (33.3px)
                    _expectedRowLengths[1] = new GridLength(60.0, GridUnitType.Pixel);
                    break;

                case MouseTestStep.Row2Top_ReleaseMouse:
                    VerifyGrid(_rowSplitGrid);
                    break;


                case MouseTestStep.Row5Center_Prepare:
                    SaveGridInfo(_rowSplitGrid);
                    // Move up 10 px

                    // 0     | 1      | 2              | 3     | 4              | 5     | 6
                    // 40px  | 60px   | 33.3* (33.3px) | 5px   | 23.3* (23.3px) | 5px   | 43.3* (43.3px)
                    _expectedRowLengths[2] = new GridLength(33.333333, GridUnitType.Star);
                    _expectedRowLengths[4] = new GridLength(23.333333, GridUnitType.Star);
                    _expectedRowLengths[6] = new GridLength(43.333333, GridUnitType.Star);
                    break;

                case MouseTestStep.Row5Center_ReleaseMouse:
                    VerifyGrid(_rowSplitGrid);
                    break;


                // Drag a Splitter that is not in a grid
                case MouseTestStep.CanvasGridSplitter_Prepare:
                    SaveGridInfo(_rootGrid);
                    break;
                    
                case MouseTestStep.CanvasGridSplitter_ReleaseMouse:
                    VerifyGrid(_rootGrid);
                    break;

                case MouseTestStep.End:

                    break;
            }

            if (_mouseTestStep != MouseTestStep.End)
            {
                _mouseTestStep++;
                DRT.ResumeAt(new DrtTest(MouseTestProc));
            }
        }

        #endregion

        #region Keyboard Test

        private void KeyboardTest()
        {
            if (DRT.Verbose) Console.WriteLine("\n---GridSplitter Keyboard Tests");

            DRT.ResumeAt(new DrtTest(KeyboardTestProc));
        }

        private enum KeyboardTestStep 
        {
            Start,

            // Resize columns
            Column0Right_Prepare,
            Column0Right_KeyRight,
            Column0Right_KeyLeft,
            Column0Right_KeyUp,
            Column0Right_KeyDown,

            Row0Bottom_Prepare,
            Row0Bottom_KeyRight,
            Row0Bottom_KeyLeft,
            Row0Bottom_KeyUp,
            Row0Bottom_KeyDown,
                        
            End,
        }

        KeyboardTestStep _keyboardTestStep = KeyboardTestStep.Start;


        private void KeyboardTestProc()
        {
            if (DRT.Verbose) Console.WriteLine("Keyboard test = " + _keyboardTestStep);

            switch (_keyboardTestStep)
            {
                case KeyboardTestStep.Start:
                    break;

                // Resize columns
                case KeyboardTestStep.Column0Right_Prepare:
                    _column0Right.Focus();
                    break;
                case KeyboardTestStep.Column0Right_KeyRight:
                    DRT.PressKey(Key.Right); // move right 50px
                    DRT.PressKey(Key.Right);
                    DRT.PressKey(Key.Right);
                    DRT.PressKey(Key.Right);
                    DRT.PressKey(Key.Right);
                    break;
                case KeyboardTestStep.Column0Right_KeyLeft:
                    DRT.PressKey(Key.Left); // move left 20 px
                    DRT.PressKey(Key.Left);
                    break;
                case KeyboardTestStep.Column0Right_KeyUp:
                    DRT.PressKey(Key.Up); // don't move
                    DRT.PressKey(Key.Up);
                    break;
                case KeyboardTestStep.Column0Right_KeyDown:
                    DRT.PressKey(Key.Down); // don't move
                    DRT.PressKey(Key.Down);
                    break;

                case KeyboardTestStep.Row0Bottom_Prepare:
                    _row0Bottom.Focus();
                    break;
                case KeyboardTestStep.Row0Bottom_KeyRight:
                    DRT.PressKey(Key.Right); // don't move
                    DRT.PressKey(Key.Right);
                    break;
                case KeyboardTestStep.Row0Bottom_KeyLeft:
                    DRT.PressKey(Key.Left); // don't move
                    DRT.PressKey(Key.Left);
                    break;
                case KeyboardTestStep.Row0Bottom_KeyUp:
                    DRT.PressKey(Key.Up); // move up 10 px
                    break;
                case KeyboardTestStep.Row0Bottom_KeyDown:
                    DRT.PressKey(Key.Down); // move down 30 px
                    DRT.PressKey(Key.Down);
                    DRT.PressKey(Key.Down);
                    break;
                case KeyboardTestStep.End:
                    break;
            }
            DRT.ResumeAt(new DrtTest(KeyboardTestVerifyProc));
        }

        private void KeyboardTestVerifyProc()
        {
            switch (_keyboardTestStep)
            {
                case KeyboardTestStep.Start:
                    break;

                // Resize columns
                case KeyboardTestStep.Column0Right_Prepare:
                    DRT.Assert(_column0Right.IsKeyboardFocused);
                    SaveGridInfo(_columnSplitGrid);

                    // 0     | 1      | 2              | 3     | 4                | 5     | 6
                    //  5px  | 40px   | 61.7* (61.7px) | 5px   | 41.7* (41.667px) | 5px   | 51.7* (51.7px)
                    break;
                case KeyboardTestStep.Column0Right_KeyRight:
                    // Move Right 50 px

                    // 0     | 1      | 2     | 3     | 4     | 5     | 6
                    // 55px  | 40px   | 61.7* | 5px   | 41.7* | 5px   | 51.7* 
                    _expectedColumnLengths[0] = new GridLength(55, GridUnitType.Pixel);
                    VerifyGrid(_columnSplitGrid); ;
                    break;
                case KeyboardTestStep.Column0Right_KeyLeft:
                    // move left 20 px

                    // 0     | 1      | 2     | 3     | 4     | 5     | 6
                    // 35px  | 40px   | 61.7* | 5px   | 41.7* | 5px   | 51.7*                     
                    _expectedColumnLengths[0] = new GridLength(35, GridUnitType.Pixel);
                    VerifyGrid(_columnSplitGrid);
                    break;
                case KeyboardTestStep.Column0Right_KeyUp:
                    //Shouldn't Change
                    VerifyGrid(_columnSplitGrid);
                    break;
                case KeyboardTestStep.Column0Right_KeyDown:
                    //Shouldn't Change
                    VerifyGrid(_columnSplitGrid);
                    break;

                case KeyboardTestStep.Row0Bottom_Prepare:
                    DRT.Assert(_row0Bottom.IsKeyboardFocused);
                    SaveGridInfo(_rowSplitGrid);
                    break;
                case KeyboardTestStep.Row0Bottom_KeyRight:
                    //Shouldn't Change                    
                    // 0     | 1      | 2     | 3     | 4     | 5     | 6
                    // 40px  | 60px   | 33.3* | 5px   | 23.3* | 5px   | 43.3*
                    VerifyGrid(_rowSplitGrid);
                    break;
                case KeyboardTestStep.Row0Bottom_KeyLeft:
                    //Shouldn't Change
                    VerifyGrid(_rowSplitGrid);
                    break;
                case KeyboardTestStep.Row0Bottom_KeyUp:
                    // move up 10 px

                    // 0     | 1      | 2     | 3     | 4     | 5     | 6
                    // 30px  | 60px   | 33.3* | 5px   | 23.3* | 5px   | 43.3*
                    _expectedRowLengths[0] = new GridLength(30, GridUnitType.Pixel);
                    VerifyGrid(_rowSplitGrid);
                    break;
                case KeyboardTestStep.Row0Bottom_KeyDown:
                    // move down 30 px

                    // 0     | 1      | 2     | 3     | 4     | 5     | 6
                    // 60px  | 60px   | 33.3* | 5px   | 23.3* | 5px   | 43.3*
                        
                    _expectedRowLengths[0] = new GridLength(60, GridUnitType.Pixel);
                    VerifyGrid(_rowSplitGrid);
                    break;
            }

            if (_keyboardTestStep != KeyboardTestStep.End)
            {
                _keyboardTestStep++;
                DRT.ResumeAt(new DrtTest(KeyboardTestProc));
            }
        }
        #endregion

        #endregion
    }
}
