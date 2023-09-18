// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the Stretch enum
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_StretchEnum : ApiTest
    {
        public WCP_StretchEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.Stretch);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create an ImageBrush, a Pen and three Rects
            BitmapImage imgsrc = new BitmapImage(new Uri("S1.png", UriKind.RelativeOrAbsolute));
            ImageBrush brush = new ImageBrush(imgsrc);
            brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            brush.Viewport = new Rect(new Point(0.0, 0.0), new Point(1.0, 1.0));
            brush.TileMode = TileMode.None;
            Pen pen = new Pen(Brushes.White, 2.0);
            Rect rect1 = new Rect(new Point(3, 3), new Point(187, 35));
            Rect rect2 = new Rect(new Point(3, 40), new Point(187, 72));
            Rect rect3 = new Rect(new Point(3, 80), new Point(187, 112));
            Rect rect4 = new Rect(new Point(3, 120), new Point(187, 152));

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - None
            // Usage: Stretch.None
            // Notes: Value = int32(0x00000000)
            CommonLib.LogStatus("Test #1 - None");

            // Set the Stretch of an ImageBrush to None
            brush.Stretch = Stretch.None;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush, pen, rect1);

            // Check the Stretch value to assure that it is None.
            _class_testresult &= _helper.CompareProp("None", (int)Stretch.None, (int)brush.Stretch);
            #endregion

            #region Test #2 - Fill
            // Usage: Stretch.Fill
            // Notes: Value = int32(0x00000001)
            CommonLib.LogStatus("Test #2 - Fill");

            // Set the Stretch of an ImageBrush to Fill
            ImageBrush brush2 = brush.Clone();
            brush2.Stretch = Stretch.Fill;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush2, pen, rect2);

            // Check the Stretch value to assure that it is Fill.
            _class_testresult &= _helper.CompareProp("Fill", (int)Stretch.Fill, (int)brush2.Stretch);
            #endregion

            #region Test #3 - Uniform
            // Usage: Stretch.Uniform
            // Notes: Value = int32(0x00000002)
            CommonLib.LogStatus("Test #3 - Uniform");

            // Set the Stretch of an ImageBrush to Uniform
            ImageBrush brush3 = brush.Clone();
            brush3.Stretch = Stretch.Uniform;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush3, pen, rect3);

            // Check the Stretch value to assure that it is Uniform.
            _class_testresult &= _helper.CompareProp("Uniform", (int)Stretch.Uniform, (int)brush3.Stretch);
            #endregion

            #region Test #4 - UniformToFill
            // Usage: Stretch.UniformToFill
            // Notes: Value = int32(0x00000003)
            CommonLib.LogStatus("Test #4 - UniformToFill");

            // Set the Stretch of an ImageBrush to UniformToFill
            ImageBrush brush4 = brush.Clone();
            brush4.Stretch = Stretch.UniformToFill;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush4, pen, rect4);

            // Check the Stretch value to assure that it is UniformToFill.
            _class_testresult &= _helper.CompareProp("UniformToFill", (int)Stretch.UniformToFill, (int)brush4.Stretch);
            #endregion
            #endregion End Of SECTION I

            #region TEST LOGGING
            // Log the programmatic result for this API test using the
            // Automation Framework LogTest method.  If This result is False,
            // it will override the result of a Visual Comparator.  Conversely,
            // if a Visual Comparator is False it will override a True result
            // from this test.
            CommonLib.LogStatus("Logging the Test Result");
            CommonLib.LogTest("Result for: "+ (_objectType + " test: " ));
            #endregion End of TEST LOGGING
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
