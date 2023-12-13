// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the GradientSpreadMethod enum
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_GradientSpreadMethodEnum : ApiTest
    {
        public WCP_GradientSpreadMethodEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.GradientSpreadMethod);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create a LinearGradientBrush and three Rects
            LinearGradientBrush brush = new LinearGradientBrush(Colors.Red, Colors.Blue, 0.0);
            brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            brush.StartPoint = new Point(0.25, 0.5);
            brush.EndPoint = new Point(0.75, 0.5);

            Rect rect1 = new Rect(new Point(20, 10), new Point(180, 50));
            Rect rect2 = new Rect(new Point(20, 60), new Point(180, 100));
            Rect rect3 = new Rect(new Point(20, 110), new Point(180, 150));

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - Pad
            // Usage: GradientSpreadMethod.Pad
            // Notes: Value = int32(0x00000000)  Pads the area up to the stops with solid color.
            CommonLib.LogStatus("Test #1 - Pad");

            // Set the SpreadMethod of a Gradient Brush to Pad
            brush.SpreadMethod = GradientSpreadMethod.Pad;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush, null, rect1);

            // Check the SpreadMethod value to assure that it is Pad.
            _class_testresult &= _helper.CompareProp("Pad", (int)GradientSpreadMethod.Pad, (int)brush.SpreadMethod);
            #endregion

            #region Test #2 - Reflect
            // Usage: GradientSpreadMethod.Reflect
            // Notes: Value = int32(0x00000001)  Reflects the GradientSpread of the first
            // half of the fill with the opposite fill in the second half.
            CommonLib.LogStatus("Test #2 - Reflect");

            // Set the SpreadMethod of a Gradient Brush to Reflect
            LinearGradientBrush brush2 = brush.Clone();
            brush2.SpreadMethod = GradientSpreadMethod.Reflect;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush2, null, rect2);

            // Check the SpreadMethod value to assure that it is Reflect.
            _class_testresult &= _helper.CompareProp("Reflect", (int)GradientSpreadMethod.Reflect, (int)brush2.SpreadMethod);
            #endregion

            #region Test #3 - Repeat
            // Usage: GradientSpreadMethod.Repeat
            // Notes: Value = int32(0x00000002)  Repeats the GradientSpread of the first
            // half of the fill.
            CommonLib.LogStatus("Test #3 - Repeat");

            // Set the SpreadMethod of a Gradient Brush to Repeat
            LinearGradientBrush brush3 = brush.Clone();
            brush3.SpreadMethod = GradientSpreadMethod.Repeat;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush3, null, rect3);

            // Check the SpreadMethod value to assure that it is Repeat.
            _class_testresult &= _helper.CompareProp("Repeat", (int)GradientSpreadMethod.Repeat, (int)brush3.SpreadMethod);
            #endregion
            #endregion End Of SECTION I

            #region TEST LOGGING
            // Log the programmatic result for this API test using the
            // Automation Framework LogTest method.  If This result is False,
            // it will override the result of a Visual Comparator.  Conversely,
            // if a Visual Comparator is False it will override a True result
            // from this test.
            CommonLib.LogTest("Result for :" + _objectType);
            #endregion End of TEST LOGGING
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
