// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2004
 *
 *   Description:   Render a 2D panel using RenderTargetBitmap
 *   Author:       Microsoft
 *
 ************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Threading;
using System.IO;
using Microsoft.Test.Graphics;
using FileMode = System.IO.FileMode;

using TrustedFileStream = System.IO.FileStream;
using TrustedStreamWriter = System.IO.StreamWriter;
using Microsoft.Test.Graphics.ReferenceRender;


namespace Microsoft.Test.Graphics
{
    public sealed class SceneRenderer2D
    {
        public SceneRenderer2D(Size windowSize, Panel panel, Color background)
        {
            _rootPanel = panel;
            _rootWindowSize = windowSize;
            _backgroundColor = background;
        }

        public RenderBuffer Render()
        {
            _rootPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            _rootPanel.Arrange(new Rect(_rootPanel.DesiredSize));
            _rootPanel.UpdateLayout();
            RenderTargetBitmap render = new RenderTargetBitmap((int)_rootWindowSize.Width, (int)_rootWindowSize.Height, Microsoft.Test.Display.Monitor.Dpi.x, Microsoft.Test.Display.Monitor.Dpi.y, PixelFormats.Default);
            render.Render(_rootPanel);
            Color[,] image = ColorOperations.ToColorArray(render);

            RenderBuffer buffer = new RenderBuffer(image, _backgroundColor);
            buffer.ClearToleranceBuffer(RenderTolerance.DefaultColorTolerance);
            return buffer;
        }

        private Panel _rootPanel;
        private Size _rootWindowSize;
        private Color _backgroundColor;
    }
}