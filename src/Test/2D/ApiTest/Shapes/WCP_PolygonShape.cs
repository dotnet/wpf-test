// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing System.Windows.Shapes.Polygon class
//  Author:  Microsoft
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
    internal class PolygonShape : WCP_MILShapeAPITest
    {
        public PolygonShape( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Shapes.Polygon);
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

            #region Constructors
            #region default constructor
            CommonLib.LogStatus("Testing the default constructor");

            Polygon polygon1 = new Polygon();

            _class_testresult &= _helper.CheckType(polygon1, _objectType);
            #endregion

            #endregion

            #region public properties in Polygon class
            #region FillRule property
            CommonLib.LogStatus("Testing FillRule property");
            polygon1.FillRule = FillRule.Nonzero;
            if (polygon1.FillRule == FillRule.Nonzero)
            {
                CommonLib.LogStatus("Pass: Polygon - FillRule has the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: Polygon - Expected value: 'NonZero', Actual value: '" + polygon1.FillRule.ToString() + "'");
                _class_testresult = false;
            }
            #endregion

            #region Points property
            CommonLib.LogStatus("Testing Points property");
            PointCollection pc1 = new PointCollection();
            pc1.Add(new Point(18.1, 32.2));
            pc1.Add(new Point(10, 100));
            pc1.Add(new Point(50, 90));
            polygon1.Points = pc1;
            if (polygon1.Points.Count == 3 && polygon1.Points[1].X == 10 && polygon1.Points[1].Y == 100)
            {
                CommonLib.LogStatus("Pass: Points - PointsCollection contains the expected value");
            }
            else
            {
                CommonLib.LogFail("Fail: Points - doesn't have the expected value");
            }
            #endregion
            #endregion

            #region Section III: inherited Properties from Shape base class
            _class_testresult &= ShareTest.RunCommonTest(_helper, polygon1);
            #endregion

            CommonLib.LogTest("Results for :" + _objectType);
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
