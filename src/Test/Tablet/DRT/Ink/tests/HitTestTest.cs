// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Ink.Internal;
using System.Windows.Input;

namespace DRT
{
    //used to test that derived strokes are created when point erasing
    public class MyStroke : Stroke
    {
        public MyStroke(StylusPointCollection stylusPoints)
            : base(stylusPoints)
        {
            CloneCalled = false;
        }

        public override Stroke Clone()
        {
            CloneCalled = true;
            return base.Clone();
        }

        public bool CloneCalled;
    }

    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class HitTestTest : DrtInkTestcase
    {
        public override void Run()
        {
            // StrokeCollection APIs
            TestGetBounds();
            TestClipWithLasso();
            TestClipWithRect();
            TestEraseWithLasso();
            TestEraseWithRect();
            TestEraseWithShape();
            TestHistTestWithPoint(); //StrokeCollectionHitTest(Point) and StrokeCollectionHitTest(Point, double)
            TestHitTestWithLasso();
            TestHitTestWithRect();
            TestHitTestWithShape();


            // IncrementalHitTesters
            TestIncrementalLassoHitTester();
            TestIncrementalStrokeHitTester();

            // Stroke APIs
            TestStrokeGetBounds();
            TestStrokeGetClipResultWithLasso();
            TestStrokeGetClipResultWithRect();
            TestStrokeGetEraseResultWithLasso();
            TestStrokeGetEraseResultWithRect();
            TestStrokeGetEraseResultWithShape();
            TestStrokeHitTestWithPoint();
            TestStrokeHitTestWithRect();
            TestStrokeHitTestWithLasso();
            TestStrokeHitTestWithShape();
            TestStrokeCollectionClipWithLine();

            TestClipEraseCloneResults();
            // done
            Success = true;
        }

        #region Test StrokeCollection APIs

        public void TestClipEraseCloneResults()
        {
            //test to ensure that when the full stroke is hit, a clone is returned.

            StylusPoint[] strokePoints = new StylusPoint[]{
                                                new StylusPoint(10d, 10d),
                                                new StylusPoint(20d, 20d),
                                              };

            Stroke stroke = new Stroke(new StylusPointCollection(strokePoints));

            StrokeCollection clipResults = stroke.GetClipResult(new Rect(new Point(0, 0), new Point(30, 30)));
            if (clipResults.Count != 1 ||
                object.ReferenceEquals(clipResults[0], stroke) ||
                clipResults[0].StylusPoints.Count != 2 ||
                clipResults[0].StylusPoints[0].X != stroke.StylusPoints[0].X ||
                clipResults[0].StylusPoints[0].Y != stroke.StylusPoints[0].Y ||
                clipResults[0].StylusPoints[1].X != stroke.StylusPoints[1].X ||
                clipResults[0].StylusPoints[1].Y != stroke.StylusPoints[1].Y)
            {
                throw new InvalidOperationException("Invalid GetClipResult return value");
            }


            StrokeCollection eraseResults = stroke.GetEraseResult(new Rect(new Point(0, 0), new Point(1, 1)));
            if (eraseResults.Count != 1 ||
                object.ReferenceEquals(eraseResults[0], stroke) ||
                eraseResults[0].StylusPoints.Count != 2 ||
                eraseResults[0].StylusPoints[0].X != stroke.StylusPoints[0].X ||
                eraseResults[0].StylusPoints[0].Y != stroke.StylusPoints[0].Y ||
                eraseResults[0].StylusPoints[1].X != stroke.StylusPoints[1].X ||
                eraseResults[0].StylusPoints[1].Y != stroke.StylusPoints[1].Y)
            {
                throw new InvalidOperationException("Invalid GetEraseResult return value");
            }

        }

        public void TestStrokeCollectionClipWithLine()
        {
            Stroke stroke1 = new Stroke(new StylusPointCollection(new Point[] { new Point(100, 100), new Point(200, 200), new Point(300, 300) }));
            Stroke stroke2 = new Stroke(new StylusPointCollection(new Point[] { new Point(120, 120), new Point(220, 220), new Point(320, 320) }));
            Stroke stroke3 = new Stroke(new StylusPointCollection(new Point[] { new Point(140, 140), new Point(240, 240), new Point(340, 340) }));

            StrokeCollection sc = new StrokeCollection();
            sc.Add(stroke1);
            sc.Add(stroke2);
            sc.Add(stroke3);

            Point[] lassoLine = new Point[] { new Point(100, 150), new Point(400, 150) };
            Point[] lassoPoint = new Point[] { new Point(100, 150) };

            if (stroke1.GetClipResult(lassoLine).Count != 0 ||
                stroke2.GetClipResult(lassoLine).Count != 0 ||
                stroke3.GetClipResult(lassoLine).Count != 0 ||
                stroke1.GetClipResult(lassoPoint).Count != 0 ||
                stroke1.GetClipResult(lassoPoint).Count != 0 ||
                stroke1.GetClipResult(lassoPoint).Count != 0)
            {
                throw new InvalidOperationException("Bad results clip testing with a point or line, expected zero count");
            }

            StrokeCollection sc2 = sc.Clone();
            sc.Clip(lassoLine);
            sc2.Clip(lassoPoint);
            if (sc.Count != 0 || sc2.Count != 0)
            {
                throw new InvalidOperationException("Bad results clip testing a strokecollection with a point or line, expected zero count");
            }
        }
        /// <summary>
        /// StrokeCollection.GetBounds()
        /// </summary>
        public void TestGetBounds()
        {
            // Test 1: a simple case (a vertical line)
            Stroke s = CreateDefaltHeightWidthStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(s);

            Rect rect = strokes.GetBounds();
            Rect rectp = new Rect(10f - s.DrawingAttributes.Width / 2,
                                    10f - s.DrawingAttributes.Height / 2,
                                    s.DrawingAttributes.Width,
                                    (100f - 10f) + s.DrawingAttributes.Height);

            if (!DoubleUtil.AreClose(rect, rectp))
            {
                throw new InvalidOperationException("StrokeCollection.GetBounds failed to return the correct bounds for strokes");
            }


            // Test 2: another simplecase
            StylusPoint[] strokePoints = new StylusPoint[]{
                                                new StylusPoint(10d, 10d),
                                                new StylusPoint(20d, 20d),
                                                new StylusPoint(20d, 30d),
                                                new StylusPoint(30d, 40d),
                                                new StylusPoint(30d, 50d),
                                                new StylusPoint(50d, 60d),
                                                new StylusPoint(70d, 70d),
                                                new StylusPoint(80d, 80d),
                                                new StylusPoint(90d, 90d)
                                              };
            Stroke stroke2 = new Stroke(new StylusPointCollection(strokePoints));
            StrokeCollection strokes2 = new StrokeCollection();
            strokes2.Add(stroke2);

            Rect rect2 = strokes2.GetBounds();
            Rect rect2p = new Rect(10d - stroke2.DrawingAttributes.Width / 2,
                                   10d - stroke2.DrawingAttributes.Height / 2,
                                   82.00314960629936, //80d + stroke2.DrawingAttributes.Width,
                                   80d + stroke2.DrawingAttributes.Height
                                   );

            if (!DoubleUtil.AreClose(rect2, rect2p))
            {
                throw new InvalidOperationException("StrokeCollection.GetBounds failed to return the correct bounds for strokes");
            }

            // Test 3:  a customized tip width and height
            StylusPoint[] strokePoints3 = new StylusPoint[]{
                                                new StylusPoint(10f, 10f),
                                                new StylusPoint(20f, 20f),
                                                new StylusPoint(20f, 30f),
                                                new StylusPoint(30f, 40f),
                                                new StylusPoint(30f, 50f),
                                                new StylusPoint(50f, 60f),
                                                new StylusPoint(70f, 70f),
                                                new StylusPoint(80f, 80f),
                                                new StylusPoint(90f, 90f),
                                                new StylusPoint(100f, 100f)
                                              };
            Stroke stroke3 = new Stroke(new StylusPointCollection(strokePoints3));
            stroke3.DrawingAttributes.Width = 1000f;
            stroke3.DrawingAttributes.Height = 1000f;
            StrokeCollection strokes3 = new StrokeCollection();
            strokes3.Add(stroke3);

            Rect rect3 = strokes3.GetBounds();
            Rect rect3p = new Rect(10f - stroke3.DrawingAttributes.Width / 2,
                                   10f - stroke3.DrawingAttributes.Height / 2,
                                   90f + stroke3.DrawingAttributes.Width,
                                   90f + stroke3.DrawingAttributes.Height);

            if (!DoubleUtil.AreClose(rect3, rect3p))
            {
                throw new InvalidOperationException("StrokeCollection.GetBounds failed to return the correct bounds for strokes");
            }

            // Test 4: a strokecollection with two strokes
            StrokeCollection strokes4 = new StrokeCollection();
            strokes4.Add(s);
            strokes4.Add(stroke2);
            Rect rect4 = strokes4.GetBounds();

            Rect rect4p = new Rect(10f - stroke2.DrawingAttributes.Width / 2,
                                   10f - stroke2.DrawingAttributes.Height / 2,
                                   82.00314960629936, //(90f - 10f) + stroke2.DrawingAttributes.Width,
                                   (100f - 10f) + stroke2.DrawingAttributes.Height);

            if (!DoubleUtil.AreClose(rect4, rect4p))
            {
                throw new InvalidOperationException("StrokeCollection.GetBounds failed to return the correct bounds for strokes");
            }

        }

        /// <summary>
        /// StrokeCollection.Clip(Point[] lassoPoints)
        /// Test clipping with lasso points:
        /// </summary>
        public void TestClipWithLasso()
        {
            ////////////////////////
            // 1. test a simple case
            ////////////////////////
            Point[] lassoPoints = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(s);
            strokes.Clip(lassoPoints);

            if (strokes.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }

            Stroke stroke = strokes[0];

            if (!ValidateStrokeFrom10To24(stroke))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }


            //////////////////////////////
            // 2. test another simple case
            /////////////////////////////
            Point[] lassoPoints2 = new Point[]{
                                                new Point(0f, 85f),
                                                new Point(25f, 85f),
                                                new Point(25f, 110f),
                                                new Point(0f, 110f)
                                              };

            Stroke s2 = CreateStrokeFrom10To100();
            s2.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes2 = new StrokeCollection();
            strokes2.Add(s);

            strokes2.Clip(lassoPoints2);

            if (strokes2.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }

            Stroke stroke2 = strokes2[0];

            if (!ValidateStrokeFrom86To100(stroke2))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }


