// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  HitTest tests for StreamGeometry
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class StreamGeometryHitTest : HitTestBase
    {
        public StreamGeometryHitTest( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext ctx)
        {
            RunBVT();
            CommonLib.LogTest("Results for Hittesting with EllipseGeometry");
        }

        private void RunBVT()
        {
            testedGeometry = CreateStreamGeometry(new EllipseGeometry(new Point(100, 100), 200, 200));

            CommonLib.LogStatus("The hit geometry is in the target StreamGeometry, so expect True.");
            TestContains1(new RectangleGeometry(new Rect(150, 150, 30, 30)), true);

            CommonLib.LogStatus("The hit RectangleGeometry is NOT in the target StreamGeometry, so expect False.");
            TestContains1(new RectangleGeometry(new Rect(-1000, 0, 1, 1)), false);

            CommonLib.LogStatus("The Point(120, 120) is in the StreamGeometry, so the result should be True");
            TestContains2(new Point(120, 120), true);

            CommonLib.LogStatus("The Poine(1000, 10000) is NOT in the StreamGeometry, so the result should be False");
            TestContains2(new Point(1000, 10000), false);

            CommonLib.LogStatus("The Poine(310, 100) is NOT in the StreamGeometry, but it falls on the PenWidth, so the result should be True");
            TestContains3(new Pen(Brushes.Black, 60), new Point(310, 100), true);

            CommonLib.LogStatus("The Poine(-20, 90) is NOT in the StreamGeometry, nor it falls in the penwidth, so the result should be False");
            TestContains3(new Pen(Brushes.Black, 20), new Point(-20, 90), false);

            CommonLib.LogStatus("The hit geometry is inside of the target StreamGeometry with Absolute Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(310, 100, 20, 20)), 100, ToleranceType.Absolute, false);

            CommonLib.LogStatus("The hit RectangleGeometry is inside of the target StreamGeometry with Relative Tolerance, so the result should be True");
            TestContains4(new RectangleGeometry(new Rect(120, 120, 20, 20)), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The hit Point(110,200) is inside of the target StreamGeometry with Absolute Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Absolute, true);


            CommonLib.LogStatus("The hit Point(110,200) is inside of the target StreamGeometry with Relative Tolerance, so the result should be True");
            TestContains5(new Point(110, 200), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The hit Point(305,100) is NOT inside of the target StreamGeometry with Absolute Tolerance, but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(305, 100), 0, ToleranceType.Absolute, true);

            CommonLib.LogStatus("The hit Point(305, 100) is NOT inside of the target StreamGeometry with Relative Tolerance. but it is on the PenWidth, so the result should be True");
            TestContains6(new Pen(Brushes.Black, 20), new Point(305, 100), 0, ToleranceType.Relative, true);

            CommonLib.LogStatus("The target EllipseGeometry fully contains the hit StreamGeometry, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail1(new EllipseGeometry(new Point(200, 200), 10, 10), IntersectionDetail.FullyContains);

            PathGeometry hitGeometry = new PathGeometry();
            PathFigure hitPF = new PathFigure();
            hitPF.StartPoint = new Point(100, 310);
            hitPF.Segments.Add(new PolyLineSegment(new Point[] { new Point(104, 300), new Point(104, 305) }, true));
            hitPF.IsClosed = true;
            hitGeometry.Figures.Add(hitPF);
            CommonLib.LogStatus("The hit PathGeometry fully falls on the Stroke of the pen, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail2(new Pen(Brushes.Black, 40), hitGeometry, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully inside of the StreamGeometry with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully inside of the StreamGeometry with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail3(hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Absolute tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Absolute, IntersectionDetail.FullyContains);

            CommonLib.LogStatus("The hitGeometry is fully in the stroke of the pen with Relative tolerance, so the expect IntersectionDetail will be FullyContains");
            TestContainsWithDetail4(new Pen(Brushes.Black, 40), hitGeometry, 0, ToleranceType.Relative, IntersectionDetail.FullyContains);
        }

        private StreamGeometry CreateStreamGeometry(Geometry g)
        {
            // It is the only way to generate an empty StreamGeometry in our model.
            if (g == null)
            {
                return new StreamGeometry();
            }

            //First convert any Geometry object into PathGeometry
            PathGeometry pg = PathGeometry.CreateFromGeometry(g);
            if (pg == null)
            {
                throw new ApplicationException("Something is wrong: pg SHOULD NOT be null!");
            }

            StreamGeometry streamGeometry = new StreamGeometry();

            // Only when the g is PathGeometry, we need to directly get the Transform from it.
            // For other geometry types, we don't have to do that.
            // The reason is when doing the PathGeometry.CreatFrom( g ), the transform has already transformed
            //  the points of the geometry, so the returned PathGeometry from the call contains the Transform
            //  information already.
            if (g is PathGeometry)
            {
                streamGeometry.Transform = g.Transform;
            }

            // Getting FillRule value from the generated PathGeometry
            streamGeometry.FillRule = pg.FillRule;

            using (StreamGeometryContext sgc = streamGeometry.Open())
            {
                foreach (PathFigure pf in pg.Figures)
                {
                    sgc.BeginFigure(pf.StartPoint /* Start Point */, pf.IsFilled /* is filled */, pf.IsClosed /* is Closed */ );
                    foreach (PathSegment ps in pf.Segments)
                    {
                        SegmentOneToOneMatch(sgc, ps);
                    }
                }
            }
            return streamGeometry;
        }

        private void SegmentOneToOneMatch(StreamGeometryContext sgc, PathSegment ps)
        {
            if (ps is ArcSegment)
            {
                sgc.ArcTo(((ArcSegment)ps).Point,
                        ((ArcSegment)ps).Size,
                        ((ArcSegment)ps).RotationAngle,
                        ((ArcSegment)ps).IsLargeArc,
                        ((ArcSegment)ps).SweepDirection,
                        ((ArcSegment)ps).IsStroked,
                        ((ArcSegment)ps).IsSmoothJoin
                        );
            }
            else
                if (ps is BezierSegment)
                {
                    sgc.BezierTo(((BezierSegment)ps).Point1,
                            ((BezierSegment)ps).Point2,
                            ((BezierSegment)ps).Point3,
                            ((BezierSegment)ps).IsStroked,
                            ((BezierSegment)ps).IsSmoothJoin
                            );
                }
                else
                    if (ps is LineSegment)
                    {
                        sgc.LineTo(((LineSegment)ps).Point,
                                ((LineSegment)ps).IsStroked,
                                ((LineSegment)ps).IsSmoothJoin
                                );
                    }
                    else
                        if (ps is PolyBezierSegment)
                        {
                            sgc.PolyBezierTo(((PolyBezierSegment)ps).Points,
                                    ((PolyBezierSegment)ps).IsStroked,
                                    ((PolyBezierSegment)ps).IsSmoothJoin
                                    );
                        }
                        else
                            if (ps is PolyLineSegment)
                            {
                                sgc.PolyLineTo(((PolyLineSegment)ps).Points,
                                        ((PolyLineSegment)ps).IsStroked,
                                        ((PolyLineSegment)ps).IsSmoothJoin
                                        );
                            }
                            else
                                if (ps is PolyQuadraticBezierSegment)
                                {
                                    sgc.PolyQuadraticBezierTo(((PolyQuadraticBezierSegment)ps).Points,
                                            ((PolyQuadraticBezierSegment)ps).IsStroked,
                                            ((PolyQuadraticBezierSegment)ps).IsSmoothJoin
                                            );
                                }
                                else
                                    if (ps is QuadraticBezierSegment)
                                    {
                                        sgc.QuadraticBezierTo(((QuadraticBezierSegment)ps).Point1,
                                                ((QuadraticBezierSegment)ps).Point2,
                                                ((QuadraticBezierSegment)ps).IsStroked,
                                                ((QuadraticBezierSegment)ps).IsSmoothJoin
                                                );
                                    }
                                    else
                                    {
                                        throw new ApplicationException("Unsupported Segment type!");
                                    }

        }
    }
}