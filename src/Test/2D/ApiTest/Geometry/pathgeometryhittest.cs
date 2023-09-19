// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  HitTest tests for PathGeometry
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class PathGeometryHitTest : HitTestBase
    {
        public PathGeometryHitTest( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext ctx)
        {
            testedGeometry = new PathGeometry();
            ((PathGeometry)testedGeometry).AddGeometry(new EllipseGeometry(new Point(200, 100), 80, 50));
            ((PathGeometry)testedGeometry).AddGeometry(new RectangleGeometry(new Rect(100, 150, 200, 200)));


            RunBVT();

                CommonLib.LogTest("PathGeometry HitTesting result:");

        }

        private void RunBVT()
        {
            CommonLib.LogStatus("The hit geometry is in the target, so expect True.");
            TestContains1(new RectangleGeometry(new Rect(120, 170, 50, 50)), true);

            CommonLib.LogStatus("The hit RectangleGeometry is NOT in the target, so expect False.");
            TestContains1(new EllipseGeometry(new Rect(-1000, 0, 1, 1)), false);

            CommonLib.LogStatus("The hit Point(120, 120) is in the target, so the result should be True");
            TestContains2(new Point(200, 120), true);

            CommonLib.LogStatus("The Poine(1000, 10000) is NOT in the target Geometry, so the result should be False");
            TestContains2(new Point(1000, 10000), false);

            CommonLib.LogStatus("The hit Poine(80, 200) is NOT in the target geometry, but it falls on the PenWidth, so the result should be True");
            TestContains3(new Pen(Brushes.Black, 60), new Point(80, 200), true);

            CommonLib.LogStatus("The Poine(-20, 90) is NOT in the target PathGeometry, nor it falls in the penwidth, so the result should be False");
            TestContains3(new Pen(Brushes.Black, 20), new Point(-20, 90), false);

            CommonLib.LogStatus("The hit geometry is inside of the target PathGeometry with Absolute Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(110, 200, 50, 50)), 100, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The hit Geometry is inside of the target PathGeometry with Relative Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(110, 200, 50, 50)), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The hit Point(110,200) is inside of the target EllipseGeometry with Absolute Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The hit Point(110,200) is inside of the target EllipseGeometry with Relative Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The hit Point(100, 145) is NOT inside of the target EllipseGeometry with Absolute Tolerance, but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(100, 145), 0, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The hit Point(100, 145) is NOT inside of the target RectangleGeometry with Relative Tolerance. but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(100, 145), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The target EllipseGeometry fully contains the hit EllipseGeometry, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail1(new EllipseGeometry(new Point(200, 200), 10, 10), IntersectionDetail.FullyContains);

            PathGeometry hitGeometry = new PathGeometry();
            hitGeometry.AddGeometry(new RectangleGeometry(new Rect(90, 180, 20, 20)));
            CommonLib.LogStatus("The hit PathGeometry fully falls on the Stroke of the pen, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail2(new Pen(Brushes.Black, 40), hitGeometry, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully inside of the PathGeometry with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully inside of the target PathGeometry with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);
        }
    }
}