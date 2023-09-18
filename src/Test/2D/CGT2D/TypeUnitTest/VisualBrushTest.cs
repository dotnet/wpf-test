// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.ReferenceRender;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Super-basic Tests for VisualBrush.
    /// This only supports the VisualBrushes that have
    /// no tiling, stretching, transforms, or special
    /// viewport and viewboxes set.
    /// </summary>
    public class VisualBrushTest : RenderingTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            ParseOptionalParameters(v);

            _brush = ResourceGenerator2D.MakeBrush(v) as VisualBrush;
            if (_brush == null)
            {
                throw new ArgumentException("A valid VisualBrush could not be created");
            }
            _firstPass = true;
        }

        private void ParseOptionalParameters(Variation v)
        {
            _forceSave = (v["ForceSave"] != null)
                            ? StringConverter.ToBool(v["ForceSave"])
                            : false;
            FactoryParser.MakeTolerance(v);
        }

        /// <summary/>
        public override void RunTheTest()
        {
            RenderWindowContent();
            _baselineCapture = GetScreenCapture();

            RenderWindowContent();
            VerifyWithinContext();
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            if (_firstPass)
            {
                _firstPass = false;
                Log("Rendering the Visual first (baseline)");
                return _brush.Visual;
            }
            else
            {
                Log("Rendering the Visual inside a VisualBrush");
                DrawingVisual v = new DrawingVisual();
                using (DrawingContext ctx = v.RenderOpen())
                {
                    ctx.DrawRectangle(_brush, null, new Rect(0, 0, WindowWidth, WindowHeight));
                }
                return v;
            }
        }

        /// <summary/>
        public override void Verify()
        {
            // Compare two screen captures

            RenderBuffer baseline = new RenderBuffer(_baselineCapture, BackgroundColor);
            baseline.AddDefaultTolerances();

            Color[,] screenCapture = GetScreenCapture();
            int differences = RenderVerifier.VerifyRender(screenCapture, baseline);

            // Log failures, if any
            if (differences > 0)
            {
                AddFailure("{0} pixels did not meet the tolerance criteria.", differences);
            }
            if (Failures != 0 || _forceSave)
            {
                RenderBuffer diff = RenderVerifier.ComputeDifference(screenCapture, baseline);
                PhotoConverter.SaveImageAs(screenCapture, logPrefix + "_Rendered.png");
                PhotoConverter.SaveImageAs(baseline.FrameBuffer, logPrefix + "_Expected_fb.png");
                PhotoConverter.SaveImageAs(baseline.ToleranceBuffer, logPrefix + "_Expected_tb.png", false);
                PhotoConverter.SaveImageAs(diff.ToleranceBuffer, logPrefix + "_Diff_tb.png");
            }
        }
    
        private VisualBrush _brush;
        private bool _firstPass;
        private Color[,] _baselineCapture;

        // Optional paramters
        private bool _forceSave;
    }
}
