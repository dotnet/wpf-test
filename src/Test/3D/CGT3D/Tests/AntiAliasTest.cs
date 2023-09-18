// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Test Anti-Aliasing along silhouette edges
    /// </summary>
    public class AntiAliasTest : AntiAliasColorScanTest
    {
        /// <summary/>
        public override void Verify()
        {
            base.Verify();
            if (Failures > 0 || forceSave)
            {
                Log("");
                string filename = null;

                // Save the Tolerance stencil image
                filename = logPrefix + "_Expected_tb.png";
                PhotoConverter.SaveImageAs(renderBuffer.ToleranceBuffer, filename, false);
                LogImageSaved("AntiAlias test area", filename);

                // Save z-buffer image
                Color[,] zbuffer = PhotoConverter.ToColorArray(renderBuffer.ZBuffer);
                filename = logPrefix + "_Expected_zb.png";
                LogImageSaved("Expected Depth Buffer", filename);
            }
        }

        /// <summary/>
        protected override void GatherColorsFromCapture(Color[,] capture)
        {
            // Force tolerances so that only silhouette edge tolerance renders
            RenderTolerance.PixelToEdgeTolerance = 0;
            RenderTolerance.SilhouetteEdgeTolerance = 1;
            RenderTolerance.TextureLookUpTolerance = 0;

            // Render scene with TR
            SceneRenderer sceneRenderer = parameters.SceneRenderer;
            renderBuffer = sceneRenderer.Render(InterpolationMode.Gouraud);
            Color edgeIgnore = Color.FromArgb(0x00, 0xff, 0xff, 0xff);

            // Now scan capture at points where the edge tolerance is white,
            // effectively using ToleranceBuffer as a stencil buffer.
            int width = capture.GetLength(0);
            int height = capture.GetLength(1);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Only gather color info at the edges
                    if (renderBuffer.ToleranceBuffer[x, y] == edgeIgnore)
                    {
                        Color c = capture[x, y];
                        if (colors.ContainsKey(c))
                        {
                            // color is already in the list, increment count
                            colors[c]++;
                        }
                        else
                        {
                            // new color, add it
                            colors.Add(c, 1);
                        }
                    }
                }
            }
        }
    }
}

