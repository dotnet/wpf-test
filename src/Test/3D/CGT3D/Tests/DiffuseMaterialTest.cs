// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.ReferenceRender;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class DiffuseMaterialTest : VisualVerificationTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            parameters = new UnitTestObjects(v);

            if (parameters.Model is GeometryModel3D)
            {
                if (!(((GeometryModel3D)parameters.Model).Material is DiffuseMaterial))
                {
                    throw new ApplicationException("Material must be DiffuseMaterial for DiffuseMaterialTest to work");
                }
            }
            else
            {
                throw new ApplicationException("DiffuseMaterialTest requires a model that uses a Material to work");
            }
        }

        /// <summary/>
        public override void RunTheTest()
        {

            // render a simple 2D rectangle
            isControlRender = true;
            RenderWindowContent();
            controlCapture = GetScreenCapture();

            // render a full screen polygon with ambient light
            isControlRender = false;
            RenderWindowContent();
            // This calls VerifyCapturedContent below inside a context delegate
            VerifyWithinContext();
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            if (isControlRender)
            {
                Brush brush2D = ((DiffuseMaterial)((GeometryModel3D)parameters.Model).Material).Brush;
                Rectangle r = new Rectangle(new Rect(0, 0, WindowWidth, WindowHeight), brush2D);
                return r.Visual;
            }
            else
            {
                return parameters.Content;
            }
        }

        /// <summary/>
        public override void Verify()
        {
            Log("Verifying...");

            // We're using the fullscreen mesh.
            // It should all be the same color as a fullscreen Rectangle with the same brush

            // Get opaque (alpha = 255) light color
            Color solidLightColor = parameters.Light.Color;
            solidLightColor.A = 0xff;

            Color[,] screenCapture = GetScreenCapture();
            int width = screenCapture.GetLength(0);
            int height = screenCapture.GetLength(1);
            Color[,] expectedValue = new Color[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // blend pixel with light
                    Color litMeshColor = ColorOperations.Modulate(solidLightColor, controlCapture[x, y]);
                    expectedValue[x, y] = litMeshColor;
                }
            }

            // use renderbuffer for error difference estimation
            RenderBuffer renderBuffer = new RenderBuffer(expectedValue, BackgroundColor);

            RenderTolerance.IgnoreViewportBorders = true;
            renderBuffer.AddDefaultTolerances();

            renderBuffer.EnsureCorrectBitDepth();

            int differences = RenderVerifier.VerifyRender(screenCapture, renderBuffer, numAllowableMismatches, VScanToleranceFile);
            if (differences > 0)
            {
                AddFailure("{0} pixels did not meet the tolerance criteria.", differences);
            }
            if (Failures != 0)
            {
                RenderBuffer diff = RenderVerifier.ComputeDifference(screenCapture, renderBuffer);
                PhotoConverter.SaveImageAs(screenCapture, logPrefix + "_Rendered.png");
                PhotoConverter.SaveImageAs(renderBuffer.FrameBuffer, logPrefix + "_Expected_fb.png");
                PhotoConverter.SaveImageAs(renderBuffer.ToleranceBuffer, logPrefix + "_Expected_tb.png");
                PhotoConverter.SaveImageAs(diff.ToleranceBuffer, logPrefix + "_Diff_tb.png");
                PhotoConverter.SaveImageAs(diff.FrameBuffer, logPrefix + "_Diff_fb.png");
            }
        }

        /// <summary/>
        protected bool isControlRender;
        /// <summary/>
        protected Color[,] controlCapture;
        /// <summary/>
        protected UnitTestObjects parameters;
    }
}