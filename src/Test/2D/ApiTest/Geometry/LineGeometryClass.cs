// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing LineGeometry class
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
    /// Summary description for LineGeometryClass.
    /// </summary>
    internal class LineGeometryClass : ApiTest
    {
        public LineGeometryClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("LineGeometry Class");
            string objectType = "System.Windows.Media.LineGeometry";

            #region Section I: Constructors
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");
            LineGeometry lg1 = new LineGeometry();
            CommonLib.TypeVerifier(lg1, objectType);
            #endregion

            #region Test #2: LineGeometry(Point1, Point2)
            CommonLib.LogStatus("Test #2: LineGeometry(point1, point2)");
            LineGeometry lg2 = new LineGeometry(new Point(200.9, 120.01), new Point(28, 10));
            CommonLib.TypeVerifier(lg2, objectType);
            #endregion
            #endregion

            #region Section II: public methods
            #region Test #5: Copy() method
            CommonLib.LogStatus("Test #5: Copy() method");
            LineGeometry lg5 = lg2.Clone();
            CommonLib.GenericVerifier(lg5 != null && lg5.StartPoint.X == 200.9 && lg5.StartPoint.Y == 120.01, "Copy() method");
            #endregion

            #region Test #5.5:  GetRenderBounds(pen) method
            CommonLib.LogStatus("Test #5.5: GetRenderBounds(pen) method");
            LineGeometry lg55 = new LineGeometry(new Point(0, 0), new Point(100, 0));
            Rect bound55 = lg55.GetRenderBounds(new Pen(Brushes.Black, 5));
            CommonLib.GenericVerifier(bound55.Contains(new Rect(0, -2.5, 100, 5)), "GetRenderBounds(pen)");
            #endregion

            #region Test #5.6:  GetRenderBounds(pen) method
            CommonLib.LogStatus("Test #5.6: GetRenderBounds(pen) method with PenWidth=0");
            LineGeometry lg56 = new LineGeometry(new Point(0, 0), new Point(100, 0));
            Rect bound56 = lg56.GetRenderBounds(new Pen(Brushes.Black, 0));
            CommonLib.RectVerifier(bound56, new Rect(0, 0, 100, 0));
            #endregion

            #region Test #5.7:  GetRenderBounds(pen) method
            CommonLib.LogStatus("Test #5.6: GetRenderBounds(pen) method with no width LineGeometry");
            LineGeometry lg57 = new LineGeometry(new Point(0, 0), new Point(0, 0));
            Rect bound57 = lg57.GetRenderBounds(new Pen(Brushes.Black, 0));
            CommonLib.RectVerifier(bound57, new Rect(0, 0, 0, 0));
            #endregion

            #region Test #5.8:  GetRenderBounds(pen) method
            CommonLib.LogStatus("Test #5.8: GetRenderBounds(pen) method with no width LineGeometry and big pen");
            LineGeometry lg58 = new LineGeometry(new Point(0, 0), new Point(0, 0));
            Rect bound58 = lg58.GetRenderBounds(new Pen(Brushes.Red, 1000));
            CommonLib.RectVerifier(bound58, new Rect(0, -500, 0, 1000));
            #endregion

            #region Test #6: CloneCurrentValue() method
            CommonLib.LogStatus("Test #6: CloneCurrentValue() method");
            LineGeometry lg6 = lg2.CloneCurrentValue() as LineGeometry;
            CommonLib.GenericVerifier(lg6 != null && lg6.StartPoint.X == 200.9 && lg6.StartPoint.Y == 120.01, "CloneCurrentValue() method");
            #endregion
            #endregion

            #region Section III: public properties
            #region Test #7: Bounds property
            CommonLib.LogStatus("Test #7: Bounds property");
            Rect boundRect = lg2.Bounds;
            CommonLib.RectVerifier(boundRect, new Rect(28, 10, 172.9, 110.01));
            #endregion

            #region Test #7.1: Bounds property
            CommonLib.LogStatus("Test #7.1: Bounds property with a non vertical/Horizontal LineGeometry");
            LineGeometry lg71 = new LineGeometry(new Point(100, 100), new Point(130, 130));
            Rect bound71 = lg71.Bounds;
            CommonLib.RectVerifier(bound71, new Rect(100, 100, 30, 30));
            #endregion

            #region Test #7.2: Bounds property
            CommonLib.LogStatus("Test #7.2: Bounds property with 0 width LineGeometry");
            LineGeometry lg72 = new LineGeometry(new Point(100, 100), new Point(100, 100));
            Rect bound72 = lg72.Bounds;
            CommonLib.RectVerifier(bound72, new Rect(100, 100, 0, 0));
            #endregion

            #region Test #7.3: Bounds property
            CommonLib.LogStatus("Test #7.3: Bounds property with LineGeometry going to negative area");
            LineGeometry lg73 = new LineGeometry(new Point(-100, -100), new Point(100, 100));
            Rect bound73 = lg73.Bounds;
            CommonLib.RectVerifier(bound73, new Rect(-100, -100, 200, 200));
            #endregion

            #region Test #7.5: GetArea(double Tolerance) method
            CommonLib.LogStatus("Test #7.5: GetArea(double Tolerance) method");
            LineGeometry testobj75 = new LineGeometry(new Point(3.23, 20), new Point(232, 10));
            double result75 = testobj75.GetArea(-232323.233, ToleranceType.Absolute);
            CommonLib.GenericVerifier(result75 == 0, "GetArea(double Tolerance) method");
            #endregion

            #region Test 7.6:  GetArea() method circular testing
            CommonLib.LogStatus("Test #7.6: GetArea() method circular testing");
            LineGeometry lg76 = new LineGeometry(new Point(10, 23), new Point(100, 100.32));
            double result76 = lg76.GetArea();
            CommonLib.GenericVerifier(result76 == 0, "GetArea() method");
            CommonLib.CircularTestGeometry(lg76);
            #endregion

            #region Test #8a: EndPoint property with basevalue
            CommonLib.LogStatus("Test #8a: EndPoint property with basevalue");
            LineGeometry lg8a = new LineGeometry();
            CommonLib.PointVerifier(lg8a.EndPoint, new Point(0, 0));
            #endregion

            #region Test #8b: EndPoint property in Invalid state
            CommonLib.LogStatus("Test #8b: EndPoint property in Invalid state");
            LineGeometry lg8b = new LineGeometry();
            lg8b.InvalidateProperty(LineGeometry.EndPointProperty);
            CommonLib.PointVerifier(lg8b.EndPoint, new Point(0, 0));
            #endregion

            #region Test #8: EndPoint property
            lg2.EndPoint = new Point(10.01, 10.10);
            CommonLib.GenericVerifier(lg2.EndPoint.X == 10.01 && lg2.EndPoint.Y == 10.10, "EndPoint property");
            #endregion

            #region Test #8.5:  EndPointAnimations property
            CommonLib.LogStatus("Test #8.5:  EndPointAnimations property");
            PointAnimation pointAnim1 = new PointAnimation(new Point(32, 10), new Point(100, 27.3), new Duration(TimeSpan.FromMilliseconds(1000)));
            PointAnimation pointAnim2 = new PointAnimation(new Point(10, 32.3), new Point(90.1, 32.123), new Duration(TimeSpan.FromMilliseconds(2000)));
            AnimationClock pointClock1 = pointAnim1.CreateClock();
            AnimationClock pointClock2 = pointAnim2.CreateClock();
            lg2.ApplyAnimationClock(LineGeometry.EndPointProperty, pointClock1);
            lg2.ApplyAnimationClock(LineGeometry.EndPointProperty, pointClock2);
            CommonLib.GenericVerifier(pointClock1 != null, "EndPointAnimation property 1");
            CommonLib.GenericVerifier(pointClock2 != null, "EndPointAnimation property 2");
            #endregion

            #region Test #9:CanFreeze property
            CommonLib.LogStatus("Test #9: CanFreeze property");
            CommonLib.GenericVerifier(!lg2.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #12a: StartPoint property with basevalue
            CommonLib.LogStatus("Test #12a:  StartPoint property with basevalue");
            LineGeometry lg12a = new LineGeometry();
            CommonLib.GenericVerifier(lg12a.StartPoint.X == 0 && lg12a.StartPoint.Y == 0, "StartPoint property with basevalue");
            #endregion

            #region Test #12b: StartPoint property in Invalid state
            CommonLib.LogStatus("Test #12b: StartPoint property in Invalid state");
            LineGeometry lg12b = new LineGeometry();
            lg12b.InvalidateProperty(LineGeometry.StartPointProperty);
            CommonLib.PointVerifier(lg12b.StartPoint, new Point(0, 0));
            #endregion

            #region Test #12: StartPoint property
            CommonLib.LogStatus("Test #12: StartPoint property");
            lg2.StartPoint = new Point(100.29, 100.1);
            CommonLib.GenericVerifier(lg2.StartPoint.X == 100.29 && lg2.StartPoint.Y == 100.1, "StartPoint property");
            #endregion

            #region Test #12.5 - StartPointAnimations
            CommonLib.LogStatus("Test #12.5 - StartPointAnimations");
            PointAnimation pointAnim3 = new PointAnimation(new Point(-22.3, 10), new Point(32, 100), new Duration(TimeSpan.FromMilliseconds(1000)));
            AnimationClock pointClock3 = pointAnim3.CreateClock();
            lg2.ApplyAnimationClock(LineGeometry.StartPointProperty, pointClock3);
            CommonLib.GenericVerifier(pointClock3 != null, "StartPointAnimation");
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;
            CommonLib.LogTest("Results for type: "+ objectType);

            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), 1), lg2);
            #endregion
        }
    }
}