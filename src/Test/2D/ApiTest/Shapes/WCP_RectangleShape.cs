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
    internal class RectangleShape : WCP_MILShapeAPITest
    {
        public RectangleShape( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Shapes.Rectangle);
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

            double rectWidth = 120;
            double rectHeight = 120;
            //double rectTop = 50;
            //double rectLeft = 50;
            double radiusX = 10;
            double radiusY = 10;

            #endregion

            #region Test - Default constructor
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            CommonLib.LogStatus("Test: Rectangle default constructor");

            // Create a Rectangle 
            Rectangle rect = new Rectangle();

            // Confirm that the right type of object was created.
            _class_testresult &= _helper.CheckType(rect, _objectType);

            #endregion

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");


            rect.Width = rectWidth;
            rect.Height = rectHeight;
            //rect.RectangleTop = rectTop;
            //rect.RectangleLeft = rectLeft;
            rect.RadiusX = radiusX;
            rect.RadiusY = radiusY;

            // compare the value of the property and log the results
            _class_testresult &= CheckProperty("RectangleWidth", rect.Width, rectWidth);
            _class_testresult &= CheckProperty("RectangleHeight", rect.Height, rectHeight);
            //class_testresult &= CheckProperty("RectangleTop", rect.RectangleTop, rectTop);
            //class_testresult &= CheckProperty("RectangleLeft", rect.RectangleLeft, rectLeft);
            _class_testresult &= CheckProperty("RadiusX", rect.RadiusX, radiusX);
            _class_testresult &= CheckProperty("RadiusY", rect.RadiusY, radiusY);

            CommonLib.LogStatus("Test RenderedGeometry property");
            Rectangle rectangle2 = new Rectangle();
            Geometry geo2 = rectangle2.RenderedGeometry;
            _class_testresult &= geo2.Bounds == Rect.Empty;

            CommonLib.LogStatus("Test GeometryTransform property");
            Rectangle rectangle3 = new Rectangle();
            _class_testresult &= rectangle3.GeometryTransform == Transform.Identity;

            #endregion // PROPERTIES

            #region Test Shape Properties

            _class_testresult &= ShareTest.RunCommonTest( _helper, rect);

            #endregion // Shape Properties

            CommonLib.LogTest("Result for: "+ _objectType);
        }

        private bool CheckProperty(string prop, double actual, double expected)
        {
            if (actual == expected)
            {
                CommonLib.LogStatus("Pass: " + prop + "=" + actual.ToString());
                return true;
            }

            CommonLib.LogFail("Fail: " + prop + " : Actual = " + actual.ToString() + " , Expected = " + expected.ToString());

            return false;
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
        private double _w;
        private double _h;

    }
}
