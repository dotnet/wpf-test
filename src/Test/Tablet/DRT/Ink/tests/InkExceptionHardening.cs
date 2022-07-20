// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
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
    public sealed class DrtInkExceptionHardeningTests : DrtTestSuite
    {
        private Queue<DrtTest>      _subTests;

        private DrawingAttributes   _da;
        private Guid                _guid;
        private int _onDrawingAttributesAttributeChangedCallbackCount;
        private int _onDrawingAttributesPropertyDataChangedCallbackCount;

        private IncrementalLassoHitTester _lassoHitTester;
        private int _onLassoSelectionChangedCallbackCount;

        private IncrementalStrokeHitTester _strokeHitTester;
        private int _onStrokeHitCallbackCount;

        private Stroke _stroke;
        private int _onStrokeDrawingAttributesReplacedCallbackCount;
        private int _onStrokeInvalidatedCallbackCount;
        private int _onStrokePacketsChangedCallbackCount;
        private int _onStrokePropertyDataChangedCallbackCount;
        private int _onStrokeCollectionStrokesChangedCallbackCount;

        private StrokeCollection _strokes;
        private int _onStrokeCollectionPropertyDataChangedCallbackCount;

        public DrtInkExceptionHardeningTests()
            : base("ExceptionHardeningTests")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            // initialize the suite here.  This includes loading the tree.
            _subTests = new Queue<DrtTest>();

            // return the lists of tests to run against the tree
            return new DrtTest[] { 
                    new DrtTest(Initialize),
                    // DrawingAttributes Tests
                    new DrtTest(InitializeDrawingAttributesTests),
                    new DrtTest(UninitializeDrawingAttributesTests),
                    // HitTester Tests
                    new DrtTest(InitializeHitTesterTests),
                    // Stroke Tests
                    new DrtTest(InitializeStrokeTests),
                    // StrokeCollection Tests
                    new DrtTest(InitializeStrokeCollectionTests),
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

        private void InitializeDrawingAttributesTests()
        {
            _da = new DrawingAttributes();
            _guid = Guid.NewGuid();

            _onDrawingAttributesAttributeChangedCallbackCount = 0;
            _onDrawingAttributesPropertyDataChangedCallbackCount = 0;
            _da.AttributeChanged += new PropertyDataChangedEventHandler(OnDrawingAttributesAttributeChanged);
            _da.PropertyDataChanged += new PropertyDataChangedEventHandler(OnDrawingAttributesPropertyDataChanged);

            // Change Attributes on DrawingAttributes
            _subTests.Enqueue(new DrtTest(ChangeDrawingAttributesProperty));
            _subTests.Enqueue(new DrtTest(ChangeDrawingAttributesProperty));
            _subTests.Enqueue(new DrtTest(VerifyDrawingAttributesAttributeChangedCount));
            // Change PropertyData on DrawingAttributes
            _subTests.Enqueue(new DrtTest(AddDrawingAttributesProperty));
            _subTests.Enqueue(new DrtTest(RemoveDrawingAttributesProperty));
            _subTests.Enqueue(new DrtTest(VerifyDrawingAttributesPropertyDataChangedCount));

            DRT.ResumeAt(RunTests);
        }

        private void OnDrawingAttributesAttributeChanged(object sender, PropertyDataChangedEventArgs e)
        {
            _onDrawingAttributesAttributeChangedCallbackCount++;
            throw new ReliabilityAssertException("OnDrawingAttributesAttributeChanged");
        }

        private void OnDrawingAttributesPropertyDataChanged(object sender, PropertyDataChangedEventArgs e)
        {
            _onDrawingAttributesPropertyDataChangedCallbackCount++;
            throw new ReliabilityAssertException("OnDrawingAttributesAttributeChanged");
        }

        private void UninitializeDrawingAttributesTests()
        {
            _onDrawingAttributesAttributeChangedCallbackCount = 0;
            _da.AttributeChanged -= new PropertyDataChangedEventHandler(OnDrawingAttributesAttributeChanged);
            _da.PropertyDataChanged -= new PropertyDataChangedEventHandler(OnDrawingAttributesPropertyDataChanged);
            _da = null;
        }

        private void ChangeDrawingAttributesProperty()
        {
            _da.Width = _da.Width + 10;
        }

        private void AddDrawingAttributesProperty()
        {
            _da.AddPropertyData(_guid, 0/*Dummy Value*/);
        }

        private void RemoveDrawingAttributesProperty()
        {
            _da.RemovePropertyData(_guid);
        }

        private void VerifyDrawingAttributesAttributeChangedCount()
        {
            DRT.Assert(_onDrawingAttributesAttributeChangedCallbackCount == 2, "Unexpected number of callbacks to DrawingAttributes.AttributeChanged!");
        }

        private void VerifyDrawingAttributesPropertyDataChangedCount()
        {
            DRT.Assert(_onDrawingAttributesPropertyDataChangedCallbackCount == 2, "Unexpected number of callbacks to DrawingAttributes.PropertyDataChanged!");
        }

        private void InitializeHitTesterTests()
        {
            // IncrementalLassoHitTester
            _subTests.Enqueue(new DrtTest(StartLassoHitTest));
            _subTests.Enqueue(new DrtTest(AddPoint_110_0));
            _subTests.Enqueue(new DrtTest(AddPoint_0_150));
            _subTests.Enqueue(new DrtTest(AddPoint_110_300));
            _subTests.Enqueue(new DrtTest(AddPoint_0_300));
            _subTests.Enqueue(new DrtTest(EndLassoHitTest));
            _subTests.Enqueue(new DrtTest(VerifyLassoSelectionChangedCount));
            
            // IncrementalStrokeHitTester
            _subTests.Enqueue(new DrtTest(StartStrokeHitTest));
            _subTests.Enqueue(new DrtTest(AddPoint_0_0));
            _subTests.Enqueue(new DrtTest(AddPoint_20_0));
            _subTests.Enqueue(new DrtTest(AddPoint_20_10));
            _subTests.Enqueue(new DrtTest(AddPoint_0_20));
            _subTests.Enqueue(new DrtTest(AddPoint_Remaining));
            _subTests.Enqueue(new DrtTest(EndStrokeHitTest));
            _subTests.Enqueue(new DrtTest(VerifyStrokeHitCount));
         
            DRT.ResumeAt(RunTests);
        }

        private void StartLassoHitTest()
        {
            // 1. Test a simple case (a vertical line)
            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(new Stroke(new StylusPointCollection(new Point[] { new Point(100, 100), new Point(100, 200) })));
            _lassoHitTester = strokes.GetIncrementalLassoHitTester(90);

            _lassoHitTester.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
        }

        private void AddPoint_110_0()
        {
            _lassoHitTester.AddPoint(new Point(110, 0));
        }

        private void AddPoint_0_150()
        {
            _lassoHitTester.AddPoint(new Point(0, 150));
        }

        private void AddPoint_110_300()
        {
            _lassoHitTester.AddPoint(new Point(110, 300));
        }

        private void AddPoint_0_300()
        {
            _lassoHitTester.AddPoint(new Point(0, 300));
        }

        private void EndLassoHitTest()
        {
            _lassoHitTester.EndHitTesting();
            _lassoHitTester.SelectionChanged -= new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            _lassoHitTester = null;
        }

        private void VerifyLassoSelectionChangedCount()
        {
            DRT.Assert(_onLassoSelectionChangedCallbackCount == 2, "Unexpected number of callbacks to IncrementalLassoHitTester.SelectionChanged!");
        }

        private void OnLassoSelectionChanged(object sender, LassoSelectionChangedEventArgs args)
        {
            _onLassoSelectionChangedCallbackCount++;
            throw new ReliabilityAssertException("OnDrawingAttributesAttributeChanged");
        }

        private void StartStrokeHitTest()
        {
            Point[] strokePoints = new Point[]{
                                                new Point(10f, 10f),
                                                new Point(10f, 20f),
                                                new Point(10f, 30f),
                                                new Point(10f, 40f),
                                                new Point(10f, 50f),
                                                new Point(10f, 60f),
                                                new Point(10f, 70f),
                                                new Point(10f, 80f),
                                                new Point(10f, 90f),
                                                new Point(10f, 100f)
                                              };
            Stroke stroke =  new Stroke(new StylusPointCollection(strokePoints));

            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(stroke);

            _strokeHitTester = strokes.GetIncrementalStrokeHitTester(new RectangleStylusShape(2, 2, 0));

            _strokeHitTester.StrokeHit += new StrokeHitEventHandler(OnStrokeHit);

        }

        private void AddPoint_0_0()
        {
            _strokeHitTester.AddPoint(new Point(0, 0));
        }

        private void AddPoint_20_0()
        {
            _strokeHitTester.AddPoint(new Point(20, 0));
        }

        private void AddPoint_20_10()
        {
            _strokeHitTester.AddPoint(new Point(20, 10));
        }

        private void AddPoint_0_20()
        {
            _strokeHitTester.AddPoint(new Point(0, 20));
        }

        private void AddPoint_Remaining()
        {
            _strokeHitTester.AddPoints(new Point[]{new Point(0, 21),
                                            new Point(5, 25),
                                            new Point(10, 31),
                                            new Point(20, 40)});
        }

        private void EndStrokeHitTest()
        {
            _strokeHitTester.EndHitTesting();
            _strokeHitTester.StrokeHit -= new StrokeHitEventHandler(OnStrokeHit);
            _strokeHitTester = null;
        }

        private void VerifyStrokeHitCount()
        {
            DRT.Assert(_onStrokeHitCallbackCount == 2, "Unexpected number of callbacks to IncrementalStrokeHitTester.StrokeHit!");
            _onStrokeHitCallbackCount = 0;
        }

        private void OnStrokeHit(object sender, StrokeHitEventArgs args)
        {
            _onStrokeHitCallbackCount++;
            throw new ReliabilityAssertException("OnDrawingAttributesAttributeChanged");
        }

        private void InitializeStrokeTests()
        {
            // Stroke.DrawingAttributesReplaced
            _subTests.Enqueue(new DrtTest(StartDrawingAttributesReplaceTest));
            _subTests.Enqueue(new DrtTest(ReplaceDrawingAttributes));
            _subTests.Enqueue(new DrtTest(ReplaceDrawingAttributes));
            _subTests.Enqueue(new DrtTest(VerifyDrawingAttributesReplacedCount));

            // Stroke.Invalidated
            _subTests.Enqueue(new DrtTest(StartInvaliatedTest));
            _subTests.Enqueue(new DrtTest(ReplaceDrawingAttributes));
            _subTests.Enqueue(new DrtTest(ChangeStrokeDrawingAttributes));
            _subTests.Enqueue(new DrtTest(VerifyInvalidatedCount));

            // Stroke.StylusPointsChanged
            _subTests.Enqueue(new DrtTest(StartPacketsChangedTest));
            _subTests.Enqueue(new DrtTest(ChangeStrokeStylusPackets));
            _subTests.Enqueue(new DrtTest(ChangeStrokeTransform));
            _subTests.Enqueue(new DrtTest(VerifyPacketsChangedCount));

            // Stroke.StylusPointsChanged
            _subTests.Enqueue(new DrtTest(StartPropertyDataChangedTest));
            _subTests.Enqueue(new DrtTest(AddStrokePropertyData));
            _subTests.Enqueue(new DrtTest(RemoveStrokePropertyData));
            _subTests.Enqueue(new DrtTest(VerifyStrokePropertyDataChanged));

            DRT.ResumeAt(RunTests);
        }

        private void StartDrawingAttributesReplaceTest()
        {
            _stroke = new Stroke(new StylusPointCollection(new Point[] { new Point(100, 100), new Point(100, 200) }));
            _stroke.DrawingAttributesReplaced += new DrawingAttributesReplacedEventHandler(OnDrawingAttributesReplaced);
            _onStrokeDrawingAttributesReplacedCallbackCount = 0;
        }

        private void ReplaceDrawingAttributes()
        {
            _stroke.DrawingAttributes = new DrawingAttributes();
        }

        private void VerifyDrawingAttributesReplacedCount()
        {
            DRT.Assert(_onStrokeDrawingAttributesReplacedCallbackCount == 2, "Unexpected number of callbacks to Stroke.DrawingAttributesReplaced!");
            _onStrokeDrawingAttributesReplacedCallbackCount = 0;
        }

        private void OnDrawingAttributesReplaced(object sender, DrawingAttributesReplacedEventArgs e)
        {
            _onStrokeDrawingAttributesReplacedCallbackCount++;
            throw new ReliabilityAssertException("OnDrawingAttributesAttributeChanged");
        }

        private void StartInvaliatedTest()
        {
            _stroke = new Stroke(new StylusPointCollection(new Point[] { new Point(100, 100), new Point(100, 200) }));
            _stroke.Invalidated += new EventHandler(OnInvalidated);
            _onStrokeInvalidatedCallbackCount = 0;
        }

        private void ChangeStrokeDrawingAttributes()
        {
            _stroke.DrawingAttributes.Color = Colors.Red;
        }

        private void VerifyInvalidatedCount()
        {
            DRT.Assert(_onStrokeInvalidatedCallbackCount == 2, "Unexpected number of callbacks to Stroke.Invalidated!");
            _onStrokeInvalidatedCallbackCount = 0;
        }

        private void OnInvalidated(object sender, EventArgs e)
        {
            _onStrokeInvalidatedCallbackCount++;
            throw new ReliabilityAssertException("OnInvalidated");
        }

        private void StartPacketsChangedTest()
        {
            _stroke = new Stroke(new StylusPointCollection(new Point[] { new Point(100, 100), new Point(100, 200) }));
            _stroke.StylusPointsChanged += new EventHandler(OnStylusPointsChanged);
            _onStrokePacketsChangedCallbackCount = 0;
        }

        private void ChangeStrokeStylusPackets()
        {
            _stroke.StylusPoints = new StylusPointCollection(new Point[] { new Point(0, 0), new Point(300, 200) });
        }

        private void ChangeStrokeTransform()
        {
            _stroke.Transform(new Matrix(0.5d, 0d, 0d, 0.5d, 0d, 0d), false);
        }

        private void VerifyPacketsChangedCount()
        {
            DRT.Assert(_onStrokePacketsChangedCallbackCount == 1, "Unexpected number of callbacks to Stroke.StylusPointsChanged!  Expected 2, got " + _onStrokePacketsChangedCallbackCount.ToString());
            _onStrokePacketsChangedCallbackCount = 0;
        }

        private void OnStylusPointsChanged(object sender, EventArgs e)
        {
            _onStrokePacketsChangedCallbackCount++;
            throw new ReliabilityAssertException("OnStylusPointsChanged");
        }

        private void StartPropertyDataChangedTest()
        {
            _stroke = new Stroke(new StylusPointCollection(new Point[] { new Point(100, 100), new Point(100, 200) }));
            _stroke.PropertyDataChanged += new PropertyDataChangedEventHandler(OnStrokePropertyDataChanged);
            _onStrokePropertyDataChangedCallbackCount = 0;
        }

        private void AddStrokePropertyData()
        {
            _stroke.AddPropertyData(_guid, 0);
        }

        private void RemoveStrokePropertyData()
        {
            _stroke.RemovePropertyData(_guid);
        }

        private void VerifyStrokePropertyDataChanged()
        {
            DRT.Assert(_onStrokePropertyDataChangedCallbackCount == 2, "Unexpected number of callbacks to Stroke.PropertyDataChanged!");
            _onStrokePropertyDataChangedCallbackCount = 0;
        }

        private void OnStrokePropertyDataChanged(object sender, PropertyDataChangedEventArgs e)
        {
            _onStrokePropertyDataChangedCallbackCount++;
            throw new ReliabilityAssertException("OnStrokePropertyDataChanged");
        }

        private void InitializeStrokeCollectionTests()
        {
            // StrokeCollection.PropertyDataChanged
            _subTests.Enqueue(new DrtTest(StartStrokeCollectionPropertyDataChangedTests));
            _subTests.Enqueue(new DrtTest(AddStrokeCollectionPropertyData));
            _subTests.Enqueue(new DrtTest(RemoveStrokeCollectionPropertyData));
            _subTests.Enqueue(new DrtTest(VerifyStrokeCollectionPropertyDataChanged));

            // StrokeCollection.StrokesChanged
            _subTests.Enqueue(new DrtTest(StartStrokeCollectionStrokesChangedTests));
            _subTests.Enqueue(new DrtTest(AddStrokeToStrokeCollection));
            _subTests.Enqueue(new DrtTest(InsertStrokeToStrokeCollection));
            _subTests.Enqueue(new DrtTest(RemoveStrokeFromStrokeCollection));
            _subTests.Enqueue(new DrtTest(VerifyStrokeCollectionStrokesChanged));

            DRT.ResumeAt(RunTests);
        }

        private void StartStrokeCollectionPropertyDataChangedTests()
        {
            _strokes = new StrokeCollection();
            _strokes.PropertyDataChanged += new PropertyDataChangedEventHandler(OnStrokeCollectionPropertyDataChanged);
            _onStrokeCollectionPropertyDataChangedCallbackCount = 0;
        }

        private void AddStrokeCollectionPropertyData()
        {
            _strokes.AddPropertyData(_guid, 0);
        }

        private void RemoveStrokeCollectionPropertyData()
        {
            _strokes.RemovePropertyData(_guid);
        }

        private void VerifyStrokeCollectionPropertyDataChanged()
        {
            DRT.Assert(_onStrokeCollectionPropertyDataChangedCallbackCount == 2, "Unexpected number of callbacks to StrokeCollection.PropertyDataChanged!");
            _onStrokeCollectionPropertyDataChangedCallbackCount = 0;
        }

        private void OnStrokeCollectionPropertyDataChanged(object sender, PropertyDataChangedEventArgs e)
        {
            _onStrokeCollectionPropertyDataChangedCallbackCount++;
            throw new ReliabilityAssertException("OnStrokePropertyDataChanged");
        }

        private void StartStrokeCollectionStrokesChangedTests()
        {
            _strokes = new StrokeCollection();
            _strokes.StrokesChanged += new StrokeCollectionChangedEventHandler(OnStrokeCollectionStrokesChanged);
            _onStrokeCollectionStrokesChangedCallbackCount = 0;
        }

        private void AddStrokeToStrokeCollection()
        {
            _strokes.Add(new Stroke(new StylusPointCollection(new Point[]{new Point(0, 0), new Point(100, 100)})));
        }

        private void InsertStrokeToStrokeCollection()
        {
            _strokes.Insert(0, new Stroke(new StylusPointCollection(new Point[] { new Point(200, 0), new Point(100, 100) })));
        }

        private void RemoveStrokeFromStrokeCollection()
        {
            _strokes.RemoveAt(0);
        }

        private void VerifyStrokeCollectionStrokesChanged()
        {
            DRT.Assert(_onStrokeCollectionStrokesChangedCallbackCount == 3, "Unexpected number of callbacks to StrokeCollection.StrokesChanged!");
            _onStrokeCollectionStrokesChangedCallbackCount = 0;
        }

        private void OnStrokeCollectionStrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            _onStrokeCollectionStrokesChangedCallbackCount++;
            throw new ReliabilityAssertException("OnStrokeCollectionPropertyDataChanged");
        }


    }
}
