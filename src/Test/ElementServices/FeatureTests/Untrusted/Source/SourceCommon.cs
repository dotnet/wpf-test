// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using Avalon.Test.CoreUI.Common;

using System.Runtime.InteropServices;


namespace Avalon.Test.CoreUI.Source
{

    /// <summary>
    /// Simple Hello UIElement to render something on the screen
    /// </summary>
    public class HelloElement : System.Windows.FrameworkElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HelloElement(){}

        /// <summary>
        /// Attribute that holds the Source where this element is rendered
        /// </summary>
        public HwndSource Source = null;

        /// <summary>
        /// Event where tells you that the control is already rendered.
        /// </summary>
        public event RenderHandler RenderedSourcedHandlerEvent;


        /// <summary>
        /// Delegate for RenderSourcedHandlerEvent
        /// </summary>
        public delegate void RenderHandler (UIElement target, HwndSource Source);

        /// <summary>
        /// Set the Font Color
        /// </summary>
        public SolidColorBrush FontColor = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff,0xff));
//        public SolidColorBrush FontColor = Brushes.White.

        /// <summary>
        /// Rendered method that it is called by the MIL
        /// </summary>
        /// <param name="ctx"></param>
        protected override void OnRender(DrawingContext ctx)
        {
            string greet = "Simple Test Case";

            FormattedText text = new FormattedText(greet, 
                Language.GetSpecificCulture(), 
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                55,
                FontColor);

            ctx.DrawText(text, new Point((RenderSize.Width / 2) - (text.Width / 2), 
                (RenderSize.Height / 2) - (text.Height / 2)));

            
            if(this.RenderedSourcedHandlerEvent != null)
            {
                this.RenderedSourcedHandlerEvent(this,Source);
            }
        }

        /// <summary>
        /// HitTestCore implements precise hit testing against render contents
        /// </summary>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParams)
        { 
            Rect r = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
            if (r.Contains(hitTestParams.HitPoint))
            {
                return new PointHitTestResult(this, hitTestParams.HitPoint);
            }

            return null;
        }


    }

}

