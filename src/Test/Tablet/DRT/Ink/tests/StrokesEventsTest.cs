// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace DRT
{
    /// <summary>
    /// Summary description for StrokesEventsTest.
    /// </summary>
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class StrokesEventsTest : DrtInkTestcase
    {
        System.Collections.Generic.Queue<StrokeCollectionChangedEventArgs> _queue = new System.Collections.Generic.Queue<StrokeCollectionChangedEventArgs>();
        StrokeCollection _lastAdded;
        StrokeCollection _lastRemoved;
        StrokeCollection _strokes = new StrokeCollection();
        StrokeCollection _strokesOther = new StrokeCollection();
        StrokeCollection _strokesReplaceWith = new StrokeCollection();
        StrokeCollection _strokesToReplace = new StrokeCollection();
        bool _caughtException;


        public override void Run ()
        {
            TestStrokeCollectionEvents();

            TestStrokeEvents();

            TestStrokeCollectionChangedReadOnly();

            Success = true;
        }

        private void TestStrokeCollectionChangedReadOnly()
        {
            StrokeCollectionChangedEventHandler scChanged =
                delegate(object sender, StrokeCollectionChangedEventArgs args)
                {
                    if (args.Added.Count != 1 ||
                        args.Removed.Count != 0 ||
                        !((ICollection<Stroke>)args.Added).IsReadOnly ||
                        !((IList)args.Added).IsReadOnly ||
                        !((ICollection<Stroke>)args.Removed).IsReadOnly ||
                        !((IList)args.Removed).IsReadOnly)
                    {
                        throw new InvalidOperationException("Unexpected IsReadOnly values on StrokeCollection changed event args");
                    }

                    int exceptionCount = 0;
                    try
                    {
                        args.Added.Add(new Stroke(new StylusPointCollection(new Point[] { new Point(10, 10) })));
                    }
                    catch (NotSupportedException)
                    {
                        exceptionCount++;
                    }

                    if (exceptionCount != 1)
                    {
                        throw new InvalidOperationException("StrokeCollection event args failed to throw when modified");
                    }

                    try
                    {
                        args.Removed.Add(new Stroke(new StylusPointCollection(new Point[] { new Point(10, 10) })));
                    }
                    catch (NotSupportedException)
                    {
                        exceptionCount++;
                    }

                    if (exceptionCount != 2)
                    {
                        throw new InvalidOperationException("StrokeCollection event args failed to throw when modified");
                    }
                };

            StrokeCollection strokes = new StrokeCollection();
            strokes.StrokesChanged += scChanged;

            strokes.Add(new Stroke(new StylusPointCollection(new Point[] { new Point(10, 10) })));
        }

        private void TestStrokeEvents()
        {
            int eventCount = 0;
            int expectedEventCount = 0;

            Stroke stroke = new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(10f, 10f), new StylusPoint(20f, 20f) }));
            DrawingAttributes orginalDrawingAttributes = stroke.DrawingAttributes;
            DrawingAttributes newDrawingAttributes = new DrawingAttributes();
            newDrawingAttributes.Color = Colors.Blue;

            Guid daChangedGuid = Guid.Empty;
            object oldDaChangedValue = null;
            object newDaChangedValue = null;

            DrawingAttributes daReplacedNewDrawingAttributes = null;
            DrawingAttributes daReplacedPreviousDrawingAttributes = null;

            Stroke stroke2 = null;


            //
            // DrawingAttributesChanged anon delegate
            //
            PropertyDataChangedEventHandler daChangedAnonymousDelegate =
                delegate(object sender, PropertyDataChangedEventArgs args)
            {
                eventCount++;
                daChangedGuid = args.PropertyGuid;
                oldDaChangedValue = args.PreviousValue;
                newDaChangedValue = args.NewValue;
            };

            //
            // DrawingAttributesReplaced anon delegate
            //
            DrawingAttributesReplacedEventHandler daReplacedAnonymousDelegate =
                delegate(object sender, DrawingAttributesReplacedEventArgs args)
            {
                eventCount++;
                daReplacedNewDrawingAttributes = args.NewDrawingAttributes;
                daReplacedPreviousDrawingAttributes = args.PreviousDrawingAttributes;
            };

            //
            // StylusPointsChanged anon delegate
            //
            EventHandler stylusPointsChangedAnonymousDelegate =
                delegate(object sender, EventArgs args)
            {
                eventCount++;
            };

            //
            // PropertyDataChanged anon delegate
            //
            PropertyDataChangedEventHandler propDataChangedAnonymousDelegate =
                delegate(object sender, PropertyDataChangedEventArgs args)
            {
                eventCount++;

            };

            //
            // Invalidated anon delegate
            //
            EventHandler invalidatedAnonymousDelegate =
                delegate(object sender, EventArgs args)
            {
                eventCount++;
            };



            try
            {

                //
                // listen up
                //
                stroke.DrawingAttributesChanged += daChangedAnonymousDelegate;
                stroke.DrawingAttributesReplaced += daReplacedAnonymousDelegate;
                stroke.StylusPoints.Changed += stylusPointsChangedAnonymousDelegate;
                stroke.PropertyDataChanged += propDataChangedAnonymousDelegate;
                stroke.Invalidated += invalidatedAnonymousDelegate;

                //
                // start the test
                //

                //
                // 1) Test Stroke.DrawingAttributesChanged
                //
                expectedEventCount += 2;    //DrawingAttributesChanged and Invalidated
                stroke.DrawingAttributes.Color = Colors.Red;

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke.DrawingAttributesChanged or Invalidated");
                }
                if (daChangedGuid != DrawingAttributeIds.Color)
                {
                    throw new InvalidOperationException("Didn't get the right PropertyGuid in the stroke.DrawingAttributesChanged event");
                }
                if (((Color)oldDaChangedValue) != Colors.Black)
                {
                    throw new InvalidOperationException("Didn't get the right PreviousValue in the stroke.DrawingAttributesChanged event");
                }
                if (((Color)newDaChangedValue) != Colors.Red)
                {
                    throw new InvalidOperationException("Didn't get the right NewValue in the stroke.DrawingAttributesChanged event");
                }


                //
                // 2) Test Stroke.DrawingAttributesReplaced
                //
                expectedEventCount += 2;    //DrawingAttributesChanged and Invalidated
                stroke.DrawingAttributes = newDrawingAttributes;

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke.DrawingAttributesReplaced or Invalidated");
                }
                if (!Object.ReferenceEquals(daReplacedNewDrawingAttributes, newDrawingAttributes))
                {
                    throw new InvalidOperationException("Didn't get the right new DrawingAttributes in the stroke.DrawingAttributesReplaced event");
                }
                if (!Object.ReferenceEquals(daReplacedPreviousDrawingAttributes, orginalDrawingAttributes))
                {
                    throw new InvalidOperationException("Didn't get the right previous DrawingAttributes in the stroke.DrawingAttributesReplaced event");
                }

                //
                // 3) Test Stroke.StylusPointsChanged
                //
                expectedEventCount += 2;    //StylusPointsChanged and Invalidated
                stroke.StylusPoints.Add(new StylusPoint(1,3));

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke.StylusPointsChanged or Invalidated");
                }


                expectedEventCount += 0;        // No event should be filed if the Matrix is Identity
                stroke.Transform(Matrix.Identity, true);

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Got an unexpected event for calling Stroke.Transform with identity");
                }

                expectedEventCount += 2;        //StylusPointsChanged and Invalidated
                Matrix matrix = Matrix.Identity;
                matrix.Scale(2, 3);
                stroke.Transform(matrix, false);

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke.StylusPointsChanged or Invalidated");
                }

                // StylusPointsChanged, Invalidated and DA changed
                // Note: Invalidated event will fire once if matrix != Matrix.Identity.
                expectedEventCount += 3;
                stroke.Transform(matrix, true);   // This will change DA.StylusTipTransform

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke.StylusPointsChanged, Invalidated or DAChanged");
                }


                //
                // clone the stroke and keep testing
                //
                stroke2 = stroke.Clone();
                stroke2.DrawingAttributesChanged += daChangedAnonymousDelegate;
                stroke2.DrawingAttributesReplaced += daReplacedAnonymousDelegate;
                stroke2.StylusPoints.Changed += stylusPointsChangedAnonymousDelegate;
                stroke2.PropertyDataChanged += propDataChangedAnonymousDelegate;
                stroke2.Invalidated += invalidatedAnonymousDelegate;


                orginalDrawingAttributes = stroke2.DrawingAttributes;
                newDrawingAttributes = new DrawingAttributes();

                //
                // 5) Test Stroke.DrawingAttributesChanged
                //
                expectedEventCount += 2;    //DrawingAttributesChanged and Invalidated
                stroke2.DrawingAttributes.Color = Colors.Red;

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke2.DrawingAttributesChanged or Invalidated");
                }
                if (daChangedGuid != DrawingAttributeIds.Color)
                {
                    throw new InvalidOperationException("Didn't get the right PropertyGuid in the stroke2.DrawingAttributesChanged event");
                }
                if (((Color)oldDaChangedValue) != Colors.Blue)
                {
                    throw new InvalidOperationException("Didn't get the right PreviousValue in the stroke2.DrawingAttributesChanged event");
                }
                if (((Color)newDaChangedValue) != Colors.Red)
                {
                    throw new InvalidOperationException("Didn't get the right NewValue in the stroke.DrawingAttributesChanged event");
                }


                //
                // 6) Test Stroke.DrawingAttributesReplaced
                //
                expectedEventCount += 2;    //DrawingAttributesChanged and Invalidated
                stroke2.DrawingAttributes = newDrawingAttributes;

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke2.DrawingAttributesReplaced or Invalidated");
                }
                if (!Object.ReferenceEquals(daReplacedNewDrawingAttributes, newDrawingAttributes))
                {
                    throw new InvalidOperationException("Didn't get the right new DrawingAttributes in the stroke2.DrawingAttributesReplaced event");
                }
                if (!Object.ReferenceEquals(daReplacedPreviousDrawingAttributes, orginalDrawingAttributes))
                {
                    throw new InvalidOperationException("Didn't get the right previous DrawingAttributes in the stroke2.DrawingAttributesReplaced event");
                }

                //
                // 7) Test Stroke.StylusPointsChanged
                //
                expectedEventCount += 2;    //StylusPointsChanged and Invalidated
                stroke2.StylusPoints.Add(new StylusPoint(1, 3));

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke.StylusPointsChanged or Invalidated on cloned stroke");
                }


                expectedEventCount += 0;        // No event should be filed if the Matrix is Identity
                stroke2.Transform(Matrix.Identity, true);

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Got an unexpected event for calling Stroke.Transform with identity on cloned stroke");
                }

                expectedEventCount += 2;        //StylusPointsChanged and Invalidated
                Matrix matrix2 = Matrix.Identity;
                matrix2.Scale(2, 3);
                stroke2.Transform(matrix2, false);

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke.StylusPointsChanged or Invalidated on cloned stroke");
                }

                // StylusPointsChanged, Invalidated and DA changed
                // Note: Invalidated event will fire once if matrix != Matrix.Identity.
                expectedEventCount += 3;
                stroke2.Transform(matrix2, true);   // This will change DA.StylusTipTransform

                if (expectedEventCount != eventCount)
                {
                    throw new InvalidOperationException("Didn't get an event for stroke.StylusPointsChanged, Invalidated or DAChanged on cloned stroke");
                }


            }
            finally
            {
                if (stroke != null)
                {
                    stroke.DrawingAttributesChanged -= daChangedAnonymousDelegate;
                    stroke.DrawingAttributesReplaced -= daReplacedAnonymousDelegate;
                    stroke.StylusPoints.Changed -= stylusPointsChangedAnonymousDelegate;
                    stroke.PropertyDataChanged -= propDataChangedAnonymousDelegate;
                    stroke.Invalidated -= invalidatedAnonymousDelegate;
                }

                if (stroke2 != null)
                {
                    stroke2.DrawingAttributesChanged -= daChangedAnonymousDelegate;
                    stroke2.DrawingAttributesReplaced -= daReplacedAnonymousDelegate;
                    stroke2.StylusPoints.Changed -= stylusPointsChangedAnonymousDelegate;
                    stroke2.PropertyDataChanged -= propDataChangedAnonymousDelegate;
                    stroke2.Invalidated -= invalidatedAnonymousDelegate;
                }
            }
        }

        private void TestStrokeCollectionEvents()
        {
            StrokeCollectionChangedEventArgs args;

            _strokesOther.Add(new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(1, 1), new StylusPoint(1, 1) })));
            _strokesOther.Add(new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(2, 2), new StylusPoint(2, 2) })));
            _strokesOther.Add(new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(3, 3), new StylusPoint(3, 3) })));

            _strokesReplaceWith.Add(new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(4, 4), new StylusPoint(4, 4) })));
            _strokesReplaceWith.Add(new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(5, 5), new StylusPoint(5, 5) })));

            _strokes.StrokesChanged += new StrokeCollectionChangedEventHandler(strokesChanged);

            ////////////////////////////////////////////////////////
            // Test 1:  Perform single stroke add operation
            ////////////////////////////////////////////////////////
            ResetStrokes(false);
            _strokes.Add(_strokesOther[0]);
            if (!_lastAdded.Contains(_strokesOther[0]))
            {
                throw new InvalidOperationException("Added stroke missing");
            }

            ////////////////////////////////////////////////////////
            // Test 2:  Perform single stroke remove operation
            ////////////////////////////////////////////////////////
            _strokes.Remove(_strokesOther[0]);
            if (!_lastRemoved.Contains(_strokesOther[0]))
            {
                throw new InvalidOperationException("Removed stroke missing");
            }

            ////////////////////////////////////////////////////////
            // Test 3:  Perform multiple stroke add operation
            ////////////////////////////////////////////////////////
            ResetStrokes(false);
            _strokes.Add(_strokesOther);

            // Fire a single event.
            DRT.Assert(_queue.Count == 1);
            args = _queue.Dequeue();

            for (int i = 0; i < 3; i++)
            {
                if (!args.Added.Contains(_strokesOther[i]))
                {
                    throw new InvalidOperationException("Added _strokes missing");
                }
            }
            foreach (Stroke s in _strokesOther)
            {
                if (!_lastAdded.Contains(s))
                {
                    throw new InvalidOperationException("Added _strokes missing");
                }
            }


            ////////////////////////////////////////////////////////
            // Test 4:  Perform multiple stroke remove operation
            ////////////////////////////////////////////////////////
            _strokes.Remove(_strokesOther);
            args = _queue.Dequeue();
            for (int i = 0; i < 3; i++)
            {
                if (!args.Removed.Contains(_strokesOther[i]))
                {
                    throw new InvalidOperationException("Added _strokes missing");
                }
            }

            foreach (Stroke s in _strokesOther)
            {
                if (!_lastRemoved.Contains(s))
                {
                    throw new InvalidOperationException("Added _strokes missing");
                }
            }

            ////////////////////////////////////////////////////////
            // Test 5:  Remove(this) test.
            ////////////////////////////////////////////////////////
            _strokes.Remove(_strokes);
            if (_strokes.Count != 0)
            {
                throw new InvalidOperationException("Remove this failed");
            }
            foreach (Stroke s in _strokesOther)
            {
                if (!_lastRemoved.Contains(s))
                {
                    throw new InvalidOperationException("Removed _strokes missing");
                }
            }

            ////////////////////////////////////////////////////////
            // Test 6: Add existing single stroke
            ////////////////////////////////////////////////////////
            ResetStrokes(true);
            _strokes.Add(_strokesOther[0]);
            try
            {
                _strokes.Add(_strokesOther[0]);
            }
            catch (ArgumentException)
            {
                // We expect an ArgumentException being thrown.
                _caughtException = true;
            }
            if ( _caughtException == false)
            {
                throw new InvalidOperationException("Added incorrectly fired");
            }

            ////////////////////////////////////////////////////////
            // Test 7: Add existing multiple _strokes
            ////////////////////////////////////////////////////////
            try
            {
                _strokes.Add(_strokesOther);
            }
            catch (ArgumentException)
            {
                // We expect an ArgumentException being thrown.
                _caughtException = true;
            }
            if (_caughtException == false)
            {
                throw new InvalidOperationException("Added incorrectly fired");
            }

            ////////////////////////////////////////////////////////
            // Test 8: Test out of order remove
            ////////////////////////////////////////////////////////
            StrokeCollection sc = new StrokeCollection();
            StrokeCollection scToRemove = new StrokeCollection();
            sc.Add(_strokesOther[0]);
            sc.Add(_strokesOther[1]);
            sc.Add(_strokesOther[2]);
            sc.Add(_strokesReplaceWith[0]);
            sc.Add(_strokesReplaceWith[1]);

            scToRemove.Add(_strokesOther[2]);
            scToRemove.Add(_strokesOther[0]);
            scToRemove.Add(_strokesOther[1]);
            scToRemove.Add(_strokesReplaceWith[1]);

            sc.Remove(scToRemove);
            if (sc.Count != 1)
            {
                throw new InvalidOperationException("Out of order Remove failed");
            }


            ////////////////////////////////////////////////////////
            // Test 9: Remove non-contained _strokes
            ////////////////////////////////////////////////////////
            ResetStrokes(false);
            try
            {
                _strokes.Remove(_strokesOther);
            }
            catch (ArgumentException)
            {
                // We expect an ArgumentException being thrown.
                _caughtException = true;
            }
            if (_lastRemoved != null || _caughtException == false)
            {
                throw new InvalidOperationException("Removed incorrectly fired");
            }

            ////////////////////////////////////////////////////////
            // Test 10: Replace(Stroke, StrokeCollection)
            ////////////////////////////////////////////////////////
            _strokes.Add(_strokesOther);
            _strokes.Replace(_strokes[1], _strokesReplaceWith);
            if (_strokes.Count != 4)
            {
                throw new InvalidOperationException("Replace failed to replace _strokes[1] with _strokesReplaceWith");
            }
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        if (_strokes[i] != _strokesOther[0])
                        {
                            throw new InvalidOperationException("Replace failed to replace _strokes[1] with _strokesReplaceWith");
                        }
                        break;
                    case 1:
                        if (_strokes[i] != _strokesReplaceWith[0])
                        {
                            throw new InvalidOperationException("Replace failed to replace _strokes[1] with _strokesReplaceWith");
                        }
                        break;
                    case 2:
                        if (_strokes[i] != _strokesReplaceWith[1])
                        {
                            throw new InvalidOperationException("Replace failed to replace _strokes[1] with _strokesReplaceWith");
                        }
                        break;
                    case 3:
                        if (_strokes[i] != _strokesOther[2])
                        {
                            throw new InvalidOperationException("Replace failed to replace _strokes[1] with _strokesReplaceWith");
                        }
                        break;
                }
            }
            if (!_lastRemoved.Contains(_strokesOther[1]))
            {
                throw new InvalidOperationException("Removed stroke missing");
            }
            foreach (Stroke s in _strokesReplaceWith)
            {
                if (!_lastAdded.Contains(s))
                {
                    throw new InvalidOperationException("_strokes added is invalid");
                }
            }
