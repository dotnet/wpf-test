// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression Test 


using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class Regression_Bug21 : ApiTest
    {
        //--------------------------------------------------------------------
        public Regression_Bug21( double left, double top,
            double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _helper = new HelperClass();
            Update();
        }
        protected override void OnRender(DrawingContext DC)
        {
            #region Initialization stage
            CommonLib.Stage = TestStage.Initialize;
            #endregion

            #region Running stage
            CommonLib.Stage = TestStage.Run;

            #region Regression Test for Regression_Bug21
            // for now, just make sure that it doesn't crash or hit assertion fail.
            DC.PushClip(new RectangleGeometry(new Rect(100, 100, 50, 50)));
            Pen pen = new Pen(new SolidColorBrush(Colors.Yellow), 10);
            DC.DrawRectangle(new SolidColorBrush(Colors.Red), pen, new Rect(100, 100, 200, 210));
            DC.Pop();
            CommonLib.LogStatus("Pass: doesn't crash/hit assertion fail");
            _class_testresult &= true;
            #endregion

            #endregion // Running stage

            CommonLib.LogTest("Result for: " + _class_testresult);
        }

        private bool _class_testresult;
        private HelperClass _helper;
    }
}

