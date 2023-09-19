// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//	GMC API Tests - Testing System.Windows.Shapes.Path class
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
    internal class PathShape : WCP_MILShapeAPITest
    {
        //--------------------------------------------------------------------
        public PathShape( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Shapes.Path);
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
            #region Default constructor
            CommonLib.LogStatus("Testing the default constructor");
            Path path1 = new Path();
            _class_testresult &= _helper.CheckType(path1, _objectType);
            #endregion
            #endregion

            #region Section II: public methods in Path class
            #region Data property
            CommonLib.LogStatus("Testing Data property");
            path1.Data = new RectangleGeometry(new Rect(new Point(2.2, 10.1), new Point(100, 89.11)), 6.4, 10);
            if (path1.Data != null && path1.Data is RectangleGeometry && ((RectangleGeometry)path1.Data).RadiusX == 6.4)
            {
                CommonLib.LogStatus("Pass: Data - Geometry has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: Data - Expected value 'RectangleGeometry object', Actaul value: '" + ((path1.Data == null) ? "NULL' " : path1.Data.ToString()));
                _class_testresult = false;
            }
            #endregion

            #endregion

            #region Section II: inherited properties from Shape class
            _class_testresult &= ShareTest.RunCommonTest(_helper, path1);
            #endregion

            CommonLib.LogTest("Result for :" + _objectType);
        }

        //--------------------------------------------------------------------------------
        #region private variables
        private bool _class_testresult;

        private Type _objectType;
        private HelperClass _helper;
        private double _w;
        private double _h;
        #endregion
    }
}
