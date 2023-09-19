// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// 3D Hit Testing using VisualTreeHelper.HitTest (i.e. from a 2D Point)
    /// Hit test against the whole client area.  Verification is quick because
    /// this is a lot of work.
    /// </summary>
    public class ClientAreaHitTestingTest : PointHitTestingTestBase
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            v.AssertExistenceOf("Tolerance");
            _tolerance = StringConverter.ToByte(v["Tolerance"]);
        }

        /// <summary/>
        protected override void VerifyHitTesting()
        {
            SceneRenderer renderer = parameters.SceneRenderer;
            RenderBuffer buffer = renderer.Render();
            Color[,] toleranceBuffer = buffer.ToleranceBuffer;
            float[,] zBuffer = buffer.ZBuffer;
            float clearValue = buffer.ZBufferClearValue;

            // *NOTE: We are iterating on the # of pixels in the window (Device Dependent)

            Rect ddWindowBounds = new Rect(ddBounds.WindowSize);
            int left = (int)ddWindowBounds.X;
            int top = (int)ddWindowBounds.Y;
            int right = left + (int)ddWindowBounds.Width;
            int bottom = top + (int)ddWindowBounds.Height;

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color pixelTolerance = toleranceBuffer[x, y];
                    if (pixelTolerance.A > _tolerance || pixelTolerance.G > _tolerance || pixelTolerance.B > _tolerance)
                    {
                        // If we aren't sure the pixel should be drawn, we shouldn't hit test it
                        continue;
                    }
                    ddPoint = new Point(x + Const.pixelCenterX, y + Const.pixelCenterY);
                    diPoint = MathEx.ConvertToDeviceIndependentPixels(ddPoint);
                    theirAnswers.Clear();
                    HitTestAllModels();

                    if (zBuffer[x, y] == clearValue)
                    {
                        if (theirAnswers.Count != 0)
                        {
                            AddFailure("I did not expect to hit anything at location ({0})", diPoint);
                            Log("Avalon says I hit {0} objects", theirAnswers.Count);
                        }
                    }
                    else
                    {
                        if (theirAnswers.Count == 0)
                        {
                            AddFailure("I expected to hit something at location ({0})", diPoint);
                            Log("Avalon says I missed");
                        }
                    }
                }
            }
            if (Failures > 0)
            {
                Color[,] screenCapture = GetScreenCapture();
                PhotoConverter.SaveImageAs(screenCapture, variation.TestClass + "_Rendered.png");
                Log("Rendered Image: " + variation.TestClass + "_Rendered.png");
            }

            int area = DpiScaledWindowWidth * DpiScaledWindowHeight;
            Log("Results: {0} / {1} pixels correct", area - Failures, area);
        }

        private byte _tolerance;
    }
}