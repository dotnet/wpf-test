// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  Extent Circular test for Geometry
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for CloseSegmentClass.
    /// </summary>
    internal class CircularTest : ApiTest
    {
        public CircularTest( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("Circular Test");

            #region Test 1: HitTest on Geometry
            CommonLib.LogStatus("Test 1.1: HitTest on EllipseGeometry");
            CircularTestGeometry(new EllipseGeometry(new Point(100, 200), 200, 100));

            CommonLib.LogStatus("Test 1.2: HitTest on RectangleGeometry");
            CircularTestGeometry(new RectangleGeometry(new Rect(100, 200, 200, 100), 20, 30));

            CommonLib.LogStatus("Test 1.3: HitTest on PathGeoemtry with BowTie shape");
            PathGeometry path = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(0, 0);
            figure.Segments.Add(new LineSegment(new Point(100, 0), true));
            figure.Segments.Add(new LineSegment(new Point(0, 100), true));
            figure.Segments.Add(new LineSegment(new Point(100, 100), true));
            figure.IsClosed = true;
            path.Figures.Add(figure);
            CircularTestGeometry(path);

            CommonLib.LogStatus("Test 1.4: HitTest on PathGeometry.GetWidenedPathGeometry(new Pen(Brushes.Black, 0))");
            PathGeometry pathWidened = path.GetWidenedPathGeometry(new Pen(Brushes.Black, 10));
            CircularTestGeometry(pathWidened);

            CommonLib.LogStatus("Test 1.5: HitTest on LineGeometry");
            CircularTestGeometry(new LineGeometry(new Point(100, 100), new Point(200, 300)));
            #endregion

            #region Test 2: PathGeometry vs. PathGeometry.GetWidenedPathGeometry()
            CommonLib.LogStatus("Test 2: PathGeometry vs. PathGeometry.GetWidenedPathGeometry() in GetRenderBounds and HitTesting");
            PathGeometry pg2 = new PathGeometry();
            PathFigure figure2 = new PathFigure();
            figure2.StartPoint = new Point(75, 80);
            Point[] pointArray = new Point[] { new Point(-10, 0), new Point(210, 0), new Point(125, 80), new Point(210, 170), new Point(-10, 170), new Point(75, 80) };
            figure2.Segments.Add(new PolyBezierSegment(pointArray, true));
            figure2.IsClosed = true;
            pg2.Figures.Add(figure2);
            PathGeometry pg3 = pg2.GetWidenedPathGeometry(new Pen(Brushes.Black, 10));

            CommonLib.LogStatus("Test 2.1: GetArea() between PathGeometry and PathGeometry.GetWidenedPathGeometry");
            //GetArea() between PathGeometry and PathGeometry.GetWidenedPathGeometry
            PathGeometry pgSquare = new PathGeometry();
            PathFigure pfSquare = new PathFigure();
            pfSquare.StartPoint = new Point(50, 50);
            pfSquare.Segments.Add(new LineSegment(new Point(120, 50), true));
            pfSquare.Segments.Add(new LineSegment(new Point(120, 120), true));
            pfSquare.Segments.Add(new LineSegment(new Point(50, 120), true));
            pfSquare.IsClosed = true;
            pgSquare.Figures.Add(pfSquare);

            PathGeometry pgSquareWiden = pgSquare.GetWidenedPathGeometry(new Pen(Brushes.Black, 10));
            Console.WriteLine("Hittest: at point (47, 47): " + pgSquareWiden.FillContains(new Point(47, 47)));
            CombinedGeometry gcSquare = new CombinedGeometry(GeometryCombineMode.Exclude,
                new RectangleGeometry(new Rect(45, 45, 80, 80)),
                new RectangleGeometry(new Rect(55, 55, 60, 60)));

            CommonLib.LogStatus("pgSquareWiden.GetArea(0, ToleranceType.Absolute) = " + pgSquareWiden.GetArea(0, ToleranceType.Absolute));
            CommonLib.LogStatus("gcSquare.GetArea(0, ToleranceType.Absolute) = " + gcSquare.GetArea(0, ToleranceType.Absolute));
            CommonLib.GenericVerifier(pgSquareWiden.GetArea(0, ToleranceType.Absolute) == gcSquare.GetArea(0, ToleranceType.Absolute), "GetArea() in Circular test");

            CommonLib.LogStatus("Test 2.2: Bounds between PathGeometry and PathGeometry.GetWidenedPathGeometry()");
            CommonLib.LogStatus("pgSquareWiden.Bounds = " + pgSquareWiden.Bounds);
            CommonLib.LogStatus("gcSquare.Bounds = " + gcSquare.Bounds);
            CommonLib.RectVerifier(pgSquareWiden.Bounds, gcSquare.Bounds);

            CommonLib.LogStatus("Test 2.3: Verify pgSquareWiden has the same hittest number in the fixed resolution with gcSquare");
            //Verify pgSquareWiden has the same hittest number in the fixed resolution with gcSquare
            int pgHits = HitTest(pgSquareWiden, 100);
            int gcHits = HitTest(gcSquare, 100);
            CommonLib.LogStatus("pgSquareWiden's HitTest = " + pgHits);
            CommonLib.LogStatus("gcSquare's HitTest = " + gcHits);
            CommonLib.GenericVerifier(pgHits == gcHits, "HitTest of PathGeometry vs. PathGeometry.GetWidenedPathGeometry");

            #endregion

            #region Test 3: PathGeometry vs. PathFigure.GetFlattenPathFigure()
            CommonLib.LogStatus("Test 3: PathGeometry vs. PathGeometry.GetFlattenPathGeometry()");
            PathFigure pfFlatten = path.Figures[0].GetFlattenedPathFigure(0, ToleranceType.Absolute);
            PathGeometry pgFlatten = new PathGeometry();
            pgFlatten.Figures.Add(pfFlatten);

            CommonLib.LogStatus("Test 3.1:  GetArea(0) between PathGeometry vs PathFigure.GetFlattenedPathFigure");
            double pgArea = path.GetArea(0, ToleranceType.Absolute);
            double pgFlattenArea = pgFlatten.GetArea(0, ToleranceType.Absolute);
            CommonLib.LogStatus("GetArea(0, ToleranceType.Absolute) of PathGeometry = " + pgArea);
            CommonLib.LogStatus("GetArea(0, ToleranceType.Absolute) of pgFlatten    = " + pgFlattenArea);
            CommonLib.GenericVerifier(Math.Round(pgArea, 2) == Math.Round(pgFlattenArea, 2), "GetArea(0, ToleranceType.Absolute) of pgFlatten");

            CommonLib.LogStatus("Test 3.2:  Bounding box of PathGeometry and the pgFlatten");
            CommonLib.LogStatus("Bounds of pg = {" + path.Bounds + "}");
            CommonLib.LogStatus("Bounds of pgFlatten = {" + pgFlatten.Bounds + "}");
            CommonLib.RectVerifier(path.Bounds, pgFlatten.Bounds);

            CommonLib.LogStatus("Test 3.3:  HitTesting on PathGeometry and the pgFlatten");
            int pgHit = HitTest(path, 100);
            CommonLib.LogStatus("HitTest of path  = " + pgHit);

            int pgFlattenHit = HitTest(pgFlatten, 100);
            CommonLib.LogStatus("HitTest of pgFlatten = " + pgFlattenHit);

            CommonLib.GenericVerifier(pgHit == pgFlattenHit, "HitTest of pgFlatten");
            #endregion

            #region  Test 4:  Ellipse vs. 2 Arcs
            CommonLib.LogStatus("Test 4:  Ellipse vs. 2 Arcs");
            EllipseGeometry ellipse = new EllipseGeometry(new Point(200, 200), 20, 20);

            PathGeometry ellipsePG = new PathGeometry();
            PathFigure ellipsePF = new PathFigure();
            ellipsePF.StartPoint = new Point(180, 200);
            ellipsePF.Segments.Add(new ArcSegment(new Point(220, 200), new Size(20, 20), 0, true, SweepDirection.Clockwise, true));
            ellipsePF.Segments.Add(new ArcSegment(new Point(180, 200), new Size(20, 20), 0, true, SweepDirection.Clockwise, true));
            ellipsePG.Figures.Add(ellipsePF);

            //Bounding box of Ellipse and the 2 Arcs
            CommonLib.LogStatus("Test 4.1:  Bounding box of Ellipse and the 2 Arcs");
            CommonLib.LogStatus("Bounds of ellipse   = " + ellipse.Bounds);
            CommonLib.LogStatus("Bounds of ellipsePG = " + ellipsePG.Bounds);
            CommonLib.GenericVerifier(ellipse.Bounds == ellipsePG.Bounds, "Bounds of Ellipse and the 2 Arcs");

            //GetArea(0) of Ellipse and 2 Arcs
            CommonLib.LogStatus("Test 4.2:  GetArea(0, ToleranceType.Absolute) of Ellipse and 2 Arcs");
            CommonLib.LogStatus("GetArea(0, ToleranceType.Absolute) of ellipse   = " + ellipse.GetArea(0, ToleranceType.Absolute));
            CommonLib.LogStatus("GetArea(0, ToleranceType.Absolute) of ellipsePG = " + ellipsePG.GetArea(0, ToleranceType.Absolute));
            CommonLib.GenericVerifier(Math.Round(ellipse.GetArea(0, ToleranceType.Absolute), 0) == Math.Round(ellipsePG.GetArea(0, ToleranceType.Absolute), 0), "GetArea(0, ToleranceType.Absolute) of ellipse and 2 Arcs");

            //HitTesting between ellipse and the 2 Arcs
            CommonLib.LogStatus("Test 4.3:  HitTesting between ellipse and the 2 Arcs");
            int ellipseHit = HitTest(ellipse, 100);
            CommonLib.LogStatus("HitTest # of ellipse = " + ellipseHit);
            int ellipsePGHit = HitTest(ellipsePG, 100);
            CommonLib.LogStatus("HitTest # of 2 Arcs = " + ellipsePGHit);
            //As long as the difference is less than 50, then they are close enough.
            CommonLib.GenericVerifier(Math.Abs(ellipseHit - ellipsePGHit) < 50, "HitTesting between ellipse and the 2 Arcs");

            #endregion


            CommonLib.LogTest("Result for Circular Test");

            RectangleGeometry background = new RectangleGeometry(new Rect(0, 0, 400, 400));
            DC.DrawGeometry(Brushes.White, null, background);
            DC.DrawGeometry(Brushes.Black, new Pen(Brushes.Red, 1), ellipse);

            ellipsePG.Transform = new TranslateTransform(100, 0);
            DC.DrawGeometry(Brushes.Black, new Pen(Brushes.Red, 1), ellipsePG);

            DC.DrawGeometry(Brushes.Black, new Pen(Brushes.Red, 1), pgSquareWiden);

            //Shift the CombinedGeometry right of 100 pixel, so it will not block the PathGeometry
            gcSquare.Transform = new TranslateTransform(100, 0);
            DC.DrawGeometry(Brushes.Black, new Pen(Brushes.Red, 1), gcSquare);

            //Shift the flattened PathGeometry down of 120 pixel, so it will not block the PathGeometry
            pgFlatten.Transform = new TranslateTransform(0, 120);
            DC.DrawGeometry(Brushes.Black, new Pen(Brushes.Red, 1), pgFlatten);

        }

        /// <summary>
        /// Perform the test on a given geometry.
        /// Return false if the test failed
        /// </summary>
        public void CircularTestGeometry(Geometry geometry)
        {
            int resolution = 100;
            int probes = resolution * resolution;
            double margin = 0.1;
            double ComputedArea = geometry.GetArea(0, ToleranceType.Absolute);
            double BoundsArea = Math.Abs(geometry.Bounds.Width * geometry.Bounds.Height);
            int hits = 0;
            double dx = geometry.Bounds.Width / resolution;
            double dy = geometry.Bounds.Height / resolution;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (geometry.FillContains(new Point(geometry.Bounds.Left + i * dx,
                                                        geometry.Bounds.Top + j * dy)))
                    {
                        hits += 1;
                    }
                }
            }

            // hits / probes should be roughly equal to ComputedArea / BoundsArea
            double HitArea = BoundsArea * hits;
            HitArea /= probes;

            double error = Math.Abs(HitArea - ComputedArea);

            if (error <= BoundsArea * margin)
                CommonLib.GenericVerifier(true, geometry.ToString() + " HitTest and GetArea");
            else
                CommonLib.GenericVerifier(false, geometry.ToString() + " HitTest and GetArea");

        }

        /// <summary>
        /// Get HitTest hit number
        /// </summary>
        /// <param name="geo">The tested Geometry</param>
        /// <param name="resolution">the partition ratio</param>
        /// <returns># of Hits on the tested Geometry</returns>
        public int HitTest(Geometry geo, int resolution)
        {
            int hits = 0;

            double dx = geo.Bounds.Width / resolution;
            double dy = geo.Bounds.Height / resolution;

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (geo.FillContains(new Point(geo.Bounds.Left + i * dx,
                                                        geo.Bounds.Top + j * dy)))
                    {
                        hits += 1;
                    }
                }
            }

            return hits;
        }
    }
}