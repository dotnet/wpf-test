// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  HitTest tests for RectangleGeometry
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class RectangleGeometryHitTest : HitTestBase
    {
        public RectangleGeometryHitTest( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        
        protected override void OnRender(DrawingContext ctx)
        {
            testedGeometry = new RectangleGeometry(new Rect(100, 100, 200, 200), 20, 20);
            RunBVT();

            CommonLib.LogTest("Result for RectangleGeometry with HitTesting");
        }

        private void RunBVT()
        {
            CommonLib.LogStatus("The target geometry EllipseGeometry is in the RectangleGeometry, so expect True.");
            TestContains1(new EllipseGeometry(new Rect(150, 150, 30, 30)), true);

            CommonLib.LogStatus("The target geometry RectangleGeometry is NOT in the base RectangleGeometry, so expect False.");
            TestContains1(new RectangleGeometry(new Rect(0, 0, 1, 1)), false);

            CommonLib.LogStatus("The Poine(120, 120) is in the RectangleGeometry, so the result should be True");
            TestContains2(new Point(120, 120), true);

            CommonLib.LogStatus("The Poine(0, 0) is NOT in the RectangleGeometry, so the result should be False");
            TestContains2(new Point(0, 0), false);

            CommonLib.LogStatus("The Poine(90, 90) is NOT in the RectangleGeometry, but it falls on the PenWidth, so the result should be True");
            TestContains3(new Pen(Brushes.Black, 60), new Point(90, 90), true);

            CommonLib.LogStatus("The Poine(-20, 90) is NOT in the RectangleGeometry, nor it falls in the penwidth, so the result should be False");
            TestContains3(new Pen(Brushes.Black, 20), new Point(-20, 90), false);

            CommonLib.LogStatus("The tested RectangleGeometry is inside of the target RectangleGeometry with Absolute Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(120, 120, 20, 20)), 0, ToleranceType.Absolute, true);


            CommonLib.LogStatus("The tested RectangleGeometry is inside of the target RectangleGeometry with Relative Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(120, 120, 20, 20)), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The tested Point(110,200) is inside of the target RectangleGeometry with Absolute Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Absolute, true);


            CommonLib.LogStatus("The tested Point(110,200) is inside of the target RectangleGeometry with Relative Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The tested Point(110,200) is NOT inside of the target RectangleGeometry with Absolute Tolerance, but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(110, 200), 0, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The tested Point(110, 200) is NOT inside of the target RectangleGeometry with Relative Tolerance. but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(110, 200), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The target RectangleGeometry fully contains the hit EllipseGeometry, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail1(new EllipseGeometry(new Point(200, 200), 10, 10), IntersectionDetail.FullyContains);

            PathGeometry hitGeometry = new PathGeometry();
            PathFigure hitPF = new PathFigure();
            hitPF.StartPoint = new Point(110, 200);
            hitPF.Segments.Add(new PolyLineSegment(new Point[] { new Point(114, 200), new Point(114, 210) }, true));
            hitPF.IsClosed = true;
            hitGeometry.Figures.Add(hitPF);
            CommonLib.LogStatus("The hit PathGeometry fully falls on the Stroke of the pen, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail2(new Pen(Brushes.Black, 40), hitGeometry, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully inside of the RectangleGeometry with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully inside of the RectangleGeometry with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);
        }
    }
}