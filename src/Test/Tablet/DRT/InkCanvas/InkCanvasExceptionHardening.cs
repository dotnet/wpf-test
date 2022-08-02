// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Threading;

namespace DRT
{
    public sealed class InkCanvasExceptionHardeningTests : InkCanvasEditingTests
    {
        private Queue<DrtTest>      _subTests;

        private int _onEventCallbackCount;


        public InkCanvasExceptionHardeningTests()
            : base("ExceptionHardeningTests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // initialize the suite here.  This includes loading the tree.
            base.PrepareTests();
            _subTests = new Queue<DrtTest>();

            // return the lists of tests to run against the tree
            return new DrtTest[] { 
                    new DrtTest(Initialize),
                    // DefaultDrawingAttributes Tests
                    new DrtTest(RunDefaultDrawingAttributesReplacedTest),
                    // EditingMode Test
                    new DrtTest(RunEditingModeTest),
                    // EditingModeInverted Test
                    new DrtTest(RunEditingModeInvertedTest),
                    // Gesture
                    new DrtTest(RunGestureTest),
                    // StrokeCollected
                    new DrtTest(RunStrokeCollectedTest),
                    // Selection Change
                    new DrtTest(RunSelectionChangeTest),
                    // Erase Test
                    new DrtTest(RunEraseTest),
                    // StrokesReplaced Test
                    new DrtTest(RunStrokesReplacedTest),
                    new DrtTest(Uninitialize),
                };
        }

        // Testing an action that Avalon reacts to asynchronously:
        private void Initialize()
        {
            // Set up an exception handler we'll use to eat
            // ReliabilityAssertExceptions thrown by the tests.
            // This simulates an application ignoring the exceptions.
            Dispatcher.CurrentDispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(OnDispatcherException);


        }

        private void RunTests()
        {
            if ( _subTests.Count == 0 )
            {
                return;
            }

            DrtTest next = _subTests.Dequeue();
            DRT.ResumeAt(RunTests);
            DRT.ResumeAt(next);
        }

        private void Uninitialize()
        {
            // Set up an exception handler we'll use to eat
            // ReliabilityAssertExceptions thrown by the tests.
            // This simulates an application ignoring the exceptions.
            Dispatcher.CurrentDispatcher.UnhandledException -= new DispatcherUnhandledExceptionEventHandler(OnDispatcherException);
        }

        // DispatcherException used to eat ReliabilityAssertExceptions thrown
        // by the tests.
        // This simulates an application ignoring the exceptions.
        private void OnDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if ( e.Exception is ReliabilityAssertException ||
                e.Exception.InnerException is ReliabilityAssertException )
            {
                // Handle the ReliabilityAssertException -- Dispatcher will move
                // on to next queue item.
                e.Handled = true;
            }
        }

        private void RunDefaultDrawingAttributesReplacedTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.DefaultDrawingAttributesReplaced += new DrawingAttributesReplacedEventHandler(OnDefaultDrawingAttributesReplaced);

            // Change Attributes on DrawingAttributes
            _subTests.Enqueue(new DrtTest(ReplaceDefaultDrawingAttributes));
            _subTests.Enqueue(new DrtTest(ReplaceDefaultDrawingAttributes));
            _subTests.Enqueue(new DrtTest(VerifyDefaultDrawingAttributesReplacedCount));

