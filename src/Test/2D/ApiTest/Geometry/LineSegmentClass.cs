// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  GMC API Tests - Testing LineSegment class
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
    /// Summary description for LineSegmentClass.
    /// </summary>
    internal class LineSegmentClass : ApiTest
    {
        public LineSegmentClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("BezierSegment Class");

            string objectType = "System.Windows.Media.LineSegment";

            #region Section I: Initialization stage
            #region Test #1: Default constructor
            CommonLib.LogStatus("Test #1: Default constructor");
            LineSegment ls1 = new LineSegment();
            CommonLib.TypeVerifier(ls1, objectType);
            #endregion

            #region Test #2: LineSegment(Point, bool)
            CommonLib.LogStatus("Test #2: LineSegment(Point, bool)");

            LineSegment ls2 = new LineSegment(new Point(20, 150), false);
            CommonLib.TypeVerifier(ls2, objectType);
            #endregion

            #endregion

            #region Section II:  Property test section
            #region Test #4a: Point property with basevalue
            CommonLib.LogStatus("Test #4a: Point property with basevalue");
            LineSegment ls4a = new LineSegment();
            CommonLib.GenericVerifier(ls4a.Point.X == 0 && ls4a.Point.Y == 0, "Point property with basevalue");
            #endregion

            #region Test #4b: Point property in invalid stage
            CommonLib.LogStatus("Test #4b:  Point property in invalid stage");
            LineSegment ls4b = new LineSegment();
            ls4b.InvalidateProperty(LineSegment.PointProperty);
            CommonLib.GenericVerifier(ls4b.Point.X == 0 && ls4b.Point.Y == 0, "Point property in invalid stage");
            #endregion

            #region Test #4: Point property
            CommonLib.LogStatus("Test #f: Point property");
            ls1.Point = new Point(300, 300);
            CommonLib.PointVerifier(ls1.Point, new Point(300, 300));
            #endregion

            #region Test #5: PointAnimations property
            CommonLib.LogStatus("Test #5: PointAnimations property");
            PointAnimation pointAnim = new PointAnimation(new Point(10.0, 120.3), new Point(3.1, 90), new Duration(TimeSpan.FromMilliseconds(200)));
            AnimationClock pointClock = pointAnim.CreateClock();
            ls1.ApplyAnimationClock(LineSegment.PointProperty, pointClock);
            CommonLib.GenericVerifier(pointClock != null, "PointAnimation property");
            #endregion

            #region Test #7: CanFreeze property
            CommonLib.LogStatus("Test #7: CanFreeze property");
            CommonLib.GenericVerifier(!ls1.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #8.5a: IsStroked property with basevalue
            CommonLib.LogStatus("Test #8.5a: IsStroked property with basevalue");
            LineSegment ls85a = new LineSegment();
            CommonLib.GenericVerifier(ls85a.IsStroked, "IsStroked with basevalue");
            #endregion

            #region Test #8.5b: IsStroked property in Invalid state
            CommonLib.LogStatus("Test #8.5b: IsStroked property in Invalid state");
            LineSegment ls85b = new LineSegment();
            ls85b.InvalidateProperty(LineSegment.IsStrokedProperty);
            CommonLib.GenericVerifier(ls85b.IsStroked, "IsStroke in Invalid stage");
            #endregion

            #region Test #8.5: IsStroked property
            CommonLib.LogStatus("Test #8.5: IsStroked property");
            ls2.IsStroked = true;
            CommonLib.GenericVerifier(ls2.IsStroked, "IsStroked property");
            #endregion
            #endregion

            #region Section III : public method test
            #region Test #9: Copy() method
            CommonLib.LogStatus("Test #9: Copy() method");
            //Copy() method will be tested in depth in IChangable test framework
            LineSegment ls4 = ls2.Clone();
            CommonLib.GenericVerifier(ls4 != null && ls4 is LineSegment, "Copy() method");
            #endregion

            #region Test #10: CloneCurrentValue() method
            CommonLib.LogStatus("Test #10: CloneCurrentValue() method");
            LineSegment ls5 = ls1.CloneCurrentValue() as LineSegment;
            CommonLib.GenericVerifier(ls5 != null, "CloneCurrentValue() property");
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogTest("Unit test result for :" + objectType);

            PathFigure pf = new PathFigure();

            pf.StartPoint = new Point(30, 30);
            pf.Segments.Add(ls2);

            PathGeometry pg = new PathGeometry();

            pg.Figures.Add(pf);
            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), 1), pg);
            #endregion
        }
    }
}