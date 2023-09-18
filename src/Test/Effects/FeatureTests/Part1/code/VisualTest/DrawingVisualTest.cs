// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test rendering of DrawingVisual. 
 * Owner: Microsoft 
 ********************************************************************/
using System.IO;
using Microsoft.Test.Configuration;
using Microsoft.Test.Logging;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Graphics;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Test Drawing Visual.
    /// </summary>`
   //commenting ignored cases
    // [Test(1, "DrawingVisual", "DrawingVisual", SupportFiles =
    //               @"common\InvariantTheme.xaml,FeatureTests\Effects\Masters\DrawingVisual*.png,FeatureTests\Effects\Model\testprofilebad.xml",
    //               Description = "Test applying Effect on DrawingVisual."
    //     )]
    public class DrawingVisualTest : EffectsWindowTest
    {
        private static readonly string s_toleranceFileName = "testprofilebad.xml";
        private readonly string _masterImageFileName;
        private EffectType EffectType { get; set; }
        public DrawMethod DrawMethod { get; set; }
        public double Opacity { get; set; }
        public TransformType TransformType { get; set; }
        public RenderingMode RenderingMode { get; set; }

        /// <summary />
        [Variation(EffectType.BlurEffect, DrawMethod.DrawRectangle, 1, TransformType.None, RenderingMode.Hardware, "DrawingVisual1.png")]
        [Variation(EffectType.DropShadowEffect, DrawMethod.DrawRectangle, 0.5, TransformType.RotateTransform, RenderingMode.Hardware, "DrawingVisual2.png")]
        [Variation(EffectType.DropShadowEffect, DrawMethod.DrawRectangle, 0.9, TransformType.RotateTransform, RenderingMode.Software, "DrawingVisual3.png")]
        [Variation(EffectType.None, DrawMethod.DrawRectangle, 0.75, TransformType.ScaleTransform, RenderingMode.Hardware, "DrawingVisual4.png")]
        public DrawingVisualTest(EffectType effectType, DrawMethod drawMethod, double opacity, TransformType transformType, RenderingMode renderingMode, string masterImage)
        {
            EffectType = effectType;
            DrawMethod = drawMethod;
            TransformType = transformType;
            Opacity = opacity;
            RenderingMode = renderingMode;
            _masterImageFileName = masterImage;

            RunSteps += new TestStep(RunTest);
        }


        /// <summary>
        /// RunTest Create the object tree, and verify with visual validation. 
        /// </summary>
        /// <returns></returns>
        TestResult RunTest()
        {
            FrameworkElement content = ContructObjectTree();

            ContentRoot.Content = content;
            RenderingModeHelper.SetRenderingMode(Window, RenderingMode);

            WaitFor(1000);

            if (!EffectsTestHelper.VerifyWindowAgainstMasterImage(Window, _masterImageFileName, s_toleranceFileName))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private FrameworkElement ContructObjectTree()
        {
            DrawingVisualPanel panel = new DrawingVisualPanel();
            panel.Height = 200;
            panel.Width = 200;

            CustomDrawingVisual drawingVisual = null;
            switch (DrawMethod)
            {
                case DrawMethod.DrawRectangle:
                    drawingVisual = new RectangleDrawingVisual(Brushes.Green, new Pen(Brushes.Red, 5), new System.Windows.Rect(50, 50, 50, 50));
                    break;

                default:
                    break;
            }
            drawingVisual.Effect = CreateEffect();
            drawingVisual.Opacity = Opacity;
            drawingVisual.Transform = CreateTransform();

            panel.AddVisual(drawingVisual);

            panel.InvalidateVisual();

            return panel;
        }
        private Transform CreateTransform()
        {
            Transform transform = null;
            switch (TransformType)
            {
                case TransformType.RotateTransform:
                    transform = new RotateTransform(45, 100, 100);
                    break;

                case TransformType.ScaleTransform:
                    transform = new ScaleTransform(0.7, 0.7);
                    break;

                case TransformType.None:
                    break;

                default:
                    throw new NotImplementedException(String.Format("Not implement transform type: {0}.", TransformType));
            }
            return transform;
        }
        private Effect CreateEffect()
        {
            Effect effect = null;
            switch (EffectType)
            {
                case EffectType.BlurEffect:
                    effect = new BlurEffect();
                    break;
                case EffectType.DropShadowEffect:
                    effect = new DropShadowEffect();
                    break;
                default:
                    break;
            }

            return effect;
        }
    }

    public enum EffectType
    {
        BlurEffect,
        DropShadowEffect,
        None
    }
    public enum DrawMethod
    {
        DrawRectangle,
        DrawDrawing,
        DrawEllipse,
        DrawGeometry,
        DrawGlyphRun,
        DrawImage,
        DrawLine,
        DrawRoundedRectangle,
        DrawText
    }

    public enum TransformType
    {
        ScaleTransform,
        RotateTransform,
        MatrixTransform,
        SkewTransform,
        TranslateTransform,
        None
    }
}