            DRT.ResumeAt(RunTests);
        }

        private void ReplaceDefaultDrawingAttributes()
        {
            _inkCanvas.DefaultDrawingAttributes = new DrawingAttributes();
        }

        private void VerifyDefaultDrawingAttributesReplacedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.DefaultDrawingAttributesReplaced!");
            _onEventCallbackCount = 0;
            _inkCanvas.DefaultDrawingAttributesReplaced -= new DrawingAttributesReplacedEventHandler(OnDefaultDrawingAttributesReplaced);
        }

        private void OnDefaultDrawingAttributesReplaced(object sender, DrawingAttributesReplacedEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnDrawingAttributesAttributeChanged");
        }

        private void RunEditingModeTest()
        {

            // InkCanvas.EditingModeChanged
            _subTests.Enqueue(new DrtTest(StartEditingModeChangedTest));
            _subTests.Enqueue(new DrtTest(ChangeEditingMode));
            _subTests.Enqueue(new DrtTest(ChangeEditingMode));
            _subTests.Enqueue(new DrtTest(VerifyEditingModeChangedCount));

            DRT.ResumeAt(RunTests);
        }

        private void ChangeEditingMode()
        {
            if ( _inkCanvas.EditingMode == InkCanvasEditingMode.Ink )
            {
                _inkCanvas.EditingMode = InkCanvasEditingMode.Select;
            }
            else
            {
                _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
        }

        private void StartEditingModeChangedTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingModeChanged += new RoutedEventHandler(OnEditingModeChanged);
        }

        private void VerifyEditingModeChangedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.EditingModeChanged!");
            _onEventCallbackCount = 0;
            _inkCanvas.EditingModeChanged -= new RoutedEventHandler(OnEditingModeChanged);
        }

        private void OnEditingModeChanged(object sender, RoutedEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnEditingModeChanged");
        }

        private void RunEditingModeInvertedTest()
        {

            // InkCanvas.EditingModeChanged
            _subTests.Enqueue(new DrtTest(StartEditingModeInvertedChangedTest));
            _subTests.Enqueue(new DrtTest(ChangeEditingModeInverted));
            _subTests.Enqueue(new DrtTest(ChangeEditingModeInverted));
            _subTests.Enqueue(new DrtTest(VerifyEditingModeInvertedChangedCount));

            DRT.ResumeAt(RunTests);
        }


        private void ChangeEditingModeInverted()
        {
            if ( _inkCanvas.EditingModeInverted == InkCanvasEditingMode.Ink )
            {
                _inkCanvas.EditingModeInverted = InkCanvasEditingMode.Select;
            }
            else
            {
                _inkCanvas.EditingModeInverted = InkCanvasEditingMode.Ink;
            }
        }

        private void StartEditingModeInvertedChangedTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingModeInvertedChanged += new RoutedEventHandler(OnEditingModeInvertedChanged);
        }

        private void VerifyEditingModeInvertedChangedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.EditingModeInvertedChanged!");
            _onEventCallbackCount = 0;
            _inkCanvas.EditingModeInvertedChanged -= new RoutedEventHandler(OnEditingModeChanged);
        }

        private void OnEditingModeInvertedChanged(object sender, RoutedEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnEditingModeInvertedChanged");
        }

        private void RunGestureTest()
        {
            GestureRecognizer gestureRecognizer = new GestureRecognizer();
            if (gestureRecognizer.IsRecognizerAvailable)
            {
                
                // InkCanvas.Gesture
                _subTests.Enqueue(new DrtTest(StartGestureTest));
                _subTests.Enqueue(new DrtTest(DrawStroke));
                _subTests.Enqueue(new DrtTest(DrawStroke));
                _subTests.Enqueue(new DrtTest(VerifyGestureCount));

                DRT.ResumeAt(RunTests);
            }
            else
            {
                DRT.LogOutput("No gesture recognizer has been found. Skip RunGestureTest gracefully.");
            }
        }

        private void StartGestureTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingMode = InkCanvasEditingMode.GestureOnly;

            _inkCanvas.Gesture += new InkCanvasGestureEventHandler(OnGesture);
        }

        public void ReDrawStroke()
        {
            _inkCanvas.Strokes.Clear();
            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            DrawStroke();
        }

        public void DrawStroke()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            // Right Gesture
            //  Point(393f, 203.723333333333f),
            //  Point(482f, 214.723333333333f),
            //  Point(535f, 213.723333333333f),
            //  Point(539f, 212.723333333333f),
            //  Point(542f, 211.723333333333f),
            //  Point(542f, 210.723333333333f)  

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(393f, 203.723333333333f)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(393f, 203.723333333333f)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(482f, 214.723333333333f)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(535f, 213.723333333333f)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(539f, 212.723333333333f)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(542f, 211.723333333333f)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(542f, 210.723333333333f)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(542f, 210.723333333333f)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        private void VerifyGestureCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.Gesture!");
            _onEventCallbackCount = 0;
            _inkCanvas.Gesture -= new InkCanvasGestureEventHandler(OnGesture);
        }

        private void OnGesture(object sender, InkCanvasGestureEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnGesture");
        }

        private void RunStrokeCollectedTest()
        {
            // InkCanvas.Gesture
            _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(DrawStroke));
            _subTests.Enqueue(new DrtTest(DrawStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokeCollectedCount));

            DRT.ResumeAt(RunTests);
        }

        private void StartStrokeCollectedTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;

            _inkCanvas.StrokeCollected += new InkCanvasStrokeCollectedEventHandler(OnStrokeCollected);
        }

        private void VerifyStrokeCollectedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.Gesture!");
            _onEventCallbackCount = 0;
            _inkCanvas.StrokeCollected -= new InkCanvasStrokeCollectedEventHandler(OnStrokeCollected);
        }

        private void OnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnStrokeCollected");
        }

        private void RunSelectionChangeTest()
        {
            // Draw a stroke
            _inkCanvas.Strokes.Clear();
            _subTests.Enqueue(new DrtTest(DrawStroke));

            // InkCanvas.SelectionChanging
            _subTests.Enqueue(new DrtTest(StartSelectionChangingTest));
            _subTests.Enqueue(new DrtTest(ToggleCurrentSelection));
            _subTests.Enqueue(new DrtTest(ToggleCurrentSelection));
            _subTests.Enqueue(new DrtTest(VerifySelectionChangingCount));

            // InkCanvas.SelectionChanged
            _subTests.Enqueue(new DrtTest(StartSelectionChangedTest));
            _subTests.Enqueue(new DrtTest(ToggleCurrentSelection));
            _subTests.Enqueue(new DrtTest(ToggleCurrentSelection));
            _subTests.Enqueue(new DrtTest(VerifySelectionChangedCount));

            // Draw a stroke
            _inkCanvas.Strokes.Clear();
            _subTests.Enqueue(new DrtTest(DrawStroke));

            // InkCanvas.SelectionMoving
            _subTests.Enqueue(new DrtTest(StartSelectionMovingTest));
            _subTests.Enqueue(new DrtTest(MoveSelection));
            _subTests.Enqueue(new DrtTest(MoveSelection));
            _subTests.Enqueue(new DrtTest(VerifySelectionMovingCount));

            // InkCanvas.SelectionMoved
            _subTests.Enqueue(new DrtTest(StartSelectionMovedTest));
            _subTests.Enqueue(new DrtTest(MoveSelection));
            _subTests.Enqueue(new DrtTest(MoveSelection));
            _subTests.Enqueue(new DrtTest(VerifySelectionMovedCount));

            // InkCanvas.SelectionResizing
            _subTests.Enqueue(new DrtTest(StartSelectionResizingTest));
            _subTests.Enqueue(new DrtTest(ResizeSelection));
            _subTests.Enqueue(new DrtTest(ResizeSelection));
            _subTests.Enqueue(new DrtTest(VerifySelectionResizingCount));

            // InkCanvas.SelectionResized
            _subTests.Enqueue(new DrtTest(StartSelectionResizedTest));
            _subTests.Enqueue(new DrtTest(ResizeSelection));
            _subTests.Enqueue(new DrtTest(ResizeSelection));
            _subTests.Enqueue(new DrtTest(VerifySelectionResizedCount));

            DRT.ResumeAt(RunTests);
        }

        private void StartSelectionChangingTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;

            _inkCanvas.SelectionChanging += new InkCanvasSelectionChangingEventHandler(OnSelectionChanging);
        }

        private void ToggleCurrentSelection()
        {
            StrokeCollection selectedStrokes = _inkCanvas.GetSelectedStrokes();
            if ( selectedStrokes.Count != 0 )
            {
                _inkCanvas.Select(new StrokeCollection());
            }
            else
            {
                _inkCanvas.Select(_inkCanvas.Strokes);
            }
        }

        private void VerifySelectionChangingCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.SelectionChanging!");
            _onEventCallbackCount = 0;
            _inkCanvas.SelectionChanging -= new InkCanvasSelectionChangingEventHandler(OnSelectionChanging);
        }

        private void OnSelectionChanging(object sender, InkCanvasSelectionChangingEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnSelectionChanging");
        }

        private void StartSelectionChangedTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;

            _inkCanvas.SelectionChanged += new EventHandler(OnSelectionChanged);
        }

        private void VerifySelectionChangedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.SelectionChanged!");
            _onEventCallbackCount = 0;
            _inkCanvas.SelectionChanged -= new EventHandler(OnSelectionChanged);
        }

        private void OnSelectionChanged(object sender, EventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnSelectionChanged");
        }

        private void StartSelectionMovingTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;
            _inkCanvas.Select(_inkCanvas.Strokes);

            _inkCanvas.SelectionMoving += new InkCanvasSelectionEditingEventHandler(OnSelectionMoving);
        }

        private void MoveSelection()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            Rect bounds = _inkCanvas.GetSelectionBounds();
            DRT.Trace(string.Format("Bounds = {0}", bounds));

            // Bottom-Right
            Point point = new Point(( bounds.Left + bounds.Right ) / 2,
                                ( bounds.Top + bounds.Bottom ) / 2);

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, point));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, point));
            point.Offset(-15, -15);
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, point));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, point));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        private void VerifySelectionMovingCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.SelectionMoving!");
            _onEventCallbackCount = 0;
            _inkCanvas.SelectionMoving -= new InkCanvasSelectionEditingEventHandler(OnSelectionMoving);
        }

        private void OnSelectionMoving(object sender,  InkCanvasSelectionEditingEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnSelectionMoving");
        }

        private void StartSelectionMovedTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;
            _inkCanvas.Select(_inkCanvas.Strokes);

            _inkCanvas.SelectionMoved += new EventHandler(OnSelectionMoved);
        }

        private void VerifySelectionMovedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.SelectionMoved!");
            _onEventCallbackCount = 0;
            _inkCanvas.SelectionMoved -= new EventHandler(OnSelectionMoved);
        }

        private void OnSelectionMoved(object sender, EventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnSelectionMoved");
        }

        private void StartSelectionResizingTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;
            _inkCanvas.Select(_inkCanvas.Strokes);

            _inkCanvas.SelectionResizing += new InkCanvasSelectionEditingEventHandler(OnSelectionResizing);
        }

        private void ResizeSelection()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            Rect bounds = _inkCanvas.GetSelectionBounds();
            DRT.Trace(string.Format("Bounds = {0}", bounds));

            // Bottom-Right
            Point pt = new Point(bounds.Right, bounds.Bottom);
            pt.Offset(ResizeHandleOffset, ResizeHandleOffset);

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, pt));

            pt.Offset(5, 5);
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, pt));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        private void VerifySelectionResizingCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.SelectionResizing!");
            _onEventCallbackCount = 0;
            _inkCanvas.SelectionResizing -= new InkCanvasSelectionEditingEventHandler(OnSelectionResizing);
        }

        private void OnSelectionResizing(object sender, InkCanvasSelectionEditingEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnSelectionResizing");
        }

        private void StartSelectionResizedTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;
            _inkCanvas.Select(_inkCanvas.Strokes);

            _inkCanvas.SelectionResized += new EventHandler(OnSelectionResized);
        }

        private void VerifySelectionResizedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.SelectionResized!");
            _onEventCallbackCount = 0;
            _inkCanvas.SelectionResized -= new EventHandler(OnSelectionResized);
        }

        private void OnSelectionResized(object sender, EventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnSelectionResized");
        }

        private void RunEraseTest()
        {
            // InkCanvas.StrokeErasing
            _subTests.Enqueue(new DrtTest(StartStrokeErasingTest));
            _subTests.Enqueue(new DrtTest(EraseStroke));
            _subTests.Enqueue(new DrtTest(EraseStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokeErasingCount));

            // InkCanvas.StrokeErased
            _subTests.Enqueue(new DrtTest(StartStrokeErasedTest));
            _subTests.Enqueue(new DrtTest(EraseStroke));
            _subTests.Enqueue(new DrtTest(ReDrawStroke));
            _subTests.Enqueue(new DrtTest(EraseStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokeErasedCount));

            DRT.ResumeAt(RunTests);
        }

        private void StartStrokeErasingTest()
        {
            // Draw a stroke
            _inkCanvas.Strokes.Clear();
            _onEventCallbackCount = 0;

            _inkCanvas.StrokeErasing += new InkCanvasStrokeErasingEventHandler(OnStrokeErasing);

            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            DRT.ResumeAt(DrawStroke);
        }

        private void EraseStroke()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;

            DRT.MouseButtonUp();            // just in case someone left it down

            Rect bounds = _inkCanvas.Strokes.GetBounds();
            DRT.Trace(string.Format("Bounds = {0}", bounds));

            // Top-Left
            Point pt = new Point((bounds.Left + bounds.Right) / 2, (bounds.Top + bounds.Bottom) / 2);

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, pt));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, pt));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        private void VerifyStrokeErasingCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.StrokeErasing!");
            _onEventCallbackCount = 0;
            _inkCanvas.StrokeErasing -= new InkCanvasStrokeErasingEventHandler(OnStrokeErasing);
        }

        private void OnStrokeErasing(object sender, InkCanvasStrokeErasingEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnStrokeErasing");
        }

        private void StartStrokeErasedTest()
        {
            // Draw a stroke
            _inkCanvas.Strokes.Clear();

            _onEventCallbackCount = 0;

            _inkCanvas.StrokeErased += new RoutedEventHandler(OnStrokeErased);
            
            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            DRT.ResumeAt(DrawStroke);

        }

        private void VerifyStrokeErasedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.StrokeErased!");
            _onEventCallbackCount = 0;
            _inkCanvas.StrokeErased -= new RoutedEventHandler(OnStrokeErased);
        }

        private void OnStrokeErased(object sender, RoutedEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnStrokeErased");
        }

        private void RunStrokesReplacedTest()
        {
            _onEventCallbackCount = 0;
            _inkCanvas.StrokesReplaced += new InkCanvasStrokesReplacedEventHandler(OnStrokesReplaced);

            // InkCanvas.StrokeErasing
            _subTests.Enqueue(new DrtTest(ReplaceStrokes));
            _subTests.Enqueue(new DrtTest(ReplaceStrokes));
            _subTests.Enqueue(new DrtTest(VerifyStrokesReplacedCount));

            DRT.ResumeAt(RunTests);
        }

        private void ReplaceStrokes()
        {
            StrokeCollection newStrokes = new StrokeCollection();
            newStrokes.Add(new Stroke(new StylusPointCollection(new Point[] { new Point(100, 100), new Point(100, 200) })));
            _inkCanvas.Strokes = newStrokes;
        }

        private void VerifyStrokesReplacedCount()
        {
            DRT.Assert(_onEventCallbackCount == 2, "Unexpected number of callbacks to InkCanvas.StrokesReplace!");
            _onEventCallbackCount = 0;
            _inkCanvas.StrokesReplaced -= new InkCanvasStrokesReplacedEventHandler(OnStrokesReplaced);
        }

        private void OnStrokesReplaced(object sender, InkCanvasStrokesReplacedEventArgs e)
        {
            _onEventCallbackCount++;
            throw new ReliabilityAssertException("OnStrokesReplaced");
        }

    }
}