            ////////////////////////////////////////////
            // 3. now try a little bit of a complex case
            ////////////////////////////////////////////
            Point[] lassoPoints3 = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f),
                                                new Point(0f, 85f),
                                                new Point(25f, 85f),
                                                new Point(25f, 110f),
                                                new Point(0f, 110f)
                                              };


            Stroke s3 = CreateStrokeFrom10To100();
            s3.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes3 = new StrokeCollection();
            strokes3.Add(s);

            strokes3.Clip(lassoPoints3);

            if (strokes3.Count != 2)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }

            Stroke stroke3 = strokes3[0];
            Stroke stroke4 = strokes3[1];

            if (!ValidateStrokeFrom10To24(stroke3))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }

            if (!ValidateStrokeFrom86To100(stroke4))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }

            ///////////////////////////////////////
            // 4. Try another more complicated case
            ///////////////////////////////////////
            Point[] lassoPoints4 = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(20f, 0f),
                                                new Point(5f, 37.5f),
                                                new Point(0f, 38f)
                                              };

            Stroke s4 = CreateStrokeFrom10To100();
            s4.DrawingAttributes.FitToCurve = false;
            s4.DrawingAttributes.StylusTip = StylusTip.Rectangle;
            s4.DrawingAttributes.Width = 5f;
            s4.DrawingAttributes.Height = 3f;

            StrokeCollection strokes4 = new StrokeCollection();
            strokes4.Add(s4);
            strokes4.Clip(lassoPoints4);

            if (strokes4.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }

            Stroke stroke5 = strokes4[0];

            if (!ValidateStrokeFrom10To17(stroke5))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }
        }

        /// <summary>
        /// StrokeCollection.Clip(Rect rect)
        /// </summary>
        public void TestClipWithRect()
        {
            ////////////////////////
            // 1. test a simple case
            ////////////////////////
            Rect rect = new Rect(0, 0, 25, 25);
            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(s);
            strokes.Clip(rect);

            if (strokes.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct number of strokes");
            }

            Stroke stroke = strokes[0];

            if (!ValidateStrokeFrom10To24(stroke))
            {
                throw new InvalidOperationException("StrokeCollection.Clip failed to create the correct points in the stroke");
            }

        }

        /// <summary>
        /// StrokeCollection.Erase(Point[] lassoPoints)
        /// </summary>
        public void TestEraseWithLasso()
        {
            //1. test a simple case
            Point[] lassoPoints = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(s);

            strokes.Erase(lassoPoints);

            if (strokes.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct number of strokes");
            }

            Stroke stroke = strokes[0];
            if (!ValidateStrokeFrom26To100(stroke))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }


            //2. test another simple case
            Point[] lassoPoints2 = new Point[]{
                                                new Point(0f, 85f),
                                                new Point(25f, 85f),
                                                new Point(25f, 110f),
                                                new Point(0f, 110f)
                                              };

            Stroke s2 = CreateStrokeFrom10To100();
            s2.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes2 = new StrokeCollection();
            strokes2.Add(s);

            strokes2.Erase(lassoPoints2);

            if (strokes2.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct number of strokes");
            }

            Stroke stroke2 = strokes2[0];

            if (!ValidateStrokeFrom10To84(stroke2))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }


            //3. now try a little bit of a complex case
            Point[] lassoPoints3 = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f),
                                                new Point(0f, 85f),
                                                new Point(25f, 85f),
                                                new Point(25f, 110f),
                                                new Point(0f, 110f)
                                              };


            Stroke s3 = CreateStrokeFrom10To100();
            s3.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes3 = new StrokeCollection();
            strokes3.Add(s);

            strokes3.Erase(lassoPoints3);

            if (strokes3.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct number of strokes");
            }

            Stroke stroke3 = strokes3[0];

            if (!ValidateStrokeFrom26To84(stroke3))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }

            //4. A complex case
            StylusPoint[] strokePoints4 = new StylusPoint[] {new StylusPoint	 (405.969420453505,415.222632526627)	,
                                                new StylusPoint	 (429.99359111301,410.022278511917)	    ,
                                                new StylusPoint	 (473.061311685537,402.990813928648)	,
                                                new StylusPoint	 (500.015259254738,402.990813928648)	,
                                                new StylusPoint	 (498.062074648274,404.016235847041)	,
                                                new StylusPoint	 (469.057283242286,411.999877925962)	,
                                                new StylusPoint	 (421.985534226508,424.011963255715)	,
                                                new StylusPoint	 (392.394787438581,430.787072359386)
                                                };
            Point[] lassoPoints4 = new Point[] {new Point	 (451.966917935728,395.007171849727)	,
                                                new Point	 (451.966917935728,395.007171849727)	,
                                                new Point	 (453.041169469283,394.018372142705)	,
                                                new Point	 (453.041169469283,394.018372142705)	,
                                                new Point	 (451.966917935728,394.018372142705)	,
                                                new Point	 (451.966917935728,394.018372142705)	,
                                                new Point	 (450.990325632496,394.018372142705)	,
                                                new Point	 (449.037141026032,394.018372142705)	,
                                                new Point	 (448.0605487228,394.018372142705)	,
                                                new Point	 (446.986297189245,394.018372142705)	,
                                                new Point	 (446.009704886013,394.018372142705)	,
                                                new Point	 (445.033112582781,394.018372142705)	,
                                                new Point	 (442.005676442763,395.007171849727)	,
                                                new Point	 (440.052491836299,395.995971556749)	,
                                                new Point	 (438.001647999512,397.021393475143)	,
                                                new Point	 (434.974211859493,398.010193182165)	,
                                                new Point	 (432.044434949797,398.998992889187)	,
                                                new Point	 (429.016998809778,401.013214514603)	,
                                                new Point	 (425.989562669759,402.002014221625)	,
                                                new Point	 (423.059785760064,404.016235847041)	,
                                                new Point	 (420.032349620045,405.993835261086)	,
                                                new Point	 (417.004913480026,408.008056886502)	,
                                                new Point	 (413.977477340007,410.022278511917)	,
                                                new Point	 (411.047700430311,413.025299844356)	,
                                                new Point	 (407.04367198706,415.0028992584)	,
                                                new Point	 (404.016235847041,418.005920590838)	,
                                                new Point	 (400.988799707022,420.020142216254)	,
                                                new Point	 (399.035615100559,423.023163548692)	,
                                                new Point	 (396.00817896054,425.000762962737)	,
                                                new Point	 (394.054994354076,428.003784295175)	,
                                                new Point	 (392.004150517289,430.018005920591)	,
                                                new Point	 (390.050965910825,433.021027253029)	,
                                                new Point	 (388.000122074038,434.998626667074)	,
                                                new Point	 (388.000122074038,437.012848292489)	,
                                                new Point	 (387.023529770806,440.015869624928)	,
                                                new Point	 (387.023529770806,443.018890957366)	,
                                                new Point	 (387.023529770806,444.99649037141)	,
                                                new Point	 (387.023529770806,447.999511703848)	,
                                                new Point	 (387.023529770806,450.013733329264)	,
                                                new Point	 (388.000122074038,451.002533036286)	,
                                                new Point	 (388.97671437727,453.016754661702)	,
                                                new Point	 (390.050965910825,454.005554368725)	,
                                                new Point	 (392.980742820521,456.01977599414)	,
                                                new Point	 (395.031586657308,457.997375408185)	,
                                                new Point	 (399.035615100559,460.011597033601)	,
                                                new Point	 (402.063051240577,462.025818659017)	,
                                                new Point	 (404.992828150273,463.014618366039)	,
                                                new Point	 (408.996856593524,464.003418073061)	,
                                                new Point	 (413.000885036775,464.003418073061)	,
                                                new Point	 (416.028321176794,463.014618366039)	,
                                                new Point	 (420.032349620045,462.025818659017)	,
                                                new Point	 (423.059785760064,460.011597033601)	,
                                                new Point	 (427.063814203314,457.008575701163)	,
                                                new Point	 (429.99359111301,453.016754661702)	,
                                                new Point	 (433.997619556261,449.024933622242)	,
                                                new Point	 (437.02505569628,444.007690664388)	,
                                                new Point	 (440.052491836299,441.00466933195)	,
                                                new Point	 (442.005676442763,438.001647999512)	,
                                                new Point	 (444.05652027955,434.998626667074)	,
                                                new Point	 (445.033112582781,431.995605334635)	,
                                                new Point	 (445.033112582781,428.003784295175)	,
                                                new Point	 (446.009704886013,424.011963255715)	,
                                                new Point	 (446.009704886013,421.008941923276)	,
                                                new Point	 (446.986297189245,417.017120883816)	,
                                                new Point	 (446.986297189245,413.025299844356)	,
                                                new Point	 (446.986297189245,410.022278511917)	,
                                                new Point	 (446.009704886013,407.019257179479)	,
                                                new Point	 (445.033112582781,405.005035554064)	,
                                                new Point	 (444.05652027955,402.990813928648)	,
                                                new Point	 (442.982268745994,402.002014221625)	,
                                                new Point	 (442.982268745994,400.024414807581)	,
                                                new Point	 (441.029084139531,398.998992889187)	,
                                                new Point	 (440.052491836299,398.010193182165)	,
                                                new Point	 (438.978240302744,395.995971556749)	,
                                                new Point	 (438.001647999512,394.018372142705)	,
                                                new Point	 (438.001647999512,392.992950224311)	,
                                                new Point	 (437.02505569628,392.992950224311)	,
                                                new Point	 (437.02505569628,392.992950224311)	,
                                                new Point	 (437.02505569628,392.992950224311)	,
                                                new Point	 (436.048463393048,392.992950224311)	,
                                                new Point	 (434.974211859493,392.992950224311)	,
                                                new Point	 (433.997619556261,392.992950224311)	,
                                                new Point	 (433.021027253029,392.992950224311)	,
                                                new Point	 (432.044434949797,392.992950224311)	,
                                                new Point	 (430.970183416242,394.018372142705)	,
                                                new Point	 (430.970183416242,394.018372142705)	,
                                                new Point	 (430.970183416242,395.007171849727)	,
                                                new Point	 (430.970183416242,395.007171849727)
                                            };
            Stroke s4 = new Stroke(new StylusPointCollection(strokePoints4));
            StrokeCollection strokes4 = new StrokeCollection();
            strokes4.Add(s4);
            strokes4.Erase(lassoPoints4);
        }

        /// <summary>
        /// StrokeCollection.Erase(Rect rect)
        /// </summary>
        public void TestEraseWithRect()
        {
            //1. test a simple case
            Rect rect = new Rect(0, 0, 25, 25);
            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(s);

            strokes.Erase(rect);

            if (strokes.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct number of strokes");
            }

            Stroke stroke = strokes[0];
            if (!ValidateStrokeFrom26To100(stroke))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }
        }

        /// <summary>
        /// StrokeCollection.Erase(Point[], StylusShape)
        /// </summary>
        public void TestEraseWithShape()
        {
            ////////////////////////
            // 1. test a simple case
            //    inking stylus tip is elliptical
            ////////////////////////
            Point[] erasingPoints = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;


            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(s);

            strokes.Erase(erasingPoints, new EllipseStylusShape(4f, 4f, 0f));

            if (strokes.Count != 2)
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct number of strokes");
            }

            Stroke stroke10 = strokes[0];
            Stroke stroke11 = strokes[1];
            if (!ValidateStrokeFrom10To22(stroke10))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }
            if (!ValidateStrokeFrom28To100(stroke11))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }


            //try it with derived strokes
            MyStroke ms = CreateCustomStrokeFrom10To100();
            ms.DrawingAttributes.FitToCurve = false;


            StrokeCollection mstrokes = new StrokeCollection();
            mstrokes.Add(ms);

            mstrokes.Erase(erasingPoints, new EllipseStylusShape(4f, 4f, 0f));

            if (mstrokes.Count != 2)
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct number of strokes");
            }

            Stroke mstroke10 = mstrokes[0];
            Stroke mstroke11 = mstrokes[1];
            if (!ValidateStrokeFrom10To22(mstroke10))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }
            if (!ValidateStrokeFrom28To100(mstroke11))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }

            if (mstroke10.GetType() != typeof(MyStroke) ||
                mstroke11.GetType() != typeof(MyStroke))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create derived strokes");
            }

            if (!((MyStroke)mstroke10).CloneCalled ||
                !((MyStroke)mstroke11).CloneCalled)
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to call Clone on derived strokes");
            }


            ////////////////////////
            // 2. test a simple case
            //    inking stylus tip is rectangle
            ////////////////////////
            Point[] erasingPoints2 = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(11f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s2 = CreateStrokeFrom10To100();
            s2.DrawingAttributes.FitToCurve = false;
            s2.DrawingAttributes.StylusTip = StylusTip.Rectangle;
            s2.DrawingAttributes.Width = 4f;
            s2.DrawingAttributes.Height = 2f;

            StrokeCollection strokes2 = new StrokeCollection();
            strokes2.Add(s2);

            strokes2.Erase(erasingPoints2, new EllipseStylusShape(6f, 6f, 0f));

            if (strokes2.Count != 2)
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct number of strokes");
            }

            Stroke stroke20 = strokes2[0];
            Stroke stroke21 = strokes2[1];
            if (!ValidateStrokeFrom10To16(stroke20))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }
            if (!ValidateStrokeFrom29To100(stroke21))
            {
                throw new InvalidOperationException("StrokeCollection.Erase failed to create the correct points in the stroke");
            }

        }

        /// <summary>
        /// Test StrokeCollection.HitTest(Point point)
        ///      StrokeCollection.HitTest(Point point, double diameter)
        /// </summary>
        public void TestHistTestWithPoint()
        {
            // Test 1:  StrokeCollection.HitTest(Point point)
            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(s);

            StrokeCollection topHits = strokes.HitTest(new Point(10, 10));

            if (topHits.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes");
            }
            if (topHits[0] != s)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct stroke");
            }

            // Test 2:  StrokeCollection.HitTest(Point point, double diameter)
            Stroke s2 = CreateStrokeFrom10To100();
            s2.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes2 = new StrokeCollection();
            strokes2.Add(s2);

            StrokeCollection topHits2 = strokes2.HitTest(new Point(10, 17.5), 1f);

            if (topHits2.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes2");
            }
            if (topHits2[0] != s2)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct stroke");
            }

            //now test multiple strokes
            strokes.Add(s2);
            StrokeCollection topHits3 = strokes.HitTest(new Point(10, 10));

            if (topHits3.Count != 2)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes");
            }
            if (topHits3[0] != s || topHits3[1] != s2)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct stroke");
            }

            //
            // verify hit testing with out of range diameters
            //
            int exceptionCount = 0;
            try
            {
                StrokeCollection topHits4 = strokes.HitTest(new Point(10, 10), DrawingAttributes.MinWidth - 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionCount++;
            }

            if (exceptionCount != 1)
            {
                throw new InvalidOperationException("Incorrectly allowed to hit test by point with a diameter that is too small");
            }

            try
            {
                StrokeCollection topHits4 = strokes.HitTest(new Point(10, 10), DrawingAttributes.MaxWidth + 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionCount++;
            }

            if (exceptionCount != 2)
            {
                throw new InvalidOperationException("Incorrectly allowed to hit test by point with a diameter that is too big");
            }

        }

        /// <summary>
        /// Test StrokeCollection.HitTest(Point[] lassoPoints, int percentage)
        /// </summary>
        public void TestHitTestWithLasso()
        {
            // 1. Test a simple case (a vertical line)
            Stroke s1 = CreateStrokeFrom10To100();
            s1.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes1 = new StrokeCollection();
            strokes1.Add(s1);

            Point[] lassoPoints1 = new Point[] { new Point(5, 5), new Point(5, 20), new Point(20, 20), new Point(20, 5) };
            StrokeCollection topHits = strokes1.HitTest(lassoPoints1, 50);

            if (topHits.Count != 0)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes");
            }

            lassoPoints1 = new Point[] { new Point(0, 0), new Point(20, 0), new Point(20, 50), new Point(0, 50) };
            topHits = strokes1.HitTest(lassoPoints1, 40);

            if (topHits.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes");
            }

            if (topHits[0] != s1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct stroke");
            }


            ///////////////////////////////////////
            // 2. Try a more complicated case
            ///////////////////////////////////////
            Point[] lassoPoints2 = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(20f, 0f),
                                                new Point(5f, 37.5f),
                                                new Point(0f, 38f)
                                              };

            Stroke s2 = CreateStrokeFrom10To100();
            s2.DrawingAttributes.FitToCurve = false;
            s2.DrawingAttributes.StylusTip = StylusTip.Rectangle;
            s2.DrawingAttributes.Width = 5f;
            s2.DrawingAttributes.Height = 3f;

            StrokeCollection strokes2 = new StrokeCollection();
            strokes2.Add(s2);
            topHits = strokes2.HitTest(lassoPoints2, 18);

            if (topHits.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest with lasso points failed");
            }

            if (topHits[0] != s2)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct stroke");
            }

            ///////////////////////////////////////
            // 3
            ///////////////////////////////////////
            Point[] lassoPoints3 = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(20f, 0f),
                                                new Point(5f, 37.5f),
                                                new Point(0f, 38f)
                                              };

            Stroke s3 = new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(5, 5) }));
            s3.DrawingAttributes.FitToCurve = false;
            s3.DrawingAttributes.StylusTip = StylusTip.Rectangle;
            s3.DrawingAttributes.Width = 5f;
            s3.DrawingAttributes.Height = 3f;

            StrokeCollection strokes3 = new StrokeCollection();
            strokes3.Add(s3);
            topHits = strokes3.HitTest(lassoPoints3, 99);

            if (topHits.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest with lasso points failed");
            }

            if (topHits[0] != s3)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct stroke");
            }

            lassoPoints3 = new Point[]{
                                        new Point(0f, 10f),
                                        new Point(20f, 10f),
                                        new Point(5f, 37.5f),
                                        new Point(0f, 38f)
                                        };

            topHits = strokes3.HitTest(lassoPoints3, 50);

            if (topHits.Count != 0)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes");
            }


        }

        /// <summary>
        /// Test StrokeCollection.HitTest(Rect rect, int percentage)
        /// </summary>
        public void TestHitTestWithRect()
        {
            // 1. Test a simple case (a vertical line)
            Stroke s1 = CreateStrokeFrom10To100();
            s1.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes1 = new StrokeCollection();
            strokes1.Add(s1);

            Rect rect = new Rect(0, 0, 25, 50);
            StrokeCollection topHits = strokes1.HitTest(rect, 70);

            if (topHits.Count != 0)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes");
            }

            Rect rect2 = new Rect(0, 0, 25, 75);
            topHits = strokes1.HitTest(rect2, 70);

            if (topHits.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes");
            }

            if (topHits[0] != s1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct stroke");
            }

        }

        /// <summary>
        /// Test StrokeCollection.HitTest(Point[], StylusShape)
        /// </summary>
        public void TestHitTestWithShape()
        {
            ////////////////////////
            // 1. test a simple case
            //    inking stylus tip is elliptical
            ////////////////////////
            Point[] erasingPoints = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;


            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(s);

            StrokeCollection hits = strokes.HitTest(erasingPoints, new EllipseStylusShape(4f, 4f, 0f));

            if (hits.Count != 1)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct number of strokes");
            }
            if (hits[0] != s)
            {
                throw new InvalidOperationException("StrokeCollection.HitTest failed to return the correct hit strokes");
            }
        }
        #endregion

        #region Test IncrementalHitTester
        /// <summary>
        /// Test IncrementalLassoHitTester
        /// </summary>
        public void TestIncrementalLassoHitTester()
        {
            // 1. Test a simple case (a vertical line)
            StrokeCollection strokes = new StrokeCollection();
            strokes.Add(new Stroke(new StylusPointCollection(new StylusPoint[] { new StylusPoint(100, 100), new StylusPoint(100, 200) })));
            IncrementalLassoHitTester incTester = strokes.GetIncrementalLassoHitTester(90);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester.AddPoint(new Point(110, 0));
            incTester.AddPoint(new Point(0, 150));
            incTester.AddPoint(new Point(110, 300));
            VerifyHitTestWithLasso(strokes, new StrokeCollection(), "TestIncrementalLassoHitTester");

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester.AddPoint(new Point(0, 300));
            VerifyHitTestWithLasso(new StrokeCollection(), strokes, "TestIncrementalLassoHitTester");
            incTester.EndHitTesting();

            bool caughtException = false;
            try
            {
                incTester.AddPoint(new Point(110, 12));
            }
            catch (InvalidOperationException)
            {
                // We expect an ArgumentException being thrown.
                caughtException = true;
            }
            if (caughtException == false)
            {
                throw new InvalidOperationException("IncrementalHitTester.AddPoint should throw exception if EndHitTesting has been called");
            }

            caughtException = false;
            try
            {
                incTester.AddPoints(new Point[] { new Point(110, 12), new Point(1, 5) });
            }
            catch (InvalidOperationException)
            {
                // We expect an ArgumentException being thrown.
                caughtException = true;
            }
            if (caughtException == false)
            {
                throw new InvalidOperationException("IncrementalHitTester.AddPoints should throw exception if EndHitTesting has been called");
            }

            caughtException = false;
            try
            {
                incTester.AddPoints(new Point[] { new Point(110, 12), new Point(1, 5) });
            }
            catch (InvalidOperationException)
            {
                // We expect an ArgumentException being thrown.
                caughtException = true;
            }
            if (caughtException == false)
            {
                throw new InvalidOperationException("IncrementalHitTester.AddPoints should throw exception if EndHitTesting has been called");
            }

            // 2. Test the weight calculation

            Stroke s2 = CreateStrokeFrom10To100();
            StrokeCollection strokes2 = new StrokeCollection();
            strokes2.Add(s2);
            IncrementalLassoHitTester incTester2 = strokes2.GetIncrementalLassoHitTester(99);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester2.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester2.AddPoint(new Point(0, 0));
            incTester2.AddPoint(new Point(20, 0));
            incTester2.AddPoint(new Point(20, 10));
            incTester2.AddPoint(new Point(20, 20));
            incTester2.AddPoints(new Point[]{new Point(20, 20),
                                            new Point(20, 25),
                                            new Point(20, 31)});
            incTester2.AddPoints(new Point[]{new Point(20, 36),
                                            new Point(20, 43.2),
                                            new Point(20, 58.9),
                                            new Point(20, 78.9)});
            incTester2.AddPoints(new Point[]{new Point(20, 78.9),
                                            new Point(20, 79.0),
                                            new Point(20, 85.0),
                                            new Point(20, 91)});
            incTester2.AddPoints(new Point[]{new Point(20, 95),
                                            new Point(20, 100.1),
                                            new Point(20, 110),
                                            new Point(10, 110)});
            incTester2.AddPoint(new Point(0, 100));
            VerifyHitTestWithLasso(strokes2, new StrokeCollection(), "TestIncrementalLassoHitTester");
            incTester2.EndHitTesting();


            // 3. Test on-closing-segment
            Stroke s3 = CreateStrokeFrom10To100();
            StrokeCollection strokes3 = new StrokeCollection();
            strokes3.Add(s3);
            IncrementalLassoHitTester incTester3 = strokes3.GetIncrementalLassoHitTester(99);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester3.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester3.AddPoint(new Point(0, 0));
            incTester3.AddPoint(new Point(20, 0));
            incTester3.AddPoint(new Point(20, 10));
            incTester3.AddPoint(new Point(20, 20));
            incTester3.AddPoints(new Point[]{new Point(20, 20),
                                            new Point(20, 25),
                                            new Point(20, 31)});
            incTester3.AddPoints(new Point[]{new Point(20, 36),
                                            new Point(20, 43.2),
                                            new Point(20, 58.9),
                                            new Point(20, 80.0)});
            incTester3.AddPoints(new Point[]{new Point(20, 78.9),
                                            new Point(20, 79.0),
                                            new Point(20, 85.0),
                                            new Point(20, 91)});
            incTester3.AddPoints(new Point[]{new Point(20, 95),
                                            new Point(20, 100)});
            incTester3.AddPoints(new Point[]{new Point(20, 110),
                                            new Point(10, 110)});
            incTester3.AddPoint(new Point(0, 100));
            VerifyHitTestWithLasso(strokes3, new StrokeCollection(), "TestIncrementalLassoHitTester");
            incTester3.EndHitTesting();

            // 4. reverse the order of AddPoints in test #3
            Stroke s4 = CreateStrokeFrom10To100();
            StrokeCollection strokes4 = new StrokeCollection();
            strokes4.Add(s4);
            IncrementalLassoHitTester incTester4 = strokes4.GetIncrementalLassoHitTester(99);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester4.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester4.AddPoint(new Point(0, 0));
            incTester4.AddPoint(new Point(0, 100));
            incTester4.AddPoints(new Point[]{new Point(10, 110),
                                            new Point(20, 110)});
            incTester4.AddPoints(new Point[]{new Point(20, 100),
                                            new Point(20, 95)});
            incTester4.AddPoints(new Point[]{new Point(20, 91),
                                             new Point(20, 85.0),
                                             new Point(20, 79.0),
                                             new Point(20, 78.9),
                                            });
            incTester4.AddPoints(new Point[]{new Point(20, 80.0),
                                            new Point(20, 58.9),
                                            new Point(20, 43.2),
                                            new Point(20, 36),
                                            });
            incTester4.AddPoints(new Point[]{new Point(20, 31),
                                            new Point(20, 25),
                                            new Point(20, 20)});
            incTester4.AddPoint(new Point(20, 20));
            incTester4.AddPoint(new Point(20, 10));
            incTester4.AddPoint(new Point(20, 0));
            VerifyHitTestWithLasso(strokes4, new StrokeCollection(), "TestIncrementalLassoHitTester");
            incTester4.EndHitTesting();

            // 5. Test horizontal closing segment
            Stroke s5 = CreateStrokeFrom10To100();
            StrokeCollection strokes5 = new StrokeCollection();
            strokes5.Add(s5);
            IncrementalLassoHitTester incTester5 = strokes5.GetIncrementalLassoHitTester(99);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester5.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester5.AddPoint(new Point(0, 50));
            incTester5.AddPoint(new Point(0, 0));
            incTester5.AddPoint(new Point(20, 0));
            incTester5.AddPoint(new Point(20, 10));
            incTester5.AddPoints(new Point[]{new Point(20, 20),
                                            new Point(20, 25),
                                            new Point(20, 31)});
            incTester5.AddPoints(new Point[]{new Point(20, 36),
                                            new Point(20, 43.2),
                                            new Point(20, 50)});
            incTester5.AddPoints(new Point[]{new Point(20, 51),
                                            new Point(20, 79.0),
                                            new Point(20, 85.0),
                                            new Point(20, 91)});
            incTester5.AddPoints(new Point[]{new Point(20, 95),
                                            new Point(20, 100)});
            incTester5.AddPoints(new Point[]{new Point(20, 110),
                                            new Point(10, 110)});
            incTester5.AddPoint(new Point(0, 100));
            VerifyHitTestWithLasso(strokes5, new StrokeCollection(), "TestIncrementalLassoHitTester");
            incTester5.EndHitTesting();

            // 6. Reverse the 5
            Stroke s6 = CreateStrokeFrom10To100();
            StrokeCollection strokes6 = new StrokeCollection();
            strokes6.Add(s6);
            IncrementalLassoHitTester incTester6 = strokes6.GetIncrementalLassoHitTester(99);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester6.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester6.AddPoint(new Point(0, 50));
            incTester6.AddPoint(new Point(0, 100));
            incTester6.AddPoint(new Point(10, 110));
            incTester6.AddPoint(new Point(20, 110));
            incTester6.AddPoints(new Point[]{new Point(20, 100),
                                            new Point(20, 96),
                                            new Point(20, 91)});
            incTester6.AddPoints(new Point[]{new Point(20, 86),
                                            new Point(20, 79),
                                            new Point(20, 50)});
            incTester6.AddPoints(new Point[]{new Point(20, 40),
                                            new Point(20, 30),
                                            new Point(20, 20),
                                            new Point(20, 10)});
            incTester6.AddPoints(new Point[]{new Point(20, 0),
                                            new Point(0, 0)});
            VerifyHitTestWithLasso(strokes6, new StrokeCollection(), "TestIncrementalLassoHitTester");
            incTester6.EndHitTesting();

            // 7. Test lasso with a loop
            Stroke s7 = CreateStrokeFrom10To100();
            Stroke s7_2 = new Stroke(new StylusPointCollection(new StylusPoint[]{new StylusPoint(40, 20), new StylusPoint(40, 30)}));
            StrokeCollection strokes7 = new StrokeCollection();
            strokes7.Add(s7);
            strokes7.Add(s7_2);
            IncrementalLassoHitTester incTester7 = strokes7.GetIncrementalLassoHitTester(90);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester7.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester7.AddPoint(new Point(50, 0));
            incTester7.AddPoint(new Point(20, 30));
            incTester7.AddPoint(new Point(20, 110));
            incTester7.AddPoint(new Point(0, 110));
            incTester7.AddPoints(new Point[]{new Point(0, 15)});
            StrokeCollection hits7 = new StrokeCollection();
            hits7.Add(s7);
            VerifyHitTestWithLasso(hits7, new StrokeCollection(), "TestIncrementalLassoHitTester");
            incTester7.AddPoints(new Point[]{new Point(20, 15),
                                            new Point(50, 50)});

            incTester7.AddPoints(new Point[]{new Point(60, 50),
                                            new Point(50, 0)});

            VerifyHitTestWithLasso(hits7, new StrokeCollection(), "TestIncrementalLassoHitTester");

            incTester7.EndHitTesting();

            // 8. Test lasso with two loops. First loop at lasso point
            Stroke s8 = CreateStrokeFrom10To100();
            Stroke s8_2 = new Stroke(new StylusPointCollection(new StylusPoint[]{new StylusPoint(40, 20), new StylusPoint(40, 30)}));
            StrokeCollection strokes8 = new StrokeCollection();
            strokes8.Add(s8);
            strokes8.Add(s8_2);
            IncrementalLassoHitTester incTester8 = strokes8.GetIncrementalLassoHitTester(90);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester8.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester8.AddPoints(new Point[]{new Point(50, 0), new Point(30, 20)});
            incTester8.AddPoint(new Point(20, 20));
            incTester8.AddPoint(new Point(20, 110));
            incTester8.AddPoint(new Point(0, 110));
            incTester8.AddPoints(new Point[]{new Point(0, 15)});
            StrokeCollection hits8 = new StrokeCollection();
            hits8.Add(s8);
            VerifyHitTestWithLasso(hits8, new StrokeCollection(), "TestIncrementalLassoHitTester");
            incTester8.AddPoints(new Point[]{new Point(15, 15),
                                            new Point(50, 50)});
            incTester8.AddPoints(new Point[]{new Point(60, 50),
                                            new Point(50, 0)});

            VerifyHitTestWithLasso(hits8, new StrokeCollection(), "TestIncrementalLassoHitTester");

            incTester8.EndHitTesting();



            // 9. Test lasso with loop. Special case looped at the first point
            Stroke s9 = CreateStrokeFrom10To100();
            Stroke s9_2 = new Stroke(new StylusPointCollection(new StylusPoint[]{new StylusPoint(40, 20), new StylusPoint(40, 30)}));
            StrokeCollection strokes9 = new StrokeCollection();
            strokes9.Add(s9);
            strokes9.Add(s9_2);
            IncrementalLassoHitTester incTester9 = strokes9.GetIncrementalLassoHitTester(90);

            _hitStrokes = null;
            _unhitStrokes = null;
            incTester9.SelectionChanged += new LassoSelectionChangedEventHandler(OnLassoSelectionChanged);
            incTester9.AddPoints(new Point[]{new Point(50, 0), new Point(30, 20)});
            incTester9.AddPoint(new Point(20, 20));
            incTester9.AddPoint(new Point(20, 110));
            incTester9.AddPoint(new Point(0, 110));
            incTester9.AddPoints(new Point[]{new Point(0, 15)});
            StrokeCollection hits9 = new StrokeCollection();
            hits9.Add(s9);
            VerifyHitTestWithLasso(hits9, new StrokeCollection(), "TestIncrementalLassoHitTester");
            incTester9.AddPoints(new Point[]{new Point(15, 15),
                                            new Point(50, 0)});
            incTester9.AddPoints(new Point[]{new Point(60, 50),
                                            new Point(50, 0)});

            VerifyHitTestWithLasso(hits9, new StrokeCollection(), "TestIncrementalLassoHitTester");

            incTester9.EndHitTesting();


        }

        /// <summary>
        /// Test IncrementalStrokeHitTester
        /// </summary>
        public void TestIncrementalStrokeHitTester()
        {
            // 2. Test the weight calculation
            Stroke s1 = CreateStrokeFrom10To100();
            _incrementalStrokeHitTesterStrokes = new StrokeCollection();
            _incrementalStrokeHitTesterStrokes.Add(s1);
            IncrementalStrokeHitTester incTester1 = _incrementalStrokeHitTesterStrokes.GetIncrementalStrokeHitTester(new RectangleStylusShape(2, 2, 0));

            incTester1.StrokeHit+= new StrokeHitEventHandler(OnPointEraseResultChanged);
            incTester1.AddPoint(new Point(0, 0));
            incTester1.AddPoint(new Point(20, 0));
            incTester1.AddPoint(new Point(20, 10));
            incTester1.AddPoint(new Point(0, 20));

            VerifyStrokeCount(_incrementalStrokeHitTesterStrokes, 2, "TestIncrementalStrokeHitTester");
            incTester1.AddPoints(new Point[]{new Point(0, 21),
                                            new Point(5, 25),
                                            new Point(10, 31),
                                            new Point(20, 40)});

            VerifyStrokeCount(_incrementalStrokeHitTesterStrokes, 3, "TestIncrementalStrokeHitTester");
            incTester1.EndHitTesting();

        }
        #endregion

        #region Test Stroke APIs
        /// <summary>
        ///
        /// </summary>
        public void TestStrokeGetBounds()
        {
            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            Rect rect = s.GetBounds();
            Rect rectp = new Rect(10f - s.DrawingAttributes.Width / 2,
                                    10f - s.DrawingAttributes.Height / 2,
                                    s.DrawingAttributes.Width,
                                    (100f - 10f) + s.DrawingAttributes.Height);

            if (!DoubleUtil.AreClose(rect, rectp))
            {
                throw new InvalidOperationException("Stroke.GetBounds failed to return the correct bounds");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void TestStrokeGetClipResultWithLasso()
        {
            Point[] lassoPoints = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = s.GetClipResult(lassoPoints);

            if (strokes.Count != 1)
            {
                throw new InvalidOperationException("Stroke.GetClipResult failed to create the correct number of strokes");
            }

            Stroke stroke = strokes[0];

            if (!ValidateStrokeFrom10To24(stroke))
            {
                throw new InvalidOperationException("Stroke.GetClipResult failed to create the correct points in the stroke");
            }

        }

        /// <summary>
        ///
        /// </summary>
        public void TestStrokeGetClipResultWithRect()
        {
            Rect rect = new Rect(0,0,25, 25);
            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = s.GetClipResult(rect);

            if (strokes.Count != 1)
            {
                throw new InvalidOperationException("Stroke.GetClipResult failed to create the correct number of strokes");
            }

            Stroke stroke = strokes[0];

            if (!ValidateStrokeFrom10To24(stroke))
            {
                throw new InvalidOperationException("Stroke.GetClipResult failed to create the correct points in the stroke");
            }
        }


        /// <summary>
        ///
        /// </summary>
        public void TestStrokeGetEraseResultWithLasso()
        {
            Point[] lassoPoints = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = s.GetEraseResult(lassoPoints);

            if (strokes.Count != 1)
            {
                throw new InvalidOperationException("Stroke.GetEraseResult  failed to create the correct number of strokes");
            }

            Stroke stroke = strokes[0];
            if (!ValidateStrokeFrom26To100(stroke))
            {
                throw new InvalidOperationException("Stroke.GetEraseResult  failed to create the correct points in the stroke");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void TestStrokeGetEraseResultWithRect()
        {
            Rect rect = new Rect(0,0,25, 25);
            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;

            StrokeCollection strokes = s.GetEraseResult(rect);

            if (strokes.Count != 1)
            {
                throw new InvalidOperationException("Stroke.GetEraseResult failed to create the correct number of strokes");
            }

            Stroke stroke = strokes[0];
            if (!ValidateStrokeFrom26To100(stroke))
            {
                throw new InvalidOperationException("Stroke.GetEraseResult  failed to create the correct points in the stroke");
            }

        }

        /// <summary>
        ///
        /// </summary>
        public void TestStrokeGetEraseResultWithShape()
        {
            ////////////////////////
            // 1. test a simple case
            //    inking stylus tip is elliptical
            ////////////////////////
            Point[] erasingPoints = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;


            StrokeCollection strokes = s.GetEraseResult(erasingPoints, new EllipseStylusShape(4f, 4f, 0f));

            if (strokes.Count != 2)
            {
                throw new InvalidOperationException("Stroke.GetEraseResult failed to create the correct number of strokes");
            }

            Stroke stroke10 = strokes[0];
            Stroke stroke11 = strokes[1];
            if (!ValidateStrokeFrom10To22(stroke10))
            {
                throw new InvalidOperationException("Stroke.GetEraseResult failed to create the correct points in the stroke");
            }
            if (!ValidateStrokeFrom28To100(stroke11))
            {
                throw new InvalidOperationException("Stroke.GetEraseResult  failed to create the correct points in the stroke");
            }

        }

        /// <summary>
        ///
        /// </summary>
        public void TestStrokeHitTestWithPoint()
        {
            // Test 1:  StrokeCollection.HitTest(Point point)
              Stroke s = CreateStrokeFrom10To100();
              s.DrawingAttributes.FitToCurve = false;

              bool isHit = s.HitTest(new Point(10, 10));

              if (isHit == false)
              {
                  throw new InvalidOperationException("Stroke.HitTest failed to return the correct result");
              }

              // Test 2:  StrokeCollection.HitTest(Point point, double diameter)
              Stroke s2 = CreateStrokeFrom10To100();
              s2.DrawingAttributes.FitToCurve = false;


              isHit = s2.HitTest(new Point(10, 10), 2);

              if (isHit == false)
              {
                  throw new InvalidOperationException("Stroke.HitTest failed to return the correct result");
              }
        }

        /// <summary>
        ///
        /// </summary>
        public void TestStrokeHitTestWithRect()
        {
            Stroke s1 = CreateStrokeFrom10To100();
            s1.DrawingAttributes.FitToCurve = false;

            Rect rect = new Rect(5, 5, 15, 15);
            bool isHit= s1.HitTest(rect, 50);

            if (isHit != false)
            {
                throw new InvalidOperationException("Stroke.HitTest failed to return the correct result");
            }

            rect = new Rect(0, 0, 20, 50);
            isHit = s1.HitTest(rect, 40);

            if (isHit != true)
            {
                throw new InvalidOperationException("Stroke.HitTest failed to return the correct result");
            }

        }
        /// <summary>
        ///
        /// </summary>
        public void TestStrokeHitTestWithLasso()
        {
            Stroke s1 = CreateStrokeFrom10To100();
            s1.DrawingAttributes.FitToCurve = false;

            Point[] lassoPoints1 = new Point[] { new Point(5, 5), new Point(5, 20), new Point(20, 20), new Point(20, 5) };
            bool isHit= s1.HitTest(lassoPoints1, 50);

            if (isHit != false)
            {
                throw new InvalidOperationException("Stroke.HitTest failed to return the correct result");
            }

            lassoPoints1 = new Point[] { new Point(0, 0), new Point(20, 0), new Point(20, 50), new Point(0, 50) };
            isHit = s1.HitTest(lassoPoints1, 40);

            if (isHit != true)
            {
                throw new InvalidOperationException("Stroke.HitTest failed to return the correct result");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void TestStrokeHitTestWithShape()
        {
            ////////////////////////
            // 1. test a simple case
            //    inking stylus tip is elliptical
            ////////////////////////
            Point[] erasingPoints = new Point[]{
                                                new Point(0f, 0f),
                                                new Point(25f, 0f),
                                                new Point(25f, 25f),
                                                new Point(0f, 25f)
                                              };

            Stroke s = CreateStrokeFrom10To100();
            s.DrawingAttributes.FitToCurve = false;


            bool isHit = s.HitTest(erasingPoints, new EllipseStylusShape(4f, 4f, 0f));

            if (isHit == false)
            {
                throw new InvalidOperationException("Stroke.HitTest failed to return the correct result");
            }

        }

        #endregion

        #region Helper functions
        private Stroke CreateStrokeFrom10To100()
        {
            Stroke stroke = CreateDefaltHeightWidthStrokeFrom10To100();
            stroke.DrawingAttributes.Height = 2d;
            stroke.DrawingAttributes.Width = 2d;
            return stroke;
        }

        private Stroke CreateDefaltHeightWidthStrokeFrom10To100()
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
            Stroke stroke = new Stroke(new StylusPointCollection(strokePoints));
            return stroke;
        }

        private MyStroke CreateCustomStrokeFrom10To100()
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
            MyStroke stroke = new MyStroke(new StylusPointCollection(strokePoints));
            stroke.DrawingAttributes.Height = 2d;
            stroke.DrawingAttributes.Width = 2d;
            return stroke;
        }



        private bool ValidateStrokeFrom10To16(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 2 ||
                points[0].X != 10f ||
                points[0].Y != 10f ||
                points[1].X != 10f ||
                points[1].Y != 15.200288607650478d)
            {
                return false;
            }
            return true;
        }

        private bool ValidateStrokeFrom10To17(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 2 ||
                points[0].X != 10f ||
                points[0].Y != 10f ||
                points[1].X != 10f ||
                points[1].Y != 17.25f )
            {
                return false;
            }
            return true;
        }

        private bool ValidateStrokeFrom10To22(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 3 ||
                points[0].X != 10f ||
                points[0].Y != 10f ||
                points[1].X != 10f ||
                points[1].Y != 20f ||
                points[2].X != 10f ||
                points[2].Y != 22f)
            {
                return false;
            }
            return true;
        }

        private bool ValidateStrokeFrom29To100(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 9 ||
                points[0].X != 10f ||
                points[0].Y != 29f ||
                points[1].X != 10f ||
                points[1].Y != 30f ||
                points[2].X != 10f ||
                points[2].Y != 40f ||
                points[3].X != 10f ||
                points[3].Y != 50f ||
                points[4].X != 10f ||
                points[4].Y != 60f ||
                points[5].X != 10f ||
                points[5].Y != 70f ||
                points[6].X != 10f ||
                points[6].Y != 80f ||
                points[7].X != 10f ||
                points[7].Y != 90f ||
                points[8].X != 10f ||
                points[8].Y != 100f)
            {
                return false;
            }
            return true;
        }

        private bool ValidateStrokeFrom28To100(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 9 ||
                points[0].X != 10f ||
                points[0].Y != 28f ||
                points[1].X != 10f ||
                points[1].Y != 30f ||
                points[2].X != 10f ||
                points[2].Y != 40f ||
                points[3].X != 10f ||
                points[3].Y != 50f ||
                points[4].X != 10f ||
                points[4].Y != 60f ||
                points[5].X != 10f ||
                points[5].Y != 70f ||
                points[6].X != 10f ||
                points[6].Y != 80f ||
                points[7].X != 10f ||
                points[7].Y != 90f ||
                points[8].X != 10f ||
                points[8].Y != 100f)
            {
                return false;
            }
            return true;
        }
        private bool ValidateStrokeFrom10To24(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 3 ||
                points[0].X != 10f ||
                points[0].Y != 10f ||
                points[1].X != 10f ||
                points[1].Y != 20f ||
                points[2].X != 10f ||
                points[2].Y != 24f)
            {
                return false;
            }
            return true;
        }

        private bool ValidateStrokeFrom86To100(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 3 ||
                points[0].X != 10f ||
                points[0].Y != 86f ||
                points[1].X != 10f ||
                points[1].Y != 90f ||
                points[2].X != 10f ||
                points[2].Y != 100f)
            {
                return false;
            }
            return true;
        }

        private bool ValidateStrokeFrom26To100(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 9 ||
                points[0].X != 10f ||
                points[0].Y != 26f ||
                points[1].X != 10f ||
                points[1].Y != 30f ||
                points[2].X != 10f ||
                points[2].Y != 40f ||
                points[3].X != 10f ||
                points[3].Y != 50f ||
                points[4].X != 10f ||
                points[4].Y != 60f ||
                points[5].X != 10f ||
                points[5].Y != 70f ||
                points[6].X != 10f ||
                points[6].Y != 80f ||
                points[7].X != 10f ||
                points[7].Y != 90f ||
                points[8].X != 10f ||
                points[8].Y != 100f)
            {
                return false;
            }
            return true;
        }

        private bool ValidateStrokeFrom26To84(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 8 ||
                points[0].X != 10f ||
                points[0].Y != 26f ||
                points[1].X != 10f ||
                points[1].Y != 30f ||
                points[2].X != 10f ||
                points[2].Y != 40f ||
                points[3].X != 10f ||
                points[3].Y != 50f ||
                points[4].X != 10f ||
                points[4].Y != 60f ||
                points[5].X != 10f ||
                points[5].Y != 70f ||
                points[6].X != 10f ||
                points[6].Y != 80f ||
                points[7].X != 10f ||
                points[7].Y != 84f)
            {
                return false;
            }
            return true;
        }

        private bool ValidateStrokeFrom10To84(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 9 ||
                points[0].X != 10f ||
                points[0].Y != 10f ||
                points[1].X != 10f ||
                points[1].Y != 20f ||
                points[2].X != 10f ||
                points[2].Y != 30f ||
                points[3].X != 10f ||
                points[3].Y != 40f ||
                points[4].X != 10f ||
                points[4].Y != 50f ||
                points[5].X != 10f ||
                points[5].Y != 60f ||
                points[6].X != 10f ||
                points[6].Y != 70f ||
                points[7].X != 10f ||
                points[7].Y != 80f ||
                points[8].X != 10f ||
                points[8].Y != 84f)
            {
                return false;
            }
            return true;
        }


        private bool ValidateStrokeFrom31To59(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 4 ||
                points[0].X != 10f ||
                points[0].Y != 31f ||
                points[1].X != 10f ||
                points[1].Y != 40f ||
                points[2].X != 10f ||
                points[2].Y != 50f ||
                points[3].X != 10f ||
                points[3].Y != 59f
               )
            {
                return false;
            }
            return true;
        }
        private bool ValidateStrokeFrom10To29(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;

            if (points.Count != 3 ||
                points[0].X != 10f ||
                points[0].Y != 10f ||
                points[1].X != 10f ||
                points[1].Y != 20f ||
                points[2].X != 10f ||
                points[2].Y != 29f
                )
            {
                return false;
            }
            return true;

        }

        private bool ValidateStrokeFrom61To100(Stroke s)
        {
            StylusPointCollection points = s.StylusPoints;
            if (points.Count !=5 ||
                points[0].X != 10f ||
                points[0].Y != 61f ||
                points[1].X != 10f ||
                points[1].Y != 70f ||
                points[2].X != 10f ||
                points[2].Y != 80f ||
                points[3].X != 10f ||
                points[3].Y != 90f ||
                points[4].X != 10f ||
                points[4].Y != 100f
                )
            {
                return false;
            }
            return true;
        }

        private void OnLassoSelectionChanged(object sender, LassoSelectionChangedEventArgs args)
        {
            _hitStrokes = args.SelectedStrokes;
            _unhitStrokes = args.DeselectedStrokes;
        }

        private void OnStrokeEraseResultChanged(object sender, StrokeHitEventArgs args)
        {
            if (args == null)
            {
                throw new InvalidOperationException(String.Format("StrokeHitEventArgs should not be null"));       

            }
            _incrementalStrokeHitTesterStrokes.Remove(args.HitStroke);
        }

        private void OnPointEraseResultChanged(object sender, StrokeHitEventArgs args)
        {
            if (args == null)
            {
                throw new InvalidOperationException(String.Format("StrokeHitEventArgs should not be null"));       

            }
            if ((args.GetPointEraseResults()).Count == 0)
            {
                _incrementalStrokeHitTesterStrokes.Remove(args.HitStroke);
            }
            else
            {
                _incrementalStrokeHitTesterStrokes.Replace(args.HitStroke, args.GetPointEraseResults());
            }
        }



        private void VerifyHitTestWithLasso(StrokeCollection hit,  StrokeCollection unhit, String Msg)
        {
            if (_hitStrokes == null || _unhitStrokes == null || _hitStrokes.Count != hit.Count || _unhitStrokes.Count!= unhit.Count)
            {
               throw new InvalidOperationException(
                        String.Format("Unexpected results in {0}", Msg));
            }
        }

        private void VerifyStrokeCount(StrokeCollection strokes, int expected, String Msg)
        {
            if (strokes == null || strokes.Count != expected)
            {
               throw new InvalidOperationException(
                        String.Format("Unexpected results in {0}", Msg));
            }
        }



        #endregion
        #region Private fields
        private StrokeCollection _hitStrokes;
        private StrokeCollection _unhitStrokes;
        private StrokeCollection _incrementalStrokeHitTesterStrokes;
        #endregion
    }
}

