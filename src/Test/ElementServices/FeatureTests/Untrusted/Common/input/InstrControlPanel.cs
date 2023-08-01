// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;


namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// This class represents an instrumented panel based on a framework Control (InstrControlPanel).
    /// An InstrControlPanel is a UIElement that contains other UIElements or ContentElements.
    /// It also contains debug-mode instrumentation showing what events are passing throught it.
    /// </summary>
    /// <example>
    /// This example fragment shows how to initialize a window source with an InstrPanel as the root element.
    /// <code>
    /// Dispatcher context = new Dispatcher().
    /// 
    /// using(context.Access()) {
    ///   HwndSource source = CreateStandardSource(10, 10, 100, 100).
    ///   UIElement rootElement = new InstrControlPanel().
    ///   Visual v = rootElement.
    ///   source.RootVisual = v.
    /// }
    /// </code>
    /// </example>
    public class InstrControlPanel: Control
    {
        /// <summary>
        /// Construct an instrumented panel.
        /// </summary>
        public InstrControlPanel() : base()
        {
        }


        /// <summary>
        /// Override to render an instrumented element.
        /// </summary>
        /// <param name="ctx">Drawing context.</param>
        protected override void OnRender(DrawingContext ctx)
        {
            FormattedText text = new FormattedText("IP", 
                Language.GetSpecificCulture(), 
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                8,
                Brushes.White);
            ctx.DrawText(text, new Point(20, 20));
            
            // Call event handler
            if(this.Rendered != null)
            {
                this.Rendered(this,ctx);
            }
            
        }

        /// <summary>
        /// Determine hit test result for this control.
        /// </summary>
        /// <param name="hitTestParams">Hit test argument package.</param>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParams)
        {
            // We have a hit test if user clicks anywhere inside control boundaries.
            Rect r = new Rect(0, 0, RenderSize.Width, RenderSize.Height);
        
            if (r.Contains(hitTestParams.HitPoint))
            {
                return new PointHitTestResult(this, hitTestParams.HitPoint);
            }

            return null;
        }
        
        /// <summary>
        /// Fires when the control has completely rendered.
        /// </summary>
        public event RenderEventHandler Rendered;

        /// <summary>
        /// Standard mouse-move event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnMouseMove(MouseEventArgs args)
        {

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            Debug.WriteLine ("   Hello from InstrControlPanel: " + pt.X+","+pt.Y);

            // Keep routing this event.
            args.Handled = false;
        }
        
        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnKeyDown(KeyEventArgs args)
        {

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            //Point pt = args.GetPosition(null).
            Debug.WriteLine ("   Hello from InstrControlPanel: " + args.Key.ToString()+","+args.KeyStates.ToString());

            // Keep routing this event.
            args.Handled = false;
        }


        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnKeyUp(KeyEventArgs args)
        {

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            //Point pt = args.GetPosition(null).
            Debug.WriteLine ("   Hello from InstrControlPanel: " + args.Key.ToString()+","+args.KeyStates.ToString());

            // Keep routing this event.
            args.Handled = false;
        }

        /// <summary>
        /// Standard query cursor event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnQueryCursor(QueryCursorEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrControlPanel: Cursor='" + ((args.Cursor != null) ? args.Cursor.ToString() : "") + "'");

            // Keep routing this event.
            args.Handled = false;
        }
    }
}
