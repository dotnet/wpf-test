// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Threading;

namespace DRT
{
    public sealed class InkCanvasMidStrokeTests : InkCanvasEditingTests
    {
        private Queue<DrtTest>      _subTests;
        private bool                _testGesture;
        private bool                _testStrokeCollected;
        private bool                _testStrokeErased;
        private bool                _testSelectionChanged;
        private bool                _testStrokesDecreased;
        private bool                _testStrokesIncreased;
        private int                 _cachedStrokesCount;
        private bool                _IsRecognizerAvailable;

        public InkCanvasMidStrokeTests()
            : base("MidStrokeTests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            // initialize the suite here.  This includes loading the tree.
            base.PrepareTests();
            _subTests = new Queue<DrtTest>();

            GestureRecognizer gestureRecognizer = new GestureRecognizer();
            _IsRecognizerAvailable = gestureRecognizer.IsRecognizerAvailable;
            if ( !_IsRecognizerAvailable )
            {
                DRT.LogOutput("No gesture recognizer has been found. Skip GestureTest gracefully.");
            }

            // return the lists of tests to run against the tree
            return new DrtTest[] { 
                    new DrtTest(InkToOthers),
                    new DrtTest(EraseByStrokeToOthers),
                    new DrtTest(SelectToOthers),
                };
        }

