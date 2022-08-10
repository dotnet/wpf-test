// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Runtime.InteropServices;

namespace DRT
{
    public class InkCanvasEditingTests : DrtTabletTestSuite
    {
        protected enum InputAction
        {
            MouseMove = 0,
            MouseLeftDown = 1,
            MouseLeftUp = 2,
        }

        protected class TestInput
        {
            private InputAction _action;
            private Point       _position;

            public TestInput(InputAction action, Point position)
            {
                _action = action;
                _position = position;
            }

            public InputAction Action
            {
                get
                {
                    return _action;
                }
            }

            public Point Position
            {
                get
                {
                    return _position;
                }
            }
        }

        protected InkCanvas           _inkCanvas;
        private StylusInputSimulation.Mouse _mouseDriver;
        protected Queue<TestInput>    _inputQueue;
        private bool                _eventFired;
        protected const int           ResizeHandleOffset = 6;  // Handle Offset

        #region Constructor

        public InkCanvasEditingTests()
            : this("InkCanvasEditingTests")
        {
        }

        public InkCanvasEditingTests(string drtName)
            : base(drtName)
        {
            _mouseDriver = new StylusInputSimulation.Mouse();
            _inputQueue = new Queue<TestInput>();
        }

        #endregion Constructor

        #region Override Methods

        public override DrtTest[] PrepareTests()
        {
            // For example to create a tree via code:
            Visual root = new Border();

            _inkCanvas = new InkCanvas();
            _inkCanvas.Background = Brushes.White;

            ((Border)(root)).Child = _inkCanvas;

            DRT.Show(root);

            return new DrtTest[]{
                                new DrtTest(DrawStrokeWithDefaultDefaultDrawingAttributes),
                                new DrtTest(VerifyDrawStrokeWithDefaultDefaultDrawingAttributes),
                                new DrtTest(DrawStrokeWithRedEllipse),
                                new DrtTest(VerifyDrawStrokeWithRedEllipse),
                                new DrtTest(EraseWithPointEraser),
                                new DrtTest(VerifyEraseWithPointEraser),
                                new DrtTest(DrawStrokeWithYellowHighlighter),
                                new DrtTest(VerifyDrawStrokeWithYellowHighlighter),
                                new DrtTest(EraseStrokeWithStrokeEraser),
                                new DrtTest(VerifyEraseStrokeWithStrokeEraser),
                                new DrtTest(SelectStroke),
                                new DrtTest(VerifySelectStroke),
                                new DrtTest(ResizeStroke),
                                new DrtTest(VerifyResizeStroke),
                                new DrtTest(MoveStroke),
                                new DrtTest(VerifyMoveStroke),
                                new DrtTest(DeselectStroke),
                                new DrtTest(VerifyDeselectStroke),
                                new DrtTest(SwitchToNoneMode),
                                new DrtTest(VerifySwitchToNoneMode),
                                new DrtTest(TapSelectStroke),
                                new DrtTest(VerifySelectStroke),
                                };
        }

        #endregion Override Methods

        #region Tests

