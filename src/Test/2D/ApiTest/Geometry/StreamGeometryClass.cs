// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing StreamGeometry class
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
    /// Summary description for ElliseGeometryClass.
    /// </summary>
    internal class StreamGeometryClass : ApiTest
    {
        public StreamGeometryClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("StreamGeometry Class");
            String objectType = "System.Windows.Media.StreamGeometry";

            #region Section I: Constructors
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");
            StreamGeometry sg1 = new StreamGeometry();
            CommonLib.TypeVerifier(sg1, objectType);
            #endregion

            #region Test #2: StreamGeometryContext
            CommonLib.LogStatus("Test #2: StreamGeometryContext");
            StreamGeometry sg2 = new StreamGeometry();
            using (StreamGeometryContext sgc = sg2.Open())
            {
                sgc.BeginFigure(new Point(0, 0), false, true);
            }
            CommonLib.TypeVerifier(sg2, objectType);
            #endregion
            #endregion

            #region Section II: public methods
            #region Test #3: MayHaveCurves()
            CommonLib.LogStatus("Test #3: MayHaveCurves()");
            StreamGeometry sg3_empty = new StreamGeometry();

            EllipseGeometry eg3_NoSize = new EllipseGeometry(new Point(0, 0), 0, 0);
            StreamGeometry sg3_EG_NoSize = CreateStreamGeometry(eg3_NoSize);

            EllipseGeometry eg3_Normal = new EllipseGeometry(new Point(-0.23, 0), 10, 10);
            StreamGeometry sg3_EG_Normal = CreateStreamGeometry(eg3_Normal);

            RectangleGeometry rg3 = new RectangleGeometry(new Rect(0, 0, 10, 10), 0, 0);
            StreamGeometry sg3_RG = CreateStreamGeometry(rg3);

            StreamGeometry sg3_Context = new StreamGeometry();
            using (StreamGeometryContext sgc = sg3_Context.Open())
            {
                sgc.BeginFigure(new Point(0, 100.32), true, true);
                sgc.ArcTo(new Point(2, -120), new Size(100, 230), 45, true, SweepDirection.Counterclockwise, true, false);
            }

            CommonLib.GenericVerifier(
                            sg3_empty.MayHaveCurves() == false &&
                            sg3_EG_Normal.MayHaveCurves() == true &&
                            sg3_EG_NoSize.MayHaveCurves() == true &&
                            sg3_RG.MayHaveCurves() == false &&
                            sg3_Context.MayHaveCurves() == true,
                            "MayHaveCurves()"
                            );
            #endregion

            #region Test #4: Clone() method
            CommonLib.LogStatus("Test #4: Clone() method");
            try
            {
                StreamGeometry sg4_empty = sg3_empty.Clone();
                StreamGeometry sg4_EG_NoSize = sg3_EG_NoSize.Clone();
                StreamGeometry sg4_EG_Normal = sg3_EG_Normal.Clone();
                StreamGeometry sg4_RG = sg3_RG.Clone();
                StreamGeometry sg4_Context = sg3_Context.Clone();
                CommonLib.GenericVerifier(true, "Clone() method");
            }
            catch (Exception e)
            {
                CommonLib.GenericVerifier(false, "Clone()");
                CommonLib.LogStatus(e.InnerException.ToString());
            }

            #endregion

            #region Test #5:  GetRenderBounds(Pen) method
            CommonLib.LogStatus("Test #5: GetRenderBounds (Pen) method");
            Rect bound5_Empty = sg3_empty.GetRenderBounds(new Pen(Brushes.Black, 5));
            Rect bound5_EG_NoSize_NullPen = sg3_EG_NoSize.GetRenderBounds(null);
            Rect bound5_EG_NoSize2 = sg3_EG_NoSize.GetRenderBounds(new Pen(Brushes.Black, 5));
            Rect bound5_RG = sg3_RG.GetRenderBounds(new Pen(Brushes.Red, 4.23));
            Rect bound5_Context = sg3_Context.GetRenderBounds(new Pen(Brushes.Red, 0));
            Rect bound5_Context2 = sg3_Context.GetRenderBounds(new Pen(Brushes.Red, 5.00));

            CommonLib.RectVerifier(bound5_Empty, Rect.Empty);
            CommonLib.RectVerifier(bound5_EG_NoSize_NullPen, new Rect(0, 0, 0, 0));
            CommonLib.RectVerifier(bound5_EG_NoSize2, new Rect(-2.5, -2.5, 5, 5));
            CommonLib.RectVerifier(bound5_RG, new Rect(-2.12, -2.12, 14.23, 14.23));
            CommonLib.RectVerifier(bound5_Context, new Rect(0, -251.57, 273.41, 351.89));
            CommonLib.RectVerifier(bound5_Context2, new Rect(-2.53, -254.02, 278.44, 357.43));
            #endregion

            #region Test #6:  GetRenderBounds(Pen, Double, ToleranceType) method
            CommonLib.LogStatus("Test #6: GetRenderBounds (Pen, Double, ToleranceType) method");
            Rect bound6_Empty = sg3_empty.GetRenderBounds(new Pen(Brushes.Black, 0), 100, ToleranceType.Absolute);
            Rect bound6_EG_Normal = sg3_EG_Normal.GetRenderBounds(new Pen(Brushes.Red, 10), -10, ToleranceType.Relative);
            Rect bound6_EG_NoSize = sg3_EG_NoSize.GetRenderBounds(new Pen(Brushes.Blue, 5.0), -100.323, ToleranceType.Absolute);

            Rect bound6_RG = sg3_RG.GetRenderBounds(new Pen(Brushes.Red, Double.Epsilon));
            Rect bound6_Context = sg3_Context.GetRenderBounds(new Pen(Brushes.Red, Double.PositiveInfinity), 0.222, ToleranceType.Relative);

            CommonLib.RectVerifier(bound6_Empty, Rect.Empty);
            CommonLib.RectVerifier(bound6_EG_Normal, new Rect(-15.23, -15, 30, 30));
            CommonLib.RectVerifier(bound6_EG_NoSize, new Rect(-2.5, -2.5, 5, 5));
            CommonLib.RectVerifier(bound6_RG, new Rect(0, 0, 10, 10));
            CommonLib.RectVerifier(bound6_Context, Rect.Empty);
            #endregion

            #region Test #7: CloneCurrentValue() method
            CommonLib.LogStatus("Test #7: CloneCurrentValue() method");
            EllipseGeometry eg7 = new EllipseGeometry(new Point(100, 2), 120.23, 28.232);
            StreamGeometry sg7 = CreateStreamGeometry(eg7);
            StreamGeometry result7 = sg7.CloneCurrentValue();

            CommonLib.RectVerifier(sg7.Bounds, result7.Bounds);
            #endregion

            #region Test #8 - GetArea(double Tolerance) method
            CommonLib.LogStatus("Test #8 - GetArea(double Tolerance) method");
            double result8Empty = sg3_empty.GetArea(23.23, ToleranceType.Absolute);
            double result8_EG_Normal = sg3_EG_Normal.GetArea(100, ToleranceType.Relative);
            double result8_EG_NoSize = sg3_EG_NoSize.GetArea(-100, ToleranceType.Absolute);
            double result8_RG = sg3_RG.GetArea(0, ToleranceType.Absolute);
            double result8_Context = sg3_Context.GetArea(-123, ToleranceType.Relative);
            CommonLib.GenericVerifier(
                result8Empty == 0 &&
                result8_EG_Normal == 200.0 &&
                result8_EG_NoSize == 0.0 &&
                result8_RG == 100.0 &&
                Math.Round(result8_Context, 1) == 59558.6,
                "GetArea(double Tolerance) method");
            #endregion

            #region Test #9 - GetArea() method
            CommonLib.LogStatus("Test #9 - GetArea() method");
            double result9Empty = sg3_empty.GetArea();
            double result9_EG_Normal = sg3_EG_Normal.GetArea();
            double result9_EG_NoSize = sg3_EG_NoSize.GetArea();
            double result9_RG = sg3_RG.GetArea();
            double result9_Context = sg3_Context.GetArea();
            CommonLib.GenericVerifier(
                        result9Empty == 0 &&
                        Math.Round(result9_EG_Normal, 2) == 309.39 &&
                        result9_EG_NoSize == 0.0 &&
                        result9_RG == 100.0 &&
                        Math.Round(result9_Context, 2) == 59494.79,
                        "GetArea() method");
            #endregion

            #region Test #10 GetOutlinedPathGeometry()
            CommonLib.LogStatus("Test #10 GetOutlinedPathGeometry()");
            PathGeometry pg10_Empty = sg3_empty.GetOutlinedPathGeometry();
            PathGeometry pg10_EG_Normal = sg3_EG_Normal.GetOutlinedPathGeometry();
            PathGeometry pg10_EG_NoSize = sg3_EG_NoSize.GetOutlinedPathGeometry();
            PathGeometry pg10_RG = sg3_RG.GetOutlinedPathGeometry();
            PathGeometry pg10_Context = sg3_Context.GetOutlinedPathGeometry();
            CommonLib.GenericVerifier(
                    pg10_Empty.Bounds == sg3_empty.Bounds &&
                    CommonLib.HitTest(pg10_EG_Normal, 100) == CommonLib.HitTest(sg3_EG_Normal, 100) &&
                    pg10_EG_NoSize.Bounds == Rect.Empty &&
                    CommonLib.HitTest(pg10_RG, 100) == CommonLib.HitTest(sg3_RG, 100) &&
                    Math.Abs(CommonLib.HitTest(pg10_Context, 100) - CommonLib.HitTest(sg3_Context, 100)) < 10,
                    "GetOutlinedPathGeometry()"
                    );
            #endregion

            #region Test #11: GetFlattendPathGeometry()
            CommonLib.LogStatus("Test #11: GetFlattenedPathGeometry()");
            PathGeometry pg11_Empty = sg3_empty.GetFlattenedPathGeometry();
            PathGeometry pg11_EG_Normal = sg3_EG_Normal.GetFlattenedPathGeometry();
            PathGeometry pg11_EG_NoSize = sg3_EG_NoSize.GetFlattenedPathGeometry();
            PathGeometry pg11_RG = sg3_RG.GetFlattenedPathGeometry();
            PathGeometry pg11_Context = sg3_Context.GetFlattenedPathGeometry();
            CommonLib.GenericVerifier(
                    pg11_Empty.Bounds == sg3_empty.Bounds &&
                    CommonLib.HitTest(pg11_EG_Normal, 100) == CommonLib.HitTest(sg3_EG_Normal, 100) &&
                    CommonLib.HitTest(pg11_EG_NoSize, 100) == CommonLib.HitTest(sg3_EG_NoSize, 100) &&
                    CommonLib.HitTest(pg11_RG, 100) == CommonLib.HitTest(sg3_RG, 100) &&
                    Math.Abs(CommonLib.HitTest(pg11_Context, 100) - CommonLib.HitTest(sg3_Context, 100)) < 10,
                    "GetFlattenedPathGeometry()"
                    );
            #endregion

            #region Test #12: GetWidenedPathGeometry()
            CommonLib.LogStatus("Test #11: GetWidenedPathGeometry()");
            Pen pen12 = new Pen(Brushes.Black, 5);
            PathGeometry pg12_Empty = sg3_empty.GetWidenedPathGeometry(pen12);
            PathGeometry pg12_EG_Normal = sg3_EG_Normal.GetWidenedPathGeometry(pen12);
            PathGeometry pg12_EG_NoSize = sg3_EG_NoSize.GetWidenedPathGeometry(pen12);
            PathGeometry pg12_RG = sg3_RG.GetWidenedPathGeometry(pen12);
            PathGeometry pg12_Context = sg3_Context.GetWidenedPathGeometry(pen12);
            CommonLib.GenericVerifier(
                    pg12_Empty.Bounds == sg3_empty.GetRenderBounds(pen12) &&
                    pg12_EG_Normal.Bounds == sg3_EG_Normal.GetRenderBounds(pen12) &&
                    pg12_EG_NoSize.Bounds == sg3_EG_NoSize.GetRenderBounds(pen12) &&
                    pg12_RG.Bounds == sg3_RG.GetRenderBounds(pen12) &&
                    pg12_Context.Bounds == sg3_Context.GetRenderBounds(pen12),
                    "GetWidenedPathGeometry()"
                    );
            #endregion

            #region Test #13:  IsEmpty()
            CommonLib.LogStatus("Test #13:  IsEmpty()");
            bool bool13_empty = sg3_empty.IsEmpty();

            StreamGeometry sg13_EmptyContext = new StreamGeometry();
            using (StreamGeometryContext sgc = sg13_EmptyContext.Open())
            {
            }
            bool bool13_EmptyContext = sg13_EmptyContext.IsEmpty();

            CombinedGeometry cg13 = new CombinedGeometry();
            cg13.GeometryCombineMode = GeometryCombineMode.Exclude;
            cg13.Geometry1 = sg3_empty;
            cg13.Geometry2 = sg3_empty;
            StreamGeometry sg13_EmptyCG = CreateStreamGeometry(cg13);
            bool bool13_EmptyCG = sg13_EmptyCG.IsEmpty();

            bool bool13_EG_Normal = sg3_EG_Normal.IsEmpty();
            bool bool13_Context = sg3_Context.IsEmpty();

            CommonLib.GenericVerifier(
                        bool13_empty == true &&
                        bool13_EmptyContext == true &&
                        bool13_EmptyCG == true &&
                        bool13_EG_Normal == false &&
                        bool13_Context == false,
                        "IsEmpty()"
                        );
            #endregion

            #region Test #13.5: PathGeometry.CreateFromGeometry()
            CommonLib.LogStatus("Test #13.5: PathGeometry.CreateFromGeometry() )");
            StreamGeometry sg13_5 = new StreamGeometry();
            sg13_5.FillRule = FillRule.Nonzero;
            sg13_5.Transform = new ScaleTransform(100, 100);
            PathGeometry pg13_5 = PathGeometry.CreateFromGeometry(sg13_5);

            CommonLib.GenericVerifier(
                pg13_5 != null &&
                pg13_5.FillRule == FillRule.Nonzero &&
                pg13_5.Transform is ScaleTransform &&
                ((ScaleTransform)pg13_5.Transform).ScaleX == 100,
                "PathGeometry.CreateFromGeometry() "
            );
            #endregion

            #region Test #13.6: Clear()
            CommonLib.LogStatus("Test #13.6: Clear()");
            StreamGeometry sg136_Empty = new StreamGeometry();

            EllipseGeometry eg136 = new EllipseGeometry(new Point(100, 100), 100, 100);
            StreamGeometry sg136_Normal = CreateStreamGeometry(eg136);

            StreamGeometry sg136_NoClose = new StreamGeometry();
            StreamGeometryContext sgc136 = sg136_NoClose.Open();
            sgc136.BeginFigure(new Point(0, 0), true, true);
            sgc136.LineTo(new Point(10, 10), false, true);

            sg136_Empty.Clear();
            sg136_Normal.Clear();
            sg136_NoClose.Clear();

            bool result136 = sg136_Empty.Bounds == Rect.Empty;
            result136 &= sg136_Normal.Bounds == Rect.Empty;
            result136 &= sg136_NoClose.Bounds == Rect.Empty;

            CommonLib.GenericVerifier(result136, "Clear()");
            #endregion

            #region Test #13.7: StreamGeometryContext.Close()
            CommonLib.LogStatus("Test #13.7: StreamGeometryContext.Close()");
            StreamGeometry sg137 = new StreamGeometry();
            StreamGeometryContext sgc137 = sg137.Open();
            sgc137.BeginFigure(new Point(0, 10), true, true);
            sgc137.PolyLineTo(new Point[] { new Point(0, 100), new Point(100, 100) }, true, true);
            // The StreamGeometryContext is not closed yet, so there is actually no content in the StreamGeometry
            // Expecting the Bounds of the StreamGeometry to be Empty
            Rect innerBound = sg137.Bounds;
            sgc137.Close();
            // The context is closed, the stream context is written
            // so the Bounds won't be Empty in this case.
            Rect fullBound = sg137.Bounds;
            bool result137 = innerBound == Rect.Empty;
            result137 &= fullBound == new Rect(0, 10, 100, 90);
            CommonLib.GenericVerifier(result137, "StreamGeometryContext.Close()");
            #endregion
            #endregion

            #region Section III: public properties
            #region Test #14: Bounds property
            CommonLib.LogStatus("Test #14: Bounds property");
            Rect bound14_Empty = sg3_empty.Bounds;
            Rect bound14_EG_Normal = sg3_EG_Normal.Bounds;
            Rect bound14_EG_NoSize = sg3_EG_NoSize.Bounds;
            Rect bound14_RG = sg3_RG.Bounds;
            Rect bound14_Context = sg3_Context.Bounds;

            CommonLib.RectVerifier(bound14_Empty, Rect.Empty);
            CommonLib.RectVerifier(bound14_EG_Normal, new Rect(-10.2299995422363, -10, 20, 20));
            CommonLib.RectVerifier(bound14_EG_NoSize, new Rect(0, 0, 0, 0));
            CommonLib.RectVerifier(bound14_RG, new Rect(0, 0, 10, 10));
            CommonLib.RectVerifier(bound14_Context, new Rect(0, -251.570373535156, 273.413787841797, 351.89037322998));
            #endregion

            #region Test #15: FillRule property
            CommonLib.LogStatus("Test #15: FillRule property");
            StreamGeometry sg15 = new StreamGeometry();
            //Default value of the FillRule property
            FillRule defaultFillRule = sg15.FillRule;

            //FillRule property in Invalid state
            sg15.InvalidateProperty(StreamGeometry.FillRuleProperty);
            FillRule invalidateFillRule = sg15.FillRule;

            //Setting the FillRule property
            sg15.FillRule = FillRule.Nonzero;

            CommonLib.GenericVerifier(
                    defaultFillRule == FillRule.EvenOdd &&
                    invalidateFillRule == FillRule.EvenOdd &&
                    sg15.FillRule == FillRule.Nonzero,
                    "FillRule property"
                    );
            #endregion

            #endregion

            #region Circular testing for GetArea() and Bounds
            //CommonLib.CircularTestGeometry(sg3_empty);
            CommonLib.CircularTestGeometry(sg3_EG_NoSize);
            CommonLib.CircularTestGeometry(sg3_RG);
            CommonLib.CircularTestGeometry(sg3_Context);
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;
            CommonLib.LogTest("Result for:" + objectType);

            DC.DrawGeometry(Brushes.Black, new Pen(new SolidColorBrush(Colors.Blue), 2), sg3_empty);
            DC.DrawGeometry(Brushes.Black, new Pen(new SolidColorBrush(Colors.Blue), 2), sg3_EG_Normal);
            DC.DrawGeometry(Brushes.Black, new Pen(new SolidColorBrush(Colors.Yellow), 2), sg3_RG);
            DC.DrawGeometry(Brushes.Black, new Pen(new SolidColorBrush(Colors.Yellow), 2), sg3_Context);
            DC.DrawGeometry(Brushes.Black, new Pen(Brushes.Red, 10), sg136_NoClose);
            #endregion
        }

        private StreamGeometry CreateStreamGeometry(Geometry g)
        {
            // It is the only way to generate an empty StreamGeometry in our model.
            if (g == null)
            {
                return new StreamGeometry();
            }

            //First covert any Geometry object into PathGeometry
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
