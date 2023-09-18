// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  HitTest tests for GeometryGroup
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class GeometryGroupHitTest : HitTestBase
    {
        public GeometryGroupHitTest( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext ctx)
        {
            testedGeometry = new GeometryGroup();
            ((GeometryGroup)testedGeometry).Children.Add(new RectangleGeometry(new Rect(100, 100, 100, 100)));
            ((GeometryGroup)testedGeometry).Children.Add(new RectangleGeometry(new Rect(150, 150, 150, 150)));

            RunBVT();

            CommonLib.LogTest("Result for HitTesting of GeometryGroup");
        }

        private void RunBVT()
        {
            CommonLib.LogStatus("The hit geometry is in the target GeometryGroup, so expect True.");
            TestContains1(new RectangleGeometry(new Rect(100, 100, 30, 30)), true);

            CommonLib.LogStatus("The hit RectangleGeometry is NOT in the target Geometry, so expect False.");
            TestContains1(new RectangleGeometry(new Rect(-1000, 0, 1, 1)), false);

            CommonLib.LogStatus("The Point(120, 120) is in the GeometryGroup, so the result should be True");
            TestContains2(new Point(120, 120), true);

            CommonLib.LogStatus("The Poine(1000, 10000) is NOT in the GeometryGroup, so the result should be False");
            TestContains2(new Point(1000, 10000), false);

            CommonLib.LogStatus("The Poine(120, 210) is NOT in the GeometryGroup, but it falls on the PenWidth, so the result should be True");
            TestContains3(new Pen(Brushes.Black, 40), new Point(120, 210), true);

            CommonLib.LogStatus("The Poine(-20, 90) is NOT in the GeometryGroup, nor it falls in the penwidth, so the result should be False");
            TestContains3(new Pen(Brushes.Black, 20), new Point(-20, 90), false);

            CommonLib.LogStatus("The hit geometry is inside of the target GeometryGroup with Absolute Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(100, 100, 20, 20)), 100, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The hit RectangleGeometry is inside of the target GeometryGroup with Relative Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(120, 120, 20, 20)), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The hit Point(110,200) is inside of the target GeometryGroup with Absolute Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The hit Point(110,200) is inside of the target GeometryGroup with Relative Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The hit Point(205,200) is NOT inside of the target GeometryGroup with Absolute Tolerance, but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(205, 200), 0, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The hit Point(205, 200) is NOT inside of the target RectangleGeometry with Relative Tolerance. but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(205, 200), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The target GeometryGroup fully contains the hit Geometry, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail1(new EllipseGeometry(new Point(120, 120), 10, 10), IntersectionDetail.FullyContains);

            PathGeometry hitGeometry = new PathGeometry();
            hitGeometry.AddGeometry(new RectangleGeometry(new Rect(150, 205, 5, 5)));

            CommonLib.LogStatus("The hit PathGeometry fully falls on the Stroke of the pen, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail2(new Pen(Brushes.Black, 60), hitGeometry, IntersectionDetail.FullyContains);

            //CommonLib.LogStatus("The hitGeometry is fully inside of the GeometryGroup with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            //TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);
        }
    }
}