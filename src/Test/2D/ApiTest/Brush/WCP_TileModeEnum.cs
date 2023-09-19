// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the TileMode enum
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_TileModeEnum : ApiTest
    {
        public WCP_TileModeEnum( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(System.Windows.Media.TileMode);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            Pen pen = new Pen(Brushes.Black, 2.0);
            DrawingBrush brush = new DrawingBrush();
            DrawingGroup DG = new DrawingGroup();
            DrawingContext dctx = DG.Open();
            dctx.DrawGeometry(Brushes.Blue, null, new RectangleGeometry(new Rect(0, 0, 50, 50)));
            dctx.DrawGeometry(Brushes.White, null, new RectangleGeometry(new Rect(50, 0, 50, 50)));
            dctx.DrawGeometry(Brushes.White, null, new RectangleGeometry(new Rect(0, 50, 50, 50)));
            dctx.DrawGeometry(Brushes.Red, null, new RectangleGeometry(new Rect(50, 50, 50, 50)));
            dctx.Close();

            brush.Drawing = DG;
            brush.Viewport = new Rect(0, 0, 0.5, 0.5);
            brush.Stretch = Stretch.Fill;
            brush.AlignmentX = AlignmentX.Left;
            brush.AlignmentY = AlignmentY.Top;

            Rect rect1 = new Rect(0, 0, 100, 100);
            Rect rect2 = new Rect(0, 100, 100, 100);
            Rect rect3 = new Rect(100, 0, 100, 100);
            Rect rect4 = new Rect(100, 100, 100, 50);
            Rect rect5 = new Rect(100, 150, 100, 50);


            #region SECTION I - ENUMERATIONS
            CommonLib.LogStatus("***** SECTION I - ENUMERATIONS *****");

            #region Test #1 - FlipX
            // Usage: TileMode.FlipX
            // Notes: Value = int32(0x00000001)
            CommonLib.LogStatus("Test #1 - FlipX");

            // Set the TileMode of a DrawingBrush to FlipX
            brush.TileMode = TileMode.FlipX;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush, pen, rect1);

            // Check the TileMode value to assure that it is FlipX.
            _class_testresult &= _helper.CompareProp("FlipX", (int)TileMode.FlipX, (int)brush.TileMode);
            #endregion

            #region Test #2 - FlipY
            // Usage: TileMode.FlipY
            // Notes: Value = int32(0x00000002)
            CommonLib.LogStatus("Test #2 - FlipY");

            // Set the TileMode of a DrawingBrush to FlipY
            DrawingBrush brush2 = brush.Clone();
            brush2.TileMode = TileMode.FlipY;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush2, pen, rect2);

            // Check the TileMode value to assure that it is FlipY.
            _class_testresult &= _helper.CompareProp("FlipY", (int)TileMode.FlipY, (int)brush2.TileMode);
            #endregion

            #region Test #3 - FlipXY
            // Usage: TileMode.FlipXY
            // Notes: Value = int32(0x00000003)
            CommonLib.LogStatus("Test #3 - FlipXY");

            // Set the TileMode of a DrawingBrush to FlipXY
            DrawingBrush brush3 = brush.Clone();
            brush3.TileMode = TileMode.FlipXY;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush3, pen, rect3);

            // Check the TileMode value to assure that it is FlipXY.
            _class_testresult &= _helper.CompareProp("FlipXY", (int)TileMode.FlipXY, (int)brush3.TileMode);
            #endregion

            #region Test #4 - Tile
            // Usage: TileMode.Tile
            // Notes: Value = int32(0x00000004)
            CommonLib.LogStatus("Test #4 - Tile");

            // Set the TileMode of a DrawingBrush to Tile
            DrawingBrush brush4 = brush.Clone();
            brush4.TileMode = TileMode.Tile;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush4, pen, rect4);

            // Check the TileMode value to assure that it is Tile.
            _class_testresult &= _helper.CompareProp("Tile", (int)TileMode.Tile, (int)brush4.TileMode);
            #endregion

            #region Test #5 - None
            // Usage: TileMode.None
            // Notes: Value = int32(0x00000005)
            CommonLib.LogStatus("Test #5 - None");

            // Set the TileMode of a DrawingBrush to None
            DrawingBrush brush5 = brush.Clone();
            brush5.TileMode = TileMode.None;

            // Fill a rectangle with the brush
            DC.DrawRectangle(brush5, pen, rect5);

            // Check the TileMode value to assure that it is None.
            _class_testresult &= _helper.CompareProp("None", (int)TileMode.None, (int)brush5.TileMode);
            #endregion
            #endregion End Of SECTION I

            CommonLib.LogTest("Result for:"+_objectType );
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
