// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace Avalon.Test.CoreUI.CoreInput.Common.Controls
{
    /// <summary>
    /// This class represents an instrumented panel framework element (InstrFrameworkPanel).
    /// An InstrFrameworkPanel is a FrameworkElement that contains other FrameworkElements or FrameworkContentElements.
    /// It also contains debug-mode instrumentation showing what events are passing throught it.
    /// </summary>
    /// <example>
    /// This example fragment shows how to initialize a window source with an InstrFrameworkPanel as the root element.
    /// <code>
    ///        Window _win = new Window().
    ///        _win.Content = new InstrFrameworkPanel().
    /// </code>
    /// </example>
    public class InstrFrameworkPanel: FrameworkElement
    {
        /// <summary>
        /// Construct an instrumented panel.
        /// </summary>
        public InstrFrameworkPanel() : base()
        {
            _children = new VisualCollection(this);         
            Width = 40;
            Height = 40;
        }

        /// <summary>
        /// Get the Children Collection
        /// </summary>    
        public VisualCollection Children
        {
            get
            {
                return _children;
            }
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

        private VisualCollection _children; 	

        /// <summary>
        /// Override to render an instrumented element.
        /// </summary>
        /// <param name="ctx">Drawing context.</param>
        protected override void OnRender(DrawingContext ctx)
        {
            // Render a cross.
            ctx.DrawLine(
                new Pen(new SolidColorBrush(this.Color), 2.0f),
                new Point(0, 0),
                new Point(Width, Height));
            ctx.DrawLine(
                new Pen(new SolidColorBrush(this.Color), 2.0f),
                new Point(0, Height),
                new Point(Width, 0));
        }

        /// <summary>
        /// This element's color.
        /// </summary>
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        private Color _color = Color.FromRgb(0x00, 0xff, 0x00);

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
        /// Standard key event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnKeyDown(KeyEventArgs args)
        {

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            //Point pt = args.GetPosition(null).
            Debug.WriteLine ("   Hello from InstrFrameworkPanel: " + args.Key.ToString()+","+args.KeyStates.ToString());

            // Keep routing this event.
            args.Handled = false;

            base.OnKeyDown(args);
        }


        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnKeyUp(KeyEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Debug.WriteLine ("   Hello from InstrFrameworkPanel: " + args.Key.ToString()+","+args.KeyStates.ToString());

            // Keep routing this event.
            args.Handled = false;

            base.OnKeyUp(args);
        }

        /// <summary>
        /// Standard mouse move handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnMouseMove(MouseEventArgs args)
        {
            Point position = args.GetPosition(this);

            // Log some debugging data
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrFrameworkPanel: " + position.X + "," + position.Y);

            base.OnMouseMove(args);
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Debug.WriteLine ("   Hello from InstrFrameworkPanel: old focus='" + args.OldFocus+"',new focus='"+args.NewFocus+"'");

            // Keep routing this event.
            args.Handled = false;

            base.OnGotKeyboardFocus(args);
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Debug.WriteLine ("   Hello from InstrFrameworkPanel: old focus='" + args.OldFocus+"',new focus='"+args.NewFocus+"'");

            // Keep routing this event.
            args.Handled = false;

            base.OnLostKeyboardFocus(args);
        }

        /// <summary>
        /// Standard left mouse button down event handler. 
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrFrameworkPanel: " + args.ButtonState.ToString() + "," + args.ChangedButton.ToString() + "," + args.ClickCount);

            // Request focus for ourselves.
            this.Focus();

            // Keep routing this event.
            args.Handled = false;

            base.OnMouseLeftButtonDown(args);
        }

        /// <summary>
        /// Standard left mouse button up event handler. 
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrFrameworkPanel: " + args.ButtonState.ToString() + "," + args.ChangedButton.ToString() + "," + args.ClickCount);

            // Request focus for ourselves.
            this.Focus();

            // Keep routing this event.
            args.Handled = false;

            base.OnMouseLeftButtonUp(args);
        }

        /// <summary>
        /// Standard MouseWheel handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnMouseWheel(MouseWheelEventArgs args)
        {
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrFrameworkPanel: Delta=" + args.Delta.ToString());

            // Keep routing this event.
            args.Handled = false;

            base.OnMouseWheel(args);
        }

        /// <summary>
        /// Standard query cursor event handler.
        /// </summary>
        /// <param name="args">Event-specific arguments.</param>
        protected override void OnQueryCursor(QueryCursorEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine(" [" + args.RoutedEvent.Name + "]");
            Debug.WriteLine("   Hello from InstrFrameworkPanel: Cursor='" + ((args.Cursor != null) ? args.Cursor.ToString() : "") + "'");

            // Keep routing this event.
            args.Handled = false;

            base.OnQueryCursor(args);
        }
    }
}

