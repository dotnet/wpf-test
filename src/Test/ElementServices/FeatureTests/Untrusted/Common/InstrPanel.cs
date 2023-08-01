// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;

namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// This class represents an instrumented panel control (InstrPanel).
    /// An InstrPanel is a UIElement that contains other UIElements or ContentElements.
    /// It also contains debug-mode instrumentation showing what events are passing throught it.
    /// </summary>
    /// <example>
    /// This example fragment shows how to initialize a window source with an InstrPanel as the root element.
    /// <code>
    /// Dispatcher context = new Dispatcher().
    /// 
    /// using(context.Access()) {
    ///   HwndSource source = CreateStandardSource(10, 10, 100, 100).
    ///   UIElement rootElement = new InstrPanel().
    ///   Visual v = rootElement.
    ///   source.RootVisual = v.
    /// }
    /// </code>
    /// </example>
    public class InstrPanel : UIElement
    {
        /// <summary>
        /// Construct an instrumented panel.
        /// </summary>
        public InstrPanel()
            : base()
        {
                _children = new VisualCollection(this);         
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {            
            if(_children == null)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }
            if(index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException("index is out of range");
            }

            return _children[index];
        }

        /// <summary>
        /// Returns the Visual children count.
        /// </summary>            
        protected override int VisualChildrenCount
        {           
            get 
            { 
                if(_children == null)
                {
                    throw new ArgumentOutOfRangeException("_children is null");
                }                
                return _children.Count; 
            }
        } 
            
        /// <summary>
        /// Override to render an instrumented element.
        /// </summary>
        /// <param name="ctx">Drawing context.</param>
        protected override void OnRender(DrawingContext ctx)
        {
            FormattedText text = new FormattedText("IP",
                new CultureInfo("en-US"),
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                8,
                this.TextBrush);
            ctx.DrawText(text, new Point(20, 20));

            // Call event handler
            if (this.Rendered != null)
            {
                this.Rendered(this, ctx);
            }

        }

        /// <summary>
        /// What brush is used to draw the text?
        /// </summary>
        public Brush TextBrush
        {
            get
            {
                return _textBrush;
            }
            set
            {
                _textBrush = value;
            }
        }

        private Brush _textBrush = Brushes.White;
        private VisualCollection _children; 	
        
        /// <summary>
        /// Fires when the control has completely rendered.
        /// </summary>
        public event RenderEventHandler Rendered;

        /// <summary>
        /// Append visual child to this element.
        /// </summary>
        /// <param name="child">Visual object.</param>
        public void AppendChild(Visual child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Remove visual child from this element.
        /// </summary>
        /// <param name="child">Visual object.</param>
        public void RemoveChild(Visual child)
        {
            _children.Remove(child);
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

        #region Input overrides

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnKeyDown(KeyEventArgs args)
        {

            // Log some debugging data
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");
            //Point pt = args.GetPosition(null).
            Debug.WriteLine("   Hello from InstrPanel: " + args.Key.ToString() + "," + args.KeyStates.ToString());

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
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");
            //Point pt = args.GetPosition(null).
            Debug.WriteLine("   Hello from InstrPanel: " + args.Key.ToString() + "," + args.KeyStates.ToString());

            // Keep routing this event.
            args.Handled = false;
        }

        /// <summary>
        /// Standard mouse-move event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnMouseMove(MouseEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");

            Point pt = args.GetPosition(null);

            Debug.WriteLine("   Hello from InstrPanel: " + pt.X + "," + pt.Y);

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

            Debug.WriteLine("   Hello from InstrPanel: Cursor='" + args.Cursor.ToString() + "'");

            // Keep routing this event.
            args.Handled = false;
        }

        #endregion
    }

    /// <summary>
    /// Delegate for Rendered event.
    /// </summary>
    public delegate void RenderEventHandler(UIElement target, DrawingContext ctx);
}

