// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing PathFigure class
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for CommonLib.log.
    /// </summary>
    internal class PathFigureClass : ApiTest
    {
        public PathFigureClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        /// <summary>
        /// Convert numerical geometry path string from harcoded invariantculture form to 
        /// localized numerical form.
        /// </summary>
        private string LocalizeNumber(string inString )
        {
            string outString = inString;
            outString=outString.Replace("-Infinity", CultureInfo.CurrentCulture.NumberFormat.NegativeInfinitySymbol);
            outString=outString.Replace("Infinity",CultureInfo.CurrentCulture.NumberFormat.PositiveInfinitySymbol);
            return outString;
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("PathFigure Class");

            string objectType = "System.Windows.Media.PathFigure";

            #region Section I: Initialization stage
            CommonLib.Stage = TestStage.Initialize;

            #region Test #1: Default constructor
            CommonLib.LogStatus("Test #1: Default constructor");

            PathFigure pf1 = new PathFigure();
            CommonLib.TypeVerifier(pf1, objectType);
            #endregion

            #region Test #2: PathFigure(IEnumerable<PathSegment> segments, bool isFillable)
            CommonLib.LogStatus("Test #2: PathFigure(IEnumerable<PathSegment> segments, bool isFillable)");
            PathSegment[] segmentList = new PathSegment[] {
                        new ArcSegment (new Point (50, 30), new Size (10, 23), 20, false, SweepDirection.Counterclockwise, false)
                        };

            PathFigure pf2 = new PathFigure(new Point(10, 10), segmentList, false);
            CommonLib.TypeVerifier(pf2, objectType);
            #endregion

            #region Test #3: PathFigure(IEnumerable<PathSegment> segments)
            CommonLib.LogStatus("Test #3: PathFigure(IEnumerable<PathSegment> segments)");
            PathFigure pf3 = new PathFigure(new Point(10, 10), segmentList, false);
            CommonLib.TypeVerifier(pf3, objectType);
            #endregion

            #region Test #4: PathFigure() constructor - common way to add segment
            CommonLib.LogStatus("Test #4: PathFigure() constructor - common way to add segment");
            PathFigure pf4 = new PathFigure();
            pf4.StartPoint = new Point(10, 10);
            pf4.Segments.Add(new LineSegment(new Point(30, 20), true));
            pf4.Segments.Add(new BezierSegment(new Point(100, 20), new Point(60, 60), new Point(90, 100), true));
            CommonLib.TypeVerifier(pf4, objectType);
            #endregion

            #endregion

            #region Section II - public properties
            #region Test #5: CanFreeze
            CommonLib.LogStatus("Test #5: CanFreeze");
            CommonLib.GenericVerifier(pf1.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #7a: IsFilled property with basevalue
            CommonLib.LogStatus("Test #7a:  IsFilled property with basevalue");
            PathFigure pf7a = new PathFigure();
            CommonLib.GenericVerifier(pf7a.IsFilled, "IsFilled with basevalue");
            #endregion

            #region Test #7b: IsFilled property in invalid stage
            CommonLib.LogStatus("Test #7b: IsFilled property in invalid stage");
            PathFigure pf7b = new PathFigure();
            pf7b.InvalidateProperty(PathFigure.IsFilledProperty);
            CommonLib.GenericVerifier(pf7b.IsFilled, "IsFilled in invalid stage");
            #endregion

            #region Test #7: IsFilled property
            CommonLib.LogStatus("Test #7: IsFilled property");
            pf4.IsFilled = true;
            CommonLib.GenericVerifier(pf4.IsFilled, "IsFilled property");
            #endregion

            #region Test #7.2: IsFilledProperty DP
            CommonLib.LogStatus("Test #7.2: IsFilledProperty DP");
            PathFigure pf72 = new PathFigure();
            pf72.SetValue(PathFigure.IsFilledProperty, true);
            bool result72 = (bool)pf72.GetValue(PathFigure.IsFilledProperty);
            CommonLib.GenericVerifier(result72 == true, "IsFilledProperty DP");
            #endregion

            #region Test #8.4:  Segments property
            CommonLib.LogStatus("Test #8.4: Segments property");
            int result84 = pf2.Segments.Count;
            CommonLib.GenericVerifier(result84 == 1, "Segments property");
            #endregion

            #region Test #8.4a:  SegmentsProperty DP
            CommonLib.LogStatus("Test #8.4a: SegmentsProperty DP");
            PathFigure pf84a = new PathFigure();
            PathSegmentCollection psc84a = new PathSegmentCollection();
            foreach (PathSegment ps in segmentList)
            {
                psc84a.Add(ps);
            }
            pf84a.SetValue(PathFigure.SegmentsProperty, psc84a);
            PathSegmentCollection resultPSC84a = pf84a.GetValue(PathFigure.SegmentsProperty) as PathSegmentCollection;
            CommonLib.GenericVerifier(resultPSC84a.Count == 1, "SegmentsProperty DP");
            #endregion

            #endregion

            #region Section III: public methods
            #region Test #9: LineSegment()
            CommonLib.LogStatus("Test #9 - LineSegment");
            pf1.StartPoint = new Point(0, 0);
            pf1.Segments.Add(new LineSegment(new Point(90, 10), true));
            CommonLib.GenericVerifier(pf1.Segments.Count == 1 && pf1.Segments[0] is LineSegment, "LineSegment");
            #endregion

            #region Test #10: ArcTo()
            CommonLib.LogStatus("Test #10: AcrTo()");
            pf4.Segments.Add(new ArcSegment(new Point(180, 100), new Size(20, 20), 50, true, SweepDirection.Counterclockwise, true));
            CommonLib.GenericVerifier(pf4.Segments.Count == 3 && pf4.Segments[2] is ArcSegment, "AcrTo() method");
            #endregion

            #region Test #11: BezierTo ( System.Windows.Point pt1 , System.Windows.Point pt2 , System.Windows.Point ptDest )
            CommonLib.LogStatus("Test #11: BezierTo");
            pf1.Segments.Add(new BezierSegment(new Point(30, 20), new Point(100, 20), new Point(20, 100), true));
            CommonLib.GenericVerifier(pf1.Segments.Count == 2 && pf1.Segments[1] is BezierSegment, "BezierTo");
            #endregion

            #region Test #12: Copy()
            CommonLib.LogStatus("Test #13: Copy()");
            PathFigure pf5 = pf4.Clone();
            CommonLib.GenericVerifier(pf5 != null && pf5.Segments.Count == pf4.Segments.Count, "Copy() method");
            #endregion

            #region Test #13: CloneCurrentValue()
            PathFigure pf6 = pf4.CloneCurrentValue();
            CommonLib.GenericVerifier(pf6 != null && pf6.Segments.Count == pf4.Segments.Count, "CloneCurrentValue() method");
            #endregion

            #region Test #14: GetFlattenedPathFigure()
            CommonLib.LogStatus("Test #14: GetFlattenedPathFigure()");
            PathFigure pf7 = new PathFigure();
            pf7.StartPoint = new Point(0, 0);
            pf7.Segments.Add(new BezierSegment(new Point(23, 12), new Point(-0.23, -23), new Point(210, 1), true));
            pf7.IsClosed = true;
            PathGeometry pg7 = new PathGeometry();
            pg7.Figures.Add(pf7);
            double result7 = pg7.GetArea();

            PathFigure pf7_Flattened = pf7.GetFlattenedPathFigure();
            PathGeometry pgFlattened = new PathGeometry();
            pgFlattened.Figures.Add(pf7_Flattened);
            double result7_Flattened = pgFlattened.GetArea();
            double difference = Math.Abs(result7 - result7_Flattened);
            //As long as the difference of a PathFigure and a flattened PathFigure 
            //is not bigger than 1, they are close enough.
            CommonLib.GenericVerifier(difference < 1, "GetFlattenedPathFigure()");
            #endregion

            #region Test #14.5:  GetFlattenedPathFigure( double tolerance, ToleranceType type )
            CommonLib.LogStatus("Test #14.5:  GetFlattenedPathFigure( double tolerance, ToleranceType type )");
            PathFigure pf145_empty = new PathFigure();
            PathFigure resultEmpty = pf145_empty.GetFlattenedPathFigure(100, ToleranceType.Absolute);
            bool result145 = resultEmpty.Segments.Count == 0;

            PathFigure pf145_Absolute = new PathFigure();
            pf145_Absolute.Segments.Add(new BezierSegment(new Point(0, 1), new Point(10, 30), new Point(-100, -100), true));
            pf145_Absolute.Segments.Add(new QuadraticBezierSegment(new Point(0, 2), new Point(10, 10), false));
            pf145_Absolute.IsClosed = true;
            PathFigure resultAbsolute = pf145_Absolute.GetFlattenedPathFigure(-1, ToleranceType.Absolute);
            PathGeometry pg145_Absolue = new PathGeometry();
            pg145_Absolue.Figures.Add(pf145_Absolute);
            PathGeometry pg_result145 = new PathGeometry();
            pg_result145.Figures.Add(resultAbsolute);
            result145 &= Math.Abs(pg145_Absolue.GetArea() - pg_result145.GetArea()) < 1.1;

            PathFigure pf145_Relative = new PathFigure();
            pf145_Relative.Segments.Add(new ArcSegment(new Point(10, 10), new Size(10, 10), 1, true, SweepDirection.Counterclockwise, true));
            pf145_Relative.IsClosed = true;
            PathFigure result145_Relative = pf145_Relative.GetFlattenedPathFigure(10, ToleranceType.Relative);
            PathGeometry pg145_relative = new PathGeometry(new PathFigure[] { pf145_Relative });
            PathGeometry pg_result145Relaive = new PathGeometry(new PathFigure[] { result145_Relative });
            result145 &= Math.Abs(pg145_relative.GetArea() - pg_result145Relaive.GetArea()) < 47.5;

            CommonLib.GenericVerifier(result145, "GetFlattenedPathFigure() method");

            #endregion

            #region Test #15:  LineTo()
            CommonLib.LogStatus("Test #16: LineTo");
            pf4.Segments.Add(new LineSegment(new Point(10, 200), true));
            CommonLib.GenericVerifier(pf4.Segments.Count == 4 && pf4.Segments[3] is LineSegment, "LineTo");
            #endregion

            #region Test #16: PolyBezierTo()
            CommonLib.LogStatus("Test #17: PolyBezierTo");
            pf4.Segments.Add(new PolyBezierSegment(new Point[] { new Point(20, 20), new Point(100, 10), new Point(0, 100) }, true));
            CommonLib.GenericVerifier(pf4.Segments.Count == 5 && pf4.Segments[4] is PolyBezierSegment, "PolyBezierTo");
            #endregion

            #region Test #17: PolyLineTo()
            CommonLib.LogStatus("Test #17: PolyLineto()");
            pf4.Segments.Add(new PolyLineSegment(new Point[] { new Point(39, 10), new Point(54, 102), new Point(29, 39) }, true));
            CommonLib.GenericVerifier(pf4.Segments.Count == 6 && pf4.Segments[5] is PolyLineSegment, "PolyLineto");
            #endregion

            #region Test #18: PolyQuadraticBezierTo()
            CommonLib.LogStatus("Test #18: PolyQuadraticBezierTo()");
            pf4.Segments.Add(new PolyQuadraticBezierSegment(new Point[] { new Point(100, 20), new Point(20, 80) }, true));
            CommonLib.GenericVerifier(pf4.Segments.Count == 7 && pf4.Segments[6] is PolyQuadraticBezierSegment, "PolyQuadraticBezierTo()");
            #endregion

            #region Test #19: StartAt Property
            CommonLib.LogStatus("Test #19: StartAt property");
            PathFigure pf8 = new PathFigure();
            pf8.StartPoint = new Point(20, 20);
            CommonLib.GenericVerifier(pf8.Segments.Count == 0, "StartPoint property");
            #endregion

            #region Test #20: IsClosed property
            CommonLib.LogStatus("Test #20: IsClosed property ");
            pf4.IsClosed = true;
            CommonLib.GenericVerifier(pf4.Segments.Count == 7, "IsClosed property");
            #endregion

            #region Test #21: MayHaveCurves()
            CommonLib.LogStatus("Test #21: MayHaveCurves()");
            PathFigure pf21_NoCurve = new PathFigure();
            pf21_NoCurve.Segments.Add(new LineSegment(new Point(23, 21), true));
            pf21_NoCurve.IsClosed = true;
            bool result21_NoCurve = pf21_NoCurve.MayHaveCurves();

            PathFigure pf21_Curve = new PathFigure();
            pf21_Curve.Segments.Add(new LineSegment(new Point(32, 12), false));
            pf21_Curve.Segments.Add(new BezierSegment(new Point(90, 23), new Point(0, 23), new Point(23, 12), true));
            bool result21_Curve = pf21_Curve.MayHaveCurves();

            CommonLib.GenericVerifier(result21_NoCurve == false && result21_Curve == true, "MayHaveCurves()");
            #endregion

            /* DISABLEDUNSTABLETEST Test# 22 Path Figure To String & Test# 22.5
            #region Test #22: ToString()
            CommonLib.LogStatus("Test #22: ToString()");
            // Empty PathFigure
            PathFigure pf22_Empty = new PathFigure();

            // PathFigure uses default startpoint
            PathFigure pf22_NoStartPoint = new PathFigure();
            pf22_NoStartPoint.Segments.Add(new ArcSegment(new Point(1000, 1), new Size(10, 10), 45, true, SweepDirection.Clockwise, true));
            pf22_NoStartPoint.IsClosed = true;

            // PathFigure is not closed
            PathFigure pf22_NotClosed = new PathFigure();
            pf22_NotClosed.StartPoint = new Point(-100, 100.23);
            pf22_NotClosed.Segments.Add(new LineSegment(new Point(-100.323, 0), false));
            pf22_NotClosed.IsClosed = false;

            // PathFigure contains all different segments
            PathFigure pf22_All = new PathFigure();
            pf22_All.Segments.Add(new LineSegment(new Point(-1, 100), true));
            pf22_All.Segments.Add(new ArcSegment(new Point(100, 100), new Size(100, 0), -10000, false, SweepDirection.Counterclockwise, false));
            pf22_All.Segments.Add(new BezierSegment(new Point(0, 0), new Point(100, Double.Epsilon), new Point(-1, Double.NegativeInfinity), true));
            pf22_All.Segments.Add(new PolyLineSegment(new Point[] { }, true));
            pf22_All.Segments.Add(new PolyBezierSegment(new Point[] { new Point(435.3, -123.23), new Point(0, 0), new Point(100, 10) }, true));
            pf22_All.Segments.Add(new PolyQuadraticBezierSegment(new Point[] { new Point(0, 0), new Point(-1, -1) }, false));
            pf22_All.Segments.Add(new QuadraticBezierSegment(new Point(0, 0), new Point(Double.NegativeInfinity, Double.PositiveInfinity), true));

            // PathFigure contains no segments
            PathFigure pf22_NoSegment = new PathFigure();
            pf22_NoSegment.StartPoint = new Point(-0, -1);
            pf22_NoSegment.IsClosed = true;

            string pf22localizedEmptyString = LocalizeNumber("M0,0");
            string pf22localizedNoStartPointString = LocalizeNumber("M0,0A10,10,45,1,1,1000,1z");
            string pf22localizedNotClosedString = LocalizeNumber("M-100,100.23L-100.323,0");
            string pf22localizedAllString = LocalizeNumber("M0,0L-1,100A100,0,-10000,0,0,100,100C0,0,100,4.94065645841247E-324,-1,-InfinityLC435.3,-123.23 0,0 100,10Q0,0 -1,-1Q0,0,-Infinity,Infinity");

            bool result22 = MathEx.EqualsIgnoringSeparators(pf22localizedEmptyString, pf22_Empty.ToString());
            result22 &= MathEx.EqualsIgnoringSeparators(pf22localizedNoStartPointString, pf22_NoStartPoint.ToString());
            result22 &= MathEx.EqualsIgnoringSeparators(pf22localizedNotClosedString, pf22_NotClosed.ToString());
            result22 &= MathEx.EqualsIgnoringSeparators(pf22localizedAllString,pf22_All.ToString());

            CommonLib.LogStatus(pf22localizedEmptyString + " : " + pf22_Empty.ToString() + " \n");
            CommonLib.LogStatus(pf22localizedNoStartPointString + " : " + pf22_NoStartPoint.ToString() + " \n");
            CommonLib.LogStatus(pf22localizedNotClosedString + " : " + pf22_NotClosed.ToString() + " \n");
            CommonLib.LogStatus(pf22localizedAllString + " : " + pf22_All.ToString() + " \n");

            CommonLib.GenericVerifier(result22, "ToString()");
        
            #endregion

            #region Test #22.5 - ToString(System.IFormatProvider provider)
            CommonLib.LogStatus("Test #22.5 - ToString(System.IFormatProvider provider)");
            bool result23 = MathEx.EqualsIgnoringSeparators("M0,0", pf22_Empty.ToString(CultureInfo.InvariantCulture));
            result23 &= MathEx.EqualsIgnoringSeparators("M0,0A10,10,45,1,1,1000,1z", pf22_NoStartPoint.ToString(CultureInfo.InvariantCulture));
            result23 &= MathEx.EqualsIgnoringSeparators("M-100,100.23L-100.323,0", pf22_NotClosed.ToString(CultureInfo.InvariantCulture));
            result23 &= MathEx.EqualsIgnoringSeparators(
                "M0,0L-1,100A100,0,-10000,0,0,100,100C0,0,100,4.94065645841247E-324,-1,-InfinityLC435.3,-123.23 0,0 100,10Q0,0 -1,-1Q0,0,-Infinity,Infinity",
                pf22_All.ToString(CultureInfo.InvariantCulture)
                );
            CommonLib.GenericVerifier(result23, "ToString(IFormatProvider)");
            #endregion
            */
            #endregion

            #region Test #23 - Regression for Regression_Bug7
            CommonLib.LogStatus("Test #23 - Regression for Regression_Bug7");
            try
            {
                PathFigure pf23 = pf4.GetFlattenedPathFigure(5.0, ToleranceType.Absolute);
                CommonLib.LogStatus("No exception is thrown, Regression_Bug7 is still fixed.");
            }
            catch (System.NotImplementedException)
            {
                CommonLib.GenericVerifier(false, "Incorrect System.NotImplementedExcetionException is thrown!");
            }

            #region Test #24 - Regression for Regression_Bug8
            CommonLib.LogStatus("Test #24 - Regression for Regression_Bug8");
            PathGeometry pg24 = new PathGeometry();
            PathFigure pf24 = new PathFigure();
            pf24.StartPoint = new Point(2, 100);
            pf24.Segments.Add(new LineSegment(new Point(2, 100), true));
            pf24.Segments.Add(new PolyLineSegment(new Point[] { new Point(39, 10), new Point(54, 102), new Point(29, 39) }, true));
            pg24.Figures.Add(pf24);
            CommonLib.LogStatus("Test #24 - if there is no Assertion and no exception during rendering");
            CommonLib.LogStatus("\t, then Regression_Bug8 remains fixed");
            #endregion

            #region Test #25 - Regression for Regression_Bug9
            CommonLib.LogStatus("Test #25 - Regression for Regression_Bug9");
            PathFigure pf25 = new PathFigure();
            pf25.StartPoint = new Point(-119292, 415092);
            pf25.Segments.Add(new ArcSegment(new Point(180, 100), new Size(20, 20), 50, true, SweepDirection.Counterclockwise, true));
            PathFigure pf25_temp = pf25.GetFlattenedPathFigure(5.0, ToleranceType.Absolute);
            CommonLib.LogStatus("No exception, no assertion, Regression_Bug237 remains fixed");
            #endregion
            #endregion

            #region PathFigureEnumerator section
            #region #26 - Create PathFigureEnumerator out of a PathFigure
            CommonLib.LogStatus("Test #26 - Create PathFigureEnumerator out of a PathFigure");
            PathFigureCollection pfc26 = new PathFigureCollection();
            PathFigure pf26_1 = new PathFigure();
            pf26_1.StartPoint = new Point(3, 2);

            PathFigure pf26_2 = new PathFigure();
            pf26_2.StartPoint = new Point(0, 2);
            pf26_2.Segments.Add(new LineSegment(new Point(32, 33), true));

            PathFigure pf26_3 = new PathFigure();
            pf26_3.StartPoint = new Point(-1, 2);
            pf26_3.Segments.Add(new ArcSegment(new Point(32, 2), new Size(100, 32), 45, true, SweepDirection.Counterclockwise, true));

            pfc26.Add(pf26_1);
            pfc26.Add(pf26_2);
            pfc26.Add(pf26_3);

            IEnumerator pfe = ((IEnumerable)pfc26).GetEnumerator();
            if (pfe == null)
            {
                throw new System.ApplicationException("Error:  fail to create PathFigureEnumerator");
            }

            PathFigureCollection pfc_empty = new PathFigureCollection();
            IEnumerator pfe_empty = ((IEnumerable)pfc_empty).GetEnumerator();
            #endregion

            #region Test #27 - MoveNext method
            CommonLib.LogStatus("Test #27 - MoveNext method");
            pfe.MoveNext();
            pfe.MoveNext();
            object currentObject = pfe.Current;
            CommonLib.GenericVerifier(currentObject is PathFigure && ((PathFigure)currentObject).Segments[0] is LineSegment, "MoveNext() method");
            #endregion

            #region Test #28 - Reset() method
            CommonLib.LogStatus("Test #28 - Reset() method");
            pfe.Reset();
            pfe.MoveNext();
            object co28 = pfe.Current;
            CommonLib.GenericVerifier(co28 is PathFigure && ((PathFigure)co28).Segments.Count == 0, "Reset() method");
            #endregion

            #region Test #29 - MoveNext method in an empty PathFigureEnumerator
            CommonLib.LogStatus("Test #29 - MoveNext method in a empty PathFigureEnumerator");
            bool result29 = pfe_empty.MoveNext();

            CommonLib.GenericVerifier(result29 == false, "MoveNext() in empty PathFigureEnumerator");
            #endregion

            #region Test #30 - Current property in an empty PathFigureEnumerator
            CommonLib.LogStatus("Test #30 - Current property in an empty PathFigureEnumerator");
            try
            {
                object result30 = pfe_empty.Current;
                CommonLib.GenericVerifier(false, "Current property in an empty PathFigureEnumerator");
            }
            catch (System.InvalidOperationException)
            {
                CommonLib.GenericVerifier(true, "Current property in an empty PathFigureEnumerator");
            }
            catch (System.Exception)
            {
                CommonLib.GenericVerifier(false, "Current property in an empty PathFigureEnumerator");
            }
            #endregion
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;
            CommonLib.LogTest("Results for :" + objectType);


            //Paint the surface in White for the background
            RectangleGeometry rg = new RectangleGeometry(new Rect(0, 0, m_width, m_height));
            DC.DrawGeometry(Brushes.White, null, rg);

            PathGeometry pg = new PathGeometry();
            pg.Figures.Add(pf1);
            pg.Figures.Add(pf8);
            pg.Figures.Add(pf4);
            pg.Figures.Add(pf2);
            pg.Figures.Add(pf3);
            DC.DrawGeometry(new SolidColorBrush(Colors.Blue), null, pg);
            DC.DrawGeometry(new SolidColorBrush(Colors.Red), null, pg24);

            PathGeometry pg986447 = new PathGeometry();
            pg986447.Figures.Add(new PathFigure());
            pg986447.Figures[0].StartPoint = new Point(20, 20);
            pg986447.Figures[0].Segments.Add(new LineSegment(new Point(100, 100), true));
            pg986447.Figures[0].Segments.Add(new LineSegment(new Point(100, 20), true));
            pg986447.Figures[0].Segments.Add(new LineSegment(new Point(20, 100), true));

            Pen pen986447 = new Pen(new SolidColorBrush(Colors.Yellow), 5);
            DC.DrawGeometry(null, pen986447, pg986447);
            #endregion
        }
    }
}