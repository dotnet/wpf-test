// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the AlignmentY enum
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_AlignmentYEnum : ApiTest
    {

        public WCP_AlignmentYEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(AlignmentY);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Create an ImageBrush, a Pen and three Rects
            BitmapImage imgsrc = new BitmapImage(new Uri("blue.jpg", UriKind.RelativeOrAbsolute));
            ImageBrush brush = new ImageBrush(imgsrc);
            brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            brush.Viewport = new Rect(new Point(0.0, 0.0), new Point(1.0, 1.0));
            brush.TileMode = TileMode.None;
            brush.Stretch = Stretch.Uniform;
            Pen pen = new Pen(Brushes.White, 2.0);
            Rect rect1 = new Rect(new Point(10, 10), new Point(50, 160));
            Rect rect2 = new Rect(new Point(60, 10), new Point(100, 160));
            Rect rect3 = new Rect(new Point(110, 10), new Point(150, 160));

            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - Top
            // Usage: AlignmentY.Top
            // Notes: Value = int32(0x00000000)
            CommonLib.LogStatus("Test #1 - Top");

            // Set the AlignmentY of an ImageBrush to Top
            brush.AlignmentY = AlignmentY.Top;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush, pen, rect1);

            // Check the AlignmentY value to assure that it is Top.
            _class_testresult &= _helper.CompareProp("Top", (int)AlignmentY.Top, (int)brush.AlignmentY);
            #endregion

            #region Test #2 - Center
            // Usage: AlignmentY.Center
            // Notes: Value = int32(0x00000001)
            CommonLib.LogStatus("Test #2 - Center");

            // Set the AlignmentY of an ImageBrush to Center
            ImageBrush brush2 = brush.Clone();
            brush2.AlignmentY = AlignmentY.Center;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush2, pen, rect2);

            // Check the AlignmentY value to assure that it is Center.
            _class_testresult &= _helper.CompareProp("Center", (int)AlignmentY.Center, (int)brush2.AlignmentY);
            #endregion

            #region Test #3 - Bottom
            // Usage: AlignmentY.Bottom
            // Notes: Value = int32(0x00000002)
            CommonLib.LogStatus("Test #3 - Bottom");

            // Set the AlignmentY of an ImageBrush to Bottom
            ImageBrush brush3 = brush.Clone();
            brush3.AlignmentY = AlignmentY.Bottom;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush3, pen, rect3);

            // Check the AlignmentY value to assure that it is Bottom.
            _class_testresult &= _helper.CompareProp("Bottom", (int)AlignmentY.Bottom, (int)brush3.AlignmentY);
            #endregion
            #endregion End Of SECTION I

            CommonLib.LogTest("Test result for: "+_class_testresult );
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
