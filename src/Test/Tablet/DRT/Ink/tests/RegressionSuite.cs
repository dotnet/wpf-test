// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Ink.Internal;
using System.IO;
using System.Diagnostics;
using System.Windows.Input;

namespace DRT
{
    /// <summary>
    /// Summary description for RegressionSuite.
    /// </summary>
    [TestedSecurityLevelAttribute(SecurityLevel.PartialTrust)]
    public class RegressionSuite : DrtInkTestcase
    {
        public override void Run()
        {

            Trace.WriteLine("Verifying Stroke.Copy");
            Stroke_Copy();

#if PORT_FINISHED

            Trace.WriteLine("Verifying TabletPCBugs: BuildTransformTalbe corrupts ISF");
            TestBuildTransformTableCorruptsISF();


            Trace.WriteLine ("Verifying TabletPCBugs: clip returns EmptyCollection instead of Null");
            TestClipReturnEmptyCollectionInsteadOfNull();

            Trace.WriteLine ("Verifying TabletPCBugs: ICollection.CopyTo should support loosely typed arrays");
            TestCopyToSupportLooselyTypedArrays();

            Trace.WriteLine ("Verifying TabletPCBugs: NearestPoint returns inverted FIndex ");
            try
            {
                TestNearestPointReturnsInvertedFIndex();
            }
            catch (InvalidOperationException)
            {
                DRT.Trace ("\"TabletPCBugs NearestPoint returns inverted FIndex\" has not been fixed");
            }

            Trace.WriteLine ("Verifying TabletPCBugs: Base64 stream is unsaveable");
            TestBase64StreamIsUnsaveable();

            Trace.WriteLine ("Verifying TabletPCBugs: Clip returns bogus data");
            TestClipReturnsBogusData();

            Trace.WriteLine ("Verifying TabletPCBugs: AddStrokesAtRectangle clips wrong strokes ");
            TestAddStrokesAtRectangleClipsWrongStrokes();

            Trace.WriteLine ("Verifying TabletPCBugs: AddStrokesAtRectangle corrupted with Undo ");
            TestAddStrokesAtRectangleCorruptedWithUndo();

            Trace.WriteLine ("Verifying TabletPCBugs: AddStrokesAtRectangle doesn't copy Stroke ExtendedProperties");
            TestAddStrokesAtRectangleDoesntCopyStrokeExtendedProperties();

            Trace.WriteLine ("Verifying TabletPCBugs: Dirty flag is true after load");
            TestDirtyFlagIsTrueAfterLoad();

            Trace.WriteLine ("Verifying TabletPCBugs: Null exception after Undo and save");
            TestNullExceptionAfterUndoAndSave();

            Trace.WriteLine ("Verifying TabletPCBugs: Clip incorrectly undone");
            try
            {
                TestClipIncorrectlyUndone();
            }
            catch (InvalidOperationException)
            {
                DRT.Trace ("\"Verifying TabletPCBugs: Clip incorrectly undone\" has not been fixed");
            }

            Trace.WriteLine ("Verifying TabletPCBugs: ExtractStrokes extracts wrong strokes");
            TextExtractStrokesExtractsWrongStrokes();

            try
            {
                Trace.WriteLine ("Verifying TabletPCBugs: HitTest with lasso hits wrong stroke");
                TestHitTestWithLassoHitsWrongStroke();
            }
            catch (InvalidOperationException)
            {
                DRT.Trace("\"Verifying TabletPCBugs: HitTest with lasso hits wrong stroke\" has not been fixed");
            }

            try
            {
                Trace.WriteLine ("Verifying TabletPCBugs: HitTest with lasso returns no strokes");
                TestHitTestWithLassoReturnsNoStrokes();
            }
            catch (InvalidOperationException)
            {
                DRT.Trace("\"Verifying TabletPCBugs: HitTest with lasso returns no strokes\" has regressed");
            }

            try
            {
                Trace.WriteLine("Verifying TabletPCBugs: Round-tripping ISF via v2 Ink causes scaling");
                TestRoundTrippingISFCausesScalling();
            }
            catch (FileNotFoundException)
            {
                DRT.Trace("Missing Microsoft.Ink");
            }

            Trace.WriteLine ("Verifying TabletPCBugs: Undo fails for ExtractStrokes (deleted)");
            TestUndoFailsForExtractStrokes();

            Trace.WriteLine ("Verifying TabletPCBugs: Undo/Redo fails for ExtractStrokes");
            TestUndoRedoFailsForExtractStrokes();

            try
            {
                Trace.WriteLine("Verifying TabletPCBugs: HitTest with lasso returns no strokes 2");
                TestHitTestWithLassoReturnsNoStrokes2();
            }
            catch (FileNotFoundException)
            {
                DRT.Trace("Missing Microsoft.Ink");
            }

            Trace.WriteLine ("Verifying TabletPCBugs: ExtractStrokes results in odd results");
            TestExtractStrokesResultsInOddResults();

            Trace.WriteLine ("Verifying TabletPCBugs: Undo fails for Width Transform");
            TestUndoFailsForWidthTransform();

            Trace.WriteLine ("Verifying TabletPCBugs: Inconsistent edge clipping for fully contained strokes");
            TestInconsistentEdgeClippingForFullyContainedStrokes();

            Trace.WriteLine ("Verifying TabletPCBugs: Verify Ink can be used in partial trust scenario");
            TestVerifyInkCanBeUsedInPartialTrustScenario();

            Trace.WriteLine ("Verifying TabletPCBugs: ScaleToRectangle mishandles pen width");
            TestScaleToRectangleMishandlesPenWidth();

            try
            {
                Trace.WriteLine ("Verifying TabletPCBugs: Extract fails with true flags");
                TestExtractFailsWithTrueFlag();
            }
            catch (InvalidOperationException)
            {
                DRT.Trace("\"Verifying TabletPCBugs: Extract fails with true flags\" has not been fixed");
            }

            Trace.WriteLine ("Verifying TabletPCBugs: DrawingAttributes does not dirty Ink");
            TestDrawingAttributesDoesNotDirtyInk();

            Trace.WriteLine("Verifying TabletPCBugs: ExtendedProperties IndexOf fails");
            TestExtendedPropertiesIndexOfFails();

            Trace.WriteLine("Verifying TabletPCBugs: ExtendedProperties.Remove doesn't disconnect events");
            TestExtendedPropertiesRemoveDoesntDisconnectEvents();

            Trace.WriteLine("Verifying TabletPCBugs: Multiple ExtendedProperties not roundtripped in Stroke DA");
            TestMultipleExtendedPropertiesNotRoundtrippedInStrokeDA();

#endif
            Trace.WriteLine("Verifying TabletPCBugs: Error occurs when deserializing InkCanvas data");
            TestErrorOccursWhenDeserializingInkCanvasData();

            Trace.WriteLine("Verifying TabletPCBugs: Error occurs when inserting existing item at end of single item list");
            try
            {
                TestErrorWhenInsertingExistingItemAtEndOfSingleItemList();
            }
            catch (ArgumentException)
            {
                DRT.Trace("\"Verifying TabletPCBugs: Error occurs when inserting existing item at end of single item list\" has been fixed");
            }

            Trace.WriteLine("Verifying TabletPCBugs: Error occurs when inserting existing item before later item in 2 item list");
            try
            {
                TestErrorOccursWhenInsertingExistingItemBeforeLaterItemIn2ItemList();
            }
            catch (ArgumentException)
            {
                DRT.Trace("\"Verifying TabletPCBugs: Error occurs when inserting existing item before later item in 2 item list\" has been fixed");
            }

            Trace.WriteLine("Verifying TabletPCBugs: Error occurs when inserting existing item before later item in 2 item list (b)");
            try
            {
                TestErrorOccursWhenInsertingExistingItemBeforeLaterItemIn2ItemListB();
            }
            catch (ArgumentException)
            {
                DRT.Trace("\"Verifying TabletPCBugs: Error occurs when inserting existing item before later item in 2 item list (b)\" has been fixed");
            }

            Trace.WriteLine("Verifying TabletPCBugs Error occurs when replacing last item");
            TestErrorOccursWhenReplacingLastItem();

            Trace.WriteLine("Verifying TabletPCBugs: Invalid state occurs after replace");
            TestInvalidStateOccursAfterReplace();

            Trace.WriteLine("Verifying TabletPCBugs: Invalid state occurs after replace (b)");
            TestInvalidStateOccursAfterReplaceB();

            //Trace.WriteLine("Verifying TabletPCBugs: TestStrokesRotated");
            //TestStrokesRotated();

            Trace.WriteLine("Verifying TabletPCBugs: Invalid exception thrown by CopyTo");
            TestInvalidExceptionThrownByCopyTo();

            Trace.WriteLine("Verifying TabletPCBugs: Incorrect events fire for index setter");
            TestIncorrectEventsFireForIndexSetter();

            //            Trace.WriteLine("Verifying TabletPCBugs: Create a stroke with button info and save it");
            //            TestCreateAStrokeWithButtonInfoAndSaveIt()

            Trace.WriteLine("Verifying WindowsOSBugs: Extract with Lasso does not work correctly");
            TestExtractWithLassoDoesNotWorkCorrectly();

            //Trace.WriteLine("Verifying WindowsOSBugs: Clip with lasso sometimes does not work when the ink is done with very fast movement of mouse/pen, pentip");
            //TestClipWithLassoSometimesDoesNotWorkWithFastMovement();

            Trace.WriteLine("Verifying WindowsOSBugs: System.ArgumentException   when calling  StrokeCOllection.Clip( )");
            TestSystemArgumentExceptionWhenCallingStrokeCollectionClip();

            Success = true;
        }

#if PORT_FINISHED
        ////////////////////////////////////////////////////////
        // Inconsistent edge clipping for fully contained strokes
        ////////////////////////////////////////////////////////
        private void TestInconsistentEdgeClippingForFullyContainedStrokes()
        {
            StrokeCollection testInk = new StrokeCollection();

            testInk.CreateStroke (new StylusPoint[] { new StylusPoint (2000, 1500), new StylusPoint (1600, 1900) });
            testInk.Clip (new Rect (new StylusPoint (1000, 1000), new StylusPoint (2000, 2000)));
        }