        // Testing an action that Avalon reacts to asynchronously:
        private void ResetStrokes()
        {
            _inkCanvas.Strokes.Clear();
            _inkCanvas.Strokes.Add(new Stroke(new System.Windows.Input.StylusPointCollection(new Point[] { new Point(100d, 100d), new Point(100d, 200d) })));
            _inkCanvas.Strokes.Add(new Stroke(new System.Windows.Input.StylusPointCollection(new Point[] { new Point(130d, 100d), new Point(130d, 200d) })));
            _inkCanvas.Strokes.Add(new Stroke(new System.Windows.Input.StylusPointCollection(new Point[] { new Point(160d, 100d), new Point(160d, 200d) })));
            _inkCanvas.Strokes.Add(new Stroke(new System.Windows.Input.StylusPointCollection(new Point[] { new Point(190d, 100d), new Point(190d, 200d) })));
            _inkCanvas.Strokes.Add(new Stroke(new System.Windows.Input.StylusPointCollection(new Point[] { new Point(210d, 100d), new Point(210d, 200d) })));
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

        private void InkToOthers()
        {
            _subTests.Enqueue(new DrtTest(ResetStrokes));

            if ( _IsRecognizerAvailable )
            {
                // From Ink To InkAndGesture
                _subTests.Enqueue
                (
                    (DrtTest)delegate()
                    {
                        DRT.Trace("***** From Ink To InkAndGesture *****");
                    }
                );
                _subTests.Enqueue(new DrtTest(ChangeToInk));
                _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
                _subTests.Enqueue(new DrtTest(StartGestureTest));
                _subTests.Enqueue(new DrtTest(BeginStroke));
                _subTests.Enqueue(new DrtTest(ChangeToInkAndGesture));
                _subTests.Enqueue(new DrtTest(EndStroke));
                _subTests.Enqueue(new DrtTest(VerifyGesture));
                _subTests.Enqueue(new DrtTest(VerifyNoneStrokeCollected));
                _subTests.Enqueue(new DrtTest(EndGestureTest));
                _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));

                // From Ink To GestureOnly
                _subTests.Enqueue
                (
                    (DrtTest)delegate()
                    {
                        DRT.Trace("***** From Ink To GestureOnly *****");
                    }
                );
                _subTests.Enqueue(new DrtTest(ChangeToInk));
                _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
                _subTests.Enqueue(new DrtTest(StartGestureTest));
                _subTests.Enqueue(new DrtTest(BeginStroke));
                _subTests.Enqueue(new DrtTest(ChangeToGestureOnly));
                _subTests.Enqueue(new DrtTest(EndStroke));
                _subTests.Enqueue(new DrtTest(VerifyGesture));
                _subTests.Enqueue(new DrtTest(VerifyNoneStrokeCollected));
                _subTests.Enqueue(new DrtTest(EndGestureTest));
                _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));
            }

            // From Ink To EraseByPoint
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From Ink To EraseByPoint *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ChangeToInk));
            _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(StartEraseTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByPoint));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(VerifyNoneStrokeCollected));
            _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
            _subTests.Enqueue(new DrtTest(EndEraseTest));
            _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));

            // From Ink To EraseByStroke
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From Ink To EraseByStroke *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToInk));
            _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(StartEraseTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(VerifyNoneStrokeCollected));
            _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
            _subTests.Enqueue(new DrtTest(EndEraseTest));
            _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));

            // From Ink To Select
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From Ink To Select *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToInk));
            _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(StartSelectTest));
            _subTests.Enqueue(new DrtTest(BeginSelectStroke));
            _subTests.Enqueue(new DrtTest(ChangeToSelect));
            _subTests.Enqueue(new DrtTest(EndSelectStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokesSelected));
            _subTests.Enqueue(new DrtTest(VerifyNoneStrokeCollected));
            _subTests.Enqueue(new DrtTest(EndSelectTest));
            _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));

            // From Ink To None
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From Ink To None *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToInk));
            _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(ChangeToNone));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(VerifyNoneStrokeCollected));
            _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));

            DRT.ResumeAt(RunTests);

        }

        private void EraseByStrokeToOthers()
        {
            _subTests.Enqueue(new DrtTest(ResetStrokes));

            // From EraseByStroke To Ink
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From EraseByStroke To Ink *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
            _subTests.Enqueue(new DrtTest(StartEraseTest));
            _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(ChangeToInk));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
            _subTests.Enqueue(new DrtTest(VerifyStrokeCollected));
            _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(EndEraseTest));

            if ( _IsRecognizerAvailable )
            {
                // From EraseByStroke To InkAndGesture
                _subTests.Enqueue
                (
                    (DrtTest)delegate()
                    {
                        DRT.Trace("***** From EraseByStroke To InkAndGesture *****");
                    }
                );
                _subTests.Enqueue(new DrtTest(ResetStrokes));
                _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
                _subTests.Enqueue(new DrtTest(StartEraseTest));
                _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
                _subTests.Enqueue(new DrtTest(StartGestureTest));
                _subTests.Enqueue(new DrtTest(BeginStroke));
                _subTests.Enqueue(new DrtTest(ChangeToInkAndGesture));
                _subTests.Enqueue(new DrtTest(EndStroke));
                _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
                _subTests.Enqueue(new DrtTest(VerifyGesture));
                _subTests.Enqueue(new DrtTest(VerifyNoneStrokeCollected));
                _subTests.Enqueue(new DrtTest(EndGestureTest));
                _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));
                _subTests.Enqueue(new DrtTest(EndEraseTest));

                // From EraseByStroke To GestureOnly
                _subTests.Enqueue
                (
                    (DrtTest)delegate()
                    {
                        DRT.Trace("***** From EraseByStroke To GestureOnly *****");
                    }
                );
                _subTests.Enqueue(new DrtTest(ResetStrokes));
                _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
                _subTests.Enqueue(new DrtTest(StartEraseTest));
                _subTests.Enqueue(new DrtTest(StartGestureTest));
                _subTests.Enqueue(new DrtTest(BeginStroke));
                _subTests.Enqueue(new DrtTest(ChangeToGestureOnly));
                _subTests.Enqueue(new DrtTest(EndStroke));
                _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
                _subTests.Enqueue(new DrtTest(VerifyGesture));
                _subTests.Enqueue(new DrtTest(EndGestureTest));
                _subTests.Enqueue(new DrtTest(EndEraseTest));
            }

            // From EraseByStroke To EraseByPoint
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From EraseByStroke To EraseByPoint *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
            _subTests.Enqueue(new DrtTest(StartEraseTest));
            _subTests.Enqueue(new DrtTest(StartStrokesCountTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(EndStrokesCountTest));
            _subTests.Enqueue(new DrtTest(VerifyStrokesDecreasted));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByPoint));
            _subTests.Enqueue(new DrtTest(StartStrokesCountTest));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(EndStrokesCountTest));
            _subTests.Enqueue(new DrtTest(VerifyStrokesIncreasted));
            _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
            _subTests.Enqueue(new DrtTest(EndEraseTest));
            
            // From EraseByStroke To Select
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From EraseByStroke To Select *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
            _subTests.Enqueue(new DrtTest(StartSelectTest));
            _subTests.Enqueue(new DrtTest(BeginSelectStroke));
            _subTests.Enqueue(new DrtTest(ChangeToSelect));
            _subTests.Enqueue(new DrtTest(EndSelectStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokesSelected));
            _subTests.Enqueue(new DrtTest(EndSelectTest));

            // From EraseByStroke To None
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From EraseByStroke To None *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
            _subTests.Enqueue(new DrtTest(StartEraseTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
            _subTests.Enqueue(new DrtTest(EndEraseTest));
            _subTests.Enqueue(new DrtTest(StartEraseTest));
            _subTests.Enqueue(new DrtTest(ChangeToNone));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(VerifyNoneStrokesErased));
            _subTests.Enqueue(new DrtTest(EndEraseTest));

            DRT.ResumeAt(RunTests);

        }

        private void SelectToOthers()
        {
            _subTests.Enqueue(new DrtTest(ResetStrokes));

            // From Select To Ink
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From Select To Ink *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ChangeToSelect));
            _subTests.Enqueue(new DrtTest(StartSelectTest));
            _subTests.Enqueue(new DrtTest(StartStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(BeginSelectStroke));
            _subTests.Enqueue(new DrtTest(ChangeToInk));
            _subTests.Enqueue(new DrtTest(EndSelectStroke));
            _subTests.Enqueue(new DrtTest(VerifyNoneStrokesSelected));
            _subTests.Enqueue(new DrtTest(VerifyStrokeCollected));
            _subTests.Enqueue(new DrtTest(EndStrokeCollectedTest));
            _subTests.Enqueue(new DrtTest(EndSelectTest));

            if ( _IsRecognizerAvailable )
            {
                // From Select To InkAndGesture
                _subTests.Enqueue
                (
                    (DrtTest)delegate()
                    {
                        DRT.Trace("***** From Select To InkAndGesture *****");
                    }
                );
                _subTests.Enqueue(new DrtTest(ResetStrokes));
                _subTests.Enqueue(new DrtTest(ChangeToSelect));
                _subTests.Enqueue(new DrtTest(StartGestureTest));
                _subTests.Enqueue(new DrtTest(BeginStroke));
                _subTests.Enqueue(new DrtTest(ChangeToInkAndGesture));
                _subTests.Enqueue(new DrtTest(EndStroke));
                _subTests.Enqueue(new DrtTest(VerifyGesture));
                _subTests.Enqueue(new DrtTest(EndGestureTest));

                // From Select To GestureOnly
                _subTests.Enqueue
                (
                    (DrtTest)delegate()
                    {
                        DRT.Trace("***** From Select To GestureOnly *****");
                    }
                );
                _subTests.Enqueue(new DrtTest(ResetStrokes));
                _subTests.Enqueue(new DrtTest(ChangeToSelect));
                _subTests.Enqueue(new DrtTest(StartGestureTest));
                _subTests.Enqueue(new DrtTest(BeginStroke));
                _subTests.Enqueue(new DrtTest(ChangeToGestureOnly));
                _subTests.Enqueue(new DrtTest(EndStroke));
                _subTests.Enqueue(new DrtTest(VerifyGesture));
                _subTests.Enqueue(new DrtTest(EndGestureTest));
            }

            // From Select To EraseByPoint
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From Select To EraseByPoint *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToSelect));
            _subTests.Enqueue(new DrtTest(StartEraseTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByPoint));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
            _subTests.Enqueue(new DrtTest(EndEraseTest));

            // From Select To EraseByStroke
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From Select To EraseByStroke *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToSelect));
            _subTests.Enqueue(new DrtTest(StartEraseTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(VerifyStrokesErased));
            _subTests.Enqueue(new DrtTest(EndEraseTest));
            
            // From Select To None
            _subTests.Enqueue
            (
                (DrtTest)delegate()
                {
                    DRT.Trace("***** From Select To None *****");
                }
            );
            _subTests.Enqueue(new DrtTest(ResetStrokes));
            _subTests.Enqueue(new DrtTest(ChangeToSelect));
            _subTests.Enqueue(new DrtTest(StartSelectTest));
            _subTests.Enqueue(new DrtTest(BeginStroke));
            _subTests.Enqueue(new DrtTest(ChangeToEraseByStroke));
            _subTests.Enqueue(new DrtTest(EndStroke));
            _subTests.Enqueue(new DrtTest(VerifyNoneStrokesSelected));
            _subTests.Enqueue(new DrtTest(EndSelectTest));

            DRT.ResumeAt(RunTests);

        }

        private void ChangeToInk()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void ChangeToInkAndGesture()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.InkAndGesture;
        }

        private void ChangeToGestureOnly()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.InkAndGesture;
        }

        private void ChangeToEraseByPoint()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void ChangeToEraseByStroke()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.EraseByStroke;
        }

        private void ChangeToSelect()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.Select;
        }

        private void ChangeToNone()
        {
            _inkCanvas.EditingMode = InkCanvasEditingMode.None;
        }

        private void StartStrokeCollectedTest()
        {
            _inkCanvas.StrokeCollected += new InkCanvasStrokeCollectedEventHandler(OnInkCanvasStrokeCollected);
            _testStrokeCollected = false;
        }

        private void EndStrokeCollectedTest()
        {
            _testStrokeCollected = false;
            _inkCanvas.StrokeCollected -= new InkCanvasStrokeCollectedEventHandler(OnInkCanvasStrokeCollected);
        }

        private void StartGestureTest()
        {
            _inkCanvas.Gesture += new InkCanvasGestureEventHandler(OnInkCanvasGesture);
            _testGesture = false;
        }

        private void EndGestureTest()
        {
            _testGesture = false;
            _inkCanvas.Gesture -= new InkCanvasGestureEventHandler(OnInkCanvasGesture);
        }

        private void StartEraseTest()
        {
            _inkCanvas.StrokeErased += new RoutedEventHandler(OnInkCanvasStrokeErased);
            _testStrokeErased = false;
        }

        private void EndEraseTest()
        {
            _testStrokeErased = false;
            _inkCanvas.StrokeErased -= new RoutedEventHandler(OnInkCanvasStrokeErased);
        }

        private void StartSelectTest()
        {
            _inkCanvas.SelectionChanged += new EventHandler(OnInkCanvasSelectionChanged);
            _testSelectionChanged = false;
        }

        private void EndSelectTest()
        {
            _testSelectionChanged = false;
            _inkCanvas.SelectionChanged -= new EventHandler(OnInkCanvasSelectionChanged);
        }

        private void StartStrokesCountTest()
        {
            _cachedStrokesCount = _inkCanvas.Strokes.Count;
            _testStrokesDecreased = false;
            _testStrokesIncreased = false;
        }

        private void EndStrokesCountTest()
        {
            int currentStrokeCount = _inkCanvas.Strokes.Count;
            if ( _cachedStrokesCount > currentStrokeCount )
            {
                _testStrokesDecreased = true;
            }
            else if ( _cachedStrokesCount < currentStrokeCount )
            {
                _testStrokesIncreased = true;
            }
        }

        private void VerifyStrokesDecreasted()
        {
            DRT.Assert(_testStrokesDecreased && !_testStrokesIncreased, "Failed: Expect strokes being decreased!");
        }

        private void VerifyStrokesIncreasted()
        {
            DRT.Assert(!_testStrokesDecreased && _testStrokesIncreased, "Failed: Expect strokes being increased!");
        }

        private void VerifyGesture()
        {
            DRT.Assert(_testGesture, "Failed: Expect Gesture fired!");
        }

        private void VerifyStrokesSelected()
        {
            DRT.Assert(_testSelectionChanged, "Failed: Expect selection changed!");
        }

        private void VerifyStrokesErased()
        {
            DRT.Assert(_testStrokeErased, "Failed: Expect stroke erased!");
        }

        private void VerifyStrokeCollected()
        {
            DRT.Assert(_testStrokeCollected, "Failed: Expect stroke collected!");
        }

        private void VerifyNoneGesture()
        {
            DRT.Assert(!_testGesture, "Failed: Expect no Gesture fired!");
        }

        private void VerifyNoneStrokesSelected()
        {
            DRT.Assert(!_testSelectionChanged, "Failed: Expect no selection changed!");
        }

        private void VerifyNoneStrokesErased()
        {
            DRT.Assert(!_testStrokeErased, "Failed: Expect none stroke erased!");
        }

        private void VerifyNoneStrokeCollected()
        {
            DRT.Assert(!_testStrokeCollected, "Failed: Expect none stroke collected!");
        }

        private void OnInkCanvasGesture(object sender, InkCanvasGestureEventArgs e)
        {
            ReadOnlyCollection<GestureRecognitionResult> results = e.GetGestureRecognitionResults();
            if ( results.Count != 0 && results[0].ApplicationGesture == ApplicationGesture.Right )
            {
                _testGesture = true;
            }
        }

        private void OnInkCanvasStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            _testStrokeCollected = true;
        }

        private void OnInkCanvasStrokeErased(object sender, RoutedEventArgs e)
        {
            _testStrokeErased = true;
        }

        private void OnInkCanvasSelectionChanged(object sender, EventArgs e)
        {
            _testSelectionChanged = true;
        }

        private void BeginStroke()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(90d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(90d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(120d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(150d, 150d)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        private void EndStroke()
        {
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(180d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(210d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(240d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(270d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(300d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(300d, 150d)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        private void BeginSelectStroke()
        {
            DRT.MouseButtonUp();            // just in case someone left it down

            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(90d, 90d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftDown, new Point(90d, 90d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(120d, 90d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(150d, 90d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(150d, 125d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(150d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(150d, 175d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(150d, 210d)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }

        private void EndSelectStroke()
        {
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(120d, 210d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(90d, 210d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(90d, 175d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(90d, 150d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(90d, 125d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseMove, new Point(90d, 90d)));
            _inputQueue.Enqueue(new TestInput(InputAction.MouseLeftUp, new Point(90d, 90d)));

            DRT.ResumeAt(new DrtTest(SendInputs));
        }


    }
}
