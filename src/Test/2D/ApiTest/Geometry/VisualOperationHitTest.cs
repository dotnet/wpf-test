// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Program:  VisualOperation HitTest geometry tests
//  Author:   Microsoft
//
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class VisualOperationHitTest : HitTestBase
    {
        public VisualOperationHitTest( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext ctx)
        {
            RunBVT();

            CommonLib.LogStatus("In conclusion:");
            CommonLib.LogTest("Outcome for VisualOperation HitTest on Clipped Geometry");
        }

        private void RunBVT()
        {
            Geometry clippedRegion1 = new RectangleGeometry(new Rect(200, 200, 40, 40));
            Geometry testGeometry1 = new RectangleGeometry(new Rect(100, 100, 200, 200));

            CommonLib.LogStatus("Test #1: PointHitTest on Clipped Geometry");
            CommonLib.LogStatus("the hit point is in the testGeometry1 but out of clippedregion, so the result should be false");
            TestVisualTreeHelperPointHitTestOnClippedGeometry(clippedRegion1, testGeometry1, new Point(105, 105), false);

            CommonLib.LogStatus("Test #2: PointHitTest on Clipped Geometry");
            CommonLib.LogStatus("the hit point is in the clipped region, so the result is true");
            TestVisualTreeHelperPointHitTestOnClippedGeometry(clippedRegion1, testGeometry1, new Point(205, 205), true);

            CommonLib.LogStatus("Test #3: PointHitTest on Clipped Geometry");
            CommonLib.LogStatus("the hit point is way out of the testgeometry1, so the result is false");
            TestVisualTreeHelperPointHitTestOnClippedGeometry(clippedRegion1, testGeometry1, new Point(-199, 1000), false);
        }
    }
}
