// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  HitTest tests for LineGeometry
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class LineGeometryHitTest : HitTestBase
    {
        public LineGeometryHitTest( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext ctx)
        {
            testedGeometry = new LineGeometry(new Point(100, 100), new Point(300, 300));
            RunBVT();

            CommonLib.LogTest("LineGeometry HitTesting result:");
        }

        private void RunBVT()
        {
            CommonLib.LogStatus("LineGeometry has no filled region, so with this Contains call, no hit geometry should return true.");
            TestContains1(new RectangleGeometry(new Rect(200, 200, 1, 1)), false);

            CommonLib.LogStatus("Point(200,200) is on the LineGeometry, so should return true.");
            TestContains2(new Point(200, 200), true);

            CommonLib.LogStatus("Hit point (105, 100 ) is in the stroke of the pen, so the result should be true");
            TestContains3(new Pen(Brushes.Black, 30), new Point(105, 100), true);

            CommonLib.LogStatus("Hit point (5, -10) is NOT in the stroke of the pen, so the result should be false");
            TestContains3(new Pen(Brushes.Black, 30), new Point(5, -10), false);

            CommonLib.LogStatus("LineGeometry has no filled region, so no matter how big the tolerance is, no hit geometry should return true.");
            TestContains4(new RectangleGeometry(new Rect(90, 90, 2, 2)), 1000, ToleranceType.Absolute, false);

            CommonLib.LogStatus("Point(100,200) is definitely on the LineGeometry, so should return true");
            TestContains5(new Point(100, 100), 0, ToleranceType.Absolute, true);

            CommonLib.LogStatus("hit point (105, 115) is not in the stroke of the pen, but with the tolerance, it is in, so the result should be true");
            TestContains6(new Pen(Brushes.Red, 1), new Point(105, 115), 2000, ToleranceType.Relative, true);

            CommonLib.LogStatus("Since LineGeometry has no filled region, this API result return Empty for any hit geometry");
            TestContainsWithDetail1(new RectangleGeometry(new Rect(100, 100, 0, 0)), IntersectionDetail.Empty);

            CommonLib.LogStatus("Hit geometry falls in the stroke of the pen, so the result should be FullContains");
            PathGeometry hitPG = new PathGeometry();
            PathFigure hitPF = new PathFigure();
            hitPF.StartPoint = new Point(102, 102);
            hitPF.Segments.Add(new PolyLineSegment(new Point[] { new Point(105, 102), new Point(105, 105), new Point(102, 105) }, true));
            hitPF.IsClosed = true;
            hitPG.Figures.Add(hitPF);
            TestContainsWithDetail2(new Pen(Brushes.Black, 20), hitPG, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("LineGeometry has no filled region, so no matter how big the tolerance is, the result should be Empty");
            TestContainsWithDetail3(new RectangleGeometry(new Rect(105, 105, 20, 20)), 100, ToleranceType.Absolute, IntersectionDetail.Empty);

            CommonLib.LogStatus("The hit PathGeometry will fall in the stroke of the pen, so the result is FullContains");
            TestContainsWithDetail4(new Pen(Brushes.Green, 100), hitPG, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);
        }
    }
}