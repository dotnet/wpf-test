// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the ColorInterpolationMode enum 
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_ColorInterpolationModeEnum : ApiTest
    {
        //--------------------------------------------------------------------

        public WCP_ColorInterpolationModeEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.ColorInterpolationMode);
            _helper = new HelperClass();
            Update();
        }

        //--------------------------------------------------------------------

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create a LinearGradientBrush and two Rects
            LinearGradientBrush brush = new LinearGradientBrush(Colors.Red, Colors.Blue, 0.0);
            brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            brush.StartPoint = new Point(0.25, 0.5);
            brush.EndPoint = new Point(0.75, 0.5);
            Rect rect1 = new Rect(new Point(20, 10), new Point(180, 80));
            Rect rect2 = new Rect(new Point(20, 90), new Point(180, 160));

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - ScRgbLinearInterpolation
            // Usage: ColorInterpolationMode.ScRgbLinearInterpolation
            // Notes: Value = int32(0x00000000)
            CommonLib.LogStatus("Test #1 - ScRgbLinearInterpolation");

            // Set the ColorInterpolationMode of a Brush to ScRgbLinearInterpolation
            brush.ColorInterpolationMode = ColorInterpolationMode.ScRgbLinearInterpolation;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush, null, rect1);

            // Check the ColorInterpolationMode value to assure that it is ScRgbLinearInterpolation.
            _class_testresult &= _helper.CompareProp("ScRgbLinearInterpolation", (int)ColorInterpolationMode.ScRgbLinearInterpolation, (int)brush.ColorInterpolationMode);
            #endregion

            #region Test #2 - SRgbLinearInterpolation
            // Usage: ColorInterpolationMode.SRgbLinearInterpolation
            // Notes: Value = int32(0x00000001)
            CommonLib.LogStatus("Test #2 - SRgbLinearInterpolation");

            // Set the ColorInterpolationMode of a Brush to SRgbLinearInterpolation
            LinearGradientBrush brush2 = brush.Clone();
            brush2.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush2, null, rect2);

            // Check the ColorInterpolationMode value to assure that it is SRgbLinearInterpolation.
            _class_testresult &= _helper.CompareProp("SRgbLinearInterpolation", (int)ColorInterpolationMode.SRgbLinearInterpolation, (int)brush2.ColorInterpolationMode);
            #endregion
            #endregion End Of SECTION I

            #region TEST LOGGING            
            CommonLib.LogTest("Result for :"+ _objectType );
            #endregion End of TEST LOGGING
        }

        //--------------------------------------------------------------------

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
