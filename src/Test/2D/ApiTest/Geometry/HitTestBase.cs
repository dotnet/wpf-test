// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  Base class for all HitTest geometry tests
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class HitTestBase : ApiTest
    {
        public HitTestBase( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
        }

        protected override void OnRender(DrawingContext ctx)
        {
        }

        protected void TestContains1(Geometry g, bool expectedResult)
        {
            CommonLib.LogStatus("Testing FillContains(Geometry) - expectedResult=" + expectedResult);
            bool realResult = testedGeometry.FillContains(g);
            CommonLib.GenericVerifier(realResult == expectedResult, "Contains(Geometry)");
        }

        protected void TestContains2(Point p, bool expectedResult)
        {
            CommonLib.LogStatus("Testing FillContains(Point)");
            bool realResult = testedGeometry.FillContains(p);
            CommonLib.GenericVerifier(realResult == expectedResult, "Contains(Point)");
        }

        protected void TestContains3(Pen pen, Point point, bool expectedResult)
        {
            CommonLib.LogStatus("Testing FillContains(Pen, Point)");
            bool realResult = testedGeometry.StrokeContains(pen, point);
            CommonLib.GenericVerifier(realResult == expectedResult, "Contains(Pen, Point)");
        }

        protected void TestContains4(Geometry g, Double d, ToleranceType tt, bool expectedResult)
        {
            CommonLib.LogStatus("Testing FillContains(Geometry, Double, ToleranceType)");
            bool realResult = testedGeometry.FillContains(g, d, tt);
            CommonLib.GenericVerifier(realResult == expectedResult, "Contains(Geometry, Double, ToleranceType)");
        }

        protected void TestContains5(Point point, Double d, ToleranceType tt, bool expectedResult)
        {
            CommonLib.LogStatus("Testing FillContains(Point, Double, ToleranceType)");
            bool realResult = testedGeometry.FillContains(point, d, tt);
            CommonLib.GenericVerifier(realResult == expectedResult, "Contains(Point, Double, ToleranceType)");
        }

        protected void TestContains6(Pen pen, Point point, Double d, ToleranceType tt, bool expectedResult)
        {
            CommonLib.LogStatus("Testing StrokeContains(Pen, Point, Double, ToleranceType)");
            bool realResult = testedGeometry.StrokeContains(pen, point, d, tt);
            CommonLib.GenericVerifier(realResult == expectedResult, "Contains(Pen, Point, Double, ToleranceType)");
        }

        protected void TestContainsWithDetail1(Geometry g, IntersectionDetail expectedResult)
        {
            CommonLib.LogStatus("Testing FillContainsWithDetail(Geometry)");
            IntersectionDetail realResult = testedGeometry.FillContainsWithDetail(g);
            VerifyResult(expectedResult, realResult, "ContainsWithDetail(Geometry)");
        }

        protected void TestContainsWithDetail2(Pen pen, PathGeometry pathG, IntersectionDetail expectedResult)
        {
            CommonLib.LogStatus("Testing StrokeContainsWithDetail(Pen, PathGeometry)");
            IntersectionDetail realResult = testedGeometry.StrokeContainsWithDetail(pen, pathG);
            VerifyResult(expectedResult, realResult, "ContainsWithDetail(Pen, PathGeometry)");
        }

        protected void TestContainsWithDetail3(Geometry g, Double d, ToleranceType tt, IntersectionDetail expectedResult)
        {
            CommonLib.LogStatus("Testing FillContainsWithDetail(Geometry, Double, ToleranceType)");
            IntersectionDetail realResult = testedGeometry.FillContainsWithDetail(g, d, tt);
            VerifyResult(expectedResult, realResult, "ContainsWithDetail(Geometry, Double, ToleranceType)");
        }

        protected void TestContainsWithDetail4(Pen pen, PathGeometry pathG, Double d, ToleranceType tt, IntersectionDetail expectedResult)
        {
            CommonLib.LogStatus("Testing StrokeContainsWithDetail(Pen, PathGeometry, Double, ToleranceType)");
            IntersectionDetail realResult = testedGeometry.StrokeContainsWithDetail(pen, pathG, d, tt);
            VerifyResult(expectedResult, realResult, "ContainsWithDetail(Pen, PathGeometry, Double, ToleranceType)");
        }

        protected void TestVisualTreeHelperPointHitTestOnClippedGeometry(Geometry clippedRegion, Geometry geometry, Point p, bool expectedResult)
        {
            CommonLib.LogStatus("Testing VisualTreeHelper.PointHitTest on a clipped Geometry");

            DrawingVisual testVisual = new DrawingVisual();
            using (DrawingContext ctx = testVisual.RenderOpen())
            {
                ctx.PushClip(clippedRegion);
                ctx.DrawGeometry(Brushes.Black, null, geometry);
            }

            PointHitTestResult hitResult = VisualTreeHelper.HitTest(testVisual, p) as PointHitTestResult;

            bool actualHitResult = (hitResult != null && VisualTreeHelper.GetContentBounds(hitResult.VisualHit).Equals(VisualTreeHelper.GetContentBounds(testVisual))) ? true : false;

            VerifyResult(expectedResult, actualHitResult, "PointHitTest on Clipped Geometry");
        }

        protected void VerifyResult(object expectedResult, object realResult, string message)
        {
            if (CommonLib.DeepEqual(expectedResult, realResult))
            {
                CommonLib.LogStatus("Pass:  " + message);
            }
            else
            {
                CommonLib.LogFail(message);
                CommonLib.LogStatus("Expected result = " + expectedResult + "; Real result =" + realResult);;
            }
        }

        protected Geometry testedGeometry = null;
    }
}