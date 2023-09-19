// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing PathGeometry class
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Collections;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for ArcSegmentClass.
    /// </summary>
    internal class PathGeometryClass : ApiTest
    {
        public PathGeometryClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {            
            Update();
        }

        private int GetPathLength(string pathString)
        {
            //tokenize the string
            string[] tokens = pathString.Split(' ');
            //return the number of tokens
            return tokens.Length;
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("PathGeometry Class");

            string objectType = "System.Windows.Media.PathGeometry";

            #region Section I: Initialization stage
            CommonLib.Stage = TestStage.Initialize;

            PathFigure pf1 = new PathFigure();

            pf1.StartPoint = new Point(20, 20);
            pf1.Segments.Add(new LineSegment(new Point(100, 20), true));
            pf1.IsClosed = true;

            PathFigure pf2 = new PathFigure();

            pf2.StartPoint = new Point(29, 10);
            pf2.Segments.Add(new BezierSegment(new Point(20, 10), new Point(100, 1), new Point(10, 140), true));
            pf2.IsClosed = true;

            PathFigure[] pathFigureList = new PathFigure[] { pf1, pf2 };

            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");

            PathGeometry pg1 = new PathGeometry();

            CommonLib.TypeVerifier(pg1, objectType);
            #endregion

            #region Test #2: PathGeometry(IEnumerable<PathFigure> figures);
            CommonLib.LogStatus("Test #2: PathGeometry(IEnumerable<PathFigure> figures)");
            PathGeometry pg2 = new PathGeometry(pathFigureList);
            CommonLib.TypeVerifier(pg2, objectType);
            #endregion

            #region Test #3: PathGeometry(IEnumerable<PathFigure> figures, FillRule FillRule, Transform transform)
            CommonLib.LogStatus("Test #3: PathGeometry(IEnumerable<PathFigure> figures, FillRule FillRule, Transform transform)");
            PathGeometry pg3 = new PathGeometry(pathFigureList, FillRule.EvenOdd, new RotateTransform(45));
            CommonLib.TypeVerifier(pg3, objectType);
            #endregion
            #endregion

            #region Section II: public property
            #region Test #5: Bounds property
            CommonLib.LogStatus("Test #5: Bounds property");
            Rect rect = pg3.Bounds;

            Rect rect1 = pg1.Bounds;

            PathGeometry pg5 = new PathGeometry();
            PathFigure pf5 = new PathFigure();
            pf5.StartPoint = new Point(10, 10);
            pf5.Segments.Add(new PolyLineSegment(new Point[] { new Point(100, 10), new Point(100, 100), new Point(10, 100) }, true));
            pf5.IsClosed = true;
            pg5.Figures.Add(pf5);
            pg5.Transform = new TranslateTransform(10, 10);

            CommonLib.RectVerifier(rect, new Rect(-91.92, 27.03, 148.49, 79.04));
            CommonLib.RectVerifier(rect1, Rect.Empty);
            CommonLib.RectVerifier(pg5.Bounds, new Rect(20, 20, 90, 90));
            #endregion

            #region Test #6a: Figures property with basevalue
            CommonLib.LogStatus("Test #6a: Figures property with basevalue");
            PathGeometry pg6a = new PathGeometry();
            CommonLib.GenericVerifier(pg6a.Figures.Count == 0, "Figures property with basevalue");
            #endregion

            #region Test #6b: Figures property in Invalid state
            CommonLib.LogStatus("Test #6b: Figure property in Invalid state");
            PathGeometry pg6b = new PathGeometry();
            pg6b.InvalidateProperty(PathGeometry.FiguresProperty);
            CommonLib.GenericVerifier(pg6b.Figures.Count == 0, "Figures property in Invalid state");
            #endregion

            #region Test #6: Figures property
            CommonLib.LogStatus("Test #6: Figures property");
            pg3.Figures.Add(new PathFigure(new Point(0, 0), new PathSegment[] { new ArcSegment(new Point(10, 10), new Size(20, 10), 2, true, SweepDirection.Counterclockwise, false) }, false));
            CommonLib.GenericVerifier(pg3.Figures.Count == 3, "Figures property");
            #endregion

            #region Test #7a:  FillRule property with basevalue
            CommonLib.LogStatus("Test #7a:  FillRule property with basevalue");
            PathGeometry pg7a = new PathGeometry();
            CommonLib.GenericVerifier(pg7a.FillRule == FillRule.EvenOdd, "FillRule with basevalue");
            #endregion

            #region Test #7b:  FillRule property in invalid stage
            CommonLib.LogStatus("Test #7b:  FillRule property in invalid stage");
            PathGeometry pg7b = new PathGeometry();
            pg7b.InvalidateProperty(PathGeometry.FillRuleProperty);
            CommonLib.GenericVerifier(pg7a.FillRule == FillRule.EvenOdd, "FillRule in invalid stage");
            #endregion

            #region Test #7: FillRule property
            CommonLib.LogStatus("Test #7: FillMode property");
            pg3.FillRule = FillRule.EvenOdd;
            CommonLib.GenericVerifier(pg3.FillRule == FillRule.EvenOdd, "FillRule property");
            #endregion

            #region Test #8: CanFreeze property
            CommonLib.LogStatus("Test #8: CanFreeze property");
            CommonLib.GenericVerifier(pg3.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #9: GetArea() method
            CommonLib.LogStatus("Test #9: GetArea() method");
            double result9 = pg3.GetArea();
            double result9Empty = pg1.GetArea();
            double result9Tran = pg5.GetArea();

            CommonLib.GenericVerifier(
                            Math.Round(result9, 2) == 3147.75 &&
                            MathEx.AreCloseEnough(result9Empty,0.0) &&
                            Math.Round(result9Tran, 2) == 8100.00,
                            "GetArea() method"
                            );
            #endregion

            #region Test #9.5: GetArea( Double, ToleranceType ) method
            CommonLib.LogStatus("Test #9.5: GetArea( Double, ToleranceType ) method");
            double result95 = pg3.GetArea(0, ToleranceType.Absolute);
            double result95Empty = pg1.GetArea(10000, ToleranceType.Absolute);
            double result95Tran = pg5.GetArea(-10, ToleranceType.Relative);

            CommonLib.GenericVerifier(
                            Math.Round(result95, 2) == 3158.51 &&
                            MathEx.AreCloseEnough(result95Empty , 0) &&
                            Math.Round(result95Tran, 2) == 8100.0,
                            "GetArea(Double, ToleranceType) method"
                            );
            #endregion
            #endregion

            #region Section III: public methods

            #region Test #11: Figures.Add()
            CommonLib.LogStatus("Test #11: Figures.Add()");
            pg3.Figures.Add(new PathFigure(new Point(10, 10), new PathSegment[] { new LineSegment(new Point(90, 10), false) }, false));
            CommonLib.GenericVerifier(pg3.Figures.Count == 4, "Figures.Add() method");
            #endregion

            #region Test #12: AddGeometry()
            CommonLib.LogStatus("Test #12: AddGeometry()");
            pg3.AddGeometry(new LineGeometry(new Point(1, 1), new Point(100, 100)));

            PathGeometry pg12 = new PathGeometry();
            pg12.AddGeometry(new LineGeometry());

            PathGeometry pg12_pg = new PathGeometry();
            pg12_pg.AddGeometry(new PathGeometry());

            PathGeometry pg12_Ellipse = new PathGeometry();
            pg12_pg.AddGeometry(new EllipseGeometry());

            PathGeometry pg12_CombinedGeometry = new PathGeometry();
            pg12_CombinedGeometry.AddGeometry(new CombinedGeometry());

            PathGeometry pg12_GeometryGroup = new PathGeometry();
            pg12_GeometryGroup.AddGeometry(new GeometryGroup());

            PathGeometry pg12_Rectangle = new PathGeometry();
            pg12_Rectangle.AddGeometry(new RectangleGeometry());

            PathGeometry pg12_AllSegments = new PathGeometry();
            pg12_AllSegments.Transform = new RotateTransform(45);
            PathFigure pf12_AllSegments = new PathFigure();
            pf12_AllSegments.StartPoint = new Point(1, 1);
            pf12_AllSegments.Segments.Add(new LineSegment(new Point(10, 20), true));
            pf12_AllSegments.Segments.Add(new ArcSegment(new Point(10, 100), new Size(1, 1), 45, true, SweepDirection.Counterclockwise, true));
            pf12_AllSegments.Segments.Add(new BezierSegment(new Point(10, -23), new Point(-100, -100), new Point(0, 10), true));
            pf12_AllSegments.Segments.Add(new PolyBezierSegment(new Point[] { new Point(-123, 10), new Point(double.Epsilon, double.PositiveInfinity) }, false));
            pf12_AllSegments.Segments.Add(new PolyLineSegment(new Point[] { new Point(123, 200), new Point(1002, 23020), new Point(0, 10) }, true));
            pf12_AllSegments.Segments.Add(new PolyQuadraticBezierSegment(new Point[] { new Point(123, 200), new Point(1002, 23020), new Point(0, 10) }, true));
            pf12_AllSegments.Segments.Add(new QuadraticBezierSegment(new Point(19, 23), new Point(0, 0), false));
            pf12_AllSegments.Segments.Add(new PolyLineSegment());
            pg12_AllSegments.Figures.Add(pf12_AllSegments);

            PathGeometry pg12_all = new PathGeometry();
            pg12_all.AddGeometry(pg12_AllSegments);

            RectangleGeometry rg12 = new RectangleGeometry();
            PathGeometry pg12_EmptyRG = new PathGeometry();
            pg12_EmptyRG.AddGeometry(rg12);

            bool result12 = pg3.Figures.Count == 5 && pg3.Figures[4].Segments.Count == 1 && pg3.Figures[4].Segments[0] is LineSegment;
            result12 &= pg12.Figures.Count == 1 && pg12.Figures[0].Segments[0] is LineSegment;
            result12 &= pg12_pg.Figures.Count == 1 && pg12_pg.Figures[0].Segments.Count == 4;
            result12 &= pg12_Ellipse.Figures.Count == 0;
            result12 &= pg12_CombinedGeometry.Figures.Count == 0;
            result12 &= pg12_GeometryGroup.Figures.Count == 0;
            result12 &= pg12_all.Figures.Count == 1 && pg12_all.Figures[0].Segments.Count == 8;
            result12 &= pg12_EmptyRG.Figures.Count == 0;

            CommonLib.GenericVerifier(result12, "AddGeometry() method");
            #endregion

            #region Test #14: Copy()
            CommonLib.LogStatus("Test #14: Copy() method");
            PathGeometry pg14 = pg3.Clone();
            CommonLib.GenericVerifier(pg14 != null && pg14.Figures.Count == pg3.Figures.Count, "Copy() method");
            #endregion

            #region Test #16: CloneCurrentValue()
            CommonLib.LogStatus("Test #16: CloneCurrentValue()");
            PathGeometry pg6 = pg3.CloneCurrentValue();
            CommonLib.GenericVerifier(pg6 != null && pg6 is PathGeometry && pg6.Figures.Count == pg3.Figures.Count, "CloneCurrentValue() method");
            #endregion

            #region Test #17: GetOutlinedPathGeometry()
            CommonLib.LogStatus("Test #17: GetOutlinedPathGeometry()");
            PathFigure pf17 = new PathFigure();

            pf17.StartPoint = new Point(20, 20);
            pf17.Segments.Add(new ArcSegment(new Point(180, 100), new Size(20, 20), 50, true, SweepDirection.Counterclockwise, true));
            pf17.Segments.Add(new LineSegment(new Point(100, 20), true));
            pf17.Segments.Add(new BezierSegment(new Point(20, 10), new Point(100, 1), new Point(10, 140), true));
            pf17.Segments.Add(new PolyBezierSegment(new Point[] { new Point(20, 20), new Point(100, 10), new Point(0, 100) }, true));
            pf17.Segments.Add(new PolyLineSegment(new Point[] { new Point(39, 10), new Point(54, 102), new Point(29, 39) }, true));
            pf17.Segments.Add(new PolyQuadraticBezierSegment(new Point[] { new Point(100, 20), new Point(20, 80) }, true));
            pf17.IsClosed = true;
            PathGeometry pg7_Main = new PathGeometry(new PathFigure[] { pf17 });
            PathGeometry pg7 = pg7_Main.GetOutlinedPathGeometry();
            CommonLib.GenericVerifier(pg7.Figures.Count == 10, "GetOutlinedPathGeometry() method");
            #endregion

            #region Test #18: GetWidenedPathGeometry()
            CommonLib.LogStatus("Test #18: GetWidenedPathGeometry()");
            PathGeometry pg8 = pg3.GetWidenedPathGeometry(new Pen(Brushes.Black, 3));
            CommonLib.GenericVerifier(pg8 != null, "GetWidenedPathGeometry() method");
            #endregion

            #region Test #20 - Contains(Point)
            CommonLib.LogStatus("Test #20 - Contains(Point)");
            PathGeometry pg20 = new PathGeometry(pathFigureList);

            //Point(30,10) is inside of pg20, but Point(100, 100) is not.
            CommonLib.GenericVerifier(pg20.FillContains(new Point(30, 10)) && !pg20.FillContains(new Point(100, 100)), "FillContains() metbod");
            #endregion

            #region Test #21 - GetPointAtFractionLength(double, out Point, out Point)
            CommonLib.LogStatus("Test #21 - GetPointFractionLength(double, out Point, out Point");
            PathGeometry pg21 = new PathGeometry();
            pg21.AddGeometry(new LineGeometry(new Point(10, 20), new Point(10, 100)));
            Point p1, p2;
            // Getting midpoint on the LineGeometry
            pg21.GetPointAtFractionLength(0.5, out p1, out p2);

            bool result21 = p1.Equals(new Point(10, 60)) && p2.Equals(new Point(0, 1));

            // Getting startpoint of the LineGeometry
            pg21.GetPointAtFractionLength(0, out p1, out p2);
            result21 &= p1.Equals(new Point(10, 20)) && p2.Equals(new Point(0, 1));

            // Getting EndPoint of the LineGeometry
            pg21.GetPointAtFractionLength(1, out p1, out p2);
            result21 &= p1.Equals(new Point(10, 100)) && p2.Equals(new Point(0, 1));

            // Getting a point that is out of the normal 0 and 1 range
            pg21.GetPointAtFractionLength(-100, out p1, out p2);
            result21 &= p1.Equals(new Point(10, 20)) && p2.Equals(new Point(0, 1));

            // Getting a point is way out of the 0 and 1 range
            pg21.GetPointAtFractionLength(2e245, out p1, out p2);
            result21 &= p1.Equals(new Point(10, 100)) && p2.Equals(new Point(0, 1));

            // Try to get point from an empty Geometry
            PathGeometry pg21_Empty = new PathGeometry();
            pg21_Empty.GetPointAtFractionLength(1, out p1, out p2);
            result21 &= p1.Equals(new Point()) && p2.Equals(new Point());

            CommonLib.GenericVerifier(result21, "GetPointAtFractionLength() method");
            #endregion

            #region Test #22 - GetRenderBounds(Pen)
            CommonLib.LogStatus("Test #22 - GetRenderBounds(Pen)");
            PathGeometry pg22 = new PathGeometry();
            pg22.AddGeometry(new LineGeometry(new Point(10, 20), new Point(10, 100)));
            Rect rect22 = pg22.GetRenderBounds(new Pen(Brushes.Red, 5.0));

            PathGeometry pg22Empty = new PathGeometry();
            Rect result22Empty = pg22Empty.GetRenderBounds(new Pen(Brushes.Black, 5.0));

            Rect result22Tran = pg5.GetRenderBounds(new Pen(Brushes.Red, 10.0));

            CommonLib.RectVerifier(rect22, new Rect(7.5, 20, 5, 80));
            CommonLib.RectVerifier(result22Empty, Rect.Empty);
            CommonLib.RectVerifier(result22Tran, new Rect(15, 15, 100, 100));
            #endregion

            #region Test #22.5 - GetRenderBounds( Pen, Double, ToleranceType )
            CommonLib.LogStatus("Test #22.5 - GetRenderBounds( Pen, Double, ToleranceType ) ");
            Rect result225 = pg22.GetRenderBounds(new Pen(Brushes.Red, 5.0), 0, ToleranceType.Absolute);
            Rect result225Empty = pg22Empty.GetRenderBounds(new Pen(Brushes.Black, 5.0), -10, ToleranceType.Absolute);
            Rect result225NoPen = pg5.GetRenderBounds(new Pen(Brushes.Red, 0), 1000, ToleranceType.Relative);
            Rect result225Tran = pg5.GetRenderBounds(new Pen(Brushes.Black, 10.0), 100, ToleranceType.Relative);

            CommonLib.RectVerifier(result225, new Rect(7.5, 20, 5, 80));
            CommonLib.RectVerifier(result225Empty, Rect.Empty);
            CommonLib.RectVerifier(result225NoPen, new Rect(20, 20, 90, 90));
            CommonLib.RectVerifier(result225Tran, new Rect(20, 20, 90, 90));
            #endregion

            #region Test #23 - Contains(Geometry)
            CommonLib.LogStatus("Test #23 - Contains(Geometry)");
            PathGeometry pg23 = new PathGeometry();
            pg23.AddGeometry(new RectangleGeometry(new Rect(new Point(44, 22), new Size(100, 100))));

            //pg23 does contain the RectangleGeometry, so Contains should return true.
            //However, pg23 doesn't contain the LineGeometry, so Contains should return false
            CommonLib.GenericVerifier(pg23.FillContains(new RectangleGeometry(new Rect(new Point(55, 40), new Size(20, 20)))) && !pg23.FillContains(new LineGeometry()), "FillContains(Geometry) method");
            #endregion

            #region Test #24 - ContainsWithDetail()
            CommonLib.LogStatus("Test #24 - ContainsWithDetail()");
            PathGeometry pg24 = new PathGeometry();
            pg24.AddGeometry(new RectangleGeometry(new Rect(new Point(10, 10), new Size(10, 10))));
            bool result24 = true;

            //The RectangleGeometry Intersects with the PathGeometry, so ContainsWithDetail should return IntersectionDetail.Intersects
            result24 &= pg24.FillContainsWithDetail(new RectangleGeometry(new Rect(0, 0, 15, 15))) == IntersectionDetail.Intersects;

            //This RectangleGeometry is inside of the PathGeometry, so ContainsWithDetail should return IntersectionDetail.FullContains
            result24 &= pg24.FillContainsWithDetail(new RectangleGeometry(new Rect(new Point(12, 12), new Size(5, 5)))) == IntersectionDetail.FullyContains;

            //This target geometry which is the PathGeometry is inside of the hit Geometry which is a bigger RectangleGeometry this time, so ContainsWidthDetail should return IntersectDetail.FullInside
            result24 &= pg24.FillContainsWithDetail(new RectangleGeometry(new Rect(new Point(0, 0), new Size(50, 50)))) == IntersectionDetail.FullyInside;

            //The hit geometry which is an EllipseGeometry has no intersection with the target PathGeometry, so ContainsWidthDetail should return Empty
            result24 &= pg24.FillContainsWithDetail(new EllipseGeometry(new Point(100, 100), 40, 40)) == IntersectionDetail.Empty;

            CommonLib.GenericVerifier(result24, "ContainsWithDetail() method");
            #endregion

            #region Test #25:  Regression case for Regression_Bug11
            CommonLib.LogStatus("Test #25: Regression case for Regression_Bug11");
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(0, 0);
            figure.Segments.Add(new ArcSegment(new Point(400, 0), new Size(200, 200), 0, true, SweepDirection.Counterclockwise, true));
            PathGeometry pg25 = new PathGeometry();
            pg25.Figures.Add(figure);
            Rect bound25 = pg25.Bounds;
            CommonLib.RectVerifier(bound25, new Rect(new Point(0, 0), new Size(400, 200)));
            #endregion

            #region Test #26:  Regression case for Regression_Bug12
            CommonLib.LogStatus("Test #26:  Regression case for Regression_Bug12");
            PathSegmentCollection collection = new PathSegmentCollection();
            collection.Insert(0, new LineSegment(new Point(0, 2), true));
            //No exception, means pass and no regression.
            CommonLib.LogStatus("No regression on Regression_Bug12");
            #endregion

            #region Test #27:  Regression case for Regression_Bug13
            CommonLib.LogStatus("Test #27:  Regression case for Regression_Bug13");
            PathGeometry pg27 = new PathGeometry();
            Rect bound27 = pg27.Bounds;
            if (bound27 == Rect.Empty)
            {
                CommonLib.LogStatus("No regression for Regression_Bug13");
            }
            #endregion

            #region Test #28:  Regression for Regression_Bug14
            CommonLib.LogStatus("Test #28:  Regresion test for Regression_Bug14");
            try
            {
                DC.DrawLine(null, new Point(10, 10), new Point(100, 10));
                CommonLib.LogStatus("Regression_Bug14 is still fixed, no regression!");
            }
            catch (System.NullReferenceException)
            {
                CommonLib.GenericVerifier(false, "Regression for Regression_Bug14");
            }
            #endregion

            #region Test #29:  Regression for Regression_Bug15
            CommonLib.LogStatus("Test #29:  Regression for Regression_Bug239");
            PathGeometry pg29 = new PathGeometry();
            pg29.AddGeometry(new LineGeometry(new Point(0, 0), new Point(0, 0)));
            pg29.AddGeometry(new LineGeometry(new Point(10, 10), new Point(100, 0)));
            PathGeometry pg29_temp = pg29.GetWidenedPathGeometry(new Pen(Brushes.Black, 3));
            CommonLib.LogStatus("No exception, no assertion, the Regression_Bug239 remains fixed");
            #endregion

            #region Test #30:  Regression for Regression_Bug16
            CommonLib.LogStatus("Test #30:  Regression for Regression_Bug16");
            PathGeometry pg = new PathGeometry();
            for (int i = 0; i < 8000; i++)
            {
                pg.AddGeometry(new RectangleGeometry(new Rect(new Point(2, 2), new Size(5, 5))));
            }
            CommonLib.LogStatus("No hang, no exception, then Regression_Bug17 stays fixed");
            #endregion

            #region Test #31:  Regression test for Regression_Bug238
            CommonLib.LogStatus("Test #31: Regression test for Regression_Bug238");
            // box at 0.25, 0.25, dimensions .5x.5
            PathGeometry pg31 = new PathGeometry();
            PathFigure figure31 = new PathFigure();
            figure31.StartPoint = new Point(0.25, 0.25);
            figure31.Segments.Add(new LineSegment(new Point(0.75, 0.25), true));
            figure31.Segments.Add(new LineSegment(new Point(0.75, 0.75), true));
            figure31.Segments.Add(new LineSegment(new Point(0.25, 0.75), true));
            figure31.IsClosed = true;
            pg31.Figures.Add(figure31);

            // badPen: width: 0.1, DashArrays.Dash
            Pen badPen = new Pen(new SolidColorBrush(Color.FromRgb(0, 0, 0)), 0.1);
            badPen.DashStyle = DashStyles.Dash;
            PathGeometry pg31_temp = pg31.GetWidenedPathGeometry(badPen);
            CommonLib.LogStatus("No exception means Regression_Bug238 is still fixed");
            #endregion

            #region Test #32: private GetPathFigureCollection()
            CommonLib.LogStatus("Test #32: private GetPathFigureCollection()");
            PathGeometry pg32_inner = new PathGeometry();
            pg32_inner.AddGeometry(new LineGeometry(new Point(-32.2, 0.22), new Point(45647, 23.0000003)));
            PathGeometry pg32 = new PathGeometry();
            pg32.AddGeometry(pg32_inner);
            CommonLib.GenericVerifier(pg32.Figures[0].Segments[0] is LineSegment, "GetPathFigureCollection() method");
            #endregion

            #region Test #33: Clear() method
            CommonLib.LogStatus("Test #33: Clear() method");
            PathFigure pf33 = new PathFigure();
            pf33.IsClosed = true;
            pf33.StartPoint = new Point(23, 2);
            pf33.Segments.Add(new LineSegment(new Point(0, 32), true));
            pf33.Segments.Add(new BezierSegment(new Point(0, 23), new Point(-102, 32), new Point(32, 1), true));

            PathGeometry pg33 = new PathGeometry();
            pg33.Figures.Add(pf33);
            pg33.Figures.Add(new PathFigure());
            pg33.AddGeometry(new LineGeometry(new Point(32, 2), new Point(68, 6)));

            pg33.Clear();

            int count = pg33.Figures.Count;

            //logter Clear() calls, the figurecollection should contain no figure anymore.
            CommonLib.GenericVerifier(count == 0, "Clear()");
            #endregion

            #region Test #34: MayHaveCurves() method
            CommonLib.LogStatus("Test #34: MayHaveCurves() method");
            PathGeometry pg34_NoCurve = new PathGeometry();
            pg34_NoCurve.AddGeometry(new LineGeometry(new Point(23, 232), new Point(-12, 3)));

            PathGeometry pg34_Curves = new PathGeometry();
            pg34_Curves.AddGeometry(new EllipseGeometry(new Point(23, 3), 32, 100));
            pg34_Curves.AddGeometry(new RectangleGeometry(new Rect(23, 12, 3, 100)));

            PathGeometry pg34_Empty = new PathGeometry();

            PathGeometry pg34_ClearFigures = new PathGeometry();
            pg34_ClearFigures.AddGeometry(new EllipseGeometry(new Point(0, 0), 10, 100));
            pg34_ClearFigures.Figures.Clear();

            bool result34 = pg34_NoCurve.MayHaveCurves() == false;
            result34 &= pg34_Curves.MayHaveCurves() == true;
            result34 &= pg34_Empty.MayHaveCurves() == false;
            result34 &= pg34_ClearFigures.MayHaveCurves() == false;

            CommonLib.GenericVerifier(result34, "MayHaveCurves()");
            #endregion

            #region Test: 34.1:  IsEmpty() method
            CommonLib.LogStatus("Test #35: IsEmpty() method");
            PathGeometry pg341_NoFigure = new PathGeometry();

            PathGeometry pg341_ClearFigures = new PathGeometry();
            pg341_ClearFigures.Figures.Add(new PathFigure());
            pg341_ClearFigures.Figures.Add(new PathFigure());
            pg341_ClearFigures.Figures.Clear();

            PathGeometry pg341_Normal = new PathGeometry();
            pg341_Normal.Figures.Add(new PathFigure());
            pg341_Normal.AddGeometry(new LineGeometry(new Point(10, 2), new Point(10, 10)));

            PathGeometry pg341_Empty = new PathGeometry();

            PathGeometry pg341_AllSegments = new PathGeometry();
            pg341_AllSegments.Transform = new RotateTransform(45);
            PathFigure pf341_AllSegments = new PathFigure();
            pf341_AllSegments.StartPoint = new Point(1, 1);
            pf341_AllSegments.Segments.Add(new LineSegment(new Point(10, 20), true));
            pf341_AllSegments.Segments.Add(new ArcSegment(new Point(10, 100), new Size(1, 1), 45, true, SweepDirection.Counterclockwise, true));
            pf341_AllSegments.Segments.Add(new BezierSegment(new Point(10, -23), new Point(-100, -100), new Point(0, 10), true));
            pf341_AllSegments.Segments.Add(new PolyBezierSegment(new Point[] { new Point(-123, 10), new Point(double.Epsilon, double.PositiveInfinity) }, false));
            pf341_AllSegments.Segments.Add(new PolyLineSegment(new Point[] { new Point(123, 200), new Point(1002, 23020), new Point(0, 10) }, true));
            pf341_AllSegments.Segments.Add(new PolyQuadraticBezierSegment(new Point[] { new Point(123, 200), new Point(1002, 23020), new Point(0, 10) }, true));
            pf341_AllSegments.Segments.Add(new QuadraticBezierSegment(new Point(19, 23), new Point(0, 0), false));
            pf341_AllSegments.Segments.Add(new PolyLineSegment());
            pg341_AllSegments.Figures.Add(pf341_AllSegments);

            bool result341 = pg341_NoFigure.IsEmpty() == true;
            result341 &= pg341_ClearFigures.IsEmpty() == true;
            result341 &= pg341_Normal.IsEmpty() == false;
            result341 &= pg341_Empty.IsEmpty() == true;
            result341 &= pg341_AllSegments.IsEmpty() == false;

            CommonLib.GenericVerifier(result341, "IsEmpty() method");
            #endregion

            #region Test: 34.2: CreateFromGeometry() method
            CommonLib.LogStatus("Test: 34.2: CreateFromGeometry() method");
            PathGeometry pg342_Good = PathGeometry.CreateFromGeometry(new RectangleGeometry());
            PathGeometry pg342_null = PathGeometry.CreateFromGeometry(null);

            StreamGeometry sg34_2 = new StreamGeometry();
            sg34_2.FillRule = FillRule.Nonzero;
            sg34_2.Transform = new RotateTransform(45);

            using (StreamGeometryContext sgc34_2 = sg34_2.Open())
            {
                sgc34_2.BeginFigure(new Point(0, 0), false, true);
                sgc34_2.BezierTo(new Point(1, 10), new Point(1, 1), new Point(-23.12, 10.22), true, true);
            }
            PathGeometry pg342_SG = PathGeometry.CreateFromGeometry(sg34_2);

            RectangleGeometry rg342 = new RectangleGeometry();
            GeometryGroup gg342 = new GeometryGroup();
            gg342.Children.Add(rg342);
            gg342.Children.Add(new LineGeometry(new Point(10, 10), new Point(-100, 100)));
            PathGeometry pg342_Empty = PathGeometry.CreateFromGeometry(gg342);

            bool result342 = pg342_Good != null;
            result342 &= pg342_null == null;
            result342 &= pg342_SG != null && pg342_SG.Transform is RotateTransform && pg342_SG.FillRule == FillRule.Nonzero;
            result342 &= pg342_Empty != null;

            CommonLib.GenericVerifier(result342, "CreateFromGeometry() method");
            #endregion

            #endregion

            #region Section IV: base class Geometry

            #region Test #35: Geometry.Combine(Geometry1, Geometry2, GeometryCombineMode, Transform, tolerance, ToleranceType)
            CommonLib.LogStatus("Test #35: Geometry.Combine(Geometry1, Geometry2, GeometryCombineMode, Transform, tolerance, ToleranceType)");
            RectangleGeometry rg35 = new RectangleGeometry(new Rect(0, 0, 200, 200));
            EllipseGeometry eg35 = new EllipseGeometry(new Rect(100, 100, 50, 50));
            PathGeometry pg35 = Geometry.Combine(rg35, eg35, GeometryCombineMode.Intersect, new RotateTransform(45), 0, ToleranceType.Absolute);

            CombinedGeometry cg35 = new CombinedGeometry();
            cg35.Geometry1 = rg35;
            cg35.Geometry2 = eg35;
            cg35.GeometryCombineMode = GeometryCombineMode.Intersect;
            cg35.Transform = new RotateTransform(45);

            //The idea of this test is that Geometry.Combine(...) and CombinedGeometry have the same Geometries, GeometryCombineMode and transform
            //They should produce the same geometry.  
            //The way to verify if those returned geometries are the same is by comparing the values of GetArea().
            //They should be within 1% of each other. 
            double areaPG = pg35.GetArea();
            double areaCG = cg35.GetArea();
            CommonLib.LogStatus("Test Disabled by Microsoft - see devdiv Regression_Bug25");
            CommonLib.GenericVerifier(Math.Abs(areaPG - areaCG)/areaPG < 0.01, "Geometry.Combine()");            
			CommonLib.LogStatus("areaPG:"+areaPG+" areaCG:" +areaCG);
            #endregion
#if !WPF35

            #region Test #35a: Geometry.Combine - verify Regression_Bug18
            CommonLib.LogStatus("Test #35a Geometry.Combine - verify Regression_Bug18");
            int len35aPost = 0;
            int len35aPostIdeal = 59;//from known good run
            EllipseGeometry eg35a1 = new EllipseGeometry(new Rect(100, 100, 50, 50));
            EllipseGeometry eg35a2 = new EllipseGeometry(new Rect(110, 110, 50, 50));

            PathGeometry pg35a = Geometry.Combine(eg35a1, eg35a2, GeometryCombineMode.Intersect, null);
            len35aPost = GetPathLength(pg35a.ToString());
            
            //if the path length has gone up by more than 25%, we've regressed
            bool result = false;
            result = (len35aPost < (len35aPostIdeal * 1.25f));
            CommonLib.GenericVerifier(result, "Geometry.Combine - verify Regression_Bug18");
            if (false == result)
            {
                CommonLib.Log.LogEvidence("Test #35a Geometry.Combine expected geometry length of " + (len35aPostIdeal * 1.25f).ToString() + " or less, actual result was " + len35aPost.ToString());
            }

            //closer ellipses
            //verifying Regression_Bug18
            CommonLib.LogStatus("Test #35b Geometry.Combine - verify Regression_Bug18");
            int len35bPost = 0;
            int len35bPostIdeal = 73;//from known good run
            EllipseGeometry eg35b1 = new EllipseGeometry(new Rect(100, 100, 50, 50));
            EllipseGeometry eg35b2 = new EllipseGeometry(new Rect(101, 101, 50, 50));

            PathGeometry pg35b = Geometry.Combine(eg35b1, eg35b2, GeometryCombineMode.Intersect, null);
            len35bPost = GetPathLength(pg35b.ToString());

            //if the path length has gone up by more than 25%, we've regressed
            result = (len35bPost < (len35bPostIdeal * 1.25f));
            CommonLib.GenericVerifier(result, "Geometry.Combine - verify Regression_Bug18"); 
            if (false == result)
            {
                CommonLib.Log.LogEvidence("Test #35b Geometry.Combine expected geometry length of " + (len35bPostIdeal * 1.25f).ToString() + " or less, actual result was " + len35bPost.ToString());
            }

            // now an ellipse inside a rect, like the original bug
            CommonLib.LogStatus("Test #35c Geometry.Combine - verify Regression_Bug18");
            int len35cPost = 0;
            int len35cPostIdeal = 92;//from known good run
            RectangleGeometry rg35c1 = new RectangleGeometry(new Rect(100, 100, 100, 100));
            EllipseGeometry eg35c2 = new EllipseGeometry(new Rect(100, 100, 50, 50));

            PathGeometry pg35c = Geometry.Combine(rg35c1, eg35c2, GeometryCombineMode.Exclude, null);
            len35cPost = GetPathLength(pg35c.ToString());

            //if the path length has gone up by more than 25%, we've regressed
            result = (len35cPost < (len35cPostIdeal * 1.25f));            
            CommonLib.GenericVerifier(result, "Geometry.Combine - verify Regression_Bug18"); 
            if (false == result)
            {
                CommonLib.Log.LogEvidence("Test #35c Geometry.Combine expected geometry length of " + (len35cPostIdeal * 1.25f).ToString() + " or less, actual result was " + len35cPost.ToString());
            }
            #endregion

            #endif 

            #region Test #36: Geometry.GetArea()
            CommonLib.LogStatus("Test #36: Geometry.GetArea()");
            EllipseGeometry eg36 = new EllipseGeometry(new Point(23, 2), 100, 100);
            double area36 = ((Geometry)eg36).GetArea();
            //The area of an Ellipse = PI * Radius * Radius
            //As long as the difference is smaller than 1, then GetArea is accurate enough
            CommonLib.GenericVerifier(Math.Abs(area36 - Math.PI * Math.Pow(100.0, 2.0)) < 1, "Geometry.GetArea()");
            #endregion

            #region Test #37: Geometry.GetFlattenedPathGeometry()
            CommonLib.LogStatus("Test #37: Geometry.GetFlattenedPathGeometry()");
            EllipseGeometry eg37 = new EllipseGeometry(new Point(100, 100), 50, 50);
            PathGeometry pg37 = ((Geometry)eg37).GetFlattenedPathGeometry();

            PathGeometry pg37_EG = eg37.GetFlattenedPathGeometry();
            //The idea is that both PathGeometries from Geometry.GetFlattenPathGeometry() or from EllipseGeometry
            //should be the same, so are the GetArea() values.
            CommonLib.GenericVerifier(pg37_EG.GetArea() == pg37.GetArea(), "Geometry.GetFlattenedPathGeometry()");
            #endregion

            #region Test #38: Geometry.GetFlattenedPathGeometry(double tolerance,ToleranceType type)
            CommonLib.LogStatus("Test #38: Geometry.GetFlattenedPathGeometry(double tolerance,ToleranceType type)");
            EllipseGeometry eg38 = new EllipseGeometry();
            PathGeometry pg38 = ((Geometry)eg38).GetFlattenedPathGeometry(0.5, ToleranceType.Relative);

            PathGeometry pg38_Ellipse = eg38.GetFlattenedPathGeometry(0.5, ToleranceType.Relative);
            double area38 = pg38.GetArea();
            double area38_Ellipse = pg38_Ellipse.GetArea();

            CommonLib.GenericVerifier(area38 == area38_Ellipse, "Geometry.GetFlattenedPathGeometry(double, ToleranceType)");
            #endregion

            #region Test #39: Geometry.GetOutlinedPathGeometry(double, ToleranceType)
            CommonLib.LogStatus("Test #39: GetOutlinedPathGeometry(double, ToleranceType)");
            EllipseGeometry eg39 = new EllipseGeometry(new Point(100, 100), 50, 100);
            PathGeometry pg39 = ((Geometry)eg39).GetOutlinedPathGeometry(100, ToleranceType.Absolute);
            PathGeometry pg39_Ellipse = eg39.GetOutlinedPathGeometry(100, ToleranceType.Absolute);
            CommonLib.GenericVerifier(pg39.GetArea() == pg39_Ellipse.GetArea(), "Geometry.GetOutlinedPathGeometry(double, ToleranceType)");
            #endregion

            #region Test #40: GetRenderBounds(Pen, double, ToleranceType)
            CommonLib.LogStatus("Test #40: GetRenderBounds(Pen, double, ToleranceType)");
            EllipseGeometry eg40 = new EllipseGeometry(new Point(100, 100), 80, 80);
            Rect bound40 = ((Geometry)eg40).GetRenderBounds(new Pen(Brushes.Black, 5.9), 100, ToleranceType.Relative);
            Rect bound40_Ellipse = eg40.GetRenderBounds(new Pen(Brushes.Black, 5.9), 100, ToleranceType.Relative);
            CommonLib.RectVerifier(bound40, bound40_Ellipse);
            #endregion

            #region Test #41: Geometry.ToString()
            CommonLib.LogStatus("Test #41: Geometry.ToString()");
            PathGeometry pg41 = new PathGeometry();
            PathFigure figure41 = new PathFigure();
            figure41.Segments.Add(new LineSegment(new Point(100, 100), false));
            pg41.Figures.Add(figure41);
            string result41 = pg41.ToString(System.Globalization.CultureInfo.InvariantCulture);
            CommonLib.GenericVerifier(string.Compare(result41, "M0,0L100,100", false, System.Globalization.CultureInfo.InvariantCulture) == 0,
                "Geometry.ToString()");
            #endregion

            #region Test #42:  Geometry.Parse()
            CommonLib.LogStatus(" Test #42: Geometry.Parse()");
            string goodString = "F1 M100,23 L103,3 Z";
            string badString = "XXXXXXXX";
            string emptyString = "";

            Geometry gGood = Geometry.Parse(goodString);
            PathGeometry gPG = PathGeometry.CreateFromGeometry(gGood);
            Geometry gEmpty = Geometry.Parse(emptyString);

            try
            {
                Geometry gBad = Geometry.Parse(badString);
                //If it reaches the below line, that means no exception is thrown.  Wrong!
                CommonLib.GenericVerifier(false, "Geometry.Parse(badstring)");
            }
            catch (System.FormatException)
            {
                CommonLib.LogStatus("System.FormatException is thrown when parsing the bad string.  It is correct");
            }

            CommonLib.GenericVerifier(gPG != null && gPG.Figures.Count == 1 && gPG.Figures[0].Segments.Count > 0
                                        && gEmpty != null && gEmpty.Bounds == Rect.Empty,
                                        "Geometry.Parse()"
                                     );

            #endregion

            #region Test #43:  Geometry.Empty
            CommonLib.LogStatus("Test #43:  Geometry.Empty");
            Geometry emptyG = Geometry.Empty;
            bool result43 = true;
            // HitTest GeometryEmpty
            result43 &= !emptyG.FillContains(new LineGeometry(new Point(10, 10), new Point(10, 10)));
            result43 &= !emptyG.FillContains(new Point(0, 0));
            result43 &= !emptyG.FillContains(new EllipseGeometry(), 1000, ToleranceType.Absolute);
            result43 &= !emptyG.FillContains(new PathGeometry(new PathFigure[] { new PathFigure() }), -1, ToleranceType.Relative);
            result43 &= !emptyG.FillContains(new Point(10, 10), 0, ToleranceType.Absolute);
            result43 &= !emptyG.FillContains(new Point(10, 10), double.PositiveInfinity, ToleranceType.Relative);
            result43 &= emptyG.FillContainsWithDetail(new RectangleGeometry(new Rect(0, 9, 0, 0))) == IntersectionDetail.Empty;
            result43 &= emptyG.FillContainsWithDetail(new LineGeometry(), 0, ToleranceType.Relative) == IntersectionDetail.Empty;
            result43 &= emptyG.FillContainsWithDetail(new LineGeometry(), 0, ToleranceType.Absolute) == IntersectionDetail.Empty;
            result43 &= !emptyG.StrokeContains(new Pen(Brushes.Black, double.MaxValue), new Point(0, 0));
            result43 &= !emptyG.StrokeContains(new Pen(Brushes.Black, double.MaxValue), new Point(0, 0), 0, ToleranceType.Absolute);
            result43 &= !emptyG.StrokeContains(new Pen(Brushes.Black, double.Epsilon), new Point(0, 0), 0, ToleranceType.Relative);

            // Get Bounds
            result43 &= emptyG.Bounds == Rect.Empty;
            result43 &= emptyG.GetRenderBounds(new Pen(Brushes.Black, 2)) == Rect.Empty;
            result43 &= emptyG.GetRenderBounds(new Pen(Brushes.Black, 2), 100, ToleranceType.Absolute) == Rect.Empty;

            // MayHaveCurves()
            result43 &= !emptyG.MayHaveCurves();

            // Get APIs
            result43 &= emptyG.GetArea() == 0;
            result43 &= emptyG.GetFlattenedPathGeometry().Figures.Count == 0;
            result43 &= emptyG.GetOutlinedPathGeometry().Figures.Count == 0;
            result43 &= emptyG.GetWidenedPathGeometry(new Pen(Brushes.Black, 100)).Figures.Count == 0;
            result43 &= emptyG.GetAsFrozen().IsFrozen == true;

            // IsEmpty()
            result43 &= emptyG.IsEmpty() == true;

            // Properties            
            result43 &= emptyG.IsFrozen == true;
            result43 &= emptyG.IsSealed == true;
            result43 &= emptyG.Transform == Transform.Identity;
            #endregion
            #endregion

            #region Circular Test for GetArea() and Bounds
            CommonLib.LogStatus("Circular Test for GetArea() and Bounds ");
            CommonLib.CircularTestGeometry(pg22);
            CommonLib.CircularTestGeometry(pg5);
            #endregion

            #region Section V : Running stage
            CommonLib.Stage= TestStage.Run;
            CommonLib.LogTest("Outcome for: "+ objectType );

            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), 1), pg2);
            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), 1), pg3);

            #endregion
        }
    }
}