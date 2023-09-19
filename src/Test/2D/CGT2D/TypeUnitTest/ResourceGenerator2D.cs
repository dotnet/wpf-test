// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;


namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Parses Variations and creates resources
    /// </summary>
    public class ResourceGenerator2D
    {
        /// <summary/>
        public static Brush MakeBrush(Variation v)
        {
            v.AssertExistenceOf("BrushType");

            switch (v["BrushType"])
            {
                // This is a special case for basic BVTs on VisualBrush until GTO is ready to test them.
                // We only allow this configuration of the TileBrush properties because the VisualBrushTest
                //  isn't programmed to understand anything more.
                case "VisualOnly":
                    v.AssertExistenceOf("BrushVisual");

                    VisualBrush brush = new VisualBrush(VisualFactory.MakeVisual(v["BrushVisual"]));
                    brush.Stretch = Stretch.None;
                    brush.TileMode = TileMode.None;
                    brush.Viewport = new Rect(0, 0, 1, 1);
                    brush.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;

                    brush.Viewbox = new Rect(0, 0, v.WindowWidth, v.WindowHeight);
                    brush.ViewboxUnits = BrushMappingMode.Absolute;
                    return brush;

                default:
                    throw new ArgumentException("Don't know how to make brush of type: " + v["BrushType"]);
            }
        }
    }
}
