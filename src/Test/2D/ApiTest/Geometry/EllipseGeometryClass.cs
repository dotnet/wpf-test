// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing EllipseGeometry class
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
    internal class EllipseGeometryClass : ApiTest
    {
        public EllipseGeometryClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("LineGeometry Class");
            string objectType = "System.Windows.Media.EllipseGeometry";

            #region Section I: Constructors
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");
            EllipseGeometry eg1 = new EllipseGeometry();
            CommonLib.TypeVerifier(eg1, objectType);
            #endregion

            #region Test #2: EllipseGeometry(Rect)
            CommonLib.LogStatus("Test #2: EllipseGeometry(Rect)");
            EllipseGeometry eg2 = new EllipseGeometry(new Rect(new Point(20.01, 20.01), new Point(60.398, 40.23)));
            CommonLib.TypeVerifier(eg2, objectType);
            #endregion

            #region Test #3: EllipseGeometry(point, radiusX, radiusY)
            CommonLib.LogStatus("Test #3: EllipseGeometry(point, radiusX, radiusY)");
            EllipseGeometry eg3 = new EllipseGeometry(new Point(40.38, 50.12), 23.3, 10.3);
            CommonLib.TypeVerifier(eg3, objectType);
            #endregion
            #endregion

            #region Section II: public methods
            #region Test #5: MayHaveCurves()
            CommonLib.LogStatus("Test #5: MayHaveCurves()");
            //No width/Height on this EllipseGeometry
            EllipseGeometry eg5_NoWidthHeight = new EllipseGeometry(new Point(0, 0), 0, 0);
            EllipseGeometry eg5_Normal = new EllipseGeometry(new Point(23, 10), 100, 32);
            if (eg5_NoWidthHeight.MayHaveCurves() == true && eg5_Normal.MayHaveCurves())
            {
                CommonLib.LogStatus("MayHaveCurves() passed!");
            }
            else
            {
                CommonLib.LogFail("MayHaveCurves() failed");
                
            }
            #endregion

            #region Test #6: Copy() method
            CommonLib.LogStatus("Test #6: Copy() method");
            EllipseGeometry eg6 = eg2.Clone();
            CommonLib.GenericVerifier(eg6 != null && Math.Round(eg6.Center.X, 2) == 40.20 && Math.Round(eg6.Center.Y, 2) == 30.12, "Clone() method");
            #endregion

            #region Test #6.5:  GetRenderBounds(Pen) method
            CommonLib.LogStatus("Test #6.5: GetRenderBounds (Pen) method");
            EllipseGeometry eg65 = new EllipseGeometry(new Point(100, 100), 1, 1);
            Rect eg_bound = eg65.GetRenderBounds(new Pen(Brushes.Black, 0));

            EllipseGeometry eg65NoSize = new EllipseGeometry(new Point(1000, 0), 0, 0);
            Rect result65NoSize = eg65NoSize.GetRenderBounds(new Pen(Brushes.Red, 10));

            EllipseGeometry eg65Normal = new EllipseGeometry(new Point(-10, 100.32), 10, 20);
            Rect result65Normal = eg65Normal.GetRenderBounds(new Pen(Brushes.Blue, 5.0));

            Rect result65EpsilonPen = eg65Normal.GetRenderBounds(new Pen(Brushes.Red, Double.Epsilon));
            Rect result65HughPen = eg65Normal.GetRenderBounds(new Pen(Brushes.Red, Double.PositiveInfinity));

            CommonLib.RectVerifier(eg_bound, new Rect(99, 99, 2, 2));
            CommonLib.RectVerifier(result65NoSize, new Rect(995, -5, 10, 10));
            CommonLib.RectVerifier(result65Normal, new Rect(-22.5, 77.82, 25, 45));
            CommonLib.RectVerifier(result65EpsilonPen, new Rect(-20, 80.32, 20, 40));
            CommonLib.RectVerifier(result65HughPen, new Rect(Double.NegativeInfinity, Double.NegativeInfinity, Double.PositiveInfinity, Double.PositiveInfinity));
            #endregion

            #region Test #6.5:  GetRenderBounds(Pen, Double, ToleranceType) method
            CommonLib.LogStatus("Test #6.6: GetRenderBounds (Pen, Double, ToleranceType) method");
            EllipseGeometry eg66 = new EllipseGeometry(new Point(100, 100), 1, 1);
            Rect eg_bound66 = eg66.GetRenderBounds(new Pen(Brushes.Black, 0), 100, ToleranceType.Absolute);

            EllipseGeometry eg66NoSize = new EllipseGeometry(new Point(1000, 0), 0, 0);
            Rect result66NoSize = eg66NoSize.GetRenderBounds(new Pen(Brushes.Red, 10), -10, ToleranceType.Relative);

            EllipseGeometry eg66Normal = new EllipseGeometry(new Point(-10, 100.32), 10, 20);
            Rect result66Normal = eg66Normal.GetRenderBounds(new Pen(Brushes.Blue, 5.0), -100.323, ToleranceType.Absolute);

            Rect result66EpsilonPen = eg66Normal.GetRenderBounds(new Pen(Brushes.Red, Double.Epsilon));
            Rect result66HughPen = eg66Normal.GetRenderBounds(new Pen(Brushes.Red, Double.PositiveInfinity), 0.222, ToleranceType.Relative);

            CommonLib.RectVerifier(eg_bound66, new Rect(99, 99, 2, 2));
            CommonLib.RectVerifier(result66NoSize, new Rect(995, -5, 10, 10));
            CommonLib.RectVerifier(result66Normal, new Rect(-22.5, 77.82, 25, 45));
            CommonLib.RectVerifier(result66EpsilonPen, new Rect(-20, 80.32, 20, 40));
            CommonLib.RectVerifier(result66HughPen, new Rect(Double.NegativeInfinity, Double.NegativeInfinity, Double.PositiveInfinity, Double.PositiveInfinity));
            #endregion

            #region Test #7: CloneCurrentValue() method
            CommonLib.LogStatus("Test #7: CloneCurrentValue() method");
            EllipseGeometry eg7 = eg3.CloneCurrentValue();
            CommonLib.GenericVerifier(eg7 != null && eg7.Center.X == 40.38 && eg7.Center.Y == 50.12, "CloneCurrentValue() method");
            #endregion

            #region Test #7.55 - GetArea(double Tolerance) method
            CommonLib.LogStatus("Test #7.55 - GetArea(double Tolerance) method");
            EllipseGeometry testobj755 = new EllipseGeometry(new Rect(new Point(0, 0.232), new Size(23.33, 11.235)));
            double result755 = testobj755.GetArea(23.23, ToleranceType.Absolute);

            EllipseGeometry eg755Empty = new EllipseGeometry();
            double result755Empty = eg755Empty.GetArea(100, ToleranceType.Relative);

            EllipseGeometry eg755Tran = new EllipseGeometry(new Point(0, 0), 100, 50, new TranslateTransform(10, 10));
            double result755Tran = eg755Tran.GetArea(-100, ToleranceType.Absolute);

            CommonLib.GenericVerifier(Math.Round(result755, 4) == 205.8627 && result755Empty == 0.0 && Math.Round(result755Tran, 2) == 15707.96, "GetArea(double Tolerance) method");
            #endregion

            #region Test #7.6 - GetArea() method
            CommonLib.LogStatus("Test #7.6 - GetArea() method");
            double result76 = testobj755.GetArea();
            double result76Empty = eg755Empty.GetArea();
            double result76Tran = eg755Tran.GetArea();
            CommonLib.GenericVerifier(Math.Round(result76, 4) == 205.8627 && result76Empty == 0.0 && Math.Round(result76Tran, 2) == 15707.96, "GetArea() method");
            #endregion

            #endregion

            #region Section III: public properties
            #region Test #8: Bounds property
            CommonLib.LogStatus("Test #8: Bounds property");
            Rect rect = eg2.Bounds;
            CommonLib.RectVerifier(rect, new Rect(20.01, 20.01, 40.388, 20.22));

            EllipseGeometry eg8Empty = new EllipseGeometry();
            EllipseGeometry eg8NoSize = new EllipseGeometry(new Point(10, 10), 0, 0);
            EllipseGeometry eg8Circle = new EllipseGeometry(new Point(0.11, -10), 100, 27.32);
            EllipseGeometry eg8Tran = new EllipseGeometry(new Point(100, 100), 10, 200);
            eg8Tran.Transform = new TranslateTransform(10, 10);

            CommonLib.RectVerifier(eg8Empty.Bounds, new Rect(0, 0, 0, 0));
            CommonLib.RectVerifier(eg8NoSize.Bounds, new Rect(10, 10, 0, 0));
            CommonLib.RectVerifier(eg8Circle.Bounds, new Rect(-99.89, -37.32, 200, 54.64));
            CommonLib.RectVerifier(eg8Tran.Bounds, new Rect(100, -90, 20, 400));
            #endregion

            #region Test #8.1:  Regression case for Regression_Bug4
            CommonLib.LogStatus("Test #8.1: Regression case for Regression_Bug4");
            EllipseGeometry eg81 = new EllipseGeometry(new Point(100, 100), 0, 0);
            Rect bounds81 = eg81.GetRenderBounds(new Pen(Brushes.Black, 0));
            CommonLib.RectVerifier(bounds81, new Rect(100.00, 100.00, 0, 0));
            #endregion

            #region Test #9a: Center property with default value
            CommonLib.LogStatus("Test #9a: Center property with default value");
            EllipseGeometry eg9a = new EllipseGeometry();
            CommonLib.GenericVerifier(eg9a.Center.X == 0 && eg9a.Center.Y == 0, "Center property with default value");
            #endregion

            #region Test #9b: Center property in Invalid state
            CommonLib.LogStatus("Test #9b: Center property in Invalid state");
            EllipseGeometry eg9b = new EllipseGeometry();
            eg9b.InvalidateProperty(EllipseGeometry.CenterProperty);
            CommonLib.GenericVerifier(eg9b.Center.X == 0 && eg9b.Center.Y == 0, "Center property in Invalid state");
            #endregion

            #region Test #9: Center property
            CommonLib.LogStatus("Test #9: Center property");
            eg2.Center = new Point(60.309, 78.23);
            CommonLib.GenericVerifier(eg2.Center.X == 60.309 && eg2.Center.Y == 78.23, "Center property");
            #endregion

            #region Test #10: CenterAnimations property
            CommonLib.LogStatus("Test #10: CenterAnimations property");
            PointAnimation pointAnim = new PointAnimation(new Point(0, 1), new Point(40, 40), new Duration(TimeSpan.FromMilliseconds(1000)));
            AnimationClock pointClock = pointAnim.CreateClock();
            eg2.ApplyAnimationClock(EllipseGeometry.CenterProperty, pointClock);
            CommonLib.GenericVerifier(pointClock != null, "CenterAnimations property");
            #endregion

            #region Test #11: CanFreeze property
            CommonLib.LogStatus("Test #11: CanFreeze property");
            CommonLib.GenericVerifier(!eg2.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #13a: RadiusX property with basevalue
            CommonLib.LogStatus("Test #13a: RadiusX property with basevalue");
            EllipseGeometry eg13a = new EllipseGeometry();
            CommonLib.GenericVerifier(eg13a.RadiusX == 0, "RadiusX with basevalue");
            #endregion

            #region Test #13b:  RadiusX property with LocalPropertyState.Invalid
            CommonLib.LogStatus("Test #13b:  RadiusX property with LocalPropertyState.Invalid");
            EllipseGeometry eg13b = new EllipseGeometry();
            eg13b.InvalidateProperty(EllipseGeometry.RadiusXProperty);
            CommonLib.GenericVerifier(eg13b.RadiusX == 0, "RadiusX property in Invalid state");
            #endregion

            #region Test #13: RadiusX property
            CommonLib.LogStatus("Test #13: RadiusX property");
            eg2.RadiusX = 12.1;
            CommonLib.GenericVerifier(eg2.RadiusX == 12.1, "RadiusX property");
            #endregion

            #region Test #14: RadiusXAnimations property
            CommonLib.LogStatus("Test #14: RadiusXAnimations property");
            DoubleAnimation doubleAnimX = new DoubleAnimation(2.2, 3.43, new Duration(TimeSpan.FromMilliseconds(1000)));
            AnimationClock doubleClockX = doubleAnimX.CreateClock();
            eg2.ApplyAnimationClock(EllipseGeometry.RadiusXProperty, doubleClockX);
            CommonLib.GenericVerifier(doubleClockX != null, "RadiusXAnimation property");
            #endregion

            #region Test #15a: RadiusY property with default value
            CommonLib.LogStatus("Test #15a: RadiusY property with default value");
            EllipseGeometry eg15a = new EllipseGeometry();
            CommonLib.GenericVerifier(eg15a.RadiusY == 0, "RadiusY property with default value");
            #endregion

            #region Test #15b: RadiusY property in Invalid state
            CommonLib.LogStatus("Test #15b: RadiusY property in Invalid state");
            EllipseGeometry eg15b = new EllipseGeometry();
            eg15b.InvalidateProperty(EllipseGeometry.RadiusYProperty);
            CommonLib.GenericVerifier(eg15b.RadiusY == 0, "RadiusY property in Invalid state");
            #endregion

            #region Test #15: RadiusY property
            CommonLib.LogStatus("Test #15: RadiusY property");
            eg2.RadiusY = 40.2;
            CommonLib.GenericVerifier(eg2.RadiusY == 40.2, "RadiusY property");
            #endregion

            #region Test #16: RadiusYAnimations property
            CommonLib.LogStatus("Test #16: RadiusY property");
            DoubleAnimation doubleAnimY = new DoubleAnimation(31.28, 10.3, new Duration(TimeSpan.FromMilliseconds(200)));
            AnimationClock doubleClockY = doubleAnimY.CreateClock();
            eg2.ApplyAnimationClock(EllipseGeometry.RadiusYProperty, doubleClockY);
            CommonLib.GenericVerifier(doubleClockY != null, "RadiusYAnimation property");
            #endregion
            #endregion

            #region Circular testing for GetArea() and Bounds
            CommonLib.CircularTestGeometry(eg8Empty);
            CommonLib.CircularTestGeometry(eg8NoSize);
            CommonLib.CircularTestGeometry(eg8Circle);
            CommonLib.CircularTestGeometry(eg8Tran);
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogTest("Result for" + objectType);

            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Blue), 2), eg2);
            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Blue), 2), eg3);
            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Yellow), 2), testobj755);

            // --------------------------------------------------------------------------------------------------------
            // -----                        EllipseGeometry Bad Parameter test                                    -----
            EllipseGeometry[] egBad = new EllipseGeometry[] {
                                    //new EllipseGeometry( Rect.Empty ),
                                    new EllipseGeometry( new Rect( Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon ) ),
                                    new EllipseGeometry( new Rect( Double.NaN, Double.NaN, 50, Double.PositiveInfinity ) ),
                                    new EllipseGeometry( new Rect( Double.MaxValue, Double.MaxValue, 100, 32 ) ),
                                    new EllipseGeometry( new Rect( Double.MinValue, Double.MinValue, 100, 1000 ) ),
                                    new EllipseGeometry( new Rect( Double.NegativeInfinity, Double.NegativeInfinity, 23, 11 ) ),
                                    new EllipseGeometry( new Rect( Double.PositiveInfinity, Double.PositiveInfinity, 100, 132 ) ),
                                    new EllipseGeometry( new Rect( 100, 200, Double.MaxValue, Double.MaxValue ) ),
                                    new EllipseGeometry( new Rect( 100, -203, Double.PositiveInfinity, Double.PositiveInfinity ) ),
                                    new EllipseGeometry( new Rect( new Point( Double.Epsilon, Double.Epsilon ), new Point ( Double.MaxValue, Double.MaxValue ) ) ),
                                    new EllipseGeometry( new Rect( new Point( Double.PositiveInfinity, Double.PositiveInfinity ), new Point ( Double.NegativeInfinity, Double.NegativeInfinity ) ) ),
                                    new EllipseGeometry( new Rect( new Point( Double.NaN, Double.NaN ), new Point( Double.NaN, Double.NaN ) ) ),
                                    new EllipseGeometry( new Point( Double.NaN, Double.NaN ), 10, 10 ),
                                    new EllipseGeometry( new Point( Double.Epsilon, Double.Epsilon ), Double.Epsilon, Double.Epsilon ),
                                    new EllipseGeometry( new Point( Double.NaN, Double.NaN ), Double.NaN, Double.NaN ),
                                    new EllipseGeometry( new Point( Double.MaxValue, Double.MaxValue ), Double.MaxValue, Double.MaxValue ),
                                    new EllipseGeometry( new Point( Double.MinValue, Double.MinValue ), 10, Double.Epsilon ),
                                    new EllipseGeometry( new Point( Double.PositiveInfinity, Double.PositiveInfinity ), Double.PositiveInfinity, Double.PositiveInfinity ),
                                    new EllipseGeometry( new Point( Double.NegativeInfinity, Double.NegativeInfinity ), Double.MaxValue, Double.NaN ),
                                    new EllipseGeometry( new Point( 10, 10 ), 10, 100, null ),
                                    new EllipseGeometry( new Point( 10, 10 ), 10, 100, Transform.Identity ),
                              };

            foreach (EllipseGeometry egb in egBad)
            {
                DC.DrawGeometry(null, new Pen(Brushes.Red, 2), egb);
            }

            // ----------------------------------------------------------------------------------------------------------
            // ----                          DrawEllipse DrawDirect Bad Parameter test                               ----
            DC.DrawEllipse(Brushes.Black, new Pen(Brushes.Red, 10), new Point(Double.NaN, Double.NaN), 10, 10);
            DC.DrawEllipse(Brushes.Black, new Pen(Brushes.Red, 10), new Point(Double.Epsilon, Double.Epsilon), Double.Epsilon, Double.Epsilon);
            DC.DrawEllipse(Brushes.Black, new Pen(Brushes.Red, 10), new Point(Double.NaN, Double.NaN), Double.NaN, Double.NaN);
            DC.DrawEllipse(Brushes.Black, new Pen(Brushes.Red, 10), new Point(Double.MaxValue, Double.MaxValue), Double.MaxValue, Double.MaxValue);
            DC.DrawEllipse(Brushes.Black, new Pen(Brushes.Red, 10), new Point(Double.MinValue, Double.MinValue), 10, Double.Epsilon);
            DC.DrawEllipse(Brushes.Black, new Pen(Brushes.Red, 10), new Point(Double.PositiveInfinity, Double.PositiveInfinity), Double.PositiveInfinity, Double.PositiveInfinity);
            DC.DrawEllipse(Brushes.Black, new Pen(Brushes.Red, 10), new Point(Double.NegativeInfinity, Double.NegativeInfinity), Double.MaxValue, Double.NaN);
            #endregion
        }
    }
}