        public void DrawStrokeWithDefaultDefaultDrawingAttributes()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(50, 50)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(50, 50)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(150, 50)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(150, 100)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(50, 100)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(50, 50)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(50, 50)));
            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyDrawStrokeWithDefaultDefaultDrawingAttributes()
        {
            DRT.AssertEqual(1, _inkCanvas.Strokes.Count, "Failed to draw stroke with the default DefaultDrawingAttributes!");
        }

        protected void SendInputs()
        {
            if ( _inputQueue.Count == 0 )
            {
                return;
            }

            TestInput input = _inputQueue.Dequeue();

            switch ( input.Action )
            {
                case InputAction.MouseMove:
                    _mouseDriver.SimulateMove(DRT.MainWindow, input.Position);
                    DRT.Trace(string.Format("Move: Position = {0}", input.Position));
                    break;
                case InputAction.MouseLeftDown:
                    _mouseDriver.SimulateLeftDown(DRT.MainWindow, input.Position);
                    DRT.Trace(string.Format("Down: Position = {0}", input.Position));
                    break;
                case InputAction.MouseLeftUp:
                    _mouseDriver.SimulateLeftUp(DRT.MainWindow, input.Position);
                    DRT.Trace(string.Format("Up: Position = {0}", input.Position));
                    break;
            }
            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void DrawStrokeWithRedEllipse()
        {
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(100, 150)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(100, 150)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(120, 150)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(130, 155)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(140, 145)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(150, 150)));

            DrawingAttributes da = new DrawingAttributes();
            da.Color = Colors.Red;
            da.Height = 7.0f;
            da.Width = 7.0f;
            da.StylusTip = StylusTip.Ellipse;
            _inkCanvas.DefaultDrawingAttributes = da;

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyDrawStrokeWithRedEllipse()
        {
            DRT.AssertEqual(2, _inkCanvas.Strokes.Count, "Failed to draw stroke with the red ellipse pen!");
        }

        public void EraseWithPointEraser()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(120, 130)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(120, 130)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(125, 155)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(125, 155)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyEraseWithPointEraser()
        {
            DRT.AssertEqual(3, _inkCanvas.Strokes.Count, "Failed to erase a stroke with point eraser!");
        }

        public void DrawStrokeWithYellowHighlighter()
        {
            _inkCanvas.DefaultDrawingAttributes.Color = Colors.Yellow;
            _inkCanvas.DefaultDrawingAttributes.Width = 10;
            _inkCanvas.DefaultDrawingAttributes.Height = 30;
            _inkCanvas.DefaultDrawingAttributes.IsHighlighter = true;

            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(50, 160)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(50, 160)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(155, 160)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(155, 160)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyDrawStrokeWithYellowHighlighter()
        {
            DRT.AssertEqual(4, _inkCanvas.Strokes.Count, "Failed to draw a stroke with yellow hightlighter!");
        }

        public void EraseStrokeWithStrokeEraser()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(100, 40)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(100, 40)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(100, 170)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(100, 170)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyEraseStrokeWithStrokeEraser()
        {
            DRT.AssertEqual(1, _inkCanvas.Strokes.Count, "Failed to draw a stroke with yellow hightlighter!");
        }

        public void SelectStroke()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;

            DRT.MouseButtonUp();            // just in case someone left it down

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(121, 141)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(121, 141)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(121, 137)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(121, 134)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(126, 131)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(129, 130)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(134, 129)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(139, 129)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(141, 131)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(144, 134)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(145, 139)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(145, 141)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(145, 144)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(145, 147)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(145, 151)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(145, 154)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(145, 156)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(144, 158)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(142, 160)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(140, 162)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(138, 162)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(134, 162)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(128, 159)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(123, 154)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(120, 149)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(119, 148)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(119, 147)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(119, 145)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(120, 145)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(120, 145)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void TapSelectStroke()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;

            DRT.MouseButtonUp();            // just in case someone left it down

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(130, 155)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(130, 155)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(130, 155)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifySelectStroke()
        {
            StrokeCollection selectedStrokes = _inkCanvas.GetSelectedStrokes();
            DRT.AssertEqual(1, selectedStrokes.Count, "Failed to select a stroke!");
        }

        public void ResizeStroke()
        {
            _eventFired = false;
            _inkCanvas.SelectionResized += new EventHandler(OnInkCanvasSelectionChanged);
            
            Rect selectionRect = _inkCanvas.GetSelectionBounds();

            // Bottom-Right
            Point pt = new Point(selectionRect.Right, selectionRect.Bottom);
            pt.Offset(ResizeHandleOffset, ResizeHandleOffset);

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, pt));

            pt.Offset(15, 15);
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, pt));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyResizeStroke()
        {
            DRT.Assert(_eventFired, "Failed to resize a stroke!");
            _inkCanvas.SelectionResized -= new EventHandler(OnInkCanvasSelectionChanged);

        }

        public void MoveStroke()
        {
            _eventFired = false;
            _inkCanvas.SelectionMoved += new EventHandler(OnInkCanvasSelectionChanged);

            Rect selectionRect = _inkCanvas.GetSelectionBounds();

            // Bottom-Right
            Point pt = new Point((selectionRect.Left + selectionRect.Right) / 2, 
                                (selectionRect.Top + selectionRect.Bottom) / 2);

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, pt));

            pt.Offset(-10, -10);
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, pt));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyMoveStroke()
        {
            DRT.Assert(_eventFired, "Failed to move a stroke!");
            _inkCanvas.SelectionMoved -= new EventHandler(OnInkCanvasSelectionChanged);

        }

        public void DeselectStroke()
        {
            Rect selectionRect = _inkCanvas.GetSelectionBounds();

            Point pt = new Point(selectionRect.Right, selectionRect.Bottom);

            pt.Offset(50, 50);

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, pt));

            pt.Offset(1, 2);
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, pt));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifyDeselectStroke()
        {
            StrokeCollection selectedStrokes = _inkCanvas.GetSelectedStrokes();
            DRT.AssertEqual(0, selectedStrokes.Count, "Failed to deselect a stroke!");
        }

        public void SwitchToNoneMode()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.None;

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(86, 97)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(86, 97)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(146, 103)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(146, 103)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        public void VerifySwitchToNoneMode()
        {
            DRT.AssertEqual(1, _inkCanvas.Strokes.Count, "Failed to test on None mode!");
        }

        private void OnInkCanvasSelectionChanged(object sender, EventArgs args)
        {
            _eventFired = true;
        }

        #endregion Tests

    }
}
