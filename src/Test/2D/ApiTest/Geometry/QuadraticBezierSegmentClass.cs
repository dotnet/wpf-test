// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing QuadraticBezierSegment class
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for QuadraticBezierSegmentClass.
    /// </summary>
    internal class QuadraticBezierSegmentClass : ApiTest
    {
        public QuadraticBezierSegmentClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("QuadraticBezierSegment Class");

            string objectType = "System.Windows.Media.QuadraticBezierSegment";

            #region Section I: Constructors
            #region Test #1: Default constructor
            CommonLib.LogStatus("Test #1: Default Constructor");
            QuadraticBezierSegment qbs1 = new QuadraticBezierSegment();
            CommonLib.TypeVerifier(qbs1, objectType);
            #endregion

            #region Test #2: QuadraticBezierSegment (Point point1, Point point2, Boolean isStroked)
            CommonLib.LogStatus("Test #2: QuadraticBezierSegment (Point point1, Point point2, Boolean isStroked) ");
            QuadraticBezierSegment qbs2 = new QuadraticBezierSegment(new Point(20.01, 20.01), new Point(30, 100), false);
            CommonLib.TypeVerifier(qbs2, objectType);
            #endregion

            #endregion

            #region Section II: public methods
            #region Test #3: Copy() method
            CommonLib.LogStatus("Test #3: Copy() method");
            QuadraticBezierSegment qbs3 = qbs2.Clone();
            CommonLib.GenericVerifier(qbs3 != null && qbs3.Point1.X == 20.01 && qbs3.Point1.Y == 20.01, "Copy() method");
            #endregion

            #region Test #3.1: Copy() method with invalidated field
            CommonLib.LogStatus("Test #3.1: Copy() method with invalidated field");
            QuadraticBezierSegment qbs31 = new QuadraticBezierSegment();
            qbs31.InvalidateProperty(QuadraticBezierSegment.Point1Property);
            qbs31.InvalidateProperty(QuadraticBezierSegment.Point2Property);
            QuadraticBezierSegment qbs31_copy = qbs31.Clone();
            CommonLib.GenericVerifier(qbs31_copy != null, "Copy() method with Invalidated field");
            #endregion

            #region Test #4: CloneCurrentValue() method
            CommonLib.LogStatus("Test #4: CloneCurrentValue() method");
            QuadraticBezierSegment qbs4 = qbs2.CloneCurrentValue();
            CommonLib.GenericVerifier(qbs4 != null && qbs4.Point1.X == 20.01 && qbs4.Point1.Y == 20.01, "CloneCurrentValue() method");
            #endregion

            #region Test #4.1: CloneCurrentValue() on an animated QuadraticBezierSegment
            CommonLib.LogStatus("Test #4.1: CloneCurrentValue() on an animated QuadraticBezierSegment ");
            QuadraticBezierSegment qbs41 = new QuadraticBezierSegment(new Point(20.01, 20.01), new Point(30, 100), false);

            PointAnimation pd1 = new PointAnimation();
            pd1.To = new Point(32, 33);
            pd1.BeginTime = null;
            pd1.Duration = TimeSpan.FromMilliseconds(1000);
            PointAnimation pd2 = new PointAnimation();
            pd2.To = new Point(32, 33);
            pd2.BeginTime = null;
            pd2.Duration = TimeSpan.FromMilliseconds(1000);

            AnimationClock pdClock1 = pd1.CreateClock();
            AnimationClock pdClock2 = pd2.CreateClock();
            qbs41.ApplyAnimationClock(QuadraticBezierSegment.Point1Property, pdClock1);
            qbs41.ApplyAnimationClock(QuadraticBezierSegment.Point2Property, pdClock2);
            pdClock1.Controller.Begin();
            pdClock2.Controller.Begin();

            QuadraticBezierSegment qbs41_ret = qbs41.CloneCurrentValue();
            CommonLib.GenericVerifier(qbs41_ret.Point1 == new Point(20.01, 20.01) && qbs41_ret.Point2 == new Point(30, 100), "CloneCurrentValue() in an animated QuadraticBezierSegment");
            #endregion
            #endregion

            #region Section III: public properties
            #region Test #5: CanFreeze Property
            CommonLib.LogStatus("Test #5: CanFreeze property");
            CommonLib.GenericVerifier(qbs2.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #8a: Point1 property with basevalue
            CommonLib.LogStatus("Test #8a: Point1 property with basevalue");
            QuadraticBezierSegment qbs8a = new QuadraticBezierSegment();
            CommonLib.GenericVerifier(qbs8a.Point1.X == 0 && qbs8a.Point1.Y == 0, "Point1 property with basevalue");
            #endregion

            #region Test #8b: Point1 property in Invalid state
            CommonLib.LogStatus("Test #8b: Point1 property in Invalid state");
            QuadraticBezierSegment qbs8b = new QuadraticBezierSegment();
            qbs8b.InvalidateProperty(QuadraticBezierSegment.Point1Property);
            CommonLib.GenericVerifier(qbs8b.Point1.X == 0 && qbs8b.Point1.Y == 0, "Point1 property in Invalid state");
            #endregion

            #region Test #8: Point1 property
            CommonLib.LogStatus("Test #8: Point1 property");
            qbs2.Point1 = new Point(0, 0);
            CommonLib.GenericVerifier(qbs2.Point1.X == 0 && qbs2.Point1.Y == 0, "Point1 property");
            #endregion

            #region Test #8.5: Point1Animations property
            CommonLib.LogStatus("Test #8.5: Poin1Animations property");
            PointAnimation pointAnim1 = new PointAnimation(new Point(23.2, 23), new Point(23.11, 120), new Duration(TimeSpan.FromMilliseconds(1000)));
            PointAnimation pointAnim2 = new PointAnimation(new Point(90, 100.0000001), new Duration(TimeSpan.FromMilliseconds(200)), FillBehavior.HoldEnd);
            AnimationClock pointClock1 = pointAnim1.CreateClock();
            AnimationClock pointClock2 = pointAnim2.CreateClock();
            qbs2.ApplyAnimationClock(QuadraticBezierSegment.Point1Property, pointClock1);
            qbs2.ApplyAnimationClock(QuadraticBezierSegment.Point1Property, pointClock2);
            CommonLib.GenericVerifier(pointClock1 != null, "Point1Animation property 1");
            CommonLib.GenericVerifier(pointClock2 != null, "Point1Animation property 2");
            #endregion

            #region Test #9a: Point2 property with basevalue
            CommonLib.LogStatus("Test #9a: Point2 property with basevalue");
            QuadraticBezierSegment qbs9a = new QuadraticBezierSegment();
            CommonLib.GenericVerifier(qbs9a.Point2.X == 0 && qbs9a.Point2.Y == 0, "Point2 property with basevalue");
            #endregion

            #region Test #9b: Point2 property in Invalid state
            CommonLib.LogStatus("Test #9b: Point2 property in Invalid state");
            QuadraticBezierSegment qbs9b = new QuadraticBezierSegment();
            qbs9b.InvalidateProperty(QuadraticBezierSegment.Point2Property);
            CommonLib.GenericVerifier(qbs9b.Point2.X == 0 && qbs9b.Point2.Y == 0, "Point2 property in Invalid state");
            #endregion

            #region Test #9: Point2 property
            CommonLib.LogStatus("Test #9: Point2 property");
            qbs2.Point2 = new Point(23.22, -100);
            CommonLib.GenericVerifier(qbs2.Point2.X == 23.22 && qbs2.Point2.Y == -100, "Point2 property");
            #endregion

            #region Test #10: Point2Animations property
            CommonLib.LogStatus("Test #10: Point2Animations property");
            PointAnimation pointAnim3 = new PointAnimation(new Point(-12, 22), new Duration(TimeSpan.FromMilliseconds(100)), FillBehavior.HoldEnd);
            AnimationClock pointClock3 = pointAnim3.CreateClock();
            qbs2.ApplyAnimationClock(QuadraticBezierSegment.Point2Property, pointClock3);
            CommonLib.GenericVerifier(pointClock3 != null, "Point2Animation property");
            #endregion
            #endregion
            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;
            CommonLib.LogTest("Result for:" + objectType);

            PathFigure pf = new PathFigure(new Point(30, 20), new PathSegment[] { qbs2, qbs9b }, false);
            pf.IsClosed = true;
            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf);
            Rect pgBound = pg.Bounds;
            DC.DrawGeometry(new SolidColorBrush(Colors.Red), null, pg);
            #endregion
        }
    }
}
