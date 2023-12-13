// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing GeometryGroup class
//  Author:   Microsoft
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for GeometryGroupClass.
    /// </summary>
    internal class GeometryGroupClass : ApiTest
    {
        public GeometryGroupClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("GeometryGroup Class");
            string objectType = "System.Windows.Media.GeometryGroup";

            #region Section I: Constructor
            #region Test #1: default constructor
            CommonLib.LogStatus("Test #1: default constructor");
            GeometryGroup gg1 = new GeometryGroup();
            CommonLib.TypeVerifier(gg1, objectType);
            #endregion
            #endregion

            #region Section II: Properties
            #region Test #2: Bounds property with empty GeometryGroup
            CommonLib.LogStatus("Test #2: Bounds property with empty GeometryGroup");
            GeometryGroup gg2 = new GeometryGroup();
            Rect r2 = gg2.Bounds;
            CommonLib.RectVerifier(r2, Rect.Empty);
            #endregion

            #region Test #3: Bounds property with GeometryGroup with data
            CommonLib.LogStatus("Test #3: Bounds property with GeometryGroup with data");
            GeometryGroup gg3 = new GeometryGroup();
            gg3.Children.Add(new LineGeometry(new Point(12, 33), new Point(100, 330)));
            gg3.Children.Add(new RectangleGeometry(new Rect(0, -100.32, 3.23, 10.32)));
            Rect r3 = gg3.Bounds;
            CommonLib.RectVerifier(r3, new Rect(0, -100.32, 100, 430.32));
            #endregion

            #region Test #4: Children property
            CommonLib.LogStatus("Test #4: Children property ");
            GeometryGroup gg4 = new GeometryGroup();
            gg4.Children.Add(new LineGeometry(new Point(12, 33), new Point(100, 330)));
            gg4.Children.Add(new RectangleGeometry(new Rect(0, -100.32, 100, 430.32)));
            GeometryCollection gc4 = gg4.Children;
            CommonLib.GenericVerifier(gc4.Count == 2, "Children property");
            #endregion

            #region Test #5: FillRule property
            CommonLib.LogStatus("Test #5: FillRule property");
            GeometryGroup gg5 = new GeometryGroup();
            gg5.FillRule = FillRule.Nonzero;
            CommonLib.GenericVerifier(gg5.FillRule == FillRule.Nonzero, "FillRule property");
            #endregion
            #endregion

            #region Section III: Methods
            #region Test #8: Contains(Point, double, ToleranceType)
            CommonLib.LogStatus("Test #8: Contains(Point, double, ToleranceType)");
            GeometryGroup gg8 = new GeometryGroup();
            gg8.Children.Add(new LineGeometry(new Point(12, 33), new Point(100, 330)));
            gg8.Children.Add(new RectangleGeometry(new Rect(0, 0, 3.23, 10.32)));
            //The point(12,33) is not really on the gg8, but with the Tolerance=2 and ToleranceType=Relative
            //Contains(...) should still return true
            bool result8_1 = gg8.FillContains(new Point(11, 33), 2, ToleranceType.Relative);

            //Even though the Tolerance=3, the area that considered contained will be a rounded Rectangle around
            //the RectangleGeometry, (new Rect(0 - 3, 0 - 3, 3.23 + 2 * 3, 10.32 + 2 * 3), 3, 3)
            //So Point(-3, -3) is actually not on this area, and Contains(...) should return false.
            bool result8_2 = gg8.FillContains(new Point(-3, -3), 3, ToleranceType.Absolute);

            CommonLib.GenericVerifier(result8_1 == true && result8_2 == false, "Contains(Point, double, ToleranceType)");
            #endregion

            #region Test #9: Contains(Pen, hitPoint, tolerance, type)
            CommonLib.LogStatus("Test #9: Contains(Pen, hitPoint, tolerance, type)");
            GeometryGroup gg9 = new GeometryGroup();
            gg9.Children.Add(new EllipseGeometry(new Point(100, 100.32), 30.23, 2));
            //The Point(100, 104.32) fall at the stroke of the gg9, since this Contains takes Pen into consideration,
            //the result will be true.
            bool result9_1 = gg9.StrokeContains(new Pen(Brushes.Black, 5), new Point(100, 104.32), 0, ToleranceType.Absolute);
            CommonLib.GenericVerifier(result9_1 == true, "Contains(Pen, hitPoint, tolerance, type");
            #endregion

            #region Test #10: Copy()
            CommonLib.LogStatus("Test #10: Copy()");
            GeometryGroup gg10 = new GeometryGroup();
            gg10.Children.Add(new EllipseGeometry(new Point(100, 100), 29, 32));
            GeometryGroup gg10Result = gg10.Clone();
            //gg10Result should contain one child and the child is EllipseGeometry
            CommonLib.GenericVerifier(gg10Result.Children.Count == 1 && gg10Result.Children[0] is EllipseGeometry, "Copy()");
            #endregion

            #region Test #11: Copy() with invalidated field
            CommonLib.LogStatus("Test #11: Copy() with invalidated field");
            GeometryGroup gg11 = new GeometryGroup();
            gg11.Children.Add(new EllipseGeometry(new Point(100, 100), 29, 32));
            gg11.InvalidateProperty(GeometryGroup.ChildrenProperty);
            GeometryGroup gg11Result = gg11.Clone();
            //gg11Result should contain one child and the child is EllipseGeometry
            CommonLib.GenericVerifier(gg11Result.Children.Count == 1 && gg11Result.Children[0] is EllipseGeometry, "Copy() with invalidated field");
            #endregion

            #region Test #12: GetArea(double, ToleranceType)
            CommonLib.LogStatus("Test #12: GetArea()");
            GeometryGroup gg12 = new GeometryGroup();
            gg12.Children.Add(new EllipseGeometry(new Point(50, 50), 50, 50));
            double gg12Result_Absolute = gg12.GetArea(0, ToleranceType.Absolute);
            double gg12Result_Relative = gg12.GetArea(10, ToleranceType.Relative);
            CommonLib.GenericVerifier(Math.Round(gg12Result_Absolute, 2) == 7856.18 && Math.Round(gg12Result_Relative, 2) == 5000.00, "GetArea()");
            #endregion

            #region Test #13: CloneCurrentValue
            CommonLib.LogStatus("Test #13: CloneCurrentValue");
            GeometryGroup gg13 = new GeometryGroup();
            gg13.Children.Add(new RectangleGeometry(new Rect(38, 12, 33, 100)));
            GeometryGroup gg13_result = gg13.CloneCurrentValue();
            CommonLib.GenericVerifier(gg13_result != null && gg13_result.Children.Count == 1, "CloneCurrentValue()");
            #endregion

            #region Test #14: IsEmpty()
            CommonLib.LogStatus("Test #14: IsEmpty()");
            GeometryGroup gg14 = new GeometryGroup();
            //The gg14 is empty right now, so IsEmpty should be true
            bool gg14_result_1 = gg14.IsEmpty();

            gg14.Children.Add(new LineGeometry(new Point(9, 23), new Point(32, 100)));
            //Now gg14 contains a LineGeometry, so IsEmpty should be false
            bool gg14_result_2 = gg14.IsEmpty();

            gg14.Children.Clear();

            gg14.Children.Add(new GeometryGroup());
            //Now gg14 contains an empty GroupGeometry, IsEmpty should be true.
            bool gg14_result_3 = gg14.IsEmpty();

            CommonLib.GenericVerifier(gg14_result_1 == true && gg14_result_2 == false && gg14_result_3 == true, "IsEmpty()");
            #endregion

            #region Test #15: MayHaveCurves()
            CommonLib.LogStatus("Test #15: MayHaveCurves()");
            GeometryGroup gg15 = new GeometryGroup();
            //gg15 is empty, so MayHaveCurves() should return false;
            bool gg15_result_1 = gg15.MayHaveCurves();

            gg15.Children.Add(new RectangleGeometry(new Rect(10, 32, 23, 10), 2, 3));
            //gg15 contains a rounded RectangleGeometry, so MayHaveCurves() should return true
            bool gg15_result_2 = gg15.MayHaveCurves();

            gg15.Children.Clear();
            gg15.Children.Add(new RectangleGeometry(new Rect(10, 32, 23, 1)));
            //gg15 only contains a rectanglegeoemtry, so MayHaveCurves() should return false
            bool gg15_result_3 = gg15.MayHaveCurves();
            CommonLib.GenericVerifier(gg15_result_1 == false && gg15_result_2 == true && gg15_result_3 == false, "MayHaveCurves()");
            #endregion

            #endregion

            #region IV:  Regression cases
            #region Regression for Regression_Bug6
            CommonLib.LogStatus("Regression for Regression_Bug6");
            RectangleGeometry rg_1 = new RectangleGeometry(new Rect(0, 0, 10, 20));
            RectangleGeometry rg_2 = new RectangleGeometry(new Rect(5, 0, 10, 20));
            GeometryGroup g1186685 = new GeometryGroup();
            g1186685.Children.Add(rg_1);
            g1186685.Children.Add(rg_2);
            g1186685.FillRule = FillRule.EvenOdd;
            double areaEvenOdd = g1186685.GetArea();

            g1186685.FillRule = FillRule.Nonzero;
            double areaNonzero = g1186685.GetArea();

            if (areaEvenOdd == areaNonzero)
            {
                CommonLib.LogFail("Regression_Bug6 regressed again");

            }
            else
            {
                CommonLib.LogStatus("Regression_Bug6 is still fixed!");
            }

            #endregion

            #endregion
            CommonLib.LogTest("Results for:"+ objectType);
        }
    }
}