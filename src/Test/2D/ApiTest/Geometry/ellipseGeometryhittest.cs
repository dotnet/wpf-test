// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  HitTest tests for EllipseGeometry
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Graphics
{
    internal class EllipseGeometryHitTest : HitTestBase
    {
        public EllipseGeometryHitTest( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext ctx)
        {
            testedGeometry = new EllipseGeometry(new Point(100, 100), 200, 200);
            RunBVT();

            CommonLib.LogTest("Result for: EllipseGeometry HitTesting");
        }

        private void RunBVT()
        {
            CommonLib.LogStatus("The hit geometry is in the target EllipseGeometry, so expect True.");
            TestContains1(new RectangleGeometry(new Rect(150, 150, 30, 30)), true);

            CommonLib.LogStatus("The hit RectangleGeometry is NOT in the target EllipseGeometry, so expect False.");
            TestContains1(new RectangleGeometry(new Rect(-1000, 0, 1, 1)), false);

            CommonLib.LogStatus("The Point(120, 120) is in the EllipseGeometry, so the result should be True");
            TestContains2(new Point(120, 120), true);

            CommonLib.LogStatus("The Poine(1000, 10000) is NOT in the EllipseGeometry, so the result should be False");
            TestContains2(new Point(1000, 10000), false);

            CommonLib.LogStatus("The Poine(310, 100) is NOT in the EllipseGeometry, but it falls on the PenWidth, so the result should be True");
            TestContains3(new Pen(Brushes.Black, 60), new Point(310, 100), true);

            CommonLib.LogStatus("The Poine(-20, 90) is NOT in the EllipseGeometry, nor it falls in the penwidth, so the result should be False");
            TestContains3(new Pen(Brushes.Black, 20), new Point(-20, 90), false);

            CommonLib.LogStatus("The hit geometry is inside of the target EllipseGeometry with Absolute Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(310, 100, 20, 20)), 100, ToleranceType.Absolute, false);

            CommonLib.LogStatus("The hit RectangleGeometry is inside of the target EllipseGeometry with Relative Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(120, 120, 20, 20)), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The hit Point(110,200) is inside of the target EllipseGeometry with Absolute Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Absolute, true);


            CommonLib.LogStatus("The hit Point(110,200) is inside of the target EllipseGeometry with Relative Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The hit Point(305,100) is NOT inside of the target EllipseGeometry with Absolute Tolerance, but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(305, 100), 0, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The hit Point(305, 100) is NOT inside of the target EllipseGeometry with Relative Tolerance. but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(305, 100), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The target EllipseGeometry fully contains the hit EllipseGeometry, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail1(new EllipseGeometry(new Point(200, 200), 10, 10), IntersectionDetail.FullyContains);

            PathGeometry hitGeometry = new PathGeometry();
            PathFigure hitPF = new PathFigure();
            hitPF.StartPoint = new Point(100, 310);
            hitPF.Segments.Add(new PolyLineSegment(new Point[] { new Point(104, 300), new Point(104, 305) }, true));
            hitPF.IsClosed = true;
            hitGeometry.Figures.Add(hitPF);
            CommonLib.LogStatus("The hit PathGeometry fully falls on the Stroke of the pen, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail2(new Pen(Brushes.Black, 40), hitGeometry, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully inside of the EllipseGeometry with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully inside of the EllipseGeometry with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);
        }
    }
}