// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the BrushMappingMode enum
//
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_BrushMappingModeEnum : ApiTest
    {
        public WCP_BrushMappingModeEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.BrushMappingMode);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create a LinearGradientBrush and two Rects
            LinearGradientBrush brush = new LinearGradientBrush(Colors.Yellow, Colors.Blue, 0.0);
            Rect rect1 = new Rect(new Point(20, 10), new Point(180, 78));
            Rect rect2 = new Rect(new Point(20, 92), new Point(180, 160));

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - Absolute
            // Usage: BrushMappingMode.Absolute
            // Notes: Value = int32(0x00000000)  The Brush will map to the coordinates of
            // the entire user space. Uses a 0 - X value relative to the space being filled.
            CommonLib.LogStatus("Test #1 - Absolute");

            // Set the BrushMappingMode of a Brush to Absolute
            brush.MappingMode = BrushMappingMode.Absolute;

            // Set the Start and End Points of the Brush to Absolute coordinates
            brush.StartPoint = new Point(60, 44);
            brush.EndPoint = new Point(140, 44);

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush, null, rect1);

            // Check the MappingMode value to assure that it is Absolute.
            _class_testresult &= _helper.CompareProp("Absolute", (int)BrushMappingMode.Absolute, (int)brush.MappingMode);
            #endregion

            #region Test #2 - RelativeToBoundingBox
            // Usage: BrushMappingMode.RelativeToBoundingBox
            // Notes: Value = int32(0x00000001)  The Brush will map to the coordinates of a
            // shape that is being filled.  Uses a 0-1 scale relative to the bounding box
            // of the shape.
            CommonLib.LogStatus("Test #2 - RelativeToBoundingBox");

            // Set the BrushMappingMode of a Brush to RelativeToBoundingBox
            brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;

            // Set the Start and End Points of the Brush to RelativeToBoundingBox coordinates
            brush.StartPoint = new Point(0.25, 0.5);
            brush.EndPoint = new Point(0.75, 0.5);

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush, null, rect2);

            // Check the MappingMode value to assure that it is RelativeToBoundingBox.
            _class_testresult &= _helper.CompareProp("RelativeToBoundingBox", (int)BrushMappingMode.RelativeToBoundingBox, (int)brush.MappingMode);
            #endregion
            #endregion End Of SECTION I

            CommonLib.LogTest("Result for: " + _objectType);
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
