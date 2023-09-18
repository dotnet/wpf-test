// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  GMC API Tests - Testing CombinedGeometry class
//  Author:   Microsoft
//
using System;
using System.ComponentModel.Design.Serialization;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{ 
    /// <summary>
    /// Summary description for LineGeometryClass.
    /// </summary>
    internal class CombinedGeometryClass : ApiTest
    {

        public CombinedGeometryClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("CombinedGeometry Class");
            string objectType = "System.Windows.Media.CombinedGeometry";

            #region Section I: Constructors
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");
            CombinedGeometry cg1 = new CombinedGeometry();
            CommonLib.TypeVerifier(cg1, objectType);
            #endregion

            #region Test #2: CombinedGeometry(Geometry1, Geometry2)
            CommonLib.LogStatus("Test #2: CombinedGeometry(Geometry1, Geometry2)");
            CombinedGeometry cg2 = new CombinedGeometry(new LineGeometry(), new RectangleGeometry());
            CommonLib.TypeVerifier(cg2, objectType);
            #endregion

            #region Test #3: CombinedGeometry(CombinedMode, Geometry1, Geometry2)
            CommonLib.LogStatus("Test #3: CombinedGeometry(CombinedMode, Geometry1, Geometry2)");
            CombinedGeometry cg3 = new CombinedGeometry(GeometryCombineMode.Union, new EllipseGeometry(), new PathGeometry());
            CommonLib.TypeVerifier(cg3, objectType);
            #endregion

            #region Test #4: CombinedGeomtry(CombinedMode, Geometry1, Geometry2, Transform)
            CommonLib.LogStatus("Test #4: CombinedGeometry(GeometryCombineMode, Geometry1, Geometry2, Transform)");
            CombinedGeometry cg4 = new CombinedGeometry(GeometryCombineMode.Exclude, new LineGeometry(), new LineGeometry(), new RotateTransform(45));
            CommonLib.TypeVerifier(cg4, objectType);
            #endregion
            #endregion

            #region Section II: public methods
            #region Test #5: Copy() method
            CommonLib.LogStatus("Test #5: Copy() method");
            CombinedGeometry cg5 = new CombinedGeometry(new LineGeometry(), new LineGeometry());
            CombinedGeometry cgCopy = cg5.Clone();
            CommonLib.GenericVerifier(cg5.Geometry1 is LineGeometry && cg5.Geometry2 is LineGeometry, "Copy() method");
            #endregion

            #region Test #6: Contains() method
            CommonLib.LogStatus("Test #6: Contains() method");
            CombinedGeometry cg6 = new CombinedGeometry(new RectangleGeometry(new Rect(20, 30, 50, 50)), new RectangleGeometry(new Rect(40, 40, 32, 100)));
            //should return true with point(45, 45)when CombinedMode=CombinedMode.Union
            bool result6 = cg6.FillContains(new Point(25, 33));

            cg6.GeometryCombineMode = GeometryCombineMode.Intersect;
            //should return false with Point (0, 0) when CombinedMode=CombinedMode.Intersect
            result6 &= !cg6.FillContains(new Point(25, 33));

            cg6.GeometryCombineMode = GeometryCombineMode.Exclude;
            //should return true with Point(25, 33) when CombinedMode=CombinedMode.Exclude
            result6 &= cg6.FillContains(new Point(25, 33));

            cg6.GeometryCombineMode = GeometryCombineMode.Xor;
            //should return true with Point(25, 33) when CombinedMode=CombinedMode.Xor
            result6 &= cg6.FillContains(new Point(25, 33));

            CommonLib.GenericVerifier(result6, "Contains()");
            #endregion

            #region Test #7: GetArea(double, ToleranceType)

            CommonLib.LogStatus("Test #7: GetArea(double, ToleranceType) method");
            CombinedGeometry cg7 = new CombinedGeometry(new RectangleGeometry(new Rect(10, 20, 33, 23)), new EllipseGeometry(new Point(32, 2), 20, 11));
            double result7 = cg7.GetArea(-1000000.0000000, ToleranceType.Absolute);

            CombinedGeometry cg7Empty = new CombinedGeometry();
            double result7Empty = cg7Empty.GetArea(0, ToleranceType.Relative);

            CombinedGeometry cg7Tran = new CombinedGeometry(new RectangleGeometry(new Rect(10, 20, 33, 23)), new EllipseGeometry(new Point(32, 2), 20, 11));
            cg7Tran.Transform = new RotateTransform(-100323, 100, 32);
            double result7Tran = cg7Tran.GetArea(100, ToleranceType.Absolute);

            CombinedGeometry cg7NoSize = new CombinedGeometry(GeometryCombineMode.Union,
                                                              new LineGeometry(new Point(100, 100), new Point(120, 120)),
                                                              new LineGeometry(new Point(100, 100), new Point(120, 120))
                                                              );
            double result7NoSize = cg7NoSize.GetArea(100, ToleranceType.Relative);

            CommonLib.LogStatus("Test #7 Disabled, see Regression_Bug25");            
            CommonLib.GenericVerifier(Math.Round(result7, 4) == 1450.3437 &&
                                      MathEx.AreCloseEnough(result7Empty, 0) &&
                                      Math.Round(result7Tran, 4) == 1218.9340 &&
                                      MathEx.AreCloseEnough(result7NoSize, 0),
                                      "GetArea(double, ToleranceType) method"
                                   );
            
            #endregion

            #region Test #7.1 GetArea ()
            CommonLib.LogStatus("Test #7.1 GetArea() ");
            double result71 = cg7.GetArea();
            double result71Empty = cg7Empty.GetArea();
            double result71Tran = cg7Tran.GetArea();
            double result71NoSize = cg7NoSize.GetArea();
            CommonLib.LogStatus("Test #7.1 Disabled, see Regression_Bug25");           
            CommonLib.GenericVerifier(Math.Round(result71, 2) == 1445.9 &&
                          MathEx.AreCloseEnough(result71Empty, 0) &&
                          Math.Round(result71Tran, 2) == 1445.54 &&
                          MathEx.AreCloseEnough(result71NoSize, 0),
                          "GetArea() method"
                       );            
            #endregion

            #region Test #8: CloneCurrentValue() method
            CommonLib.LogStatus("Test #8: CloneCurrentValue() method");
            CombinedGeometry cg8 = new CombinedGeometry(new LineGeometry(), new LineGeometry());
            CombinedGeometry retval = cg8.CloneCurrentValue();
            CommonLib.GenericVerifier(retval.Geometry1 is LineGeometry && retval.Geometry2 is LineGeometry, "CloneCurrentValue() method");
            #endregion

            #region Test #10 - GetRenderBounds(Pen) method
            CommonLib.LogStatus("Test #10 - GetRenderBounds(Pen) method");
            CombinedGeometry cg10 = new CombinedGeometry(GeometryCombineMode.Exclude, new EllipseGeometry(new Point(30, 12), 100, 100), new RectangleGeometry(new Rect(50, 50, 29, 30)));
            Rect retval10 = cg10.GetRenderBounds(new Pen(Brushes.Black, 2));

            cg10.Transform = new TranslateTransform(10, 10);
            Rect retval10Tran = cg10.GetRenderBounds(new Pen(Brushes.Black, 2.0));

            CombinedGeometry cg10_2 = new CombinedGeometry();
            Rect retval10_2 = cg10_2.GetRenderBounds(new Pen(Brushes.Black, 10));

            Rect retval10EmptyPen = cg10.GetRenderBounds(new Pen(Brushes.Red, 0));

            CombinedGeometry cg10NoSize = new CombinedGeometry(
                                                            GeometryCombineMode.Intersect,
                                                            new RectangleGeometry(new Rect(0, 100, 29, 10)),
                                                            new RectangleGeometry(new Rect(1000, 1000, 20, 10))
                                                            );
            Rect retval10NoSize = cg10NoSize.GetRenderBounds(new Pen(Brushes.Red, 1000000));

            CommonLib.RectVerifier(retval10, new Rect(-71, -89, 202, 202));
            CommonLib.RectVerifier(retval10Tran, new Rect(-61, -79, 202, 202));
            CommonLib.RectVerifier(retval10_2, Rect.Empty);
            CommonLib.RectVerifier(retval10EmptyPen, new Rect(-60, -78, 200, 200));
            CommonLib.RectVerifier(retval10NoSize, Rect.Empty);
            #endregion

            #region Test #10.1 - GetRenderBounds(Pen, Double, ToleranceType) method
            CommonLib.LogStatus("Test #10.1 - GetRenderBounds(Pen, Double, ToleranceType) method");
            Rect result101Tran = cg10.GetRenderBounds(new Pen(Brushes.Black, 2.0), 100, ToleranceType.Absolute);
            Rect result101Empty = cg10_2.GetRenderBounds(new Pen(Brushes.Red, 2.0), -100, ToleranceType.Absolute);
            Rect result101EmptyPen = cg10.GetRenderBounds(new Pen(Brushes.Red, 0), 0.1232, ToleranceType.Relative);
            Rect result101NoSize = cg10NoSize.GetRenderBounds(new Pen(Brushes.Red, 10000), -10, ToleranceType.Relative);

            CommonLib.RectVerifier(result101Tran, new Rect(-61, -79, 202, 202));
            CommonLib.RectVerifier(result101Empty, Rect.Empty);
            CommonLib.RectVerifier(result101EmptyPen, new Rect(-60, -78, 200, 200));
            CommonLib.RectVerifier(result101NoSize, Rect.Empty);

            #endregion

            #region Test #11: internal GetPathFigureCollection() method
            CommonLib.LogStatus("Test #11: internal GetPathFigureCollection() method");
            PathGeometry pg11 = new PathGeometry();
            CombinedGeometry cg11 = new CombinedGeometry(new RectangleGeometry(new Rect(0, 0, 100, 122)), new RectangleGeometry(new Rect(30, 30, 22, 100)));
            pg11.AddGeometry(cg11);
            CommonLib.GenericVerifier(pg11.Figures[0].Segments.Count == 1, "internal GetPathFigureCollection() method");
            #endregion

            #region Test #11.5: MayHaveCurves()
            CommonLib.LogStatus("Test #11.5: MayHaveCurves()");
            //Do NOT have curve in this CG
            CombinedGeometry cg115_NoCurve = new CombinedGeometry();
            cg115_NoCurve.Geometry1 = new RectangleGeometry(new Rect(32, 1, 32, 32));

            CombinedGeometry cg115_Curve = new CombinedGeometry();
            cg115_Curve.Geometry2 = new EllipseGeometry(new Point(90, 23), 13, 33);
            cg115_Curve.Geometry1 = new RectangleGeometry(new Rect(100, 40, 200, 200));
            cg115_Curve.GeometryCombineMode = GeometryCombineMode.Intersect;

            if (cg115_NoCurve.MayHaveCurves() == false && cg115_Curve.MayHaveCurves() == true)
            {
                CommonLib.LogStatus("MayHaveCurves() passed!");
            }
            else
            {
                CommonLib.LogFail("MayHaveCurves() failed");
            }
            #endregion
            #endregion

            #region Section III: public properties
            #region Test #12: Bounds property
            CommonLib.LogStatus("Test #12: Bounds property");
            CombinedGeometry cg12 = new CombinedGeometry(GeometryCombineMode.Intersect, new LineGeometry(new Point(10, 1), new Point(100, 200)), new LineGeometry(new Point(-1, -1), new Point(0, 0)));
            Rect retval12 = cg12.Bounds;

            CombinedGeometry cg12Empty = new CombinedGeometry();
            Rect result12Empty = cg12Empty.Bounds;

            CombinedGeometry cg12Normal = new CombinedGeometry(GeometryCombineMode.Exclude,
                                                                new EllipseGeometry(new Point(30, 12), 100, 100),
                                                                new RectangleGeometry(new Rect(50, 50, 29, 30))
                                                              );
            Rect result12Normal = cg12Normal.Bounds;

            cg12Normal.Transform = new TranslateTransform(10, 10);
            Rect result12logterTransform = cg12Normal.Bounds;

            CommonLib.RectVerifier(retval12, Rect.Empty);
            CommonLib.RectVerifier(result12Empty, Rect.Empty);
            CommonLib.RectVerifier(result12Normal, new Rect(-70, -88, 200, 200));
            CommonLib.RectVerifier(result12logterTransform, new Rect(-60, -78, 200, 200));
            #endregion

            #region Test #13a: GeometryCombineMode property with basevalue
            CommonLib.LogStatus("Test #13a: GeometryCombineMode property with basevalue");
            CombinedGeometry cg13a = new CombinedGeometry();
            CommonLib.GenericVerifier(cg13a.GeometryCombineMode == GeometryCombineMode.Union, "GeometryCombineMode property with basevalue");
            #endregion

            #region Test #13b: GeometryCombineMode property in Invalid state
            CommonLib.LogStatus("Test #13b: GeometryCombineMode property in Invalid state");
            CombinedGeometry cg13b = new CombinedGeometry();
            cg13b.InvalidateProperty(CombinedGeometry.GeometryCombineModeProperty);
            CommonLib.GenericVerifier(cg13b.GeometryCombineMode == GeometryCombineMode.Union, "GeometryCombineMode property in Invalid state");
            #endregion

            #region Test #13c: CombinedMode property
            CombinedGeometry cg13c = new CombinedGeometry(GeometryCombineMode.Intersect, new LineGeometry(), new LineGeometry());
            cg13c.GeometryCombineMode = GeometryCombineMode.Exclude;
            CommonLib.GenericVerifier(cg13c.GeometryCombineMode == GeometryCombineMode.Exclude, "GeometryCombineMode property");
            #endregion

            //#region Test #8.5:  EndPointAnimations property
            //CommonLib.LogStatus("Test #8.5:  EndPointAnimations property");
            //lg2.BeginAnimation(LineGeometry.EndPointProperty, new PointAnimation(new Point(32, 10), new Point(100, 27.3), new Duration(TimeSpan.FromMilliseconds(1000))));
            //lg2.BeginAnimation(LineGeometry.EndPointProperty, new PointAnimation(new Point(10, 32.3), new Point(90.1, 32.123), new Duration(TimeSpan.FromMilliseconds(2000))));
            //Timeline endPointAnimation = lg2.BeginAnimation(LineGeometry.EndPointProperty, null);
            //CommonLib.GenericVerifier(endPointAnimation != null, "EndPointAnimation property");
            //#endregion

            #region Test #14a: Geometry1 property with basevalue
            CommonLib.LogStatus("Test #14a:  Geometry1 property with basevalue");
            CombinedGeometry cg14a = new CombinedGeometry();
            CommonLib.GenericVerifier(cg14a.Geometry1.Bounds.IsEmpty, "Geometry1 property with basevalue");
            #endregion

            #region Test #14b: Geometry1 property in Invalid state
            CommonLib.LogStatus("Test #14b: Geometry1 property in Invalid state");
            CombinedGeometry cg14b = new CombinedGeometry();
            cg14b.InvalidateProperty(CombinedGeometry.Geometry1Property);
            CommonLib.GenericVerifier(cg14b.Geometry1.Bounds.IsEmpty, "Geometry1 property in Invalid state");
            #endregion

            #region Test #14: Geometry1 property
            CommonLib.LogStatus("Test #14: Geometry1 property");
            CombinedGeometry cg14 = new CombinedGeometry();
            cg14.Geometry1 = new LineGeometry(new Point(3, 2), new Point(23, 44));
            CommonLib.GenericVerifier(cg14.Geometry1 is LineGeometry, "Geometry1 property");
            #endregion

            #region Test #15a: Geometry2 property with basevalue
            CommonLib.LogStatus("Test #15a:  Geometry2 property with basevalue");
            CombinedGeometry cg15a = new CombinedGeometry();
            CommonLib.GenericVerifier(cg15a.Geometry2.Bounds.IsEmpty, "Geometry2 property with basevalue");
            #endregion

            #region Test #15b: Geometry2 property in Invalid state
            CommonLib.LogStatus("Test #15b: Geometry2 property in Invalid state");
            CombinedGeometry cg15b = new CombinedGeometry();
            cg15b.InvalidateProperty(CombinedGeometry.Geometry1Property);
            CommonLib.GenericVerifier(cg15b.Geometry2.Bounds.IsEmpty, "Geometry2 property in Invalid state");
            #endregion

            #region Test #15: Geometry2 property
            CommonLib.LogStatus("Test #15: Geometry2 property");
            CombinedGeometry cg15 = new CombinedGeometry();
            cg15.Geometry2 = new LineGeometry(new Point(3, 2), new Point(23, 44));
            CommonLib.GenericVerifier(cg15.Geometry2 is LineGeometry, "Geometry2 property");
            #endregion

            #region Test #16:CanFreeze property
            CommonLib.LogStatus("Test #16: CanFreeze property");
            CombinedGeometry cg16 = new CombinedGeometry(new RectangleGeometry(new Rect(0, 0, 23, 3)), new RectangleGeometry(new Rect(2, 3, 30, 12)));
            CommonLib.GenericVerifier(cg16.CanFreeze, "CanFreeze property");
            #endregion

            #endregion

            #region Circular Test with GetArea() and Bounds
            CommonLib.LogStatus("Circular Test with GetArea() and Bounds");
            CommonLib.CircularTestGeometry(cg7);
            CommonLib.CircularTestGeometry(cg7Tran);
            #endregion

            #region Section IV: Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogTest("Test result for: " + objectType);

            DC.DrawGeometry(null, new Pen(new SolidColorBrush(Colors.Red), 1), cg4);
            #endregion
        }
    }
}