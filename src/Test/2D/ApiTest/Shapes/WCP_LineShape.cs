// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing System.Windows.Shapes.Line class
//	Author:  Microsoft
//
using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Graphics
{
    internal class LineShape : WCP_MILShapeAPITest
    {
        public LineShape( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Shapes.Line);
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

            #endregion

            #region Section I: Constructos
            #region Test #1: Default constructor
            CommonLib.LogStatus("Test #1: Default constructor");

            Line line1 = new Line();

            _class_testresult &= _helper.CheckType(line1, _objectType);
            #endregion

            #endregion

            #region Section II: Properties and Methods defined in Line class
            #region Test #3: X1 property
            CommonLib.LogStatus("Test #3: X1 Property");
            line1.X1 = 10.03;
            _class_testresult &= (line1.X1 == 10.03) ? true : false;
            if (_class_testresult)
            {
                CommonLib.LogStatus("X1 property passed the test");
            }
            else
            {
                CommonLib.LogStatus("X1 property failed the test");
            }
            #endregion

            #region Test #4: X2 property
            CommonLib.LogStatus("Test #4: X2 property");
            line1.X2 = 20.132;
            _class_testresult &= (line1.X2 == 20.132) ? true : false;
            if (_class_testresult)
            {
                CommonLib.LogStatus("X2 property passed the test");
            }
            else
            {
                CommonLib.LogStatus("X2 property failed the test");
            }
            #endregion

            #region Test #5: Y1 property
            CommonLib.LogStatus("Test #5: Y1 property");
            line1.Y1 = 0.33;
            _class_testresult &= (line1.Y1 == 0.33) ? true : false;
            if (_class_testresult)
            {
                CommonLib.LogStatus("Y1 property passed the test");
            }
            else
            {
                CommonLib.LogStatus("Y1 property failed the test");
            }
            #endregion

            #region Test #6: Y2 property
            CommonLib.LogStatus("Test #6: Y2 property");
            line1.Y2 = 10.3;
            _class_testresult &= (line1.Y2 == 10.3) ? true : false;
            if (_class_testresult)
            {
                CommonLib.LogStatus("Y2 property passed the test");
            }
            else
            {
                CommonLib.LogStatus("Y2 property failed the test");
            }
            #endregion

            #region Test RenderedGeometry
            CommonLib.LogStatus("Test RenderedGeometry property");
            Line line2 = new Line();
            _class_testresult &= line2.RenderedGeometry is StreamGeometry &&
                            line2.RenderedGeometry.Bounds == Rect.Empty;

            Line line3 = new Line();
            line3.X1 = 10;
            line3.Y1 = 10;
            line3.X2 = 100;
            line3.Y2 = 0;
            ShareTest.ForceRender(line3);
            LineGeometry lg3 = line3.RenderedGeometry as LineGeometry;
            _class_testresult &= (lg3.StartPoint == new Point(line3.X1, line3.Y1) &&
                                    lg3.EndPoint == new Point(line3.X2, line3.Y2));
            #endregion

            #region Test GeometryTransform
            CommonLib.LogStatus("Test GeometryTransform property");
            Line line4 = new Line();
            line4.X1 = 0;
            line4.Y1 = 0;
            line4.X2 = 50;
            line4.Y2 = 50;
            line4.Width = 100;
            line4.Height = 100;

            line4.Stroke = Brushes.Black;
            line4.StrokeThickness = 1;

            ShareTest.ForceRender(line4);

            Transform transform4 = line4.GeometryTransform;
            _class_testresult &= transform4 == Transform.Identity;

            line4.Stretch = Stretch.Fill;
            ShareTest.ForceRender(line4);

            Transform transform4_NonIdentity = line4.GeometryTransform;
            _class_testresult &= _helper.CompareMatrix("GeometryTransform", transform4_NonIdentity, 1.98, 0, 0, 1.98, 0.5, 0.5);
            #endregion
            #endregion

            #region Section III: inherited Properties from Shape base class
            _class_testresult &= ShareTest.RunCommonTest( _helper, line1);
            #endregion

            CommonLib.LogTest("Result for: " + _objectType);
        }

        #region private variables
        private bool _class_testresult;

        private Type _objectType;
        private HelperClass _helper;
        private double _w;
        private double _h;
        #endregion
    }
}