        ////////////////////////////////////////////////////////
        // Clip return empty collection instead of null
        ////////////////////////////////////////////////////////
        private void TestClipReturnEmptyCollectionInsteadOfNull()
        {
            Ink ink = new Ink ();

            // hit test with empty rectangle should return empty stroke collection
            StrokeCollection strokes = ink.Strokes.HitTest (new Rect (0, 0, 0, 0), 0);

            if (strokes.Count != 0)
            {
                throw new InvalidOperationException ("Non-empty strokes collection for empty hit test");
            }
        }

        ////////////////////////////////////////////////////////
        // BuildTransformTable corrupts ISF
        ////////////////////////////////////////////////////////
        private void TestBuildTransformTableCorruptsISF()
        {
            Ink ink = DrtHelpers.LoadInk("HitTest.isf");

            for (int i = ink.Count - 1; i >= 0; i--)
            {
                if (i > 1)
                {
                    ink.Delete (ink[i]);
                }
            }

            System.Windows.Media.Matrix matrix = new System.Windows.Media.Matrix ((double).191676, 0, 0, (double).221515, (double)3290.37, (double)3399.68);
            foreach (Stroke s in ink.Strokes)
            {
                s.Transform (matrix, false);
            }

            ink.Strokes.Transform (matrix, false);

            Ink inkTest = new Ink (ink);
        }

        void undoEvent (object sender, UndoStateChangedEventArgs e)
        {
        }

        ////////////////////////////////////////////////////////
        // ICollection.CopyTo should support loosely typed arrays
        ////////////////////////////////////////////////////////
        private void TestCopyToSupportLooselyTypedArrays()
        {
            Ink ink = new Ink ();
            Guid guid1 = new Guid ("6DC08737-B9D4-4fc0-B9BF-8101320CFF7D");
            Guid guid2 = new Guid ("8FF30DAC-DD37-4674-BE19-7D7E5ABC786E");

            ink.ExtendedProperties.Add (guid1, "hello");
            ink.ExtendedProperties.Add (guid2, "world");

            object[] objects = new object[2];

            ((ICollection)ink.ExtendedProperties).CopyTo (objects, 0);
            if (((ExtendedProperty)objects[0]).Id != guid1 || (string)((ExtendedProperty)objects[0]).Data != "hello")
            {
                throw new InvalidOperationException ("Property mismatch");
            }

            if (((ExtendedProperty)objects[1]).Id != guid2 || (string)((ExtendedProperty)objects[1]).Data != "world")
            {
                throw new InvalidOperationException ("Property mismatch");
            }

            ExtendedProperty[] props = new ExtendedProperty[2];

            ink.ExtendedProperties.CopyTo (props, 0);
            if (props[0].Id != guid1 || (string)(props[0].Data) != "hello")
            {
                throw new InvalidOperationException ("Property mismatch");
            }

            if (props[1].Id != guid2 || (string)(props[1].Data) != "world")
            {
                throw new InvalidOperationException ("Property mismatch");
            }

            ink.CreateStroke (new StylusPoint[] { new StylusPoint (1, 1), new StylusPoint (1, 1) });
            ink.CreateStroke (new StylusPoint[] { new StylusPoint (2, 2), new StylusPoint (2, 2) });
            ((ICollection)ink.Strokes).CopyTo (objects, 0);
            if (((Stroke)objects[0]).Id != 1 || ((Stroke)objects[1]).Id != 2)
            {
                throw new InvalidOperationException ("Stroke mismatch");
            }

            Stroke[] strokes = new Stroke[2];

            ink.Strokes.CopyTo (strokes, 0);
            if (strokes[0].Id != 1 || strokes[1].Id != 2)
            {
                throw new InvalidOperationException ("Stroke mismatch");
            }
        }

        ////////////////////////////////////////////////////////
        // NearestPoint returns inverted FIndex
        ////////////////////////////////////////////////////////
        private void TestNearestPointReturnsInvertedFIndex()
        {
            Ink ink = new Ink ();

            ink.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (100, 100) });

            NearestPointResult result = ink.NearestPoint (new StylusPoint (0, 0));

