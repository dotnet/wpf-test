// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.ReferenceRender;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Render and verify bounds of complex Visual3D trees
    /// </summary>
    public class RenderBoundsTest : Visual3DRenderingTest
    {
        /// <summary/>
        public override void Verify()
        {
            SceneRenderer renderer = parameters.SceneRenderer;

            Log("Invoking SceneRenderer...");
            renderBuffer = renderer.RenderWithoutBackground();

            TestContentBounds();
            TestDescendantBounds();

            VisualTreeHelper.GetDescendantBounds(parameters.Content);
        }

        private void TestContentBounds()
        {
            Log("Testing VisualTreeHelper.GetContentBounds...");

            Visual v = (variation.UseViewport3D) ? (Visual)parameters.Viewport : (Visual)parameters.Visual;
            Rect theirAnswer = VisualTreeHelper.GetContentBounds(v);
            Rect myAnswer = Rect.Empty;

            Check(theirAnswer, myAnswer);

            if (!variation.UseViewport3D)
            {
                Log("Testing Viewport3DVisual.ContentBounds...");

                // These two values should be the same (regardless if the above test failed)
                Rect localContentBounds = parameters.Visual.ContentBounds;
                Check(theirAnswer, localContentBounds);
            }
        }

        private void TestDescendantBounds()
        {
            Log("Testing VisualTreeHelper.GetDescendantBounds...");

            Visual v = (variation.UseViewport3D) ? (Visual)parameters.Viewport : (Visual)parameters.Visual;
            Rect theirAnswer = VisualTreeHelper.GetDescendantBounds(v);
            Rect myAnswer = MathEx.ConvertToDeviceIndependentPixels(renderBuffer.TightRenderedBounds);

            Check(theirAnswer, myAnswer);

            if (!variation.UseViewport3D)
            {
                Log("Testing Viewport3DVisual.DescendantBounds...");

                // These two values should be the same (regardless if the above test failed)
                Rect localContentBounds = parameters.Visual.DescendantBounds;
                Check(theirAnswer, localContentBounds);
            }
        }

        private void Check(Rect theirAnswer, Rect myAnswer)
        {
            Log("Their Bounds = {0}", theirAnswer);
            Log("   My Bounds = {0}", myAnswer);

            if (!MathEx.ContainsCloseEnough(theirAnswer, myAnswer))
            {
                AddFailure("Their bounds should completely contain mine");
            }
        }
    }
}