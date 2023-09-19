// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// 3D Hit Testing using VisualTreeHelper.HitTest (i.e. from a 2D Point)
    /// </summary>
    public class PointHitTestingTest : PointHitTestingTestBase
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            v.AssertExistenceOf("Point");
            diPoint = StringConverter.ToPoint(v["Point"]);
            diPoint.X = diPoint.X + Const.pixelCenterX;
            diPoint.Y = diPoint.Y + Const.pixelCenterY;
            ddPoint = MathEx.ConvertToAbsolutePixels(diPoint);
        }

        /// <summary/>
        protected override void VerifyHitTesting()
        {
            // Set theirAnswer with Avalon's hit testing results
            HitTestAllModels();

            // Set myAnswer with my hit testing results
            Ray ray = PointToRay();
            DoRayHitTesting(ray);

            CompareHitTestResults();
        }
    }
}