            if (result.FloatingPointIndex != 0)
            {
                throw new InvalidOperationException ("Invalid FIndex");
            }
        }

        ////////////////////////////////////////////////////////
        // Clip returns bogus data
        ////////////////////////////////////////////////////////
        private void TestClipReturnsBogusData()
        {
            Ink ink = new Ink ();

            ink.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (200, 200) });
            ink.Clip (new Rect (9.5F, 9.5F, 1, 1));

            StylusPoint[] points = ink[0].GetPoints (0, ink[0].Count);

            if (points[0] != new StylusPoint (10, 10) || points[1] != new StylusPoint (11, 11))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }

            Ink ink1 = new Ink ();

            ink1.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (100, 100), new StylusPoint (200, 200) });
            ink1.Clip (new Rect (9.5F, 9.5F, 1, 1));

            StylusPoint[] points1 = ink1[0].GetPoints (0, ink1[0].Count);

            if (points1[0] != new StylusPoint (10, 10) || points1[1] != new StylusPoint (11, 11))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }

            Ink ink2 = new Ink ();

            ink2.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (200, 200) });
            ink2.Clip (new Rect (9.5F, 9.5F, 2, 2));

            StylusPoint[] points2 = ink2[0].GetPoints (0, ink2[0].Count);

            if (points2[0] != new StylusPoint (10, 10) || points2[1] != new StylusPoint (12, 12))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }

            Ink ink3 = new Ink ();

            ink3.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (200, 200) });
            ink3.Clip (new Rect (9.5F, 9.5F, 5, 1));

            StylusPoint[] points3 = ink3[0].GetPoints (0, ink3[0].Count);

            if (points3[0] != new StylusPoint (10, 10) || points3[1] != new StylusPoint (11, 11))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }

            Ink ink4 = new Ink ();

            ink4.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (200, 200) });
            ink4.Clip (new Rect (9.5F, 9.5F, 1, 5));

            StylusPoint[] points4 = ink4[0].GetPoints (0, ink4[0].Count);

            if (points4[0] != new StylusPoint (10, 10) || points4[1] != new StylusPoint (11, 11))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }

            Ink ink5 = new Ink ();

            ink5.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (200, 200) });
            ink5.Clip (new Rect (9, 9, 5, 1));

            StylusPoint[] points5 = ink5[0].GetPoints (0, ink5[0].Count);

            if (points5[0] != new StylusPoint (9, 9) || points5[1] != new StylusPoint (10, 10))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }

            Ink ink6 = new Ink ();

            ink6.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (200, 200), new StylusPoint (5, 5) });
            ink6.Clip (new Rect (9, 9, 1, 10));

            StylusPoint[] points6 = ink6[0].GetPoints (0, ink6[0].Count);

            if (points6[0] != new StylusPoint (9, 9) || points6[1] != new StylusPoint (10, 10))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }

            Ink ink7 = new Ink ();

            ink7.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (0, 1) });
            ink7.Clip (new Rect (0, 1, 2, 2));

            StylusPoint[] points7 = ink7[0].GetPoints (0, ink7[0].Count);

            if (points7[0] != new StylusPoint (0, 1))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }

            Ink ink8 = new Ink ();

            ink8.CreateStroke (new StylusPoint[] { new StylusPoint (0, 5), new StylusPoint (0, 6) });
            ink8.Clip (new Rect (0, 4, 2, 2));

            StylusPoint[] points8 = ink8[0].GetPoints (0, ink8[0].Count);

            if (points8[0] != new StylusPoint (0, 5) || points8[1] != new StylusPoint (0, 6))
            {
                throw new InvalidOperationException ("Incorrect clipping algorithm");
            }
        }

        static readonly string CorruptedPacketData = @"base64:ALEBAwRFBUgEBQE1GRQyCACAKAIAAAAAMwgAgBACAAAAABGrqtNBCooBxwGC/WH6vYlKLZQKXNhKSwsAEAWUGrcu8tktzKIoliqyiUSkqCwAAJUsKlhcVJZS5VmwWIlzZLUtKIL8cfjkJUBYsAWWUgslQLhSZ5kqrUs3KSiwlilyiKlgJZUBDcG5VgsFgsLKAEqVKBLLBYWLFIIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

        ////////////////////////////////////////////////////////
        // Base64 stream is unsaveable
        ////////////////////////////////////////////////////////
        private void TestBase64StreamIsUnsaveable()
        {
            Ink ink = new Ink (CorruptedPacketData);

            ink.Dirty = false;
            ink.Save ();
            try
            {
                Ink inkCorrupted = new Ink (CorruptedPacketData.Substring (7));
            }
            catch (ArgumentException)
            {
                return;
            }
            throw new InvalidOperationException ("Missing base64 header not detected");
        }

        ////////////////////////////////////////////////////////
        // AddStrokesAtRectangle clips wrong strokes
        ////////////////////////////////////////////////////////
        private void TestAddStrokesAtRectangleClipsWrongStrokes()
        {
            Ink ink = new Ink ();

            ink.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (50, 50), new StylusPoint (200, 200) });
            ink.CreateStroke (new StylusPoint[] { new StylusPoint (10, 20), new StylusPoint (30, 40), new StylusPoint (50, 60) });

            Ink copyInk = new Ink ();

            copyInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 200), new StylusPoint (300, 400) });
            copyInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 90), new StylusPoint (80, 70) });
            ink.AddStrokesAtRectangle (copyInk.Strokes, copyInk.GetBoundingBox (BoundingBoxMode.Default));
            // Result: 10.4878,50.4878,49.5122,89.5122;39.7561,60.2439,20.2439,79.7561
        }

        ////////////////////////////////////////////////////////
        // AddStrokesAtRectangle corrupted with Undo
        ////////////////////////////////////////////////////////
        private void TestAddStrokesAtRectangleCorruptedWithUndo()
        {
            Ink ink;
            Ink copyInk;
            { // verify without scaling involved
                ink = new Ink ();
                copyInk = new Ink ();
                copyInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 200), new StylusPoint (300, 400) });
                copyInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 90), new StylusPoint (80, 70) });
                ink.UndoStateChanged += new UndoStateChangedEventHandler (UndoEvent);
                ink.AddStrokesAtRectangle (copyInk.Strokes, copyInk.GetBoundingBox (BoundingBoxMode.Default));
                lastUndoObject.Do();
                lastUndoObject.Do();
            }
            {   // verify with scaling involved
                ink = new Ink ();
                copyInk = new Ink ();
                copyInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 200), new StylusPoint (300, 400) });
                copyInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 90), new StylusPoint (80, 70) });
                copyInk.Save ();
                ink.UndoStateChanged += new UndoStateChangedEventHandler (UndoEvent);
                ink.AddStrokesAtRectangle (copyInk.Strokes, new Rect (new StylusPoint (80, 70), new StylusPoint (300, 400)));

                byte[] data = ink.Save ();
                Ink tempInk = new Ink (data);

                lastUndoObject.Do();
                lastUndoObject.Do();
            }
        }

        ////////////////////////////////////////////////////////
        // AddStrokesAtRectangle doesn't copy Stroke ExtendedProperties
        ////////////////////////////////////////////////////////
        private void TestAddStrokesAtRectangleDoesntCopyStrokeExtendedProperties()
        {
            Ink ink = new Ink ();
            Ink copyInk = new Ink ();

            copyInk.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (50, 50), new StylusPoint (200, 200) });
            copyInk[0].ExtendedProperties.Add (Guid.NewGuid (), "hello world");
            ink.AddStrokesAtRectangle (copyInk.Strokes, copyInk.GetBoundingBox (BoundingBoxMode.Default));
            if ((ink[0].ExtendedProperties.Count != 1) || (string)(ink[0].ExtendedProperties[0].Data) != "hello world")
            {
                throw new InvalidOperationException ("Stroke ExtendedProperties are not copied");
            }
        }

        ////////////////////////////////////////////////////////
        // Dirty flag is true after load
        ////////////////////////////////////////////////////////
        private void TestDirtyFlagIsTrueAfterLoad()
        {
            Ink ink = new Ink (CorruptedPacketData);

            if (ink.Dirty)
            {
                throw new InvalidOperationException ("Dirty flag is true after ink load");
            }
        }

        ////////////////////////////////////////////////////////
        // Null exception after Undo and save
        ////////////////////////////////////////////////////////
        private void TestNullExceptionAfterUndoAndSave()
        {
            Ink testInk = new Ink ();
            StrokeCollection startStrokes = testInk.CreateStrokes ();

            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 100), new StylusPoint (200, 200), new StylusPoint (300, 0) }));
            testInk.UndoStateChanged += new UndoStateChangedEventHandler (UndoEvent);

            Ink inkBeforeChange = new Ink (testInk.Save ());
            Rect rect = new Rect (new StylusPoint (100, 100), new StylusPoint (250, 250));

            testInk.Clip (rect);

            Ink inkAfterChange = new Ink (testInk.Save ());

            lastUndoObject.Do();

            Ink inkBeforeChange2 = new Ink (testInk.Save ());

            lastUndoObject.Do();

            Ink inkAfterChange2 = new Ink (testInk.Save ());
        }

        ////////////////////////////////////////////////////////
        // Clip incorrectly undone
        ////////////////////////////////////////////////////////
        private void TestClipIncorrectlyUndone()
        {
            Ink testInk = new Ink ();
            StrokeCollection startStrokes = testInk.CreateStrokes ();

            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (300, 300) }));
            testInk.UndoStateChanged += new UndoStateChangedEventHandler (UndoEvent);

            // Clip the stroke
            Rect rect = new Rect (new StylusPoint (100, 100), new StylusPoint (200, 200));

            testInk.Clip (rect);
            if (Points (testInk[0])[0] != new StylusPoint (100, 100) || Points (testInk[0])[1] != new StylusPoint (200, 200))
            {
                throw new InvalidOperationException ("Clipping operation failed");
            }

            lastUndoObject.Do();
            if (Points (testInk[0])[0] != new StylusPoint (0, 0) || Points (testInk[0])[1] != new StylusPoint (300, 300))
            {
                throw new InvalidOperationException ("Undo of Clipping operation failed");
            }

            lastUndoObject.Do();
            if (Points (testInk[0])[0] != new StylusPoint (100, 100) || Points (testInk[0])[1] != new StylusPoint (200, 200))
            {
                throw new InvalidOperationException ("Restore of clipping operation failed");
            }
        }

        ////////////////////////////////////////////////////////
        // ExtractStrokes extracts wrong strokes
        ////////////////////////////////////////////////////////
        private void TextExtractStrokesExtractsWrongStrokes()
        {
            Ink testInk = new Ink ();
            Ink otherInk;
            StrokeCollection startStrokes = testInk.CreateStrokes ();

            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (-100, 0), new StylusPoint (100, 0) }));
            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (0, -100), new StylusPoint (0, 100) }));

            Rect rect = new Rect (new StylusPoint (-50, -50), new StylusPoint (50, 50));

            otherInk = testInk.ExtractStrokes (rect);

            if (Points (testInk[0])[0] != new StylusPoint (-100, 0) ||
                Points (testInk[0])[1] != new StylusPoint (-50, 0) ||
                Points (testInk[1])[0] != new StylusPoint (50, 0) ||
                Points (testInk[1])[1] != new StylusPoint (100, 0) ||
                Points (testInk[2])[0] != new StylusPoint (0, -100) ||
                Points (testInk[2])[1] != new StylusPoint (0, -50) ||
                Points (testInk[3])[0] != new StylusPoint (0, 50) ||
                Points (testInk[3])[1] != new StylusPoint (0, 100))
            {
                throw new InvalidOperationException ("Extract source failed");
            }
            if (Points (otherInk[0])[0] != new StylusPoint (-50, 0) || Points (otherInk[0])[1] != new StylusPoint (50, 0) ||
                Points (otherInk[1])[0] != new StylusPoint (0,-50) || Points (otherInk[1])[1] != new StylusPoint (0, 50))
            {
                throw new InvalidOperationException ("Extract target failed");
            }
        }

        ////////////////////////////////////////////////////////
        // HitTest with lasso hits wrong stroke
        ////////////////////////////////////////////////////////
        private void TestHitTestWithLassoHitsWrongStroke()
        {
            Ink testInk = new Ink ();
            StrokeCollection startStrokes = testInk.CreateStrokes ();

            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (50, -100), new StylusPoint (50, 10) }));
            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (60, 20), new StylusPoint (60, 200) }));
            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (-1000, -1000), new StylusPoint (-2000, -2000) }));

            StylusPoint[] lassoPoints = new StylusPoint[] { new StylusPoint (-50, -10), new StylusPoint (50, -10), new StylusPoint (15, 50) };
            StrokeCollection strokes = testInk.Strokes.HitTest (lassoPoints, 100);

            if (strokes.Count != 0)
            {
                throw new InvalidOperationException ("HitTest percentage for partial segments failed");
            }
        }

        ////////////////////////////////////////////////////////
        // HitTest with lasso returns no strokes
        ////////////////////////////////////////////////////////
        private void TestHitTestWithLassoReturnsNoStrokes()
        {
            Ink testInk = new Ink ();
            StrokeCollection startStrokes = testInk.CreateStrokes ();

            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (10, 0), new StylusPoint (20, 0) }));
            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (10, 10), new StylusPoint (20, 10) }));

            StylusPoint[] lassoPoints = new StylusPoint[] { new StylusPoint(-50, -10), new StylusPoint(50, -10), new StylusPoint(15, 100)};
            StrokeCollection strokes = testInk.Strokes.HitTest (lassoPoints, 100);

            if (strokes.Count != 2)
            {
                throw new InvalidOperationException ("HitTest percentage for partial segments failed");
            }
        }

        ////////////////////////////////////////////////////////
        // Round-tripping ISF via v2 Ink causes scaling
        ////////////////////////////////////////////////////////
        private void TestRoundTrippingISFCausesScalling()
        {
            Microsoft.Ink.Ink oldInk = new Microsoft.Ink.Ink ();
            oldInk.CreateStroke (new System.Drawing.StylusPoint[] { new System.Drawing.Point (0, 0), new System.Drawing.Point (50, 50), new System.Drawing.Point (100, 100) });
            oldInk.CreateStroke (new System.Drawing.StylusPoint[] { new System.Drawing.Point (0, 0), new System.Drawing.Point (50, 50), new System.Drawing.Point (100, 100) });
            oldInk.CreateStroke (new System.Drawing.StylusPoint[] { new System.Drawing.Point (0, 0), new System.Drawing.Point (50, 50), new System.Drawing.Point (100, 100) });

            byte[] oldBytes = oldInk.Save ();
            Ink newInk = new Ink (oldBytes);
            byte[] newBytes = newInk.Save ();

            Microsoft.Ink.Ink newInk1 = new Microsoft.Ink.Ink ();
            newInk1.Load (oldBytes);

            Microsoft.Ink.Ink newInk2 = new Microsoft.Ink.Ink ();
            newInk2.Load (newBytes);

            if (newInk1.Count != newInk2.Count)
            {
                throw new InvalidOperationException ("Stroke count failed to roundtrip");
            }
            for (int i = 0; i < newInk1.Count; i++)
            {
                for (int j = 0; j < newInk1[i].Count; j++)
                {
                    if (newInk1[i].GetPoints (0, newInk1[i].Count)[j] !=
                    newInk2[i].GetPoints (0, newInk2[i].Count)[j])
                    {
                        throw new InvalidOperationException ("Stroke points failed to roundtrip");
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////
        // Undo fails for ExtractStrokes (deleted)
        ////////////////////////////////////////////////////////
        private void TestUndoFailsForExtractStrokes()
        {
            Ink otherInk;
            Ink testInk = new Ink ();

            // SourceStrokesWidth Parameter
            StrokeCollection startStrokes = testInk.CreateStrokes ();

            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (10, 20), new StylusPoint (30, 40) }));
            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (50, 60), new StylusPoint (70, 80) }));
            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 200), new StylusPoint (300, 400), new StylusPoint (500, 600) }));
            startStrokes.Add (testInk.CreateStroke (new StylusPoint[] { new StylusPoint (700, 800), new StylusPoint (900, 1000), new StylusPoint (1100, 1200) }));

            StrokeCollection strokesToDelete = testInk.CreateStrokes (new int[] { 2, 3 });
            StrokeCollection strokesToExtract = testInk.CreateStrokes (new int[] { 2, 3 });

            // delete strokes 2 and 3
            testInk.Delete (strokesToDelete);

            // Registers generic event handler with ink object for implementing undo/redo stack
            testInk.UndoStateChanged += new UndoStateChangedEventHandler (UndoEvent);

            Ink inkBeforeChange = new Ink (testInk.Save ());

            // Try extracting deleted strokes (2 and 3)
            otherInk = testInk.ExtractStrokes (strokesToExtract);

            Ink inkAfterChange = new Ink (testInk.Save ());

            lastUndoObject.Do();

            Ink inkBeforeChange2 = new Ink (testInk.Save ());

            lastUndoObject.Do(); // perform redo
            Ink inkAfterChange2 = new Ink (testInk.Save ());

        }

        ////////////////////////////////////////////////////////
        // Undo/Redo fails for ExtractStrokes
        ////////////////////////////////////////////////////////
        private void TestUndoRedoFailsForExtractStrokes()
        {
            Ink otherInk;
            Ink testInk = new Ink ();

            testInk.CreateStroke (new StylusPoint[] { new StylusPoint (-100, 0), new StylusPoint (100, 0) });
            testInk.CreateStroke (new StylusPoint[] { new StylusPoint (0, -100), new StylusPoint (0, 100) });

            // Registers generic event handler with ink object for implementing undo/redo stack
            testInk.UndoStateChanged += new UndoStateChangedEventHandler (UndoEvent);

            Ink inkBeforeChange = new Ink (testInk.Save ());

            if (Points (testInk[0])[0] != new StylusPoint (-100, 0) ||
                Points (testInk[0])[1] != new StylusPoint (100, 0) ||
                Points (testInk[1])[0] != new StylusPoint (0, -100) ||
                Points (testInk[1])[1] != new StylusPoint (0, 100) ||
                testInk.Count != 2)
            {
                throw new InvalidOperationException ("Ink setup operation failed");
            }

            // Try extracting middle region
            otherInk = testInk.ExtractStrokes (new Rect (new StylusPoint (-50, -50), new StylusPoint (51, 51)));
            if (Points (testInk[0])[0] != new StylusPoint (-100, 0) ||
                Points (testInk[0])[1] != new StylusPoint (-50, 0) ||
                Points (testInk[1])[0] != new StylusPoint (51, 0) ||
                Points (testInk[1])[1] != new StylusPoint (100, 0) ||
                Points (testInk[2])[0] != new StylusPoint (0, -100) ||
                Points (testInk[2])[1] != new StylusPoint (0, -50) ||
                Points (testInk[3])[0] != new StylusPoint (0, 51) ||
                Points (testInk[3])[1] != new StylusPoint (0, 100) ||
                testInk.Count != 4)
            {
                throw new InvalidOperationException ("Extract operation failed");
            }

            Ink inkAfterChange = new Ink (testInk.Save ());

            lastUndoObject.Do(); // perform undo
            if (Points (testInk[0])[0] != new StylusPoint (-100, 0) ||
                Points (testInk[0])[1] != new StylusPoint (100, 0) ||
                Points (testInk[1])[0] != new StylusPoint (0, -100) ||
                Points (testInk[1])[1] != new StylusPoint (0, 100) ||
                testInk.Count != 2)
            {
                throw new InvalidOperationException ("Undo of Extract operation failed");
            }

            Ink inkBeforeChange2 = new Ink (testInk.Save ());

            lastUndoObject.Do(); // perform redo
            Ink inkAfterChange2 = new Ink (testInk.Save ());
            if (Points (testInk[0])[0] != new StylusPoint (-100, 0) ||
                Points (testInk[0])[1] != new StylusPoint (-50, 0) ||
                Points (testInk[1])[0] != new StylusPoint (51, 0) ||
                Points (testInk[1])[1] != new StylusPoint (100, 0) ||
                Points (testInk[2])[0] != new StylusPoint (0, -100) ||
                Points (testInk[2])[1] != new StylusPoint (0, -50) ||
                Points (testInk[3])[0] != new StylusPoint (0, 51) ||
                Points (testInk[3])[1] != new StylusPoint (0, 100) ||
                testInk.Count != 4)
            {
                throw new InvalidOperationException ("Redo of Extract operation failed");
            }

        }

        ////////////////////////////////////////////////////////
        // HitTest with lasso returns no strokes
        ////////////////////////////////////////////////////////
        private void TestHitTestWithLassoReturnsNoStrokes2()
        {
            {
                Microsoft.Ink.Ink ink = new Microsoft.Ink.Ink ();

                ink.CreateStroke (new System.Drawing.StylusPoint[] { new System.Drawing.Point (10, 0), new System.Drawing.Point (20, 0) });

                System.Drawing.StylusPoint[] lasso = new System.Drawing.StylusPoint[] { new System.Drawing.Point (-50, -10), new System.Drawing.Point (50, -10),
                    new System.Drawing.Point (15, 50), new System.Drawing.Point (-63, -22)};

                Microsoft.Ink.Strokes strokesTest = ink.HitTest (lasso, 100);

                if (strokesTest.Count != 1)
                {
                    throw new InvalidOperationException ("v1 HitTest failed to detect stroke");
                }
            }

            {
                Ink testInk = new Ink ();

                Stroke s1 = testInk.CreateStroke (new StylusPoint[] { new StylusPoint (10, 0), new StylusPoint (20, 0) });

                StylusPoint[] lassoPoints = new StylusPoint[] { new StylusPoint (-50, -10), new StylusPoint (50, -10),
                    new StylusPoint (15, 50), new StylusPoint (-63, -22)};

                StrokeCollection strokes = testInk.Strokes.HitTest (lassoPoints, 100);

                if (strokes.Count != 1)
                {
                    throw new InvalidOperationException ("HitTest failed to detect stroke");
                }
            }
        }

        ////////////////////////////////////////////////////////
        // ExtractStrokes results in odd results
        ////////////////////////////////////////////////////////
        private void TestExtractStrokesResultsInOddResults()
        {
            Ink testInk = new Ink ();
            Ink otherInk;

            testInk.CreateStroke (new StylusPoint[] { new StylusPoint (10, 20), new StylusPoint (30, 40) });
            testInk.CreateStroke (new StylusPoint[] { new StylusPoint (100, 200), new StylusPoint (300, 400) });
            testInk.CreateStroke (new StylusPoint[] { new StylusPoint (1000, 2000), new StylusPoint (3000, 4000) });

            Rect rect = new Rect (new StylusPoint (10, 20), new StylusPoint (11, 21));

            otherInk = testInk.ExtractStrokes (rect);
        }

        ////////////////////////////////////////////////////////
        // Undo fails for Width Transform
        ////////////////////////////////////////////////////////
        private void TestUndoFailsForWidthTransform()
        {
            Ink testInk = new Ink ();

            Stroke s1 = testInk.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (1, 1) });
            s1.DrawingAttributes.Width = 10;

            Matrix matrixTransform = new Matrix (40000, 0, 0, 40000, 0, 0);

            testInk.UndoStateChanged += new UndoStateChangedEventHandler (UndoEvent);

                // perform transformation
            testInk.Transform (matrixTransform, true);

            if (Points (testInk[0])[0] != new StylusPoint (0, 0) ||
                Points (testInk[0])[1] != new StylusPoint (40000, 40000) ||
                testInk[0].DrawingAttributes.Width != 400000 ||
                testInk.Count != 1)
            {
                throw new InvalidOperationException ("Transform operation failed");
            }
                // undo transform
            lastUndoObject.Do();
            if (Points (testInk[0])[0] != new StylusPoint (0, 0) ||
                Points (testInk[0])[1] != new StylusPoint (1, 1) ||
                testInk[0].DrawingAttributes.Width != 10 ||
                testInk.Count != 1)
            {
                throw new InvalidOperationException ("Undo Transform operation failed");
            }

                // redo transform
            lastUndoObject.Do();
            if (Points (testInk[0])[0] != new StylusPoint (0, 0) ||
                Points (testInk[0])[1] != new StylusPoint (40000, 40000) ||
                !AreClose(testInk[0].DrawingAttributes.Width, 400000) ||
                testInk.Count != 1)
            {
                throw new InvalidOperationException ("Redo Transform operation failed");
            }

        }

        ////////////////////////////////////////////////////////
        // ScaleToRectangle mishandles pen width
        ////////////////////////////////////////////////////////
        private void TestScaleToRectangleMishandlesPenWidth()
        {
            Ink testInk = new Ink ();

            testInk.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (1, 1) });

            Rect rect = new Rect (new StylusPoint (0, 0), new StylusPoint (5000, 5000));

            testInk.ScaleToRectangle (rect);

            if (Points(testInk[0])[0] != new StylusPoint(0,0) ||
                Points(testInk[0])[1] != new StylusPoint(5000,5000))
            {
                throw new InvalidOperationException ("ScaleToRectangle mishandled pen width");
            }
        }

        ////////////////////////////////////////////////////////
        // Verify Ink can be used in partial trust scenario
        ////////////////////////////////////////////////////////
        private void TestVerifyInkCanBeUsedInPartialTrustScenario()
        {
            InkInPartialTrust untrustedInk;
            System.AppDomain internetDomain = null;
            try
            {
                internetDomain = System.AppDomain.CreateDomain ("MyInternetZone", new System.Security.Policy.Evidence (new object[] { new System.Security.Policy.Zone (System.Security.SecurityZone.Internet) }, null));
            }
                    // if we can't create an AppDomain, then we are already running partially trusted
            catch (System.Security.SecurityException)
            {
            }
            if (internetDomain != null)
            {
                untrustedInk = (InkInPartialTrust)internetDomain.CreateInstanceAndUnwrap (typeof(InkInPartialTrust).Assembly.ToString (), typeof(InkInPartialTrust).ToString ());
            }
            // if test harness is already running under partial trust, then just execute the test
            else
            {
                untrustedInk = new InkInPartialTrust ();
            }
            untrustedInk.Validate ();
        }

        ////////////////////////////////////////////////////////
        // Extract fails with true flags
        ////////////////////////////////////////////////////////
        private void TestExtractFailsWithTrueFlag()
        {
            Ink testInk = new Ink ();
            Stroke s = testInk.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (100, 0) });
            Rect boundingBox = testInk.GetBoundingBox ();
            Ink destInk = testInk.ExtractStrokes (boundingBox, true);

            if (testInk.Count != 0 ||
                destInk.Count != 1 ||
                Points(destInk[0])[0] != new StylusPoint (0, 0) ||
                Points (destInk[0])[1] != new StylusPoint (100, 0))
            {
                throw new InvalidOperationException ("ExtractStrokes failed for line");
            }
        }

        ////////////////////////////////////////////////////////
        // DrawingAttributes does not dirty Ink
        ////////////////////////////////////////////////////////
        private void TestDrawingAttributesDoesNotDirtyInk()
        {
            Ink testInk = new Ink ();
            Stroke s = testInk.CreateStroke (new StylusPoint[] { new StylusPoint (0, 0), new StylusPoint (100, 0) });

            if (!testInk.Dirty)
            {
                throw new InvalidOperationException ("Invalid starting dirty flag");
            }

                // reset the dirty flag
            testInk.Save ();

            if (testInk.Dirty)
            {
                throw new InvalidOperationException ("Invalid intermediate dirty flag");
            }

            testInk[0].DrawingAttributes.Width = 1.0F;
            if (!testInk.Dirty)
            {
                throw new InvalidOperationException ("Invalid final dirty flag");
            }
        }

        ////////////////////////////////////////////////////////
        // ExtendedProperties IndexOf fails
        ////////////////////////////////////////////////////////
        private void TestExtendedPropertiesIndexOfFails()
        {
            Object objData = "sampleData";
            Guid myGuid = Guid.NewGuid();
            Ink myInk = new Ink();
            Object inkIndex = myInk.ExtendedProperties.Add(myGuid, objData);
            int index = myInk.ExtendedProperties.IndexOf((ExtendedProperty)inkIndex);

            if (index != 0)
            {
                throw new InvalidOperationException("IndexOf operation failed");
            }
        }
        ////////////////////////////////////////////////////////
        // ExtendedProperties.Remove doesn't disconnect events
        ////////////////////////////////////////////////////////
        private void TestExtendedPropertiesRemoveDoesntDisconnectEvents()
        {
            Ink testInk = new Ink();
            Guid guid1 = new Guid("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

            testInk.ExtendedProperties.Add(guid1, "string1");

            // Register generic event handler
            lastUndoObject = null;
            testInk.UndoStateChanged += new UndoStateChangedEventHandler(UndoEvent);

            ExtendedProperty ep = testInk.ExtendedProperties[0];

            testInk.ExtendedProperties.Remove(ep);
            if (lastUndoObject == null)
            {
                throw new InvalidOperationException("EP Remove did not generate undo unit");
            }

            lastUndoObject = null;

            // trigger change to disconnected EP - no undo event should fire
            ep.Data = "string2";
            if (lastUndoObject != null)
            {
                throw new InvalidOperationException("disconnected EP change generated an undo unit");
            }
        }

        ////////////////////////////////////////////////////////
        // Multiple ExtendedProperties not roundtripped in Stroke DA
        ////////////////////////////////////////////////////////
        private void TestMultipleExtendedPropertiesNotRoundtrippedInStrokeDA()
        {
            Ink testInk = new Ink();
            Stroke s1 = testInk.CreateStroke(new StylusPoint[] { new StylusPoint(0, 0), new StylusPoint(100, 100) });
            Guid guid1 = new Guid("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
            Guid guid2 = new Guid("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb");

            s1.DrawingAttributes.Merge(new DrawingAttribute(guid1, "string1"));
            s1.DrawingAttributes.Merge(new DrawingAttribute(guid2, "string2"));
            if (s1.DrawingAttributes.Count != 2)
            {
                throw new InvalidOperationException("EPs not added to Stroke DA");
            }

            Ink newInk = new Ink(testInk.Save());

            s1 = newInk[0];
            if (s1.DrawingAttributes.Count != 2)
            {
                throw new InvalidOperationException("EPs not persisted with Stroke DA");
            }
        }

        public class InkInPartialTrust : System.MarshalByRefObject
        {
            public void Validate()
            {
                Ink testInk = new Ink ();

                testInk.CreateStroke (new StylusPoint[] { new StylusPoint (2000, 1500), new StylusPoint (1600, 1900) });

                // use compression library which is unmanaged today...
                testInk.Save (PersistenceFormat.InkSerializedFormat, CompressionMode.Maximum);

                try
                {
                        // this should fail since this object is running in InternetZone
                    File.Exists (System.Windows.Forms.Application.ExecutablePath);

                    throw new InvalidOperationException ("Partial Domain not setup correctly");
                }
                catch (System.Security.SecurityException)
                {
                    return;
                }
            }
        }

        IUndoUnit lastUndoObject;

        void UndoEvent (object sender, UndoStateChangedEventArgs args)
        {
            lastUndoObject = args.Change;
        }

        StylusPoint[] Points (Stroke stroke)
        {
            return stroke.GetPoints (0, stroke.Count);
        }
#endif // PORT_FINISHED

        internal static float FLT_EPSILON = 1.192092896e-05F;
        /// <summary>
        /// AreClose
        /// </summary>
        public static bool AreClose(float a, float b)
        {
            // This computes (|a-b| / (|a| + |b| + 10.0f)) < FLT_EPSILON
            float eps = ((float)Math.Abs(a) + (float)Math.Abs(b) + 10.0f) * FLT_EPSILON;
            float delta = a - b;

            return (-eps < delta) && (eps > delta);
        }
        internal static double Epsilon = 2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */
        public static bool AreClose(double value1, double value2)
        {
            return AreClose(value1, value2, Epsilon);
        }

        public static bool AreClose(double value1, double value2, double epsilon)
        {
            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2) return true;

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleUtil.Epsilon
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * epsilon;
            double delta = value1 - value2;

            return (-eps < delta) && (eps > delta);
        }

        private void Stroke_Copy()
        {
            DrawingAttributes da = new DrawingAttributes();
            da.Color = Colors.Green;

            StylusPointCollection pts = new StylusPointCollection();
            pts.Add(new StylusPoint(1f, 1f));
            Stroke s = new Stroke(pts, da);

            if (s.StylusPoints.Count != 1)
            {
                throw new InvalidOperationException("Invalid source points");
            }
            if (s.DrawingAttributes.Color != Colors.Green)
            {
                throw new InvalidOperationException("Invalid source drawing attributes");
            }

            Stroke s2 = s.Clone();
            if (s2.StylusPoints.Count != 1)
            {
                throw new InvalidOperationException("Invalid source points");
            }
            if (s2.DrawingAttributes.Color != Colors.Green)
            {
                throw new InvalidOperationException("Invalid source drawing attributes");
            }
        }

        /// <summary>
        /// Error occurs when deserializing InkCanvas data
        /// </summary>
        private void TestErrorOccursWhenDeserializingInkCanvasData()
        {
            StrokeCollection strokes_v1 = new StrokeCollection(new MemoryStream(Convert.FromBase64String("APUBHAOAgAQdA6ICZAMESBBFNRkUMggAgBQCAAAIQjMIAIAQAgAAFEIRq6rTQQovSYL8JfhOXKSzYLAqFlAAAqWVNyypRNhAgvw5+H4sWWBLAFQWCwWBcqyoWVKlRUAKBQEHagdSChwigvyd+TwlEAlsFixUgvx1+OSyyxSFzUWWUllAChgYgvyF+QSbhYsqVQCC/H34/spWdgEuLgAKMUyC/Rn6NgsFgBZUpWtyxLJYgIMWDNirLbLAgvyF+QwAsoEolQCSpYWWblLLFSpQsLAKJTGC/cX7jJYLCDcqwsqVEqyUVKCC/FH4nsSspZZVJZVSgsqURco=")));
            StrokeCollection strokes_empty = new StrokeCollection(new MemoryStream(Convert.FromBase64String("AAYCAA8AHwA=")));
        }

        /// <summary>
        /// Error occurs when inserting existing item at end of single item list
        /// Expected: ArgumentException should be thrown.
        /// </summary>
        private void TestErrorWhenInsertingExistingItemAtEndOfSingleItemList()
        {
            StrokeCollection strokes = new StrokeCollection();
            StylusPointCollection spc = new StylusPointCollection();
            spc.Add(new StylusPoint(1, 1));
            Stroke s = new Stroke(spc);
            strokes.Add(s);
            strokes.Insert(1, s);
        }

        /// <summary>
        /// Error occurs when inserting existing item before later item in 2 item list
        /// Expected: ArgumentException should be thrown.
        /// </summary>
        private void TestErrorOccursWhenInsertingExistingItemBeforeLaterItemIn2ItemList()
        {
            StrokeCollection strokes = new StrokeCollection();
            StylusPointCollection spc = new StylusPointCollection();
            spc.Add(new StylusPoint(1, 1));
            Stroke s1 = new Stroke(spc);
            Stroke s2 = new Stroke(spc);

            strokes.Add(s1);
            strokes.Add(s2);
            strokes.Insert(1, s1);
        }

        /// <summary>
        /// Error occurs when inserting existing item before later item in 2 item list
        /// Expected: ArgumentException should be thrown.
        /// </summary>
        private void TestErrorOccursWhenInsertingExistingItemBeforeLaterItemIn2ItemListB()
        {
            StrokeCollection strokes = new StrokeCollection();
            StylusPointCollection spc = new StylusPointCollection();
            spc.Add(new StylusPoint(1, 1));
            Stroke s1 = new Stroke(spc);
            Stroke s2 = new Stroke(spc);

            strokes.Add(s1);
            strokes.Add(s2);
            strokes.Add(s1);
            strokes.Insert(2, s1);
        }

        //private void TestStrokesRotated()
        //{
        //    Stroke testStroke = new Stroke(new StylusPoint[] {
        //               new StylusPoint(0, 0), new StylusPoint(1, 1), new StylusPoint(-2, 2), new StylusPoint(-3, -3), new StylusPoint(-4, 4) },null);
        //    StylusPoint[] expectedPoints = testStroke.GetRenderingPoints();

        //    RotateTransform rotation = MatrixTransform.CreateRotation(360, new StylusPoint(5, 10));
        //    testStroke.Transform *= rotation.Value;

        //    StylusPoint[] actualPoints = testStroke.GetRenderingPoints();

        //    for(int i=0; i<actualPoints.Length; i++)
        //    {
        //        double epsilonFudgeFactor = 10;
        //        if(!AreClose(actualPoints[i].X, expectedPoints[i].X, Epsilon * epsilonFudgeFactor) ||
        //           !AreClose(actualPoints[i].Y, expectedPoints[i].Y, Epsilon * epsilonFudgeFactor))
        //        {
        //            throw new InvalidOperationException("Unexpected point values detected: Actual=" + actualPoints[i].ToString() + " Expected=" + expectedPoints[i].ToString());
        //        }
        //    }

        //          = [ 120913 ]
        //          = [Test Function] TestRotate
        //          = [Parameters] InputPointData=0,0,1,1;-2,2,-3,-3;4,-4, InputDegrees=360, InputX=5, InputY=10, ExpectedPointData=0,0,1,1;-2,2,-3,-3;4,-4
        //          = Actual points:
        //          = 1.748456E-06,-8.742278E-07,1.000002,0.9999993
        //          = Not as expected:
        //          = 0,0,1,1
        //          = Actual points:
        //          = -1.999999,1.999999,-2.999998,-3.000001
        //          = Not as expected:
        //          = -2,2,-3,-3
        //          = Actual points:
        //          = 4.000002,-4
        //          = Not as expected:
        //          = 4,-4
        //        }

        /// <summary>
        /// Error occurs when replacing last item
        /// Expected: no error
        /// </summary>
        private void TestErrorOccursWhenReplacingLastItem()
        {
            StrokeCollection strokes = new StrokeCollection();
            StylusPointCollection spc = new StylusPointCollection();
            spc.Add(new StylusPoint(1, 1));
            Stroke s1 = new Stroke(spc);
            Stroke s2 = new Stroke(spc);
            Stroke s3 = new Stroke(spc);

            strokes.Add(s1);
            strokes.Add(s2);
            strokes[1] = s3;
        }

        /// <summary>
        /// Invalid state occurs after replace
        /// Expected: no error
        /// </summary>
        private void TestInvalidStateOccursAfterReplace()
        {
            StrokeCollection strokes = new StrokeCollection();
            StylusPointCollection spc = new StylusPointCollection();
            spc.Add(new StylusPoint(1, 1));
            Stroke s1 = new Stroke(spc);
            Stroke s2 = new Stroke(spc);

            strokes.Add(s1);
            strokes.Add(s2);
            if (strokes.Count != 2)
            {
                throw new InvalidOperationException("Invalid count");
            }

            try
            {
                strokes[1] = s1;
            }
            catch (ArgumentException)
            {
                if (strokes.Count != 2)
                {
                    throw new InvalidOperationException("Invalid count");
                }

                if (strokes[1] != s2)
                {
                    throw new InvalidOperationException("Invalid element");
                }
            }
        }

        /// <summary>
        /// Invalid state occurs after replace
        /// Expected: no error
        /// </summary>
        private void TestInvalidStateOccursAfterReplaceB()
        {
            bool excetionCaught = false;

            StrokeCollection strokes = new StrokeCollection();
            StylusPointCollection spc = new StylusPointCollection();
            spc.Add(new StylusPoint(1, 1));
            Stroke s1 = new Stroke(spc);
            Stroke s2 = new Stroke(spc);

            strokes.Add(s1);
            strokes.Add(s2);
            if (strokes.Count != 2)
            {
                throw new InvalidOperationException("Invalid count");
            }

            try
            {
                strokes[0] = s2;
            }
            catch (ArgumentException)
            {
                excetionCaught = true;
            }

            if (!excetionCaught)
            {
                throw new InvalidOperationException("No expected exception");
            }

            if (strokes.Count != 2)
            {
                throw new InvalidOperationException("Invalid count");
            }

            if (strokes[0] != s1)
            {
                throw new InvalidOperationException("Invalid element");
            }
        }

        /// <summary>
        /// Invalid exception thrown by CopyTo
        /// Expected: ArgumentOutOfRangeException
        /// </summary>
        private void TestInvalidExceptionThrownByCopyTo()
        {
            StrokeCollection strokes = new StrokeCollection();

            StylusPointCollection spc = new StylusPointCollection();
            spc.Add(new StylusPoint(1, 1));
            Stroke s1 = new Stroke(spc);
            Stroke s2 = new Stroke(spc);

            strokes.Add(s1);
            strokes.Add(s2);
            if (strokes.Count != 2)
            {
                throw new InvalidOperationException("Invalid count");
            }

            Stroke[] array = new Stroke[2];

            try
            {
                strokes.CopyTo(array, 1);
            }
            catch (ArgumentException)
            {
                return;
            }
            throw new InvalidOperationException("Invalid test reported success");
        }

        /// <summary>
        /// Invalid exception thrown by CopyTo
        /// Expected: ArgumentOutOfRangeException
        /// </summary>
        private Stroke CreateStroke10And20And100()
        {
            //test a simple case
            StylusPoint[] strokePoints = new StylusPoint[]{
                                                new StylusPoint(10f, 10f),
                                                new StylusPoint(10f, 20f),
                                                new StylusPoint(10f, 100f)
                                              };
            return new Stroke(new StylusPointCollection(strokePoints));
        }


        /// <summary>
        /// Incorrect events fire for index setter
        /// Expected: no error
        /// </summary>
        private void TestIncorrectEventsFireForIndexSetter()
        {
            // For verification of eventing
            //            int extendedPropertiesChangedCount = 0;
            //            int drawingAttributesChangedCount = 0;
            int strokesChangedCount = 0;
            //            StrokeCollection savedSC1 = null;
            //            StrokeCollection savedSC2 = null;
            StrokeCollection savedSC3 = null;
            //            ExtendedPropertiesChangedEventArgs savedCAEventArgs = null;
            //            PropertyDataChangedEventArgs savedDAEventArgs = null;
            StrokeCollectionChangedEventArgs savedStrokesEventArgs = null;

            StrokeCollection sc = new StrokeCollection();
            StylusPointCollection spc = new StylusPointCollection();
            spc.Add(new StylusPoint(1, 1));
            Stroke s = new Stroke(spc);
            Stroke s2 = new Stroke(spc);


            sc.Add(s);

            sc.StrokesChanged += delegate(object sender, StrokeCollectionChangedEventArgs args)
            {
                strokesChangedCount++;
                savedSC3 = (StrokeCollection)sender;
                savedStrokesEventArgs = args;
            };
            //            sc.DrawingAttributesChanged += delegate(object sender, PropertyDataChangedEventArgs args)
            //            {
            //                drawingAttributesChangedCount++;
            //                savedSC2 = (StrokeCollection)sender;
            //                savedDAEventArgs = args;
            //            };
            //            sc.ExtendedPropertiesChanged += delegate(object sender, ExtendedPropertiesChangedEventArgs args)
            //            {
            //                extendedPropertiesChangedCount++;
            //                savedCAEventArgs = args;
            //                savedSC1 = (StrokeCollection)sender;
            //            };

            sc[0] = s2;

            // Expected only one event(StrokesChangedEvent)
            if (
                /*
                                extendedPropertiesChangedCount != 0 ||
                                drawingAttributesChangedCount != 0 ||
                */
                strokesChangedCount != 1)
            {
                throw new InvalidOperationException("Incorrect number of events");
            }

            if (savedSC3 != sc //
                || savedStrokesEventArgs.Added.Count != 1 //
                || savedStrokesEventArgs.Removed.Count != 1) //
            {
                throw new InvalidOperationException("EventArgs contain wrong data");
            }

            if (savedStrokesEventArgs.Added[0] != s2 ||
                savedStrokesEventArgs.Removed[0] != s)
            {
                throw new InvalidOperationException("Added or Removed args are incorrect");
            }
        }


        //        /// <summary>
        //        /// Create a stroke with button info and save it
        //        /// Expected: no loss in button data
        //        /// </summary>
        //        private void TestCreateAStrokeWithButtonInfoAndSaveIt()
        //        {
        //            StylusPacketValueMetric[] props = new StylusPacketValueMetric[3];
        //
        //            props[0] = new StylusPacketValueMetric(StylusPacketValue.X, Int32.MinValue, Int32.MaxValue, StylusPacketValueUnit.Centimeters, (float)1000.000);
        //            props[1] = new StylusPacketValueMetric(StylusPacketValue.Y, Int32.MinValue, Int32.MaxValue, StylusPacketValueUnit.Centimeters, (float)1000.000);
        //            props[2] = new StylusPacketValueMetric(StylusPacketValue.NormalPressure, 0, 1023, StylusPacketValueUnit.Default, (float)1.0);
        //
        //            Guid buttonGuid1 = Guid.NewGuid();
        //            Guid buttonGuid2 = Guid.NewGuid();
        //            StylusPacketDescription packetDescription = new StylusPacketDescription(props, new Guid[] { buttonGuid1, buttonGuid2 });
        //            Stroke stroke = new Stroke(packetDescription, Matrix.Identity, new DrawingAttributes());
        //
        //            StrokeCollection strokes = new StrokeCollection();
        //
        //            stroke.AppendPackets(new int[] { 1, 2, 3, 0x1 | 0x2 });
        //            DrtHelpers.CompareArray<int>(stroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2 });
        ////            DrtHelpers.CompareArray<Point>(stroke.GetPoints(), new StylusPoint[] { new StylusPoint(1,2)});
        //            DrtHelpers.CompareArray<Point>(stroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2) });
        //
        //            stroke.AppendPackets(new int[] { 4, 5, 6, 0x1});
        //            DrtHelpers.CompareArray<int>(stroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1 });
        ////            DrtHelpers.CompareArray<Point>(stroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5)});
        //            DrtHelpers.CompareArray<Point>(stroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5) });
        //
        //            stroke.AppendPackets(new int[] { 7, 8, 9, 0x2 });
        //            DrtHelpers.CompareArray<int>(stroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2 });
        ////            DrtHelpers.CompareArray<Point>(stroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8)});
        //            DrtHelpers.CompareArray<Point>(stroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8) });
        //
        //            stroke.AppendPackets(new int[] { 10, 11, 12, 0 });
        //            DrtHelpers.CompareArray<int>(stroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2, 10, 11, 12, 0 });
        ////            DrtHelpers.CompareArray<Point>(stroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });
        //            DrtHelpers.CompareArray<Point>(stroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });
        //
        //            strokes.Add(stroke);
        //
        //            byte[] bytes = strokes.Save();
        //
        //            StrokeCollection restoredStrokes = new StrokeCollection(bytes);
        //            Stroke restoredStroke = restoredStrokes[0];
        //
        //            DrtHelpers.CompareArray<int>(restoredStroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2, 10, 11, 12, 0 });
        ////            DrtHelpers.CompareArray<Point>(restoredStroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });
        //            DrtHelpers.CompareArray<Point>(restoredStroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });
        //
        //            restoredStroke.AppendPackets(new int[] { 1, 2, 3, 0x1 | 0x2 });
        //            DrtHelpers.CompareArray<int>(restoredStroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2, 10, 11, 12, 0, 1, 2, 3, 0x1 | 0x2 });
        ////            DrtHelpers.CompareArray<Point>(restoredStroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11), new StylusPoint(1, 2) });
        //            DrtHelpers.CompareArray<Point>(restoredStroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11), new StylusPoint(1, 2) });
        //        }

        //            Guid buttonGuid1 = Guid.NewGuid();
        //            Guid buttonGuid2 = Guid.NewGuid();
        //            StylusPacketDescription packetDescription = new StylusPacketDescription(props, new Guid[] { buttonGuid1, buttonGuid2 });
        //            Stroke stroke = new Stroke(packetDescription, Matrix.Identity, new DrawingAttributes());

        //            StrokeCollection strokes = new StrokeCollection();

        //            stroke.AppendPackets(new int[] { 1, 2, 3, 0x1 | 0x2 });
        //            DrtHelpers.CompareArray<int>(stroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2 });
        ////            DrtHelpers.CompareArray<Point>(stroke.GetPoints(), new StylusPoint[] { new StylusPoint(1,2)});
        //            DrtHelpers.CompareArray<Point>(stroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2) });

        //            stroke.AppendPackets(new int[] { 4, 5, 6, 0x1});
        //            DrtHelpers.CompareArray<int>(stroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1 });
        ////            DrtHelpers.CompareArray<Point>(stroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5)});
        //            DrtHelpers.CompareArray<Point>(stroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5) });

        //            stroke.AppendPackets(new int[] { 7, 8, 9, 0x2 });
        //            DrtHelpers.CompareArray<int>(stroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2 });
        ////            DrtHelpers.CompareArray<Point>(stroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8)});
        //            DrtHelpers.CompareArray<Point>(stroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8) });

        //            stroke.AppendPackets(new int[] { 10, 11, 12, 0 });
        //            DrtHelpers.CompareArray<int>(stroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2, 10, 11, 12, 0 });
        ////            DrtHelpers.CompareArray<Point>(stroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });
        //            DrtHelpers.CompareArray<Point>(stroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });

        //            strokes.Add(stroke);

        //            byte[] bytes = strokes.Save();

        //            StrokeCollection restoredStrokes = new StrokeCollection(bytes);
        //            Stroke restoredStroke = restoredStrokes[0];

        //            DrtHelpers.CompareArray<int>(restoredStroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2, 10, 11, 12, 0 });
        ////            DrtHelpers.CompareArray<Point>(restoredStroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });
        //            DrtHelpers.CompareArray<Point>(restoredStroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });

        //            DrtHelpers.CompareArray<int>(restoredStroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2, 10, 11, 12, 0 });
        //            DrtHelpers.CompareArray<Point>(restoredStroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });
        //            DrtHelpers.ComparePoints(restoredStroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11) });
        //            restoredStroke.AppendPackets(new int[] { 1, 2, 3, 0x1 | 0x2 });
        //            DrtHelpers.CompareArray<int>(restoredStroke.GetPacketData(), new int[] { 1, 2, 3, 0x1 | 0x2, 4, 5, 6, 0x1, 7, 8, 9, 0x2, 10, 11, 12, 0, 1, 2, 3, 0x1 | 0x2 });
        //            DrtHelpers.CompareArray<Point>(restoredStroke.GetPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11), new StylusPoint(1, 2) });
        //            DrtHelpers.ComparePoints(restoredStroke.GetRawPoints(), new StylusPoint[] { new StylusPoint(1, 2), new StylusPoint(4, 5), new StylusPoint(7, 8), new StylusPoint(10, 11), new StylusPoint(1, 2) });
        //        }

        /// <summary>
        /// Clip with lasso sometimes does not work when the ink is done with very fast movement of mouse/pen, pentip
        /// </summary>
        private void TestClipWithLassoSometimesDoesNotWorkWithFastMovement()
        {
            Point[] lassoPoints6 = new Point[]{
                                                new Point(0f, 15f),
                                                new Point(9f, 15f),
                                                new Point(12f, 16f),
                                                new Point(20f, 25f),
                                                new Point(20f, 90f),
                                                new Point(0f, 90f),
                                              };

            Stroke s6 = CreateStroke10And20And100();
            s6.DrawingAttributes.FitToCurve = false;
            s6.DrawingAttributes.StylusTip = StylusTip.Rectangle;
            s6.DrawingAttributes.Width = 5f;
            s6.DrawingAttributes.Height = 3f;

            StrokeCollection strokes6 = new StrokeCollection();
            strokes6.Add(s6);
            strokes6.Clip(lassoPoints6);

            if (strokes6.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }

            Stroke stroke8 = strokes6[0];

            // This should give stroke from 18 to 89
            if (!ValidateStroke18To20To89(stroke8))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }
        }

        // System.ArgumentException   when calling  StrokeCOllection.Clip( )
        private void TestSystemArgumentExceptionWhenCallingStrokeCollectionClip()
        {
            Point[] lassoPoints = new Point[]{  new Point    (255,139)  ,
                                                new Point    (210,134)  ,
                                                new Point    (113,126)  ,
                                                new Point    (105,126)  ,
                                                new Point    (79,151)   ,
                                                new Point    (79,155)   ,
                                                new Point    (101,174)  ,
                                                new Point    (142,176)  ,
                                                new Point    (211,185)  ,
                                                new Point    (218,187)  ,
                                                new Point    (275,204)  ,
                                                new Point    (276,206)  ,
                                                new Point    (273,228)  ,
                                                new Point    (270,230)  ,
                                                new Point    (226,254)  ,
                                                new Point    (184,254)  ,
                                                new Point    (176,254)  ,
                                                new Point    (102,258)  ,
                                                new Point    (95,260)   ,
                                                new Point    (64,283)   ,
                                                new Point    (63,285)   ,
                                                new Point    (80,317)   ,
                                                new Point    (83,321)   ,
                                                new Point    (121,337)  ,
                                                new Point    (124,337)  ,
                                                new Point    (163,337)  ,
                                                new Point    (206,331)  ,
                                                new Point    (211,330)  ,
                                                new Point    (251,313)  ,
                                                new Point    (279,318)  ,
                                                new Point    (277,353)  ,
                                                new Point    (268,359)  ,
                                                new Point    (208,369)  ,
                                                new Point    (200,370)  ,
                                                new Point    (114,372)  ,
                                                new Point    (107,372)  ,
                                                new Point    (81,378)   ,
                                                new Point    (81,380)   ,
                                                new Point    (85,406)   ,
                                                new Point    (86,406)   ,
                                                new Point    (117,398)  ,
                                                new Point    (171,389)  ,
                                                new Point    (175,389)  ,
                                                new Point    (217,389)  ,
                                                new Point    (223,390)  ,
                                                new Point    (251,391)  ,
                                                new Point    (254,391)  ,
                                                new Point    (281,391)  ,
                                                new Point    (285,391)  ,
                                                new Point    (298,388)  ,
                                                new Point    (299,377)  ,
                                                new Point    (299,376)  ,
                                                new Point    (299,375)  ,
                                                new Point    (299,374)  ,
                                                new Point    (301,370)  ,
                                                new Point    (309,354)  ,
                                                new Point    (310,353)  ,
                                                new Point    (322,331)  ,
                                                new Point    (324,330)  ,
                                                new Point    (336,288)  ,
                                                new Point    (255,139)};

            Stroke s =
                new Stroke(new StylusPointCollection(new StylusPoint[]
                                                    {   new StylusPoint    ( 205 ,92)  ,
                                                        new StylusPoint    (208 ,144)  ,
                                                        new StylusPoint    (212 ,183)  ,
                                                        new StylusPoint    (231 ,294 ) ,
                                                        new StylusPoint    (238 ,339)  ,
                                                        new StylusPoint    (251 ,383)  }));
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes1 = new StrokeCollection();
            strokes1.Add(s);
            strokes1.Clip(lassoPoints);

            if (strokes1.Count != 3)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }

            s.DrawingAttributes.Width = 30f;
            s.DrawingAttributes.Height = 30f;

            StrokeCollection strokes2 = new StrokeCollection();
            strokes2.Add(s);
            strokes2.Clip(lassoPoints);

            if (strokes2.Count != 2)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }


            Point[] lassoPoints2 = new Point[]{new Point(7,0),
                                               new Point(11,0),
                                               new Point(11,2),
                                               new Point(9,2),
                                               new Point(9,7),
                                               new Point(7,7)};

            Stroke s2 = new Stroke(new StylusPointCollection(
                                        new StylusPoint[]{
                                            new StylusPoint(10,1),
                                            new StylusPoint(10,10)}));
            StrokeCollection strokes3 = new StrokeCollection();
            strokes3.Add(s2);
            strokes3.Clip(lassoPoints2);
            if (strokes3.Count != 0)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }
        }

        // Extract with Lasso does not work correctly
        private void TestExtractWithLassoDoesNotWorkCorrectly()
        {

            ///////////////////////////////////////
            // 5. Try a more complicated case
            ///////////////////////////////////////
            Point[] lassoPoints5 = new Point[]{
                                               new Point(-10f, 0f),
                                               new Point(20f, 0f),
                                               new Point(0f, 10f),
                                               new Point(20f, 20f),
                                               new Point(0f, 30f),
                                               new Point(20f, 70f),
                                               new Point(-10f, 70f),
                                             };

            Stroke s5 = CreateStrokeFrom10To100();
            s5.DrawingAttributes.FitToCurve = false;
            s5.DrawingAttributes.StylusTip = StylusTip.Rectangle;
            s5.DrawingAttributes.Width = 5f;
            s5.DrawingAttributes.Height = 3f;

            StrokeCollection strokes5 = new StrokeCollection();
            strokes5.Add(s5);
            strokes5.Clip(lassoPoints5);

            if (strokes5.Count != 2)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }

            // This should give stroke from 17.75 to 22.25 and 56.5 to 68.5
            Stroke stroke6 = strokes5[0];
            Stroke stroke7 = strokes5[1];

            if (!ValidateStrokeFrom18To22(stroke6))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }

            if (!ValidateStrokeFrom57To69(stroke7))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }
        }

        private bool ValidateStrokeFrom18To22(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 3 ||
                points[0].X != 10f ||
                points[0].Y != 17.75f ||
                points[1].X != 10f ||
                points[1].Y != 20f ||
                points[2].X != 10f ||
                points[2].Y != 22.25f)
            {
                return false;
            }
            return true;
        }
        private bool ValidateStrokeFrom57To69(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;
            if (points.Count != 3 ||
                points[0].X != 10f ||
                points[0].Y != 56.5f ||
                points[1].X != 10f ||
                points[1].Y != 60f ||
                points[2].X != 10f ||
                points[2].Y != 68.5f)
            {
                return false;
            }
            return true;
        }


        private Stroke CreateStrokeFrom10To100()
        {
            //test a simple case
            StylusPoint[] strokePoints = new StylusPoint[]{
                                                new StylusPoint(10f, 10f),
                                                new StylusPoint(10f, 20f),
                                                new StylusPoint(10f, 30f),
                                                new StylusPoint(10f, 40f),
                                                new StylusPoint(10f, 50f),
                                                new StylusPoint(10f, 60f),
                                                new StylusPoint(10f, 70f),
                                                new StylusPoint(10f, 80f),
                                                new StylusPoint(10f, 90f),
                                                new StylusPoint(10f, 100f)
                                              };
            return new Stroke(new StylusPointCollection(strokePoints));
        }

        private bool ValidateStroke18To20To89(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 3 ||
                points[0].X != 10f ||
                points[0].Y != 18f ||
                points[1].X != 10f ||
                points[1].Y != 20f ||
                points[2].X != 10f ||
                points[2].Y != 89f)
            {
                return false;
            }
            return true;
        }
    }
}
