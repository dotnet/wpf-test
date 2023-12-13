// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression Test 



using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_Regression_Bug1 : ApiTest
    {
        public WCP_Regression_Bug1( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _testname = "Regression_Bug1: ImageBrush:  Image DPI isn't taken into account when drawing";
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_testname);

            // Create string variables for all Image names at the top of this method for easier test maintenance
            String img96dpi = "Tulip.jpg";
            String img48dpi = "Tulip48dpi.jpg";

            // Create two ImageSources
            BitmapImage imgsrc96dpi = new BitmapImage(new Uri(img96dpi, UriKind.RelativeOrAbsolute));
            BitmapImage imgsrc48dpi = new BitmapImage(new Uri(img48dpi, UriKind.RelativeOrAbsolute));

            // Create four Rects
            Rect TopLeft = new Rect(new Point(10, 10), new Point(210, 210));
            Rect TopRight = new Rect(new Point(220, 10), new Point(420, 210));
            Rect BottomLeft = new Rect(new Point(10, 220), new Point(210, 420));
            Rect BottomRight = new Rect(new Point(220, 220), new Point(420, 420));

            #region TopLeft - 48dpi Image with Stretch = None
            CommonLib.LogStatus("TopLeft - 48dpi Image with Stretch = None");

            ImageBrush imb1 = new ImageBrush(imgsrc48dpi);
            imb1.AlignmentX = AlignmentX.Left;
            imb1.AlignmentY = AlignmentY.Top;
            imb1.Stretch = Stretch.None;

            DC.DrawRectangle(imb1, null, TopLeft);
            #endregion

            #region TopRight - 48dpi Image with Stretch = Fill
            CommonLib.LogStatus("TopRight - 48dpi Image with Stretch = Fill");

            ImageBrush imb2 = new ImageBrush(imgsrc48dpi);
            imb2.AlignmentX = AlignmentX.Left;
            imb2.AlignmentY = AlignmentY.Top;
            imb2.Stretch = Stretch.Fill;

            DC.DrawRectangle(imb2, null, TopRight);
            #endregion

            #region BottomLeft - 96dpi Image with Stretch = None
            CommonLib.LogStatus("BottomLeft - 96dpi Image with Stretch = None");

            ImageBrush imb3 = new ImageBrush(imgsrc96dpi);
            imb3.AlignmentX = AlignmentX.Left;
            imb3.AlignmentY = AlignmentY.Top;
            imb3.Stretch = Stretch.None;

            DC.DrawRectangle(imb3, null, BottomLeft);
            #endregion

            #region BottomRight - 96dpi Image with Stretch = Fill
            CommonLib.LogStatus("BottomRight - 96dpi Image with Stretch = Fill");

            ImageBrush imb4 = new ImageBrush(imgsrc96dpi);
            imb4.AlignmentX = AlignmentX.Left;
            imb4.AlignmentY = AlignmentY.Top;
            imb4.Stretch = Stretch.Fill;

            DC.DrawRectangle(imb4, null, BottomRight);
            #endregion

            CommonLib.LogTest("Result for:" + _testname);
        }

        private String _testname;
        private HelperClass _helper;
    }
}
