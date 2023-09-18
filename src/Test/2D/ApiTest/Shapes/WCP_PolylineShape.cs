// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  GMC API Tests - Testing System.Windows.Shapes.Shape class
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class PolylineShape : WCP_MILShapeAPITest
    {
        //--------------------------------------------------------------------
        public PolylineShape( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Shapes.Polyline);
            _helper = new HelperClass();
            _w = width;
            _h = height;
            RunTest();
        }

        private void RunTest()
        {
            CommonLib.LogStatus(_objectType.ToString());

            #region Initialization stage
            CommonLib.Stage = TestStage.Initialize;
            FillRule polyFillRule = FillRule.EvenOdd;

            Point[] points = new Point[5];
            points[0] = new Point(0, 0);
            points[1] = new Point(10, 10);
            points[2] = new Point(20, 20);
            points[3] = new Point(30, 30);
            points[4] = new Point(40, 40);

            PointCollection polyPointCollection = new PointCollection();
            foreach (Point p in points)
            {
                polyPointCollection.Add(p);
            }

            #endregion

            #region Test - Default constructor
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            CommonLib.LogStatus("Test: Polyline default constructor");

            // Create a Polyline 
            Polyline poly = new Polyline();

            // Confirm that the right type of object was created.
            _class_testresult &= _helper.CheckType(poly, _objectType);

            #endregion


            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");


            poly.FillRule = polyFillRule;
            poly.Points = polyPointCollection;

            // compare the value of the property and log the results
            if (poly.FillRule == FillRule.EvenOdd)
            {
                CommonLib.LogStatus("Pass: FillRule =" + poly.FillRule.ToString());
                _class_testresult &= true;
            }
            else
            {
                CommonLib.LogStatus("Fail : FillRule : Actual = " + poly.FillRule.ToString() + " , Expected = " + FillRule.EvenOdd.ToString());
                _class_testresult &= false;
            }

            // Check Point Collection
            foreach (Point pt in poly.Points)
                _class_testresult &= CheckPoint(pt, points);

            #endregion // PROPERTIES

            #region Test Shape Properties

            _class_testresult &= ShareTest.RunCommonTest(_helper, poly);

            #endregion // Shape Properties

            CommonLib.LogTest("Result for :" + _objectType);
        }

        private bool CheckPoint(Point pt, Point[] pts)
        {
            foreach (Point exp in pts)
            {
                if (pt.X == exp.X && pt.Y == exp.Y)
                {
                    CommonLib.LogStatus("Pass: Point = ( " + pt.X + " , " + pt.Y + " ) found ");
                    return true;
                }
            }

            CommonLib.LogFail("Fail: Point = ( " + pt.X + " , " + pt.Y + " ) not found ");
            return false;
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
        private double _w;
        private double _h;

    }
}
