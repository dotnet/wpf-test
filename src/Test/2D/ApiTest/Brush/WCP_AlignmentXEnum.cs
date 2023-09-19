// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the AlignmentX enum
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_AlignmentXEnum : ApiTest
    {
        //--------------------------------------------------------------------

        public WCP_AlignmentXEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _objectType = typeof(AlignmentX);
            _helper = new HelperClass();
            Update();
        }

        //--------------------------------------------------------------------

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create an ImageBrush, a Pen and three Rects
            BitmapImage imgsrc = new BitmapImage(new Uri("blue.jpg", UriKind.RelativeOrAbsolute));
            ImageBrush brush = new ImageBrush(imgsrc);
            brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            brush.Viewport = new Rect(new Point(0.0, 0.0), new Point(1.0, 1.0));
            brush.TileMode = TileMode.None;
            brush.Stretch = Stretch.None;
            Pen pen = new Pen(Brushes.White, 2.0);
            Rect rect1 = new Rect(new Point(20, 10), new Point(180, 50));
            Rect rect2 = new Rect(new Point(20, 60), new Point(180, 100));
            Rect rect3 = new Rect(new Point(20, 110), new Point(180, 150));

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - Left
            // Usage: AlignmentX.Left
            // Notes: Value = int32(0x00000000)
            CommonLib.LogStatus("Test #1 - Left");

            // Set the AlignmentX of an ImageBrush to Left
            brush.AlignmentX = AlignmentX.Left;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush, pen, rect1);

            // Check the AlignmentX value to assure that it is Left.
            _class_testresult &= _helper.CompareProp("Left", (int)AlignmentX.Left, (int)brush.AlignmentX);
            #endregion

            #region Test #2 - Center
            // Usage: AlignmentX.Center
            // Notes: Value = int32(0x00000001)
            CommonLib.LogStatus("Test #2 - Center");

            // Set the AlignmentX of an ImageBrush to Center
            ImageBrush brush2 = brush.Clone();
            brush2.AlignmentX = AlignmentX.Center;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush2, pen, rect2);

            // Check the AlignmentX value to assure that it is Center.
            _class_testresult &= _helper.CompareProp("Center", (int)AlignmentX.Center, (int)brush2.AlignmentX);
            #endregion

            #region Test #3 - Right
            // Usage: AlignmentX.Right
            // Notes: Value = int32(0x00000002)
            CommonLib.LogStatus("Test #3 - Right");

            // Set the AlignmentX of an ImageBrush to Right
            ImageBrush brush3 = brush.Clone();
            brush3.AlignmentX = AlignmentX.Right;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush3, pen, rect3);

            // Check the AlignmentX value to assure that it is Right.
            _class_testresult &= _helper.CompareProp("Right", (int)AlignmentX.Right, (int)brush3.AlignmentX);
            #endregion
            #endregion End Of SECTION I
            
            CommonLib.LogTest("Result for :"+_objectType );
        }
        private Type _objectType;
        private bool _class_testresult;
        private HelperClass _helper;
    }
}