/*
            ////////////////////////////////////////////////////////
            // Test 11: Replace(StrokeCollection, StrokeCollection)
            ////////////////////////////////////////////////////////
           // ResetStrokes(true);

            _strokesToReplace.Clear();
            _strokesToReplace.Add(_strokes[1]);
            _strokesToReplace.Add(_strokes[2]);
            _strokes.Replace(_strokesToReplace, _strokesReplaceWith);
            if (_strokes.Count != 3)
            {
                throw new InvalidOperationException("Replace failed to replace _strokes[1-2] with _strokesReplaceWith");
            }
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        if (_strokes[i] != _strokesOther[0])
                        {
                            throw new InvalidOperationException("Replace failed to replace _strokes[1-2] with _strokesReplaceWith");
                        }
                        break;
                    case 1:
                        if (_strokes[i] != _strokesReplaceWith[0])
                        {
                            throw new InvalidOperationException("Replace failed to replace _strokes[1-2] with _strokesReplaceWith");
                        }
                        break;
                    case 2:
                        if (_strokes[i] != _strokesReplaceWith[1])
                        {
                            throw new InvalidOperationException("Replace failed to replace _strokes[1-2] with _strokesReplaceWith");
                        }
                        break;
                }
            }
            if (!_lastRemoved.Contains(_strokesOther[1]) || !_lastRemoved.Contains(_strokesOther[2]))
            {
                throw new InvalidOperationException("Removed stroke missing");
            }
            if (!_lastRemoved.Contains(_strokesOther[2]))
            {
                throw new InvalidOperationException("Removed stroke missing");
            }
            foreach (Stroke s in _strokesReplaceWith)
            {
                if (!_lastAdded.Contains(s))
                {
                    throw new InvalidOperationException("_strokes removed is invalid");
                }
            }

            ////////////////////////////////////////////////////////
            // Test 12: Replace(StrokeCollection, StrokeCollection) failure
            ////////////////////////////////////////////////////////
            ResetStrokes(true);
            _strokesToReplace.Add(_strokes[0]);
            _strokesToReplace.Add(_strokes[2]);

            try
            {
                _strokes.Replace(_strokesToReplace, _strokesReplaceWith);
            }
            catch (ArgumentException)
            {
                _caughtException = true;
            }
            if (!_caughtException || _lastAdded != null || _lastRemoved != null)
            {
                throw new InvalidOperationException("Replace failure test failed");
            }
            for (int i = 0; i < _strokes.Count; i++)
            {
                if (_strokes[i] != _strokesOther[i])
                {
                    throw new InvalidOperationException("Replace failure test failed");
                }
            }
*/
            ////////////////////////////////////////////////////////
            // Test 13: Clear _strokes
            ////////////////////////////////////////////////////////
            ResetStrokes(true);
            _strokes.Clear();
            if (_strokes.Count != 0)
            {
                throw new InvalidOperationException("Clear failed to remove _strokes");
            }
        }

        private void strokesChanged (object sender, StrokeCollectionChangedEventArgs e)
        {
            _queue.Enqueue(e);
            _lastAdded = new StrokeCollection(e.Added);
            _lastRemoved = new StrokeCollection(e.Removed);
        }

        void ResetStrokes (bool all)
        {
            if (all)
            {
                _strokes = _strokesOther.Clone();
            }
            else
            {
                _strokes = new StrokeCollection();
            }
            _strokes.StrokesChanged +=new StrokeCollectionChangedEventHandler(strokesChanged);
            _lastAdded = null;
            _lastRemoved = null;
            _caughtException = false;
            _queue.Clear();
            _strokesToReplace.Clear();
        }
    }
}
