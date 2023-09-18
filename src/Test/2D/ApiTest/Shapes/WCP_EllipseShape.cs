// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing System.Windows.Shapes.Ellipse class
//	Author:  Microsoft
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class EllipseShape : WCP_MILShapeAPITest
    {
        public EllipseShape( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Shapes.Ellipse);
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

            #region Section I: Constructors
            #region default constructor
            CommonLib.LogStatus("Testing default constructor");

            Ellipse ellipse1 = new Ellipse();

            _class_testresult &= _helper.CheckType(ellipse1, _objectType);
            #endregion

            #endregion

            #region Section III: inherited Properties from Shape base class
            _class_testresult &= ShareTest.RunCommonTest(_helper, ellipse1);

            #region Test RenderedGeometry
            CommonLib.LogStatus("Test RenderedGeometry property");
            Ellipse ellipse2 = new Ellipse();
            Geometry geo2 = ellipse2.RenderedGeometry;
            _class_testresult &= geo2 == Geometry.Empty;
            #endregion

            #region Test GeometryTransform
            CommonLib.LogStatus("Test GeometryTransform property");
            Ellipse ellipse3 = new Ellipse();
            _class_testresult &= ellipse3.GeometryTransform == Transform.Identity;
            #endregion
            #endregion

            CommonLib.LogTest("Result for :" + _objectType);
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
