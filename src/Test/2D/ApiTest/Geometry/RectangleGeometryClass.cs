// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing RectangleGeometry class
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using AN = System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{ 
    /// <summary>
    /// Summary description for RectangleGeometryClass.
    /// </summary>
    internal class RectangleGeometryClass : ApiTest
    {
        public RectangleGeometryClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("RectangleGeometry Class");
            string objectType = "System.Windows.Media.RectangleGeometry";

            #region Section I: Constructors
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default construtor");
            RectangleGeometry rg1 = new RectangleGeometry();
            CommonLib.TypeVerifier(rg1, objectType);
            #endregion

            #region Test #2: RectangleGeometry(rect)
            CommonLib.LogStatus("Test #2: RectangleGeometry(rect)");
            RectangleGeometry rg2 = new RectangleGeometry(new Rect(new Point(39.01, 10.01), new Point(100.01, 79.10)));
            CommonLib.TypeVerifier(rg2, objectType);
            #endregion

            #region Test #3: RectangleGeometry(rect, radiusX, radiusY)
            CommonLib.LogStatus("Test #3: RectangleGeometry(rect, radiusX, radiusY)");
            RectangleGeometry rg3 = new RectangleGeometry(new Rect(new Point(29.12, 10.38), new Point(39.23, 120.12)), 29.2, 10.3);
            CommonLib.TypeVerifier(rg3, objectType);
            #endregion

            #endregion

            #region Section II: public methods
            #region Test #6: Copy() method
            CommonLib.LogStatus("Test #6: Copy() method");
            RectangleGeometry rg6 = rg3.Clone();
            CommonLib.GenericVerifier(rg6 != null && rg6.RadiusX == 29.2 && rg6.RadiusY == 10.3, "Copy() method");
            #endregion

            #region Test #6.1 - GetRenderBounds (Pen) method
            CommonLib.LogStatus("Test #6.1 - GetRenderBound () method");
            RectangleGeometry rg61empty = new RectangleGeometry();
            Rect result61Empty = rg61empty.GetRenderBounds(new Pen(Brushes.Red, 1000));

            RectangleGeometry rg61Normal = new RectangleGeometry(new Rect(100, 100, 20, 20), 0, 0);
            Rect result61Normal = rg61Normal.GetRenderBounds(new Pen(Brushes.Red, 5.0));

            RectangleGeometry rg61Rounded = new RectangleGeometry(new Rect(-100, 50, 30.5, 10.5), 10, 10);
            Rect result61Rounded = rg61Rounded.GetRenderBounds(new Pen(Brushes.Black, 10.0));

            Rect result61NoPen = rg61Normal.GetRenderBounds(null);

            Rect result61ZeroPen = rg61Normal.GetRenderBounds(new Pen(Brushes.Red, 0.0));

            CommonLib.RectVerifier(result61Empty, Rect.Empty);
            CommonLib.RectVerifier(result61Normal, new Rect(97.5, 97.5, 25, 25));
            CommonLib.RectVerifier(result61Rounded, new Rect(-105, 45, 40.5, 20.5));
            CommonLib.RectVerifier(result61NoPen, new Rect(100, 100, 20, 20));
            CommonLib.RectVerifier(result61ZeroPen, new Rect(100, 100, 20, 20));
            #endregion

            #region Test #6.2 - GetRenderBounds ( Pen, Double, ToleranceType ) method
            CommonLib.LogStatus("Test #6.2 - GetRenderBounds ( Pen, Double, ToleranceType ) method");
            RectangleGeometry rg62empty = new RectangleGeometry();
            Rect result62Empty = rg62empty.GetRenderBounds(new Pen(Brushes.Red, 1000), 100, ToleranceType.Relative);

            RectangleGeometry rg62Normal = new RectangleGeometry(new Rect(100, 100, 20, 20), 0, 0);
            Rect result62Normal = rg62Normal.GetRenderBounds(new Pen(Brushes.Red, 5.0), 10, ToleranceType.Absolute);

            RectangleGeometry rg62Rounded = new RectangleGeometry(new Rect(-100, 50, 30.5, 10.5), 10, 10);
            Rect result62Rounded = rg62Rounded.GetRenderBounds(new Pen(Brushes.Black, 10.0), -100, ToleranceType.Absolute);

            Rect result62NoPen = rg62Normal.GetRenderBounds(null, 10, ToleranceType.Relative);

            Rect result62ZeroPen = rg62Normal.GetRenderBounds(new Pen(Brushes.Red, 0.0), -10, ToleranceType.Relative);

            CommonLib.RectVerifier(result62Empty, Rect.Empty);
            CommonLib.RectVerifier(result62Normal, new Rect(97.5, 97.5, 25, 25));
            CommonLib.RectVerifier(result62Rounded, new Rect(-105, 45, 40.5, 20.5));
            CommonLib.RectVerifier(result62NoPen, new Rect(100, 100, 20, 20));
            CommonLib.RectVerifier(result62ZeroPen, new Rect(100, 100, 20, 20));
            #endregion

            #region Test #6.6 - GetArea(double Tolerance, ToleranceType)
            CommonLib.LogStatus("Test #6.6 - GetArea(double Tolerance) method");
            RectangleGeometry rg66 = new RectangleGeometry(new Rect(new Point(32, 90), new Size(69, 10)));
            double result66 = rg66.GetArea(0.232233, ToleranceType.Absolute);

            RectangleGeometry rg66Empty = new RectangleGeometry();
            double result66Empty = rg66Empty.GetArea(100, ToleranceType.Relative);

            RectangleGeometry rg66Tran = new RectangleGeometry(new Rect(0, 0, 100, 100), 5, 5, new RotateTransform(45));
            double result66Tran = rg66Tran.GetArea(0, ToleranceType.Relative);

            CommonLib.GenericVerifier(result66 == 690 && result66Empty == 0.0 && Math.Round(result66Tran, 2) == 9978.54, "GetArea(double Tolerance, ToleranceType) method");
            #endregion

            #region Test #6.8 - GetArea()
            CommonLib.LogStatus("Test #6.8 - GetArea() method");
            RectangleGeometry rg68 = new RectangleGeometry(new Rect(new Point(32, 90), new Size(69, 10)));
            double result68 = rg68.GetArea(0.232233, ToleranceType.Absolute);

            RectangleGeometry rg68Empty = new RectangleGeometry();
            double result68Empty = rg68Empty.GetArea(100, ToleranceType.Relative);

            RectangleGeometry rg68Tran = new RectangleGeometry(new Rect(0, 0, 100, 100), 5, 5, new RotateTransform(45));
            double result68Tran = rg68Tran.GetArea();

            CommonLib.GenericVerifier(result68 == 690 && result68Empty == 0.0 && Math.Round(result68Tran, 2) == 9978.54, "GetArea() method");
            #endregion

            #region Test #7: CloneCurrentValue() method
            CommonLib.LogStatus("Test #7: CloneCurrentValue() method");
            RectangleGeometry rg7 = rg2.CloneCurrentValue() as RectangleGeometry;
            //Verify the new RectangleGeometry object is not NULL and contains the correct values
            CommonLib.GenericVerifier(rg7 != null && rg7.Rect.Left == 39.01 && rg7.Rect.Top == 10.01, "CloneCurrentValue() method");
            #endregion

            #region Test #7.1 - MayHaveCurves()
            CommonLib.LogStatus("Test #7.1 - MayHaveCurves()");
            RectangleGeometry rg71 = new RectangleGeometry(new Rect(0, 0, 0, 0));
            bool result71 = rg71.MayHaveCurves();
            //Rectangle doesn't have curve
            CommonLib.GenericVerifier(result71 == false, "MayHaveCurves()");
            #endregion
            #endregion

            #region Section III: public properties
            #region Test #8: Bounds property
            CommonLib.LogStatus("Test #8: Bounds property");
            Rect rect = rg2.Bounds;
            CommonLib.RectVerifier(rect, new Rect(39.01, 10.01, 61, 69.09));

            RectangleGeometry rg8empty = new RectangleGeometry();
            RectangleGeometry rg8Normal = new RectangleGeometry(new Rect(100, 100, 20, 20), 0, 0);
            RectangleGeometry rg8Rounded = new RectangleGeometry(new Rect(-100, 50, 30.5, 10.5), 10, 10);
            RectangleGeometry rg8Line = new RectangleGeometry(new Rect(10, -10, 100, 0), 10, 10);
            RectangleGeometry rg8Rotate = new RectangleGeometry(new Rect(10, 10, 100, 20), 4, 10);
            rg8Rotate.Transform = new RotateTransform(45);

            CommonLib.RectVerifier(rg8empty.Bounds, Rect.Empty);
            CommonLib.RectVerifier(rg8Normal.Bounds, new Rect(100, 100, 20, 20));
            CommonLib.RectVerifier(rg8Rounded.Bounds, new Rect(-100, 50, 30.5, 10.5));
            CommonLib.RectVerifier(rg8Line.Bounds, new Rect(10, -10, 100, 0));
            CommonLib.RectVerifier(rg8Rotate.Bounds, new Rect(-11.86, 16.42, 80.29, 80.29));
            #endregion

            #region Test #9: CanFreeze property
            CommonLib.LogStatus("Test #9: CanFreeze property");
            CommonLib.GenericVerifier(rg2.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #12a: RadiusX property with basevalue
            CommonLib.LogStatus("Test #12a: RadiusX property with basevalue");
            RectangleGeometry rg12a = new RectangleGeometry();
            CommonLib.GenericVerifier(rg12a.RadiusX == 0, "RadiusX property with basevalue");
            #endregion

            #region Test #12b: RadiusX property in Invalid state
            CommonLib.LogStatus("Test #12b: RadiusX property in Invalid state");
            RectangleGeometry rg12b = new RectangleGeometry();
            rg12b.InvalidateProperty(RectangleGeometry.RadiusXProperty);
            CommonLib.GenericVerifier(rg12b.RadiusX == 0, "RadiusX property in Invalid state");
            #endregion

            #region Test #12: RadiusX property
            CommonLib.LogStatus("Test #12: RadiusX property");
            rg2.RadiusX = 10.292;
            CommonLib.GenericVerifier(rg2.RadiusX == 10.292, "RadiusX property");
            #endregion

            #region Test #14a: RadiusY property with basevalue
            CommonLib.LogStatus("Test #14a: RadiusY property with basevalue");
            RectangleGeometry rg14a = new RectangleGeometry();
            CommonLib.GenericVerifier(rg14a.RadiusY == 0, "RadiusY property with basevalue");
            #endregion

            #region Test #14b: RadiusY property in Invalid state
            CommonLib.LogStatus("Test #14b: RadiusY property in Invalid state");
            RectangleGeometry rg14b = new RectangleGeometry();
            rg14b.InvalidateProperty(RectangleGeometry.RadiusYProperty);
            CommonLib.GenericVerifier(rg14b.RadiusY == 0, "RadiusY property in Invalid state");
            #endregion

            #region Test #14: RadiusY property
            CommonLib.LogStatus("Test #14: RadiusY property");
            rg2.RadiusY = 2.23;
            CommonLib.GenericVerifier(rg2.RadiusY == 2.23, "RadiusY property");
            #endregion

            #region Test #16a: Rect property with basevalue
            CommonLib.LogStatus("Test #16a:  Rect property with basevalue");
            RectangleGeometry rg16a = new RectangleGeometry();
            CommonLib.GenericVerifier(rg16a.Rect.IsEmpty, "Rect property with basevalue");
            #endregion

            #region Test #16b:  Rect property in Invalid state
            CommonLib.LogStatus("Test #16b:  Rect property in Invalid state");
            RectangleGeometry rg16b = new RectangleGeometry();
            rg16b.InvalidateProperty(RectangleGeometry.RectProperty);
            CommonLib.GenericVerifier(rg16a.Rect.IsEmpty, "Rect property in Invalid state");
            #endregion

            #region Test #16: Rect property
            CommonLib.LogStatus("Test #16: Rect property");
            rg2.Rect = new Rect(new Point(21.39, 21.392), new Point(101.1, 101.1));
            CommonLib.RectVerifier(rg2.Rect, new Rect(new Point(21.39, 21.392), new Point(101.1, 101.1)));
            #endregion

            #endregion

            #region Regression cases
            #region Regression case for Regression_Bug19
            CommonLib.LogStatus("Regression case for Regression_Bug19");
            RectangleGeometry rg18 = new RectangleGeometry(Rect.Empty);
            Rect bound18 = rg18.Bounds;
            CommonLib.LogStatus("No regression for Regression_Bug19");
            #endregion
            #endregion

            #region Circular test for GetArea() and Bounds
            CommonLib.LogStatus("Circular test for GetArea() and Bounds");
            CommonLib.CircularTestGeometry(rg66);
            CommonLib.CircularTestGeometry(rg66Tran);
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;
            CommonLib.LogTest("Results from :" + objectType);

            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), 1), rg2);
            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), 1), rg3);

            // -----------------------------------------------------------------------------
            // ----         Bad Parameter test section                                  ----
            RectangleGeometry[] rgBad = new RectangleGeometry[] {
                                    new RectangleGeometry( Rect.Empty ),
                                    new RectangleGeometry( new Rect( Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon ) ),
                                    new RectangleGeometry( new Rect( Double.NaN, Double.NaN, 50, Double.PositiveInfinity ) ),
                                    new RectangleGeometry( new Rect( Double.MaxValue, Double.MaxValue, 100, 32 ) ),
                                    new RectangleGeometry( new Rect( Double.MinValue, Double.MinValue, 100, 1000 ) ),
                                    new RectangleGeometry( new Rect( Double.NegativeInfinity, Double.NegativeInfinity, 23, 11 ) ),
                                    new RectangleGeometry( new Rect( Double.PositiveInfinity, Double.PositiveInfinity, 100, 132 ) ),
                                    new RectangleGeometry( new Rect( 100, 200, Double.MaxValue, Double.MaxValue ) ),
                                    new RectangleGeometry( new Rect( 100, -203, Double.PositiveInfinity, Double.PositiveInfinity ) ),
                                    new RectangleGeometry( new Rect( new Point( Double.Epsilon, Double.Epsilon ), new Point ( Double.MaxValue, Double.MaxValue ) ) ),
                                    new RectangleGeometry( new Rect( new Point( Double.PositiveInfinity, Double.PositiveInfinity ), new Point ( Double.NegativeInfinity, Double.NegativeInfinity ) ) ),
                                    new RectangleGeometry( new Rect( new Point( Double.NaN, Double.NaN ), new Point( Double.NaN, Double.NaN ) ) ),
                                    //Regression_Bug20 - Assertion
                                    //new RectangleGeometry( new Rect( new Point( 100, 100 ), new Point( -.232, 1000 ) ), Double.NaN, Double.NaN ),
                                    new RectangleGeometry( new Rect( 0, 0, 0, 0 ), Double.Epsilon, Double.Epsilon ),
                                    new RectangleGeometry( new Rect( 10, 10, 200, 10 ), Double.PositiveInfinity, Double.PositiveInfinity ),
                                    new RectangleGeometry( new Rect( 10, 10, 200, 10 ), Double.NegativeInfinity, Double.NegativeInfinity ),
                                    new RectangleGeometry( new Rect( 10, 10, 200, 10 ), Double.MaxValue, Double.MaxValue ),
                                    new RectangleGeometry( new Rect( 10, 10, 200, 10 ), Double.MinValue, Double.MinValue ),
                                    new RectangleGeometry( new Rect( 10, 10, 200, 10 ), Double.MinValue, Double.MaxValue ),
                                    new RectangleGeometry( new Rect( 10, 10, 200, 10 ), 10, 2, null ),
                                    new RectangleGeometry( new Rect( 10, 10, 200, 10 ), 10, 2, Transform.Identity ),
                                };
            foreach (RectangleGeometry rgb in rgBad)
            {
                DC.DrawGeometry(Brushes.Black, new Pen(Brushes.Red, 3), rgb);
            }

            // ---------------------------------------------------------------------------------
            // ----             DrawRectangle bad parameter test section                    ----
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), Rect.Empty);
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.Epsilon, Double.Epsilon, Double.Epsilon, Double.Epsilon));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, 50, Double.PositiveInfinity));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.MaxValue, Double.MaxValue, 100, 32));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.MinValue, Double.MinValue, 100, 1000));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NegativeInfinity, Double.NegativeInfinity, 23, 11));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.PositiveInfinity, Double.PositiveInfinity, 100, 132));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(100, 200, Double.MaxValue, Double.MaxValue));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(100, -203, Double.PositiveInfinity, Double.PositiveInfinity));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(new Point(Double.Epsilon, Double.Epsilon), new Point(Double.MaxValue, Double.MaxValue)));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(new Point(Double.PositiveInfinity, Double.PositiveInfinity), new Point(Double.NegativeInfinity, Double.NegativeInfinity)));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(new Point(Double.NaN, Double.NaN), new Point(Double.NaN, Double.NaN)));
            DC.DrawRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN));

            DC.DrawRoundedRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), Double.Epsilon, Double.Epsilon);
            DC.DrawRoundedRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), Double.PositiveInfinity, Double.PositiveInfinity);
            DC.DrawRoundedRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), Double.NegativeInfinity, Double.NegativeInfinity);
            DC.DrawRoundedRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), Double.MaxValue, Double.MaxValue);
            DC.DrawRoundedRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), Double.MinValue, Double.MinValue);
            DC.DrawRoundedRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), Double.MaxValue, Double.MinValue);
            DC.DrawRoundedRectangle(Brushes.Black, new Pen(Brushes.Red, 3), new Rect(Double.NaN, Double.NaN, Double.NaN, Double.NaN), Double.NegativeInfinity, Double.PositiveInfinity);
            #endregion

        }
    }
}
