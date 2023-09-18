// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Test.Graphics.CachedComposition
{
    class Factory
    {

        #region Public methods

        /// <summary>
        /// Create a ChangeableContent that matches the given requirements.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ChangeableContent CreateContent(Requirements req)
        {
            ChangeableContent content;
            //first use the requirements struct to choose which type of content we make
            switch (req.context)
            {
                case Context.LayeredWindows:
                    content = new LayeredWindowsContent();
                    break;
                case Context.MediaElement:
                    content = new MediaElementContent();
                    break;
                case Context.MultipleMonitor:
                    content = new MultipleMonitorContent();
                    break;
                case Context.OpacityClippedOutput:
                    content = new OpacityClippedOutputContent();
                    break;
                case Context.RenderTargetBitmap:
                    content = new RenderTargetBitmapContent();
                    break;
                case Context.TransformedBrush:
                    content = new TransformedBrushContent();
                    break;
                case Context.AnimatedTransformedBrush:
                    content = new AnimatedTransformedBrushContent();
                    break;
                case Context.VP2DV3D:
                    content = new VP2DV3DContent();
                    break;
                case Context.TileBrush:
                    content = new TileBrushContent();
                    break;
                default:
                    throw new ArgumentException("Invalid content choice specified");
            }

            return content;

		}
        /// <summary>
        /// Create a ChangeDetector that matchs the given requirements.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public ChangeDetector CreateDetector(Requirements req)
        {
            ChangeDetector detector;
            switch (req.detectorType)
            {
                case Detector.ETW:
                    detector = new ETWDetector(); 
                    break;             
                case Detector.Visual:
                    detector = new VisualChangeDetector();
                    break;
                default:
                    throw new ArgumentException("Invalid detector choice specified");
            }
            return detector;
        }

        /// <summary>
        /// Create a ChangeableContent and a ChangeDetector that match the given requirements.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public Changer CreateChanger(Requirements req)
        {
            Changer changer;

            switch (req.invalidatingProperty)
            {
                case InvalidatingProperty.Animation:
                    changer = new AnimationChanger();
                    break;
                case InvalidatingProperty.DataBinding:
                    changer = new DataBindingChanger();
                    break;
                case InvalidatingProperty.EnableClearType:
                    changer = new EnableClearTypeChanger();
                    break;
                case InvalidatingProperty.Layout:
                    changer = new LayoutChanger();
                    break;
                case InvalidatingProperty.RenderScale:
                    changer = new RenderScaleChanger();
                    break;
                case InvalidatingProperty.Theme:
                    changer = new ThemeChanger();
                    break;
                case InvalidatingProperty.Code:
                    changer = new CodeChanger();
                    break;
                default:
                    throw new ArgumentException("Invalid invalidating property choice specified");
            }
            return changer;
        }

        #endregion
    }


}
