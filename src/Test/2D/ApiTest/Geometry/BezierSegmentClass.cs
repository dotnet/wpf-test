// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing BezierSegment class
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for BezierSegment.
    /// </summary>
    internal class BezierSegmentClass : ApiTest
    {
        public BezierSegmentClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("BezierSegment Class");

            string objectType = "System.Windows.Media.BezierSegment";

            #region Section I: Initialization stage
            #region Test #1 - Default constructor
            CommonLib.LogStatus("Test #1: Default Constructor");
            BezierSegment bezierSegment1 = new BezierSegment();
            CommonLib.TypeVerifier(bezierSegment1, objectType);
            #endregion

            #region Test #2 - Constructor with Point, Point, Point, bool
            CommonLib.LogStatus("Test #2: Constructor with Point, Point, Point and isStroke");
            BezierSegment bezierSegment2 = new BezierSegment(new Point(10, 10), new Point(250, 100), new Point(20, 180), true);
            CommonLib.TypeVerifier(bezierSegment2, objectType);

            #endregion

            #region Test #2.5 - Constructor public BezierSegment ( System.Windows.Point point1 , PointAnimationCollection point1Animations , System.Windows.Point point2 , PointAnimationCollection point2Animations , System.Windows.Point point3 , PointAnimationCollection point3Animations , System.Boolean isStroked )
            CommonLib.LogStatus("Test #2.5 - Constructor public BezierSegment ( System.Windows.Point point1 , PointAnimationCollection point1Animations , System.Windows.Point point2 , PointAnimationCollection point2Animations , System.Windows.Point point3 , PointAnimationCollection point3Animations , System.Boolean isStroked )");
            BezierSegment bezierSegment25 = new BezierSegment(new Point(29, 10), new Point(39, 100), new Point(100, 120), true);
            CommonLib.TypeVerifier(bezierSegment25, objectType);
            #endregion
            #endregion

            #region Section II: BezierSegment properties testing
            #region Test 3a: Point1 property with basevalue
            CommonLib.LogStatus("Test 3a: Point1 property with basevalue");
            BezierSegment bs3a = new BezierSegment();
            CommonLib.GenericVerifier(bs3a.Point1.X == 0 && bs3a.Point1.Y == 0, "Point1 property with basevalue");
            #endregion

            #region Test 3b: Point1 property in Invalid stage
            CommonLib.LogStatus("Test 3b: Point1 property in Invalid stage");
            BezierSegment bs3b = new BezierSegment();
            bs3b.InvalidateProperty(BezierSegment.Point1Property);
            CommonLib.GenericVerifier(bs3b.Point1.X == 0 && bs3b.Point1.Y == 0, "Point1 property in Invalid stage");
            #endregion

            #region Test 3: Point1 property
            CommonLib.LogStatus("Test 3: Point1 property");
            bezierSegment1.Point1 = new Point(0, 20);
            CommonLib.PointVerifier(bezierSegment1.Point1, new Point(0, 20));
            #endregion

            #region Test 3.5: Point1Animations property
            PointAnimation pointAnim = new PointAnimation(new Point(29, 100), new Point(10, 10), new Duration(TimeSpan.FromMilliseconds(1000)));
            AnimationClock pointClock = pointAnim.CreateClock();
            bezierSegment25.ApplyAnimationClock(BezierSegment.Point1Property, pointClock);
            CommonLib.GenericVerifier(pointClock != null, "RotationAngleAnimations property");
            #endregion

            #region Test 4a: Point2 property with basevalue
            CommonLib.LogStatus("Test 4a: Point2 property with basevalue");
            BezierSegment bs4a = new BezierSegment();
            CommonLib.GenericVerifier(bs4a.Point2.X == 0 && bs4a.Point2.Y == 0, "Point2 property with basevalue");
            #endregion

            #region Test 4b: Point2 property in Invalid stage
            CommonLib.LogStatus("Test 4b: Point2 property in Invalid stage");
            BezierSegment bs4b = new BezierSegment();
            bs4b.InvalidateProperty(BezierSegment.Point2Property);
            CommonLib.GenericVerifier(bs4b.Point2.X == 0 && bs4b.Point2.Y == 0, "Point2 property in Invalid stage");
            #endregion

            #region Test 4: Point2 property
            CommonLib.LogStatus("Test 4: Point2 property");
            bezierSegment1.Point2 = new Point(120, 120);
            CommonLib.PointVerifier(bezierSegment1.Point2, new Point(120, 120));
            #endregion

            #region Test #4.5 - Point2Animations property
            PointAnimation pointAnim1 = new PointAnimation(new Point(30, 10), new Point(10, 10), new Duration(TimeSpan.FromMilliseconds(200)));
            PointAnimation pointAnim2 = new PointAnimation(new Point(0, 0), new Point(10, 200), new Duration(TimeSpan.FromMilliseconds(2000)));
            AnimationClock pointClock1 = pointAnim1.CreateClock();
            AnimationClock pointClock2 = pointAnim2.CreateClock();
            bezierSegment25.ApplyAnimationClock(BezierSegment.Point2Property, pointClock1);
            bezierSegment25.ApplyAnimationClock(BezierSegment.Point2Property, pointClock2);
            CommonLib.GenericVerifier(pointClock1 != null, "Point2Animation property 1");
            CommonLib.GenericVerifier(pointClock2 != null, "Point2Animation property 2");
            #endregion

            #region Test 5a: Point3 property with basevalue
            CommonLib.LogStatus("Test 5a: Point3 property with basevalue");
            BezierSegment bs5a = new BezierSegment();
            CommonLib.GenericVerifier(bs5a.Point3.X == 0 && bs5a.Point3.Y == 0, "Point3 property with basevalue");
            #endregion

            #region Test 5b: Point2 property in Invalid stage
            CommonLib.LogStatus("Test 5b: Point3 property in Invalid stage");
            BezierSegment bs5b = new BezierSegment();
            bs5b.InvalidateProperty(BezierSegment.Point3Property);
            CommonLib.GenericVerifier(bs5b.Point3.X == 0 && bs5b.Point3.Y == 0, "Point3 property in Invalid stage");
            #endregion

            #region Test 5: Point3 property
            CommonLib.LogStatus("Test 5: Point 3 property");
            bezierSegment1.Point3 = new Point(90, 70);
            CommonLib.PointVerifier(bezierSegment1.Point3, new Point(90, 70));
            #endregion

            #region Test #5.5 - Point2Animations property
            CommonLib.LogStatus("Test #5.5 - Point3Animations property");
            PointAnimation pointAnim3 = new PointAnimation(new Point(30, 10), new Point(10, 10), new Duration(TimeSpan.FromMilliseconds(200)));
            PointAnimation pointAnim4 = new PointAnimation(new Point(0, 0), new Point(10, 200), new Duration(TimeSpan.FromMilliseconds(2000)));
            PointAnimation pointAnim5 = new PointAnimation(new Point(30, -10), new Point(30, 32), new Duration(TimeSpan.FromMilliseconds(1000)), FillBehavior.HoldEnd);
            AnimationClock pointClock3 = pointAnim3.CreateClock();
            AnimationClock pointClock4 = pointAnim4.CreateClock();
            AnimationClock pointClock5 = pointAnim5.CreateClock();
            bezierSegment25.ApplyAnimationClock(BezierSegment.Point3Property, pointClock3);
            bezierSegment25.ApplyAnimationClock(BezierSegment.Point3Property, pointClock4);
            bezierSegment25.ApplyAnimationClock(BezierSegment.Point3Property, pointClock5);
            CommonLib.GenericVerifier(pointClock3 != null, "Point3Animation property 3");
            CommonLib.GenericVerifier(pointClock4 != null, "Point3Animation property 4");
            CommonLib.GenericVerifier(pointClock5 != null, "Point3Animation property 5");
            #endregion

            #region Test #7: CanFreeze property
            CommonLib.LogStatus("Test #7: CanFreeze property");
            CommonLib.GenericVerifier(!bezierSegment25.CanFreeze, "CanFreeze property");
            #endregion

            #endregion

            #region Section III: BezierSegment public method
            #region Test 9: Copy method
            CommonLib.LogStatus("Test 9: Copy method");
            BezierSegment bezierSegment9 = bezierSegment25.Clone() as BezierSegment;
            CommonLib.GenericVerifier(bezierSegment9 != null, "Copy() method");
            #endregion

            #region Test 10: CloneCurrentValue method
            CommonLib.LogStatus("Test 10: CloneCurrentValue method");
            BezierSegment bezierSegment10 = bezierSegment1.CloneCurrentValue() as BezierSegment;
            CommonLib.GenericVerifier(bezierSegment10 != null && bezierSegment10.Point1.X == 0 && bezierSegment10.Point1.Y == 20, "CloneCurrentValue method");
            #endregion

            #region Test #13 - Internal AddToFigure ()
            CommonLib.LogStatus("Test #13 - Internal AddToFigure ()");
            PathGeometry pg13 = new PathGeometry();
            pg13.Transform = new RotateTransform(98);
            PathGeometry pg131 = new PathGeometry();
            PathFigure pf13 = new PathFigure();
            pf13.StartPoint = new Point(32, 34);
            pf13.Segments.Add(new BezierSegment(new Point(10, 10), new Point(250, 100), new Point(20, 180), true));
            pg131.Figures.Add(pf13);
            pg131.Transform = new RotateTransform(45);
            pg13.AddGeometry(pg131);
            CommonLib.GenericVerifier(pg13.Figures[0].Segments[0] is BezierSegment, "Internal AddtoFigure()");
            #endregion
            #endregion

            #region Section IV : Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogTest("Result for :" + objectType);

            PathFigure pf = new PathFigure();
            pf.StartPoint = new Point(0, 0);
            pf.Segments.Add(bezierSegment1);
            pf.Segments.Add(bezierSegment2);

            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);

            DC.DrawGeometry(new SolidColorBrush(Colors.Blue), null, pg);
            #endregion
        }
    }
}